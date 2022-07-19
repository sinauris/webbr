@echo off
set var=%1
set ip=%var:~6,-1%
start "PsShutdown" "C:\utils\remote\psshutdown.exe" \\%ip% -s -t 0