using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xicheng.Common;

namespace Xicheng.UIsystem.gen
{
    public class TMPNode:Node
    {
        public TMPNode(Transform transform, Transform root) : base(transform, root)
        {
            
        }

        public override string getVarDefine()
        {
            // return $@"    self.{transform.name} = FindText(t, ""{GameUtils.GetPath(root, transform)}"")";
            string text = $@"    private TextMeshProUGUI _{transform.name};";
            return text;
        }

        public override string getLocalFind()
        {
            string text = $@"        _{transform.name} = Finder.TextMeshPro(transform,""{GameUtility.GetPath(root, transform)}"");";
            return text;
        }
    }
}