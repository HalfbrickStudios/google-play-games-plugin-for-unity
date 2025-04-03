@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO ^>^> Navigating to the top-level directory of the repository...
CD /d "%~dp0"

:ask_confirmation
SET /P USER_INPUT=^>^> Have you executed the "build-tarball.cmd" script to ensure the latest changes have been compiled? (y)es/no =^> 
IF /I "!USER_INPUT!" == "y" (
    ECHO ^>^> Proceeding with the prepare process...
) ELSE IF /I "!USER_INPUT!" == "yes" (
    ECHO ^>^> Proceeding with the prepare process...
) ELSE IF /I "!USER_INPUT!" == "no" (
    ECHO ^>^> Stopping prepare process; executing "build-tarball.cmd" to ensure the latest changes have been compiled...
    CALL build-tarball.cmd
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

REM Prepare to extract the compressed tarball
SET "OUTPUT_DIR=current-build"
SET "PACKAGE_FILENAME=%PACKAGE_ID%.tgz"
SET "PACKAGE_PATH=%OUTPUT_DIR%\%PACKAGE_FILENAME%"

ECHO ^>^> Extracting compressed tarball "%PACKAGE_PATH%" at "%OUTPUT_DIR%"
7z x "%CD%\%PACKAGE_PATH%" "-o%CD%\%OUTPUT_DIR%" -y -sns

REM Check if the extraction was successful
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> Compressed tarball extraction failed with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

REM Prepare to extract the tarball
SET "PACKAGE_FILENAME=%PACKAGE_ID%.tar"
SET "PACKAGE_PATH=%OUTPUT_DIR%\%PACKAGE_FILENAME%"

ECHO ^>^> Extracting uncompressed tarball: %PACKAGE_PATH%
7z x "%CD%\%PACKAGE_PATH%" "-o%CD%\%OUTPUT_DIR%" -y -sns

REM Check if the extraction was successful
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> Uncompressed tarball extraction failed with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

REM Prepare to rename the package folder
SET "PACKAGE_PATH=%OUTPUT_DIR%\%PACKAGE_ID%"

ECHO ^>^> Renaming extracted folder to "%PACKAGE_ID%"
MOVE %OUTPUT_DIR%/package %PACKAGE_PATH%

REM Prepare to rename the package folder
SET "PACKAGE_DEPENDENCIES=%PACKAGE_PATH%\Editor\GooglePlayGamesPluginDependencies.xml"

ECHO ^>^> Patching EDM4U XML specification file...
powershell -NoProfile -Command "(Get-Content -LiteralPath '%PACKAGE_DEPENDENCIES%') -replace 'Assets/GooglePlayGames/com.google.play.games', 'Packages/com.google.play.games' | Set-Content -LiteralPath '%PACKAGE_DEPENDENCIES%.new'"

REM Check if the patch was successful
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> Failed to patch the EDM4U XML specification file with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

ECHO ^>^> Overwriting EDM4U XML specification file...
MOVE /Y "%PACKAGE_DEPENDENCIES%.new" "%PACKAGE_DEPENDENCIES%"

REM Check if the overwrite was successful
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> Failed to overwrite the EDM4U XML specification file with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

ECHO ^>^> NPM prepare completed successfully^^!
:end_registry
PAUSE
