#!/bin/bash

# 配置参数
WORKSPACE=$(dirname "$0")  # 自动获取脚本所在目录
LUBAN_DLL="$WORKSPACE/Tools/Luban/Luban.dll"
CONF_ROOT="$WORKSPACE/DataTables"
LOG_FILE="$WORKSPACE/luban.log"

CODE_DIR="$(realpath "$WORKSPACE/../../Client/Assets/Scripts/Hotfix/TableScript")"
DATA_DIR="$(realpath "$WORKSPACE/../../Client/Assets/AssetsPackage/DataTable")"

echo "代码输出目录: $CODE_DIR"
echo "数据输出目录: $DATA_DIR"
# 确保目录存在且有写权限
mkdir -p "$CODE_DIR" && chmod u+w "$CODE_DIR"
mkdir -p "$DATA_DIR" && chmod u+w "$DATA_DIR"

# 验证目录所有权
if [ ! -w "$CODE_DIR" ]; then
    echo "错误：无写权限: $CODE_DIR"
    sudo chown -R $USER "$CODE_DIR"
fi

if [ ! -w "$DATA_DIR" ]; then
    echo "错误：无写权限: $DATA_DIR"
    sudo chown -R $USER "$DATA_DIR"
fi



# 检查依赖
if ! command -v dotnet &> /dev/null; then
    echo "错误：未找到 .NET SDK，请先安装"
    exit 1
fi

# 检查 Luban DLL 是否存在
if [ ! -f "$LUBAN_DLL" ]; then
    echo "错误：找不到 Luban.dll ($LUBAN_DLL)"
    exit 1
fi

# 记录开始时间
start_time=$(date +%s)
echo "===== Luban 生成开始 $(date) =====" | tee "$LOG_FILE"

# 运行 Luban 工具
dotnet "$LUBAN_DLL" \
    -t all \
    -d json \
    -c cs-simple-json \
    --conf "$CONF_ROOT/luban.conf" \
    -x outputCodeDir="$CODE_DIR" \
    -x outputDataDir="$DATA_DIR"

# 检查执行结果
exit_code=${PIPESTATUS[0]}
if [ $exit_code -ne 0 ]; then
    echo "错误：Luban 执行失败 (退出码: $exit_code)" | tee -a "$LOG_FILE"
    exit $exit_code
fi

# 计算耗时
end_time=$(date +%s)
duration=$((end_time - start_time))
echo "===== 生成完成! 耗时: ${duration}秒 =====" | tee -a "$LOG_FILE"

# 显示日志路径
echo "详细日志已保存至: $LOG_FILE"

# 可选：自动打开日志文件
# if command -v xdg-open &> /dev/null; then
#     xdg-open "$LOG_FILE"
# elif command -v open &> /dev/null; then
#     open "$LOG_FILE"
# fi