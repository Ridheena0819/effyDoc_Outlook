#!/usr/bin/env python3
"""
Simple Outlook Plugin Installer Generator
Creates a working .exe installer for the effyDOC Outlook plugin
"""

import os
import sys
import base64
import json
from pathlib import Path
from datetime import datetime

def create_batch_installer(version="1.0.0", backend_url="http://localhost:8001", output_dir="./"):
    """Create a Windows batch installer that can be converted to .exe"""
    
    # Batch script content
    batch_content = f'''@echo off
title effyDOC Outlook Plugin Installer v{version}
color 1F
cls

echo.
echo    ___  __  __       ___   ___   ___ 
echo   / _ \\/ _^|/ _^|_   _^|   \\ / _ \\ / ___^|
echo  ^|  __/  _^|  _^| ^| ^| ^| ^|) ^| (_) ^| (__ 
echo   \\___|_^| ^|_^|  \\_, ^|___/ \\___/ \\___|
echo                ^|__/                 
echo.
echo      Outlook Plugin Installer v{version}
echo   ========================================
echo.

echo Welcome to the effyDOC Outlook Plugin installer!
echo This will install the plugin to work with Microsoft Outlook.
echo.
pause

echo Checking prerequisites...
echo.

REM Check if Outlook is installed
where outlook.exe >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [ERROR] Microsoft Outlook not found
    echo Please install Microsoft Outlook and try again
    echo.
    pause
    exit /b 1
)
echo [OK] Microsoft Outlook found

REM Check .NET Framework
reg query "HKLM\\SOFTWARE\\Microsoft\\NET Framework Setup\\NDP\\v4\\Full\\" /v Release >nul 2>&1
if %ERRORLEVEL% NEQ 0 (
    echo [WARNING] .NET Framework 4.7.2+ recommended
    echo Plugin may still work with older versions
) else (
    echo [OK] .NET Framework detected
)

echo.
echo Creating installation directories...

REM Create installation directories
set INSTALL_DIR=%USERPROFILE%\\AppData\\Roaming\\effyDOC\\OutlookPlugin
set MANIFEST_DIR=%USERPROFILE%\\AppData\\Roaming\\Microsoft\\AddIns\\effyDOC

if not exist "%INSTALL_DIR%" (
    mkdir "%INSTALL_DIR%" 2>nul
    if %ERRORLEVEL% EQU 0 (
        echo [OK] Created installation directory
    ) else (
        echo [ERROR] Failed to create installation directory
        pause
        exit /b 1
    )
) else (
    echo [OK] Installation directory exists
)

if not exist "%MANIFEST_DIR%" (
    mkdir "%MANIFEST_DIR%" 2>nul
    if %ERRORLEVEL% EQU 0 (
        echo [OK] Created manifest directory
    ) else (
        echo [ERROR] Failed to create manifest directory
        pause
        exit /b 1
    )
) else (
    echo [OK] Manifest directory exists
)

echo.
echo Creating plugin manifest...

REM Create the Outlook add-in manifest
(
echo ^<?xml version="1.0" encoding="UTF-8"?^>
echo ^<OfficeApp xmlns="http://schemas.microsoft.com/office/appforoffice/1.1"
echo            xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
echo            xsi:type="TaskPaneApp"^>
echo   ^<Id^>effydoc-outlook-plugin-2024^</Id^>
echo   ^<Version^>{version}^</Version^>
echo   ^<ProviderName^>effyDOC^</ProviderName^>
echo   ^<DefaultLocale^>en-US^</DefaultLocale^>
echo   ^<DisplayName DefaultValue="effyDOC Document Tracker"/^>
echo   ^<Description DefaultValue="Track document engagement in real-time directly from Outlook. Send trackable documents and see when recipients open, click, and read them."/^>
echo   ^<IconUrl DefaultValue="{backend_url}/icon-32.png"/^>
echo   ^<HighResolutionIconUrl DefaultValue="{backend_url}/icon-64.png"/^>
echo   ^<SupportUrl DefaultValue="{backend_url}/support"/^>
echo   ^<AppDomains^>
echo     ^<AppDomain^>{backend_url}^</AppDomain^>
echo   ^</AppDomains^>
echo   ^<Hosts^>
echo     ^<Host Name="Mailbox"/^>
echo   ^</Hosts^>
echo   ^<Requirements^>
echo     ^<Sets^>
echo       ^<Set Name="Mailbox" MinVersion="1.8"/^>
echo     ^</Sets^>
echo   ^</Requirements^>
echo   ^<FormSettings^>
echo     ^<Form xsi:type="ItemRead"^>
echo       ^<DesktopSettings^>
echo         ^<SourceLocation DefaultValue="{backend_url}/outlook-addin/"/^>
echo         ^<RequestedHeight^>500^</RequestedHeight^>
echo       ^</DesktopSettings^>
echo     ^</Form^>
echo     ^<Form xsi:type="ItemEdit"^>
echo       ^<DesktopSettings^>
echo         ^<SourceLocation DefaultValue="{backend_url}/outlook-addin/"/^>
echo         ^<RequestedHeight^>500^</RequestedHeight^>
echo       ^</DesktopSettings^>
echo     ^</Form^>
echo   ^</FormSettings^>
echo   ^<Permissions^>ReadWriteMailbox^</Permissions^>
echo   ^<Rule xsi:type="RuleCollection" Mode="Or"^>
echo     ^<Rule xsi:type="ItemIs" ItemType="Message" FormType="Read"/^>
echo     ^<Rule xsi:type="ItemIs" ItemType="Message" FormType="Edit"/^>
echo   ^</Rule^>
echo ^</OfficeApp^>
) > "%MANIFEST_DIR%\\manifest.xml"

if %ERRORLEVEL% EQU 0 (
    echo [OK] Manifest created successfully
) else (
    echo [ERROR] Failed to create manifest
    pause
    exit /b 1
)

echo.
echo Registering plugin with Outlook...

REM Register the add-in with Office 2016/2019/365
reg add "HKCU\\SOFTWARE\\Microsoft\\Office\\16.0\\WEF\\Developer" /v "effyDOC" /t REG_SZ /d "%MANIFEST_DIR%\\manifest.xml" /f >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [OK] Registered with Office 2016/2019/365
) else (
    echo [WARNING] Could not register with Office 2016/2019/365
)

REM Register the add-in with Office 2013
reg add "HKCU\\SOFTWARE\\Microsoft\\Office\\15.0\\WEF\\Developer" /v "effyDOC" /t REG_SZ /d "%MANIFEST_DIR%\\manifest.xml" /f >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo [OK] Registered with Office 2013
) else (
    echo [WARNING] Could not register with Office 2013
)

echo.
echo Creating configuration files...

REM Create plugin configuration
(
echo {{
echo   "pluginVersion": "{version}",
echo   "backendURL": "{backend_url}",
echo   "installDate": "%date% %time%",
echo   "installationPath": "%INSTALL_DIR%",
echo   "manifestPath": "%MANIFEST_DIR%\\manifest.xml",
echo   "features": {{
echo     "documentTracking": true,
echo     "realTimeAnalytics": true,
echo     "emailIntegration": true,
echo     "nativeOutlookIntegration": true
echo   }}
echo }}
) > "%INSTALL_DIR%\\config.json"

REM Create README file
(
echo effyDOC Outlook Plugin Installation Complete!
echo ==========================================
echo.
echo Installation Date: %date% %time%
echo Plugin Version: {version}
echo Installation Path: %INSTALL_DIR%
echo Manifest Location: %MANIFEST_DIR%\\manifest.xml
echo Backend URL: {backend_url}
echo.
echo SUCCESS - INSTALLATION COMPLETED!
echo.
echo Next Steps:
echo 1. Restart Microsoft Outlook ^(if currently running^)
echo 2. Look for the effyDOC panel in your Outlook sidebar
echo 3. Sign in with your effyDOC account credentials
echo 4. Start tracking your document engagement!
echo.
echo Features Available:
echo • Track when emails are opened
echo • Monitor link clicks
echo • See page-by-page reading analytics
echo • Real-time engagement notifications
echo • Send trackable document attachments
echo • Live dashboard with metrics
echo.
echo Troubleshooting:
echo • If plugin doesn't appear: Restart Outlook completely
echo • Ensure you have an active effyDOC account
echo • Check that Outlook allows add-ins ^(File → Options → Add-ins^)
echo • Verify internet connection for real-time features
echo.
echo Support Resources:
echo • Platform Dashboard: {backend_url}
echo • Help Documentation: Available in the plugin interface
echo.
echo Thank you for choosing effyDOC! 🚀
) > "%INSTALL_DIR%\\README.txt"

REM Create uninstaller
(
echo @echo off
echo title effyDOC Outlook Plugin Uninstaller
echo.
echo Removing effyDOC Outlook Plugin...
echo.
echo Removing registry entries...
reg delete "HKCU\\SOFTWARE\\Microsoft\\Office\\16.0\\WEF\\Developer" /v "effyDOC" /f ^>nul 2^>^&1
reg delete "HKCU\\SOFTWARE\\Microsoft\\Office\\15.0\\WEF\\Developer" /v "effyDOC" /f ^>nul 2^>^&1
echo.
echo Removing installation files...
rmdir /s /q "%INSTALL_DIR%" ^>nul 2^>^&1
rmdir /s /q "%MANIFEST_DIR%" ^>nul 2^>^&1
echo.
echo effyDOC Outlook Plugin removed successfully!
echo Please restart Outlook to complete the removal.
echo.
pause
) > "%INSTALL_DIR%\\Uninstall.bat"

echo [OK] Configuration files created

echo.
echo ===================================
echo    INSTALLATION COMPLETED!
echo ===================================
echo.
echo The effyDOC Outlook Plugin has been successfully installed.
echo.
echo Next Steps:
echo 1. Restart Microsoft Outlook ^(if currently running^)
echo 2. Look for the effyDOC panel in your Outlook sidebar
echo 3. Sign in with your effyDOC account
echo 4. Start tracking documents!
echo.
echo Installation guide: %INSTALL_DIR%\\README.txt
echo Uninstaller: %INSTALL_DIR%\\Uninstall.bat
echo.

set /p RESTART="Would you like to start Microsoft Outlook now? (Y/N): "
if /i "%RESTART%"=="Y" (
    echo Starting Microsoft Outlook...
    start outlook.exe
    if %ERRORLEVEL% EQU 0 (
        echo [OK] Outlook started successfully
    ) else (
        echo [WARNING] Could not start Outlook automatically
        echo Please start Outlook manually
    )
)

echo.
echo Thank you for using effyDOC! 🚀
echo.
pause
'''

    # Save the batch file
    output_path = Path(output_dir) / "EffyDocOutlookPlugin-Setup.bat"
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(batch_content)
    
    return output_path

def create_installer_info(version, backend_url):
    """Create installer metadata"""
    return {
        "version": version,
        "backend_url": backend_url,
        "created_at": datetime.now().isoformat(),
        "installer_type": "batch_to_exe",
        "features": [
            "Document tracking",
            "Real-time analytics", 
            "Email integration",
            "Native Outlook integration"
        ],
        "requirements": [
            "Windows 7 or later",
            "Microsoft Outlook 2013 or later",
            ".NET Framework 4.0+"
        ]
    }

if __name__ == "__main__":
    # Get parameters from command line or use defaults
    version = sys.argv[1] if len(sys.argv) > 1 else "1.0.0"
    backend_url = sys.argv[2] if len(sys.argv) > 2 else "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com"
    output_dir = sys.argv[3] if len(sys.argv) > 3 else "../frontend/public"
    
    print(f"🚀 Creating effyDOC Outlook Plugin Installer v{version}")
    print(f"📡 Backend URL: {backend_url}")
    print(f"📁 Output Directory: {output_dir}")
    print()
    
    # Ensure output directory exists
    Path(output_dir).mkdir(parents=True, exist_ok=True)
    
    # Create the batch installer
    installer_path = create_batch_installer(version, backend_url, output_dir)
    print(f"✅ Batch installer created: {installer_path}")
    
    # Create installer info
    info = create_installer_info(version, backend_url)
    info_path = Path(output_dir) / "installer-info.json"
    with open(info_path, 'w') as f:
        json.dump(info, f, indent=2)
    print(f"📋 Installer info created: {info_path}")
    
    # Calculate file size
    file_size = installer_path.stat().st_size
    print(f"📊 Installer size: {file_size:,} bytes ({file_size/1024:.1f} KB)")
    
    print()
    print("🎉 Installer generation completed!")
    print()
    print("The batch file can be converted to .exe using:")
    print("1. Bat2Exe converter")
    print("2. Advanced BAT to EXE Converter") 
    print("3. IExpress (built into Windows)")
    print()
    print("Or users can run the .bat file directly for installation.")