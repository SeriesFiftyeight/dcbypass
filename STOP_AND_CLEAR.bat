@echo off
:: DC Bypass - Stop and Clear Utility
chcp 65001 >nul
cd /d "%~dp0"

echo ===================================================
echo   Stopping DC Bypass and Clearing Network Cache...
echo ===================================================

taskkill /f /im dcbypass.exe 2>nul
taskkill /f /im dcbaypass.exe 2>nul
taskkill /f /im DiscordBypass.exe 2>nul
taskkill /f /im goodbyedpi.exe 2>nul
sc stop GoodbyeDPI 2>nul
sc stop WinDivert 2>nul
sc stop WinDivert14 2>nul

echo.
echo [1/2] Flushing DNS Cache...
ipconfig /flushdns >nul

echo [2/2] All services and drivers have been stopped successfully.
echo.
echo ===================================================
echo   Process Complete!
echo ===================================================
timeout /t 3
