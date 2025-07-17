using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Xicheng.Common;

namespace Xicheng.UIsystem.gen
{
    public class LoopListNode:Node
    {
        public LoopListNode(Transform transform, Transform root) : base(transform, root)
        {
            
        }

        public override string getVarDefine()
        {
            // return $@"    self.{transform.name} = FindText(t, ""{GameUtils.GetPath(root, transform)}"")";
            string text = $@"    private LoopListView2 {VarDefine};";
            return text;
        }

        public override string getLocalFind()
        {
            string text = $@"        {VarDefine} = Finder.LoopListView2(transform,""{GameUtility.GetPath(root, transform)}"");";
            return text;
        }
    }
}