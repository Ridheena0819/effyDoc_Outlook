import requests
import json
import unittest
import os
import sys
from datetime import datetime

# Get the backend URL from the frontend .env file
BACKEND_URL = "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/api"

def test_build_outlook_installer():
    """Test the build-outlook-installer endpoint"""
    print("\nTesting build-outlook-installer endpoint...")
    
    # Test variables
    base_url = BACKEND_URL
    headers = {"Content-Type": "application/json"}
    
    # 1. Register an admin user for testing
    print("1. Registering admin user...")
    admin_email = "admin.test@example.com"
    admin_password = "SecurePassword123!"
    
    user_data = {
        "email": admin_email,
        "full_name": "Admin Test User",
        "role": "admin",  # Admin role required
        "organization": "Test Organization",
        "password": admin_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Save token for subsequent tests
    admin_token = data["access_token"]
    admin_headers = headers.copy()
    admin_headers["Authorization"] = f"Bearer {admin_token}"
    print("✅ Admin user registration successful")
    
    # 2. Register a regular user for testing permissions
    print("2. Registering regular user...")
    regular_email = "regular.test@example.com"
    regular_password = "SecurePassword123!"
    
    user_data = {
        "email": regular_email,
        "full_name": "Regular Test User",
        "role": "editor",  # Non-admin role
        "organization": "Test Organization",
        "password": regular_password
    }
    
    response = requests.post(f"{base_url}/auth/register", json=user_data)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Save token for subsequent tests
    regular_token = data["access_token"]
    regular_headers = headers.copy()
    regular_headers["Authorization"] = f"Bearer {regular_token}"
    print("✅ Regular user registration successful")
    
    # 3. Test build-outlook-installer endpoint with admin user
    print("3. Testing build-outlook-installer with admin user...")
    installer_config = {
        "version": "1.1.0",
        "backendURL": "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com"
    }
    
    response = requests.post(f"{base_url}/build-outlook-installer", json=installer_config, headers=admin_headers)
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    data = response.json()
    
    # Verify response structure
    assert "success" in data, "Missing success in response"
    assert data["success"] == True, f"Expected success=True, got {data['success']}"
    assert "message" in data, "Missing message in response"
    assert "installer_path" in data, "Missing installer_path in response"
    assert "version" in data, "Missing version in response"
    assert data["version"] == "1.1.0", f"Expected version=1.1.0, got {data['version']}"
    
    print("✅ Admin user can build installer successfully")
    
    # 4. Test build-outlook-installer endpoint with regular user (should fail)
    print("4. Testing build-outlook-installer with regular user (should fail)...")
    
    response = requests.post(f"{base_url}/build-outlook-installer", json=installer_config, headers=regular_headers)
    assert response.status_code == 403, f"Expected 403, got {response.status_code}"
    
    print("✅ Regular user cannot build installer (permission denied)")
    
    # 5. Test with invalid parameters
    print("5. Testing with invalid parameters...")
    
    # Missing version
    invalid_config = {
        "backendURL": "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com"
    }
    
    response = requests.post(f"{base_url}/build-outlook-installer", json=invalid_config, headers=admin_headers)
    # This should still work as version has a default value
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Missing backendURL
    invalid_config = {
        "version": "1.1.0"
    }
    
    response = requests.post(f"{base_url}/build-outlook-installer", json=invalid_config, headers=admin_headers)
    # This should still work as backendURL has a default value from environment
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("✅ Endpoint handles invalid parameters correctly")
    
    # 6. Verify installer files exist
    print("6. Verifying installer files exist...")
    
    # Check installer-info.json
    response = requests.get("https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/installer-info.json")
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    installer_info = response.json()
    
    # Verify installer info structure
    assert "version" in installer_info, "Missing version in installer-info.json"
    assert "backend_url" in installer_info, "Missing backend_url in installer-info.json"
    assert "created_at" in installer_info, "Missing created_at in installer-info.json"
    assert "features" in installer_info, "Missing features in installer-info.json"
    assert "requirements" in installer_info, "Missing requirements in installer-info.json"
    
    # Check .exe installer file
    response = requests.head("https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/EffyDocOutlookPlugin-Setup.exe")
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    # Check .bat installer file
    response = requests.head("https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com/EffyDocOutlookPlugin-Setup.bat")
    assert response.status_code == 200, f"Expected 200, got {response.status_code}"
    
    print("✅ All installer files exist and are accessible")
    
    print("\n✅ All build-outlook-installer tests passed successfully!")

if __name__ == "__main__":
    test_build_outlook_installer()