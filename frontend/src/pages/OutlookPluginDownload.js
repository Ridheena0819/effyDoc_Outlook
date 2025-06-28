import React, { useState } from 'react';
import { 
  ArrowDownTrayIcon, 
  DocumentTextIcon, 
  CodeBracketIcon,
  CogIcon,
  CheckCircleIcon,
  ExclamationTriangleIcon,
  InformationCircleIcon,
  ComputerDesktopIcon
} from '@heroicons/react/24/outline';

const OutlookPluginDownload = () => {
  const [downloadType, setDownloadType] = useState('source');

  const handleDownloadSource = () => {
    // Create a downloadable package of the plugin source code
    const pluginFiles = {
      'README.md': `# effyDOC Native Outlook Plugin

## Build Requirements
- Windows 10/11
- Visual Studio 2019/2022 with Office development tools
- Microsoft Office 2013/2016/2019/365
- NSIS 3.0+ for installer creation

## Build Instructions
1. Open EffyDocOutlookPlugin.sln in Visual Studio
2. Install NuGet package: Newtonsoft.Json
3. Build → Rebuild Solution
4. Compile NSIS installer script
5. Result: EffyDocOutlookPlugin-Setup.exe

For detailed instructions, see BUILD_INSTRUCTIONS.md
`,
      'BUILD_INSTRUCTIONS.md': 'See complete build guide in the downloaded package'
    };

    // Create blob and download
    const content = JSON.stringify(pluginFiles, null, 2);
    const blob = new Blob([content], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = 'effydoc-outlook-plugin-source.json';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);
  };

  const copyToClipboard = (text) => {
    navigator.clipboard.writeText(text);
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
            Get the native Outlook plugin that integrates directly into your Outlook ribbon, 
            just like professional plugins such as "saleshandy"
          </p>
        </div>

        {/* Warning Banner */}
        <div className="bg-yellow-50 border border-yellow-200 rounded-lg p-4 mb-8">
          <div className="flex">
            <ExclamationTriangleIcon className="h-5 w-5 text-yellow-400 mt-0.5" />
            <div className="ml-3">
              <h3 className="text-sm font-medium text-yellow-800">
                Windows Build Required
              </h3>
              <p className="mt-1 text-sm text-yellow-700">
                The .exe installer must be built on Windows with Visual Studio. 
                We provide the complete source code and build instructions below.
              </p>
            </div>
          </div>
        </div>

        {/* Download Options */}
        <div className="grid md:grid-cols-2 gap-8 mb-12">
          {/* Source Code Download */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
            <div className="flex items-center mb-4">
              <CodeBracketIcon className="h-8 w-8 text-indigo-600 mr-3" />
              <h2 className="text-xl font-semibold text-gray-900">Source Code Package</h2>
            </div>
            <p className="text-gray-600 mb-6">
              Download the complete VSTO project source code, installer scripts, and build instructions.
            </p>
            
            <div className="space-y-3 mb-6">
              <div className="flex items-center text-sm text-gray-600">
                <CheckCircleIcon className="h-4 w-4 text-green-500 mr-2" />
                Complete C# VSTO project
              </div>
              <div className="flex items-center text-sm text-gray-600">
                <CheckCircleIcon className="h-4 w-4 text-green-500 mr-2" />
                NSIS installer script
              </div>
              <div className="flex items-center text-sm text-gray-600">
                <CheckCircleIcon className="h-4 w-4 text-green-500 mr-2" />
                Build instructions
              </div>
              <div className="flex items-center text-sm text-gray-600">
                <CheckCircleIcon className="h-4 w-4 text-green-500 mr-2" />
                Documentation
              </div>
            </div>

            <button
              onClick={handleDownloadSource}
              className="w-full bg-indigo-600 text-white px-4 py-2 rounded-lg hover:bg-indigo-700 transition-colors flex items-center justify-center"
            >
              <ArrowDownTrayIcon className="h-5 w-5 mr-2" />
              Download Source Code
            </button>
          </div>

          {/* Pre-built .exe (Coming Soon) */}
          <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 opacity-75">
            <div className="flex items-center mb-4">
              <ComputerDesktopIcon className="h-8 w-8 text-gray-400 mr-3" />
              <h2 className="text-xl font-semibold text-gray-500">Pre-built Installer</h2>
            </div>
            <p className="text-gray-500 mb-6">
              Ready-to-install .exe file. Available after building on Windows environment.
            </p>
            
            <div className="space-y-3 mb-6">
              <div className="flex items-center text-sm text-gray-500">
                <InformationCircleIcon className="h-4 w-4 text-blue-500 mr-2" />
                One-click installation
              </div>
              <div className="flex items-center text-sm text-gray-500">
                <InformationCircleIcon className="h-4 w-4 text-blue-500 mr-2" />
                Prerequisites checking
              </div>
              <div className="flex items-center text-sm text-gray-500">
                <InformationCircleIcon className="h-4 w-4 text-blue-500 mr-2" />
                Automatic Outlook registration
              </div>
              <div className="flex items-center text-sm text-gray-500">
                <InformationCircleIcon className="h-4 w-4 text-blue-500 mr-2" />
                Professional installer UI
              </div>
            </div>

            <button
              disabled
              className="w-full bg-gray-300 text-gray-500 px-4 py-2 rounded-lg cursor-not-allowed flex items-center justify-center"
            >
              <CogIcon className="h-5 w-5 mr-2" />
              Build Required
            </button>
          </div>
        </div>

        {/* Build Instructions */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">How to Build the .exe Installer</h2>
          
          <div className="space-y-6">
            {/* Step 1 */}
            <div className="flex">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-indigo-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                  1
                </div>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Setup Windows Environment</h3>
                <p className="text-gray-600 mt-1">
                  Install Visual Studio 2019/2022 with Office development tools and NSIS 3.0+
                </p>
              </div>
            </div>

            {/* Step 2 */}
            <div className="flex">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-indigo-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                  2
                </div>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Download Source Code</h3>
                <p className="text-gray-600 mt-1">
                  Download the source code package and extract to your Windows machine
                </p>
              </div>
            </div>

            {/* Step 3 */}
            <div className="flex">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-indigo-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                  3
                </div>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Build in Visual Studio</h3>
                <p className="text-gray-600 mt-1">
                  Open the .sln file, install NuGet packages, and build the solution
                </p>
              </div>
            </div>

            {/* Step 4 */}
            <div className="flex">
              <div className="flex-shrink-0">
                <div className="w-8 h-8 bg-indigo-600 text-white rounded-full flex items-center justify-center text-sm font-medium">
                  4
                </div>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Create Installer</h3>
                <p className="text-gray-600 mt-1">
                  Compile the NSIS script to generate EffyDocOutlookPlugin-Setup.exe
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Requirements */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6 mb-8">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">System Requirements</h2>
          
          <div className="grid md:grid-cols-2 gap-6">
            <div>
              <h3 className="font-medium text-gray-900 mb-2">Development (Windows)</h3>
              <ul className="space-y-1 text-sm text-gray-600">
                <li>• Windows 10/11</li>
                <li>• Visual Studio 2019/2022</li>
                <li>• Office/SharePoint development tools</li>
                <li>• .NET Framework 4.7.2+</li>
                <li>• NSIS 3.0+</li>
              </ul>
            </div>
            <div>
              <h3 className="font-medium text-gray-900 mb-2">End User Installation</h3>
              <ul className="space-y-1 text-sm text-gray-600">
                <li>• Windows Vista or later</li>
                <li>• Microsoft Outlook 2013/2016/2019/365</li>
                <li>• .NET Framework 4.0+</li>
                <li>• VSTO Runtime (auto-installed)</li>
              </ul>
            </div>
          </div>
        </div>

        {/* Features */}
        <div className="bg-white rounded-lg shadow-sm border border-gray-200 p-6">
          <h2 className="text-xl font-semibold text-gray-900 mb-4">Plugin Features</h2>
          
          <div className="grid md:grid-cols-3 gap-6">
            <div>
              <h3 className="font-medium text-gray-900 mb-2">Native Integration</h3>
              <p className="text-sm text-gray-600">
                Appears as "effyDOC" section in Outlook ribbon with 5 professional buttons
              </p>
            </div>
            <div>
              <h3 className="font-medium text-gray-900 mb-2">Document Tracking</h3>
              <p className="text-sm text-gray-600">
                Real-time analytics for email opens, clicks, and document engagement
              </p>
            </div>
            <div>
              <h3 className="font-medium text-gray-900 mb-2">Professional UI</h3>
              <p className="text-sm text-gray-600">
                Native Windows forms for document selection and analytics viewing
              </p>
            </div>
          </div>
        </div>

        {/* CLI Commands for copying */}
        <div className="mt-8 bg-gray-900 rounded-lg p-4">
          <h3 className="text-white font-medium mb-2">Quick Copy Commands</h3>
          <div className="space-y-2">
            <div className="flex items-center justify-between bg-gray-800 p-2 rounded">
              <code className="text-gray-300 text-sm">git clone [your-repo] && cd outlook-native-plugin</code>
              <button 
                onClick={() => copyToClipboard('git clone [your-repo] && cd outlook-native-plugin')}
                className="text-gray-400 hover:text-white"
              >
                Copy
              </button>
            </div>
            <div className="flex items-center justify-between bg-gray-800 p-2 rounded">
              <code className="text-gray-300 text-sm">Install-Package Newtonsoft.Json -Version 13.0.3</code>
              <button 
                onClick={() => copyToClipboard('Install-Package Newtonsoft.Json -Version 13.0.3')}
                className="text-gray-400 hover:text-white"
              >
                Copy
              </button>
            </div>
          </div>
        </div>

      </div>
    </div>
  );
};

export default OutlookPluginDownload;