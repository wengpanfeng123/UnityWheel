using UnityEngine;

namespace Xicheng.Common
{
    public class RawImageNode : Node
    {
        public RawImageNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        public override string getVarDefine()
        {
            string rawImage = $@"    private RawImage {VarDefine};";
            return rawImage;
            
            //return $@"    self.{transform.name} = FindRawImage(t, ""{GameUtility.GetPath(root, transform)}"")";
        }

        public override string getLocalFind()
        {
            string image = $@"        {VarDefine} = Finder.RawImage(transform,""{TransformUtil.GetPath(root, transform)}"");";
            return image;
            //return "local FindRawImage = CS.VirtualWorld.Utils.LuaUtils.FindRawImage";
        }
    }
}