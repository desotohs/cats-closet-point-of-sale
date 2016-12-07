@echo off
title Cat's Closet Point of Sale Server

net session 1>NUL 2>NUL
if "%errorlevel%" == "0" (
	cd %~dp0
	cats-closet-point-of-sale launch
	pause
) else (
	powershell "saps -filepath %0 -verb runas"
)
exit /b
