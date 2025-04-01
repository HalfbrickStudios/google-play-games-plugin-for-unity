@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO ^>^> Navigating to the top-level directory of the repository
CD /d "%~dp0"

ECHO ^>^> Opening Unity project as is
"%UNITY_EXE%" -batchmode -quit -projectPath "%CD%"

REM Check if the task succeeded
IF %ERRORLEVEL% equ 0 (
    ECHO ^>^> Unity open completed successfully
) ELSE (
    ECHO ^>^> Unity open failed with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

PAUSE
