@echo off
:: DC Bypass Pro - Admin Launcher
chcp 65001 >nul
cd /d "%~dp0"

:: Resolve executable path
set "EXE_PATH=%~dp0bin\dcbypass.exe"
if not exist "%EXE_PATH%" set "EXE_PATH=%~dp0dcbypass.exe"
if not exist "%EXE_PATH%" set "EXE_PATH=%~dp0dist\dcbypass.exe"

:: Check Admin Privileges
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [INFO] Requesting Administrator privileges...
    powershell -Command "Start-Process '%EXE_PATH%' -Verb RunAs"
    exit /b
)

if exist "%EXE_PATH%" (
    start "" "%EXE_PATH%"
) else (
    dotnet run
)
