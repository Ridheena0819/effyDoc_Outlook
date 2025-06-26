@echo off
echo ===================================
echo   effyDOC Outlook VSTO Add-in
echo   Build Script
echo ===================================
echo.

:: Set variables
set PROJECT_NAME=EffyDocOutlookAddin
set SOLUTION_FILE=%PROJECT_NAME%.csproj
set BUILD_CONFIG=Release
set OUTPUT_DIR=bin\%BUILD_CONFIG%
set PUBLISH_DIR=publish

:: Check if Visual Studio Build Tools are available
where msbuild >nul 2>nul
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: MSBuild not found. Please install Visual Studio Build Tools.
    echo Download from: https://visualstudio.microsoft.com/downloads/#build-tools-for-visual-studio-2022
    pause
    exit /b 1
)

echo [1/5] Cleaning previous build...
if exist "%OUTPUT_DIR%" rmdir /s /q "%OUTPUT_DIR%"
if exist "%PUBLISH_DIR%" rmdir /s /q "%PUBLISH_DIR%"

echo [2/5] Restoring NuGet packages...
nuget restore %SOLUTION_FILE%
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Failed to restore NuGet packages
    pause
    exit /b 1
)

echo [3/5] Building the solution...
msbuild %SOLUTION_FILE% /p:Configuration=%BUILD_CONFIG% /p:Platform="Any CPU" /verbosity:minimal
if %ERRORLEVEL% NEQ 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo [4/5] Creating deployment package...
mkdir "%PUBLISH_DIR%"
xcopy "%OUTPUT_DIR%\*" "%PUBLISH_DIR%\" /E /Y /Q

echo [5/5] Generating installer manifest...
echo ^<?xml version="1.0" encoding="utf-8"?^> > "%PUBLISH_DIR%\%PROJECT_NAME%.vsto"
echo ^<vstav3:addIn xmlns:vstav3="urn:schemas-microsoft-com:vsta.v3"^> >> "%PUBLISH_DIR%\%PROJECT_NAME%.vsto"
echo   ^<vstav3:addInId^>%PROJECT_NAME%^</vstav3:addInId^> >> "%PUBLISH_DIR%\%PROJECT_NAME%.vsto"
echo   ^<vstav3:friendlyName^>effyDOC Outlook Add-in^</vstav3:friendlyName^> >> "%PUBLISH_DIR%\%PROJECT_NAME%.vsto"
echo   ^<vstav3:description^>Track document engagement in real-time directly from Outlook^</vstav3:description^> >> "%PUBLISH_DIR%\%PROJECT_NAME%.vsto"
echo   ^<vstav3:formRegions /^> >> "%PUBLISH_DIR%\%PROJECT_NAME%.vsto"
echo   ^<vstav3:requestedExecutionLevel level="asInvoker" /^> >> "%PUBLISH_DIR%\%PROJECT_NAME%.vsto"
echo ^</vstav3:addIn^> >> "%PUBLISH_DIR%\%PROJECT_NAME%.vsto"

echo.
echo ===================================
echo   BUILD COMPLETED SUCCESSFULLY!
echo ===================================
echo.
echo Build artifacts are located in: %PUBLISH_DIR%\
echo.
echo To install the add-in:
echo 1. Copy the contents of '%PUBLISH_DIR%' to your target machine
echo 2. Run '%PROJECT_NAME%.exe' as Administrator
echo 3. Restart Microsoft Outlook
echo.
echo For manual installation:
echo 1. Register the add-in using the provided registry entries
echo 2. Ensure VSTO Runtime 2010 or later is installed
echo 3. Configure the backend URL in App.config
echo.
pause