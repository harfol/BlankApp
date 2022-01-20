::添加环境变量 BAT_HOME
@echo off
echo 添加 bat 环境变量
set regpath=HKEY_LOCAL_MACHINE\SYSTEM\CurrentControlSet\Control\Session Manager\Environment
set evname=BAT_HOME
reg add "%regpath%" /v %evname% /d %cd% /f
regedit /s setup.reg
pause>nul