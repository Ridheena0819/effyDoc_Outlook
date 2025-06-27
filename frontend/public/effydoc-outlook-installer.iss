; Inno Setup Script for effyDOC Outlook Add-in
; This creates a professional Windows installer (.exe)

#define MyAppName "effyDOC Outlook Add-in"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "effyDOC"
#define MyAppURL "https://d151863e-ee5d-469c-b381-52e4cf994367.preview.emergentagent.com"
#define MyAppExeName "outlook.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
AppId={{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={userappdata}\Microsoft\AddIns\effyDOC
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=license.txt
OutputDir=.
OutputBaseFilename=effyDOC-Outlook-Installer
SetupIconFile=installer-icon.ico
Compression=lzma
SolidCompression=yes
PrivilegesRequired=lowest
WizardStyle=modern
WizardImageFile=installer-banner.bmp
WizardSmallImageFile=installer-small.bmp

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 0,6.1

[Files]
; NOTE: Don't use "Flags: ignoreversion" on any shared system files

[Code]
var
  DownloadPage: TDownloadWizardPage;

function OnDownloadProgress(const Url, FileName: String; const Progress, ProgressMax: Int64): Boolean;
begin
  if Progress = ProgressMax then
    Log(Format('Successfully downloaded %s', [FileName]));
  Result := True;
end;

procedure InitializeWizard;
begin
  DownloadPage := CreateDownloadPage(SetupMessage(msgWizardPreparing), SetupMessage(msgPreparingDesc), @OnDownloadProgress);
end;

function NextButtonClick(CurPageID: Integer): Boolean;
var
  OutlookPath: String;
begin
  Result := True;
  
  if CurPageID = wpReady then begin
    // Check if Outlook is installed
    if not RegQueryStringValue(HKLM, 'SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE', '', OutlookPath) then begin
      if not RegQueryStringValue(HKCU, 'SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE', '', OutlookPath) then begin
        MsgBox('Microsoft Outlook was not found on this system.' + #13#10 + 
               'Please install Microsoft Outlook first, then run this installer again.', 
               mbError, MB_OK);
        Result := False;
        Exit;
      end;
    end;
    
    Log('Found Outlook at: ' + OutlookPath);
    
    DownloadPage.Clear;
    DownloadPage.Add('https://d151863e-ee5d-469c-b381-52e4cf994367.preview.emergentagent.com/outlook-addin/manifest.xml', 'manifest.xml', '');
    DownloadPage.Show;
    try
      try
        DownloadPage.Download; // This downloads the files to {tmp}
        Result := True;
      except
        if DownloadPage.AbortedByUser then
          Log('Aborted by user.')
        else
          SuppressibleMsgBox(AddPeriod(GetExceptionMessage), mbCriticalError, MB_OK, IDOK);
        Result := False;
      end;
    finally
      DownloadPage.Hide;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  ManifestPath: String;
begin
  if CurStep = ssPostInstall then begin
    Log('Configuring effyDOC Outlook Add-in...');
    
    // Copy downloaded manifest to installation directory
    ManifestPath := ExpandConstant('{app}\manifest.xml');
    if FileCopy(ExpandConstant('{tmp}\manifest.xml'), ManifestPath, False) then begin
      Log('Manifest copied to: ' + ManifestPath);
      
      // Register add-in in registry
      if RegWriteStringValue(HKCU, 'SOFTWARE\Microsoft\Office\16.0\WEF\Developer', 'effyDOC', ManifestPath) then
        Log('Add-in registered for Office 2016+');
      
      if RegWriteStringValue(HKCU, 'SOFTWARE\Microsoft\Office\15.0\WEF\Developer', 'effyDOC', ManifestPath) then
        Log('Add-in registered for Office 2013');
        
      // Create README file
      SaveStringToFile(ExpandConstant('{app}\README.txt'), 
        'effyDOC Outlook Add-in Installation Complete!' + #13#10 +
        '========================================' + #13#10 + #13#10 +
        'The add-in has been successfully installed and registered with Microsoft Outlook.' + #13#10 + #13#10 +
        'Next Steps:' + #13#10 +
        '1. Restart Microsoft Outlook if it''s currently running' + #13#10 +
        '2. Look for the effyDOC panel in your Outlook sidebar' + #13#10 +
        '3. Sign in with your effyDOC account credentials' + #13#10 +
        '4. Start tracking your document engagement!' + #13#10 + #13#10 +
        'Troubleshooting:' + #13#10 +
        '- If the add-in doesn''t appear, try restarting Outlook' + #13#10 +
        '- Ensure you have an active effyDOC account' + #13#10 +
        '- Visit our help center for detailed setup guides' + #13#10 + #13#10 +
        'Support: https://d151863e-ee5d-469c-b381-52e4cf994367.preview.emergentagent.com/integrations.html',
        False);
        
      Log('Installation completed successfully');
    end else begin
      Log('Failed to copy manifest file');
    end;
  end;
end;

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\README.txt"
Name: "{group}\effyDOC Website"; Filename: "{#MyAppURL}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\README.txt"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\README.txt"; Tasks: quicklaunchicon

[Run]
Filename: "outlook.exe"; Description: "{cm:LaunchProgram,Microsoft Outlook}"; Flags: nowait postinstall skipifsilent shellexec
Filename: "{app}\README.txt"; Description: "View installation instructions"; Flags: nowait postinstall skipifsilent shellexec

[UninstallRun]

[UninstallDelete]
Type: files; Name: "{app}\manifest.xml"
Type: files; Name: "{app}\README.txt"

[Registry]
; Clean up registry entries on uninstall
Root: HKCU; Subkey: "SOFTWARE\Microsoft\Office\16.0\WEF\Developer"; ValueType: none; ValueName: "effyDOC"; Flags: deletevalue uninsdeletevalue
Root: HKCU; Subkey: "SOFTWARE\Microsoft\Office\15.0\WEF\Developer"; ValueType: none; ValueName: "effyDOC"; Flags: deletevalue uninsdeletevalue

[Messages]
BeveledLabel=effyDOC Document Tracker
SetupAppTitle=Setup - {#MyAppName}
SetupWindowTitle=Setup - {#MyAppName}

[CustomMessages]
LaunchProgram=Launch %1