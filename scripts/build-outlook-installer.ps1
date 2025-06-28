# Automated Outlook Plugin Installer Builder
# This script creates a self-contained .exe installer for the effyDOC Outlook plugin

param(
    [string]$Version = "1.0.0",
    [string]$OutputPath = "../frontend/public",
    [string]$BackendURL = "https://your-backend-url.com"
)

Write-Host "🚀 Building effyDOC Outlook Plugin Installer v$Version" -ForegroundColor Green
Write-Host "=" * 60 -ForegroundColor Cyan

# Check if ps2exe is available
try {
    $ps2exeModule = Get-Module -ListAvailable -Name ps2exe
    if (-not $ps2exeModule) {
        Write-Host "Installing ps2exe module..." -ForegroundColor Yellow
        Install-Module -Name ps2exe -Force -Scope CurrentUser
    }
} catch {
    Write-Host "Error: ps2exe module is required. Please install it manually:" -ForegroundColor Red
    Write-Host "Install-Module -Name ps2exe -Force" -ForegroundColor White
    exit 1
}

# Create installer script content
$installerScript = @"
# effyDOC Outlook Plugin Installer
# Auto-generated on $(Get-Date)

param(
    [switch]`$Silent,
    [switch]`$Uninstall
)

# Configure console appearance
`$Host.UI.RawUI.WindowTitle = "effyDOC Outlook Plugin Installer v$Version"
if (`$Host.UI.RawUI.BackgroundColor -ne -1) {
    `$Host.UI.RawUI.BackgroundColor = "DarkBlue"
    `$Host.UI.RawUI.ForegroundColor = "White"
}
Clear-Host

# ASCII Art Logo
`$logo = @'
   ___  __  __       ___   ___   ___ 
  / _ \/ _|/ _|_   _|   \ / _ \ / __|
 |  __/  _|  _| | | | |) | (_) | (__ 
  \___|_| |_|  \_, |___/ \___/ \___|
               |__/                 

     Outlook Plugin Installer v$Version
'@

function Show-Banner {
    Write-Host `$logo -ForegroundColor Cyan
    Write-Host "=" * 50 -ForegroundColor Cyan
    Write-Host ""
}

function Write-Status {
    param([string]`$Message, [string]`$Color = "Yellow")
    Write-Host "[`$(Get-Date -Format 'HH:mm:ss')] `$Message" -ForegroundColor `$Color
}

function Write-Success {
    param([string]`$Message)
    Write-Host "✓ `$Message" -ForegroundColor Green
}

function Write-Error {
    param([string]`$Message)
    Write-Host "✗ `$Message" -ForegroundColor Red
}

function Write-Info {
    param([string]`$Message)
    Write-Host "ℹ `$Message" -ForegroundColor Cyan
}

function Test-Prerequisites {
    Write-Status "Checking system prerequisites..."
    
    # Check Windows version
    `$osVersion = [Environment]::OSVersion.Version
    if (`$osVersion.Major -lt 6) {
        Write-Error "Windows Vista or later is required"
        return `$false
    }
    Write-Success "Windows version: `$(`$osVersion.Major).`$(`$osVersion.Minor)"
    
    # Check .NET Framework
    try {
        `$netVersion = Get-ItemProperty "HKLM:SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" -Name Release -ErrorAction SilentlyContinue
        if (`$netVersion -and `$netVersion.Release -ge 461808) {
            Write-Success ".NET Framework 4.7.2+ detected"
        } else {
            Write-Error ".NET Framework 4.7.2 or later is required"
            Write-Info "Please download from: https://dotnet.microsoft.com/download/dotnet-framework"
            return `$false
        }
    } catch {
        Write-Error "Could not verify .NET Framework version"
        return `$false
    }
    
    # Check Outlook installation
    `$outlookPaths = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE"
    )
    
    `$outlookFound = `$false
    foreach (`$path in `$outlookPaths) {
        try {
            `$outlookPath = Get-ItemProperty -Path `$path -ErrorAction SilentlyContinue
            if (`$outlookPath) {
                Write-Success "Microsoft Outlook found: `$(`$outlookPath.'(Default)')"
                `$outlookFound = `$true
                break
            }
        } catch {
            # Continue checking
        }
    }
    
    if (-not `$outlookFound) {
        # Check for Office 365
        `$office365 = Get-ChildItem "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall" -ErrorAction SilentlyContinue | 
                     Get-ItemProperty -ErrorAction SilentlyContinue | 
                     Where-Object { `$_.DisplayName -like "*Microsoft Office*" -or `$_.DisplayName -like "*Microsoft 365*" }
        
        if (`$office365) {
            Write-Success "Microsoft Office/365 installation detected"
            `$outlookFound = `$true
        }
    }
    
    if (-not `$outlookFound) {
        Write-Error "Microsoft Outlook not found on this system"
        Write-Info "Please install Microsoft Outlook or Office 365 first"
        return `$false
    }
    
    return `$true
}

function Install-Plugin {
    Write-Status "Installing effyDOC Outlook plugin..."
    
    try {
        # Create installation directory
        `$installDir = "`$env:USERPROFILE\AppData\Roaming\effyDOC\OutlookPlugin"
        `$manifestDir = "`$env:USERPROFILE\AppData\Roaming\Microsoft\AddIns\effyDOC"
        
        if (-not (Test-Path `$installDir)) {
            New-Item -Path `$installDir -ItemType Directory -Force | Out-Null
            Write-Success "Created installation directory: `$installDir"
        }
        
        if (-not (Test-Path `$manifestDir)) {
            New-Item -Path `$manifestDir -ItemType Directory -Force | Out-Null
            Write-Success "Created manifest directory: `$manifestDir"
        }
        
        # Create plugin manifest
        `$manifestContent = @'
<?xml version="1.0" encoding="UTF-8"?>
<OfficeApp xmlns="http://schemas.microsoft.com/office/appforoffice/1.1"
           xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
           xsi:type="TaskPaneApp">
  
  <Id>effydoc-outlook-plugin-2024</Id>
  <Version>$Version</Version>
  <ProviderName>effyDOC</ProviderName>
  <DefaultLocale>en-US</DefaultLocale>
  <DisplayName DefaultValue="effyDOC Document Tracker"/>
  <Description DefaultValue="Track document engagement in real-time directly from Outlook. Send trackable documents and see when recipients open, click, and read them."/>
  
  <IconUrl DefaultValue="$BackendURL/icon-32.png"/>
  <HighResolutionIconUrl DefaultValue="$BackendURL/icon-64.png"/>
  <SupportUrl DefaultValue="$BackendURL/support"/>
  
  <AppDomains>
    <AppDomain>$BackendURL</AppDomain>
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
        <SourceLocation DefaultValue="$BackendURL/outlook-addin/"/>
        <RequestedHeight>500</RequestedHeight>
      </DesktopSettings>
    </Form>
    <Form xsi:type="ItemEdit">
      <DesktopSettings>
        <SourceLocation DefaultValue="$BackendURL/outlook-addin/"/>
        <RequestedHeight>500</RequestedHeight>
      </DesktopSettings>
    </Form>
  </FormSettings>
  
  <Permissions>ReadWriteMailbox</Permissions>
  
  <Rule xsi:type="RuleCollection" Mode="Or">
    <Rule xsi:type="ItemIs" ItemType="Message" FormType="Read"/>
    <Rule xsi:type="ItemIs" ItemType="Message" FormType="Edit"/>
  </Rule>
  
</OfficeApp>
'@
        
        # Save manifest file
        `$manifestPath = "`$manifestDir\manifest.xml"
        `$manifestContent | Out-File -FilePath `$manifestPath -Encoding UTF8
        Write-Success "Manifest created: `$manifestPath"
        
        # Register plugin in Windows registry
        `$regPaths = @(
            "HKCU:\SOFTWARE\Microsoft\Office\16.0\WEF\Developer",  # Office 2016/2019/365
            "HKCU:\SOFTWARE\Microsoft\Office\15.0\WEF\Developer"   # Office 2013
        )
        
        foreach (`$regPath in `$regPaths) {
            try {
                if (-not (Test-Path `$regPath)) {
                    New-Item -Path `$regPath -Force | Out-Null
                }
                Set-ItemProperty -Path `$regPath -Name "effyDOC" -Value `$manifestPath -Force
                Write-Success "Plugin registered in registry: `$regPath"
            } catch {
                Write-Error "Failed to register in `$regPath`: `$(`$_.Exception.Message)"
            }
        }
        
        # Create plugin configuration
        `$configContent = @"
{
  "pluginVersion": "$Version",
  "backendURL": "$BackendURL",
  "installDate": "`$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')",
  "features": {
    "documentTracking": true,
    "realTimeAnalytics": true,
    "emailIntegration": true
  }
}
"@
        `$configContent | Out-File -FilePath "`$installDir\config.json" -Encoding UTF8
        
        # Create README file
        `$readmeContent = @"
effyDOC Outlook Plugin Installation Complete!
==========================================

Installation Date: `$(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
Plugin Version: $Version
Installation Path: `$installDir
Manifest Location: `$manifestPath

✅ INSTALLATION SUCCESSFUL

Next Steps:
1. 🔄 Restart Microsoft Outlook (if currently running)
2. 👀 Look for the effyDOC panel in your Outlook sidebar
3. 🔑 Sign in with your effyDOC account credentials  
4. 📊 Start tracking your document engagement!

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
• Platform Dashboard: $BackendURL
• Help Documentation: Available in the plugin interface

Thank you for choosing effyDOC! 🚀
"@
        
        `$readmeContent | Out-File -FilePath "`$installDir\README.txt" -Encoding UTF8
        Write-Success "Installation guide created"
        
        # Create uninstaller
        `$uninstallerContent = @"
# effyDOC Outlook Plugin Uninstaller
Write-Host "Removing effyDOC Outlook Plugin..." -ForegroundColor Yellow

# Remove registry entries
Remove-ItemProperty -Path "HKCU:\SOFTWARE\Microsoft\Office\16.0\WEF\Developer" -Name "effyDOC" -ErrorAction SilentlyContinue
Remove-ItemProperty -Path "HKCU:\SOFTWARE\Microsoft\Office\15.0\WEF\Developer" -Name "effyDOC" -ErrorAction SilentlyContinue

# Remove installation files
Remove-Item -Path "`$installDir" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "`$manifestDir" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "✓ effyDOC Outlook Plugin removed successfully" -ForegroundColor Green
Write-Host "Please restart Outlook to complete the removal." -ForegroundColor Cyan
pause
"@
        
        `$uninstallerContent | Out-File -FilePath "`$installDir\Uninstall.ps1" -Encoding UTF8
        
        return `$true
    } catch {
        Write-Error "Installation failed: `$(`$_.Exception.Message)"
        return `$false
    }
}

function Uninstall-Plugin {
    Write-Status "Uninstalling effyDOC Outlook Plugin..."
    
    try {
        # Remove registry entries
        `$regPaths = @(
            "HKCU:\SOFTWARE\Microsoft\Office\16.0\WEF\Developer",
            "HKCU:\SOFTWARE\Microsoft\Office\15.0\WEF\Developer"
        )
        
        foreach (`$regPath in `$regPaths) {
            try {
                Remove-ItemProperty -Path `$regPath -Name "effyDOC" -ErrorAction SilentlyContinue
                Write-Success "Registry entry removed: `$regPath"
            } catch {
                # Ignore if doesn't exist
            }
        }
        
        # Remove installation directories
        `$installDir = "`$env:USERPROFILE\AppData\Roaming\effyDOC\OutlookPlugin"
        `$manifestDir = "`$env:USERPROFILE\AppData\Roaming\Microsoft\AddIns\effyDOC"
        
        if (Test-Path `$installDir) {
            Remove-Item -Path `$installDir -Recurse -Force
            Write-Success "Installation files removed"
        }
        
        if (Test-Path `$manifestDir) {
            Remove-Item -Path `$manifestDir -Recurse -Force
            Write-Success "Manifest files removed"
        }
        
        Write-Success "effyDOC Outlook Plugin uninstalled successfully"
        Write-Info "Please restart Outlook to complete the removal"
        
        return `$true
    } catch {
        Write-Error "Uninstallation failed: `$(`$_.Exception.Message)"
        return `$false
    }
}

function Start-OutlookIfRequested {
    if (-not `$Silent) {
        Write-Host ""
        `$startOutlook = Read-Host "Would you like to start Microsoft Outlook now? (Y/N)"
        if (`$startOutlook -eq "Y" -or `$startOutlook -eq "y") {
            Write-Status "Starting Microsoft Outlook..."
            try {
                Start-Process "outlook.exe" -ErrorAction Stop
                Write-Success "Outlook started successfully"
            } catch {
                Write-Error "Failed to start Outlook: `$(`$_.Exception.Message)"
                Write-Info "You can start Outlook manually"
            }
        }
    }
}

# Main execution
try {
    Show-Banner
    
    if (`$Uninstall) {
        if (Uninstall-Plugin) {
            Write-Host "`n🎉 Uninstallation completed successfully!" -ForegroundColor Green
        } else {
            Write-Host "`n❌ Uninstallation failed" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "Welcome to the effyDOC Outlook Plugin installer!" -ForegroundColor Green
        Write-Host "This installer will:" -ForegroundColor White
        Write-Host "  • Create plugin manifest and configuration" -ForegroundColor Gray
        Write-Host "  • Register plugin with Microsoft Outlook" -ForegroundColor Gray
        Write-Host "  • Set up document tracking capabilities" -ForegroundColor Gray
        Write-Host "  • Provide setup instructions" -ForegroundColor Gray
        Write-Host ""
        
        if (-not `$Silent) {
            `$continue = Read-Host "Continue with installation? (Y/N)"
            if (`$continue -ne "Y" -and `$continue -ne "y") {
                Write-Host "Installation cancelled by user" -ForegroundColor Yellow
                exit 0
            }
            Write-Host ""
        }
        
        # Check prerequisites
        if (-not (Test-Prerequisites)) {
            if (-not `$Silent) { pause }
            exit 1
        }
        
        # Install plugin
        if (Install-Plugin) {
            Write-Host ""
            Write-Host "🎉 Installation completed successfully!" -ForegroundColor Green
            Write-Host ""
            Write-Host "Next Steps:" -ForegroundColor Cyan
            Write-Host "1. Restart Microsoft Outlook" -ForegroundColor White
            Write-Host "2. Look for effyDOC panel in Outlook sidebar" -ForegroundColor White
            Write-Host "3. Sign in with your effyDOC account" -ForegroundColor White
            Write-Host "4. Start tracking documents!" -ForegroundColor White
            Write-Host ""
            Write-Host "📖 Installation guide: `$env:USERPROFILE\AppData\Roaming\effyDOC\OutlookPlugin\README.txt" -ForegroundColor Cyan
            Write-Host ""
            
            Start-OutlookIfRequested
        } else {
            Write-Host "`n❌ Installation failed" -ForegroundColor Red
            if (-not `$Silent) { pause }
            exit 1
        }
    }
} catch {
    Write-Error "Unexpected error: `$(`$_.Exception.Message)"
    Write-Host "Please contact support for assistance" -ForegroundColor Yellow
    if (-not `$Silent) { pause }
    exit 1
}

if (-not `$Silent) {
    Write-Host ""
    Write-Host "Thank you for using effyDOC! 🚀" -ForegroundColor Green
    pause
}
"@

# Save the installer script
$tempInstallerPath = "$env:TEMP\effydoc-outlook-installer-temp.ps1"
$installerScript | Out-File -FilePath $tempInstallerPath -Encoding UTF8

Write-Host "📄 Installer script created: $tempInstallerPath" -ForegroundColor Green

# Create the .exe using ps2exe
$exePath = "$OutputPath\EffyDocOutlookPlugin-Setup.exe"
Write-Host "🔧 Compiling to executable..." -ForegroundColor Yellow

try {
    # Import ps2exe module
    Import-Module ps2exe -Force
    
    # Compile to exe
    ps2exe -inputFile $tempInstallerPath -outputFile $exePath -title "effyDOC Outlook Plugin Installer" -version $Version -requireAdmin -noConsole:$false -noOutput:$false -noError:$false
    
    if (Test-Path $exePath) {
        Write-Host "✅ Successfully created: $exePath" -ForegroundColor Green
        Write-Host "📊 File size: $([math]::Round((Get-Item $exePath).Length / 1MB, 2)) MB" -ForegroundColor Cyan
    } else {
        throw "Executable was not created"
    }
} catch {
    Write-Host "❌ Failed to compile executable: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    # Clean up temp file
    if (Test-Path $tempInstallerPath) {
        Remove-Item $tempInstallerPath -Force
    }
}

Write-Host ""
Write-Host "🎉 Build completed successfully!" -ForegroundColor Green
Write-Host "📦 Installer ready at: $exePath" -ForegroundColor White
Write-Host ""