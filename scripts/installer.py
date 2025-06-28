#!/usr/bin/env python3
"""
effyDOC Outlook Plugin Installer
Creates and runs the installer for the effyDOC Outlook plugin
"""

import os
import sys
import json
import subprocess
import winreg
from pathlib import Path
from datetime import datetime

def main():
    """Main installer function"""
    print_banner()
    
    # Configuration
    VERSION = "1.0.0"
    BACKEND_URL = "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com"
    
    if not check_prerequisites():
        input("Press Enter to exit...")
        sys.exit(1)
    
    if not confirm_installation():
        print("Installation cancelled by user.")
        sys.exit(0)
    
    if install_plugin(VERSION, BACKEND_URL):
        print("\n🎉 Installation completed successfully!")
        print("\nNext Steps:")
        print("1. Restart Microsoft Outlook")
        print("2. Look for effyDOC panel in Outlook sidebar")
        print("3. Sign in with your effyDOC account")
        print("4. Start tracking documents!")
        
        if input("\nWould you like to start Microsoft Outlook now? (Y/N): ").lower() == 'y':
            start_outlook()
    else:
        print("\n❌ Installation failed")
        sys.exit(1)
    
    input("\nPress Enter to exit...")

def print_banner():
    """Print the installer banner"""
    print("""
   ___  __  __       ___   ___   ___ 
  / _ \\/ _|/ _|_   _|   \\ / _ \\ / __|
 |  __/  _|  _| | | | |) | (_) | (__ 
  \\___|_| |_|  \\_, |___/ \\___/ \\___|
               |__/                 

     Outlook Plugin Installer v1.0.0
  =======================================
""")
    print("Welcome to the effyDOC Outlook Plugin installer!")
    print("This installer will:")
    print("  • Create plugin manifest and configuration")
    print("  • Register plugin with Microsoft Outlook")
    print("  • Set up document tracking capabilities")
    print("  • Provide setup instructions")
    print()

def check_prerequisites():
    """Check system prerequisites"""
    print("Checking prerequisites...")
    
    # Check Windows version
    if os.name != 'nt':
        print("❌ This installer is for Windows only")
        return False
    print("✅ Windows system detected")
    
    # Check for Outlook
    outlook_found = False
    outlook_paths = [
        r"C:\Program Files\Microsoft Office\root\Office16\OUTLOOK.EXE",
        r"C:\Program Files (x86)\Microsoft Office\root\Office16\OUTLOOK.EXE",
        r"C:\Program Files\Microsoft Office\Office16\OUTLOOK.EXE",
        r"C:\Program Files (x86)\Microsoft Office\Office16\OUTLOOK.EXE",
        r"C:\Program Files\Microsoft Office\Office15\OUTLOOK.EXE",
        r"C:\Program Files (x86)\Microsoft Office\Office15\OUTLOOK.EXE"
    ]
    
    for path in outlook_paths:
        if os.path.exists(path):
            print(f"✅ Microsoft Outlook found: {path}")
            outlook_found = True
            break
    
    if not outlook_found:
        # Try to find Outlook via registry
        try:
            with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, r"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE") as key:
                outlook_path = winreg.QueryValue(key, "")
                if os.path.exists(outlook_path):
                    print(f"✅ Microsoft Outlook found: {outlook_path}")
                    outlook_found = True
        except:
            pass
    
    if not outlook_found:
        print("❌ Microsoft Outlook not found")
        print("Please install Microsoft Outlook and try again")
        return False
    
    return True

def confirm_installation():
    """Confirm installation with user"""
    response = input("Continue with installation? (Y/N): ")
    return response.lower() in ['y', 'yes']

def install_plugin(version, backend_url):
    """Install the Outlook plugin"""
    try:
        print("\nCreating installation directories...")
        
        # Create directories
        install_dir = Path.home() / "AppData" / "Roaming" / "effyDOC" / "OutlookPlugin"
        manifest_dir = Path.home() / "AppData" / "Roaming" / "Microsoft" / "AddIns" / "effyDOC"
        
        install_dir.mkdir(parents=True, exist_ok=True)
        manifest_dir.mkdir(parents=True, exist_ok=True)
        
        print(f"✅ Created installation directory: {install_dir}")
        print(f"✅ Created manifest directory: {manifest_dir}")
        
        # Create manifest file
        print("\nCreating plugin manifest...")
        manifest_content = f"""<?xml version="1.0" encoding="UTF-8"?>
<OfficeApp xmlns="http://schemas.microsoft.com/office/appforoffice/1.1"
           xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
           xsi:type="TaskPaneApp">
  
  <Id>effydoc-outlook-plugin-2024</Id>
  <Version>{version}</Version>
  <ProviderName>effyDOC</ProviderName>
  <DefaultLocale>en-US</DefaultLocale>
  <DisplayName DefaultValue="effyDOC Document Tracker"/>
  <Description DefaultValue="Track document engagement in real-time directly from Outlook. Send trackable documents and see when recipients open, click, and read them."/>
  
  <IconUrl DefaultValue="{backend_url}/icon-32.png"/>
  <HighResolutionIconUrl DefaultValue="{backend_url}/icon-64.png"/>
  <SupportUrl DefaultValue="{backend_url}/support"/>
  
  <AppDomains>
    <AppDomain>{backend_url}</AppDomain>
  </AppDomains>
  
  <Hosts>
    <Host Name="Mailbox"/>
  </Hosts>
  
  <Requirements>
    <Sets>
      <Set Name="Mailbox" MinVersion="1.8"/>
    </Sets>
  </Requirements>
  
  <FormSettings>
    <Form xsi:type="ItemRead">
      <DesktopSettings>
        <SourceLocation DefaultValue="{backend_url}/outlook-addin/"/>
        <RequestedHeight>500</RequestedHeight>
      </DesktopSettings>
    </Form>
    <Form xsi:type="ItemEdit">
      <DesktopSettings>
        <SourceLocation DefaultValue="{backend_url}/outlook-addin/"/>
        <RequestedHeight>500</RequestedHeight>
      </DesktopSettings>
    </Form>
  </FormSettings>
  
  <Permissions>ReadWriteMailbox</Permissions>
  
  <Rule xsi:type="RuleCollection" Mode="Or">
    <Rule xsi:type="ItemIs" ItemType="Message" FormType="Read"/>
    <Rule xsi:type="ItemIs" ItemType="Message" FormType="Edit"/>
  </Rule>
  
</OfficeApp>"""
        
        manifest_path = manifest_dir / "manifest.xml"
        with open(manifest_path, 'w', encoding='utf-8') as f:
            f.write(manifest_content)
        print(f"✅ Manifest created: {manifest_path}")
        
        # Register plugin in Windows registry
        print("\nRegistering plugin with Outlook...")
        
        try:
            # Office 2016/2019/365
            with winreg.CreateKey(winreg.HKEY_CURRENT_USER, r"SOFTWARE\Microsoft\Office\16.0\WEF\Developer") as key:
                winreg.SetValueEx(key, "effyDOC", 0, winreg.REG_SZ, str(manifest_path))
            print("✅ Registered with Office 2016/2019/365")
        except Exception as e:
            print(f"⚠️ Could not register with Office 2016/2019/365: {e}")
        
        try:
            # Office 2013
            with winreg.CreateKey(winreg.HKEY_CURRENT_USER, r"SOFTWARE\Microsoft\Office\15.0\WEF\Developer") as key:
                winreg.SetValueEx(key, "effyDOC", 0, winreg.REG_SZ, str(manifest_path))
            print("✅ Registered with Office 2013")
        except Exception as e:
            print(f"⚠️ Could not register with Office 2013: {e}")
        
        # Create configuration files
        print("\nCreating configuration files...")
        
        config = {
            "pluginVersion": version,
            "backendURL": backend_url,
            "installDate": datetime.now().isoformat(),
            "installationPath": str(install_dir),
            "manifestPath": str(manifest_path),
            "features": {
                "documentTracking": True,
                "realTimeAnalytics": True,
                "emailIntegration": True,
                "nativeOutlookIntegration": True
            }
        }
        
        with open(install_dir / "config.json", 'w') as f:
            json.dump(config, f, indent=2)
        
        # Create README
        readme_content = f"""effyDOC Outlook Plugin Installation Complete!
==========================================

Installation Date: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}
Plugin Version: {version}
Installation Path: {install_dir}
Manifest Location: {manifest_path}
Backend URL: {backend_url}

✅ INSTALLATION SUCCESSFUL

Next Steps:
1. Restart Microsoft Outlook (if currently running)
2. Look for the effyDOC panel in your Outlook sidebar
3. Sign in with your effyDOC account credentials
4. Start tracking your document engagement!

Features Available:
• 📧 Track when emails are opened
• 👆 Monitor link clicks
• 📄 See page-by-page reading analytics
• ⏱️ Real-time engagement notifications
• 📎 Send trackable document attachments
• 📈 Live dashboard with metrics

Troubleshooting:
• If plugin doesn't appear: Restart Outlook completely
• Ensure you have an active effyDOC account
• Check that Outlook allows add-ins (File → Options → Add-ins)
• Verify internet connection for real-time features

Support Resources:
• Platform Dashboard: {backend_url}
• Help Documentation: Available in the plugin interface

Thank you for choosing effyDOC! 🚀
"""
        
        with open(install_dir / "README.txt", 'w', encoding='utf-8') as f:
            f.write(readme_content)
        
        print("✅ Configuration files created")
        
        return True
        
    except Exception as e:
        print(f"❌ Installation failed: {e}")
        return False

def start_outlook():
    """Start Microsoft Outlook"""
    try:
        subprocess.run(["outlook.exe"], check=True)
        print("✅ Outlook started successfully")
    except:
        print("⚠️ Could not start Outlook automatically. Please start it manually.")

if __name__ == "__main__":
    main()