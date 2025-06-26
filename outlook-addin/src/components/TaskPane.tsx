import React, { useState, useEffect } from 'react';
import { authAPI } from '../services/api';
import DocumentLibrary from './DocumentLibrary';
import ContentHub from './ContentHub';
import LiveTracking from './LiveTracking';
import { 
  FolderOpen, 
  Building, 
  Activity, 
  LogIn, 
  User, 
  Wifi, 
  WifiOff,
  Settings
} from 'lucide-react';
import toast, { Toaster } from 'react-hot-toast';

interface BaseDocument {
  id: string;
  title: string;
  type: string;
  created_at: string;
  total_pages: number;
  share_link: string;
  is_trackable: boolean;
}

interface User {
  user_email: string;
  full_name: string;
  organization: string;
  role: string;
}

const TaskPane: React.FC = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [user, setUser] = useState<User | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState<'library' | 'hub' | 'tracking'>('library');
  const [selectedDocument, setSelectedDocument] = useState<BaseDocument | null>(null);
  const [loginCredentials, setLoginCredentials] = useState({ email: '', password: '' });
  const [isLoggingIn, setIsLoggingIn] = useState(false);

  useEffect(() => {
    initializeApp();
  }, []);

  const initializeApp = async () => {
    try {
      // Initialize Office.js
      Office.onReady((info) => {
        if (info.host === Office.HostType.Outlook) {
          console.log('Outlook add-in initialized successfully');
        }
      });

      // Check for existing authentication
      const token = localStorage.getItem('effydoc_token');
      const userData = localStorage.getItem('effydoc_user');

      if (token && userData) {
        try {
          const parsedUser = JSON.parse(userData);
          const sessionInfo = await authAPI.getSessionInfo();
          setUser(sessionInfo);
          setIsAuthenticated(true);
          toast.success(`Welcome back, ${sessionInfo.full_name}!`);
        } catch (error) {
          // Token might be expired
          localStorage.removeItem('effydoc_token');
          localStorage.removeItem('effydoc_user');
        }
      }
    } catch (error) {
      console.error('Error initializing app:', error);
    } finally {
      setLoading(false);
    }
  };

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setIsLoggingIn(true);
    
    try {
      const response = await authAPI.login(loginCredentials);
      
      // Store authentication data
      localStorage.setItem('effydoc_token', response.access_token);
      localStorage.setItem('effydoc_user', JSON.stringify(response.user));
      
      setUser(response.user);
      setIsAuthenticated(true);
      toast.success(`Welcome, ${response.user.full_name}!`);
      
    } catch (error: any) {
      console.error('Login failed:', error);
      toast.error(error.response?.data?.detail || 'Login failed. Please check your credentials.');
    } finally {
      setIsLoggingIn(false);
    }
  };

  const handleLogout = () => {
    localStorage.removeItem('effydoc_token');
    localStorage.removeItem('effydoc_user');
    setIsAuthenticated(false);
    setUser(null);
    setSelectedDocument(null);
    toast.success('Logged out successfully');
  };

  const handleDocumentSelect = (document: BaseDocument) => {
    setSelectedDocument(document);
    setActiveTab('tracking');
    toast.success(`Tracking ${document.title}`);
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-screen bg-gray-50">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600">Initializing effyDOC...</p>
        </div>
      </div>
    );
  }

  if (!isAuthenticated) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
        <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-md">
          <div className="text-center mb-8">
            <div className="bg-blue-600 rounded-full w-16 h-16 flex items-center justify-center mx-auto mb-4">
              <Activity className="w-8 h-8 text-white" />
            </div>
            <h1 className="text-2xl font-bold text-gray-900">effyDOC</h1>
            <p className="text-gray-600 mt-2">Outlook Document Tracker</p>
          </div>

          <form onSubmit={handleLogin} className="space-y-4">
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Email Address
              </label>
              <input
                type="email"
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Enter your email"
                value={loginCredentials.email}
                onChange={(e) => setLoginCredentials({ ...loginCredentials, email: e.target.value })}
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-2">
                Password
              </label>
              <input
                type="password"
                required
                className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                placeholder="Enter your password"
                value={loginCredentials.password}
                onChange={(e) => setLoginCredentials({ ...loginCredentials, password: e.target.value })}
              />
            </div>

            <button
              type="submit"
              disabled={isLoggingIn}
              className="w-full bg-blue-600 hover:bg-blue-700 disabled:bg-blue-400 text-white font-medium py-2 px-4 rounded-md transition-colors flex items-center justify-center"
            >
              {isLoggingIn ? (
                <>
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2"></div>
                  Signing In...
                </>
              ) : (
                <>
                  <LogIn className="w-4 h-4 mr-2" />
                  Sign In
                </>
              )}
            </button>
          </form>

          <div className="mt-6 text-center">
            <p className="text-xs text-gray-500">
              Don't have an account? Visit effyDOC to get started.
            </p>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="flex flex-col h-screen bg-gray-50">
      {/* Header */}
      <div className="bg-white border-b border-gray-200 px-4 py-3">
        <div className="flex items-center justify-between">
          <div className="flex items-center">
            <div className="bg-blue-600 rounded-lg w-8 h-8 flex items-center justify-center mr-3">
              <Activity className="w-5 h-5 text-white" />
            </div>
            <div>
              <h1 className="text-lg font-semibold text-gray-900">effyDOC</h1>
              <p className="text-xs text-gray-500">Document Tracker</p>
            </div>
          </div>
          
          <div className="flex items-center space-x-2">
            <div className="text-right">
              <p className="text-sm font-medium text-gray-900">{user?.full_name}</p>
              <p className="text-xs text-gray-500">{user?.organization}</p>
            </div>
            <button
              onClick={handleLogout}
              className="p-2 text-gray-400 hover:text-gray-600 transition-colors"
              title="Logout"
            >
              <Settings className="w-4 h-4" />
            </button>
          </div>
        </div>
      </div>

      {/* Tab Navigation */}
      <div className="bg-white border-b border-gray-200">
        <div className="flex">
          <button
            onClick={() => setActiveTab('library')}
            className={`flex-1 px-4 py-3 text-sm font-medium border-b-2 transition-colors ${
              activeTab === 'library'
                ? 'border-blue-500 text-blue-600 bg-blue-50'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:bg-gray-50'
            }`}
          >
            <div className="flex items-center justify-center">
              <FolderOpen className="w-4 h-4 mr-1" />
              My Library
            </div>
          </button>
          
          <button
            onClick={() => setActiveTab('hub')}
            className={`flex-1 px-4 py-3 text-sm font-medium border-b-2 transition-colors ${
              activeTab === 'hub'
                ? 'border-blue-500 text-blue-600 bg-blue-50'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:bg-gray-50'
            }`}
          >
            <div className="flex items-center justify-center">
              <Building className="w-4 h-4 mr-1" />
              Content Hub
            </div>
          </button>
          
          <button
            onClick={() => setActiveTab('tracking')}
            className={`flex-1 px-4 py-3 text-sm font-medium border-b-2 transition-colors ${
              activeTab === 'tracking'
                ? 'border-blue-500 text-blue-600 bg-blue-50'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:bg-gray-50'
            }`}
          >
            <div className="flex items-center justify-center">
              <Activity className="w-4 h-4 mr-1" />
              Live Tracking
            </div>
          </button>
        </div>
      </div>

      {/* Content */}
      <div className="flex-1 overflow-y-auto p-4">
        {activeTab === 'library' && (
          <DocumentLibrary onDocumentSelect={handleDocumentSelect} />
        )}
        
        {activeTab === 'hub' && (
          <ContentHub onDocumentSelect={handleDocumentSelect} />
        )}
        
        {activeTab === 'tracking' && user && (
          <LiveTracking 
            userEmail={user.user_email} 
            selectedDocumentId={selectedDocument?.id}
          />
        )}
      </div>

      {/* Toaster for notifications */}
      <Toaster 
        position="top-center"
        toastOptions={{
          duration: 3000,
          style: {
            background: '#363636',
            color: '#fff',
            fontSize: '14px',
          },
        }}
      />
    </div>
  );
};

export default TaskPane;