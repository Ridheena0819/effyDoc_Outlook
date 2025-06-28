# Build and Deployment Guide for effyDOC Native Outlook Plugin

## 🎯 Overview

This guide provides step-by-step instructions for building and deploying the effyDOC native Outlook plugin as a downloadable .exe installer.

## 🔧 Prerequisites

### Required Software (Windows Only)
1. **Visual Studio 2019/2022 Community/Professional**
   - Workload: Office/SharePoint development
   - .NET Framework 4.7.2 development tools
   - VSTO development components

2. **Microsoft Office 2013/2016/2019/365**
   - For development and testing

3. **NSIS (Nullsoft Scriptable Install System)**
   - Version 3.0 or higher
   - Download from: https://nsis.sourceforge.io/

4. **Optional Tools**
   - WiX Toolset (alternative to NSIS)
   - Code signing certificate
   - Git for version control

## 📋 Step-by-Step Build Process

### Step 1: Environment Setup

```bash
# 1. Install Visual Studio with Office development tools
# 2. Install NSIS
# 3. Clone or download the project files
# 4. Ensure all prerequisites are met
```

### Step 2: Create VSTO Project

1. **Open Visual Studio**
2. **Create New Project:**
   ```
   File → New → Project
   Templates → Visual C# → Office/SharePoint → Add-ins
   Select: Outlook VSTO Add-in
   Name: EffyDocOutlookPlugin
   ```

3. **Replace Generated Files:**
   - Copy all provided source files into the project
   - Ensure proper folder structure

### Step 3: Configure Project

1. **Install NuGet Packages:**
   ```
   Tools → NuGet Package Manager → Package Manager Console
   Install-Package Newtonsoft.Json -Version 13.0.3
   ```

2. **Add References:**
   - Microsoft.Office.Interop.Outlook
   - Microsoft.Office.Tools.Outlook
   - System.Windows.Forms
   - System.Net.Http

3. **Set Target Framework:**
   - Right-click project → Properties
   - Set Target Framework to .NET Framework 4.7.2

### Step 4: Build Configuration

1. **Debug Configuration (Development):**
   ```
   Build → Configuration Manager
   Active solution configuration: Debug
   Platform: Any CPU
   ```

2. **Release Configuration (Production):**
   ```
   Build → Configuration Manager
   Active solution configuration: Release
   Platform: Any CPU
   Deploy: Checked
   ```

### Step 5: Compile the Plugin

1. **Clean Solution:**
   ```
   Build → Clean Solution
   ```

2. **Rebuild Solution:**
   ```
   Build → Rebuild Solution
   ```

3. **Verify Output:**
   ```
   Check bin\Release\ folder for:
   - EffyDocOutlookPlugin.dll
   - EffyDocOutlookPlugin.dll.manifest
   - EffyDocOutlookPlugin.vsto
   - All dependencies
   ```

### Step 6: Create Installer

#### Option A: NSIS Installer (Recommended)

1. **Prepare Installer Assets:**
   ```
   Create Installer folder with:
   - EffyDocNativeInstaller.nsi (provided)
   - effydoc-icon.ico
   - installer-banner.bmp
   - License.txt
   ```

2. **Update NSIS Script:**
   ```nsis
   # Update file paths in EffyDocNativeInstaller.nsi
   File "path\to\EffyDocOutlookPlugin.dll"
   File "path\to\EffyDocOutlookPlugin.dll.manifest"
   File "path\to\EffyDocOutlookPlugin.vsto"
   File "path\to\Newtonsoft.Json.dll"
   ```

3. **Compile Installer:**
   ```
   Right-click EffyDocNativeInstaller.nsi
   Select: Compile NSIS Script
   Output: EffyDocOutlookPlugin-Setup.exe
   ```

#### Option B: WiX Installer

1. **Create WiX Project:**
   ```
   Add → New Project → Windows Installer XML → Setup Project
   ```

2. **Configure WiX Project:**
   - Reference main project
   - Update Product.wxs with provided content
   - Build WiX project

### Step 7: Code Signing (Production)

1. **Obtain Certificate:**
   - Purchase from trusted CA (DigiCert, Sectigo, etc.)
   - Or create self-signed for testing

2. **Sign Files:**
   ```bash
   # Sign main DLL
   signtool sign /f "certificate.pfx" /p "password" /t "http://timestamp.digicert.com" "EffyDocOutlookPlugin.dll"
   
   # Sign installer
   signtool sign /f "certificate.pfx" /p "password" /t "http://timestamp.digicert.com" "EffyDocOutlookPlugin-Setup.exe"
   ```

### Step 8: Testing

1. **Local Testing:**
   ```
   - Install plugin on development machine
   - Test all functionality
   - Verify Outlook integration
   - Check API connectivity
   ```

2. **Clean Machine Testing:**
   ```
   - Test on fresh Windows installation
   - Verify prerequisites detection
   - Test installation and uninstallation
   - Validate user experience
   ```

## 📦 Distribution

### Direct Download
1. **Upload to web server**
2. **Provide download link**
3. **Include installation instructions**

### Enterprise Distribution
1. **SCCM Package**
2. **Group Policy deployment**
3. **Network share installation**

## 🔍 Troubleshooting Build Issues

### Common Build Errors

1. **Missing Office References:**
   ```
   Solution: Install Office development tools in Visual Studio
   ```

2. **VSTO Runtime Missing:**
   ```
   Solution: Install Visual Studio Tools for Office Runtime
   ```

3. **Certificate Errors:**
   ```
   Solution: Create test certificate or disable signing for development
   ```

4. **Manifest Errors:**
   ```
   Solution: Check TargetFrameworkVersion and references
   ```

### Common Installer Issues

1. **NSIS Compilation Errors:**
   ```
   Solution: Check file paths and NSIS syntax
   ```

2. **File Not Found:**
   ```
   Solution: Verify all files exist in specified paths
   ```

3. **Permission Errors:**
   ```
   Solution: Run NSIS as administrator
   ```

## 📊 Build Automation

### PowerShell Build Script

```powershell
# BuildEffyDocPlugin.ps1
param(
    [string]$Configuration = "Release",
    [string]$Platform = "Any CPU"
)

# Build solution
MSBuild.exe "EffyDocOutlookPlugin.sln" /p:Configuration=$Configuration /p:Platform=$Platform

# Sign files (if certificate available)
if (Test-Path "certificate.pfx") {
    signtool sign /f "certificate.pfx" /p $env:CERT_PASSWORD /t "http://timestamp.digicert.com" "bin\$Configuration\EffyDocOutlookPlugin.dll"
}

# Create installer
makensis "Installer\EffyDocNativeInstaller.nsi"

Write-Host "Build completed successfully!"
```

### Batch Build Script

```batch
@echo off
echo Building effyDOC Outlook Plugin...

REM Clean and build
MSBuild.exe EffyDocOutlookPlugin.sln /t:Clean /p:Configuration=Release
MSBuild.exe EffyDocOutlookPlugin.sln /t:Build /p:Configuration=Release

REM Create installer
cd Installer
makensis EffyDocNativeInstaller.nsi
cd ..

echo Build completed!
pause
```

## 🎨 Customization

### Branding Updates
1. **Icons**: Replace icon files
2. **Colors**: Update ribbon XML
3. **Text**: Modify resource files
4. **URLs**: Update API endpoints in config

### Feature Modifications
1. **Add new ribbon buttons**
2. **Extend API service**
3. **Add new dialog forms**
4. **Implement additional tracking**

## 📋 Deployment Checklist

- [ ] All source files compiled successfully
- [ ] Dependencies included in output
- [ ] Installer creates proper registry entries
- [ ] Plugin appears in Outlook ribbon
- [ ] API connectivity working
- [ ] Error handling tested
- [ ] Installation/uninstallation tested
- [ ] Code signing completed (production)
- [ ] Documentation updated
- [ ] User testing completed

## 🔄 Update Process

### For Plugin Updates
1. **Increment version number**
2. **Update AssemblyInfo.cs**
3. **Rebuild solution**
4. **Create new installer**
5. **Test upgrade scenario**

### For API Updates
1. **Update service endpoints**
2. **Test compatibility**
3. **Update documentation**

---

**Note**: This build process creates a native Windows installer that installs a VSTO add-in directly into Outlook, appearing as a native ribbon component like "saleshandy" in the user's screenshot.