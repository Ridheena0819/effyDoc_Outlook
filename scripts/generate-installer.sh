#!/bin/bash
# Automated installer generation script for Linux/Mac environments

set -e

echo "🚀 Generating effyDOC Outlook Plugin Installer"
echo "=============================================="

# Configuration
VERSION="1.0.0"
BACKEND_URL="${BACKEND_URL:-https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com}"
OUTPUT_DIR="../frontend/public"

# Ensure output directory exists
mkdir -p "$OUTPUT_DIR"

# Check if PowerShell is available (for cross-platform)
if command -v pwsh >/dev/null 2>&1; then
    echo "✅ PowerShell Core found"
    POWERSHELL_CMD="pwsh"
elif command -v powershell >/dev/null 2>&1; then
    echo "✅ PowerShell found"
    POWERSHELL_CMD="powershell"
else
    echo "❌ PowerShell not found. Installing..."
    
    # Install PowerShell based on OS
    if [[ "$OSTYPE" == "linux-gnu"* ]]; then
        # Ubuntu/Debian
        if command -v apt-get >/dev/null 2>&1; then
            sudo apt-get update
            sudo apt-get install -y wget apt-transport-https software-properties-common
            wget -q https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb
            sudo dpkg -i packages-microsoft-prod.deb
            sudo apt-get update
            sudo apt-get install -y powershell
            POWERSHELL_CMD="pwsh"
        # RHEL/CentOS
        elif command -v yum >/dev/null 2>&1; then
            sudo rpm --import https://packages.microsoft.com/keys/microsoft.asc
            sudo yum install -y https://packages.microsoft.com/config/rhel/8/packages-microsoft-prod.rpm
            sudo yum install -y powershell
            POWERSHELL_CMD="pwsh"
        fi
    elif [[ "$OSTYPE" == "darwin"* ]]; then
        # macOS
        if command -v brew >/dev/null 2>&1; then
            brew install --cask powershell
            POWERSHELL_CMD="pwsh"
        fi
    fi
fi

if ! command -v "$POWERSHELL_CMD" >/dev/null 2>&1; then
    echo "❌ Failed to install PowerShell. Please install manually and re-run."
    exit 1
fi

echo "🔧 Running PowerShell installer builder..."

# Run the PowerShell build script
$POWERSHELL_CMD -File "./build-outlook-installer.ps1" -Version "$VERSION" -OutputPath "$OUTPUT_DIR" -BackendURL "$BACKEND_URL"

if [ $? -eq 0 ]; then
    echo ""
    echo "🎉 Installer generation completed successfully!"
    echo "📦 Installer location: $OUTPUT_DIR/EffyDocOutlookPlugin-Setup.exe"
    echo ""
    echo "Next steps:"
    echo "1. The .exe installer is now available for download"
    echo "2. Users can download and run it to install the Outlook plugin"
    echo "3. The plugin will appear in Outlook sidebar after installation"
    echo ""
else
    echo "❌ Installer generation failed"
    exit 1
fi