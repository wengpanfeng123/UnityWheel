@echo off
setlocal

:: 检查 Unity 进程是否存在
tasklist /FI "IMAGENAME eq Unity.exe" 2>NUL | find /I "Unity.exe" >NUL

:: 如果找到进程则终止
if "%ERRORLEVEL%"=="0" (
    echo Unity is running, killing process...
    taskkill /F /IM "Unity.exe" >NUL
    timeout /T 2 /NOBREAK >NUL
    echo Unity process terminated.
) else (
    echo Unity is not running.
)

endlocal