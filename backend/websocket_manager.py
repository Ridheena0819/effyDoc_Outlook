from fastapi import WebSocket, WebSocketDisconnect
from typing import List, Dict, Set
import json
import asyncio
from datetime import datetime
import logging
from models import OutlookTrackingEvent, LiveTrackingMetrics

logger = logging.getLogger(__name__)

class WebSocketManager:
    def __init__(self):
        # Track active connections by user email
        self.active_connections: Dict[str, List[WebSocket]] = {}
        # Track which documents each user is subscribed to
        self.user_subscriptions: Dict[str, Set[str]] = {}
        # Track document subscribers
        self.document_subscribers: Dict[str, Set[str]] = {}

    async def connect(self, websocket: WebSocket, user_email: str):
        """Connect a new WebSocket for a user"""
        await websocket.accept()
        
        if user_email not in self.active_connections:
            self.active_connections[user_email] = []
            self.user_subscriptions[user_email] = set()
        
        self.active_connections[user_email].append(websocket)
        logger.info(f"WebSocket connected for user: {user_email}")

    async def disconnect(self, websocket: WebSocket, user_email: str):
        """Disconnect a WebSocket for a user"""
        if user_email in self.active_connections:
            try:
                self.active_connections[user_email].remove(websocket)
                
                # Remove user if no more connections
                if not self.active_connections[user_email]:
                    del self.active_connections[user_email]
                    
                    # Clean up subscriptions
                    if user_email in self.user_subscriptions:
                        for doc_id in self.user_subscriptions[user_email]:
                            if doc_id in self.document_subscribers:
                                self.document_subscribers[doc_id].discard(user_email)
                                if not self.document_subscribers[doc_id]:
                                    del self.document_subscribers[doc_id]
                        del self.user_subscriptions[user_email]
                        
            except ValueError:
                pass  # Connection already removed
            
        logger.info(f"WebSocket disconnected for user: {user_email}")

    async def subscribe_to_document(self, user_email: str, document_id: str):
        """Subscribe a user to document tracking updates"""
        if user_email not in self.user_subscriptions:
            self.user_subscriptions[user_email] = set()
        
        self.user_subscriptions[user_email].add(document_id)
        
        if document_id not in self.document_subscribers:
            self.document_subscribers[document_id] = set()
        
        self.document_subscribers[document_id].add(user_email)
        
        logger.info(f"User {user_email} subscribed to document {document_id}")

    async def unsubscribe_from_document(self, user_email: str, document_id: str):
        """Unsubscribe a user from document tracking updates"""
        if user_email in self.user_subscriptions:
            self.user_subscriptions[user_email].discard(document_id)
        
        if document_id in self.document_subscribers:
            self.document_subscribers[document_id].discard(user_email)
            if not self.document_subscribers[document_id]:
                del self.document_subscribers[document_id]
        
        logger.info(f"User {user_email} unsubscribed from document {document_id}")

    async def send_personal_message(self, user_email: str, message: dict):
        """Send a message to all connections of a specific user"""
        if user_email in self.active_connections:
            disconnected_connections = []
            
            for connection in self.active_connections[user_email]:
                try:
                    await connection.send_text(json.dumps(message))
                except Exception as e:
                    logger.error(f"Error sending message to {user_email}: {e}")
                    disconnected_connections.append(connection)
            
            # Clean up disconnected connections
            for connection in disconnected_connections:
                await self.disconnect(connection, user_email)

    async def broadcast_to_document_subscribers(self, document_id: str, message: dict):
        """Send a message to all users subscribed to a document"""
        if document_id in self.document_subscribers:
            for user_email in self.document_subscribers[document_id].copy():
                await self.send_personal_message(user_email, message)

    async def send_tracking_update(self, document_id: str, event: OutlookTrackingEvent):
        """Send real-time tracking update to document subscribers"""
        message = {
            "type": "tracking_update",
            "document_id": document_id,
            "event": {
                "id": event.id,
                "event_type": event.event_type,
                "timestamp": event.timestamp.isoformat(),
                "user_email": event.user_email,
                "recipient_email": event.recipient_email,
                "page_number": event.page_number,
                "duration": event.duration,
                "metadata": event.metadata
            }
        }
        
        await self.broadcast_to_document_subscribers(document_id, message)
        logger.info(f"Sent tracking update for document {document_id}: {event.event_type}")

    async def send_live_metrics_update(self, document_id: str, metrics: LiveTrackingMetrics):
        """Send live metrics update to document subscribers"""
        message = {
            "type": "live_metrics",
            "document_id": document_id,
            "metrics": {
                "current_readers": metrics.current_readers,
                "recent_activity": [
                    {
                        "event_type": event.event_type,
                        "timestamp": event.timestamp.isoformat(),
                        "user_email": event.user_email,
                        "recipient_email": event.recipient_email,
                        "page_number": event.page_number
                    } for event in metrics.recent_activity[-10:]  # Last 10 events
                ],
                "today_stats": metrics.today_stats,
                "generated_at": metrics.generated_at.isoformat()
            }
        }
        
        await self.broadcast_to_document_subscribers(document_id, message)

    async def notify_currently_reading(self, document_id: str, user_email: str, page_number: int):
        """Notify that someone is currently reading a document"""
        event = OutlookTrackingEvent(
            event_type="currently_reading",
            document_id=document_id,
            recipient_email=user_email,
            page_number=page_number,
            metadata={"status": "active"}
        )
        
        await self.send_tracking_update(document_id, event)

    async def get_connected_users(self) -> Dict[str, int]:
        """Get list of connected users and their connection count"""
        return {user: len(connections) for user, connections in self.active_connections.items()}

    async def get_document_subscribers_count(self, document_id: str) -> int:
        """Get number of users subscribed to a document"""
        return len(self.document_subscribers.get(document_id, set()))

# Global WebSocket manager instance
websocket_manager = WebSocketManager()

# Utility function to send notifications
async def notify_document_activity(document_id: str, event_type: str, data: dict):
    """Helper function to send document activity notifications"""
    event = OutlookTrackingEvent(
        event_type=event_type,
        document_id=document_id,
        **data
    )
    
    await websocket_manager.send_tracking_update(document_id, event)