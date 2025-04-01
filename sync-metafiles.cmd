@ECHO OFF
SETLOCAL ENABLEDELAYEDEXPANSION

ECHO ^>^> Navigating to the top-level directory of the repository...
CD /d "%~dp0"

ECHO ^>^> Opening and closing Unity project to sync "*.meta" files...
"%UNITY_EXE%" -batchmode -quit -projectPath "%CD%"

REM Check if the task succeeded
IF %ERRORLEVEL% NEQ 0 (
    ECHO ^>^> Unity open failed with error code: %ERRORLEVEL%
    PAUSE
    EXIT /b %ERRORLEVEL%
)

ECHO ^>^> Unity synchronization completed successfully^^!
PAUSE
EXIT /b 0
