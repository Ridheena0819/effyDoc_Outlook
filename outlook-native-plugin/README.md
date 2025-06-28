# effyDOC Native Outlook Plugin

This is a native Microsoft Outlook VSTO (Visual Studio Tools for Office) add-in that provides document tracking and attachment capabilities directly in the Outlook ribbon.

## 🎯 Features

- **Native Outlook Integration**: Appears as "effyDOC" section in Outlook ribbon
- **One-Click Document Attachment**: Attach trackable documents from your effyDOC library
- **Real-Time Analytics**: View document engagement metrics directly in Outlook
- **Professional Document Creation**: Create new proposals and contracts
- **Email Tracking**: Track when recipients open emails and click document links

## 🏗️ Project Structure

```
EffyDocOutlookPlugin/
├── EffyDocOutlookPlugin.csproj          # Main VSTO project file
├── ThisAddIn.cs                         # Add-in entry point and event handlers
├── EffyDocRibbon.cs/.Designer.cs        # Ribbon UI and button handlers
├── DocumentSelector.cs/.Designer.cs     # Document selection dialog
├── TrackingPanel.cs/.Designer.cs        # Analytics viewing panel
├── Services/
│   └── EffyDocApiService.cs             # API communication service
├── Models/
│   └── DocumentModel.cs                 # Data models
└── Properties/
    └── AssemblyInfo.cs                  # Assembly metadata

Installer/
├── EffyDocNativeInstaller.nsi           # NSIS installer script
├── EffyDocOutlookInstaller.wxs          # WiX installer project
└── Assets/                              # Installer images and icons
```

## 🔧 Build Requirements

### Prerequisites
- **Windows 10/11** (required for compilation)
- **Visual Studio 2019/2022** with:
  - .NET Framework 4.7.2 or higher
  - Office/SharePoint development tools
  - VSTO development tools
- **Microsoft Office 2013/2016/2019/365** (for testing)

### Build Tools
- **NSIS 3.0+** (for .exe installer creation)
- **WiX Toolset 3.11+** (alternative installer)
- **Code signing certificate** (recommended for production)

## 🚀 Compilation Instructions

### Step 1: Setup Development Environment

1. **Install Visual Studio** with Office development tools:
   ```
   Visual Studio Installer → Modify → Workloads → Office/SharePoint development
   ```

2. **Install VSTO Runtime** (if not included):
   ```
   Download from: https://aka.ms/vstoruntimedownload
   ```

3. **Install NSIS** for installer creation:
   ```
   Download from: https://nsis.sourceforge.io/Download
   ```

### Step 2: Build the Plugin

1. **Open Visual Studio** and create new project:
   ```
   File → New → Project → Office/SharePoint → Add-ins → Outlook VSTO Add-in
   ```

2. **Replace default files** with the provided source code files

3. **Restore NuGet packages**:
   ```
   Tools → NuGet Package Manager → Package Manager Console
   Update-Package -Reinstall
   ```

4. **Build the project**:
   ```
   Build → Build Solution (Ctrl+Shift+B)
   ```

### Step 3: Create the Installer

#### Option A: Using NSIS (Recommended)

1. **Install NSIS** with additional plugins
2. **Open NSIS compiler** 
3. **Compile the installer script**:
   ```
   Right-click EffyDocNativeInstaller.nsi → Compile NSIS Script
   ```

#### Option B: Using WiX Toolset

1. **Install WiX Toolset**
2. **Add WiX project** to solution
3. **Build WiX project** to generate MSI installer

### Step 4: Code Signing (Production)

1. **Obtain code signing certificate**
2. **Sign the DLL and installer**:
   ```bash
   signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com EffyDocOutlookPlugin.dll
   signtool sign /f certificate.pfx /p password /t http://timestamp.digicert.com EffyDocOutlookPlugin-Setup.exe
   ```

## 📦 Deployment

### For End Users

1. **Download** `EffyDocOutlookPlugin-Setup.exe`
2. **Run installer** as administrator (if required)
3. **Follow installation wizard**
4. **Restart Outlook**
5. **Look for "effyDOC" section** in Outlook ribbon

### For IT Administrators

- **Silent Installation**: 
  ```bash
  EffyDocOutlookPlugin-Setup.exe /S
  ```
- **Group Policy Deployment**: Use MSI installer with GPO
- **SCCM Deployment**: Package as application with MSI

## 🔍 Troubleshooting

### Plugin Not Appearing
1. **Check Office version compatibility**
2. **Verify VSTO Runtime installation**
3. **Check Windows Event Logs** for errors
4. **Restart Outlook completely**

### API Connection Issues
1. **Check internet connectivity**
2. **Verify effyDOC account credentials**
3. **Check firewall/proxy settings**
4. **Validate API endpoint accessibility**

### Installation Failures
1. **Run as administrator**
2. **Check Windows version compatibility**
3. **Verify .NET Framework version**
4. **Close all Office applications**

## 📁 File Locations

### Installation Directory
```
%LOCALAPPDATA%\effyDOC Outlook Plugin\
├── EffyDocOutlookPlugin.dll
├── EffyDocOutlookPlugin.dll.manifest
├── EffyDocOutlookPlugin.vsto
├── Newtonsoft.Json.dll
└── README.txt
```

### Registry Entries
```
HKCU\SOFTWARE\Microsoft\Office\Outlook\Addins\EffyDocOutlookPlugin
HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\effyDOC Outlook Plugin
```

## 🔐 Security Considerations

- **Code Signing**: Always sign production releases
- **API Security**: Uses HTTPS and JWT tokens
- **User Data**: No sensitive data stored locally
- **Registry**: Only user-level registry modifications

## 🆙 Updates

### Automatic Updates
- **ClickOnce deployment** can be configured for auto-updates
- **Check for updates** functionality in settings

### Manual Updates
- **Download new installer**
- **Run over existing installation**
- **Restart Outlook**

## 📞 Support

- **Documentation**: See README.txt after installation
- **API Integration**: Uses existing effyDOC backend endpoints
- **Error Logging**: Check Windows Event Viewer under Applications

## 🎨 Customization

### Branding
- **Icons**: Replace files in `Icons/` directory
- **Colors**: Modify ribbon designer properties
- **Text**: Update string resources

### API Endpoints
- **Base URL**: Configured in `app.config`
- **Authentication**: JWT token-based
- **Error Handling**: Graceful degradation

---

**Note**: This is a native Windows-only solution. For cross-platform support, use the existing Office.js web-based add-in.