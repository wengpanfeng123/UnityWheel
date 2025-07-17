using UnityEngine;

namespace Xicheng.Common
{
    public class ImageNode : Node
    {
        public ImageNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        public override string getVarDefine()
        {
            //return $@"    self.{transform.name} = FindImage(t, ""{GameUtils.GetPath(root, transform)}"")";
            string image = $@"    private Image {VarDefine};";
            return image;
        }

        public override string getLocalFind()
        {
            string image = $@"        {VarDefine} = Finder.Image(transform,""{GameUtility.GetPath(root, transform)}"");";
            return image;
        }
    }
}