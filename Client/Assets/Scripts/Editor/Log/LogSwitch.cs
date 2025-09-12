using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Xicheng.EditorLog
{
    public static class LogSwitch
    {
        static readonly string logMacro = "ENABLE_LOG";

        [MenuItem("Tools/开启日志宏")]
        public static void EnableLog()
        {
            // 1. 获取当前选中的“构建目标组”（如 Android、iOS、PC 等）
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;

            // 2. 获取该组当前的宏定义（分号分隔的字符串 → 转为列表）
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var symbolsList = currentSymbols.Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s)) // 过滤空字符串
                .ToList();

            // 4. 若宏不存在，则添加
            if (!symbolsList.Contains(logMacro))
            {
                symbolsList.Add(logMacro);
                // 5. 拼接为分号分隔的字符串，重新设置
                string newSymbols = string.Join(";", symbolsList);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newSymbols);
                Debug.Log($"已添加宏：{logMacro}，当前组宏列表：{newSymbols}");
            }
            else
            {
                Debug.LogWarning($"宏 {logMacro} 已存在，无需重复添加。");
            }
        }

        [MenuItem("Tools/关闭日志宏")]
        public static void DisableLog()
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);
            var symbolsList = currentSymbols.Split(';')
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();

            // 若宏存在，则移除
            if (symbolsList.Contains(logMacro))
            {
                symbolsList.Remove(logMacro);
                string newSymbols = string.Join(";", symbolsList);
                PlayerSettings.SetScriptingDefineSymbolsForGroup(targetGroup, newSymbols);
                Debug.Log($"已删除宏：{logMacro}，当前组宏列表：{newSymbols}");
            }
            else
            {
                Debug.LogWarning($"宏 {logMacro} 不存在，无需删除。");
            }
        }
    }
}