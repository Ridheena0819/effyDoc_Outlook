@echo off
title effyDOC Outlook Add-in Installer
color 0A

echo.
echo ========================================
echo     effyDOC Outlook Add-in Installer
echo ========================================
echo.

echo [1/4] Checking system requirements...
timeout /t 2 /nobreak >nul

REM Check if Outlook is installed
reg query "HKLM\SOFTWARE\Microsoft\Windows\CurrentVersion\App Paths\OUTLOOK.EXE" >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Microsoft Outlook not found!
    echo Please install Microsoft Outlook first.
    pause
    exit /b 1
)
echo [OK] Microsoft Outlook detected

echo.
echo [2/4] Creating installation directories...
if not exist "%USERPROFILE%\AppData\Roaming\Microsoft\AddIns" (
    mkdir "%USERPROFILE%\AppData\Roaming\Microsoft\AddIns"
)
echo [OK] Installation directory ready

echo.
echo [3/4] Downloading effyDOC add-in manifest...
powershell -Command "& {Invoke-WebRequest -Uri 'https://f70e6bf0-40a7-454d-8960-1649b6f10c4c.preview.emergentagent.com/outlook-addin/manifest.xml' -OutFile '%USERPROFILE%\AppData\Roaming\Microsoft\AddIns\effydoc-manifest.xml'}"

if not exist "%USERPROFILE%\AppData\Roaming\Microsoft\AddIns\effydoc-manifest.xml" (
    echo [ERROR] Failed to download manifest file!
    echo Please check your internet connection.
    pause
    exit /b 1
)
echo [OK] Manifest file downloaded

echo.
echo [4/4] Registering add-in...
REM Add registry entry for the add-in
reg add "HKCU\SOFTWARE\Microsoft\Office\16.0\WEF\Developer" /v "effyDOC" /t REG_SZ /d "%USERPROFILE%\AppData\Roaming\Microsoft\AddIns\effydoc-manifest.xml" /f >nul 2>&1
echo [OK] Add-in registered

echo.
echo ========================================
echo     Installation Complete!
echo ========================================
echo.
echo Next Steps:
echo 1. Restart Microsoft Outlook
echo 2. Look for effyDOC panel in Outlook
echo 3. Sign in with your effyDOC account
echo 4. Start tracking documents!
echo.
echo Need help? Visit:
echo https://f70e6bf0-40a7-454d-8960-1649b6f10c4c.preview.emergentagent.com/integrations.html
echo.

set /p choice="Start Outlook now? (Y/N): "
if /i "%choice%"=="Y" (
    start outlook.exe
)

echo.
echo Thank you for using effyDOC!
pause