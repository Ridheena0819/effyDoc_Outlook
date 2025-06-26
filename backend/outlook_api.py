from fastapi import APIRouter, Depends, HTTPException, WebSocket, WebSocketDisconnect, Query
from fastapi.responses import JSONResponse
from typing import List, Optional, Dict, Any
from datetime import datetime, timedelta
import json
import logging

from models import *
from database import get_collection
from auth import get_current_active_user
from websocket_manager import websocket_manager, notify_document_activity

logger = logging.getLogger(__name__)
router = APIRouter(prefix="/api/outlook", tags=["outlook-addin"])

# ==================== WEBSOCKET ENDPOINT ====================

@router.websocket("/ws")
async def websocket_endpoint(
    websocket: WebSocket,
    user_email: str = Query(...),
    token: str = Query(...)
):
    """WebSocket endpoint for real-time tracking updates"""
    try:
        # TODO: Validate token here for security
        # For now, accepting connection with user_email
        
        await websocket_manager.connect(websocket, user_email)
        
        try:
            while True:
                # Receive messages from client
                data = await websocket.receive_text()
                message = json.loads(data)
                
                # Handle different message types
                if message.get("type") == "subscribe_document":
                    document_id = message.get("document_id")
                    if document_id:
                        await websocket_manager.subscribe_to_document(user_email, document_id)
                        await websocket.send_text(json.dumps({
                            "type": "subscription_confirmed",
                            "document_id": document_id
                        }))
                
                elif message.get("type") == "unsubscribe_document":
                    document_id = message.get("document_id")
                    if document_id:
                        await websocket_manager.unsubscribe_from_document(user_email, document_id)
                
                elif message.get("type") == "heartbeat":
                    await websocket.send_text(json.dumps({
                        "type": "heartbeat_response",
                        "timestamp": datetime.utcnow().isoformat()
                    }))
                    
        except WebSocketDisconnect:
            pass
            
    except Exception as e:
        logger.error(f"WebSocket error for {user_email}: {e}")
    finally:
        await websocket_manager.disconnect(websocket, user_email)

# ==================== DOCUMENT LIBRARY ENDPOINTS ====================

@router.get("/documents/my-library")
async def get_my_library(current_user: User = Depends(get_current_active_user)):
    """Get user's personal document library for Outlook add-in"""
    documents_collection = await get_collection('documents')
    
    # Get user's documents
    query = {
        "owner_id": current_user.id,
        "status": {"$ne": "archived"}
    }
    
    documents = await documents_collection.find(query).to_list(1000)
    
    # Format for Outlook add-in
    library_documents = []
    for doc in documents:
        # Calculate basic tracking stats
        tracking_events = doc.get('tracking_events', [])
        
        library_documents.append({
            "id": doc["id"],
            "title": doc["title"],
            "type": doc["type"],
            "created_at": doc["created_at"].isoformat(),
            "updated_at": doc["updated_at"].isoformat(),
            "total_pages": doc.get("total_pages", len(doc.get("pages", []))),
            "file_size": doc.get("metadata", {}).get("file_size", 0),
            "tracking_stats": {
                "total_views": len([e for e in tracking_events if e.get("action") == "VIEW"]),
                "total_shares": len([e for e in tracking_events if e.get("action") == "CREATE"]),
                "last_viewed": max([e.get("timestamp") for e in tracking_events if e.get("action") == "VIEW"], default=None)
            },
            "share_link": f"/view/{doc['id']}",
            "is_trackable": True
        })
    
    return {
        "documents": library_documents,
        "total_count": len(library_documents),
        "user_email": current_user.email
    }

@router.get("/documents/content-hub")
async def get_content_hub(current_user: User = Depends(get_current_active_user)):
    """Get admin-shared documents (Content Hub) for Outlook add-in"""
    documents_collection = await get_collection('documents')
    
    # Get admin-shared documents for user's organization
    query = {
        "organization": current_user.organization,
        "metadata.is_admin_shared": True,
        "status": {"$in": ["approved", "sent"]}
    }
    
    documents = await documents_collection.find(query).to_list(1000)
    
    # Format for Outlook add-in
    hub_documents = []
    for doc in documents:
        hub_documents.append({
            "id": doc["id"],
            "title": doc["title"],
            "type": doc["type"],
            "created_at": doc["created_at"].isoformat(),
            "owner_name": doc.get("metadata", {}).get("owner_name", "Admin"),
            "total_pages": doc.get("total_pages", len(doc.get("pages", []))),
            "description": doc.get("metadata", {}).get("description", ""),
            "tags": doc.get("tags", []),
            "share_link": f"/view/{doc['id']}",
            "is_template": doc.get("metadata", {}).get("is_template", False),
            "is_trackable": True
        })
    
    return {
        "documents": hub_documents,
        "total_count": len(hub_documents),
        "organization": current_user.organization
    }

# ==================== TRACKING ENDPOINTS ====================

@router.post("/tracking/email-sent")
async def track_email_sent(
    tracking_data: Dict[str, Any],
    current_user: User = Depends(get_current_active_user)
):
    """Track when a document is sent via email from Outlook"""
    try:
        document_id = tracking_data["document_id"]
        recipients = tracking_data["recipients"]
        subject = tracking_data.get("subject", "")
        
        # Store email tracking record
        outlook_tracking_collection = await get_collection('outlook_email_tracking')
        
        tracking_record = OutlookEmailTracking(
            document_id=document_id,
            sender_email=current_user.email,
            recipient_emails=recipients,
            subject=subject,
            email_body=tracking_data.get("email_body", ""),
            tracking_link=f"/view/{document_id}?source=outlook&sender={current_user.email}"
        )
        
        await outlook_tracking_collection.insert_one(tracking_record.dict())
        
        # Send real-time notification
        await notify_document_activity(
            document_id=document_id,
            event_type="email_sent",
            data={
                "user_email": current_user.email,
                "recipient_email": ",".join(recipients),
                "metadata": {
                    "subject": subject,
                    "recipient_count": len(recipients),
                    "source": "outlook_addin"
                }
            }
        )
        
        return {
            "message": "Email tracking started",
            "tracking_id": tracking_record.id,
            "tracking_link": tracking_record.tracking_link
        }
        
    except Exception as e:
        logger.error(f"Error tracking email sent: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.post("/tracking/event")
async def track_document_event(
    event_data: Dict[str, Any]
):
    """Track document interaction events (opened, clicked, page viewed, etc.)"""
    try:
        event = OutlookTrackingEvent(
            event_type=event_data["event_type"],
            document_id=event_data["document_id"],
            user_email=event_data.get("user_email"),
            recipient_email=event_data.get("recipient_email"),
            page_number=event_data.get("page_number"),
            duration=event_data.get("duration"),
            user_agent=event_data.get("user_agent"),
            ip_address=event_data.get("ip_address"),
            session_id=event_data.get("session_id"),
            metadata=event_data.get("metadata", {})
        )
        
        # Store event
        tracking_events_collection = await get_collection('outlook_tracking_events')
        await tracking_events_collection.insert_one(event.dict())
        
        # Update email tracking record if exists
        outlook_tracking_collection = await get_collection('outlook_email_tracking')
        
        if event.event_type == "email_opened":
            await outlook_tracking_collection.update_many(
                {"document_id": event.document_id, "recipient_emails": {"$in": [event.recipient_email]}},
                {
                    "$set": {"opened_at": event.timestamp},
                    "$inc": {"total_opens": 1},
                    "$push": {"tracking_events": event.dict()}
                }
            )
            
        elif event.event_type == "link_clicked":
            await outlook_tracking_collection.update_many(
                {"document_id": event.document_id, "recipient_emails": {"$in": [event.recipient_email]}},
                {
                    "$set": {"clicked_at": event.timestamp},
                    "$inc": {"total_clicks": 1},
                    "$push": {"tracking_events": event.dict()}
                }
            )
        
        # Send real-time notification to document owner
        await websocket_manager.send_tracking_update(event.document_id, event)
        
        return {"message": "Event tracked successfully", "event_id": event.id}
        
    except Exception as e:
        logger.error(f"Error tracking event: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.get("/tracking/live-metrics/{document_id}")
async def get_live_tracking_metrics(
    document_id: str,
    current_user: User = Depends(get_current_active_user)
):
    """Get real-time tracking metrics for a document"""
    try:
        # Check document access
        documents_collection = await get_collection('documents')
        document = await documents_collection.find_one({"id": document_id})
        
        if not document:
            raise HTTPException(status_code=404, detail="Document not found")
        
        # Check if user has access (owner or collaborator)
        if document["owner_id"] != current_user.id:
            # Check if user is a collaborator
            collaborator_ids = [c.get("user_id") for c in document.get("collaborators", [])]
            if current_user.id not in collaborator_ids:
                raise HTTPException(status_code=403, detail="Access denied")
        
        # Get recent tracking events (last 24 hours)
        tracking_events_collection = await get_collection('outlook_tracking_events')
        twenty_four_hours_ago = datetime.utcnow() - timedelta(hours=24)
        
        recent_events = await tracking_events_collection.find({
            "document_id": document_id,
            "timestamp": {"$gte": twenty_four_hours_ago}
        }).sort("timestamp", -1).to_list(100)
        
        # Calculate current readers (active in last 5 minutes)
        five_minutes_ago = datetime.utcnow() - timedelta(minutes=5)
        current_readers = []
        
        for event in recent_events:
            if (event["event_type"] == "currently_reading" and 
                event["timestamp"] >= five_minutes_ago):
                
                reader_info = {
                    "email": event.get("recipient_email", "Anonymous"),
                    "page": event.get("page_number", 1),
                    "since": event["timestamp"].isoformat(),
                    "duration": event.get("duration", 0)
                }
                
                # Avoid duplicates
                if not any(r["email"] == reader_info["email"] for r in current_readers):
                    current_readers.append(reader_info)
        
        # Calculate today's stats
        today_start = datetime.utcnow().replace(hour=0, minute=0, second=0, microsecond=0)
        today_events = [e for e in recent_events if e["timestamp"] >= today_start]
        
        today_stats = {
            "emails_opened": len([e for e in today_events if e["event_type"] == "email_opened"]),
            "links_clicked": len([e for e in today_events if e["event_type"] == "link_clicked"]),
            "page_views": len([e for e in today_events if e["event_type"] == "page_viewed"]),
            "unique_viewers": len(set([e.get("recipient_email") for e in today_events if e.get("recipient_email")]))
        }
        
        # Convert events for response
        formatted_events = []
        for event in recent_events[:10]:  # Last 10 events
            formatted_events.append(OutlookTrackingEvent(**event))
        
        metrics = LiveTrackingMetrics(
            document_id=document_id,
            current_readers=current_readers,
            recent_activity=formatted_events,
            today_stats=today_stats
        )
        
        return metrics.dict()
        
    except Exception as e:
        logger.error(f"Error getting live metrics: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.get("/tracking/document-analytics/{document_id}")
async def get_document_analytics_for_outlook(
    document_id: str,
    current_user: User = Depends(get_current_active_user)
):
    """Get comprehensive analytics for a document (Outlook add-in optimized)"""
    try:
        # Check document access (same as above)
        documents_collection = await get_collection('documents')
        document = await documents_collection.find_one({"id": document_id})
        
        if not document:
            raise HTTPException(status_code=404, detail="Document not found")
        
        if document["owner_id"] != current_user.id:
            collaborator_ids = [c.get("user_id") for c in document.get("collaborators", [])]
            if current_user.id not in collaborator_ids:
                raise HTTPException(status_code=403, detail="Access denied")
        
        # Get all tracking events for this document
        tracking_events_collection = await get_collection('outlook_tracking_events')
        all_events = await tracking_events_collection.find({"document_id": document_id}).to_list(1000)
        
        # Get email tracking records
        outlook_tracking_collection = await get_collection('outlook_email_tracking')
        email_records = await outlook_tracking_collection.find({"document_id": document_id}).to_list(1000)
        
        # Calculate analytics
        total_emails_sent = len(email_records)
        total_opens = sum([record.get("total_opens", 0) for record in email_records])
        total_clicks = sum([record.get("total_clicks", 0) for record in email_records])
        
        unique_viewers = len(set([e.get("recipient_email") for e in all_events if e.get("recipient_email")]))
        
        # Page-wise analytics
        page_analytics = {}
        for event in all_events:
            if event.get("page_number") and event.get("event_type") == "page_viewed":
                page_num = event["page_number"]
                if page_num not in page_analytics:
                    page_analytics[page_num] = {"views": 0, "unique_viewers": set(), "total_time": 0}
                
                page_analytics[page_num]["views"] += 1
                if event.get("recipient_email"):
                    page_analytics[page_num]["unique_viewers"].add(event["recipient_email"])
                if event.get("duration"):
                    page_analytics[page_num]["total_time"] += event["duration"]
        
        # Convert sets to counts
        for page_num in page_analytics:
            page_analytics[page_num]["unique_viewers"] = len(page_analytics[page_num]["unique_viewers"])
            if page_analytics[page_num]["views"] > 0:
                page_analytics[page_num]["avg_time"] = page_analytics[page_num]["total_time"] / page_analytics[page_num]["views"]
            else:
                page_analytics[page_num]["avg_time"] = 0
        
        # Recent activity (last 7 days)
        seven_days_ago = datetime.utcnow() - timedelta(days=7)
        recent_activity = [e for e in all_events if e["timestamp"] >= seven_days_ago]
        
        analytics = {
            "document_id": document_id,
            "document_title": document["title"],
            "summary": {
                "total_emails_sent": total_emails_sent,
                "total_opens": total_opens,
                "total_clicks": total_clicks,
                "unique_viewers": unique_viewers,
                "open_rate": (total_opens / total_emails_sent * 100) if total_emails_sent > 0 else 0,
                "click_rate": (total_clicks / total_emails_sent * 100) if total_emails_sent > 0 else 0
            },
            "page_analytics": page_analytics,
            "recent_activity_count": len(recent_activity),
            "total_events": len(all_events),
            "last_activity": max([e["timestamp"] for e in all_events], default=None),
            "generated_at": datetime.utcnow().isoformat()
        }
        
        return analytics
        
    except Exception as e:
        logger.error(f"Error getting document analytics: {e}")
        raise HTTPException(status_code=500, detail=str(e))

# ==================== UTILITY ENDPOINTS ====================

@router.get("/user/session-info")
async def get_user_session_info(current_user: User = Depends(get_current_active_user)):
    """Get current user session information for Outlook add-in"""
    return {
        "user_email": current_user.email,
        "full_name": current_user.full_name,
        "organization": current_user.organization,
        "role": current_user.role,
        "connected_at": datetime.utcnow().isoformat(),
        "websocket_endpoint": "/api/outlook/ws",
        "permissions": {
            "can_send_documents": True,
            "can_view_analytics": True,
            "can_access_content_hub": True,
            "can_create_documents": current_user.role in ["admin", "editor"]
        }
    }

@router.get("/documents/{document_id}/share-link")
async def generate_trackable_link(
    document_id: str,
    current_user: User = Depends(get_current_active_user)
):
    """Generate a trackable link for a document"""
    try:
        # Check document access
        documents_collection = await get_collection('documents')
        document = await documents_collection.find_one({"id": document_id})
        
        if not document:
            raise HTTPException(status_code=404, detail="Document not found")
        
        # Generate trackable link with user info
        tracking_params = f"?source=outlook&sender={current_user.email}&timestamp={int(datetime.utcnow().timestamp())}"
        trackable_link = f"/view/{document_id}{tracking_params}"
        
        return {
            "document_id": document_id,
            "document_title": document["title"],
            "trackable_link": trackable_link,
            "full_url": f"{trackable_link}",  # Frontend will prepend domain
            "generated_by": current_user.email,
            "generated_at": datetime.utcnow().isoformat(),
            "tracking_enabled": True
        }
        
    except Exception as e:
        logger.error(f"Error generating trackable link: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.get("/documents/{document_id}/content")
async def get_document_content(
    document_id: str,
    current_user: User = Depends(get_current_active_user)
):
    """Get document content for preview/editing in Outlook add-in"""
    try:
        documents_collection = await get_collection('documents')
        document = await documents_collection.find_one({"id": document_id})
        
        if not document:
            raise HTTPException(status_code=404, detail="Document not found")
        
        if document["owner_id"] != current_user.id:
            collaborator_ids = [c.get("user_id") for c in document.get("collaborators", [])]
            if current_user.id not in collaborator_ids:
                raise HTTPException(status_code=403, detail="Access denied")
        
        return {
            "id": document["id"],
            "title": document["title"],
            "type": document["type"],
            "total_pages": document.get("total_pages", len(document.get("pages", []))),
            "pages": document.get("pages", []),
            "sections": document.get("sections", []),
            "can_edit": document["owner_id"] == current_user.id
        }
        
    except Exception as e:
        logger.error(f"Error getting document content: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.get("/status")
async def outlook_addin_status():
    """Health check and status for Outlook add-in"""
    connected_users = await websocket_manager.get_connected_users()
    
    return {
        "status": "healthy",
        "service": "effyDOC Outlook Add-in API",
        "version": "1.0.0",
        "connected_users": len(connected_users),
        "active_connections": sum(connected_users.values()),
        "timestamp": datetime.utcnow().isoformat(),
        "features": {
            "real_time_tracking": True,
            "document_library": True,
            "content_hub": True,
            "live_analytics": True,
            "websocket_support": True
        }
    }