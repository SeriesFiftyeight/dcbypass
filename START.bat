@echo off
:: DC Bypass Pro - Admin Launcher
chcp 65001 >nul
cd /d "%~dp0"

:: Check Admin Privileges
net session >nul 2>&1
if %errorlevel% neq 0 (
    echo [INFO] Requesting Administrator privileges...
    powershell -Command "Start-Process '%~dp0dist\dcbypass.exe' -Verb RunAs"
    exit /b
)

if exist "%~dp0dist\dcbypass.exe" (
    start "" "%~dp0dist\dcbypass.exe"
) else (
    dotnet run
)
