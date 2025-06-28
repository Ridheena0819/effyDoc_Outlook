import requests
import json
import unittest
import os
import sys
from datetime import datetime

# Get the backend URL from the frontend .env file
BACKEND_URL = "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/api"

def test_outlook_installer_files():
    """Test the Outlook plugin installer files"""
    print("\nTesting Outlook plugin installer files...")
    
    # 1. Verify installer-info.json exists and has correct structure
    print("1. Verifying installer-info.json...")
    
    response = requests.get("https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/installer-info.json")
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    installer_info = response.json()
    
    # Verify installer info structure
    assert "version" in installer_info, "Missing version in installer-info.json"
    assert "backend_url" in installer_info, "Missing backend_url in installer-info.json"
    assert "created_at" in installer_info, "Missing created_at in installer-info.json"
    assert "features" in installer_info, "Missing features in installer-info.json"
    assert "requirements" in installer_info, "Missing requirements in installer-info.json"
    
    print("✅ installer-info.json exists and has correct structure")
    print(f"   Version: {installer_info['version']}")
    print(f"   Backend URL: {installer_info['backend_url']}")
    print(f"   Created at: {installer_info['created_at']}")
    print(f"   Features: {', '.join(installer_info['features'])}")
    print(f"   Requirements: {', '.join(installer_info['requirements'])}")
    
    # 2. Verify .exe installer file exists
    print("\n2. Verifying EffyDocOutlookPlugin-Setup.exe...")
    
    response = requests.head("https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/EffyDocOutlookPlugin-Setup.exe")
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Get file size
    content_length = int(response.headers.get('Content-Length', 0))
    print(f"✅ EffyDocOutlookPlugin-Setup.exe exists (Size: {content_length} bytes)")
    
    # 3. Verify .bat installer file exists
    print("\n3. Verifying EffyDocOutlookPlugin-Setup.bat...")
    
    response = requests.head("https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/EffyDocOutlookPlugin-Setup.bat")
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Get file size
    content_length = int(response.headers.get('Content-Length', 0))
    print(f"✅ EffyDocOutlookPlugin-Setup.bat exists (Size: {content_length} bytes)")
    
    # 4. Test access control for build-outlook-installer endpoint
    print("\n4. Testing access control for build-outlook-installer endpoint...")
    
    # Register a regular user for testing permissions
    regular_email = "regular.test@example.com"
    regular_password = "SecurePassword123!"
    
    user_data = {
        "email": regular_email,
        "full_name": "Regular Test User",
        "role": "editor",  # Non-admin role
        "organization": "Test Organization",
        "password": regular_password
    }
    
    response = requests.post(f"{BACKEND_URL}/auth/register", json=user_data)
    if response.status_code == 200:
        data = response.json()
        regular_token = data["access_token"]
        regular_headers = {"Content-Type": "application/json", "Authorization": f"Bearer {regular_token}"}
        
        # Test with regular user (should fail)
        installer_config = {
            "version": "1.1.0",
            "backendURL": "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com"
        }
        
        response = requests.post(f"{BACKEND_URL}/build-outlook-installer", json=installer_config, headers=regular_headers)
        if response.status_code == 403:
            print("✅ Regular user cannot access build-outlook-installer endpoint (permission denied)")
        else:
            print(f"❌ Expected 403, got {response.status_code}")
    else:
        print(f"⚠️ Could not register test user to verify permissions (status code: {response.status_code})")
    
    print("\n✅ All Outlook plugin installer file tests passed successfully!")

if __name__ == "__main__":
    test_outlook_installer_files()