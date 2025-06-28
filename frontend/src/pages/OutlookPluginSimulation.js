import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import toast from 'react-hot-toast';
import { 
  DocumentTextIcon, 
  ArrowPathIcon, 
  ChartBarIcon, 
  EnvelopeIcon,
  CheckCircleIcon,
  XCircleIcon,
  ExclamationTriangleIcon,
  ArrowRightCircleIcon
} from '@heroicons/react/24/outline';

const BACKEND_URL = process.env.REACT_APP_BACKEND_URL;
const API_BASE = `${BACKEND_URL}/api`;

// Create axios instance with default config
const api = axios.create({
  baseURL: API_BASE,
  timeout: 30000,
});

// Mock data for simulation
const MOCK_DATA = {
  user: {
    id: 'user-123',
    email: 'demo@effydoc.com',
    full_name: 'Demo User',
    organization: 'effyDOC Inc.',
    role: 'admin'
  },
  documents: [
    {
      id: 'doc-1',
      title: 'Business Proposal - Q3 2025',
      type: 'proposal',
      created_at: '2025-06-15T10:30:00Z',
      updated_at: '2025-06-20T14:45:00Z',
      total_pages: 5,
      total_views: 12,
      description: 'Quarterly business proposal for potential clients',
      tracking_link: '/view/doc-1?source=outlook_native'
    },
    {
      id: 'doc-2',
      title: 'Service Agreement Template',
      type: 'contract',
      created_at: '2025-05-10T09:15:00Z',
      updated_at: '2025-06-18T11:20:00Z',
      total_pages: 8,
      total_views: 24,
      description: 'Standard service agreement for new clients',
      tracking_link: '/view/doc-2?source=outlook_native'
    },
    {
      id: 'doc-3',
      title: 'Product Roadmap 2025-2026',
      type: 'document',
      created_at: '2025-06-01T16:45:00Z',
      updated_at: '2025-06-22T10:10:00Z',
      total_pages: 12,
      total_views: 36,
      description: 'Strategic product roadmap for the next fiscal year',
      tracking_link: '/view/doc-3?source=outlook_native'
    }
  ],
  contentHub: [
    {
      id: 'hub-1',
      title: 'Company Overview',
      type: 'presentation',
      created_at: '2025-04-20T14:30:00Z',
      owner_name: 'Marketing Team',
      total_pages: 15,
      description: 'Official company overview for client presentations',
      tags: ['company', 'overview', 'marketing'],
      tracking_link: '/view/hub-1?source=outlook_native',
      is_template: true
    },
    {
      id: 'hub-2',
      title: 'Sales Pitch Deck',
      type: 'presentation',
      created_at: '2025-05-05T11:45:00Z',
      owner_name: 'Sales Team',
      total_pages: 18,
      description: 'Standard sales pitch for new prospects',
      tags: ['sales', 'pitch', 'presentation'],
      tracking_link: '/view/hub-2?source=outlook_native',
      is_template: true
    }
  ],
  documentContent: {
    'doc-1': {
      id: 'doc-1',
      title: 'Business Proposal - Q3 2025',
      type: 'proposal',
      total_pages: 5,
      pages: [
        {
          page_number: 1,
          title: 'Introduction',
          content: '<h1>Business Proposal</h1><p>This proposal outlines our services and solutions for Q3 2025. We are excited to present our innovative approach to solving your business challenges.</p><h2>Company Background</h2><p>effyDOC has been a leader in document solutions since 2023, serving over 500 enterprise clients worldwide.</p>'
        },
        {
          page_number: 2,
          title: 'Services',
          content: '<h2>Our Services</h2><ul><li>Document Automation</li><li>AI-Powered Content Generation</li><li>Analytics and Tracking</li><li>Integration Solutions</li></ul>'
        }
      ],
      sections: [
        {
          id: 'section-1',
          title: 'Introduction',
          content: '<h1>Business Proposal</h1><p>This proposal outlines our services and solutions for Q3 2025. We are excited to present our innovative approach to solving your business challenges.</p><h2>Company Background</h2><p>effyDOC has been a leader in document solutions since 2023, serving over 500 enterprise clients worldwide.</p>',
          order: 1
        },
        {
          id: 'section-2',
          title: 'Services',
          content: '<h2>Our Services</h2><ul><li>Document Automation</li><li>AI-Powered Content Generation</li><li>Analytics and Tracking</li><li>Integration Solutions</li></ul>',
          order: 2
        }
      ],
      can_edit: true
    },
    'doc-2': {
      id: 'doc-2',
      title: 'Service Agreement Template',
      type: 'contract',
      total_pages: 8,
      pages: [
        {
          page_number: 1,
          title: 'Terms and Conditions',
          content: '<h1>Service Agreement</h1><p>This Service Agreement (the "Agreement") is entered into as of the date of signature (the "Effective Date") by and between effyDOC Inc. ("Provider") and the client ("Client").</p><h2>1. Services</h2><p>Provider agrees to provide Client with the following services (the "Services") as described in Exhibit A.</p>'
        }
      ],
      sections: [
        {
          id: 'section-1',
          title: 'Terms and Conditions',
          content: '<h1>Service Agreement</h1><p>This Service Agreement (the "Agreement") is entered into as of the date of signature (the "Effective Date") by and between effyDOC Inc. ("Provider") and the client ("Client").</p><h2>1. Services</h2><p>Provider agrees to provide Client with the following services (the "Services") as described in Exhibit A.</p>',
          order: 1
        }
      ],
      can_edit: true
    },
    'doc-3': {
      id: 'doc-3',
      title: 'Product Roadmap 2025-2026',
      type: 'document',
      total_pages: 12,
      pages: [
        {
          page_number: 1,
          title: 'Executive Summary',
          content: '<h1>Product Roadmap 2025-2026</h1><p>This document outlines our product strategy and development plans for the upcoming fiscal year.</p><h2>Vision</h2><p>To become the leading document solution provider by leveraging AI and analytics to transform how businesses create, share, and track documents.</p>'
        }
      ],
      sections: [
        {
          id: 'section-1',
          title: 'Executive Summary',
          content: '<h1>Product Roadmap 2025-2026</h1><p>This document outlines our product strategy and development plans for the upcoming fiscal year.</p><h2>Vision</h2><p>To become the leading document solution provider by leveraging AI and analytics to transform how businesses create, share, and track documents.</p>',
          order: 1
        }
      ],
      can_edit: true
    }
  },
  analytics: {
    'doc-1': {
      document_id: 'doc-1',
      document_title: 'Business Proposal - Q3 2025',
      summary: {
        total_views: 12,
        total_emails: 5,
        unique_viewers: 8,
        total_opens: 4,
        total_clicks: 3,
        open_rate: 80.0,
        click_rate: 60.0
      },
      recent_activity_count: 3,
      total_events: 24,
      last_activity: '2025-06-27T15:30:00Z',
      generated_at: '2025-06-28T07:45:00Z'
    },
    'doc-2': {
      document_id: 'doc-2',
      document_title: 'Service Agreement Template',
      summary: {
        total_views: 24,
        total_emails: 10,
        unique_viewers: 15,
        total_opens: 8,
        total_clicks: 6,
        open_rate: 80.0,
        click_rate: 60.0
      },
      recent_activity_count: 5,
      total_events: 43,
      last_activity: '2025-06-27T16:45:00Z',
      generated_at: '2025-06-28T07:45:00Z'
    },
    'doc-3': {
      document_id: 'doc-3',
      document_title: 'Product Roadmap 2025-2026',
      summary: {
        total_views: 36,
        total_emails: 15,
        unique_viewers: 22,
        total_opens: 12,
        total_clicks: 9,
        open_rate: 80.0,
        click_rate: 60.0
      },
      recent_activity_count: 8,
      total_events: 67,
      last_activity: '2025-06-27T18:15:00Z',
      generated_at: '2025-06-28T07:45:00Z'
    }
  }
};

const OutlookPluginSimulation = () => {
  const navigate = useNavigate();
  const [activeTab, setActiveTab] = useState('auth');
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [sessionInfo, setSessionInfo] = useState(null);
  const [documents, setDocuments] = useState([]);
  const [contentHub, setContentHub] = useState([]);
  const [selectedDocument, setSelectedDocument] = useState(null);
  const [documentContent, setDocumentContent] = useState(null);
  const [generatedAttachment, setGeneratedAttachment] = useState(null);
  const [recipients, setRecipients] = useState('');
  const [subject, setSubject] = useState('');
  const [emailSent, setEmailSent] = useState(false);
  const [analytics, setAnalytics] = useState(null);
  const [loading, setLoading] = useState({
    auth: false,
    documents: false,
    content: false,
    attachment: false,
    tracking: false,
    analytics: false
  });
  const [errors, setErrors] = useState({});

  // Authentication simulation
  const authenticatePlugin = async () => {
    try {
      setLoading({...loading, auth: true});
      
      // Simulate API call delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Use mock data for simulation
      setSessionInfo({
        user_email: MOCK_DATA.user.email,
        full_name: MOCK_DATA.user.full_name,
        organization: MOCK_DATA.user.organization,
        role: MOCK_DATA.user.role,
        connected_at: new Date().toISOString(),
        permissions: {
          can_send_documents: true,
          can_view_analytics: true,
          can_access_content_hub: true,
          can_create_documents: true
        },
        plugin_version: '1.0.0',
        api_version: 'native-v1'
      });
      
      setIsAuthenticated(true);
      setLoading({...loading, auth: false});
      toast.success('Plugin authenticated successfully');
      
      // Automatically load documents after authentication
      loadDocuments();
    } catch (error) {
      setErrors({...errors, auth: 'Authentication failed'});
      setLoading({...loading, auth: false});
      toast.error('Authentication failed');
    }
  };

  // Load user's documents
  const loadDocuments = async () => {
    try {
      setLoading({...loading, documents: true});
      
      // Simulate API call delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Use mock data for simulation
      setDocuments(MOCK_DATA.documents);
      setContentHub(MOCK_DATA.contentHub);
      
      setLoading({...loading, documents: false});
    } catch (error) {
      setErrors({...errors, documents: 'Failed to load documents'});
      setLoading({...loading, documents: false});
      toast.error('Failed to load documents');
    }
  };

  // Load document content
  const loadDocumentContent = async (documentId) => {
    try {
      setLoading({...loading, content: true});
      
      // Simulate API call delay
      await new Promise(resolve => setTimeout(resolve, 800));
      
      // Use mock data for simulation
      setDocumentContent(MOCK_DATA.documentContent[documentId]);
      
      setLoading({...loading, content: false});
    } catch (error) {
      setErrors({...errors, content: 'Failed to load document content'});
      setLoading({...loading, content: false});
      toast.error('Failed to load document content');
    }
  };

  // Generate trackable attachment
  const generateAttachment = async () => {
    try {
      if (!selectedDocument) {
        toast.error('Please select a document first');
        return;
      }
      
      setLoading({...loading, attachment: true});
      
      // Simulate API call delay
      await new Promise(resolve => setTimeout(resolve, 1200));
      
      // Generate tracking link
      const tracking_params = `?source=outlook_native&sender=${MOCK_DATA.user.email}&timestamp=${Math.floor(Date.now() / 1000)}`;
      const tracking_link = `/view/${selectedDocument.id}${tracking_params}`;
      
      // Generate HTML content for email
      const title = selectedDocument.title;
      
      const html_content = `
      <div style='border: 2px solid #4f46e5; border-radius: 8px; padding: 16px; margin: 16px 0; background: #f8fafc;'>
          <div style='display: flex; align-items: center; margin-bottom: 12px;'>
              <strong style='color: #4f46e5; font-size: 16px;'>📄 effyDOC Document</strong>
          </div>
          <h3 style='color: #1e293b; margin: 0 0 8px 0; font-size: 18px;'>${title}</h3>
          <p style='color: #64748b; margin: 0 0 12px 0; font-size: 14px;'>${selectedDocument.type} • ${selectedDocument.total_pages} pages • Updated ${new Date(selectedDocument.updated_at).toLocaleDateString('en-US', { month: 'short', day: 'numeric', year: 'numeric' })}</p>
          <p style='color: #4f46e5; font-size: 12px; margin: 12px 0;'>
              📊 This document includes tracking analytics and interactive elements
          </p>
          <div style='margin-top: 12px;'>
              <a href='${tracking_link}' 
                 style='background: #4f46e5; color: white; padding: 8px 16px; text-decoration: none; border-radius: 6px; font-size: 14px; font-weight: 500;'
                 data-effydoc-document='${selectedDocument.id}' 
                 class='effydoc-tracking-link'>
                 View Full Document
              </a>
          </div>
      </div>`;
      
      setGeneratedAttachment({
        document_id: selectedDocument.id,
        document_title: title,
        html_content: html_content,
        tracking_link: tracking_link,
        generated_at: new Date().toISOString()
      });
      
      setLoading({...loading, attachment: false});
      toast.success('Trackable attachment generated');
    } catch (error) {
      setErrors({...errors, attachment: 'Failed to generate attachment'});
      setLoading({...loading, attachment: false});
      toast.error('Failed to generate attachment');
    }
  };

  // Track email sent
  const trackEmailSent = async () => {
    try {
      if (!selectedDocument || !generatedAttachment) {
        toast.error('Please generate an attachment first');
        return;
      }
      
      if (!recipients.trim()) {
        toast.error('Please enter at least one recipient');
        return;
      }
      
      setLoading({...loading, tracking: true});
      
      // Simulate API call delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      setEmailSent(true);
      setLoading({...loading, tracking: false});
      toast.success('Email tracking started');
      
      // Load analytics after sending email
      loadAnalytics();
    } catch (error) {
      setErrors({...errors, tracking: 'Failed to track email'});
      setLoading({...loading, tracking: false});
      toast.error('Failed to track email');
    }
  };

  // Load document analytics
  const loadAnalytics = async () => {
    try {
      if (!selectedDocument) {
        toast.error('Please select a document first');
        return;
      }
      
      setLoading({...loading, analytics: true});
      
      // Simulate API call delay
      await new Promise(resolve => setTimeout(resolve, 1000));
      
      // Use mock data for simulation
      setAnalytics(MOCK_DATA.analytics[selectedDocument.id]);
      
      setLoading({...loading, analytics: false});
    } catch (error) {
      setErrors({...errors, analytics: 'Failed to load analytics'});
      setLoading({...loading, analytics: false});
      toast.error('Failed to load analytics');
    }
  };

  // Select a document
  const selectDocument = (document) => {
    setSelectedDocument(document);
    loadDocumentContent(document.id);
    setEmailSent(false);
    setGeneratedAttachment(null);
    setAnalytics(null);
  };

  // Reset the simulation
  const resetSimulation = () => {
    setIsAuthenticated(false);
    setSessionInfo(null);
    setDocuments([]);
    setContentHub([]);
    setSelectedDocument(null);
    setDocumentContent(null);
    setGeneratedAttachment(null);
    setRecipients('');
    setSubject('');
    setEmailSent(false);
    setAnalytics(null);
    setErrors({});
    setActiveTab('auth');
  };

  // Check plugin status
  useEffect(() => {
    const checkStatus = async () => {
      try {
        // Simulate API call
        console.log('Plugin status: healthy');
      } catch (error) {
        console.error('Error checking plugin status:', error);
      }
    };
    
    checkStatus();
  }, []);

  return (
    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
      <div className="bg-white shadow sm:rounded-lg mb-6">
        <div className="px-4 py-5 sm:p-6">
          <h1 className="text-3xl font-bold text-gray-900">Native Outlook Plugin Simulation</h1>
          <p className="mt-2 text-gray-600">
            This page simulates the native Outlook plugin integration flow
          </p>
          
          {/* Simulation Controls */}
          <div className="mt-4 flex items-center space-x-4">
            <button
              onClick={resetSimulation}
              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-red-600 hover:bg-red-700"
            >
              <ArrowPathIcon className="h-4 w-4 mr-2" />
              Reset Simulation
            </button>
            
            <div className="text-sm text-gray-500">
              Current Step: <span className="font-medium">{activeTab.charAt(0).toUpperCase() + activeTab.slice(1)}</span>
            </div>
          </div>
        </div>
      </div>
      
      {/* Simulation Tabs */}
      <div className="border-b border-gray-200">
        <nav className="-mb-px flex space-x-8">
          <button
            onClick={() => setActiveTab('auth')}
            className={`${
              activeTab === 'auth'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
          >
            1. Authentication
          </button>
          
          <button
            onClick={() => setActiveTab('documents')}
            disabled={!isAuthenticated}
            className={`${
              !isAuthenticated 
                ? 'border-transparent text-gray-300 cursor-not-allowed' 
                : activeTab === 'documents'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
          >
            2. Document Library
          </button>
          
          <button
            onClick={() => setActiveTab('attachment')}
            disabled={!selectedDocument}
            className={`${
              !selectedDocument 
                ? 'border-transparent text-gray-300 cursor-not-allowed' 
                : activeTab === 'attachment'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
          >
            3. Email Attachment
          </button>
          
          <button
            onClick={() => setActiveTab('analytics')}
            disabled={!emailSent}
            className={`${
              !emailSent 
                ? 'border-transparent text-gray-300 cursor-not-allowed' 
                : activeTab === 'analytics'
                ? 'border-indigo-500 text-indigo-600'
                : 'border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300'
            } whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm`}
          >
            4. Analytics Dashboard
          </button>
        </nav>
      </div>
      
      {/* Tab Content */}
      <div className="mt-6">
        {/* Authentication Tab */}
        {activeTab === 'auth' && (
          <div className="bg-white shadow sm:rounded-lg">
            <div className="px-4 py-5 sm:p-6">
              <h2 className="text-lg font-medium text-gray-900">Plugin Authentication</h2>
              <p className="mt-1 text-sm text-gray-500">
                Simulate the native plugin login process and JWT token storage
              </p>
              
              <div className="mt-4">
                {isAuthenticated ? (
                  <div className="rounded-md bg-green-50 p-4">
                    <div className="flex">
                      <div className="flex-shrink-0">
                        <CheckCircleIcon className="h-5 w-5 text-green-400" aria-hidden="true" />
                      </div>
                      <div className="ml-3">
                        <h3 className="text-sm font-medium text-green-800">Authentication successful</h3>
                        <div className="mt-2 text-sm text-green-700">
                          <p>You are authenticated as {sessionInfo?.user_email}</p>
                          {sessionInfo && (
                            <div className="mt-2 border border-green-200 rounded p-2 bg-green-50">
                              <h4 className="font-medium">Session Information:</h4>
                              <ul className="mt-1 list-disc list-inside text-xs">
                                <li>Full Name: {sessionInfo.full_name}</li>
                                <li>Organization: {sessionInfo.organization}</li>
                                <li>Role: {sessionInfo.role}</li>
                                <li>Plugin Version: {sessionInfo.plugin_version}</li>
                                <li>API Version: {sessionInfo.api_version}</li>
                              </ul>
                            </div>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                ) : (
                  <div>
                    <button
                      onClick={authenticatePlugin}
                      disabled={loading.auth}
                      className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                    >
                      {loading.auth ? (
                        <>
                          <ArrowPathIcon className="h-4 w-4 mr-2 animate-spin" />
                          Authenticating...
                        </>
                      ) : (
                        <>
                          Authenticate Plugin
                        </>
                      )}
                    </button>
                    
                    {errors.auth && (
                      <div className="mt-2 rounded-md bg-red-50 p-4">
                        <div className="flex">
                          <div className="flex-shrink-0">
                            <XCircleIcon className="h-5 w-5 text-red-400" aria-hidden="true" />
                          </div>
                          <div className="ml-3">
                            <h3 className="text-sm font-medium text-red-800">Authentication Error</h3>
                            <div className="mt-2 text-sm text-red-700">
                              <p>{errors.auth}</p>
                            </div>
                          </div>
                        </div>
                      </div>
                    )}
                  </div>
                )}
                
                {isAuthenticated && (
                  <div className="mt-4">
                    <button
                      onClick={() => setActiveTab('documents')}
                      className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
                    >
                      Continue to Document Library
                      <ArrowRightCircleIcon className="ml-2 h-4 w-4" />
                    </button>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
        
        {/* Document Library Tab */}
        {activeTab === 'documents' && (
          <div className="bg-white shadow sm:rounded-lg">
            <div className="px-4 py-5 sm:p-6">
              <h2 className="text-lg font-medium text-gray-900">Document Library</h2>
              <p className="mt-1 text-sm text-gray-500">
                Browse and select documents to attach to emails
              </p>
              
              <div className="mt-4">
                <button
                  onClick={loadDocuments}
                  disabled={loading.documents}
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  {loading.documents ? (
                    <>
                      <ArrowPathIcon className="h-4 w-4 mr-2 animate-spin" />
                      Loading...
                    </>
                  ) : (
                    <>
                      <ArrowPathIcon className="h-4 w-4 mr-2" />
                      Refresh Documents
                    </>
                  )}
                </button>
                
                {errors.documents && (
                  <div className="mt-2 rounded-md bg-red-50 p-4">
                    <div className="flex">
                      <div className="flex-shrink-0">
                        <XCircleIcon className="h-5 w-5 text-red-400" aria-hidden="true" />
                      </div>
                      <div className="ml-3">
                        <h3 className="text-sm font-medium text-red-800">Error</h3>
                        <div className="mt-2 text-sm text-red-700">
                          <p>{errors.documents}</p>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
                
                {/* Document Tabs */}
                <div className="mt-4">
                  <div className="border-b border-gray-200">
                    <nav className="-mb-px flex space-x-8">
                      <button
                        className="border-indigo-500 text-indigo-600 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm"
                      >
                        My Documents ({documents.length})
                      </button>
                      
                      <button
                        className="border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300 whitespace-nowrap py-4 px-1 border-b-2 font-medium text-sm"
                      >
                        Content Hub ({contentHub.length})
                      </button>
                    </nav>
                  </div>
                </div>
                
                {/* Document List */}
                <div className="mt-4">
                  {documents.length === 0 ? (
                    <div className="text-center py-12 bg-gray-50 rounded-lg">
                      <DocumentTextIcon className="mx-auto h-12 w-12 text-gray-400" />
                      <h3 className="mt-2 text-sm font-medium text-gray-900">No documents</h3>
                      <p className="mt-1 text-sm text-gray-500">
                        You don't have any documents in your library yet.
                      </p>
                    </div>
                  ) : (
                    <div className="overflow-hidden shadow ring-1 ring-black ring-opacity-5 md:rounded-lg">
                      <table className="min-w-full divide-y divide-gray-300">
                        <thead className="bg-gray-50">
                          <tr>
                            <th scope="col" className="py-3.5 pl-4 pr-3 text-left text-sm font-semibold text-gray-900 sm:pl-6">
                              Title
                            </th>
                            <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                              Type
                            </th>
                            <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                              Pages
                            </th>
                            <th scope="col" className="px-3 py-3.5 text-left text-sm font-semibold text-gray-900">
                              Views
                            </th>
                            <th scope="col" className="relative py-3.5 pl-3 pr-4 sm:pr-6">
                              <span className="sr-only">Select</span>
                            </th>
                          </tr>
                        </thead>
                        <tbody className="divide-y divide-gray-200 bg-white">
                          {documents.map((document) => (
                            <tr 
                              key={document.id}
                              className={selectedDocument?.id === document.id ? 'bg-indigo-50' : ''}
                            >
                              <td className="whitespace-nowrap py-4 pl-4 pr-3 text-sm font-medium text-gray-900 sm:pl-6">
                                {document.title}
                              </td>
                              <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                                {document.type}
                              </td>
                              <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                                {document.total_pages}
                              </td>
                              <td className="whitespace-nowrap px-3 py-4 text-sm text-gray-500">
                                {document.total_views}
                              </td>
                              <td className="relative whitespace-nowrap py-4 pl-3 pr-4 text-right text-sm font-medium sm:pr-6">
                                <button
                                  onClick={() => selectDocument(document)}
                                  className="text-indigo-600 hover:text-indigo-900"
                                >
                                  {selectedDocument?.id === document.id ? 'Selected' : 'Select'}
                                </button>
                              </td>
                            </tr>
                          ))}
                        </tbody>
                      </table>
                    </div>
                  )}
                </div>
                
                {/* Document Preview */}
                {selectedDocument && documentContent && (
                  <div className="mt-6">
                    <h3 className="text-lg font-medium text-gray-900">Document Preview</h3>
                    <div className="mt-2 border border-gray-200 rounded-lg p-4">
                      <div className="flex justify-between items-start">
                        <div>
                          <h4 className="font-medium text-lg">{documentContent.title}</h4>
                          <p className="text-sm text-gray-500">
                            {documentContent.type} • {documentContent.total_pages} pages
                          </p>
                        </div>
                        <button
                          onClick={() => setActiveTab('attachment')}
                          className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
                        >
                          Continue to Email Attachment
                          <ArrowRightCircleIcon className="ml-2 h-4 w-4" />
                        </button>
                      </div>
                      
                      <div className="mt-4 border-t border-gray-200 pt-4">
                        <h5 className="font-medium">Content Preview:</h5>
                        <div className="mt-2 max-h-60 overflow-y-auto bg-gray-50 p-4 rounded">
                          {documentContent.pages && documentContent.pages.length > 0 ? (
                            <div dangerouslySetInnerHTML={{ __html: documentContent.pages[0].content }} />
                          ) : documentContent.sections && documentContent.sections.length > 0 ? (
                            <div dangerouslySetInnerHTML={{ __html: documentContent.sections[0].content }} />
                          ) : (
                            <p className="text-gray-500 italic">No content available</p>
                          )}
                        </div>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
        
        {/* Email Attachment Tab */}
        {activeTab === 'attachment' && (
          <div className="bg-white shadow sm:rounded-lg">
            <div className="px-4 py-5 sm:p-6">
              <h2 className="text-lg font-medium text-gray-900">Email Attachment</h2>
              <p className="mt-1 text-sm text-gray-500">
                Generate trackable HTML content for email attachment
              </p>
              
              <div className="mt-4">
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h3 className="font-medium">Selected Document</h3>
                  <p className="text-sm text-gray-500">
                    {selectedDocument?.title} • {selectedDocument?.type} • {selectedDocument?.total_pages} pages
                  </p>
                </div>
                
                <div className="mt-4">
                  <button
                    onClick={generateAttachment}
                    disabled={loading.attachment}
                    className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                  >
                    {loading.attachment ? (
                      <>
                        <ArrowPathIcon className="h-4 w-4 mr-2 animate-spin" />
                        Generating...
                      </>
                    ) : (
                      <>
                        Generate Trackable Attachment
                      </>
                    )}
                  </button>
                  
                  {errors.attachment && (
                    <div className="mt-2 rounded-md bg-red-50 p-4">
                      <div className="flex">
                        <div className="flex-shrink-0">
                          <XCircleIcon className="h-5 w-5 text-red-400" aria-hidden="true" />
                        </div>
                        <div className="ml-3">
                          <h3 className="text-sm font-medium text-red-800">Error</h3>
                          <div className="mt-2 text-sm text-red-700">
                            <p>{errors.attachment}</p>
                          </div>
                        </div>
                      </div>
                    </div>
                  )}
                </div>
                
                {/* Generated Attachment Preview */}
                {generatedAttachment && (
                  <div className="mt-6">
                    <h3 className="text-lg font-medium text-gray-900">Generated Attachment</h3>
                    <div className="mt-2 border border-gray-200 rounded-lg p-4">
                      <div className="bg-gray-50 p-4 rounded">
                        <div dangerouslySetInnerHTML={{ __html: generatedAttachment.html_content }} />
                      </div>
                      
                      <div className="mt-4">
                        <h4 className="font-medium">Tracking Information:</h4>
                        <ul className="mt-1 list-disc list-inside text-sm text-gray-600">
                          <li>Document ID: {generatedAttachment.document_id}</li>
                          <li>Tracking Link: {generatedAttachment.tracking_link}</li>
                          <li>Generated At: {new Date(generatedAttachment.generated_at).toLocaleString()}</li>
                        </ul>
                      </div>
                      
                      {/* Email Form */}
                      <div className="mt-6 border-t border-gray-200 pt-4">
                        <h4 className="font-medium">Send Email Simulation</h4>
                        <div className="mt-2 space-y-4">
                          <div>
                            <label htmlFor="recipients" className="block text-sm font-medium text-gray-700">
                              Recipients (comma separated)
                            </label>
                            <input
                              type="text"
                              id="recipients"
                              value={recipients}
                              onChange={(e) => setRecipients(e.target.value)}
                              className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                              placeholder="recipient1@example.com, recipient2@example.com"
                            />
                          </div>
                          
                          <div>
                            <label htmlFor="subject" className="block text-sm font-medium text-gray-700">
                              Subject
                            </label>
                            <input
                              type="text"
                              id="subject"
                              value={subject}
                              onChange={(e) => setSubject(e.target.value)}
                              className="mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-indigo-500 focus:ring-indigo-500 sm:text-sm"
                              placeholder={`${selectedDocument?.title} - Shared via effyDOC`}
                            />
                          </div>
                          
                          <div className="pt-2">
                            <button
                              onClick={trackEmailSent}
                              disabled={loading.tracking || !recipients.trim()}
                              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-green-600 hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-green-500"
                            >
                              {loading.tracking ? (
                                <>
                                  <ArrowPathIcon className="h-4 w-4 mr-2 animate-spin" />
                                  Sending...
                                </>
                              ) : (
                                <>
                                  <EnvelopeIcon className="h-4 w-4 mr-2" />
                                  Send Email with Tracking
                                </>
                              )}
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
                
                {emailSent && (
                  <div className="mt-4">
                    <div className="rounded-md bg-green-50 p-4">
                      <div className="flex">
                        <div className="flex-shrink-0">
                          <CheckCircleIcon className="h-5 w-5 text-green-400" aria-hidden="true" />
                        </div>
                        <div className="ml-3">
                          <h3 className="text-sm font-medium text-green-800">Email sent successfully</h3>
                          <div className="mt-2 text-sm text-green-700">
                            <p>Your email has been sent and tracking has been started.</p>
                          </div>
                          <div className="mt-2">
                            <button
                              onClick={() => setActiveTab('analytics')}
                              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-indigo-600 hover:bg-indigo-700"
                            >
                              View Analytics
                              <ArrowRightCircleIcon className="ml-2 h-4 w-4" />
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
        
        {/* Analytics Dashboard Tab */}
        {activeTab === 'analytics' && (
          <div className="bg-white shadow sm:rounded-lg">
            <div className="px-4 py-5 sm:p-6">
              <h2 className="text-lg font-medium text-gray-900">Analytics Dashboard</h2>
              <p className="mt-1 text-sm text-gray-500">
                View real-time analytics for your document
              </p>
              
              <div className="mt-4">
                <button
                  onClick={loadAnalytics}
                  disabled={loading.analytics}
                  className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-indigo-600 hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500"
                >
                  {loading.analytics ? (
                    <>
                      <ArrowPathIcon className="h-4 w-4 mr-2 animate-spin" />
                      Loading...
                    </>
                  ) : (
                    <>
                      <ArrowPathIcon className="h-4 w-4 mr-2" />
                      Refresh Analytics
                    </>
                  )}
                </button>
                
                {errors.analytics && (
                  <div className="mt-2 rounded-md bg-red-50 p-4">
                    <div className="flex">
                      <div className="flex-shrink-0">
                        <XCircleIcon className="h-5 w-5 text-red-400" aria-hidden="true" />
                      </div>
                      <div className="ml-3">
                        <h3 className="text-sm font-medium text-red-800">Error</h3>
                        <div className="mt-2 text-sm text-red-700">
                          <p>{errors.analytics}</p>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
                
                {/* Analytics Dashboard */}
                {analytics && (
                  <div className="mt-6">
                    <div className="bg-gray-50 p-4 rounded-lg">
                      <h3 className="font-medium text-lg">{analytics.document_title}</h3>
                      <p className="text-sm text-gray-500">
                        Analytics as of {new Date(analytics.generated_at).toLocaleString()}
                      </p>
                    </div>
                    
                    <div className="mt-4 grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
                      {/* Total Views */}
                      <div className="bg-white overflow-hidden shadow rounded-lg">
                        <div className="px-4 py-5 sm:p-6">
                          <div className="flex items-center">
                            <div className="flex-shrink-0 bg-indigo-100 rounded-md p-3">
                              <EyeIcon className="h-6 w-6 text-indigo-600" aria-hidden="true" />
                            </div>
                            <div className="ml-5 w-0 flex-1">
                              <dl>
                                <dt className="text-sm font-medium text-gray-500 truncate">Total Views</dt>
                                <dd>
                                  <div className="text-lg font-medium text-gray-900">
                                    {analytics.summary.total_views}
                                  </div>
                                </dd>
                              </dl>
                            </div>
                          </div>
                        </div>
                      </div>
                      
                      {/* Total Emails */}
                      <div className="bg-white overflow-hidden shadow rounded-lg">
                        <div className="px-4 py-5 sm:p-6">
                          <div className="flex items-center">
                            <div className="flex-shrink-0 bg-green-100 rounded-md p-3">
                              <EnvelopeIcon className="h-6 w-6 text-green-600" aria-hidden="true" />
                            </div>
                            <div className="ml-5 w-0 flex-1">
                              <dl>
                                <dt className="text-sm font-medium text-gray-500 truncate">Total Emails</dt>
                                <dd>
                                  <div className="text-lg font-medium text-gray-900">
                                    {analytics.summary.total_emails}
                                  </div>
                                </dd>
                              </dl>
                            </div>
                          </div>
                        </div>
                      </div>
                      
                      {/* Open Rate */}
                      <div className="bg-white overflow-hidden shadow rounded-lg">
                        <div className="px-4 py-5 sm:p-6">
                          <div className="flex items-center">
                            <div className="flex-shrink-0 bg-blue-100 rounded-md p-3">
                              <ChartBarIcon className="h-6 w-6 text-blue-600" aria-hidden="true" />
                            </div>
                            <div className="ml-5 w-0 flex-1">
                              <dl>
                                <dt className="text-sm font-medium text-gray-500 truncate">Open Rate</dt>
                                <dd>
                                  <div className="text-lg font-medium text-gray-900">
                                    {analytics.summary.open_rate.toFixed(1)}%
                                  </div>
                                </dd>
                              </dl>
                            </div>
                          </div>
                        </div>
                      </div>
                      
                      {/* Unique Viewers */}
                      <div className="bg-white overflow-hidden shadow rounded-lg">
                        <div className="px-4 py-5 sm:p-6">
                          <div className="flex items-center">
                            <div className="flex-shrink-0 bg-purple-100 rounded-md p-3">
                              <DocumentTextIcon className="h-6 w-6 text-purple-600" aria-hidden="true" />
                            </div>
                            <div className="ml-5 w-0 flex-1">
                              <dl>
                                <dt className="text-sm font-medium text-gray-500 truncate">Unique Viewers</dt>
                                <dd>
                                  <div className="text-lg font-medium text-gray-900">
                                    {analytics.summary.unique_viewers}
                                  </div>
                                </dd>
                              </dl>
                            </div>
                          </div>
                        </div>
                      </div>
                    </div>
                    
                    {/* Recent Activity */}
                    <div className="mt-6">
                      <h3 className="text-lg font-medium text-gray-900">Recent Activity</h3>
                      <div className="mt-2 bg-white shadow overflow-hidden sm:rounded-md">
                        {analytics.recent_activity_count === 0 ? (
                          <div className="px-4 py-5 sm:p-6 text-center text-gray-500">
                            <ExclamationTriangleIcon className="mx-auto h-12 w-12 text-gray-400" />
                            <h3 className="mt-2 text-sm font-medium text-gray-900">No recent activity</h3>
                            <p className="mt-1 text-sm text-gray-500">
                              There has been no activity in the last 24 hours.
                            </p>
                            <p className="mt-3 text-sm text-gray-500">
                              Note: In a real environment, this would show real-time updates as recipients interact with your document.
                            </p>
                          </div>
                        ) : (
                          <ul className="divide-y divide-gray-200">
                            <li className="px-4 py-4 sm:px-6">
                              <div className="flex items-center justify-between">
                                <p className="text-sm font-medium text-indigo-600 truncate">
                                  {analytics.recent_activity_count} recent activities
                                </p>
                                <div className="ml-2 flex-shrink-0 flex">
                                  <p className="px-2 inline-flex text-xs leading-5 font-semibold rounded-full bg-green-100 text-green-800">
                                    Active
                                  </p>
                                </div>
                              </div>
                              <div className="mt-2 sm:flex sm:justify-between">
                                <div className="sm:flex">
                                  <p className="flex items-center text-sm text-gray-500">
                                    <DocumentTextIcon className="flex-shrink-0 mr-1.5 h-5 w-5 text-gray-400" aria-hidden="true" />
                                    {analytics.document_title}
                                  </p>
                                </div>
                                <div className="mt-2 flex items-center text-sm text-gray-500 sm:mt-0">
                                  <p>
                                    Last activity: {analytics.last_activity ? new Date(analytics.last_activity).toLocaleString() : 'N/A'}
                                  </p>
                                </div>
                              </div>
                            </li>
                          </ul>
                        )}
                      </div>
                    </div>
                    
                    {/* Simulation Complete */}
                    <div className="mt-8 rounded-md bg-green-50 p-4">
                      <div className="flex">
                        <div className="flex-shrink-0">
                          <CheckCircleIcon className="h-5 w-5 text-green-400" aria-hidden="true" />
                        </div>
                        <div className="ml-3">
                          <h3 className="text-sm font-medium text-green-800">Simulation Complete</h3>
                          <div className="mt-2 text-sm text-green-700">
                            <p>You have successfully completed the native Outlook plugin simulation flow.</p>
                            <p className="mt-1">
                              The simulation demonstrates the complete workflow: Login → Browse Documents → Select Document → Generate Attachment → Send Email → View Analytics
                            </p>
                          </div>
                          <div className="mt-4">
                            <button
                              onClick={resetSimulation}
                              className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md text-white bg-green-600 hover:bg-green-700"
                            >
                              <ArrowPathIcon className="h-4 w-4 mr-2" />
                              Restart Simulation
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>
                  </div>
                )}
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default OutlookPluginSimulation;