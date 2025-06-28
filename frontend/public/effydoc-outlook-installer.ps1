# effyDOC Outlook Add-in Installer Script
# This PowerShell script will be converted to .exe using ps2exe

param(
    [switch]$Install,
    [switch]$Uninstall,
    [switch]$Help
)

# Display help
if ($Help) {
    Write-Host "effyDOC Outlook Add-in Installer v1.0" -ForegroundColor Cyan
    Write-Host "=====================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage:"
    Write-Host "  .\effydoc-outlook-installer.exe -Install    Install the add-in"
    Write-Host "  .\effydoc-outlook-installer.exe -Uninstall  Remove the add-in"
    Write-Host "  .\effydoc-outlook-installer.exe -Help       Show this help"
    Write-Host ""
    exit 0
}

# Main installer logic
function Install-EffyDocAddin {
    Write-Host ""
    Write-Host "🚀 effyDOC Outlook Add-in Installer" -ForegroundColor Green
    Write-Host "=====================================" -ForegroundColor Green
    Write-Host ""
    
    try {
        # Check if Outlook is installed
        Write-Host "📧 Checking Outlook installation..." -ForegroundColor Yellow
        
        $outlookPath = Get-ItemProperty -Path "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE" -ErrorAction SilentlyContinue
        
        if (-not $outlookPath) {
            Write-Host "❌ Microsoft Outlook not found. Please install Outlook first." -ForegroundColor Red
            exit 1
        }
        
        Write-Host "✅ Outlook found: $($outlookPath.'(Default)')" -ForegroundColor Green
        
        # Download manifest file
        Write-Host "📥 Downloading effyDOC add-in manifest..." -ForegroundColor Yellow
        
        $manifestUrl = "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/outlook-addin/manifest.xml"
        $tempPath = "$env:TEMP\effydoc-manifest.xml"
        
        try {
            Invoke-WebRequest -Uri $manifestUrl -OutFile $tempPath -UseBasicParsing
            Write-Host "✅ Manifest downloaded successfully" -ForegroundColor Green
        }
        catch {
            Write-Host "❌ Failed to download manifest: $($_.Exception.Message)" -ForegroundColor Red
            exit 1
        }
        
        # Create add-ins directory if it doesn't exist
        $addinsPath = "$env:USERPROFILE\AppData\Roaming\Microsoft\AddIns"
        if (-not (Test-Path $addinsPath)) {
            New-Item -Path $addinsPath -ItemType Directory -Force | Out-Null
        }
        
        # Copy manifest to add-ins directory
        $finalPath = "$addinsPath\effydoc-outlook-manifest.xml"
        Copy-Item $tempPath $finalPath -Force
        Write-Host "✅ Manifest installed to: $finalPath" -ForegroundColor Green
        
        # Register add-in in registry
        Write-Host "📝 Registering add-in in Windows Registry..." -ForegroundColor Yellow
        
        $regPath = "HKCU:\SOFTWARE\Microsoft\Office\16.0\WEF\Developer"
        if (-not (Test-Path $regPath)) {
            New-Item -Path $regPath -Force | Out-Null
        }
        
        # Add manifest path to registry
        Set-ItemProperty -Path $regPath -Name "effyDOC" -Value $finalPath
        Write-Host "✅ Add-in registered in registry" -ForegroundColor Green
        
        # Clean up temp file
        Remove-Item $tempPath -Force -ErrorAction SilentlyContinue
        
        # Success message
        Write-Host ""
        Write-Host "🎉 effyDOC Outlook Add-in installed successfully!" -ForegroundColor Green
        Write-Host ""
        Write-Host "📋 Next Steps:" -ForegroundColor Cyan
        Write-Host "1. Restart Microsoft Outlook" -ForegroundColor White
        Write-Host "2. Look for the effyDOC panel in the Outlook sidebar" -ForegroundColor White
        Write-Host "3. Sign in with your effyDOC account credentials" -ForegroundColor White
        Write-Host "4. Start tracking your documents!" -ForegroundColor White
        Write-Host ""
        Write-Host "🔗 Need help? Visit: https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/integrations.html" -ForegroundColor Cyan
        Write-Host ""
        
        # Ask if user wants to start Outlook
        $startOutlook = Read-Host "Would you like to start Outlook now? (y/n)"
        if ($startOutlook -eq "y" -or $startOutlook -eq "Y") {
            Start-Process "outlook.exe"
        }
        
    }
    catch {
        Write-Host "❌ Installation failed: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

function Uninstall-EffyDocAddin {
    Write-Host ""
    Write-Host "🗑️ effyDOC Outlook Add-in Uninstaller" -ForegroundColor Yellow
    Write-Host "======================================" -ForegroundColor Yellow
    Write-Host ""
    
    try {
        # Remove manifest file
        $addinsPath = "$env:USERPROFILE\AppData\Roaming\Microsoft\AddIns"
        $manifestPath = "$addinsPath\effydoc-outlook-manifest.xml"
        
        if (Test-Path $manifestPath) {
            Remove-Item $manifestPath -Force
            Write-Host "✅ Manifest file removed" -ForegroundColor Green
        } else {
            Write-Host "ℹ️ Manifest file not found" -ForegroundColor Yellow
        }
        
        # Remove registry entry
        $regPath = "HKCU:\SOFTWARE\Microsoft\Office\16.0\WEF\Developer"
        if (Test-Path $regPath) {
            try {
                Remove-ItemProperty -Path $regPath -Name "effyDOC" -ErrorAction SilentlyContinue
                Write-Host "✅ Registry entry removed" -ForegroundColor Green
            }
            catch {
                Write-Host "ℹ️ Registry entry not found" -ForegroundColor Yellow
            }
        }
        
        Write-Host ""
        Write-Host "🎉 effyDOC Outlook Add-in uninstalled successfully!" -ForegroundColor Green
        Write-Host "Please restart Outlook to complete the removal." -ForegroundColor Cyan
        Write-Host ""
        
    }
    catch {
        Write-Host "❌ Uninstallation failed: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# Main execution logic
if ($Install) {
    Install-EffyDocAddin
}
elseif ($Uninstall) {
    Uninstall-EffyDocAddin
}
else {
    Write-Host "effyDOC Outlook Add-in Installer v1.0" -ForegroundColor Cyan
    Write-Host "=====================================" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Please specify an action:"
    Write-Host "  -Install     Install the effyDOC Outlook Add-in"
    Write-Host "  -Uninstall   Remove the effyDOC Outlook Add-in"
    Write-Host "  -Help        Show detailed help"
    Write-Host ""
    Write-Host "Example: .\effydoc-outlook-installer.exe -Install"
    Write-Host ""
    
    $action = Read-Host "Choose action (Install/Uninstall/Help)"
    switch ($action.ToLower()) {
        "install" { Install-EffyDocAddin }
        "uninstall" { Uninstall-EffyDocAddin }
        "help" { & $MyInvocation.MyCommand.Path -Help }
        default { 
            Write-Host "Invalid option. Use -Help for more information." -ForegroundColor Red
            exit 1
        }
    }
}