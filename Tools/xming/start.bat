@echo off
setlocal enabledelayedexpansion
set var=%1
set ip=%var:~6,-1%
start "Xming" /b "C:\utils\xming\xming.exe" :0 -dpi 85 -clipboard -trayicon -c -multiwindow -reset -terminate -unixkill
start "Plink" /b "C:\utils\xming\plink.exe" -X -ssh %ip% -l newcontact -pw 578 pavucontrol