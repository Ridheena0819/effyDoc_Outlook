# effyDOC Outlook VSTO Add-in Installation Guide

## Quick Start

### Prerequisites
1. **Windows 10/11** with Administrator privileges
2. **Microsoft Outlook 2016 or later**
3. **.NET Framework 4.7.2 or later**
4. **Visual Studio Tools for Office Runtime** (VSTO Runtime)

### Step 1: Install VSTO Runtime
If not already installed, download and install the VSTO Runtime:
```
https://www.microsoft.com/en-us/download/details.aspx?id=48217
```

### Step 2: Build the Add-in (Development)
```bash
# Navigate to the project directory
cd /app/outlook-vsto-addin

# Run the build script
./build.bat
```

### Step 3: Deploy the Add-in
1. Copy the contents of the `publish/` folder to your target directory
2. Run `EffyDocOutlookAddin.exe` as Administrator
3. Restart Microsoft Outlook

## Manual Installation

### Registry Entries
If automated installation fails, manually add these registry entries:

**For Current User (HKEY_CURRENT_USER):**
```
[HKEY_CURRENT_USER\Software\Microsoft\Office\Outlook\Addins\EffyDocOutlookAddin]
"LoadBehavior"=dword:00000003
"FriendlyName"="effyDOC Document Tracker"
"Description"="Track document engagement in real-time directly from Outlook"
"Manifest"="file:///C:/Path/To/EffyDocOutlookAddin.vsto"
```

**For All Users (HKEY_LOCAL_MACHINE):**
```
[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Office\Outlook\Addins\EffyDocOutlookAddin]
"LoadBehavior"=dword:00000003
"FriendlyName"="effyDOC Document Tracker"
"Description"="Track document engagement in real-time directly from Outlook"
"Manifest"="file:///C:/Path/To/EffyDocOutlookAddin.vsto"
```

### Load Behavior Values
- `0` = Disconnected (don't load)
- `1` = Connected (loaded)
- `2` = Bootload (load at startup)
- `3` = Demand Load (load when needed) - **Recommended**

## Configuration

### Backend Connection
Edit `App.config` to point to your effyDOC backend:

```xml
<configuration>
  <appSettings>
    <!-- Update these URLs to match your effyDOC deployment -->
    <add key="BackendUrl" value="https://your-effydoc-backend.com" />
    <add key="WebSocketUrl" value="wss://your-effydoc-backend.com/api/outlook/ws" />
    
    <!-- Optional: Enable debug mode for troubleshooting -->
    <add key="DebugMode" value="false" />
    <add key="LogLevel" value="Info" />
  </appSettings>
</configuration>
```

## Verification

### Check Installation
1. Open Microsoft Outlook
2. Look for the **effyDOC** tab in the ribbon
3. The tab should contain buttons: **Document Library**, **Live Tracking**, **Attach Document**

### Test Functionality
1. Click **Document Library** - should prompt for login
2. Enter your effyDOC credentials
3. Verify documents load in the task pane
4. Test attaching a document to an email

## Troubleshooting

### Add-in Not Loading
**Problem**: effyDOC tab doesn't appear in Outlook

**Solutions**:
1. Verify VSTO Runtime is installed
2. Check registry entries are correct
3. Ensure Outlook is running as the same user who installed the add-in
4. Try running Outlook as Administrator
5. Check Windows Event Viewer for error messages

### Authentication Issues
**Problem**: Cannot login to effyDOC

**Solutions**:
1. Verify `BackendUrl` in App.config is correct
2. Test backend connectivity in a web browser
3. Check firewall settings
4. Ensure SSL certificates are valid

### WebSocket Connection Failed
**Problem**: Real-time tracking not working

**Solutions**:
1. Verify `WebSocketUrl` in App.config
2. Check proxy/firewall settings for WebSocket connections
3. Test WebSocket endpoint manually
4. Enable debug logging to see connection attempts

### Performance Issues
**Problem**: Add-in is slow or unresponsive

**Solutions**:
1. Check internet connection speed
2. Verify backend server performance
3. Enable logging to identify bottlenecks
4. Consider using a local cache

## Security Considerations

### Certificates
For production deployment, sign the add-in with a valid code signing certificate:
```bash
signtool sign /f certificate.pfx /p password EffyDocOutlookAddin.exe
```

### Trusted Locations
Add the installation directory to Office Trusted Locations:
1. Open Outlook
2. Go to File → Options → Trust Center → Trust Center Settings
3. Click "Trusted Locations"
4. Add the add-in installation directory

### Group Policy
For enterprise deployment, use Group Policy to:
1. Deploy registry entries
2. Install VSTO Runtime
3. Configure trusted locations
4. Set security policies

## Uninstallation

### Automatic Uninstall
1. Go to Control Panel → Programs and Features
2. Find "effyDOC Outlook Add-in"
3. Click Uninstall

### Manual Uninstall
1. Close Outlook
2. Delete registry entries:
   ```
   HKEY_CURRENT_USER\Software\Microsoft\Office\Outlook\Addins\EffyDocOutlookAddin
   ```
3. Delete installation files
4. Restart Outlook

## Enterprise Deployment

### Silent Installation
For mass deployment, use the `/quiet` parameter:
```bash
EffyDocOutlookAddin.exe /quiet BACKEND_URL="https://company-effydoc.com"
```

### MSI Package
Convert to MSI for enterprise deployment tools:
1. Use tools like Advanced Installer or WiX Toolset
2. Include VSTO Runtime as prerequisite
3. Configure registry entries and file associations
4. Add custom actions for configuration

### System Center Configuration Manager (SCCM)
1. Package the MSI with VSTO Runtime
2. Create detection rules for successful installation
3. Set requirements for .NET Framework and Office versions
4. Deploy to target computer collections

## Support

For technical assistance:
- **Documentation**: Check the full README.md for detailed usage instructions
- **Logs**: Enable debug mode and check logs in `%APPDATA%\EffyDocOutlookAddin\Logs\`
- **Event Viewer**: Check Windows Event Viewer for system-level errors
- **Network**: Use tools like Fiddler to debug API communication

### Common Error Codes
- **0x80004005**: General failure - check registry and file permissions
- **0x80070005**: Access denied - run as Administrator
- **0x800A01A8**: Object required - VSTO Runtime not installed
- **0x80131040**: Assembly loading error - check .NET Framework version