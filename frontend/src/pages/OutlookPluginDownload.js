import React, { useState, useEffect } from 'react';
import { 
  ArrowDownTrayIcon, 
  DocumentTextIcon, 
  CheckCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  ComputerDesktopIcon,
  CogIcon,
  PlayIcon,
  CloudArrowDownIcon
} from '@heroicons/react/24/outline';

const OutlookPluginDownload = () => {
  const [downloadStatus, setDownloadStatus] = useState('ready');
  const [installerExists, setInstallerExists] = useState(false);

  useEffect(() => {
    // Check if installer exists
    checkInstallerAvailability();
  }, []);

  const checkInstallerAvailability = async () => {
    try {
      const response = await fetch('/EffyDocOutlookPlugin-Setup.exe', { method: 'HEAD' });
      setInstallerExists(response.ok);
    } catch (error) {
      setInstallerExists(false);
    }
  };

  const handleDownloadInstaller = () => {
    setDownloadStatus('downloading');
    
    // Create download link
    const link = document.createElement('a');
    link.href = '/EffyDocOutlookPlugin-Setup.exe';
    link.download = 'EffyDocOutlookPlugin-Setup.exe';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    
    // Reset status after a delay
    setTimeout(() => {
      setDownloadStatus('completed');
    }, 2000);
  };

  const generateInstaller = async () => {
    setDownloadStatus('generating');
    
    try {
      // Call the build script endpoint (you'll need to create this)
      const response = await fetch('/api/build-outlook-installer', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify({
          version: '1.0.0',
          backendURL: process.env.REACT_APP_BACKEND_URL || window.location.origin
        })
      });
      
      if (response.ok) {
        setInstallerExists(true);
        setDownloadStatus('ready');
      } else {
        setDownloadStatus('error');
      }
    } catch (error) {
      console.error('Error generating installer:', error);
      setDownloadStatus('error');
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="text-center mb-12">
          <div className="flex justify-center mb-4">
            <div className="bg-indigo-100 p-4 rounded-full">
              <ArrowDownTrayIcon className="h-12 w-12 text-indigo-600" />
            </div>
          </div>
          <h1 className="text-4xl font-bold text-gray-900 mb-4">
            Download effyDOC Outlook Plugin
          </h1>
          <p className="text-xl text-gray-600 max-w-3xl mx-auto">
            Install the native Outlook plugin that integrates directly into your Outlook interface. 
            Track documents, monitor engagement, and get real-time analytics.
          </p>
        </div>

        {/* Main Download Section */}
        <div className="bg-white rounded-lg shadow-lg border border-gray-200 p-8 mb-8">
          <div className="text-center">
            <div className="flex justify-center mb-6">
              <div className="bg-green-100 p-6 rounded-full">
                <ComputerDesktopIcon className="h-16 w-16 text-green-600" />
              </div>
            </div>
            
            <h2 className="text-2xl font-bold text-gray-900 mb-4">
              Ready-to-Install Plugin
            </h2>
            
            <p className="text-gray-600 mb-8 max-w-2xl mx-auto">
              Download the official effyDOC Outlook plugin installer. 
              One-click installation with automatic configuration and setup.
            </p>

            {/* Download Button */}
            {installerExists ? (
              <button
                onClick={handleDownloadInstaller}
                disabled={downloadStatus === 'downloading'}
                className="inline-flex items-center px-8 py-4 bg-indigo-600 text-white font-semibold rounded-lg hover:bg-indigo-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-lg"
              >
                {downloadStatus === 'downloading' ? (
                  <>
                    <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white mr-3"></div>
                    Downloading...
                  </>
                ) : downloadStatus === 'completed' ? (
                  <>
                    <CheckCircleIcon className="h-6 w-6 mr-3" />
                    Download Complete!
                  </>
                ) : (
                  <>
                    <ArrowDownTrayIcon className="h-6 w-6 mr-3" />
                    Download Installer (Free)
                  </>
                )}
              </button>
            ) : (
              <div className="space-y-4">
                <p className="text-amber-600 bg-amber-50 border border-amber-200 rounded-lg p-4">
                  <ExclamationTriangleIcon className="h-5 w-5 inline mr-2" />
                  Installer not yet generated. Click below to create it.
                </p>
                
                <button
                  onClick={generateInstaller}
                  disabled={downloadStatus === 'generating'}
                  className="inline-flex items-center px-8 py-4 bg-blue-600 text-white font-semibold rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed text-lg"
                >
                  {downloadStatus === 'generating' ? (
                    <>
                      <div className="animate-spin rounded-full h-5 w-5 border-b-2 border-white mr-3"></div>
                      Generating Installer...
                    </>
                  ) : (
                    <>
                      <CogIcon className="h-6 w-6 mr-3" />
                      Generate Installer
                    </>
                  )}
                </button>
              </div>
            )}
            
            {downloadStatus === 'error' && (
              <div className="mt-4 p-4 bg-red-50 border border-red-200 rounded-lg">
                <p className="text-red-600">
                  Failed to generate installer. Please try again or contact support.
                </p>
              </div>
            )}
          </div>
        </div>

        {/* Features Preview */}
        <div className="grid md:grid-cols-3 gap-6 mb-8">
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center mb-4">
              <DocumentTextIcon className="h-8 w-8 text-indigo-600 mr-3" />
              <h3 className="text-lg font-semibold text-gray-900">Document Tracking</h3>
            </div>
            <p className="text-gray-600 text-sm">
              Track when recipients open, read, and interact with your documents. 
              Get real-time notifications and detailed analytics.
            </p>
          </div>
          
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center mb-4">
              <ComputerDesktopIcon className="h-8 w-8 text-green-600 mr-3" />
              <h3 className="text-lg font-semibold text-gray-900">Native Integration</h3>
            </div>
            <p className="text-gray-600 text-sm">
              Seamlessly integrated into your Outlook interface. 
              Access all features directly from your email sidebar.
            </p>
          </div>
          
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center mb-4">
              <CloudArrowDownIcon className="h-8 w-8 text-blue-600 mr-3" />
              <h3 className="text-lg font-semibold text-gray-900">Easy Installation</h3>
            </div>
            <p className="text-gray-600 text-sm">
              One-click installation with automatic configuration. 
              No technical knowledge required.
            </p>
          </div>
        </div>

        {/* Installation Instructions */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Installation Instructions</h2>
          
          <div className="space-y-6">
            <div className="flex">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-indigo-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                  1
                </div>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Download the Installer</h3>
                <p className="text-gray-600 mt-1">
                  Click the download button above to get the EffyDocOutlookPlugin-Setup.exe file
                </p>
              </div>
            </div>

            <div className="flex">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-indigo-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                  2
                </div>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Run the Installer</h3>
                <p className="text-gray-600 mt-1">
                  Double-click the downloaded file and follow the installation wizard
                </p>
              </div>
            </div>

            <div className="flex">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-indigo-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                  3
                </div>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Restart Outlook</h3>
                <p className="text-gray-600 mt-1">
                  Close and reopen Microsoft Outlook to activate the plugin
                </p>
              </div>
            </div>

            <div className="flex">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-indigo-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                  4
                </div>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Sign In & Start Tracking</h3>
                <p className="text-gray-600 mt-1">
                  Look for the effyDOC panel in your Outlook sidebar and sign in with your account
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* System Requirements */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">System Requirements</h2>
          
          <div className="grid md:grid-cols-2 gap-6">
            <div>
              <h3 className="font-medium text-gray-900 mb-2">Windows</h3>
              <ul className="space-y-1 text-sm text-gray-600">
                <li>• Windows 7 or later</li>
                <li>• .NET Framework 4.7.2+</li>
                <li>• 50 MB free disk space</li>
                <li>• Active internet connection</li>
              </ul>
            </div>
            <div>
              <h3 className="font-medium text-gray-900 mb-2">Microsoft Outlook</h3>
              <ul className="space-y-1 text-sm text-gray-600">
                <li>• Outlook 2013 or later</li>
                <li>• Office 365 supported</li>
                <li>• Add-ins must be enabled</li>
                <li>• Administrator rights (for installation)</li>
              </ul>
            </div>
          </div>
        </div>

        {/* Support Information */}
        <div className="bg-blue-50 border border-blue-200 rounded-lg p-6">
          <div className="flex">
            <InformationCircleIcon className="h-6 w-6 text-blue-600 mt-0.5" />
            <div className="ml-3">
              <h3 className="text-lg font-medium text-blue-900">
                Need Help?
              </h3>
              <p className="mt-1 text-blue-700">
                If you encounter any issues during installation or have questions about the plugin, 
                please visit our support center or contact our team.
              </p>
              <div className="mt-4 space-x-4">
                <a 
                  href="/support" 
                  className="inline-flex items-center text-blue-600 hover:text-blue-800 font-medium"
                >
                  <DocumentTextIcon className="h-4 w-4 mr-1" />
                  Support Center
                </a>
                <a 
                  href="mailto:support@effydoc.com" 
                  className="inline-flex items-center text-blue-600 hover:text-blue-800 font-medium"
                >
                  ✉️ Contact Support
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OutlookPluginDownload;