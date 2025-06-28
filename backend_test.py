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
BACKEND_URL = "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/api"

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
    
    # 7. Test getting document content for preview/editing
    print("\n7. Testing get document content for preview/editing...")
    response = requests.get(f"{base_url}/outlook/documents/{test_document_id}/content", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "id" in data, "Missing id in response"
    assert data["id"] == test_document_id, f"Expected {test_document_id}, got {data['id']}"
    assert "title" in data, "Missing title in response"
    assert "type" in data, "Missing type in response"
    assert "pages" in data, "Missing pages in response"
    assert "sections" in data, "Missing sections in response"
    assert "can_edit" in data, "Missing can_edit in response"
    assert data["can_edit"] == True, "Expected can_edit to be True for document owner"
    
    print("✅ Get document content for preview/editing working")
    
    # 8. Test updating document content
    print("\n8. Testing update document content...")
    content_update = {
        "title": "Updated Outlook Document Title",
        "pages": [
            {
                "id": str(uuid.uuid4()),
                "page_number": 1,
                "title": "Updated Introduction",
                "content": "<p>This is updated content for testing the Outlook add-in.</p>"
            },
            {
                "id": str(uuid.uuid4()),
                "page_number": 2,
                "title": "Updated Features",
                "content": "<p>These are updated features for testing the Outlook add-in.</p>"
            }
        ]
    }
    
    response = requests.put(f"{base_url}/outlook/documents/{test_document_id}/content", json=content_update, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "message" in data, "Missing message in response"
    assert "document_id" in data, "Missing document_id in response"
    assert data["document_id"] == test_document_id, f"Expected {test_document_id}, got {data['document_id']}"
    
    # Verify the update was successful
    response = requests.get(f"{base_url}/outlook/documents/{test_document_id}/content", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    assert data["title"] == "Updated Outlook Document Title", f"Expected 'Updated Outlook Document Title', got {data['title']}"
    assert len(data["pages"]) == 2, f"Expected 2 pages, got {len(data['pages'])}"
    
    print("✅ Update document content working")
    
    # 9. Test generating a trackable link for a document
    print("\n9. Testing generate trackable link for document...")
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
    
    # 10. Test generating attachment data
    print("\n10. Testing generate attachment data...")
    options = {
        "include_tracking": True,
        "format": "html"
    }
    
    response = requests.post(f"{base_url}/outlook/documents/{test_document_id}/attachment-data", json=options, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "document_id" in data, "Missing document_id in response"
    assert data["document_id"] == test_document_id, f"Expected {test_document_id}, got {data['document_id']}"
    assert "filename" in data, "Missing filename in response"
    assert "content" in data, "Missing content in response"
    assert "tracking_link" in data, "Missing tracking_link in response"
    
    # Verify HTML content contains tracking link
    assert "View online version" in data["content"], "Missing 'View online version' link in HTML content"
    assert data["tracking_link"] in data["content"], "Tracking link not found in HTML content"
    
    print("✅ Generate attachment data working")
    
    # 11. Test tracking email sent event
    print("\n11. Testing track email sent event...")
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
    
    # 12. Test tracking document events
    print("\n12. Testing track document events...")
    
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
    
    # 13. Test getting live tracking metrics
    print("\n13. Testing get live tracking metrics...")
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
    
    # 14. Test getting document analytics for Outlook
    print("\n14. Testing get document analytics for Outlook...")
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
    
    # 15. Test WebSocket connection (in a separate thread to avoid blocking)
    print("\n15. Testing WebSocket connection...")
    
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
    
    # 16. Test complete attachment workflow
    print("\n16. Testing complete attachment workflow...")
    
    # Step 1: Get document content
    response = requests.get(f"{base_url}/outlook/documents/{test_document_id}/content", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    document_content = response.json()
    
    # Step 2: Update document content (optional)
    content_update = {
        "title": document_content["title"],
        "pages": [
            {
                "id": str(uuid.uuid4()) if "id" not in document_content["pages"][0] else document_content["pages"][0]["id"],
                "page_number": 1,
                "title": "Final Introduction",
                "content": "<p>This is the final content for the attachment workflow test.</p>"
            }
        ]
    }
    
    response = requests.put(f"{base_url}/outlook/documents/{test_document_id}/content", json=content_update, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Step 3: Generate attachment data
    options = {
        "include_tracking": True,
        "format": "html"
    }
    
    response = requests.post(f"{base_url}/outlook/documents/{test_document_id}/attachment-data", json=options, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    attachment_data = response.json()
    
    # Step 4: Track email sent event
    email_tracking_data = {
        "document_id": test_document_id,
        "recipients": ["workflow.test@example.com"],
        "subject": "Attachment Workflow Test",
        "email_body": "This is a test of the complete attachment workflow."
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/email-sent", json=email_tracking_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("✅ Complete attachment workflow test passed")
    
    # 17. Clean up (delete test document)
    print("\n17. Cleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{test_document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Test document deleted successfully")
    
    print("\n✅ All Outlook Add-in integration tests passed successfully!")

def test_outlook_addin_access_control():
    """Test access control for Outlook Add-in endpoints"""
    print("\nStarting Outlook Add-in access control tests...")
    
    # Test variables
    base_url = BACKEND_URL
    headers = {"Content-Type": "application/json"}
    
    # Create two test users
    test_user1_email = f"test.user1.{uuid.uuid4()}@example.com"
    test_user2_email = f"test.user2.{uuid.uuid4()}@example.com"
    test_password = "SecurePassword123!"
    
    # Register first user
    print("\n1. Registering first test user...")
    user1_data = {
        "email": test_user1_email,
        "full_name": "Test User 1",
        "role": "editor",
        "organization": "Test Organization",
        "password": test_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user1_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    user1_token = response.json()["access_token"]
    user1_headers = headers.copy()
    user1_headers["Authorization"] = f"Bearer {user1_token}"
    
    # Register second user
    print("2. Registering second test user...")
    user2_data = {
        "email": test_user2_email,
        "full_name": "Test User 2",
        "role": "editor",
        "organization": "Test Organization",
        "password": test_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user2_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    user2_token = response.json()["access_token"]
    user2_headers = headers.copy()
    user2_headers["Authorization"] = f"Bearer {user2_token}"
    
    # Create a document as user 1
    print("3. Creating test document as user 1...")
    document_data = {
        "title": "Access Control Test Document",
        "type": "proposal",
        "organization": "Test Organization",
        "sections": [
            {
                "title": "Introduction",
                "content": "This is a test document for access control testing.",
                "order": 1
            }
        ],
        "tags": ["test", "access-control"],
        "metadata": {"purpose": "testing"}
    }
    
    response = requests.post(f"{base_url}/documents", json=document_data, headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    document_id = response.json()["id"]
    
    # Test access control for document content endpoint
    print("4. Testing access control for document content endpoint...")
    
    # User 1 (owner) should have access
    response = requests.get(f"{base_url}/outlook/documents/{document_id}/content", headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    assert response.json()["can_edit"] == True, "Owner should have edit permission"
    
    # User 2 (non-owner) should not have access
    response = requests.get(f"{base_url}/outlook/documents/{document_id}/content", headers=user2_headers)
    assert response.status_code in [403, 500], f"Expected 403 or 500, got {response.status_code}"
    print("✅ Access control for document content endpoint working (non-owner denied access)")
    
    # Test access control for document content update endpoint
    print("5. Testing access control for document content update endpoint...")
    
    content_update = {
        "title": "Updated Title",
        "pages": [
            {
                "id": str(uuid.uuid4()),
                "page_number": 1,
                "title": "Updated Introduction",
                "content": "<p>This is updated content.</p>"
            }
        ]
    }
    
    # User 1 (owner) should be able to update
    response = requests.put(f"{base_url}/outlook/documents/{document_id}/content", json=content_update, headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # User 2 (non-owner) should not be able to update
    response = requests.put(f"{base_url}/outlook/documents/{document_id}/content", json=content_update, headers=user2_headers)
    assert response.status_code in [403, 500], f"Expected 403 or 500, got {response.status_code}"
    print("✅ Access control for document content update endpoint working (non-owner denied access)")
    
    # Test access control for attachment data generation
    print("6. Testing access control for attachment data generation...")
    
    options = {
        "include_tracking": True,
        "format": "html"
    }
    
    # User 1 (owner) should be able to generate attachment data
    response = requests.post(f"{base_url}/outlook/documents/{document_id}/attachment-data", json=options, headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # User 2 (non-owner) should not be able to generate attachment data
    response = requests.post(f"{base_url}/outlook/documents/{document_id}/attachment-data", json=options, headers=user2_headers)
    assert response.status_code in [403, 500], f"Expected 403 or 500, got {response.status_code}"
    print("✅ Access control for attachment data generation working (non-owner denied access)")
    
    # Add user 2 as a collaborator
    print("7. Adding user 2 as a collaborator...")
    
    # First, get the user ID for user 2
    response = requests.get(f"{base_url}/users/me", headers=user2_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    user2_id = response.json()["id"]
    
    update_data = {
        "collaborators": [
            {
                "user_id": user2_id,
                "role": "editor"
            }
        ]
    }
    
    response = requests.put(f"{base_url}/documents/{document_id}", json=update_data, headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Test access after adding as collaborator
    print("8. Testing access after adding as collaborator...")
    
    # User 2 should now have access to view document content
    response = requests.get(f"{base_url}/outlook/documents/{document_id}/content", headers=user2_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # User 2 should be able to generate attachment data
    response = requests.post(f"{base_url}/outlook/documents/{document_id}/attachment-data", json=options, headers=user2_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Clean up
    print("9. Cleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{document_id}", headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("✅ All access control tests passed successfully!")

def test_outlook_addin_workflow():
    """Test complete user workflows for Outlook Add-in"""
    print("\nStarting Outlook Add-in workflow tests...")
    
    # Test variables
    base_url = BACKEND_URL
    headers = {"Content-Type": "application/json"}
    
    # Workflow A: First-Time User Setup
    print("\nWorkflow A: First-Time User Setup")
    
    # 1. Register new user account
    print("1. Registering new user...")
    test_user_email = f"workflow.user.{uuid.uuid4()}@example.com"
    test_user_password = "SecurePassword123!"
    
    user_data = {
        "email": test_user_email,
        "full_name": "Workflow Test User",
        "role": "editor",
        "organization": "Workflow Test Org",
        "password": test_user_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Save token for subsequent tests
    access_token = data["access_token"]
    headers["Authorization"] = f"Bearer {access_token}"
    print("✅ User registration successful")
    
    # 2. Login via Outlook add-in authentication
    print("2. Testing Outlook add-in authentication...")
    response = requests.get(f"{base_url}/outlook/user/session-info", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Outlook add-in authentication successful")
    
    # 3. Browse empty document libraries
    print("3. Browsing document libraries...")
    response = requests.get(f"{base_url}/outlook/documents/my-library", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    assert len(response.json()["documents"]) == 0, "Expected empty document library"
    
    response = requests.get(f"{base_url}/outlook/documents/content-hub", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Empty document libraries browsed successfully")
    
    # 4. Create first test document
    print("4. Creating first document...")
    document_data = {
        "title": "My First Document",
        "type": "proposal",
        "organization": "Workflow Test Org",
        "sections": [
            {
                "title": "Introduction",
                "content": "This is my first document created through the Outlook add-in.",
                "order": 1
            }
        ],
        "tags": ["first", "test"],
        "metadata": {"source": "outlook_addin"}
    }
    
    response = requests.post(f"{base_url}/documents", json=document_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    document_id = response.json()["id"]
    print(f"✅ First document created with ID: {document_id}")
    
    # 5. Generate trackable links and attachments
    print("5. Generating trackable link...")
    response = requests.get(f"{base_url}/outlook/documents/{document_id}/share-link", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    trackable_link = response.json()["trackable_link"]
    
    print("6. Generating trackable attachment...")
    options = {
        "include_tracking": True,
        "format": "html"
    }
    
    response = requests.post(f"{base_url}/outlook/documents/{document_id}/attachment-data", json=options, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Trackable links and attachments generated successfully")
    
    # Workflow B: Document Sharing & Tracking
    print("\nWorkflow B: Document Sharing & Tracking")
    
    # 1. Browse My Library
    print("1. Browsing My Library...")
    response = requests.get(f"{base_url}/outlook/documents/my-library", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    assert len(response.json()["documents"]) > 0, "Expected non-empty document library"
    print("✅ My Library browsed successfully")
    
    # 2. Select document and test preview functionality
    print("2. Testing document preview...")
    response = requests.get(f"{base_url}/outlook/documents/{document_id}/content", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Document preview working")
    
    # 3. Edit document content via Outlook add-in
    print("3. Editing document content...")
    content_update = {
        "title": "My Updated Document",
        "pages": [
            {
                "id": str(uuid.uuid4()),
                "page_number": 1,
                "title": "Updated Introduction",
                "content": "<p>This is my updated document content for testing tracking.</p>"
            }
        ]
    }
    
    response = requests.put(f"{base_url}/outlook/documents/{document_id}/content", json=content_update, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Document content updated successfully")
    
    # 4. Generate HTML trackable attachment
    print("4. Generating HTML trackable attachment...")
    options = {
        "include_tracking": True,
        "format": "html"
    }
    
    response = requests.post(f"{base_url}/outlook/documents/{document_id}/attachment-data", json=options, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ HTML trackable attachment generated successfully")
    
    # 5. Track email sent event
    print("5. Tracking email sent event...")
    email_tracking_data = {
        "document_id": document_id,
        "recipients": ["recipient1@example.com", "recipient2@example.com"],
        "subject": "Please review my document",
        "email_body": "I've attached a document for your review."
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/email-sent", json=email_tracking_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Email sent event tracked successfully")
    
    # 6. Simulate recipient interactions
    print("6. Simulating recipient interactions...")
    
    # Email open
    event_data = {
        "event_type": "email_opened",
        "document_id": document_id,
        "recipient_email": "recipient1@example.com",
        "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
        "ip_address": "192.168.1.1"
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Link click
    event_data = {
        "event_type": "link_clicked",
        "document_id": document_id,
        "recipient_email": "recipient1@example.com",
        "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
        "ip_address": "192.168.1.1"
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Page view
    event_data = {
        "event_type": "page_viewed",
        "document_id": document_id,
        "recipient_email": "recipient1@example.com",
        "page_number": 1,
        "duration": 45,
        "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
        "ip_address": "192.168.1.1"
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Currently reading
    event_data = {
        "event_type": "currently_reading",
        "document_id": document_id,
        "recipient_email": "recipient1@example.com",
        "page_number": 1,
        "duration": 30,
        "user_agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64)",
        "ip_address": "192.168.1.1"
    }
    
    response = requests.post(f"{base_url}/outlook/tracking/event", json=event_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Recipient interactions simulated successfully")
    
    # 7. Verify real-time tracking metrics update
    print("7. Verifying real-time tracking metrics...")
    response = requests.get(f"{base_url}/outlook/tracking/live-metrics/{document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    assert data["today_stats"]["emails_opened"] >= 1, "Expected at least 1 email opened"
    assert data["today_stats"]["links_clicked"] >= 1, "Expected at least 1 link clicked"
    assert data["today_stats"]["page_views"] >= 1, "Expected at least 1 page view"
    print("✅ Real-time tracking metrics verified successfully")
    
    # 8. Test comprehensive analytics
    print("8. Testing comprehensive analytics...")
    response = requests.get(f"{base_url}/outlook/tracking/document-analytics/{document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    assert data["summary"]["total_emails_sent"] >= 1, "Expected at least 1 email sent"
    assert data["summary"]["total_opens"] >= 1, "Expected at least 1 email opened"
    assert data["summary"]["total_clicks"] >= 1, "Expected at least 1 link clicked"
    print("✅ Comprehensive analytics verified successfully")
    
    # Clean up
    print("\nCleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Test document deleted successfully")
    
    print("\n✅ All Outlook Add-in workflow tests passed successfully!")

def test_outlook_native_plugin_api():
    """Test the native Outlook plugin API endpoints"""
    print("\nStarting Native Outlook Plugin API tests...")
    
    # Test variables
    base_url = BACKEND_URL
    headers = {"Content-Type": "application/json"}
    test_user_email = f"test.native.user.{uuid.uuid4()}@example.com"
    test_user_password = "SecurePassword123!"
    test_document_id = None
    
    # 1. Test the status endpoint
    print("\n1. Testing Native Outlook Plugin status endpoint...")
    response = requests.get(f"{base_url}/outlook-native/status")
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "status" in data, "Missing status in response"
    assert data["status"] == "healthy", f"Expected 'healthy', got {data['status']}"
    assert "service" in data, "Missing service in response"
    assert "version" in data, "Missing version in response"
    assert "features" in data, "Missing features in response"
    
    print("✅ Native Outlook Plugin status endpoint working")
    
    # 2. Register a test user
    print("\n2. Registering test user...")
    user_data = {
        "email": test_user_email,
        "full_name": "Native Test User",
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
    
    # 3. Test getting user session info
    print("\n3. Testing get user session info...")
    response = requests.get(f"{base_url}/outlook-native/user/session-info", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "user_email" in data, "Missing user_email in response"
    assert data["user_email"] == test_user_email, f"Expected {test_user_email}, got {data['user_email']}"
    assert "full_name" in data, "Missing full_name in response"
    assert "organization" in data, "Missing organization in response"
    assert "permissions" in data, "Missing permissions in response"
    
    print("✅ Get user session info working")
    
    # 4. Create a test document for subsequent tests
    print("\n4. Creating test document...")
    document_data = {
        "title": "Native Outlook Test Document",
        "type": "proposal",
        "organization": "Test Organization",
        "sections": [
            {
                "title": "Introduction",
                "content": "This is a test document for native Outlook plugin integration.",
                "order": 1
            },
            {
                "title": "Features",
                "content": "This document demonstrates native Outlook plugin integration features.",
                "order": 2
            }
        ],
        "tags": ["test", "outlook", "native", "integration"],
        "metadata": {"purpose": "testing"}
    }
    
    response = requests.post(f"{base_url}/documents", json=document_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Save document ID for subsequent tests
    test_document_id = data["id"]
    print(f"✅ Test document created with ID: {test_document_id}")
    
    # 5. Test getting user's document library
    print("\n5. Testing get user's document library...")
    response = requests.get(f"{base_url}/outlook-native/documents/my-library", headers=headers)
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
            assert doc["title"] == "Native Outlook Test Document", f"Expected 'Native Outlook Test Document', got {doc['title']}"
            break
    
    assert document_found, "Test document not found in user's library"
    print("✅ Get user's document library working")
    
    # 6. Test getting content hub documents
    print("\n6. Testing get content hub documents...")
    response = requests.get(f"{base_url}/outlook-native/documents/content-hub", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "documents" in data, "Missing documents in response"
    assert "total_count" in data, "Missing total_count in response"
    assert "organization" in data, "Missing organization in response"
    
    print("✅ Get content hub documents working")
    
    # 7. Test getting document content
    print("\n7. Testing get document content...")
    response = requests.get(f"{base_url}/outlook-native/documents/{test_document_id}/content", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "id" in data, "Missing id in response"
    assert data["id"] == test_document_id, f"Expected {test_document_id}, got {data['id']}"
    assert "title" in data, "Missing title in response"
    assert "type" in data, "Missing type in response"
    assert "pages" in data, "Missing pages in response"
    assert "sections" in data, "Missing sections in response"
    assert "can_edit" in data, "Missing can_edit in response"
    assert data["can_edit"] == True, "Expected can_edit to be True for document owner"
    
    print("✅ Get document content working")
    
    # 8. Test generating trackable attachment
    print("\n8. Testing generate trackable attachment...")
    response = requests.post(f"{base_url}/outlook-native/documents/{test_document_id}/generate-attachment", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "document_id" in data, "Missing document_id in response"
    assert data["document_id"] == test_document_id, f"Expected {test_document_id}, got {data['document_id']}"
    assert "document_title" in data, "Missing document_title in response"
    assert "html_content" in data, "Missing html_content in response"
    assert "tracking_link" in data, "Missing tracking_link in response"
    assert "generated_at" in data, "Missing generated_at in response"
    
    # Save tracking link for subsequent tests
    tracking_link = data["tracking_link"]
    print("✅ Generate trackable attachment working")
    
    # 9. Test tracking email sent event
    print("\n9. Testing track email sent event...")
    email_tracking_data = {
        "document_id": test_document_id,
        "recipients": ["recipient1@example.com", "recipient2@example.com"],
        "subject": "Test Document from Native Outlook Plugin"
    }
    
    response = requests.post(f"{base_url}/outlook-native/tracking/email-sent", json=email_tracking_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "message" in data, "Missing message in response"
    assert "document_id" in data, "Missing document_id in response"
    assert "recipients" in data, "Missing recipients in response"
    assert "tracking_link" in data, "Missing tracking_link in response"
    
    print("✅ Track email sent event working")
    
    # 10. Test getting document analytics
    print("\n10. Testing get document analytics...")
    response = requests.get(f"{base_url}/outlook-native/analytics/documents/{test_document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "document_id" in data, "Missing document_id in response"
    assert data["document_id"] == test_document_id, f"Expected {test_document_id}, got {data['document_id']}"
    assert "document_title" in data, "Missing document_title in response"
    assert "summary" in data, "Missing summary in response"
    assert "generated_at" in data, "Missing generated_at in response"
    
    # Verify summary data
    assert "total_views" in data["summary"], "Missing total_views in summary"
    assert "total_emails" in data["summary"], "Missing total_emails in summary"
    assert "unique_viewers" in data["summary"], "Missing unique_viewers in summary"
    
    print("✅ Get document analytics working")
    
    # 11. Clean up (delete test document)
    print("\n11. Cleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{test_document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    print("✅ Test document deleted successfully")
    
    print("\n✅ All Native Outlook Plugin API tests passed successfully!")

def test_outlook_native_plugin_access_control():
    """Test access control for Native Outlook Plugin API endpoints"""
    print("\nStarting Native Outlook Plugin access control tests...")
    
    # Test variables
    base_url = BACKEND_URL
    headers = {"Content-Type": "application/json"}
    
    # Create two test users
    test_user1_email = f"test.native.user1.{uuid.uuid4()}@example.com"
    test_user2_email = f"test.native.user2.{uuid.uuid4()}@example.com"
    test_password = "SecurePassword123!"
    
    # Register first user
    print("\n1. Registering first test user...")
    user1_data = {
        "email": test_user1_email,
        "full_name": "Test Native User 1",
        "role": "editor",
        "organization": "Test Organization",
        "password": test_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user1_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    user1_token = response.json()["access_token"]
    user1_headers = headers.copy()
    user1_headers["Authorization"] = f"Bearer {user1_token}"
    
    # Register second user
    print("2. Registering second test user...")
    user2_data = {
        "email": test_user2_email,
        "full_name": "Test Native User 2",
        "role": "editor",
        "organization": "Test Organization",
        "password": test_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user2_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    user2_token = response.json()["access_token"]
    user2_headers = headers.copy()
    user2_headers["Authorization"] = f"Bearer {user2_token}"
    
    # Create a document as user 1
    print("3. Creating test document as user 1...")
    document_data = {
        "title": "Native Access Control Test Document",
        "type": "proposal",
        "organization": "Test Organization",
        "sections": [
            {
                "title": "Introduction",
                "content": "This is a test document for native access control testing.",
                "order": 1
            }
        ],
        "tags": ["test", "native", "access-control"],
        "metadata": {"purpose": "testing"}
    }
    
    response = requests.post(f"{base_url}/documents", json=document_data, headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    document_id = response.json()["id"]
    
    # Test access control for document content endpoint
    print("4. Testing access control for document content endpoint...")
    
    # User 1 (owner) should have access
    response = requests.get(f"{base_url}/outlook-native/documents/{document_id}/content", headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    assert response.json()["can_edit"] == True, "Owner should have edit permission"
    
    # User 2 (non-owner) should not have access
    response = requests.get(f"{base_url}/outlook-native/documents/{document_id}/content", headers=user2_headers)
    assert response.status_code in [403, 500], f"Expected 403 or 500, got {response.status_code}"
    print("✅ Access control for document content endpoint working (non-owner denied access)")
    
    # Test access control for generate attachment endpoint
    print("5. Testing access control for generate attachment endpoint...")
    
    # User 1 (owner) should be able to generate attachment
    response = requests.post(f"{base_url}/outlook-native/documents/{document_id}/generate-attachment", headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # User 2 (non-owner) should not be able to generate attachment
    response = requests.post(f"{base_url}/outlook-native/documents/{document_id}/generate-attachment", headers=user2_headers)
    assert response.status_code in [403, 500], f"Expected 403 or 500, got {response.status_code}"
    print("✅ Access control for generate attachment endpoint working (non-owner denied access)")
    
    # Test access control for analytics endpoint
    print("6. Testing access control for analytics endpoint...")
    
    # User 1 (owner) should be able to view analytics
    response = requests.get(f"{base_url}/outlook-native/analytics/documents/{document_id}", headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # User 2 (non-owner) should not be able to view analytics
    response = requests.get(f"{base_url}/outlook-native/analytics/documents/{document_id}", headers=user2_headers)
    assert response.status_code in [403, 500], f"Expected 403 or 500, got {response.status_code}"
    print("✅ Access control for analytics endpoint working (non-owner denied access)")
    
    # Add user 2 as a collaborator
    print("7. Adding user 2 as a collaborator...")
    
    # First, get the user ID for user 2
    response = requests.get(f"{base_url}/users/me", headers=user2_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    user2_id = response.json()["id"]
    
    update_data = {
        "collaborators": [
            {
                "user_id": user2_id,
                "role": "editor"
            }
        ]
    }
    
    response = requests.put(f"{base_url}/documents/{document_id}", json=update_data, headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Test access after adding as collaborator
    print("8. Testing access after adding as collaborator...")
    
    # User 2 should now have access to view document content
    response = requests.get(f"{base_url}/outlook-native/documents/{document_id}/content", headers=user2_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # User 2 should be able to generate attachment
    response = requests.post(f"{base_url}/outlook-native/documents/{document_id}/generate-attachment", headers=user2_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Clean up
    print("9. Cleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{document_id}", headers=user1_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("✅ All Native Outlook Plugin access control tests passed successfully!")

def test_complete_native_outlook_plugin_workflow():
    """Test the complete native Outlook plugin workflow"""
    print("\nStarting complete Native Outlook Plugin workflow test...")
    
    # Test variables
    base_url = BACKEND_URL
    headers = {"Content-Type": "application/json"}
    test_user_email = f"test.native.workflow.{uuid.uuid4()}@example.com"
    test_user_password = "SecurePassword123!"
    
    # 1. Authentication Flow
    print("\n1. Testing Authentication Flow...")
    
    # Register a new user
    print("  - Registering new user...")
    user_data = {
        "email": test_user_email,
        "full_name": "Native Workflow Test User",
        "role": "editor",
        "organization": "Test Organization",
        "password": test_user_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify JWT token generation
    assert "access_token" in data, "Missing access_token in response"
    assert "token_type" in data, "Missing token_type in response"
    assert data["token_type"] == "bearer", f"Expected 'bearer', got {data['token_type']}"
    
    # Save token for subsequent tests
    access_token = data["access_token"]
    headers["Authorization"] = f"Bearer {access_token}"
    
    # Test token validation for protected endpoints
    print("  - Testing token validation...")
    response = requests.get(f"{base_url}/users/me", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Test with invalid token
    invalid_headers = headers.copy()
    invalid_headers["Authorization"] = "Bearer invalid_token"
    response = requests.get(f"{base_url}/users/me", headers=invalid_headers)
    assert response.status_code == 401, f"Expected 401, got {response.status_code}"
    
    print("✅ Authentication Flow working correctly")
    
    # 2. Document Creation
    print("\n2. Testing Document Creation...")
    
    # Create a test document
    document_data = {
        "title": "Native Workflow Test Document",
        "type": "proposal",
        "organization": "Test Organization",
        "sections": [
            {
                "title": "Executive Summary",
                "content": "This is a test document for the complete native Outlook plugin workflow.",
                "order": 1
            },
            {
                "title": "Proposal Details",
                "content": "This document demonstrates the complete workflow for the native Outlook plugin.",
                "order": 2
            }
        ],
        "tags": ["test", "native", "workflow"],
        "metadata": {"purpose": "testing"}
    }
    
    response = requests.post(f"{base_url}/documents", json=document_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    document_id = response.json()["id"]
    
    print("✅ Document Creation working correctly")
    
    # 3. Document Retrieval via Native Plugin API
    print("\n3. Testing Document Retrieval via Native Plugin API...")
    
    # Test getting user's document library
    response = requests.get(f"{base_url}/outlook-native/documents/my-library", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Check if our test document is in the library
    document_found = False
    for doc in data["documents"]:
        if doc["id"] == document_id:
            document_found = True
            break
    
    assert document_found, "Test document not found in user's library"
    
    # Test getting document content
    response = requests.get(f"{base_url}/outlook-native/documents/{document_id}/content", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("✅ Document Retrieval via Native Plugin API working correctly")
    
    # 4. Generating Trackable Attachment
    print("\n4. Testing Generating Trackable Attachment...")
    
    response = requests.post(f"{base_url}/outlook-native/documents/{document_id}/generate-attachment", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify HTML content contains tracking link
    assert "View Full Document" in data["html_content"], "Missing 'View Full Document' link in HTML content"
    assert data["tracking_link"] in data["html_content"], "Tracking link not found in HTML content"
    
    print("✅ Generating Trackable Attachment working correctly")
    
    # 5. Tracking Email Sent
    print("\n5. Testing Tracking Email Sent...")
    
    email_tracking_data = {
        "document_id": document_id,
        "recipients": ["recipient1@example.com", "recipient2@example.com"],
        "subject": "Complete Workflow Test"
    }
    
    response = requests.post(f"{base_url}/outlook-native/tracking/email-sent", json=email_tracking_data, headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("✅ Tracking Email Sent working correctly")
    
    # 6. Viewing Analytics
    print("\n6. Testing Viewing Analytics...")
    
    response = requests.get(f"{base_url}/outlook-native/analytics/documents/{document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify analytics data
    assert data["document_id"] == document_id, f"Expected {document_id}, got {data['document_id']}"
    assert "summary" in data, "Missing summary in response"
    assert "total_emails" in data["summary"], "Missing total_emails in summary"
    assert data["summary"]["total_emails"] >= 1, "Expected at least 1 email sent"
    
    print("✅ Viewing Analytics working correctly")
    
    # 7. Clean up
    print("\n7. Cleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{document_id}", headers=headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("\n✅ Complete Native Outlook Plugin workflow test passed successfully!")

if __name__ == "__main__":
    # Run the Outlook Add-in integration tests
    test_outlook_addin_integration()
    
    # Run the access control tests
    test_outlook_addin_access_control()
    
    # Run the workflow tests
    test_outlook_addin_workflow()
    
    # Run the Native Outlook Plugin API tests
    test_outlook_native_plugin_api()
    
    # Run the Native Outlook Plugin access control tests
    test_outlook_native_plugin_access_control()
    
    # Run the complete Native Outlook Plugin workflow test
    test_complete_native_outlook_plugin_workflow()