from fastapi import APIRouter, Depends, HTTPException
from fastapi.responses import JSONResponse
from typing import List, Optional, Dict, Any
from datetime import datetime, timedelta
import json
import logging

from models import *
from database import get_collection
from auth import get_current_active_user

logger = logging.getLogger(__name__)
router = APIRouter(prefix="/api/outlook-native", tags=["outlook-native-plugin"])

# ==================== DOCUMENT LIBRARY ENDPOINTS ====================

@router.get("/documents/my-library")
async def get_my_library(current_user: User = Depends(get_current_active_user)):
    """Get user's personal document library for native Outlook plugin"""
    documents_collection = await get_collection('documents')
    
    # Get user's documents
    query = {
        "owner_id": current_user.id,
        "status": {"$ne": "archived"}
    }
    
    documents = await documents_collection.find(query).to_list(1000)
    
    # Format for native Outlook plugin
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
            "total_views": len([e for e in tracking_events if e.get("action") == "VIEW"]),
            "description": doc.get("description", ""),
            "tracking_link": f"/view/{doc['id']}?source=outlook_native",
            "is_trackable": True
        })
    
    return {
        "documents": library_documents,
        "total_count": len(library_documents),
        "user_email": current_user.email
    }

@router.get("/documents/content-hub")
async def get_content_hub(current_user: User = Depends(get_current_active_user)):
    """Get admin-shared documents (Content Hub) for native Outlook plugin"""
    documents_collection = await get_collection('documents')
    
    # Get admin-shared documents for user's organization
    query = {
        "organization": current_user.organization,
        "metadata.is_admin_shared": True,
        "status": {"$in": ["approved", "sent"]}
    }
    
    documents = await documents_collection.find(query).to_list(1000)
    
    # Format for native Outlook plugin
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
            "tracking_link": f"/view/{doc['id']}?source=outlook_native",
            "is_template": doc.get("metadata", {}).get("is_template", False),
            "is_trackable": True
        })
    
    return {
        "documents": hub_documents,
        "total_count": len(hub_documents),
        "organization": current_user.organization
    }

# ==================== DOCUMENT CONTENT ENDPOINTS ====================

@router.get("/documents/{document_id}/content")
async def get_document_content(
    document_id: str,
    current_user: User = Depends(get_current_active_user)
):
    """Get document content for preview in native Outlook plugin"""
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

@router.post("/documents/{document_id}/generate-attachment")
async def generate_trackable_attachment(
    document_id: str,
    current_user: User = Depends(get_current_active_user)
):
    """Generate trackable HTML attachment for native Outlook plugin"""
    try:
        documents_collection = await get_collection('documents')
        document = await documents_collection.find_one({"id": document_id})
        
        if not document:
            raise HTTPException(status_code=404, detail="Document not found")
        
        if document["owner_id"] != current_user.id:
            collaborator_ids = [c.get("user_id") for c in document.get("collaborators", [])]
            if current_user.id not in collaborator_ids:
                raise HTTPException(status_code=403, detail="Access denied")
        
        # Generate tracking link
        tracking_params = f"?source=outlook_native&sender={current_user.email}&timestamp={int(datetime.utcnow().timestamp())}"
        tracking_link = f"/view/{document_id}{tracking_params}"
        
        # Generate HTML content for email
        pages = document.get("pages", [])
        title = document.get("title", "Document")
        
        html_content = f"""
        <div style='border: 2px solid #4f46e5; border-radius: 8px; padding: 16px; margin: 16px 0; background: #f8fafc;'>
            <div style='display: flex; align-items: center; margin-bottom: 12px;'>
                <strong style='color: #4f46e5; font-size: 16px;'>📄 effyDOC Document</strong>
            </div>
            <h3 style='color: #1e293b; margin: 0 0 8px 0; font-size: 18px;'>{title}</h3>
            <p style='color: #64748b; margin: 0 0 12px 0; font-size: 14px;'>{document.get("type", "document")} • {len(pages)} pages • Updated {document.get("updated_at", datetime.utcnow()).strftime("%b %d, %Y")}</p>
            <p style='color: #4f46e5; font-size: 12px; margin: 12px 0;'>
                📊 This document includes tracking analytics and interactive elements
            </p>
            <div style='margin-top: 12px;'>
                <a href='{tracking_link}' 
                   style='background: #4f46e5; color: white; padding: 8px 16px; text-decoration: none; border-radius: 6px; font-size: 14px; font-weight: 500;'
                   data-effydoc-document='{document_id}' 
                   class='effydoc-tracking-link'>
                   View Full Document
                </a>
            </div>
        </div>"""
        
        return {
            "document_id": document_id,
            "document_title": title,
            "html_content": html_content,
            "tracking_link": tracking_link,
            "generated_at": datetime.utcnow().isoformat()
        }
        
    except Exception as e:
        logger.error(f"Error generating attachment: {e}")
        raise HTTPException(status_code=500, detail=str(e))

# ==================== TRACKING ENDPOINTS ====================

@router.post("/tracking/email-sent")
async def track_email_sent(
    tracking_data: Dict[str, Any],
    current_user: User = Depends(get_current_active_user)
):
    """Track when a document is sent via email from native Outlook plugin"""
    try:
        document_id = tracking_data["document_id"]
        recipients = tracking_data["recipients"]
        subject = tracking_data.get("subject", "")
        
        # Store tracking event
        documents_collection = await get_collection('documents')
        
        # Add tracking event to document
        tracking_event = {
            "action": "EMAIL_SENT",
            "user_id": current_user.id,
            "user_email": current_user.email,
            "timestamp": datetime.utcnow(),
            "metadata": {
                "recipients": recipients,
                "subject": subject,
                "source": "outlook_native_plugin",
                "recipient_count": len(recipients)
            }
        }
        
        await documents_collection.update_one(
            {"id": document_id},
            {
                "$push": {"tracking_events": tracking_event},
                "$set": {"last_activity": datetime.utcnow()}
            }
        )
        
        return {
            "message": "Email tracking started",
            "document_id": document_id,
            "recipients": recipients,
            "tracking_link": f"/view/{document_id}?source=outlook_native&sender={current_user.email}"
        }
        
    except Exception as e:
        logger.error(f"Error tracking email sent: {e}")
        raise HTTPException(status_code=500, detail=str(e))

@router.get("/analytics/documents/{document_id}")
async def get_document_analytics(
    document_id: str,
    current_user: User = Depends(get_current_active_user)
):
    """Get comprehensive analytics for a document (Native Outlook plugin)"""
    try:
        documents_collection = await get_collection('documents')
        document = await documents_collection.find_one({"id": document_id})
        
        if not document:
            raise HTTPException(status_code=404, detail="Document not found")
        
        if document["owner_id"] != current_user.id:
            collaborator_ids = [c.get("user_id") for c in document.get("collaborators", [])]
            if current_user.id not in collaborator_ids:
                raise HTTPException(status_code=403, detail="Access denied")
        
        # Get tracking events
        tracking_events = document.get("tracking_events", [])
        
        # Calculate analytics
        total_views = len([e for e in tracking_events if e.get("action") == "VIEW"])
        total_emails = len([e for e in tracking_events if e.get("action") == "EMAIL_SENT"])
        unique_viewers = len(set([e.get("user_email") for e in tracking_events if e.get("user_email")]))
        
        # Recent activity (last 24 hours)
        twenty_four_hours_ago = datetime.utcnow() - timedelta(hours=24)
        recent_events = [
            e for e in tracking_events 
            if e.get("timestamp", datetime.min) >= twenty_four_hours_ago
        ]
        
        analytics = {
            "document_id": document_id,
            "document_title": document["title"],
            "summary": {
                "total_views": total_views,
                "total_emails": total_emails,
                "unique_viewers": unique_viewers,
                "total_opens": len([e for e in tracking_events if e.get("action") == "OPEN"]),
                "total_clicks": len([e for e in tracking_events if e.get("action") == "CLICK"]),
                "open_rate": (len([e for e in tracking_events if e.get("action") == "OPEN"]) / max(total_emails, 1)) * 100,
                "click_rate": (len([e for e in tracking_events if e.get("action") == "CLICK"]) / max(total_emails, 1)) * 100
            },
            "recent_activity_count": len(recent_events),
            "total_events": len(tracking_events),
            "last_activity": max([e.get("timestamp") for e in tracking_events], default=None),
            "generated_at": datetime.utcnow().isoformat()
        }
        
        return analytics
        
    except Exception as e:
        logger.error(f"Error getting document analytics: {e}")
        raise HTTPException(status_code=500, detail=str(e))

# ==================== USER SESSION ENDPOINTS ====================

@router.get("/user/session-info")
async def get_user_session_info(current_user: User = Depends(get_current_active_user)):
    """Get current user session information for native Outlook plugin"""
    return {
        "user_email": current_user.email,
        "full_name": current_user.full_name,
        "organization": current_user.organization,
        "role": current_user.role,
        "connected_at": datetime.utcnow().isoformat(),
        "permissions": {
            "can_send_documents": True,
            "can_view_analytics": True,
            "can_access_content_hub": True,
            "can_create_documents": current_user.role in ["admin", "editor"]
        },
        "plugin_version": "1.0.0",
        "api_version": "native-v1"
    }

@router.get("/status")
async def outlook_native_plugin_status():
    """Health check and status for native Outlook plugin"""
    return {
        "status": "healthy",
        "service": "effyDOC Native Outlook Plugin API",
        "version": "1.0.0",
        "timestamp": datetime.utcnow().isoformat(),
        "features": {
            "document_library": True,
            "content_hub": True,
            "analytics": True,
            "tracking": True,
            "native_integration": True
        }
    }