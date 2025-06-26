# effyDOC Outlook VSTO Add-in

A native Windows VSTO (Visual Studio Tools for Office) add-in that integrates effyDOC document tracking functionality directly into Microsoft Outlook.

## Overview

This VSTO add-in replaces the web-based Outlook Add-in with a native Windows application that provides the same functionality but as an .exe/.dll-based solution. The add-in enables users to:

- Browse and attach trackable documents from their effyDOC library
- Track email engagement in real-time (opens, clicks, page views)
- Monitor document analytics and recipient interactions
- Send documents with embedded tracking capabilities

## Features

### Core Functionality
- **Document Library Integration**: Access your effyDOC documents directly from Outlook
- **Real-time Tracking**: WebSocket-based real-time notifications for document interactions
- **Trackable Attachments**: Generate HTML attachments with embedded tracking links
- **Live Analytics Dashboard**: View engagement metrics in real-time
- **Custom Ribbon Interface**: Easy access via Outlook ribbon

### Task Panes
1. **Document Library**: Browse and select documents from your library or content hub
2. **Live Tracking**: Monitor real-time document engagement and analytics
3. **Attachment Workflow**: Configure and attach trackable documents

### Advanced Features
- **Page-wise Tracking**: Track which pages recipients view and for how long
- **Multi-format Support**: Handle various document types (PDF, DOCX, HTML)
- **Notification Settings**: Configurable real-time notifications
- **Offline Capability**: Works without constant internet connection

## System Requirements

- **Operating System**: Windows 10/11
- **Microsoft Office**: Outlook 2016 or later
- **Framework**: .NET Framework 4.7.2 or later
- **VSTO Runtime**: Visual Studio Tools for Office Runtime 2010 or later

## Installation

### Option 1: Direct Installation (Recommended)
1. Download the `EffyDocOutlookAddin.exe` installer
2. Run the installer as Administrator
3. Follow the installation wizard
4. Restart Outlook
5. The effyDOC tab should appear in the Outlook ribbon

### Option 2: Manual Deployment
1. Copy the add-in files to a local directory
2. Register the add-in in Windows Registry:
   ```
   HKEY_CURRENT_USER\Software\Microsoft\Office\Outlook\Addins\EffyDocOutlookAddin
   ```
3. Set the following registry values:
   - `LoadBehavior` (DWORD): `3`
   - `FriendlyName` (String): `effyDOC Document Tracker`
   - `Description` (String): `Track document engagement in real-time`

## Configuration

### Backend Connection
The add-in connects to your effyDOC backend. Update the configuration in `App.config`:

```xml
<appSettings>
  <add key="BackendUrl" value="https://your-effydoc-backend.com" />
  <add key="WebSocketUrl" value="wss://your-effydoc-backend.com/api/outlook/ws" />
</appSettings>
```

### Authentication
1. Click the **Document Library** button in the effyDOC ribbon
2. Enter your effyDOC credentials when prompted
3. The add-in will remember your session

## Usage

### Attaching Trackable Documents
1. Compose a new email in Outlook
2. Click **Attach Document** in the effyDOC ribbon
3. Select a document from your library
4. Configure tracking options:
   - Email open notifications
   - Link click tracking
   - Page view monitoring
   - Time spent tracking
5. Click **Attach to Email**

### Monitoring Engagement
1. Click **Live Tracking** in the effyDOC ribbon
2. Select a document to monitor
3. View real-time data:
   - Current readers
   - Email open rates
   - Page-wise analytics
   - Time spent statistics

### Managing Documents
1. Click **Document Library** in the effyDOC ribbon
2. Browse your documents and content hub
3. Preview documents before attaching
4. Edit document content (if permitted)

## Architecture

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Outlook       │    │   VSTO Add-in    │    │   effyDOC       │
│   Application   │◄──►│   (Native .exe)  │◄──►│   Backend API   │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌──────────────────┐
                       │   WebSocket      │
                       │   Service        │
                       └──────────────────┘
```

### Components
- **ThisAddIn.cs**: Main add-in entry point and lifecycle management
- **EffyDocRibbon**: Custom Outlook ribbon with effyDOC buttons
- **ApiService**: REST API client for backend communication
- **WebSocketService**: Real-time communication for live tracking
- **UI Forms**: Windows Forms for document management and tracking

## API Integration

The add-in integrates with the existing effyDOC backend APIs:

### Authentication
- `POST /api/auth/login` - User authentication
- `GET /api/outlook/user/session-info` - Session information

### Document Management
- `GET /api/outlook/documents/my-library` - User's documents
- `GET /api/outlook/documents/content-hub` - Shared documents
- `GET /api/outlook/documents/{id}/content` - Document content
- `POST /api/outlook/documents/{id}/attachment-data` - Generate trackable attachment

### Tracking
- `POST /api/outlook/tracking/email-sent` - Track email send events
- `GET /api/outlook/tracking/live-metrics/{id}` - Real-time metrics
- `WebSocket /api/outlook/ws` - Real-time notifications

## Security

### Data Protection
- All communications use HTTPS/WSS encryption
- JWT tokens for secure authentication
- No sensitive data stored locally
- Secure attachment generation with embedded tracking

### Privacy
- Tracking data is only collected with user consent
- Recipients can opt-out of tracking
- Data retention follows effyDOC privacy policies

## Troubleshooting

### Common Issues

**Add-in not appearing in Outlook**
- Verify VSTO Runtime is installed
- Check Windows Registry entries
- Restart Outlook as Administrator

**Authentication failures**
- Verify backend URL in App.config
- Check internet connectivity
- Ensure effyDOC credentials are correct

**WebSocket connection issues**
- Check firewall settings
- Verify WebSocket URL configuration
- Test backend connectivity

### Logging
Enable debug logging by setting `DebugMode=true` in App.config. Logs are written to:
```
%APPDATA%\EffyDocOutlookAddin\Logs\
```

### Support
For technical support:
- Email: support@effydoc.com
- Documentation: https://docs.effydoc.com
- Issue tracker: Contact your system administrator

## Development

### Building from Source
1. Open `EffyDocOutlookAddin.csproj` in Visual Studio
2. Restore NuGet packages
3. Build in Release configuration
4. The installer will be generated in `bin/Release/`

### Dependencies
- Microsoft Office Interop libraries
- VSTO Runtime
- Newtonsoft.Json for API communication
- System.Net.WebSockets for real-time features

## Version History

### Version 1.0.0
- Initial release
- Document library integration
- Real-time tracking
- Trackable attachments
- Custom Outlook ribbon

## License

Copyright © 2024 effyDOC. All rights reserved.

This software is proprietary and confidential. Distribution and modification are restricted.