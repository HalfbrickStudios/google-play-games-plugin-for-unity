@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO ^>^> Navigating to the top-level directory of the repository...
CD /d "%~dp0"

:ask_confirmation
SET /P USER_INPUT=^>^> Have you executed the "prepare-package.cmd" script to ensure the package files have been patched? (y)es/no =^> 
IF /I "!USER_INPUT!" == "y" (
    ECHO ^>^> Proceeding with the publish process...
) ELSE IF /I "!USER_INPUT!" == "yes" (
    ECHO ^>^> Proceeding with the publish process...
) ELSE IF /I "!USER_INPUT!" == "no" (
    ECHO ^>^> Stopping publish process; executing "prepare-package.cmd" to ensure the package has been patched...
    CALL prepare-package.cmd
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

REM Prepare to publish the package
SET "OUTPUT_DIR=current-build"
SET "PACKAGE_PATH=%OUTPUT_DIR%\%PACKAGE_ID%"

ECHO ^>^> Entering the package folder: %PACKAGE_PATH%
IF EXIST "%PACKAGE_PATH%" (
    ECHO ^>^> Package folder found: %PACKAGE_PATH%
    CD /d "%PACKAGE_PATH%"
) ELSE (
    ECHO ^>^> Package folder not found: %PACKAGE_PATH%
    PAUSE
    EXIT /b 1
)

:ask_registry
SET /P USER_INPUT=^>^> Enter the registry you want to publish the package to: 
IF /I "!USER_INPUT!" == "" (
    ECHO ^>^> Not publishing to any registry
    GOTO end_registry
)

ECHO ^>^> Publishing package "%PACKAGE_ID%" registry "!USER_INPUT!"
CMD /c npm publish --registry "!USER_INPUT!"

REM Check if the extraction was successful
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> NPM publish failed with error code: %ERRORLEVEL%
    GOTO ask_registry
)

ECHO ^>^> NPM publish completed successfully^^!
:end_registry
PAUSE
