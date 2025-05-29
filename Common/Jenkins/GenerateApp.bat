@echo off
setlocal enabledelayedexpansion

:: 检查参数是否传递
::if "%~1"=="" (
::    echo 错误：缺少参数 productName
::    exit /b 1
::)

::if "%~2"=="" (
::    echo 错误：缺少参数 version
::    exit /b 1
::)

:: 管理员权限检查（可选）
:: NET SESSION >nul 2>&1 || (
::     echo 请以管理员身份运行此脚本！
::     pause
::     exit /b 1
:: )

:: 终止 Unity 进程
:CHECK_UNITY
tasklist /FI "IMAGENAME eq Unity.exe" | find /I "Unity.exe" >nul
if %ERRORLEVEL% equ 0 (
    echo 检测到正在运行的 Unity 进程，正在终止...
    taskkill /F /IM "Unity.exe" >nul
    timeout /T 1 /NOBREAK >nul
    goto CHECK_UNITY
)

:: 启动 Unity 打包
echo 正在启动 Unity 打包...
"C:\Program Files\Unity\Hub\Editor\2022.3.28f1c1\Editor\Unity.exe" ^
  -quit ^
  -batchmode ^
  -projectPath "D:\workspace\UnityProjects\MyDemo\Client" ^
  -executeMethod BuildTools.BuildApk ^
  -logFile "D:\workspace\UnityProjects\MyDemo\Client\jenkins_output.log" ^
  --productName="%~1" ^
  --version="%~2"

echo 命令执行完成

endlocal