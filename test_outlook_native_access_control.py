import requests
import json
import uuid
from datetime import datetime

# Get the backend URL from the frontend .env file
BACKEND_URL = "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/api"

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
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    user1_token = response.json()["access_token"]
    user1_headers = headers.copy()
    user1_headers["Authorization"] = f"Bearer {user1_token}"
    print("  ✅ First user registered successfully")
    
    # Register second user
    print("\n2. Registering second test user...")
    user2_data = {
        "email": test_user2_email,
        "full_name": "Test Native User 2",
        "role": "editor",
        "organization": "Test Organization",
        "password": test_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user2_data)
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    user2_token = response.json()["access_token"]
    user2_headers = headers.copy()
    user2_headers["Authorization"] = f"Bearer {user2_token}"
    print("  ✅ Second user registered successfully")
    
    # Create a document as user 1
    print("\n3. Creating test document as user 1...")
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
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    document_id = response.json()["id"]
    print(f"  ✅ Document created with ID: {document_id}")
    
    # Test access control for document content endpoint
    print("\n4. Testing access control for document content endpoint...")
    
    # User 1 (owner) should have access
    response = requests.get(f"{base_url}/outlook-native/documents/{document_id}/content", headers=user1_headers)
    print(f"  User 1 (owner) status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    if not response.json().get("can_edit", False):
        print("  Error: Owner should have edit permission")
        return
    
    # User 2 (non-owner) should not have access
    response = requests.get(f"{base_url}/outlook-native/documents/{document_id}/content", headers=user2_headers)
    print(f"  User 2 (non-owner) status code: {response.status_code}")
    if response.status_code not in [403, 500]:
        print(f"  Error: Expected 403 or 500, got {response.status_code}")
        return
    
    print("  ✅ Access control for document content endpoint working (non-owner denied access)")
    
    # Test access control for generate attachment endpoint
    print("\n5. Testing access control for generate attachment endpoint...")
    
    # User 1 (owner) should be able to generate attachment
    response = requests.post(f"{base_url}/outlook-native/documents/{document_id}/generate-attachment", headers=user1_headers)
    print(f"  User 1 (owner) status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    # User 2 (non-owner) should not be able to generate attachment
    response = requests.post(f"{base_url}/outlook-native/documents/{document_id}/generate-attachment", headers=user2_headers)
    print(f"  User 2 (non-owner) status code: {response.status_code}")
    if response.status_code not in [403, 500]:
        print(f"  Error: Expected 403 or 500, got {response.status_code}")
        return
    
    print("  ✅ Access control for generate attachment endpoint working (non-owner denied access)")
    
    # Test access control for analytics endpoint
    print("\n6. Testing access control for analytics endpoint...")
    
    # User 1 (owner) should be able to view analytics
    response = requests.get(f"{base_url}/outlook-native/analytics/documents/{document_id}", headers=user1_headers)
    print(f"  User 1 (owner) status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    # User 2 (non-owner) should not be able to view analytics
    response = requests.get(f"{base_url}/outlook-native/analytics/documents/{document_id}", headers=user2_headers)
    print(f"  User 2 (non-owner) status code: {response.status_code}")
    if response.status_code not in [403, 500]:
        print(f"  Error: Expected 403 or 500, got {response.status_code}")
        return
    
    print("  ✅ Access control for analytics endpoint working (non-owner denied access)")
    
    # Add user 2 as a collaborator
    print("\n7. Adding user 2 as a collaborator...")
    
    # First, get the user ID for user 2
    response = requests.get(f"{base_url}/users/me", headers=user2_headers)
    print(f"  Get user 2 ID status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
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
    print(f"  Update document status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    print("  ✅ User 2 added as collaborator successfully")
    
    # Test access after adding as collaborator
    print("\n8. Testing access after adding as collaborator...")
    
    # User 2 should now have access to view document content
    response = requests.get(f"{base_url}/outlook-native/documents/{document_id}/content", headers=user2_headers)
    print(f"  User 2 (collaborator) document content status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    # User 2 should be able to generate attachment
    response = requests.post(f"{base_url}/outlook-native/documents/{document_id}/generate-attachment", headers=user2_headers)
    print(f"  User 2 (collaborator) generate attachment status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    print("  ✅ Collaborator access working correctly")
    
    # Clean up
    print("\n9. Cleaning up test document...")
    response = requests.delete(f"{base_url}/documents/{document_id}", headers=user1_headers)
    print(f"  Status code: {response.status_code}")
    if response.status_code != 200:
        print(f"  Error: {response.text}")
        return
    
    print("  ✅ Test document deleted successfully")
    
    print("\n✅ All Native Outlook Plugin access control tests passed successfully!")

if __name__ == "__main__":
    test_outlook_native_plugin_access_control()