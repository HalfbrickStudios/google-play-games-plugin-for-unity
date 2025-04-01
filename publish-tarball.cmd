@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO ^>^> Navigating to the top-level directory of the repository
CD /d "%~dp0"

:ask_confirmation
SET /P USER_INPUT=^>^> Have you executed the "build-tarball.cmd" script to ensure the latest changes have been compiled? (yes/no) 
IF /I "!USER_INPUT!" == "yes" (
    ECHO ^>^> Proceeding with the publish process...
) ELSE IF /I "!USER_INPUT!" == "no" (
    ECHO ^>^> Executing build-tarball.cmd to ensure the latest changes have been compiled.
    CALL build-tarball.cmd
    EXIT /B 1
) ELSE (
    ECHO ^>^> Invalid input. Please type "yes" or "no".
    GOTO ask_confirmation
)

ECHO ^>^> Extracting version from package.json file
SET "PACKAGE_NAME=com.google.play.games"
SET "PACKAGE_JSON=Assets\Public\GooglePlayGames\%PACKAGE_NAME%\package.json"
FOR /f "delims=" %%i IN ('powershell -NoProfile -Command "(Get-Content -LiteralPath '%PACKAGE_JSON%' | ConvertFrom-Json).version"') DO SET "PACKAGE_VERSION=%%i"

REM Check if the version was extracted successfully
IF DEFINED PACKAGE_VERSION (
    SET "PACKAGE_ID=%PACKAGE_NAME%-%PACKAGE_VERSION%"
    ECHO ^>^> Publishing package: !PACKAGE_ID!
) ELSE (
    ECHO ^>^> Failed to extract version from: %PACKAGE_JSON%
    PAUSE
    EXIT /b 1
)

REM Prepare to extract the compressed tarball
SET "OUTPUT_DIR=current-build"
SET "PACKAGE_FILENAME=%PACKAGE_ID%.tgz"
SET "PACKAGE_PATH=%OUTPUT_DIR%\%PACKAGE_FILENAME%"

ECHO ^>^> Extracting compressed tarball: %PACKAGE_PATH%
7z x "%CD%\%PACKAGE_PATH%" "-o%CD%\%OUTPUT_DIR%" -y

REM Check if the extraction was successful
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> Extraction failed with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

REM Prepare to extract the tarball
SET "PACKAGE_FILENAME=%PACKAGE_ID%.tar"
SET "PACKAGE_PATH=%OUTPUT_DIR%\%PACKAGE_FILENAME%"

ECHO ^>^> Extracting tarball: %PACKAGE_PATH%
7z x "%CD%\%PACKAGE_PATH%" "-o%CD%\%OUTPUT_DIR%" -y

REM Check if the extraction was successful
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> Extraction failed with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

ECHO ^>^> Renaming extracted folder to: %PACKAGE_ID%
MOVE %OUTPUT_DIR%/package %OUTPUT_DIR%/%PACKAGE_ID%

SET /P USER_INPUT=^>^> Enter the registry you want to publish the package to: 
IF /I "!USER_INPUT!" == "" (
    ECHO ^>^> Not publishing to any registry
) ELSE (
    ECHO ^>^> Publishing to registry: !USER_INPUT!
    CD /d "%OUTPUT_DIR%\%PACKAGE_ID%"
    CMD /c npm publish --registry "!USER_INPUT!"
)

PAUSE
