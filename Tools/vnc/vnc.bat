@echo off
set var=%1
set ip=%var:~6,-1%
start "VNCViewer" "C:\utils\vnc\vnc.exe" %ip% -WarnUnencrypted=0