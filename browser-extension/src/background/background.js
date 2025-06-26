// Background service worker for effyDOC browser extension
/* global chrome */
console.log('effyDOC Browser Extension: Background service worker loaded');

// Constants
const BACKEND_URL = 'https://f70e6bf0-40a7-454d-8960-1649b6f10c4c.preview.emergentagent.com';

// Extension installation handler
chrome.runtime.onInstalled.addListener((details) => {
  console.log('effyDOC Extension installed:', details);
  
  if (details.reason === 'install') {
    // Set default settings
    chrome.storage.local.set({
      effydoc_settings: {
        auto_track: true,
        notifications_enabled: true,
        last_sync: null
      }
    });
    
    // Open welcome page
    chrome.tabs.create({
      url: `${BACKEND_URL}/register?source=browser_extension`
    });
  }
});

// Handle messages from content scripts and popup
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  console.log('Background received message:', message);
  
  switch (message.type) {
    case 'GET_AUTH_STATUS':
      getAuthStatus().then(sendResponse);
      return true; // Keep message channel open for async response
      
    case 'LOGIN':
      handleLogin(message.credentials).then(sendResponse);
      return true;
      
    case 'LOGOUT':
      handleLogout().then(sendResponse);
      return true;
      
    case 'GET_DOCUMENTS':
      getDocuments(message.libraryType).then(sendResponse);
      return true;
      
    case 'TRACK_EMAIL_SENT':
      trackEmailSent(message.data).then(sendResponse);
      return true;
      
    case 'GET_LIVE_METRICS':
      getLiveMetrics(message.documentId).then(sendResponse);
      return true;
      
    case 'INJECT_SIDEBAR':
      injectSidebar(sender.tab.id);
      sendResponse({ success: true });
      break;
      
    default:
      console.warn('Unknown message type:', message.type);
      sendResponse({ error: 'Unknown message type' });
  }
});

// Authentication functions
async function getAuthStatus() {
  try {
    const result = await chrome.storage.local.get(['effydoc_token', 'effydoc_user']);
    
    if (!result.effydoc_token) {
      return { authenticated: false };
    }
    
    // Validate token with backend
    const response = await fetch(`${BACKEND_URL}/api/users/me`, {
      headers: {
        'Authorization': `Bearer ${result.effydoc_token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (response.ok) {
      const user = await response.json();
      return { 
        authenticated: true, 
        user: user,
        token: result.effydoc_token 
      };
    } else {
      // Token invalid, clear storage
      await chrome.storage.local.remove(['effydoc_token', 'effydoc_user']);
      return { authenticated: false };
    }
  } catch (error) {
    console.error('Auth status check failed:', error);
    return { authenticated: false, error: error.message };
  }
}

async function handleLogin(credentials) {
  try {
    const response = await fetch(`${BACKEND_URL}/api/auth/login`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(credentials)
    });
    
    if (response.ok) {
      const data = await response.json();
      
      // Store authentication data
      await chrome.storage.local.set({
        effydoc_token: data.access_token,
        effydoc_user: data.user
      });
      
      return { 
        success: true, 
        user: data.user,
        token: data.access_token 
      };
    } else {
      const error = await response.json();
      return { 
        success: false, 
        error: error.detail || 'Login failed' 
      };
    }
  } catch (error) {
    console.error('Login failed:', error);
    return { 
      success: false, 
      error: error.message 
    };
  }
}

async function handleLogout() {
  try {
    await chrome.storage.local.remove(['effydoc_token', 'effydoc_user']);
    return { success: true };
  } catch (error) {
    console.error('Logout failed:', error);
    return { success: false, error: error.message };
  }
}

// Document functions
async function getDocuments(libraryType = 'my-library') {
  try {
    const authStatus = await getAuthStatus();
    if (!authStatus.authenticated) {
      return { success: false, error: 'Not authenticated' };
    }
    
    const endpoint = libraryType === 'content-hub' 
      ? '/api/outlook/documents/content-hub'
      : '/api/outlook/documents/my-library';
    
    const response = await fetch(`${BACKEND_URL}${endpoint}`, {
      headers: {
        'Authorization': `Bearer ${authStatus.token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (response.ok) {
      const data = await response.json();
      return { success: true, documents: data.documents };
    } else {
      const error = await response.json();
      return { success: false, error: error.detail };
    }
  } catch (error) {
    console.error('Get documents failed:', error);
    return { success: false, error: error.message };
  }
}

// Tracking functions
async function trackEmailSent(trackingData) {
  try {
    const authStatus = await getAuthStatus();
    if (!authStatus.authenticated) {
      return { success: false, error: 'Not authenticated' };
    }
    
    const response = await fetch(`${BACKEND_URL}/api/outlook/tracking/email-sent`, {
      method: 'POST',
      headers: {
        'Authorization': `Bearer ${authStatus.token}`,
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(trackingData)
    });
    
    if (response.ok) {
      const data = await response.json();
      return { success: true, data: data };
    } else {
      const error = await response.json();
      return { success: false, error: error.detail };
    }
  } catch (error) {
    console.error('Track email failed:', error);
    return { success: false, error: error.message };
  }
}

async function getLiveMetrics(documentId) {
  try {
    const authStatus = await getAuthStatus();
    if (!authStatus.authenticated) {
      return { success: false, error: 'Not authenticated' };
    }
    
    const response = await fetch(`${BACKEND_URL}/api/outlook/tracking/live-metrics/${documentId}`, {
      headers: {
        'Authorization': `Bearer ${authStatus.token}`,
        'Content-Type': 'application/json'
      }
    });
    
    if (response.ok) {
      const data = await response.json();
      return { success: true, metrics: data };
    } else {
      const error = await response.json();
      return { success: false, error: error.detail };
    }
  } catch (error) {
    console.error('Get live metrics failed:', error);
    return { success: false, error: error.message };
  }
}

// Sidebar injection
async function injectSidebar(tabId) {
  try {
    await chrome.scripting.executeScript({
      target: { tabId: tabId },
      function: () => {
        // This function runs in the context of the web page
        window.postMessage({ 
          type: 'EFFYDOC_SHOW_SIDEBAR',
          source: 'browser_extension' 
        }, '*');
      }
    });
  } catch (error) {
    console.error('Failed to inject sidebar:', error);
  }
}

// Periodic sync
setInterval(async () => {
  const authStatus = await getAuthStatus();
  if (authStatus.authenticated) {
    // Update badge with unread notifications count
    try {
      chrome.action.setBadgeText({ text: '' });
      chrome.action.setBadgeBackgroundColor({ color: '#2563eb' });
    } catch (error) {
      console.error('Badge update failed:', error);
    }
  }
}, 30000); // Every 30 seconds