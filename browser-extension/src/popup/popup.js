// Popup script for effyDOC browser extension
/* global chrome */

document.addEventListener('DOMContentLoaded', async () => {
  console.log('effyDOC Popup: Initialized');
  
  // Elements
  const loadingState = document.getElementById('loading-state');
  const mainContent = document.getElementById('main-content');
  const outlookCheck = document.getElementById('outlook-check');
  const statusIndicator = document.getElementById('status-indicator');
  const quickStats = document.getElementById('quick-stats');
  const openSidebarBtn = document.getElementById('open-sidebar-btn');
  const loginBtn = document.getElementById('login-btn');
  const logoutBtn = document.getElementById('logout-btn');
  const recentActivity = document.getElementById('recent-activity');
  const activityList = document.getElementById('activity-list');
  
  // Footer links
  const dashboardLink = document.getElementById('dashboard-link');
  const helpLink = document.getElementById('help-link');
  const settingsLink = document.getElementById('settings-link');
  
  // State
  let currentUser = null;
  let isOnOutlook = false;
  
  // Initialize popup
  await initializePopup();
  
  // Event listeners
  openSidebarBtn.addEventListener('click', openSidebar);
  loginBtn.addEventListener('click', openLoginPage);
  logoutBtn.addEventListener('click', handleLogout);
  
  dashboardLink.addEventListener('click', (e) => {
    e.preventDefault();
    openDashboard();
  });
  
  helpLink.addEventListener('click', (e) => {
    e.preventDefault();
    openHelp();
  });
  
  settingsLink.addEventListener('click', (e) => {
    e.preventDefault();
    openSettings();
  });
  
  async function initializePopup() {
    try {
      // Check if we're on Outlook
      const tabs = await chrome.tabs.query({ active: true, currentWindow: true });
      const currentTab = tabs[0];
      
      isOnOutlook = currentTab && (
        currentTab.url.includes('outlook.office.com') ||
        currentTab.url.includes('outlook.live.com') ||
        currentTab.url.includes('outlook.office365.com')
      );
      
      // Update Outlook detection UI
      updateOutlookDetection();
      
      // Check authentication status
      await checkAuthStatus();
      
      // Load recent activity if authenticated
      if (currentUser) {
        await loadRecentActivity();
      }
      
    } catch (error) {
      console.error('Popup initialization failed:', error);
      showError('Failed to initialize extension');
    } finally {
      loadingState.classList.add('hidden');
      mainContent.classList.remove('hidden');
    }
  }
  
  function updateOutlookDetection() {
    if (isOnOutlook) {
      outlookCheck.style.background = '#dcfce7';
      outlookCheck.style.borderColor = '#bbf7d0';
      outlookCheck.innerHTML = `
        <span class="outlook-check-icon">✅</span>
        <span class="outlook-check-text" style="color: #166534;">Outlook Web App detected - Ready to use!</span>
      `;
      openSidebarBtn.disabled = false;
    } else {
      outlookCheck.style.background = '#fef3c7';
      outlookCheck.style.borderColor = '#f59e0b';
      outlookCheck.innerHTML = `
        <span class="outlook-check-icon">⚠️</span>
        <span class="outlook-check-text">Please open Outlook Web App to use this extension</span>
      `;
      openSidebarBtn.disabled = true;
      openSidebarBtn.textContent = '📱 Open Outlook First';
    }
  }
  
  async function checkAuthStatus() {
    try {
      const response = await chrome.runtime.sendMessage({ type: 'GET_AUTH_STATUS' });
      
      if (response.authenticated) {
        currentUser = response.user;
        updateUIForAuthenticatedUser();
        await loadQuickStats();
      } else {
        currentUser = null;
        updateUIForUnauthenticatedUser();
      }
    } catch (error) {
      console.error('Auth status check failed:', error);
      currentUser = null;
      updateUIForUnauthenticatedUser();
    }
  }
  
  function updateUIForAuthenticatedUser() {
    statusIndicator.className = 'status-indicator status-connected';
    statusIndicator.innerHTML = `
      <span class="status-icon">🟢</span>
      <span>Connected as ${currentUser.full_name}</span>
    `;
    
    loginBtn.classList.add('hidden');
    logoutBtn.classList.remove('hidden');
    quickStats.classList.remove('hidden');
    recentActivity.classList.remove('hidden');
    
    if (isOnOutlook) {
      openSidebarBtn.textContent = '📱 Open Document Tracker';
    }
  }
  
  function updateUIForUnauthenticatedUser() {
    statusIndicator.className = 'status-indicator status-disconnected';
    statusIndicator.innerHTML = `
      <span class="status-icon">🔴</span>
      <span>Not connected to effyDOC</span>
    `;
    
    loginBtn.classList.remove('hidden');
    logoutBtn.classList.add('hidden');
    quickStats.classList.add('hidden');
    recentActivity.classList.add('hidden');
    
    if (isOnOutlook) {
      openSidebarBtn.textContent = '🔑 Sign In First';
    }
  }
  
  async function loadQuickStats() {
    try {
      // Load document count from My Library
      const response = await chrome.runtime.sendMessage({
        type: 'GET_DOCUMENTS',
        libraryType: 'my-library'
      });
      
      if (response.success) {
        const documentCount = response.documents.length;
        document.getElementById('stat-documents').textContent = documentCount;
        
        // Calculate total views (simplified)
        const totalViews = response.documents.reduce((sum, doc) => {
          return sum + (doc.tracking_stats?.total_views || 0);
        }, 0);
        document.getElementById('stat-views').textContent = totalViews;
      }
    } catch (error) {
      console.error('Failed to load quick stats:', error);
    }
  }
  
  async function loadRecentActivity() {
    try {
      // This would be expanded to load actual recent activity
      // For now, showing sample data
      const sampleActivities = [
        {
          icon: '📧',
          text: 'Document opened by client@example.com',
          time: '2 min ago'
        },
        {
          icon: '👆',
          text: 'Link clicked in proposal.pdf',
          time: '15 min ago'
        },
        {
          icon: '📄',
          text: 'New document created',
          time: '1 hour ago'
        }
      ];
      
      activityList.innerHTML = sampleActivities.map(activity => `
        <div class="activity-item">
          <span class="activity-icon">${activity.icon}</span>
          <span class="activity-text">${activity.text}</span>
          <span class="activity-time">${activity.time}</span>
        </div>
      `).join('');
      
    } catch (error) {
      console.error('Failed to load recent activity:', error);
      activityList.innerHTML = '<div class="activity-item">Failed to load activity</div>';
    }
  }
  
  async function openSidebar() {
    if (!isOnOutlook) {
      // Open Outlook if not already open
      await chrome.tabs.create({
        url: 'https://outlook.office.com'
      });
      return;
    }
    
    if (!currentUser) {
      // Need to sign in first
      openLoginPage();
      return;
    }
    
    try {
      // Get current tab
      const tabs = await chrome.tabs.query({ active: true, currentWindow: true });
      const currentTab = tabs[0];
      
      // Send message to content script to show sidebar
      await chrome.tabs.sendMessage(currentTab.id, {
        type: 'EFFYDOC_SHOW_SIDEBAR'
      });
      
      // Close popup
      window.close();
      
    } catch (error) {
      console.error('Failed to open sidebar:', error);
      showError('Failed to open sidebar. Please refresh Outlook and try again.');
    }
  }
  
  function openLoginPage() {
    chrome.tabs.create({
      url: 'https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/register?source=browser_extension'
    });
    window.close();
  }
  
  async function handleLogout() {
    try {
      await chrome.runtime.sendMessage({ type: 'LOGOUT' });
      currentUser = null;
      updateUIForUnauthenticatedUser();
      showSuccess('Successfully signed out');
    } catch (error) {
      console.error('Logout failed:', error);
      showError('Failed to sign out');
    }
  }
  
  function openDashboard() {
    chrome.tabs.create({
      url: 'https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/dashboard'
    });
    window.close();
  }
  
  function openHelp() {
    chrome.tabs.create({
      url: 'https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/integrations.html'
    });
    window.close();
  }
  
  function openSettings() {
    chrome.tabs.create({
      url: 'https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/profile'
    });
    window.close();
  }
  
  function showError(message) {
    const existingError = document.querySelector('.error');
    if (existingError) {
      existingError.remove();
    }
    
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error';
    errorDiv.textContent = message;
    
    mainContent.insertBefore(errorDiv, mainContent.firstChild);
    
    setTimeout(() => {
      errorDiv.remove();
    }, 5000);
  }
  
  function showSuccess(message) {
    const existingSuccess = document.querySelector('.success');
    if (existingSuccess) {
      existingSuccess.remove();
    }
    
    const successDiv = document.createElement('div');
    successDiv.className = 'success';
    successDiv.textContent = message;
    
    mainContent.insertBefore(successDiv, mainContent.firstChild);
    
    setTimeout(() => {
      successDiv.remove();
    }, 3000);
  }
  
  // Listen for auth changes
  chrome.runtime.onMessage.addListener((message, sender, sendResponse) => {
    if (message.type === 'AUTH_CHANGED') {
      checkAuthStatus();
    }
  });
  
  // Refresh data every 30 seconds if popup is open
  setInterval(() => {
    if (currentUser) {
      loadQuickStats();
      loadRecentActivity();
    }
  }, 30000);
});