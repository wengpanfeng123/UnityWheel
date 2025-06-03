using UnityEngine;
using UnityEngine.UI;
using xicheng.common;

namespace xicheng.uisystem.gen
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
            string text = $@"        {VarDefine} = Finder.Text(transform,""{GameUtility.GetPath(root, transform)}"");";
            return text;
        }
    }
}