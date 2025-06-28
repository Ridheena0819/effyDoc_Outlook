#!/bin/bash
"""
Cross-platform Windows .exe builder
Builds real Windows executable from Python code using Wine + PyInstaller
"""

set -e

echo "🚀 Building Real Windows .exe from Python Code"
echo "=============================================="

# Configuration
PYTHON_INSTALLER_SCRIPT="/app/scripts/python_installer.py"
OUTPUT_DIR="/app/frontend/public"
WINE_PREFIX="$HOME/.wine_pyinstaller"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[0;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

print_status() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if script exists
if [ ! -f "$PYTHON_INSTALLER_SCRIPT" ]; then
    print_error "Python installer script not found: $PYTHON_INSTALLER_SCRIPT"
    exit 1
fi

# Create output directory
mkdir -p "$OUTPUT_DIR"

print_status "Checking system requirements..."

# Option 1: Try Wine approach (cross-compilation)
install_wine_and_python() {
    print_status "Installing Wine for cross-compilation..."
    
    # Update package list
    sudo apt-get update
    
    # Install Wine
    sudo apt-get install -y wine
    
    # Configure Wine
    export WINEPREFIX="$WINE_PREFIX"
    winecfg &
    sleep 5
    pkill winecfg || true
    
    print_status "Downloading Windows Python..."
    
    # Download Windows Python installer
    PYTHON_URL="https://www.python.org/ftp/python/3.11.7/python-3.11.7-amd64.exe"
    PYTHON_INSTALLER="/tmp/python-installer.exe"
    
    if [ ! -f "$PYTHON_INSTALLER" ]; then
        wget -O "$PYTHON_INSTALLER" "$PYTHON_URL"
    fi
    
    print_status "Installing Python in Wine..."
    wine "$PYTHON_INSTALLER" /quiet InstallAllUsers=1 PrependPath=1
    
    print_status "Installing PyInstaller in Wine..."
    wine python -m pip install pyinstaller
    
    return 0
}

# Option 2: Docker approach
build_with_docker() {
    print_status "Building with Docker Windows container..."
    
    # Create Dockerfile for Windows build
    cat > /tmp/Dockerfile.windows << 'EOF'
FROM python:3.11-windowsservercore

WORKDIR /app
COPY python_installer.py .

RUN pip install pyinstaller

CMD ["pyinstaller", "--onefile", "--windowed", "--name", "EffyDocOutlookPlugin-Setup", "python_installer.py"]
EOF
    
    # Copy installer script
    cp "$PYTHON_INSTALLER_SCRIPT" /tmp/python_installer.py
    
    # Build with Docker
    docker build -f /tmp/Dockerfile.windows -t effydoc-installer /tmp/
    docker run --rm -v "$OUTPUT_DIR:/output" effydoc-installer
    
    return $?
}

# Option 3: GitHub Actions approach
create_github_action() {
    print_status "Creating GitHub Action for Windows build..."
    
    mkdir -p .github/workflows
    
    cat > .github/workflows/build-windows-installer.yml << 'EOF'
name: Build Windows Installer

on:
  push:
    branches: [ main, master ]
  pull_request:
    branches: [ main, master ]
  workflow_dispatch:

jobs:
  build-windows:
    runs-on: windows-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Set up Python
      uses: actions/setup-python@v4
      with:
        python-version: '3.11'
    
    - name: Install dependencies
      run: |
        python -m pip install --upgrade pip
        pip install pyinstaller
    
    - name: Build Windows executable
      run: |
        pyinstaller --onefile --windowed --name EffyDocOutlookPlugin-Setup scripts/python_installer.py
    
    - name: Upload artifact
      uses: actions/upload-artifact@v3
      with:
        name: windows-installer
        path: dist/EffyDocOutlookPlugin-Setup.exe
    
    - name: Deploy to release
      if: github.ref == 'refs/heads/main'
      run: |
        # Copy to frontend/public for download
        mkdir -p frontend/public
        cp dist/EffyDocOutlookPlugin-Setup.exe frontend/public/
EOF
    
    print_success "GitHub Action created at .github/workflows/build-windows-installer.yml"
    print_status "Push to GitHub to trigger the build"
    return 0
}

# Option 4: Cloud build service
build_with_cloud_service() {
    print_status "Using cloud build service..."
    
    # Create build request
    cat > /tmp/build_request.json << EOF
{
    "platform": "windows",
    "python_version": "3.11",
    "script": "$(base64 -w 0 < $PYTHON_INSTALLER_SCRIPT)",
    "output_name": "EffyDocOutlookPlugin-Setup.exe",
    "build_args": ["--onefile", "--windowed"]
}
EOF
    
    # This would be a real cloud service call
    print_warning "Cloud build service integration would be implemented here"
    return 1
}

# Option 5: Native Linux PyInstaller with Windows target
build_native_cross() {
    print_status "Attempting cross-compilation with native PyInstaller..."
    
    # Install required packages
    pip install pyinstaller
    
    # Try to cross-compile (this usually doesn't work but worth trying)
    cd /app/scripts
    
    print_status "Creating spec file for cross-compilation..."
    
    cat > effydoc_installer.spec << 'EOF'
# -*- mode: python ; coding: utf-8 -*-

block_cipher = None

a = Analysis(
    ['python_installer.py'],
    pathex=[],
    binaries=[],
    datas=[],
    hiddenimports=[],
    hookspath=[],
    hooksconfig={},
    runtime_hooks=[],
    excludes=[],
    win_no_prefer_redirects=False,
    win_private_assemblies=False,
    cipher=block_cipher,
    noarchive=False,
)

pyz = PYZ(a.pure, a.zipped_data, cipher=block_cipher)

exe = EXE(
    pyz,
    a.scripts,
    a.binaries,
    a.zipfiles,
    a.datas,
    [],
    name='EffyDocOutlookPlugin-Setup',
    debug=False,
    bootloader_ignore_signals=False,
    strip=False,
    upx=True,
    upx_exclude=[],
    runtime_tmpdir=None,
    console=False,
    disable_windowed_traceback=False,
    argv_emulation=False,
    target_arch=None,
    codesign_identity=None,
    entitlements_file=None,
)
EOF
    
    # Try to build
    if pyinstaller effydoc_installer.spec --distpath "$OUTPUT_DIR"; then
        print_success "Cross-compilation successful!"
        return 0
    else
        print_warning "Cross-compilation failed (expected on non-Windows)"
        return 1
    fi
}

# Option 6: Create self-extracting archive
create_self_extracting() {
    print_status "Creating self-extracting Python installer..."
    
    # Create a Python script that embeds the installer
    cat > /tmp/embedded_installer.py << 'EOF'
#!/usr/bin/env python3
import base64
import tempfile
import os
import subprocess
import sys

# Embedded Python installer script (base64 encoded)
INSTALLER_CODE = """
EOF
    
    # Embed the installer code
    base64 -w 0 < "$PYTHON_INSTALLER_SCRIPT" >> /tmp/embedded_installer.py
    
    cat >> /tmp/embedded_installer.py << 'EOF'
"""

def main():
    # Check if running on Windows
    if os.name != 'nt':
        print("This installer requires Windows.")
        sys.exit(1)
    
    # Decode and run installer
    import base64
    installer_code = base64.b64decode(INSTALLER_CODE).decode('utf-8')
    
    # Write to temp file and execute
    with tempfile.NamedTemporaryFile(mode='w', suffix='.py', delete=False) as f:
        f.write(installer_code)
        temp_path = f.name
    
    try:
        subprocess.run([sys.executable, temp_path], check=True)
    finally:
        os.unlink(temp_path)

if __name__ == "__main__":
    main()
EOF
    
    # Copy to output directory
    cp /tmp/embedded_installer.py "$OUTPUT_DIR/EffyDocOutlookPlugin-Setup.py"
    
    print_success "Self-extracting Python installer created"
    print_status "Users can run: python EffyDocOutlookPlugin-Setup.py"
    
    return 0
}

# Main execution
print_status "Starting Windows .exe build process..."

# Try different approaches in order of preference
if command -v docker >/dev/null 2>&1; then
    print_status "Docker available, attempting Docker build..."
    if build_with_docker; then
        print_success "Docker build completed successfully!"
        exit 0
    else
        print_warning "Docker build failed, trying next method..."
    fi
fi

# Try Wine approach
if command -v wine >/dev/null 2>&1 || install_wine_and_python; then
    print_status "Wine available, attempting Wine build..."
    export WINEPREFIX="$WINE_PREFIX"
    
    cd /tmp
    cp "$PYTHON_INSTALLER_SCRIPT" ./installer.py
    
    if wine python -m PyInstaller --onefile --windowed --name EffyDocOutlookPlugin-Setup installer.py --distpath "$OUTPUT_DIR"; then
        print_success "Wine build completed successfully!"
        exit 0
    else
        print_warning "Wine build failed, trying next method..."
    fi
fi

# Try native cross-compilation
if build_native_cross; then
    print_success "Native cross-compilation completed!"
    exit 0
fi

# Create GitHub Action
if create_github_action; then
    print_success "GitHub Action created for cloud building"
fi

# Fallback to self-extracting Python
if create_self_extracting; then
    print_success "Self-extracting Python installer created as fallback"
fi

# Final status
if [ -f "$OUTPUT_DIR/EffyDocOutlookPlugin-Setup.exe" ]; then
    file_size=$(stat -c%s "$OUTPUT_DIR/EffyDocOutlookPlugin-Setup.exe")
    print_success "Windows .exe created successfully!"
    print_status "File: $OUTPUT_DIR/EffyDocOutlookPlugin-Setup.exe"
    print_status "Size: $file_size bytes"
    
    # Test if it's a real executable
    if file "$OUTPUT_DIR/EffyDocOutlookPlugin-Setup.exe" | grep -q "PE32"; then
        print_success "✅ Real Windows PE32 executable created!"
    else
        print_warning "⚠️ File created but may not be a valid Windows executable"
    fi
else
    print_error "Failed to create Windows .exe file"
    print_status "Available alternatives:"
    print_status "1. Use GitHub Actions for cloud building"
    print_status "2. Use the Python script directly on Windows"
    print_status "3. Use the batch installer as fallback"
    exit 1
fi