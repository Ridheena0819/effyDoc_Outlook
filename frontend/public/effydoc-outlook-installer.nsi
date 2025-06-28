# NSIS Script for effyDOC Outlook Add-in Installer
# This creates a professional Windows .exe installer

!define PRODUCT_NAME "effyDOC Outlook Add-in"
!define PRODUCT_VERSION "1.0.0"
!define PRODUCT_PUBLISHER "effyDOC"
!define PRODUCT_WEB_SITE "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\effyDOC-Outlook"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKLM"

# Modern UI
!include "MUI2.nsh"
!include "WinMessages.nsh"
!include "LogicLib.nsh"

# Installer settings
Name "${PRODUCT_NAME}"
OutFile "effyDOC-Outlook-Installer.exe"
InstallDir "$APPDATA\Microsoft\AddIns\effyDOC"
InstallDirRegKey HKCU "${PRODUCT_DIR_REGKEY}" ""
RequestExecutionLevel user
ShowInstDetails show
ShowUnInstDetails show

# Version Information
VIProductVersion "1.0.0.0"
VIAddVersionKey "ProductName" "${PRODUCT_NAME}"
VIAddVersionKey "CompanyName" "${PRODUCT_PUBLISHER}"
VIAddVersionKey "FileVersion" "${PRODUCT_VERSION}"
VIAddVersionKey "ProductVersion" "${PRODUCT_VERSION}"
VIAddVersionKey "FileDescription" "${PRODUCT_NAME} Installer"
VIAddVersionKey "LegalCopyright" "© ${PRODUCT_PUBLISHER}"

# Modern UI Configuration
!define MUI_ABORTWARNING
!define MUI_ICON "installer-icon.ico"
!define MUI_UNICON "installer-icon.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "installer-banner.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "installer-banner.bmp"

# Welcome page
!define MUI_WELCOMEPAGE_TITLE "Welcome to effyDOC Outlook Add-in Setup"
!define MUI_WELCOMEPAGE_TEXT "This wizard will guide you through the installation of ${PRODUCT_NAME}.$\r$\n$\r$\nThe add-in enables real-time document tracking directly in Microsoft Outlook with features like:$\r$\n• Track when emails are opened$\r$\n• See when recipients click links$\r$\n• Monitor page-by-page reading$\r$\n• Real-time engagement analytics$\r$\n$\r$\nClick Next to continue."
!insertmacro MUI_PAGE_WELCOME

# License page
!insertmacro MUI_PAGE_LICENSE "license.txt"

# Directory page
!insertmacro MUI_PAGE_DIRECTORY

# Components page
!insertmacro MUI_PAGE_COMPONENTS

# Installation page
!insertmacro MUI_PAGE_INSTFILES

# Finish page
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_TEXT "Launch Microsoft Outlook"
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchOutlook"
!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\README.txt"
!define MUI_FINISHPAGE_SHOWREADME_TEXT "Show installation instructions"
!insertmacro MUI_PAGE_FINISH

# Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

# Language
!insertmacro MUI_LANGUAGE "English"

# Sections
Section "effyDOC Outlook Add-in" SEC01
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  
  # Show installation progress
  DetailPrint "Checking Microsoft Outlook installation..."
  
  # Check if Outlook is installed
  ReadRegStr $R0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE" ""
  StrCmp $R0 "" outlook_not_found outlook_found
  
  outlook_not_found:
    MessageBox MB_ICONSTOP "Microsoft Outlook was not found on this system.$\r$\nPlease install Microsoft Outlook first, then run this installer again."
    Abort
  
  outlook_found:
    DetailPrint "Microsoft Outlook found: $R0"
  
  # Create installation directory
  CreateDirectory "$INSTDIR"
  
  # Download manifest file
  DetailPrint "Downloading effyDOC add-in manifest..."
  
  # Use built-in HTTP download (NSIS 3.0+)
  NSISdl::download "https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/outlook-addin/manifest.xml" "$INSTDIR\manifest.xml"
  Pop $R0
  StrCmp $R0 "success" download_ok
    MessageBox MB_ICONSTOP "Failed to download add-in manifest.$\r$\nPlease check your internet connection and try again."
    Abort
  
  download_ok:
    DetailPrint "Manifest downloaded successfully"
  
  # Create additional files
  FileOpen $9 "$INSTDIR\README.txt" w
  FileWrite $9 "effyDOC Outlook Add-in Installation Complete!$\r$\n"
  FileWrite $9 "========================================$\r$\n$\r$\n"
  FileWrite $9 "The add-in has been successfully installed and registered with Microsoft Outlook.$\r$\n$\r$\n"
  FileWrite $9 "Next Steps:$\r$\n"
  FileWrite $9 "1. Restart Microsoft Outlook if it's currently running$\r$\n"
  FileWrite $9 "2. Look for the effyDOC panel in your Outlook sidebar$\r$\n"
  FileWrite $9 "3. Sign in with your effyDOC account credentials$\r$\n"
  FileWrite $9 "4. Start tracking your document engagement!$\r$\n$\r$\n"
  FileWrite $9 "Troubleshooting:$\r$\n"
  FileWrite $9 "- If the add-in doesn't appear, try restarting Outlook$\r$\n"
  FileWrite $9 "- Ensure you have an active effyDOC account$\r$\n"
  FileWrite $9 "- Visit our help center for detailed setup guides$\r$\n$\r$\n"
  FileWrite $9 "Support: https://29429f14-70dc-41c8-bef0-98a176108ced.preview.emergentagent.com/integrations.html$\r$\n"
  FileClose $9
  
  # Register add-in in Windows Registry
  DetailPrint "Registering add-in with Microsoft Office..."
  
  # Create registry entries for Office add-in
  WriteRegStr HKCU "SOFTWARE\Microsoft\Office\16.0\WEF\Developer" "effyDOC" "$INSTDIR\manifest.xml"
  WriteRegStr HKCU "SOFTWARE\Microsoft\Office\15.0\WEF\Developer" "effyDOC" "$INSTDIR\manifest.xml"
  
  # Registry entries for uninstaller
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\manifest.xml"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  WriteRegDWORD HKCU "${PRODUCT_UNINST_KEY}" "NoModify" 1
  WriteRegDWORD HKCU "${PRODUCT_UNINST_KEY}" "NoRepair" 1
  
  # Create uninstaller
  WriteUninstaller "$INSTDIR\uninst.exe"
  
  DetailPrint "Installation completed successfully!"
SectionEnd

Section "Desktop Shortcut" SEC02
  CreateShortCut "$DESKTOP\effyDOC Outlook Add-in.lnk" "$INSTDIR\README.txt" "" "$INSTDIR\manifest.xml" 0
SectionEnd

Section "Start Menu Shortcuts" SEC03
  CreateDirectory "$SMPROGRAMS\effyDOC"
  CreateShortCut "$SMPROGRAMS\effyDOC\effyDOC Outlook Add-in.lnk" "$INSTDIR\README.txt" "" "$INSTDIR\manifest.xml" 0
  CreateShortCut "$SMPROGRAMS\effyDOC\Uninstall.lnk" "$INSTDIR\uninst.exe"
  CreateShortCut "$SMPROGRAMS\effyDOC\effyDOC Website.lnk" "${PRODUCT_WEB_SITE}"
SectionEnd

# Section descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC01} "Installs the core effyDOC Outlook Add-in files and registers them with Microsoft Office."
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC02} "Creates a desktop shortcut for easy access to installation information."
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC03} "Creates Start Menu shortcuts for the add-in and uninstaller."
!insertmacro MUI_FUNCTION_DESCRIPTION_END

# Functions
Function LaunchOutlook
  ExecShell "" "outlook.exe"
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?" IDYES +2
  Abort
FunctionEnd

Section Uninstall
  # Remove registry entries
  DeleteRegKey HKCU "SOFTWARE\Microsoft\Office\16.0\WEF\Developer\effyDOC"
  DeleteRegKey HKCU "SOFTWARE\Microsoft\Office\15.0\WEF\Developer\effyDOC"
  DeleteRegKey HKCU "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKCU "${PRODUCT_DIR_REGKEY}"
  
  # Remove files
  Delete "$INSTDIR\manifest.xml"
  Delete "$INSTDIR\README.txt"
  Delete "$INSTDIR\uninst.exe"
  
  # Remove shortcuts
  Delete "$DESKTOP\effyDOC Outlook Add-in.lnk"
  Delete "$SMPROGRAMS\effyDOC\effyDOC Outlook Add-in.lnk"
  Delete "$SMPROGRAMS\effyDOC\Uninstall.lnk"
  Delete "$SMPROGRAMS\effyDOC\effyDOC Website.lnk"
  RMDir "$SMPROGRAMS\effyDOC"
  
  # Remove directories
  RMDir "$INSTDIR"
  
  SetAutoClose true
SectionEnd