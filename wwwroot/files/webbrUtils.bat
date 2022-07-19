@chcp 65001
@echo off
rd /s /q %SYSTEMDRIVE%\webbr\utils
robocopy S:\Department\СТП\webbr\utils %SYSTEMDRIVE%\webbr\utils /E /MT:32
regedit /s %SYSTEMDRIVE%\webbr\utils\handlers.reg
DEL /F /S /Q /A "%SYSTEMDRIVE%\webbr\utils\handlers.reg"
cls
echo Утилиты установлены, изменения в реестр внесены!
pause
