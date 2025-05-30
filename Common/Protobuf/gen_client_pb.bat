@echo off
REM 将目标目标切换到本文件所在目录
cd /d %~dp0
setlocal enabledelayedexpansion

set "Dir_Root=%~dp0"

REM proto文件所在目录
set "Dir_ProtoInput=%Dir_Root%proto"

REM proto生成C#文件的输出目录
set "Dir_CS_Output=%Dir_Root%../../Client/Assets/Scripts/Hotfix/Module/Network/Message"

REM 确保输出目录存在
if not exist "!Dir_CS_Output!" mkdir "!Dir_CS_Output!"

REM 检查protoc.exe是否存在
set "ProtocPath=protoc-31.1-win64/bin/protoc.exe"
if not exist "!ProtocPath!" (
    echo 错误: 找不到protoc.exe，请检查路径 "!ProtocPath!"
    goto :error
)

echo current dir: %~dp0
echo output dir: !Dir_CS_Output!
echo proto dir: !Dir_ProtoInput!\*.proto

REM 执行protoc.exe，并输出cs文件到目标目录
"!ProtocPath!" -I "!Dir_ProtoInput!" --csharp_out="!Dir_CS_Output!" "!Dir_ProtoInput!\*.proto"

if %errorlevel% neq 0 (
    echo error: protoc execute failure!
    goto :error
)

echo build proto successful!
goto :success

:error
echo build error
pause
exit /b 1

:success
pause
exit /b 0