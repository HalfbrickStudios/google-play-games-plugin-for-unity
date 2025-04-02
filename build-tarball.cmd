@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO ^>^> Navigating to the top-level directory of the repository...
CD /d "%~dp0"

:ask_confirmation
SET /P USER_INPUT=^>^> Have you executed the "sync-metafiles.cmd" script to ensure the "*.meta" files are up-to-date? (y)es/no =^> 
IF /I "!USER_INPUT!" == "y" (
    ECHO ^>^> Proceeding with the building process...
) ELSE IF /I "!USER_INPUT!" == "yes" (
    ECHO ^>^> Proceeding with the building process...
) ELSE IF /I "!USER_INPUT!" == "no" (
    ECHO ^>^> Stopping building process; executing "sync-metafiles.cmd" to ensure the "*.meta" files are up-to-date...
    CALL sync-metafiles.cmd
    EXIT /B 1
) ELSE (
    ECHO ^>^> Invalid input. Please type "yes" or "no".
    GOTO ask_confirmation
)

ECHO ^>^> Extracting version from "package.json" file...
SET "PACKAGE_NAME=com.google.play.games"
SET "PACKAGE_JSON=Assets\Public\GooglePlayGames\%PACKAGE_NAME%\package.json"
FOR /f "delims=" %%i IN ('powershell -NoProfile -Command "(Get-Content -LiteralPath '%PACKAGE_JSON%' | ConvertFrom-Json).version"') DO SET "PACKAGE_VERSION=%%i"

REM Check if the version was extracted successfully
IF DEFINED PACKAGE_VERSION (
    SET "PACKAGE_ID=%PACKAGE_NAME%-%PACKAGE_VERSION%"
    ECHO ^>^> Computed package identifier: !PACKAGE_ID!
) ELSE (
    ECHO ^>^> Failed to extract version from: %PACKAGE_JSON%
    PAUSE
    EXIT /b 1
)

ECHO ^>^> Running Gradle task "export_plugin_tgz" using the wrapper script...
CMD /c gradlew.bat export_plugin_tgz --stacktrace

REM Check if the task succeeded
IF %ERRORLEVEL% NEQ 0 (
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
IF NOT EXIST "%PACKAGE_PATH%" (
    ECHO ^>^> Compressed tarball not found at "%PACKAGE_PATH%"
    PAUSE
    EXIT /b 1
)

ECHO ^>^> Copying compressed tarball "%PACKAGE_PATH%" to "%OUTPUT_DIR%"
COPY "%PACKAGE_PATH%" "%OUTPUT_DIR%\"

REM Check if the task succeeded
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> Copy of "%PACKAGE_PATH%" failed with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

PAUSE
EXIT /b 0
