using UnityEngine;
using UnityEngine.UI;
using Xicheng.Common;

namespace Xicheng.UIsystem.gen
{
    public class TextNode:Node
    {
        public TextNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        public override string getVarDefine()
        {
            // return $@"    self.{transform.name} = FindText(t, ""{GameUtils.GetPath(root, transform)}"")";
            string text = $@"    private Text {VarDefine};";
            return text;
        }

        public override string getLocalFind()
        {
            string text = $@"        {VarDefine} = Finder.Text(transform,""{TransformUtil.GetPath(root, transform)}"");";
            return text;
        }
    }
}