// Content script for Outlook Web App integration
/* global chrome */

console.log('effyDOC Content Script: Loaded for Outlook Web App');

// Check if we're on an Outlook page
const isOutlookPage = () => {
  return window.location.hostname.includes('outlook.') && 
         (window.location.hostname.includes('office.com') || 
          window.location.hostname.includes('live.com') ||
          window.location.hostname.includes('office365.com'));
};

// Global variables
let effyDocSidebar = null;
let isInjected = false;
let currentUser = null;
let selectedDocuments = [];

// Initialize extension
function initializeExtension() {
  if (!isOutlookPage()) {
    console.log('Not on Outlook page, skipping initialization');
    return;
  }
  
  console.log('Initializing effyDOC extension for Outlook');
  
  // Wait for Outlook to load
  setTimeout(() => {
    createSidebar();
    setupOutlookObservers();
    addExtensionButton();
  }, 2000);
}

// Create the effyDOC sidebar
function createSidebar() {
  if (effyDocSidebar) {
    return; // Already created
  }
  
  // Create sidebar container
  effyDocSidebar = document.createElement('div');
  effyDocSidebar.id = 'effydoc-sidebar';
  effyDocSidebar.className = 'effydoc-sidebar-hidden';
  
  effyDocSidebar.innerHTML = `
    <div class="effydoc-sidebar-header">
      <div class="effydoc-logo">
        <span class="effydoc-icon">📊</span>
        <span class="effydoc-title">effyDOC</span>
      </div>
      <button class="effydoc-close-btn" onclick="window.effyDoc.toggleSidebar()">×</button>
    </div>
    
    <div class="effydoc-sidebar-content">
      <div id="effydoc-auth-section" class="effydoc-section">
        <div class="effydoc-login-form">
          <h3>Sign in to effyDOC</h3>
          <input type="email" id="effydoc-email" placeholder="Email" class="effydoc-input">
          <input type="password" id="effydoc-password" placeholder="Password" class="effydoc-input">
          <button id="effydoc-login-btn" class="effydoc-btn effydoc-btn-primary">Sign In</button>
          <div id="effydoc-auth-error" class="effydoc-error"></div>
        </div>
      </div>
      
      <div id="effydoc-main-section" class="effydoc-section effydoc-hidden">
        <div class="effydoc-tabs">
          <button class="effydoc-tab effydoc-tab-active" data-tab="library">My Library</button>
          <button class="effydoc-tab" data-tab="hub">Content Hub</button>
          <button class="effydoc-tab" data-tab="tracking">Live Tracking</button>
        </div>
        
        <div class="effydoc-tab-content">
          <div id="effydoc-library-tab" class="effydoc-tab-panel effydoc-tab-panel-active">
            <div class="effydoc-search">
              <input type="text" placeholder="Search documents..." class="effydoc-search-input">
            </div>
            <div id="effydoc-library-list" class="effydoc-document-list">
              <div class="effydoc-loading">Loading your documents...</div>
            </div>
          </div>
          
          <div id="effydoc-hub-tab" class="effydoc-tab-panel">
            <div id="effydoc-hub-list" class="effydoc-document-list">
              <div class="effydoc-loading">Loading shared content...</div>
            </div>
          </div>
          
          <div id="effydoc-tracking-tab" class="effydoc-tab-panel">
            <div id="effydoc-tracking-content">
              <div class="effydoc-no-selection">
                Select a document from your library to see live tracking data.
              </div>
            </div>
          </div>
        </div>
        
        <div class="effydoc-sidebar-footer">
          <button id="effydoc-logout-btn" class="effydoc-btn effydoc-btn-secondary">Logout</button>
        </div>
      </div>
    </div>
  `;
  
  // Inject sidebar into page
  document.body.appendChild(effyDocSidebar);
  
  // Setup event listeners
  setupSidebarEvents();
  
  console.log('effyDOC sidebar created');
}

// Add extension button to Outlook toolbar
function addExtensionButton() {
  // Try to find Outlook's toolbar
  const toolbar = document.querySelector('[role="toolbar"]') || 
                  document.querySelector('.ms-CommandBar') ||
                  document.querySelector('[data-app-section="CommandBar"]');
  
  if (!toolbar) {
    console.log('Could not find Outlook toolbar, trying again later');
    setTimeout(addExtensionButton, 3000);
    return;
  }
  
  // Check if button already exists
  if (document.getElementById('effydoc-toolbar-btn')) {
    return;
  }
  
  // Create extension button
  const button = document.createElement('button');
  button.id = 'effydoc-toolbar-btn';
  button.className = 'effydoc-toolbar-button';
  button.innerHTML = `
    <span class="effydoc-toolbar-icon">📊</span>
    <span class="effydoc-toolbar-text">effyDOC</span>
  `;
  button.title = 'Open effyDOC Document Tracker';
  button.onclick = () => window.effyDoc.toggleSidebar();
  
  // Insert button into toolbar
  toolbar.appendChild(button);
  
  console.log('effyDOC toolbar button added');
}

// Setup sidebar event listeners
function setupSidebarEvents() {
  // Login form
  const loginBtn = document.getElementById('effydoc-login-btn');
  const emailInput = document.getElementById('effydoc-email');
  const passwordInput = document.getElementById('effydoc-password');
  
  if (loginBtn) {
    loginBtn.addEventListener('click', handleLogin);
  }
  
  if (emailInput && passwordInput) {
    [emailInput, passwordInput].forEach(input => {
      input.addEventListener('keypress', (e) => {
        if (e.key === 'Enter') {
          handleLogin();
        }
      });
    });
  }
  
  // Tab switching
  document.querySelectorAll('.effydoc-tab').forEach(tab => {
    tab.addEventListener('click', (e) => {
      const tabName = e.target.getAttribute('data-tab');
      switchTab(tabName);
    });
  });
  
  // Logout
  const logoutBtn = document.getElementById('effydoc-logout-btn');
  if (logoutBtn) {
    logoutBtn.addEventListener('click', handleLogout);
  }
}

// Handle login
async function handleLogin() {
  const email = document.getElementById('effydoc-email').value;
  const password = document.getElementById('effydoc-password').value;
  const errorDiv = document.getElementById('effydoc-auth-error');
  const loginBtn = document.getElementById('effydoc-login-btn');
  
  if (!email || !password) {
    showError('Please enter both email and password');
    return;
  }
  
  loginBtn.textContent = 'Signing in...';
  loginBtn.disabled = true;
  errorDiv.textContent = '';
  
  try {
    const response = await chrome.runtime.sendMessage({
      type: 'LOGIN',
      credentials: { email, password }
    });
    
    if (response.success) {
      currentUser = response.user;
      showMainInterface();
      loadDocuments();
    } else {
      showError(response.error || 'Login failed');
    }
  } catch (error) {
    showError('Connection error. Please try again.');
  }
  
  loginBtn.textContent = 'Sign In';
  loginBtn.disabled = false;
}

// Handle logout
async function handleLogout() {
  try {
    await chrome.runtime.sendMessage({ type: 'LOGOUT' });
    currentUser = null;
    showLoginInterface();
  } catch (error) {
    console.error('Logout failed:', error);
  }
}

// Show error message
function showError(message) {
  const errorDiv = document.getElementById('effydoc-auth-error');
  if (errorDiv) {
    errorDiv.textContent = message;
  }
}

// Show main interface after login
function showMainInterface() {
  document.getElementById('effydoc-auth-section').classList.add('effydoc-hidden');
  document.getElementById('effydoc-main-section').classList.remove('effydoc-hidden');
}

// Show login interface
function showLoginInterface() {
  document.getElementById('effydoc-auth-section').classList.remove('effydoc-hidden');
  document.getElementById('effydoc-main-section').classList.add('effydoc-hidden');
  
  // Clear form
  document.getElementById('effydoc-email').value = '';
  document.getElementById('effydoc-password').value = '';
  document.getElementById('effydoc-auth-error').textContent = '';
}

// Switch tabs
function switchTab(tabName) {
  // Update tab buttons
  document.querySelectorAll('.effydoc-tab').forEach(tab => {
    tab.classList.remove('effydoc-tab-active');
  });
  document.querySelector(`[data-tab="${tabName}"]`).classList.add('effydoc-tab-active');
  
  // Update tab panels
  document.querySelectorAll('.effydoc-tab-panel').forEach(panel => {
    panel.classList.remove('effydoc-tab-panel-active');
  });
  document.getElementById(`effydoc-${tabName}-tab`).classList.add('effydoc-tab-panel-active');
  
  // Load content for specific tabs
  if (tabName === 'library') {
    loadDocuments('my-library');
  } else if (tabName === 'hub') {
    loadDocuments('content-hub');
  }
}

// Load documents
async function loadDocuments(libraryType = 'my-library') {
  const listContainer = libraryType === 'content-hub' 
    ? document.getElementById('effydoc-hub-list')
    : document.getElementById('effydoc-library-list');
  
  if (!listContainer) return;
  
  listContainer.innerHTML = '<div class="effydoc-loading">Loading documents...</div>';
  
  try {
    const response = await chrome.runtime.sendMessage({
      type: 'GET_DOCUMENTS',
      libraryType: libraryType
    });
    
    if (response.success) {
      displayDocuments(response.documents, listContainer, libraryType);
    } else {
      listContainer.innerHTML = `<div class="effydoc-error">Failed to load documents: ${response.error}</div>`;
    }
  } catch (error) {
    listContainer.innerHTML = '<div class="effydoc-error">Failed to load documents. Please try again.</div>';
  }
}

// Display documents in the list
function displayDocuments(documents, container, libraryType) {
  if (!documents || documents.length === 0) {
    container.innerHTML = '<div class="effydoc-empty">No documents found.</div>';
    return;
  }
  
  const documentsHtml = documents.map(doc => `
    <div class="effydoc-document-item" data-doc-id="${doc.id}">
      <div class="effydoc-document-header">
        <h4 class="effydoc-document-title">${doc.title}</h4>
        <span class="effydoc-document-type">${doc.type}</span>
      </div>
      <div class="effydoc-document-meta">
        <span class="effydoc-document-pages">${doc.total_pages} pages</span>
        <span class="effydoc-document-date">${new Date(doc.created_at).toLocaleDateString()}</span>
      </div>
      <div class="effydoc-document-actions">
        <button class="effydoc-btn effydoc-btn-small" onclick="window.effyDoc.insertDocument('${doc.id}', '${doc.title}')">
          📎 Insert Trackable
        </button>
        <button class="effydoc-btn effydoc-btn-small effydoc-btn-secondary" onclick="window.effyDoc.viewTracking('${doc.id}')">
          📊 Track
        </button>
      </div>
    </div>
  `).join('');
  
  container.innerHTML = documentsHtml;
}

// Setup Outlook observers for email composition
function setupOutlookObservers() {
  // Watch for email composition areas
  const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      if (mutation.addedNodes) {
        mutation.addedNodes.forEach((node) => {
          if (node.nodeType === Node.ELEMENT_NODE) {
            // Look for email composition areas
            const composeAreas = node.querySelectorAll ? 
              node.querySelectorAll('[role="textbox"], [contenteditable="true"]') : [];
            
            composeAreas.forEach(area => {
              if (area.closest('.ms-CompositeHeader') || area.closest('[data-app-section="MailCompose"]')) {
                addComposeIntegration(area);
              }
            });
          }
        });
      }
    });
  });
  
  observer.observe(document.body, {
    childList: true,
    subtree: true
  });
}

// Add integration to email compose area
function addComposeIntegration(composeArea) {
  // Find the compose container
  const composeContainer = composeArea.closest('[role="main"]') || 
                          composeArea.closest('.ms-CompositeHeader') ||
                          composeArea.parentElement;
  
  if (!composeContainer || composeContainer.querySelector('.effydoc-compose-integration')) {
    return; // Already added or can't find container
  }
  
  // Create integration button
  const integrationBtn = document.createElement('button');
  integrationBtn.className = 'effydoc-compose-integration';
  integrationBtn.innerHTML = '📎 Add effyDOC Document';
  integrationBtn.onclick = () => window.effyDoc.showSidebar();
  
  // Insert button near the compose area
  composeContainer.appendChild(integrationBtn);
}

// Global window object for external calls
window.effyDoc = {
  toggleSidebar: () => {
    if (!effyDocSidebar) {
      createSidebar();
    }
    
    if (effyDocSidebar.classList.contains('effydoc-sidebar-visible')) {
      effyDocSidebar.classList.remove('effydoc-sidebar-visible');
      effyDocSidebar.classList.add('effydoc-sidebar-hidden');
    } else {
      effyDocSidebar.classList.remove('effydoc-sidebar-hidden');
      effyDocSidebar.classList.add('effydoc-sidebar-visible');
      
      // Check authentication status
      checkAuthStatus();
    }
  },
  
  showSidebar: () => {
    if (!effyDocSidebar) {
      createSidebar();
    }
    effyDocSidebar.classList.remove('effydoc-sidebar-hidden');
    effyDocSidebar.classList.add('effydoc-sidebar-visible');
    checkAuthStatus();
  },
  
  insertDocument: async (documentId, documentTitle) => {
    try {
      // Generate trackable link
      const response = await chrome.runtime.sendMessage({
        type: 'GENERATE_LINK',
        documentId: documentId
      });
      
      if (response.success) {
        insertTextIntoCompose(`📄 ${documentTitle}\n\nView document: ${response.link}\n\n`);
        
        // Track email sent
        chrome.runtime.sendMessage({
          type: 'TRACK_EMAIL_SENT',
          data: {
            document_id: documentId,
            recipients: [], // Will be filled by backend
            subject: 'Document Shared',
            email_body: `Document shared: ${documentTitle}`
          }
        });
      }
    } catch (error) {
      console.error('Insert document failed:', error);
    }
  },
  
  viewTracking: (documentId) => {
    switchTab('tracking');
    loadTrackingData(documentId);
  }
};

// Check authentication status
async function checkAuthStatus() {
  try {
    const response = await chrome.runtime.sendMessage({ type: 'GET_AUTH_STATUS' });
    
    if (response.authenticated) {
      currentUser = response.user;
      showMainInterface();
      loadDocuments();
    } else {
      showLoginInterface();
    }
  } catch (error) {
    console.error('Auth check failed:', error);
    showLoginInterface();
  }
}

// Insert text into Outlook compose area
function insertTextIntoCompose(text) {
  // Find the active compose area
  const composeArea = document.querySelector('[role="textbox"][contenteditable="true"]') ||
                     document.querySelector('.ms-TextField-field[contenteditable="true"]') ||
                     document.querySelector('[data-app-section="MailCompose"] [contenteditable="true"]');
  
  if (composeArea) {
    const currentContent = composeArea.innerHTML || composeArea.textContent || '';
    composeArea.innerHTML = text + currentContent;
    
    // Trigger change event
    composeArea.dispatchEvent(new Event('input', { bubbles: true }));
  }
}

// Load tracking data for a document
async function loadTrackingData(documentId) {
  const trackingContainer = document.getElementById('effydoc-tracking-content');
  if (!trackingContainer) return;
  
  trackingContainer.innerHTML = '<div class="effydoc-loading">Loading tracking data...</div>';
  
  try {
    const response = await chrome.runtime.sendMessage({
      type: 'GET_LIVE_METRICS',
      documentId: documentId
    });
    
    if (response.success) {
      displayTrackingData(response.metrics, trackingContainer);
    } else {
      trackingContainer.innerHTML = `<div class="effydoc-error">Failed to load tracking data: ${response.error}</div>`;
    }
  } catch (error) {
    trackingContainer.innerHTML = '<div class="effydoc-error">Failed to load tracking data.</div>';
  }
}

// Display tracking data
function displayTrackingData(metrics, container) {
  const currentReaders = metrics.current_readers || [];
  const todayStats = metrics.today_stats || {};
  const recentActivity = metrics.recent_activity || [];
  
  const html = `
    <div class="effydoc-tracking-stats">
      <div class="effydoc-stat">
        <div class="effydoc-stat-value">${todayStats.emails_opened || 0}</div>
        <div class="effydoc-stat-label">Emails Opened</div>
      </div>
      <div class="effydoc-stat">
        <div class="effydoc-stat-value">${todayStats.links_clicked || 0}</div>
        <div class="effydoc-stat-label">Links Clicked</div>
      </div>
      <div class="effydoc-stat">
        <div class="effydoc-stat-value">${todayStats.page_views || 0}</div>
        <div class="effydoc-stat-label">Page Views</div>
      </div>
    </div>
    
    ${currentReaders.length > 0 ? `
      <div class="effydoc-current-readers">
        <h4>🟢 Currently Reading</h4>
        ${currentReaders.map(reader => `
          <div class="effydoc-reader">
            ${reader.email} - Page ${reader.page}
          </div>
        `).join('')}
      </div>
    ` : ''}
    
    <div class="effydoc-recent-activity">
      <h4>📈 Recent Activity</h4>
      ${recentActivity.length > 0 ? 
        recentActivity.slice(0, 5).map(activity => `
          <div class="effydoc-activity-item">
            <span class="effydoc-activity-type">${activity.event_type}</span>
            <span class="effydoc-activity-email">${activity.recipient_email || 'Someone'}</span>
            <span class="effydoc-activity-time">${new Date(activity.timestamp).toLocaleTimeString()}</span>
          </div>
        `).join('') : 
        '<div class="effydoc-no-activity">No recent activity</div>'
      }
    </div>
  `;
  
  container.innerHTML = html;
}

// Listen for messages from background script
chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
  if (message.type === 'EFFYDOC_SHOW_SIDEBAR') {
    window.effyDoc.showSidebar();
  }
});

// Listen for page messages
window.addEventListener('message', (event) => {
  if (event.data.type === 'EFFYDOC_SHOW_SIDEBAR' && event.data.source === 'browser_extension') {
    window.effyDoc.showSidebar();
  }
});

// Initialize when DOM is ready
if (document.readyState === 'loading') {
  document.addEventListener('DOMContentLoaded', initializeExtension);
} else {
  initializeExtension();
}