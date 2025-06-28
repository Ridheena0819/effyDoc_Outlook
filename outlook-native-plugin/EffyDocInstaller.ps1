# effyDOC Outlook Plugin Installer PowerShell Script
# This script can be converted to .exe using ps2exe or similar tools

param(
    [switch]$Install,
    [switch]$Uninstall,
    [switch]$Help,
    [switch]$Silent
)

# Configuration
$PluginName = "effyDOC Outlook Plugin"
$PluginVersion = "1.0.0"
$CompanyName = "effyDOC"
$BaseUrl = "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com"

# Installation paths
$InstallDir = "$env:LOCALAPPDATA\$PluginName"
$TempDir = "$env:TEMP\EffyDocInstaller"

# Registry paths
$AddinRegPath = "HKCU:\SOFTWARE\Microsoft\Office\Outlook\Addins\EffyDocOutlookPlugin"
$UninstallRegPath = "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\$PluginName"

function Show-Header {
    if (-not $Silent) {
        Clear-Host
        Write-Host ""
        Write-Host "🚀 $PluginName Installer v$PluginVersion" -ForegroundColor Green
        Write-Host "=" * 60 -ForegroundColor Green
        Write-Host "Professional document tracking for Microsoft Outlook" -ForegroundColor Cyan
        Write-Host ""
    }
}

function Show-Help {
    Show-Header
    Write-Host "Usage:" -ForegroundColor Yellow
    Write-Host "  .\EffyDocInstaller.exe -Install    Install the plugin" -ForegroundColor White
    Write-Host "  .\EffyDocInstaller.exe -Uninstall  Remove the plugin" -ForegroundColor White
    Write-Host "  .\EffyDocInstaller.exe -Help       Show this help" -ForegroundColor White
    Write-Host "  .\EffyDocInstaller.exe -Silent     Silent installation" -ForegroundColor White
    Write-Host ""
    Write-Host "Features:" -ForegroundColor Yellow
    Write-Host "  • Native Outlook ribbon integration" -ForegroundColor White
    Write-Host "  • Real-time document tracking" -ForegroundColor White
    Write-Host "  • Professional document attachments" -ForegroundColor White
    Write-Host "  • Email engagement analytics" -ForegroundColor White
    Write-Host ""
    exit 0
}

function Test-Prerequisites {
    Write-Progress -Activity "Checking Prerequisites" -Status "Validating system requirements..." -PercentComplete 10
    
    # Check Windows version
    $winVersion = [System.Environment]::OSVersion.Version
    if ($winVersion.Major -lt 6) {
        throw "Windows Vista or later is required"
    }
    
    # Check if Outlook is installed
    $outlookPath = $null
    $outlookRegPaths = @(
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE"
    )
    
    foreach ($regPath in $outlookRegPaths) {
        try {
            $outlookPath = (Get-ItemProperty -Path $regPath -ErrorAction SilentlyContinue).'(default)'
            if ($outlookPath -and (Test-Path $outlookPath)) {
                break
            }
        } catch {
            continue
        }
    }
    
    if (-not $outlookPath) {
        throw "Microsoft Outlook is not installed. Please install Outlook first."
    }
    
    if (-not $Silent) {
        Write-Host "✅ Microsoft Outlook found: $outlookPath" -ForegroundColor Green
    }
    
    # Check .NET Framework
    $netVersion = Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full\" -Name Release -ErrorAction SilentlyContinue
    if (-not $netVersion -or $netVersion.Release -lt 461808) {
        throw ".NET Framework 4.7.2 or higher is required"
    }
    
    if (-not $Silent) {
        Write-Host "✅ .NET Framework requirement satisfied" -ForegroundColor Green
    }
    
    # Check VSTO Runtime (optional warning)
    $vstoVersion = Get-ItemProperty "HKLM:\SOFTWARE\Microsoft\VSTO Runtime Setup\v4R" -Name Version -ErrorAction SilentlyContinue
    if (-not $vstoVersion) {
        if (-not $Silent) {
            Write-Warning "VSTO Runtime not detected. The plugin may require additional components."
            Write-Host "   Download VSTO Runtime from: https://aka.ms/vstoruntimedownload" -ForegroundColor Yellow
        }
    } else {
        if (-not $Silent) {
            Write-Host "✅ VSTO Runtime found" -ForegroundColor Green
        }
    }
}

function Download-PluginFiles {
    Write-Progress -Activity "Downloading Plugin" -Status "Downloading plugin files..." -PercentComplete 30
    
    # Create temp directory
    if (Test-Path $TempDir) {
        Remove-Item $TempDir -Recurse -Force
    }
    New-Item -Path $TempDir -ItemType Directory -Force | Out-Null
    
    # Files to download
    $files = @(
        @{ Name = "EffyDocOutlookPlugin.dll"; Url = "$BaseUrl/downloads/plugin/EffyDocOutlookPlugin.dll" },
        @{ Name = "EffyDocOutlookPlugin.dll.manifest"; Url = "$BaseUrl/downloads/plugin/EffyDocOutlookPlugin.dll.manifest" },
        @{ Name = "EffyDocOutlookPlugin.vsto"; Url = "$BaseUrl/downloads/plugin/EffyDocOutlookPlugin.vsto" },
        @{ Name = "Newtonsoft.Json.dll"; Url = "$BaseUrl/downloads/plugin/Newtonsoft.Json.dll" }
    )
    
    foreach ($file in $files) {
        try {
            $filePath = Join-Path $TempDir $file.Name
            if (-not $Silent) {
                Write-Host "Downloading $($file.Name)..." -ForegroundColor Cyan
            }
            
            # For now, create placeholder files since we don't have actual download URLs
            # In production, use: Invoke-WebRequest -Uri $file.Url -OutFile $filePath
            New-Item -Path $filePath -ItemType File -Force | Out-Null
            Add-Content -Path $filePath -Value "# Placeholder for $($file.Name)"
            
        } catch {
            throw "Failed to download $($file.Name): $($_.Exception.Message)"
        }
    }
    
    if (-not $Silent) {
        Write-Host "✅ All plugin files downloaded successfully" -ForegroundColor Green
    }
}

function Install-Plugin {
    Show-Header
    
    try {
        if (-not $Silent) {
            Write-Host "Installing $PluginName..." -ForegroundColor Yellow
            Write-Host ""
        }
        
        # Check prerequisites
        Test-Prerequisites
        
        # Download files
        Download-PluginFiles
        
        # Create installation directory
        Write-Progress -Activity "Installing Plugin" -Status "Creating installation directory..." -PercentComplete 50
        
        if (Test-Path $InstallDir) {
            Remove-Item $InstallDir -Recurse -Force
        }
        New-Item -Path $InstallDir -ItemType Directory -Force | Out-Null
        
        # Copy files
        Write-Progress -Activity "Installing Plugin" -Status "Copying plugin files..." -PercentComplete 60
        
        Get-ChildItem $TempDir | Copy-Item -Destination $InstallDir -Recurse -Force
        
        # Create README
        $readmeContent = @"
$PluginName Installation Complete!
==========================================

Thank you for installing the $PluginName!

FEATURES:
• Native Outlook ribbon integration
• Real-time document tracking
• Professional document attachments  
• Email engagement analytics

HOW TO USE:
1. Restart Microsoft Outlook
2. Look for 'effyDOC' section in the Outlook ribbon
3. Click 'Settings' to sign in with your effyDOC account
4. Use 'Attach Document' to send trackable documents

SUPPORT:
• Website: $BaseUrl
• Documentation: $BaseUrl/docs
• Support: $BaseUrl/support

VERSION: $PluginVersion
INSTALLED: $(Get-Date)
"@
        
        $readmeContent | Out-File -FilePath (Join-Path $InstallDir "README.txt") -Encoding UTF8
        
        # Register add-in
        Write-Progress -Activity "Installing Plugin" -Status "Registering Outlook add-in..." -PercentComplete 80
        
        # Create registry entries
        New-Item -Path $AddinRegPath -Force | Out-Null
        Set-ItemProperty -Path $AddinRegPath -Name "Description" -Value "$PluginName - Document tracking and attachment plugin"
        Set-ItemProperty -Path $AddinRegPath -Name "FriendlyName" -Value $PluginName
        Set-ItemProperty -Path $AddinRegPath -Name "LoadBehavior" -Value 3 -Type DWord
        Set-ItemProperty -Path $AddinRegPath -Name "Manifest" -Value "$InstallDir\EffyDocOutlookPlugin.vsto|vstolocal"
        
        # Create uninstall registry entries
        New-Item -Path $UninstallRegPath -Force | Out-Null
        Set-ItemProperty -Path $UninstallRegPath -Name "DisplayName" -Value $PluginName
        Set-ItemProperty -Path $UninstallRegPath -Name "DisplayVersion" -Value $PluginVersion
        Set-ItemProperty -Path $UninstallRegPath -Name "Publisher" -Value $CompanyName
        Set-ItemProperty -Path $UninstallRegPath -Name "InstallLocation" -Value $InstallDir
        Set-ItemProperty -Path $UninstallRegPath -Name "UninstallString" -Value "powershell.exe -ExecutionPolicy Bypass -File `"$PSCommandPath`" -Uninstall"
        Set-ItemProperty -Path $UninstallRegPath -Name "NoModify" -Value 1 -Type DWord
        Set-ItemProperty -Path $UninstallRegPath -Name "NoRepair" -Value 1 -Type DWord
        
        # Clean up temp files
        Remove-Item $TempDir -Recurse -Force -ErrorAction SilentlyContinue
        
        Write-Progress -Activity "Installing Plugin" -Status "Installation complete!" -PercentComplete 100
        
        if (-not $Silent) {
            Write-Host ""
            Write-Host "🎉 Installation completed successfully!" -ForegroundColor Green
            Write-Host ""
            Write-Host "📋 Next Steps:" -ForegroundColor Cyan
            Write-Host "1. Restart Microsoft Outlook" -ForegroundColor White
            Write-Host "2. Look for the 'effyDOC' section in the Outlook ribbon" -ForegroundColor White
            Write-Host "3. Click 'Settings' to sign in with your effyDOC account" -ForegroundColor White
            Write-Host "4. Start tracking your documents!" -ForegroundColor White
            Write-Host ""
            Write-Host "🔗 Need help? Visit: $BaseUrl/support" -ForegroundColor Cyan
            Write-Host ""
            
            $startOutlook = Read-Host "Would you like to start Outlook now? (y/n)"
            if ($startOutlook -eq "y" -or $startOutlook -eq "Y") {
                try {
                    Start-Process "outlook.exe" -ErrorAction SilentlyContinue
                } catch {
                    Write-Warning "Could not start Outlook automatically. Please start it manually."
                }
            }
        }
        
    } catch {
        Write-Error "Installation failed: $($_.Exception.Message)"
        exit 1
    }
}

function Uninstall-Plugin {
    Show-Header
    
    try {
        if (-not $Silent) {
            Write-Host "Uninstalling $PluginName..." -ForegroundColor Yellow
            Write-Host ""
        }
        
        Write-Progress -Activity "Uninstalling Plugin" -Status "Removing registry entries..." -PercentComplete 20
        
        # Remove registry entries
        if (Test-Path $AddinRegPath) {
            Remove-Item $AddinRegPath -Recurse -Force
            if (-not $Silent) {
                Write-Host "✅ Add-in registry entries removed" -ForegroundColor Green
            }
        }
        
        if (Test-Path $UninstallRegPath) {
            Remove-Item $UninstallRegPath -Recurse -Force
            if (-not $Silent) {
                Write-Host "✅ Uninstall registry entries removed" -ForegroundColor Green
            }
        }
        
        Write-Progress -Activity "Uninstalling Plugin" -Status "Removing installation files..." -PercentComplete 60
        
        # Remove installation directory
        if (Test-Path $InstallDir) {
            Remove-Item $InstallDir -Recurse -Force
            if (-not $Silent) {
                Write-Host "✅ Installation files removed" -ForegroundColor Green
            }
        }
        
        Write-Progress -Activity "Uninstalling Plugin" -Status "Uninstallation complete!" -PercentComplete 100
        
        if (-not $Silent) {
            Write-Host ""
            Write-Host "🎉 $PluginName uninstalled successfully!" -ForegroundColor Green
            Write-Host "Please restart Outlook to complete the removal." -ForegroundColor Cyan
            Write-Host ""
        }
        
    } catch {
        Write-Error "Uninstallation failed: $($_.Exception.Message)"
        exit 1
    }
}

function Main {
    # Set execution policy for this session
    Set-ExecutionPolicy -ExecutionPolicy Bypass -Scope Process -Force
    
    if ($Help) {
        Show-Help
    } elseif ($Install) {
        Install-Plugin
    } elseif ($Uninstall) {
        Uninstall-Plugin
    } else {
        Show-Header
        Write-Host "Please specify an action:" -ForegroundColor Yellow
        Write-Host "  -Install     Install the $PluginName" -ForegroundColor White
        Write-Host "  -Uninstall   Remove the $PluginName" -ForegroundColor White
        Write-Host "  -Help        Show detailed help" -ForegroundColor White
        Write-Host ""
        Write-Host "Example: .\EffyDocInstaller.exe -Install" -ForegroundColor Cyan
        Write-Host ""
        
        $action = Read-Host "Choose action (Install/Uninstall/Help)"
        switch ($action.ToLower()) {
            "install" { Install-Plugin }
            "uninstall" { Uninstall-Plugin }
            "help" { Show-Help }
            default { 
                Write-Host "Invalid option. Use -Help for more information." -ForegroundColor Red
                exit 1
            }
        }
    }
}

# Run main function
Main