import requests
import json
import unittest
import uuid
import os
import websocket
import threading
import time
from datetime import datetime

# Get the backend URL from the frontend .env file
BACKEND_URL = "https://54f44f8f-6cf4-4842-bf49-6bd490d293fd.preview.emergentagent.com/api"

def test_outlook_addin_integration():
    """Test the Outlook Add-in backend integration"""
    print("Starting Outlook Add-in API tests...")
    
    # Test variables
    base_url = BACKEND_URL
    headers = {"Content-Type": "application/json"}
    test_user_email = f"test.user.{uuid.uuid4()}@example.com"
    test_user_password = "SecurePassword123!"
    test_document_id = None
    
    # 1. Test Outlook Add-in status endpoint
    print("\n1. Testing Outlook Add-in status endpoint...")
    response = requests.get(f"{base_url}/outlook/status")
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "status" in data, "Missing status in response"
    assert data["status"] == "healthy", f"Expected 'healthy', got {data['status']}"
    assert "service" in data, "Missing service in response"
    assert "version" in data, "Missing version in response"
    assert "features" in data, "Missing features in response"
    assert "real_time_tracking" in data["features"], "Missing real_time_tracking in features"
    
    print("✅ Outlook Add-in status endpoint working")
    
    # 2. Register a test user
    print("\n2. Registering test user...")
    user_data = {
        "email": test_user_email,
        "full_name": "Test User",
        "role": "editor",
        "organization": "Test Organization",
        "password": test_user_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Save token for subsequent tests
    access_token = data["access_token"]
    headers["Authorization"] = f"Bearer {access_token}"
    print("✅ User registration successful")
    
    # 3. Test getting user session info for Outlook
    print("\n3. Testing get user session info for Outlook...")
    response = requests.get(f"{base_url}/outlook/user/session-info", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "user_email" in data, "Missing user_email in response"
    assert data["user_email"] == test_user_email, f"Expected {test_user_email}, got {data['user_email']}"
    assert "full_name" in data, "Missing full_name in response"
    assert "organization" in data, "Missing organization in response"
    assert "websocket_endpoint" in data, "Missing websocket_endpoint in response"
    assert "permissions" in data, "Missing permissions in response"
    
    print("✅ Get user session info for Outlook working")
    
    # 4. Create a test document for subsequent tests
    print("\n4. Creating test document...")
    document_data = {
        "title": "Test Outlook Document",
        "type": "proposal",
        "organization": "Test Organization",
        "sections": [
            {
                "title": "Introduction",
                "content": "This is a test document for Outlook integration.",
                "order": 1
            },
            {
                "title": "Features",
                "content": "This document demonstrates Outlook Add-in integration features.",
                "order": 2
            }
        ],
        "tags": ["test", "outlook", "integration"],
        "metadata": {"purpose": "testing"}
    }
    
    response = requests.post(f"{base_url}/documents", json=document_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Save document ID for subsequent tests
    test_document_id = data["id"]
    print(f"✅ Test document created with ID: {test_document_id}")
    
    # 5. Test getting user's document library for Outlook
    print("\n5. Testing get user's document library for Outlook...")
    response = requests.get(f"{base_url}/outlook/documents/my-library", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "documents" in data, "Missing documents in response"
    assert "total_count" in data, "Missing total_count in response"
    assert "user_email" in data, "Missing user_email in response"
    assert data["user_email"] == test_user_email, f"Expected {test_user_email}, got {data['user_email']}"
    
    # Check if our test document is in the library
    document_found = False
    for doc in data["documents"]:
        if doc["id"] == test_document_id:
            document_found = True
            assert doc["title"] == "Test Outlook Document", f"Expected 'Test Outlook Document', got {doc['title']}"
            assert "tracking_stats" in doc, "Missing tracking_stats in document"
            assert "share_link" in doc, "Missing share_link in document"
            break
    
    assert document_found, "Test document not found in user's library"
    print("✅ Get user's document library for Outlook working")
    
    # 6. Test getting content hub documents for Outlook
    print("\n6. Testing get content hub documents for Outlook...")
    response = requests.get(f"{base_url}/outlook/documents/content-hub", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "documents" in data, "Missing documents in response"
    assert "total_count" in data, "Missing total_count in response"
    assert "organization" in data, "Missing organization in response"
    
    print("✅ Get content hub documents for Outlook working")
    
    # 7. Test generating a trackable link for a document
    print("\n7. Testing generate trackable link for document...")
    response = requests.get(f"{base_url}/outlook/documents/{test_document_id}/share-link", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "document_id" in data, "Missing document_id in response"
    assert data["document_id"] == test_document_id, f"Expected {test_document_id}, got {data['document_id']}"
    assert "document_title" in data, "Missing document_title in response"
    assert "trackable_link" in data, "Missing trackable_link in response"
    assert "full_url" in data, "Missing full_url in response"
    assert "generated_by" in data, "Missing generated_by in response"
    assert data["generated_by"] == test_user_email, f"Expected {test_user_email}, got {data['generated_by']}"
    
    # Save trackable link for subsequent tests
    trackable_link = data["trackable_link"]
    print("✅ Generate trackable link for document working")
    
    # 8. Test tracking email sent event
    print("\n8. Testing track email sent event...")
    email_tracking_data = {
        "document_id": test_document_id,
        "recipients": ["recipient1@example.com", "recipient2@example.com"],
        "subject": "Test Document from Outlook",
        "email_body": "Please review the attached document."
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/email-sent", json=email_tracking_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "message" in data, "Missing message in response"
    assert "tracking_id" in data, "Missing tracking_id in response"
    assert "tracking_link" in data, "Missing tracking_link in response"
    
    # Save tracking ID for subsequent tests
    tracking_id = data["tracking_id"]
    print("✅ Track email sent event working")
    
    # 9. Test tracking document events
    print("\n9. Testing track document events...")
    
    # Test email opened event
    event_data = {
        "event_type": "email_opened",
        "document_id": test_document_id,
        "recipient_email": "recipient1@example.com",
        "user_agent": "Test User Agent",
        "ip_address": "192.168.1.1"
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "message" in data, "Missing message in response"
    assert "event_id" in data, "Missing event_id in response"
    
    # Test link clicked event
    event_data = {
        "event_type": "link_clicked",
        "document_id": test_document_id,
        "recipient_email": "recipient1@example.com",
        "user_agent": "Test User Agent",
        "ip_address": "192.168.1.1"
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Test page viewed event
    event_data = {
        "event_type": "page_viewed",
        "document_id": test_document_id,
        "recipient_email": "recipient1@example.com",
        "page_number": 1,
        "duration": 30,
        "user_agent": "Test User Agent",
        "ip_address": "192.168.1.1"
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Test currently reading event
    event_data = {
        "event_type": "currently_reading",
        "document_id": test_document_id,
        "recipient_email": "recipient1@example.com",
        "page_number": 2,
        "duration": 15,
        "user_agent": "Test User Agent",
        "ip_address": "192.168.1.1"
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("✅ Track document events working")
    
    # 10. Test getting live tracking metrics
    print("\n10. Testing get live tracking metrics...")
    response = requests.get(f"{base_url}/outlook/tracking/live-metrics/{test_document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "document_id" in data, "Missing document_id in response"
    assert data["document_id"] == test_document_id, f"Expected {test_document_id}, got {data['document_id']}"
    assert "current_readers" in data, "Missing current_readers in response"
    assert "recent_activity" in data, "Missing recent_activity in response"
    assert "today_stats" in data, "Missing today_stats in response"
    
    # Verify today's stats
    assert data["today_stats"]["emails_opened"] >= 1, "Expected at least 1 email opened"
    assert data["today_stats"]["links_clicked"] >= 1, "Expected at least 1 link clicked"
    assert data["today_stats"]["page_views"] >= 1, "Expected at least 1 page view"
    
    print("✅ Get live tracking metrics working")
    
    # 11. Test getting document analytics for Outlook
    print("\n11. Testing get document analytics for Outlook...")
    response = requests.get(f"{base_url}/outlook/tracking/document-analytics/{test_document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "document_id" in data, "Missing document_id in response"
    assert data["document_id"] == test_document_id, f"Expected {test_document_id}, got {data['document_id']}"
    assert "document_title" in data, "Missing document_title in response"
    assert "summary" in data, "Missing summary in response"
    assert "page_analytics" in data, "Missing page_analytics in response"
    
    # Verify summary data
    assert "total_emails_sent" in data["summary"], "Missing total_emails_sent in summary"
    assert "total_opens" in data["summary"], "Missing total_opens in summary"
    assert "total_clicks" in data["summary"], "Missing total_clicks in summary"
    assert "unique_viewers" in data["summary"], "Missing unique_viewers in summary"
    
    print("✅ Get document analytics for Outlook working")
    
    # 12. Test WebSocket connection (in a separate thread to avoid blocking)
    print("\n12. Testing WebSocket connection...")
    
    # Define WebSocket test function
    def test_websocket():
        try:
            # Create WebSocket URL with query parameters
            ws_url = f"wss://54f44f8f-6cf4-4842-bf49-6bd490d293fd.preview.emergentagent.com/api/outlook/ws?user_email={test_user_email}&token={access_token}"
            
            # Define WebSocket callbacks
            def on_message(ws, message):
                print(f"WebSocket received: {message}")
                message_data = json.loads(message)
                if message_data.get("type") == "subscription_confirmed":
                    print("✅ WebSocket subscription confirmed")
                    ws.close()
            
            def on_error(ws, error):
                print(f"WebSocket error: {error}")
            
            def on_close(ws, close_status_code, close_msg):
                print("WebSocket connection closed")
            
            def on_open(ws):
                print("WebSocket connection opened")
                # Subscribe to document updates
                ws.send(json.dumps({
                    "type": "subscribe_document",
                    "document_id": test_document_id
                }))
            
            # Create and start WebSocket connection
            ws = websocket.WebSocketApp(
                ws_url,
                on_open=on_open,
                on_message=on_message,
                on_error=on_error,
                on_close=on_close
            )
            
            ws.run_forever()
            
        except Exception as e:
            print(f"WebSocket test error: {e}")
    
    # Run WebSocket test in a separate thread
    try:
        websocket_thread = threading.Thread(target=test_websocket)
        websocket_thread.daemon = True
        websocket_thread.start()
        
        # Wait a moment for the WebSocket to connect
        time.sleep(2)
        
        # Trigger an event that should be sent to the WebSocket
        event_data = {
            "event_type": "currently_reading",
            "document_id": test_document_id,
            "recipient_email": "recipient2@example.com",
            "page_number": 1,
            "user_agent": "Test User Agent",
            "ip_address": "192.168.1.2"
        }
        
        response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
        assert response.status_code == 200, f"Expected 200, got {response.status_code}"
        
        # Wait for WebSocket to receive the message
        time.sleep(3)
        
        print("✅ WebSocket connection test completed")
        
    except ImportError:
        print("⚠️ websocket-client library not available. Skipping WebSocket test.")
        print("To run this test, install the required library: pip install websocket-client")
    
    # 13. Clean up (delete test document)
    print("\n13. Cleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{test_document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Test document deleted successfully")
    
    print("\n✅ All Outlook Add-in integration tests passed successfully!")

if __name__ == "__main__":
    # Run the Outlook Add-in integration tests
    test_outlook_addin_integration()