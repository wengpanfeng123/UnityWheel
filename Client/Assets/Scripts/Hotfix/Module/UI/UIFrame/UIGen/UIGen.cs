using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hotfix.Common;
using UnityEditor;
using UnityEngine;

namespace Xicheng.Common
{


    public class UIGen : MonoBehaviour
    {
#if UNITY_EDITOR
        public List<ComSetting> comSettings = new List<ComSetting>();
        private static readonly string _panelPath = PathConst.PanelPrefabPath;

        public void Generate(string prefabPath = null)
        {
            if (!prefabPath.StartsWith(_panelPath))
            {
                Debug.LogError($"只能生成{_panelPath}/下的prefab");
                return;
            }

            var fileName = Path.GetFileName(prefabPath);
            if (!fileName.StartsWith("panel") && !fileName.StartsWith("Panel") && !fileName.StartsWith("Com_"))
            {
                Debug.LogError($"只能生成Panel,panel,Com_开头的预制体,fileName:{fileName}");
                return;
            }

            CollectComs();

            var moduleName = Path.GetDirectoryName(prefabPath).Substring(_panelPath.Length + 1).Replace("\\", ".")
                .ToLower();
            new FileGen(moduleName, transform).GenToFile();
            foreach (var comSetting in comSettings)
            {
                if (comSetting.gen)
                {
                    new FileGen(moduleName, comSetting.transform).GenToFile();
                }
            }
        }

        public void CollectComs()
        {
            var children = new List<Transform>();
            GameUtility.CollectAllChildren(transform, false, ref children);

            List<ComSetting> newComSettings = new List<ComSetting>();
            foreach (var child in children)
            {
                if (!child.name.StartsWith("Com_"))
                    continue;
                var setting = new ComSetting();
                setting.transform = child;
                setting.gen = decideDefaultGenValue(child.gameObject);
                newComSettings.Add(setting);
            }

            var oldComSettingMap = new Dictionary<Transform, bool>();
            foreach (var comSetting in comSettings)
            {
                if (comSetting.transform == null)
                    continue;
                oldComSettingMap.Add(comSetting.transform, comSetting.gen);
            }

            foreach (var newComSetting in newComSettings)
            {
                if (oldComSettingMap.TryGetValue(newComSetting.transform, out var oldGen))
                {
                    newComSetting.gen = oldGen;
                }
            }

            comSettings = newComSettings;
            EditorUtility.SetDirty(this);
        }

        bool decideDefaultGenValue(GameObject go)
        {
            if (PrefabUtility.IsAnyPrefabInstanceRoot(go))
                return false;
            if (go.name.StartsWith("Com_ScrollView"))
                return false;
            if (go.name.StartsWith("Com_GridView"))
                return false;
            if (go.name.StartsWith("Com_GroupView"))
                return false;
            if (go.name.StartsWith("Com_AvatarView"))
                return false;
            if (go.name.StartsWith("Com_ScrollGridView"))
                return false;
            return true;
        }
#endif

    }
}



#if UNITY_EDITOR 
[Serializable]
public class ComSetting
{
    [SerializeField] public Transform transform;
    [SerializeField] public bool gen;
}
#endif

