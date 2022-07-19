@echo off
set var=%1
set ip=%var:~6,-1%
start "Explorer DShare" explorer.exe \\%ip%\d$