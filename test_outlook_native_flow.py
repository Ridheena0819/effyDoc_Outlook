import requests
import json
import uuid
from datetime import datetime

# Get the backend URL from the frontend .env file
BACKEND_URL = "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/api"

def test_complete_native_outlook_plugin_flow():
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
    print(f"    Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"    Error: {response.text}")
        return
    
    data = response.json()
    
    # Verify JWT token generation
    if "access_token" not in data:
        print("    Error: No access token in response")
        return
    
    # Save token for subsequent tests
    access_token = data["access_token"]
    headers["Authorization"] = f"Bearer {access_token}"
    print("    ✅ User registration successful")
    
    # Test token validation for protected endpoints
    print("  - Testing token validation...")
    response = requests.get(f"{base_url}/users/me", headers=headers)
    print(f"    Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"    Error: {response.text}")
        return
    
    print("    ✅ Token validation successful")
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
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    document_id = response.json()["id"]
    print(f"  ✅ Document created with ID: {document_id}")
    
    # 3. Document Retrieval via Native Plugin API
    print("\n3. Testing Document Retrieval via Native Plugin API...")
    
    # Test getting user's document library
    response = requests.get(f"{base_url}/outlook-native/documents/my-library", headers=headers)
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    data = response.json()
    
    # Check if our test document is in the library
    document_found = False
    for doc in data["documents"]:
        if doc["id"] == document_id:
            document_found = True
            break
    
    if not document_found:
        print("  Error: Test document not found in user's library")
        return
    
    print("  ✅ Document found in user's library")
    
    # Test getting document content
    response = requests.get(f"{base_url}/outlook-native/documents/{document_id}/content", headers=headers)
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    print("  ✅ Document content retrieved successfully")
    
    # 4. Generating Trackable Attachment
    print("\n4. Testing Generating Trackable Attachment...")
    
    response = requests.post(f"{base_url}/outlook-native/documents/{document_id}/generate-attachment", headers=headers)
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    data = response.json()
    
    # Verify HTML content contains tracking link
    if "View Full Document" not in data["html_content"]:
        print("  Error: Missing 'View Full Document' link in HTML content")
        return
    
    if data["tracking_link"] not in data["html_content"]:
        print("  Error: Tracking link not found in HTML content")
        return
    
    print("  ✅ Trackable attachment generated successfully")
    
    # 5. Tracking Email Sent
    print("\n5. Testing Tracking Email Sent...")
    
    email_tracking_data = {
        "document_id": document_id,
        "recipients": ["recipient1@example.com", "recipient2@example.com"],
        "subject": "Complete Workflow Test"
    }
    
    response = requests.post(f"{base_url}/outlook-native/tracking/email-sent", json=email_tracking_data, headers=headers)
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    print("  ✅ Email sent tracking successful")
    
    # 6. Viewing Analytics
    print("\n6. Testing Viewing Analytics...")
    
    response = requests.get(f"{base_url}/outlook-native/analytics/documents/{document_id}", headers=headers)
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    data = response.json()
    
    # Verify analytics data
    if data["document_id"] != document_id:
        print(f"  Error: Expected document_id {document_id}, got {data['document_id']}")
        return
    
    if "summary" not in data:
        print("  Error: Missing summary in response")
        return
    
    if "total_emails" not in data["summary"]:
        print("  Error: Missing total_emails in summary")
        return
    
    if data["summary"]["total_emails"] < 1:
        print(f"  Error: Expected at least 1 email sent, got {data['summary']['total_emails']}")
        return
    
    print("  ✅ Analytics retrieved successfully")
    
    # 7. Clean up
    print("\n7. Cleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{document_id}", headers=headers)
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    print("  ✅ Test document deleted successfully")
    
    print("\n✅ Complete Native Outlook Plugin workflow test passed successfully!")

if __name__ == "__main__":
    test_complete_native_outlook_plugin_flow()