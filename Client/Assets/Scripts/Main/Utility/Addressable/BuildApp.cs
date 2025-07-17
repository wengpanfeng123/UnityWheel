#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using System;
using UnityEngine;

/*  打包ab
 *  打包ab和app
 *
 */
internal class BuildLauncher
{
    public static string build_script = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
    public static string settings_asset = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
    public static readonly string ProfileName = "Default"; //配置文件名称
    private static AddressableAssetSettings _settings;

    static void GetSettingsObject(string settingsAsset)
    {
        // This step is optional, you can also use the default settings:
        //settings = AddressableAssetSettingsDefaultObject.Settings;

        _settings = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset) as AddressableAssetSettings;

        if (_settings == null)
            Debug.LogError($"{settingsAsset} couldn't be found or isn't a settings object.");
    }

    static void SetProfile(string profile)
    {
        string profileId = _settings.profileSettings.GetProfileId(profile);
        if (String.IsNullOrEmpty(profileId))
            Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                             $"using current profile instead.");
        else
            _settings.activeProfileId = profileId;
    }

    static void SetBuilder(IDataBuilder builder)
    {
        int index = _settings.DataBuilders.IndexOf((ScriptableObject)builder);

        if (index > 0)
            _settings.ActivePlayerDataBuilderIndex = index;
        else
            Debug.LogWarning($"{builder} must be added to the " +
                             $"DataBuilders list before it can be made " +
                             $"active. Using last run builder instead.");
    }

    static bool BuildAddressableContent()
    {
        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
        bool success = string.IsNullOrEmpty(result.Error);

        if (!success)
        {
            Debug.LogError("Addressables build error encountered: " + result.Error);
        }

        return success;
    }

    [MenuItem("Build/Build Addressables  only")]
    public static bool BuildAddressables()
    {
        //获取setting配置
        GetSettingsObject(settings_asset);
        //设置配置文件(本地，远端打包和加载地址)
        SetProfile(ProfileName);
        IDataBuilder builderScript = AssetDatabase.LoadAssetAtPath<ScriptableObject>(build_script) as IDataBuilder;

        if (builderScript == null)
        {
            Debug.LogError(build_script + " couldn't be found or isn't a build script.");
            return false;
        }

        SetBuilder(builderScript);

        return BuildAddressableContent();
    }

    [MenuItem("Build/Build Addressables and Player")]
    public static void BuildAddressablesAndPlayer()
    {
        bool contentBuildSucceeded = BuildAddressables();

        if (contentBuildSucceeded)
        {
            var options = new BuildPlayerOptions();
            BuildPlayerOptions playerSettings = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options);
            BuildPipeline.BuildPlayer(playerSettings);
        }
    }
}
#endif