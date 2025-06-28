import requests

# Get the backend URL from the frontend .env file
BACKEND_URL = "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/api"

def test_status_endpoint():
    """Test the status endpoint for the native Outlook plugin"""
    print("Testing Native Outlook Plugin status endpoint...")
    response = requests.get(f"{BACKEND_URL}/outlook-native/status")
    print(f"Status code: {response.status_code}")
    if response.status_code == 200:
        print("Response content:", response.json())
    else:
        print("Error response:", response.text)

if __name__ == "__main__":
    test_status_endpoint()