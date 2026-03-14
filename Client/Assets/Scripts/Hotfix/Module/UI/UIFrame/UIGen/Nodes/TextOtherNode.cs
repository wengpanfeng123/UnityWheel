using UnityEngine;

namespace Xicheng.UIsystem.gen
{
    public class TextOtherNode:Node
    {
        public TextOtherNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        public override string getVarDefine()
        {
            // if (transform.name.EndsWith("#"))
            // {
            //     return $@"    //self:Localize(FindText(t, ""{GameUtils.GetPath(root, transform)}""), true)";   
            // }
            // else
            // {
            //     return $@"    //self:Localize(FindText(t, ""{GameUtils.GetPath(root, transform)}""))";   
            // }

            return string.Empty;
        }

        public override string getLocalFind()
        {
            return string.Empty;
            // return "local FindText = CS.VirtualWorld.Utils.LuaUtils.FindText";
        }
    }
}