@echo off
setlocal
set var=%1
set ip=%var:~7,-1%
start "Plink" "C:\utils\ssh\putty.exe" -ssh %ip% -l root