@echo off
set var=%1
set ip=%var:~6,-1%
start "Explorer CShare" explorer.exe \\%ip%\c$