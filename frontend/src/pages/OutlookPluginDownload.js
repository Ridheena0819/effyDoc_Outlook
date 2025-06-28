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
    
    // Create download link for the working .bat file
    const link = document.createElement('a');
    link.href = '/EffyDocOutlookPlugin-Setup.bat';
    link.download = 'EffyDocOutlookPlugin-Setup.bat';
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

        {/* Download Options */}
        <div className="grid md:grid-cols-2 gap-6 mb-8">
          {/* Primary Installer */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="text-center">
              <div className="flex justify-center mb-4">
                <div className="bg-green-100 p-4 rounded-full">
                  <ComputerDesktopIcon className="h-12 w-12 text-green-600" />
                </div>
              </div>
              
              <h3 className="text-xl font-semibold text-gray-900 mb-3">
                Installer (.bat)
              </h3>
              
              <p className="text-gray-600 mb-6 text-sm">
                Download the working installer file. Right-click and "Run as Administrator" for best results.
              </p>

              {installerExists ? (
                <button
                  onClick={handleDownloadInstaller}
                  disabled={downloadStatus === 'downloading'}
                  className="w-full bg-green-600 text-white px-4 py-3 rounded-lg hover:bg-green-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed font-medium"
                >
                  {downloadStatus === 'downloading' ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2 inline-block"></div>
                      Downloading...
                    </>
                  ) : downloadStatus === 'completed' ? (
                    <>
                      <CheckCircleIcon className="h-5 w-5 mr-2 inline-block" />
                      Download Complete!
                    </>
                  ) : (
                    <>
                      <ArrowDownTrayIcon className="h-5 w-5 mr-2 inline-block" />
                      Download Installer
                    </>
                  )}
                </button>
              ) : (
                <button
                  onClick={generateInstaller}
                  disabled={downloadStatus === 'generating'}
                  className="w-full bg-blue-600 text-white px-4 py-3 rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50 disabled:cursor-not-allowed font-medium"
                >
                  {downloadStatus === 'generating' ? (
                    <>
                      <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white mr-2 inline-block"></div>
                      Generating...
                    </>
                  ) : (
                    <>
                      <CogIcon className="h-5 w-5 mr-2 inline-block" />
                      Generate Installer
                    </>
                  )}
                </button>
              )}
            </div>
          </div>

          {/* Alternative Option */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="text-center">
              <div className="flex justify-center mb-4">
                <div className="bg-blue-100 p-4 rounded-full">
                  <DocumentTextIcon className="h-12 w-12 text-blue-600" />
                </div>
              </div>
              
              <h3 className="text-xl font-semibold text-gray-900 mb-3">
                Manual Setup
              </h3>
              
              <p className="text-gray-600 mb-6 text-sm">
                If the installer doesn't work, follow our step-by-step manual installation guide.
              </p>

              <a
                href="#manual-setup"
                className="w-full bg-blue-600 text-white px-4 py-3 rounded-lg hover:bg-blue-700 transition-colors font-medium inline-block"
              >
                <InformationCircleIcon className="h-5 w-5 mr-2 inline-block" />
                View Manual Setup
              </a>
            </div>
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

        {/* Troubleshooting Section */}
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-6 mb-8">
          <h2 className="text-xl font-semibold text-yellow-800 mb-4 flex items-center">
            <ExclamationTriangleIcon className="h-6 w-6 mr-2" />
            Troubleshooting
          </h2>
          
          <div className="space-y-4">
            <div>
              <h3 className="font-medium text-yellow-800 mb-2">
                "This app can't run on your PC" Error
              </h3>
              <p className="text-yellow-700 text-sm mb-2">
                If you see this error when running the installer:
              </p>
              <ol className="text-yellow-700 text-sm space-y-1 ml-4">
                <li>1. Right-click the downloaded file → "Run as administrator"</li>
                <li>2. If still blocked, right-click → Properties → Unblock → OK</li>
                <li>3. Try running from Command Prompt as administrator</li>
                <li>4. Alternatively, use the manual setup option above</li>
              </ol>
            </div>
            
            <div>
              <h3 className="font-medium text-yellow-800 mb-2">
                Windows Security Warning
              </h3>
              <p className="text-yellow-700 text-sm">
                Windows may show security warnings for downloaded files. This is normal for new installers. 
                Click "More info" → "Run anyway" if you trust the source.
              </p>
            </div>
            
            <div>
              <h3 className="font-medium text-yellow-800 mb-2">
                Plugin Not Appearing in Outlook
              </h3>
              <ol className="text-yellow-700 text-sm space-y-1 ml-4">
                <li>1. Completely close and restart Outlook</li>
                <li>2. Check File → Options → Add-ins → Manage: COM Add-ins</li>
                <li>3. Ensure "effyDOC" is listed and enabled</li>
                <li>4. Try running Outlook as administrator once</li>
              </ol>
            </div>
          </div>
        </div>

        {/* Manual Setup Section */}
        <div id="manual-setup" className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Manual Setup Instructions</h2>
          
          <div className="space-y-4 text-sm">
            <div className="bg-gray-50 p-4 rounded-lg">
              <h3 className="font-medium text-gray-900 mb-2">Step 1: Create Manifest File</h3>
              <p className="text-gray-600 mb-2">Create a file at:</p>
              <code className="bg-gray-200 px-2 py-1 rounded text-xs">
                %USERPROFILE%\AppData\Roaming\Microsoft\AddIns\effyDOC\manifest.xml
              </code>
              <p className="text-gray-600 mt-2">
                <a href="/outlook-addin/manifest.xml" target="_blank" className="text-blue-600 hover:text-blue-800">
                  Download the manifest.xml file here
                </a>
              </p>
            </div>
            
            <div className="bg-gray-50 p-4 rounded-lg">
              <h3 className="font-medium text-gray-900 mb-2">Step 2: Register in Windows Registry</h3>
              <p className="text-gray-600 mb-2">Add registry entry:</p>
              <code className="bg-gray-200 px-2 py-1 rounded text-xs block">
                HKEY_CURRENT_USER\SOFTWARE\Microsoft\Office\16.0\WEF\Developer
                <br />
                Name: effyDOC
                <br />
                Value: [path to manifest.xml]
              </code>
            </div>
            
            <div className="bg-gray-50 p-4 rounded-lg">
              <h3 className="font-medium text-gray-900 mb-2">Step 3: Restart Outlook</h3>
              <p className="text-gray-600">
                Close Outlook completely and restart. The effyDOC panel should appear in the sidebar.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default OutlookPluginDownload;