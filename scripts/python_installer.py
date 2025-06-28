#!/usr/bin/env python3
"""
effyDOC Outlook Plugin Installer
Real Python-based Windows installer with GUI
"""

import os
import sys
import json
import winreg
import tkinter as tk
from tkinter import messagebox, ttk, font
from pathlib import Path
from datetime import datetime
import webbrowser
import threading
import time

class EffyDocInstaller:
    def __init__(self):
        self.version = "1.0.0"
        self.backend_url = "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com"
        self.install_dir = Path.home() / "AppData" / "Roaming" / "effyDOC" / "OutlookPlugin"
        self.manifest_dir = Path.home() / "AppData" / "Roaming" / "Microsoft" / "AddIns" / "effyDOC"
        self.setup_gui()
    
    def setup_gui(self):
        """Setup the main GUI window"""
        self.root = tk.Tk()
        self.root.title(f"effyDOC Outlook Plugin Installer v{self.version}")
        self.root.geometry("600x500")
        self.root.resizable(False, False)
        
        # Set window icon (if available)
        try:
            self.root.iconbitmap("icon.ico")
        except:
            pass
        
        # Configure styles
        self.setup_styles()
        
        # Create main frame
        self.main_frame = tk.Frame(self.root)
        self.main_frame.pack(fill="both", expand=True)
        
        self.create_header()
        self.create_content()
        self.create_footer()
        
        # Center window
        self.center_window()
    
    def setup_styles(self):
        """Setup custom styles and fonts"""
        self.title_font = font.Font(family="Segoe UI", size=18, weight="bold")
        self.header_font = font.Font(family="Segoe UI", size=12, weight="bold")
        self.normal_font = font.Font(family="Segoe UI", size=10)
        self.small_font = font.Font(family="Segoe UI", size=9)
    
    def create_header(self):
        """Create the header section"""
        header_frame = tk.Frame(self.main_frame, bg="#4f46e5", height=100)
        header_frame.pack(fill="x")
        header_frame.pack_propagate(False)
        
        # Logo and title
        title_frame = tk.Frame(header_frame, bg="#4f46e5")
        title_frame.pack(expand=True)
        
        # ASCII logo
        logo_text = """   ___  __  __       ___   ___   ___ 
  / _ \\/ _|/ _|_   _|   \\ / _ \\ / __|
 |  __/  _|  _| | | | |) | (_) | (__ 
  \\___|_| |_|  \\_, |___/ \\___/ \\___|
               |__/                 """
        
        tk.Label(title_frame, text="📄 effyDOC", font=self.title_font, 
                fg="white", bg="#4f46e5").pack(pady=(10, 0))
        
        tk.Label(title_frame, text="Outlook Plugin Installer", font=self.header_font,
                fg="#e0e7ff", bg="#4f46e5").pack()
        
        tk.Label(title_frame, text=f"Version {self.version}", font=self.small_font,
                fg="#c7d2fe", bg="#4f46e5").pack(pady=(0, 10))
    
    def create_content(self):
        """Create the main content area"""
        self.content_frame = tk.Frame(self.main_frame, padx=30, pady=20)
        self.content_frame.pack(fill="both", expand=True)
        
        # Welcome message
        welcome_text = """Welcome to the effyDOC Outlook Plugin installer!

This installer will integrate effyDOC with Microsoft Outlook, allowing you to:

• 📧 Track when emails are opened and read
• 👆 Monitor link clicks and engagement  
• 📄 See page-by-page document analytics
• ⏱️ Get real-time engagement notifications
• 📎 Send trackable document attachments
• 📈 View live engagement dashboard

The installation process is completely automated."""
        
        tk.Label(self.content_frame, text=welcome_text, font=self.normal_font,
                justify="left", wraplength=520).pack(pady=(0, 20))
        
        # Progress section
        self.create_progress_section()
        
        # Buttons
        self.create_buttons()
    
    def create_progress_section(self):
        """Create progress bar and status area"""
        progress_frame = tk.Frame(self.content_frame)
        progress_frame.pack(fill="x", pady=(0, 20))
        
        tk.Label(progress_frame, text="Installation Progress:", 
                font=self.header_font).pack(anchor="w")
        
        self.progress = ttk.Progressbar(progress_frame, length=540, mode='determinate')
        self.progress.pack(pady=(5, 10), fill="x")
        
        self.status_label = tk.Label(progress_frame, text="Ready to install", 
                                   font=self.normal_font, fg="#6b7280")
        self.status_label.pack(anchor="w")
        
        # Status details
        self.details_text = tk.Text(progress_frame, height=8, width=65, 
                                   font=self.small_font, bg="#f9fafb", 
                                   state="disabled", wrap="word")
        self.details_text.pack(pady=(10, 0), fill="x")
    
    def create_buttons(self):
        """Create action buttons"""
        button_frame = tk.Frame(self.content_frame)
        button_frame.pack(side="bottom", fill="x")
        
        # Install button
        self.install_btn = tk.Button(button_frame, text="Install Plugin", 
                                   command=self.start_installation,
                                   bg="#059669", fg="white", font=self.header_font,
                                   padx=20, pady=10, relief="flat", cursor="hand2")
        self.install_btn.pack(side="right", padx=(10, 0))
        
        # Cancel button  
        tk.Button(button_frame, text="Cancel", command=self.root.quit,
                 font=self.normal_font, padx=20, pady=10, 
                 relief="flat", cursor="hand2").pack(side="right")
        
        # Help button
        tk.Button(button_frame, text="Help", command=self.show_help,
                 font=self.normal_font, padx=20, pady=10,
                 relief="flat", cursor="hand2").pack(side="left")
    
    def create_footer(self):
        """Create footer with additional info"""
        footer_frame = tk.Frame(self.main_frame, bg="#f3f4f6", height=40)
        footer_frame.pack(fill="x", side="bottom")
        footer_frame.pack_propagate(False)
        
        tk.Label(footer_frame, text="© 2024 effyDOC. All rights reserved.", 
                font=self.small_font, bg="#f3f4f6", fg="#6b7280").pack(pady=12)
    
    def center_window(self):
        """Center the window on screen"""
        self.root.update_idletasks()
        x = (self.root.winfo_screenwidth() // 2) - (600 // 2)
        y = (self.root.winfo_screenheight() // 2) - (500 // 2)
        self.root.geometry(f"600x500+{x}+{y}")
    
    def log_message(self, message):
        """Add message to the details log"""
        self.details_text.config(state="normal")
        self.details_text.insert("end", f"[{datetime.now().strftime('%H:%M:%S')}] {message}\n")
        self.details_text.see("end")
        self.details_text.config(state="disabled")
        self.root.update()
    
    def update_progress(self, value, status):
        """Update progress bar and status"""
        self.progress['value'] = value
        self.status_label.config(text=status)
        self.root.update()
    
    def start_installation(self):
        """Start the installation in a separate thread"""
        self.install_btn.config(state="disabled", text="Installing...")
        
        # Run installation in thread to prevent GUI freeze
        thread = threading.Thread(target=self.install_plugin)
        thread.daemon = True
        thread.start()
    
    def check_prerequisites(self):
        """Check if system meets requirements"""
        self.update_progress(10, "Checking prerequisites...")
        self.log_message("Checking system requirements...")
        
        # Check Windows version
        try:
            import platform
            windows_version = platform.platform()
            self.log_message(f"System: {windows_version}")
            
            if "Windows" not in windows_version:
                raise Exception("This installer requires Windows")
        except Exception as e:
            self.log_message(f"Error checking Windows version: {e}")
            return False
        
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
                self.log_message(f"Found Outlook: {path}")
                outlook_found = True
                break
        
        if not outlook_found:
            # Check registry
            try:
                with winreg.OpenKey(winreg.HKEY_LOCAL_MACHINE, 
                                  r"SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE") as key:
                    outlook_path = winreg.QueryValue(key, "")
                    if os.path.exists(outlook_path):
                        self.log_message(f"Found Outlook via registry: {outlook_path}")
                        outlook_found = True
            except Exception as e:
                self.log_message(f"Registry check failed: {e}")
        
        if not outlook_found:
            self.log_message("ERROR: Microsoft Outlook not found!")
            return False
        
        self.log_message("✓ All prerequisites met")
        return True
    
    def create_directories(self):
        """Create necessary directories"""
        self.update_progress(30, "Creating directories...")
        
        try:
            self.install_dir.mkdir(parents=True, exist_ok=True)
            self.log_message(f"✓ Created install directory: {self.install_dir}")
            
            self.manifest_dir.mkdir(parents=True, exist_ok=True)
            self.log_message(f"✓ Created manifest directory: {self.manifest_dir}")
            
        except Exception as e:
            self.log_message(f"ERROR creating directories: {e}")
            raise
    
    def create_manifest(self):
        """Create the Outlook plugin manifest"""
        self.update_progress(50, "Creating plugin manifest...")
        
        manifest_content = f'''<?xml version="1.0" encoding="UTF-8"?>
<OfficeApp xmlns="http://schemas.microsoft.com/office/appforoffice/1.1"
           xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
           xsi:type="TaskPaneApp">
  
  <Id>effydoc-outlook-plugin-2024</Id>
  <Version>{self.version}</Version>
  <ProviderName>effyDOC</ProviderName>
  <DefaultLocale>en-US</DefaultLocale>
  <DisplayName DefaultValue="effyDOC Document Tracker"/>
  <Description DefaultValue="Track document engagement in real-time directly from Outlook. Send trackable documents and see when recipients open, click, and read them."/>
  
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
  
</OfficeApp>'''
        
        try:
            manifest_path = self.manifest_dir / "manifest.xml"
            with open(manifest_path, 'w', encoding='utf-8') as f:
                f.write(manifest_content)
            
            self.log_message(f"✓ Created manifest: {manifest_path}")
            return manifest_path
            
        except Exception as e:
            self.log_message(f"ERROR creating manifest: {e}")
            raise
    
    def register_plugin(self, manifest_path):
        """Register plugin in Windows registry"""
        self.update_progress(70, "Registering plugin...")
        
        registry_keys = [
            r"SOFTWARE\Microsoft\Office\16.0\WEF\Developer",  # Office 2016/2019/365
            r"SOFTWARE\Microsoft\Office\15.0\WEF\Developer"   # Office 2013
        ]
        
        for reg_path in registry_keys:
            try:
                with winreg.CreateKey(winreg.HKEY_CURRENT_USER, reg_path) as key:
                    winreg.SetValueEx(key, "effyDOC", 0, winreg.REG_SZ, str(manifest_path))
                self.log_message(f"✓ Registered in: {reg_path}")
                
            except Exception as e:
                self.log_message(f"Warning: Could not register in {reg_path}: {e}")
    
    def create_config(self):
        """Create configuration files"""
        self.update_progress(85, "Creating configuration...")
        
        try:
            # Plugin configuration
            config = {
                "pluginVersion": self.version,
                "backendURL": self.backend_url,
                "installDate": datetime.now().isoformat(),
                "installationPath": str(self.install_dir),
                "manifestPath": str(self.manifest_dir / "manifest.xml"),
                "features": {
                    "documentTracking": True,
                    "realTimeAnalytics": True,
                    "emailIntegration": True,
                    "nativeOutlookIntegration": True
                }
            }
            
            config_path = self.install_dir / "config.json"
            with open(config_path, 'w') as f:
                json.dump(config, f, indent=2)
            
            self.log_message(f"✓ Created configuration: {config_path}")
            
            # Create README
            readme_content = f"""effyDOC Outlook Plugin Installation Complete!
==========================================

Installation Date: {datetime.now().strftime('%Y-%m-%d %H:%M:%S')}
Plugin Version: {self.version}
Installation Path: {self.install_dir}
Manifest Location: {self.manifest_dir / 'manifest.xml'}
Backend URL: {self.backend_url}

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
• Platform Dashboard: {self.backend_url}
• Help Documentation: Available in the plugin interface

Thank you for choosing effyDOC! 🚀
"""
            
            readme_path = self.install_dir / "README.txt"
            with open(readme_path, 'w', encoding='utf-8') as f:
                f.write(readme_content)
            
            self.log_message(f"✓ Created README: {readme_path}")
            
        except Exception as e:
            self.log_message(f"ERROR creating configuration: {e}")
            raise
    
    def install_plugin(self):
        """Main installation process"""
        try:
            # Check prerequisites
            if not self.check_prerequisites():
                messagebox.showerror("Prerequisites Failed", 
                                   "System requirements not met. Please install Microsoft Outlook and try again.")
                return
            
            # Create directories
            self.create_directories()
            time.sleep(0.5)  # Visual delay
            
            # Create manifest
            manifest_path = self.create_manifest()
            time.sleep(0.5)
            
            # Register plugin
            self.register_plugin(manifest_path)
            time.sleep(0.5)
            
            # Create configuration
            self.create_config()
            time.sleep(0.5)
            
            # Complete
            self.update_progress(100, "Installation completed successfully!")
            self.log_message("🎉 Installation completed successfully!")
            
            # Show success dialog
            result = messagebox.askyesno("Installation Complete", 
                                       "effyDOC Outlook Plugin installed successfully!\n\n"
                                       "Next steps:\n"
                                       "1. Restart Microsoft Outlook\n"
                                       "2. Look for effyDOC panel in Outlook sidebar\n"
                                       "3. Sign in with your effyDOC account\n\n"
                                       "Would you like to start Microsoft Outlook now?")
            
            if result:
                self.start_outlook()
            
            # Change button to close
            self.install_btn.config(text="Close", command=self.root.quit, state="normal")
            
        except Exception as e:
            self.log_message(f"INSTALLATION FAILED: {e}")
            messagebox.showerror("Installation Failed", 
                               f"Installation failed with error:\n{str(e)}\n\n"
                               "Please contact support for assistance.")
            self.install_btn.config(text="Install Plugin", command=self.start_installation, state="normal")
    
    def start_outlook(self):
        """Start Microsoft Outlook"""
        try:
            import subprocess
            subprocess.Popen(["outlook.exe"])
            self.log_message("✓ Started Microsoft Outlook")
        except Exception as e:
            self.log_message(f"Could not start Outlook: {e}")
            messagebox.showwarning("Cannot Start Outlook", 
                                 "Could not start Outlook automatically.\nPlease start it manually.")
    
    def show_help(self):
        """Show help information"""
        help_window = tk.Toplevel(self.root)
        help_window.title("Help - effyDOC Installer")
        help_window.geometry("500x400")
        help_window.resizable(False, False)
        
        help_text = """effyDOC Outlook Plugin Installer Help

System Requirements:
• Windows 7 or later
• Microsoft Outlook 2013 or later (including Office 365)
• .NET Framework 4.0 or later
• Administrator privileges (recommended)

Installation Process:
1. The installer checks for Microsoft Outlook
2. Creates necessary directories and files
3. Registers the plugin with Windows registry
4. Configures the plugin for your system

Troubleshooting:
• If installation fails, try running as Administrator
• Ensure Outlook is closed during installation
• Check Windows Defender/antivirus settings
• Verify you have sufficient disk space

After Installation:
• Restart Outlook to activate the plugin
• The effyDOC panel will appear in Outlook sidebar
• Sign in with your effyDOC account to start tracking

For additional support:
Visit: """ + self.backend_url + """
Email: support@effydoc.com

Version: """ + self.version
        
        text_widget = tk.Text(help_window, wrap="word", padx=20, pady=20, font=self.normal_font)
        text_widget.insert("1.0", help_text)
        text_widget.config(state="disabled")
        text_widget.pack(fill="both", expand=True)
        
        tk.Button(help_window, text="Close", command=help_window.destroy,
                 font=self.normal_font, padx=20, pady=8).pack(pady=10)
    
    def run(self):
        """Run the installer application"""
        try:
            self.root.mainloop()
        except KeyboardInterrupt:
            pass

def main():
    """Main entry point"""
    try:
        # Check if running on Windows
        if os.name != 'nt':
            print("This installer is designed for Windows systems only.")
            sys.exit(1)
        
        # Create and run installer
        installer = EffyDocInstaller()
        installer.run()
        
    except Exception as e:
        print(f"Fatal error: {e}")
        if 'messagebox' in globals():
            messagebox.showerror("Fatal Error", f"Application failed to start:\n{str(e)}")
        sys.exit(1)

if __name__ == "__main__":
    main()