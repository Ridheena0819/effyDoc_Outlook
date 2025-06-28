#!/usr/bin/env python3
"""
Create a proper Windows .exe installer using auto-py-to-exe and PyInstaller
This creates a real Windows executable that will work correctly
"""

import os
import sys
import subprocess
import tempfile
import base64
from pathlib import Path

def create_python_installer():
    """Create Python installer script that will be converted to .exe"""
    
    installer_code = '''
import os
import sys
import winreg
import json
from pathlib import Path
from datetime import datetime
import tkinter as tk
from tkinter import messagebox, ttk

class OutlookPluginInstaller:
    def __init__(self):
        self.version = "1.0.0"
        self.backend_url = "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com"
        self.setup_gui()
    
    def setup_gui(self):
        """Setup the GUI installer"""
        self.root = tk.Tk()
        self.root.title("effyDOC Outlook Plugin Installer")
        self.root.geometry("500x400")
        self.root.resizable(False, False)
        
        # Header
        header_frame = tk.Frame(self.root, bg="#4f46e5", height=80)
        header_frame.pack(fill="x")
        header_frame.pack_propagate(False)
        
        title_label = tk.Label(header_frame, text="effyDOC Outlook Plugin", 
                              font=("Arial", 16, "bold"), fg="white", bg="#4f46e5")
        title_label.pack(pady=20)
        
        # Content
        content_frame = tk.Frame(self.root, padx=20, pady=20)
        content_frame.pack(fill="both", expand=True)
        
        welcome_text = """Welcome to the effyDOC Outlook Plugin installer!

This will install the plugin to integrate with Microsoft Outlook,
allowing you to track document engagement in real-time.

Features:
• Track email opens and clicks
• Monitor document reading analytics  
• Real-time engagement notifications
• Send trackable document attachments"""
        
        tk.Label(content_frame, text=welcome_text, justify="left", 
                wraplength=450, font=("Arial", 10)).pack(pady=10)
        
        # Progress bar
        self.progress = ttk.Progressbar(content_frame, length=400, mode='determinate')
        self.progress.pack(pady=20)
        
        # Status label
        self.status_label = tk.Label(content_frame, text="Ready to install", 
                                    font=("Arial", 9), fg="gray")
        self.status_label.pack()
        
        # Buttons
        button_frame = tk.Frame(content_frame)
        button_frame.pack(side="bottom", fill="x", pady=20)
        
        self.install_btn = tk.Button(button_frame, text="Install Plugin", 
                                   command=self.install_plugin, bg="#4f46e5", fg="white",
                                   font=("Arial", 10, "bold"), padx=20, pady=8)
        self.install_btn.pack(side="right", padx=(10, 0))
        
        tk.Button(button_frame, text="Cancel", command=self.root.quit,
                 font=("Arial", 10), padx=20, pady=8).pack(side="right")
    
    def update_progress(self, value, status):
        """Update progress bar and status"""
        self.progress['value'] = value
        self.status_label.config(text=status)
        self.root.update()
    
    def check_prerequisites(self):
        """Check if Outlook is installed"""
        self.update_progress(10, "Checking prerequisites...")
        
        outlook_paths = [
            r"C:\\Program Files\\Microsoft Office\\root\\Office16\\OUTLOOK.EXE",
            r"C:\\Program Files (x86)\\Microsoft Office\\root\\Office16\\OUTLOOK.EXE",
            r"C:\\Program Files\\Microsoft Office\\Office16\\OUTLOOK.EXE",
            r"C:\\Program Files (x86)\\Microsoft Office\\Office16\\OUTLOOK.EXE"
        ]
        
        for path in outlook_paths:
            if os.path.exists(path):
                return True
        
        # Check registry
        try:
            with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, 
                              r"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\App Paths\\OUTLOOK.EXE") as key:
                outlook_path = winreg.QueryValue(key, "")
                if os.path.exists(outlook_path):
                    return True
        except:
            pass
        
        return False
    
    def install_plugin(self):
        """Install the Outlook plugin"""
        try:
            if not self.check_prerequisites():
                messagebox.showerror("Error", "Microsoft Outlook not found.\\nPlease install Outlook and try again.")
                return
            
            self.update_progress(20, "Creating directories...")
            
            # Create directories
            install_dir = Path.home() / "AppData" / "Roaming" / "effyDOC" / "OutlookPlugin"
            manifest_dir = Path.home() / "AppData" / "Roaming" / "Microsoft" / "AddIns" / "effyDOC"
            
            install_dir.mkdir(parents=True, exist_ok=True)
            manifest_dir.mkdir(parents=True, exist_ok=True)
            
            self.update_progress(40, "Creating plugin manifest...")
            
            # Create manifest
            manifest_content = f"""<?xml version="1.0" encoding="UTF-8"?>
<OfficeApp xmlns="http://schemas.microsoft.com/office/appforoffice/1.1"
           xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
           xsi:type="TaskPaneApp">
  <Id>effydoc-outlook-plugin-2024</Id>
  <Version>{self.version}</Version>
  <ProviderName>effyDOC</ProviderName>
  <DefaultLocale>en-US</DefaultLocale>
  <DisplayName DefaultValue="effyDOC Document Tracker"/>
  <Description DefaultValue="Track document engagement in real-time directly from Outlook."/>
  <IconUrl DefaultValue="{self.backend_url}/icon-32.png"/>
  <HighResolutionIconUrl DefaultValue="{self.backend_url}/icon-64.png"/>
  <SupportUrl DefaultValue="{self.backend_url}/support"/>
  <AppDomains>
    <AppDomain>{self.backend_url}</AppDomain>
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
        <SourceLocation DefaultValue="{self.backend_url}/outlook-addin/"/>
        <RequestedHeight>500</RequestedHeight>
      </DesktopSettings>
    </Form>
    <Form xsi:type="ItemEdit">
      <DesktopSettings>
        <SourceLocation DefaultValue="{self.backend_url}/outlook-addin/"/>
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
            
            self.update_progress(60, "Registering plugin...")
            
            # Register plugin
            try:
                with winreg.CreateKey(winreg.HKEY_CURRENT_USER, 
                                    r"SOFTWARE\\Microsoft\\Office\\16.0\\WEF\\Developer") as key:
                    winreg.SetValueEx(key, "effyDOC", 0, winreg.REG_SZ, str(manifest_path))
            except Exception:
                pass
            
            try:
                with winreg.CreateKey(winreg.HKEY_CURRENT_USER, 
                                    r"SOFTWARE\\Microsoft\\Office\\15.0\\WEF\\Developer") as key:
                    winreg.SetValueEx(key, "effyDOC", 0, winreg.REG_SZ, str(manifest_path))
            except Exception:
                pass
            
            self.update_progress(80, "Creating configuration...")
            
            # Create config
            config = {
                "pluginVersion": self.version,
                "backendURL": self.backend_url,
                "installDate": datetime.now().isoformat(),
                "installationPath": str(install_dir),
                "manifestPath": str(manifest_path)
            }
            
            with open(install_dir / "config.json", 'w') as f:
                json.dump(config, f, indent=2)
            
            self.update_progress(100, "Installation completed!")
            
            messagebox.showinfo("Success", 
                              "effyDOC Outlook Plugin installed successfully!\\n\\n"
                              "Next steps:\\n"
                              "1. Restart Microsoft Outlook\\n"
                              "2. Look for effyDOC panel in Outlook sidebar\\n"
                              "3. Sign in with your effyDOC account")
            
            self.root.quit()
            
        except Exception as e:
            messagebox.showerror("Error", f"Installation failed: {str(e)}")
    
    def run(self):
        """Run the installer"""
        self.root.mainloop()

if __name__ == "__main__":
    installer = OutlookPluginInstaller()
    installer.run()
'''
    
    return installer_code

def build_windows_exe():
    """Build a Windows .exe from Python code using PyInstaller"""
    
    # Create temporary Python file
    installer_code = create_python_installer()
    
    with tempfile.NamedTemporaryFile(mode='w', suffix='.py', delete=False) as f:
        f.write(installer_code)
        temp_python_file = f.name
    
    output_dir = Path(__file__).parent.parent / "frontend" / "public"
    output_dir.mkdir(parents=True, exist_ok=True)
    
    try:
        # Install PyInstaller if needed
        subprocess.check_call([sys.executable, "-m", "pip", "install", "pyinstaller"])
        
        # Build the .exe
        cmd = [
            sys.executable, "-m", "PyInstaller",
            "--onefile",
            "--windowed",
            "--name", "EffyDocOutlookPlugin-Setup",
            "--distpath", str(output_dir),
            "--workpath", "/tmp/pyinstaller_build",
            "--specpath", "/tmp",
            temp_python_file
        ]
        
        print("Building Windows .exe installer...")
        result = subprocess.run(cmd, capture_output=True, text=True)
        
        if result.returncode == 0:
            exe_path = output_dir / "EffyDocOutlookPlugin-Setup.exe"
            if exe_path.exists():
                print(f"✅ Successfully created: {exe_path}")
                print(f"📊 File size: {exe_path.stat().st_size:,} bytes")
                return True
        else:
            print(f"❌ PyInstaller failed: {result.stderr}")
            return False
            
    except Exception as e:
        print(f"❌ Error building .exe: {e}")
        return False
    finally:
        # Clean up
        if os.path.exists(temp_python_file):
            os.unlink(temp_python_file)
    
    return False

if __name__ == "__main__":
    if build_windows_exe():
        print("🎉 Windows .exe installer created successfully!")
    else:
        print("❌ Failed to create Windows .exe installer")
        print("💡 The .bat file is still available as fallback")