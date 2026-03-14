#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets.Settings.GroupSchemas;
using UnityEngine;

public static class AddressablesEnvironmentSwitcher
{
    // 切换到本地开发环境
    [MenuItem("Tools/Addressables/Set Local Environment")]
    public static void SetLocalEnvironment()
    {
        SetActiveProfile("Local");
        ApplyLocalSettings();
    }

    // 切换到测试服环境
    [MenuItem("Tools/Addressables/Set Test Environment")]
    public static void SetTestEnvironment()
    {
        SetActiveProfile("Test");
        ApplyTestSettings();
    }

    // 切换到正式服环境
    [MenuItem("Tools/Addressables/Set Production Environment")]
    public static void SetProductionEnvironment()
    {
        SetActiveProfile("Production");
        ApplyProductionSettings();
    }

    private static void SetActiveProfile(string profileName)
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        string profileId = settings.profileSettings.GetProfileId(profileName);
        if (!string.IsNullOrEmpty(profileId))
        {
            settings.activeProfileId = profileId;
            Debug.Log($"已切换到Profile: {profileName}");
        }
        else
        {
            Debug.LogError($"未找到Profile: {profileName}");
        }
        SaveSettings(settings);
    }

    private static void ApplyLocalSettings()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        // 本地环境设置
        settings.BuildRemoteCatalog = false; // 不生成远程Catalog
        //settings.DisableCatalogUpdateOnStartup = false; // 禁用自动更新
        // 更新所有Group的设置
        foreach (var group in settings.groups)
        {
            foreach (var schema in group.Schemas)
            {
                if (schema is BundledAssetGroupSchema bundledSchema)
                {
                    // 本地环境使用最快加载模式
                    bundledSchema.BundleMode = BundledAssetGroupSchema.BundlePackingMode.PackTogether;
                    bundledSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.Uncompressed;
                }
            }
        }

        SaveSettings(settings);
    }

    private static void ApplyTestSettings()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        // 测试服设置
        settings.BuildRemoteCatalog = true;
        settings.DisableCatalogUpdateOnStartup = true; //初始化自动更新catalog。
        
        // 更新所有Group的设置
        foreach (var group in settings.groups)
        {
            foreach (var schema in group.Schemas)
            {
                if (schema is BundledAssetGroupSchema bundledSchema)
                {
                    // 测试服使用LZ4压缩（平衡加载速度和包大小）
                    bundledSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                }
            }
        }

        SaveSettings(settings);
    }

    private static void ApplyProductionSettings()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings == null) return;

        // 正式服设置
        settings.BuildRemoteCatalog = true;
        settings.DisableCatalogUpdateOnStartup = true; // 由游戏逻辑控制更新
    
        foreach (var group in settings.groups)
        {
            // 设置BundledAssetGroupSchema
            var bundledSchema = group.GetSchema<BundledAssetGroupSchema>();
            if (bundledSchema != null)
            {
                bundledSchema.Compression = BundledAssetGroupSchema.BundleCompressionMode.LZ4;
                bundledSchema.LoadPath.SetVariableByName(settings, "{ProductionLoadPath}/[BuildTarget]");
            }
        
            // ✅ 正确配置ContentUpdateGroupSchema（Unity 2022.3.28f1 + Addressables 1.21.21）
            var contentSchema = group.GetSchema<ContentUpdateGroupSchema>();
            if (contentSchema != null)
            {
                // // 允许内容更新
                // contentSchema.IncludeInBuild = true; // 资源包含在初始构建中
                //
                // // 设置内容更新限制（关键属性）
                // contentSchema.ContentUpdateRestriction = ContentUpdateGroupSchema.ContentUpdateRestrictionType.CanChangePostRelease;
                //
                // // 启用缓存（注意：属性名在这个版本是小写开头）
                // contentSchema.useAssetBundleCache = true;
                //
                // // 设置缓存清理策略（注意：属性名在这个版本是小写开头）
                // contentSchema.cacheClearBehavior = ContentUpdateGroupSchema.CacheClearOptions.ClearWhenNewVersionLoaded;
            }
        }

        SaveSettings(settings);
    }
    
    private static void SaveSettings(AddressableAssetSettings settings)
    {
        EditorUtility.SetDirty(settings);// 标记设置已更改
        AssetDatabase.SaveAssets();
    }
}
#endif