@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO ^>^> Navigating to the top-level directory of the repository
CD /d "%~dp0"

ECHO ^>^> Extracting version from package.json file
SET "PACKAGE_NAME=com.google.play.games"
SET "PACKAGE_JSON=Assets\Public\GooglePlayGames\%PACKAGE_NAME%\package.json"
FOR /f "delims=" %%i IN ('powershell -NoProfile -Command "(Get-Content -LiteralPath '%PACKAGE_JSON%' | ConvertFrom-Json).version"') DO SET "PACKAGE_VERSION=%%i"

REM Check if the version was extracted successfully
IF DEFINED PACKAGE_VERSION (
    SET "PACKAGE_ID=%PACKAGE_NAME%-%PACKAGE_VERSION%"
    ECHO ^>^> Building package: !PACKAGE_ID!
) ELSE (
    ECHO ^>^> Failed to extract version from: %PACKAGE_JSON%
    PAUSE
    EXIT /b 1
)

ECHO ^>^> Running Gradle task "export_plugin_tgz"
CMD /c gradlew.bat export_plugin_tgz

REM Check if the task succeeded
IF %ERRORLEVEL% equ 0 (
    ECHO ^>^> Gradle task "export_plugin_tgz" completed successfully
) ELSE (
    ECHO ^>^> Gradle task "export_plugin_tgz" failed with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

REM Prepare to copy the compressed tarball to the current-build directory
SET "BUILD_DIR=build"
SET "PACKAGE_FILENAME=%PACKAGE_ID%.tgz"
SET "PACKAGE_PATH=%BUILD_DIR%\%PACKAGE_FILENAME%"
SET "OUTPUT_DIR=current-build"

REM Copy the compressed tarball to the current-build directory
IF EXIST "%PACKAGE_PATH%" (
    COPY "%PACKAGE_PATH%" "%OUTPUT_DIR%\"
    ECHO ^>^> Package copied to: %OUTPUT_DIR%
) ELSE (
    ECHO ^>^> Package not found: %PACKAGE_PATH%
    PAUSE
    EXIT /b 1
)

PAUSE
