# NSIS Script for effyDOC Native Outlook Plugin Installer
# This creates a professional Windows .exe installer for the native VSTO add-in

!define PRODUCT_NAME "effyDOC Outlook Plugin"
!define PRODUCT_VERSION "1.0.0"
!define PRODUCT_PUBLISHER "effyDOC"
!define PRODUCT_WEB_SITE "https://62eb7680-5805-4576-a9b6-a02867b40493.preview.emergentagent.com"
!define PRODUCT_DIR_REGKEY "Software\Microsoft\Windows\CurrentVersion\App Paths\EffyDocOutlookPlugin"
!define PRODUCT_UNINST_KEY "Software\Microsoft\Windows\CurrentVersion\Uninstall\${PRODUCT_NAME}"
!define PRODUCT_UNINST_ROOT_KEY "HKCU"

# Modern UI
!include "MUI2.nsh"
!include "WinMessages.nsh"
!include "LogicLib.nsh"
!include "x64.nsh"

# Installer settings
Name "${PRODUCT_NAME}"
OutFile "EffyDocOutlookPlugin-Setup.exe"
InstallDir "$LOCALAPPDATA\${PRODUCT_NAME}"
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
!define MUI_ICON "effydoc-icon.ico"
!define MUI_UNICON "effydoc-icon.ico"
!define MUI_WELCOMEFINISHPAGE_BITMAP "installer-banner.bmp"
!define MUI_UNWELCOMEFINISHPAGE_BITMAP "installer-banner.bmp"

# Welcome page
!define MUI_WELCOMEPAGE_TITLE "Welcome to ${PRODUCT_NAME} Setup"
!define MUI_WELCOMEPAGE_TEXT "This wizard will install ${PRODUCT_NAME} on your computer.$\r$\n$\r$\nThe plugin enables seamless document tracking directly in Microsoft Outlook with features like:$\r$\n• Real-time document engagement analytics$\r$\n• One-click trackable document attachments$\r$\n• Professional document sharing$\r$\n• Email open and click tracking$\r$\n$\r$\nClick Next to continue or Cancel to exit Setup."
!insertmacro MUI_PAGE_WELCOME

# License page
!insertmacro MUI_PAGE_LICENSE "License.txt"

# Directory page
!insertmacro MUI_PAGE_DIRECTORY

# Installation page
!insertmacro MUI_PAGE_INSTFILES

# Finish page
!define MUI_FINISHPAGE_RUN
!define MUI_FINISHPAGE_RUN_TEXT "Launch Microsoft Outlook"
!define MUI_FINISHPAGE_RUN_FUNCTION "LaunchOutlook"
!define MUI_FINISHPAGE_SHOWREADME "$INSTDIR\README.txt"
!define MUI_FINISHPAGE_SHOWREADME_TEXT "View installation guide"
!define MUI_FINISHPAGE_LINK "Visit effyDOC website"
!define MUI_FINISHPAGE_LINK_LOCATION "${PRODUCT_WEB_SITE}"
!insertmacro MUI_PAGE_FINISH

# Uninstaller pages
!insertmacro MUI_UNPAGE_INSTFILES

# Language
!insertmacro MUI_LANGUAGE "English"

# Prerequisites check function
Function CheckPrerequisites
  DetailPrint "Checking system requirements..."
  
  # Check if running on supported Windows version
  ${If} ${AtMostWinXP}
    MessageBox MB_ICONSTOP "This plugin requires Windows Vista or later.$\r$\nPlease upgrade your operating system."
    Abort
  ${EndIf}
  
  # Check if Outlook is installed
  ReadRegStr $R0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE" ""
  ${If} $R0 == ""
    # Check 32-bit registry on 64-bit system
    ${If} ${RunningX64}
      SetRegView 32
      ReadRegStr $R0 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE" ""
      SetRegView 64
    ${EndIf}
    
    ${If} $R0 == ""
      MessageBox MB_ICONSTOP "Microsoft Outlook was not found on this system.$\r$\n$\r$\nPlease install Microsoft Outlook first, then run this installer again.$\r$\n$\r$\nSupported versions: Outlook 2013, 2016, 2019, or Microsoft 365."
      Abort
    ${EndIf}
  ${EndIf}
  
  DetailPrint "Microsoft Outlook found: $R0"
  
  # Check .NET Framework 4.0 or higher
  ReadRegDWORD $R1 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
  ${If} $R1 == ""
    MessageBox MB_ICONSTOP "Microsoft .NET Framework 4.0 or higher is required.$\r$\n$\r$\nPlease install .NET Framework from Microsoft, then run this installer again."
    Abort
  ${EndIf}
  
  DetailPrint ".NET Framework found"
  
  # Check VSTO Runtime
  ReadRegStr $R2 HKLM "SOFTWARE\Microsoft\VSTO Runtime Setup\v4R" "Version"
  ${If} $R2 == ""
    MessageBox MB_ICONQUESTION|MB_YESNO "Microsoft Visual Studio Tools for Office Runtime is required but not found.$\r$\n$\r$\nWould you like to download and install it now?$\r$\n$\r$\n(This is required for the plugin to work)" IDYES DownloadVSTO IDNO SkipVSTO
    
    DownloadVSTO:
      ExecShell "open" "https://www.microsoft.com/en-us/download/details.aspx?id=48217"
      MessageBox MB_ICONINFORMATION "Please install Visual Studio Tools for Office Runtime, then run this installer again."
      Abort
    
    SkipVSTO:
      MessageBox MB_ICONEXCLAMATION "The plugin may not work properly without VSTO Runtime."
  ${EndIf}
  
  DetailPrint "All prerequisites satisfied"
FunctionEnd

# Main installation section
Section "effyDOC Outlook Plugin" SEC01
  Call CheckPrerequisites
  
  SetOutPath "$INSTDIR"
  SetOverwrite ifnewer
  
  # Install main files
  DetailPrint "Installing plugin files..."
  File "EffyDocOutlookPlugin.dll"
  File "EffyDocOutlookPlugin.dll.manifest"
  File "EffyDocOutlookPlugin.vsto"
  File "Newtonsoft.Json.dll"
  
  # Create README file
  DetailPrint "Creating documentation..."
  FileOpen $9 "$INSTDIR\README.txt" w
  FileWrite $9 "effyDOC Outlook Plugin - Installation Complete!$\r$\n"
  FileWrite $9 "===================================================$\r$\n$\r$\n"
  FileWrite $9 "Thank you for installing the effyDOC Outlook Plugin!$\r$\n$\r$\n"
  FileWrite $9 "WHAT'S INSTALLED:$\r$\n"
  FileWrite $9 "• Native Outlook plugin with ribbon integration$\r$\n"
  FileWrite $9 "• Real-time document tracking capabilities$\r$\n"
  FileWrite $9 "• Professional document attachment features$\r$\n$\r$\n"
  FileWrite $9 "HOW TO USE:$\r$\n"
  FileWrite $9 "1. Restart Microsoft Outlook if it's currently running$\r$\n"
  FileWrite $9 "2. Look for the 'effyDOC' section in the Outlook ribbon$\r$\n"
  FileWrite $9 "3. Click 'Settings' to sign in with your effyDOC account$\r$\n"
  FileWrite $9 "4. Use 'Attach Document' to send trackable documents$\r$\n"
  FileWrite $9 "5. Use 'View Analytics' to see engagement metrics$\r$\n$\r$\n"
  FileWrite $9 "FEATURES:$\r$\n"
  FileWrite $9 "• Attach Document: Add trackable documents to emails$\r$\n"
  FileWrite $9 "• View Analytics: Real-time engagement tracking$\r$\n"
  FileWrite $9 "• New Proposal: Create AI-powered proposals$\r$\n"
  FileWrite $9 "• New Contract: Generate professional contracts$\r$\n$\r$\n"
  FileWrite $9 "TROUBLESHOOTING:$\r$\n"
  FileWrite $9 "• If the plugin doesn't appear: Restart Outlook completely$\r$\n"
  FileWrite $9 "• Check that you have a valid effyDOC account$\r$\n"
  FileWrite $9 "• Ensure you're connected to the internet$\r$\n"
  FileWrite $9 "• Contact support if issues persist$\r$\n$\r$\n"
  FileWrite $9 "SUPPORT & HELP:$\r$\n"
  FileWrite $9 "• Website: ${PRODUCT_WEB_SITE}$\r$\n"
  FileWrite $9 "• Documentation: ${PRODUCT_WEB_SITE}/docs$\r$\n"
  FileWrite $9 "• Support: ${PRODUCT_WEB_SITE}/support$\r$\n$\r$\n"
  FileWrite $9 "VERSION: ${PRODUCT_VERSION}$\r$\n"
  FileWrite $9 "INSTALLED: $$(Date)$\r$\n"
  FileClose $9
  
  # Register VSTO add-in
  DetailPrint "Registering Outlook add-in..."
  
  # Create registry entries for VSTO add-in
  WriteRegStr HKCU "SOFTWARE\Microsoft\Office\Outlook\Addins\EffyDocOutlookPlugin" "Description" "effyDOC document tracking and attachment plugin for Outlook"
  WriteRegStr HKCU "SOFTWARE\Microsoft\Office\Outlook\Addins\EffyDocOutlookPlugin" "FriendlyName" "effyDOC Outlook Plugin"
  WriteRegDWORD HKCU "SOFTWARE\Microsoft\Office\Outlook\Addins\EffyDocOutlookPlugin" "LoadBehavior" 0x00000003
  WriteRegStr HKCU "SOFTWARE\Microsoft\Office\Outlook\Addins\EffyDocOutlookPlugin" "Manifest" "$INSTDIR\EffyDocOutlookPlugin.vsto|vstolocal"
  
  # Registry entries for uninstaller
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "DisplayName" "$(^Name)"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "UninstallString" "$INSTDIR\uninst.exe"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "DisplayIcon" "$INSTDIR\EffyDocOutlookPlugin.dll"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "DisplayVersion" "${PRODUCT_VERSION}"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "URLInfoAbout" "${PRODUCT_WEB_SITE}"
  WriteRegStr HKCU "${PRODUCT_UNINST_KEY}" "Publisher" "${PRODUCT_PUBLISHER}"
  WriteRegDWORD HKCU "${PRODUCT_UNINST_KEY}" "NoModify" 1
  WriteRegDWORD HKCU "${PRODUCT_UNINST_KEY}" "NoRepair" 1
  WriteRegDWORD HKCU "${PRODUCT_UNINST_KEY}" "EstimatedSize" 2048
  
  # Create uninstaller
  WriteUninstaller "$INSTDIR\uninst.exe"
  
  DetailPrint "Installation completed successfully!"
SectionEnd

# Optional desktop shortcut
Section "Desktop Shortcut" SEC02
  CreateShortCut "$DESKTOP\effyDOC Outlook Plugin.lnk" "$INSTDIR\README.txt" "" "$INSTDIR\EffyDocOutlookPlugin.dll" 0
SectionEnd

# Optional start menu shortcuts
Section "Start Menu Shortcuts" SEC03
  CreateDirectory "$SMPROGRAMS\effyDOC"
  CreateShortCut "$SMPROGRAMS\effyDOC\effyDOC Outlook Plugin.lnk" "$INSTDIR\README.txt" "" "$INSTDIR\EffyDocOutlookPlugin.dll" 0
  CreateShortCut "$SMPROGRAMS\effyDOC\Uninstall.lnk" "$INSTDIR\uninst.exe"
  CreateShortCut "$SMPROGRAMS\effyDOC\effyDOC Website.lnk" "${PRODUCT_WEB_SITE}"
SectionEnd

# Section descriptions
!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC01} "Installs the core effyDOC Outlook Plugin files and registers them with Microsoft Outlook."
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC02} "Creates a desktop shortcut for easy access to installation information."
  !insertmacro MUI_DESCRIPTION_TEXT ${SEC03} "Creates Start Menu shortcuts for the plugin and uninstaller."
!insertmacro MUI_FUNCTION_DESCRIPTION_END

# Functions
Function LaunchOutlook
  # Close any existing Outlook processes gracefully
  FindWindow $R0 "rctrl_renwnd32" ""
  ${If} $R0 != 0
    SendMessage $R0 ${WM_CLOSE} 0 0
    Sleep 2000
  ${EndIf}
  
  # Launch Outlook
  ReadRegStr $R1 HKLM "SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE" ""
  ${If} $R1 != ""
    ExecShell "" "$R1"
  ${Else}
    ExecShell "" "outlook.exe"
  ${EndIf}
FunctionEnd

Function un.onUninstSuccess
  HideWindow
  MessageBox MB_ICONINFORMATION|MB_OK "$(^Name) was successfully removed from your computer.$\r$\n$\r$\nPlease restart Outlook to complete the removal."
FunctionEnd

Function un.onInit
  MessageBox MB_ICONQUESTION|MB_YESNO|MB_DEFBUTTON2 "Are you sure you want to completely remove $(^Name) and all of its components?$\r$\n$\r$\nThis will remove the plugin from Outlook." IDYES +2
  Abort
FunctionEnd

# Uninstaller
Section Uninstall
  # Remove registry entries
  DeleteRegKey HKCU "SOFTWARE\Microsoft\Office\Outlook\Addins\EffyDocOutlookPlugin"
  DeleteRegKey HKCU "${PRODUCT_UNINST_KEY}"
  DeleteRegKey HKCU "${PRODUCT_DIR_REGKEY}"
  
  # Remove files
  Delete "$INSTDIR\EffyDocOutlookPlugin.dll"
  Delete "$INSTDIR\EffyDocOutlookPlugin.dll.manifest"
  Delete "$INSTDIR\EffyDocOutlookPlugin.vsto"
  Delete "$INSTDIR\Newtonsoft.Json.dll"
  Delete "$INSTDIR\README.txt"
  Delete "$INSTDIR\uninst.exe"
  
  # Remove shortcuts
  Delete "$DESKTOP\effyDOC Outlook Plugin.lnk"
  Delete "$SMPROGRAMS\effyDOC\effyDOC Outlook Plugin.lnk"
  Delete "$SMPROGRAMS\effyDOC\Uninstall.lnk"
  Delete "$SMPROGRAMS\effyDOC\effyDOC Website.lnk"
  RMDir "$SMPROGRAMS\effyDOC"
  
  # Remove directories
  RMDir "$INSTDIR"
  
  SetAutoClose true
SectionEnd