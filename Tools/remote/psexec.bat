@echo off
set var=%1
set ip=%var:~6,-1%
start "PsExec" "C:\utils\remote\psexec.exe" \\%ip% cmd