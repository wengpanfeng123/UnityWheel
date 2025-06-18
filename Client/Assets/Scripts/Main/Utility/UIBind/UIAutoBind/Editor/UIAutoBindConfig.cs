using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Gen
{
    [CreateAssetMenu(fileName = "UIAutoBindConfig",menuName ="UI/AutoBind Config" )]
    public class UIAutoBindConfig:ScriptableObject
    {
        public string outputPath = "Assets/Scripts/UI/Generated";
        public string @namespace = "";
        public bool useSerializeField = true;
    }
}

