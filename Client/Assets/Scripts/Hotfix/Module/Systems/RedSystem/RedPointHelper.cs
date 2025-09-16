namespace XiCheng.RedSystem
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public static class RedPointHelper
    {
        public static List<string> GetAllPaths()
        {
            List<string> paths = new List<string>();
            Type type = typeof(RedPointPath);

            // 获取所有公共静态常量字段（包含编译器生成的后台字段）
            FieldInfo[] fields = type.GetFields(
                BindingFlags.Public | 
                BindingFlags.Static | 
                BindingFlags.FlattenHierarchy
            );

            foreach (FieldInfo field in fields)
            {
                // 过滤条件：
                // 1. 是字面常量（IsLiteral）
                // 2. 字段类型为string
                // 3. 不是编译器生成的字段（排除类似<XXX>k__BackingField）
                if (field.IsLiteral && 
                    !field.IsInitOnly &&
                    field.FieldType == typeof(string) &&
                    !field.Name.Contains("<"))
                {
                    if (field.GetValue(null) is string value)
                    {
                        paths.Add(value);
                    }
                }
            }

            return paths;
        }
    }
}