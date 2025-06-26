# effyDOC Outlook Add-in Professional Installer
# This PowerShell script can be compiled to .exe using ps2exe
# Usage: ps2exe -inputFile installer.ps1 -outputFile effyDOC-Outlook-Installer.exe -iconFile icon.ico -title "effyDOC Outlook Add-in Installer" -version 1.0.0.0

param(
    [switch]$Silent,
    [switch]$Uninstall
)

# Set console appearance
$Host.UI.RawUI.WindowTitle = "effyDOC Outlook Add-in Installer v1.0"
$Host.UI.RawUI.BackgroundColor = "DarkBlue"
$Host.UI.RawUI.ForegroundColor = "White"
Clear-Host

# ASCII Art Logo
$logo = @"
   ___  __  __       ___   ___   ___ 
  / _ \/ _|/ _|_   _|   \ / _ \ / __|
 |  __/  _|  _| | | | |) | (_) | (__ 
  \___|_| |_|  \_, |___/ \___/ \___|
               |__/                 

     Outlook Add-in Installer v1.0
"@

function Show-Banner {
    Write-Host $logo -ForegroundColor Cyan
    Write-Host "=" * 50 -ForegroundColor Cyan
    Write-Host ""
}

function Write-Status {
    param([string]$Message, [string]$Color = "Yellow")
    Write-Host "[$(Get-Date -Format 'HH:mm:ss')] $Message" -ForegroundColor $Color
}

function Write-Success {
    param([string]$Message)
    Write-Host "✓ $Message" -ForegroundColor Green
}

function Write-Error {
    param([string]$Message)
    Write-Host "✗ $Message" -ForegroundColor Red
}

function Write-Info {
    param([string]$Message)
    Write-Host "ℹ $Message" -ForegroundColor Cyan
}

function Test-OutlookInstallation {
    Write-Status "Checking Microsoft Outlook installation..."
    
    $outlookPaths = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE"
    )
    
    foreach ($path in $outlookPaths) {
        try {
            $outlookPath = Get-ItemProperty -Path $path -ErrorAction SilentlyContinue
            if ($outlookPath) {
                Write-Success "Microsoft Outlook found: $($outlookPath.'(Default)')"
                return $true
            }
        }
        catch {
            # Continue checking other paths
        }
    }
    
    # Check for Office 365 click-to-run
    $office365Path = "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"
    $office365Apps = Get-ChildItem $office365Path -ErrorAction SilentlyContinue | 
                     Get-ItemProperty -ErrorAction SilentlyContinue | 
                     Where-Object { $_.DisplayName -like "*Microsoft Office*" -or $_.DisplayName -like "*Microsoft 365*" }
    
    if ($office365Apps) {
        Write-Success "Microsoft Office/365 installation detected"
        return $true
    }
    
    Write-Error "Microsoft Outlook not found on this system"
    Write-Info "Please install Microsoft Outlook or Office 365 first"
    return $false
}

function Download-Manifest {
    Write-Status "Downloading effyDOC add-in manifest..."
    
    $manifestUrl = "https://54f44f8f-6cf4-4842-bf49-6bd490d293fd.preview.emergentagent.com/outlook-addin/manifest.xml"
    $tempPath = "$env:TEMP\effydoc-manifest-$(Get-Random).xml"
    
    try {
        # Use .NET WebClient for better progress indication
        $webClient = New-Object System.Net.WebClient
        
        # Add progress handler
        $webClient.add_DownloadProgressChanged({
            param($sender, $e)
            $percent = [math]::Round(($e.BytesReceived / $e.TotalBytesToReceive) * 100, 1)
            Write-Progress -Activity "Downloading manifest" -Status "$percent% Complete" -PercentComplete $percent
        })
        
        $webClient.DownloadFile($manifestUrl, $tempPath)
        $webClient.Dispose()
        Write-Progress -Activity "Downloading manifest" -Completed
        
        if (Test-Path $tempPath) {
            Write-Success "Manifest downloaded successfully"
            return $tempPath
        } else {
            throw "Download completed but file not found"
        }
    }
    catch {
        Write-Error "Failed to download manifest: $($_.Exception.Message)"
        Write-Info "Please check your internet connection and try again"
        return $null
    }
}

function Install-AddIn {
    param([string]$ManifestPath)
    
    Write-Status "Installing effyDOC Outlook Add-in..."
    
    # Create installation directory
    $installDir = "$env:USERPROFILE\AppData\Roaming\Microsoft\AddIns\effyDOC"
    $finalManifestPath = "$installDir\manifest.xml"
    
    try {
        if (-not (Test-Path $installDir)) {
            New-Item -Path $installDir -ItemType Directory -Force | Out-Null
            Write-Success "Installation directory created: $installDir"
        }
        
        # Copy manifest file
        Copy-Item $ManifestPath $finalManifestPath -Force
        Write-Success "Manifest installed to: $finalManifestPath"
        
        # Register add-in in registry for different Office versions
        $regPaths = @(
            "HKCU:\SOFTWARE\Microsoft\Office\16.0\WEF\Developer",  # Office 2016/2019/365
            "HKCU:\SOFTWARE\Microsoft\Office\15.0\WEF\Developer"   # Office 2013
        )
        
        foreach ($regPath in $regPaths) {
            try {
                if (-not (Test-Path $regPath)) {
                    New-Item -Path $regPath -Force | Out-Null
                }
                Set-ItemProperty -Path $regPath -Name "effyDOC" -Value $finalManifestPath -Force
                Write-Success "Add-in registered in registry: $regPath"
            }
            catch {
                Write-Error "Failed to register in $regPath`: $($_.Exception.Message)"
            }
        }
        
        # Create README file
        $readmeContent = @"
effyDOC Outlook Add-in Installation Complete!
============================================

Installation Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')
Installation Path: $installDir
Manifest Location: $finalManifestPath

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
• If add-in doesn't appear: Restart Outlook completely
• Ensure you have an active effyDOC account
• Check that Outlook allows add-ins (File → Options → Add-ins)
• Verify internet connection for real-time features

Support Resources:
• Integration Guide: https://54f44f8f-6cf4-4842-bf49-6bd490d293fd.preview.emergentagent.com/integrations.html
• Platform Dashboard: https://54f44f8f-6cf4-4842-bf49-6bd490d293fd.preview.emergentagent.com
• Help Documentation: Available in the add-in interface

Thank you for choosing effyDOC! 🚀
"@
        
        $readmeContent | Out-File -FilePath "$installDir\README.txt" -Encoding UTF8
        Write-Success "Installation guide created"
        
        # Create uninstaller script
        $uninstallerContent = @"
# effyDOC Outlook Add-in Uninstaller
Write-Host "Removing effyDOC Outlook Add-in..." -ForegroundColor Yellow

# Remove registry entries
Remove-ItemProperty -Path "HKCU:\SOFTWARE\Microsoft\Office\16.0\WEF\Developer" -Name "effyDOC" -ErrorAction SilentlyContinue
Remove-ItemProperty -Path "HKCU:\SOFTWARE\Microsoft\Office\15.0\WEF\Developer" -Name "effyDOC" -ErrorAction SilentlyContinue

# Remove installation files
Remove-Item -Path "$installDir" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "✓ effyDOC Outlook Add-in removed successfully" -ForegroundColor Green
Write-Host "Please restart Outlook to complete the removal." -ForegroundColor Cyan
pause
"@
        
        $uninstallerContent | Out-File -FilePath "$installDir\Uninstall.ps1" -Encoding UTF8
        
        return $true
    }
    catch {
        Write-Error "Installation failed: $($_.Exception.Message)"
        return $false
    }
    finally {
        # Clean up temp file
        if (Test-Path $ManifestPath) {
            Remove-Item $ManifestPath -Force -ErrorAction SilentlyContinue
        }
    }
}

function Start-OutlookIfRequested {
    if (-not $Silent) {
        Write-Host ""
        $startOutlook = Read-Host "Would you like to start Microsoft Outlook now? (Y/N)"
        if ($startOutlook -eq "Y" -or $startOutlook -eq "y") {
            Write-Status "Starting Microsoft Outlook..."
            try {
                Start-Process "outlook.exe" -ErrorAction Stop
                Write-Success "Outlook started successfully"
            }
            catch {
                Write-Error "Failed to start Outlook: $($_.Exception.Message)"
                Write-Info "You can start Outlook manually"
            }
        }
    }
}

function Uninstall-AddIn {
    Write-Status "Uninstalling effyDOC Outlook Add-in..."
    
    try {
        # Remove registry entries
        $regPaths = @(
            "HKCU:\SOFTWARE\Microsoft\Office\16.0\WEF\Developer",
            "HKCU:\SOFTWARE\Microsoft\Office\15.0\WEF\Developer"
        )
        
        foreach ($regPath in $regPaths) {
            try {
                Remove-ItemProperty -Path $regPath -Name "effyDOC" -ErrorAction SilentlyContinue
                Write-Success "Registry entry removed: $regPath"
            }
            catch {
                # Ignore errors if entry doesn't exist
            }
        }
        
        # Remove installation directory
        $installDir = "$env:USERPROFILE\AppData\Roaming\Microsoft\AddIns\effyDOC"
        if (Test-Path $installDir) {
            Remove-Item -Path $installDir -Recurse -Force
            Write-Success "Installation files removed"
        }
        
        Write-Success "effyDOC Outlook Add-in uninstalled successfully"
        Write-Info "Please restart Outlook to complete the removal"
        
    }
    catch {
        Write-Error "Uninstallation failed: $($_.Exception.Message)"
        return $false
    }
    
    return $true
}

# Main execution
try {
    Show-Banner
    
    if ($Uninstall) {
        if (Uninstall-AddIn) {
            Write-Host "`n🎉 Uninstallation completed successfully!" -ForegroundColor Green
        } else {
            Write-Host "`n❌ Uninstallation failed" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "Welcome to the effyDOC Outlook Add-in installer!" -ForegroundColor Green
        Write-Host "This installer will:" -ForegroundColor White
        Write-Host "  • Download the latest add-in manifest" -ForegroundColor Gray
        Write-Host "  • Install it in the correct location" -ForegroundColor Gray
        Write-Host "  • Register it with Microsoft Outlook" -ForegroundColor Gray
        Write-Host "  • Provide setup instructions" -ForegroundColor Gray
        Write-Host ""
        
        if (-not $Silent) {
            $continue = Read-Host "Continue with installation? (Y/N)"
            if ($continue -ne "Y" -and $continue -ne "y") {
                Write-Host "Installation cancelled by user" -ForegroundColor Yellow
                exit 0
            }
            Write-Host ""
        }
        
        # Check prerequisites
        if (-not (Test-OutlookInstallation)) {
            if (-not $Silent) { pause }
            exit 1
        }
        
        # Download manifest
        $manifestPath = Download-Manifest
        if (-not $manifestPath) {
            if (-not $Silent) { pause }
            exit 1
        }
        
        # Install add-in
        if (Install-AddIn -ManifestPath $manifestPath) {
            Write-Host ""
            Write-Host "🎉 Installation completed successfully!" -ForegroundColor Green
            Write-Host ""
            Write-Host "Next Steps:" -ForegroundColor Cyan
            Write-Host "1. Restart Microsoft Outlook" -ForegroundColor White
            Write-Host "2. Look for effyDOC panel in Outlook sidebar" -ForegroundColor White
            Write-Host "3. Sign in with your effyDOC account" -ForegroundColor White
            Write-Host "4. Start tracking documents!" -ForegroundColor White
            Write-Host ""
            Write-Host "📖 Installation guide created at:" -ForegroundColor Cyan
            Write-Host "   $env:USERPROFILE\AppData\Roaming\Microsoft\AddIns\effyDOC\README.txt" -ForegroundColor Gray
            Write-Host ""
            
            Start-OutlookIfRequested
        } else {
            Write-Host "`n❌ Installation failed" -ForegroundColor Red
            if (-not $Silent) { pause }
            exit 1
        }
    }
}
catch {
    Write-Error "Unexpected error: $($_.Exception.Message)"
    Write-Host "Please contact support for assistance" -ForegroundColor Yellow
    if (-not $Silent) { pause }
    exit 1
}

if (-not $Silent) {
    Write-Host ""
    Write-Host "Thank you for using effyDOC! 🚀" -ForegroundColor Green
    pause
}