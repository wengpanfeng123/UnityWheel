using UnityEngine;

namespace Xicheng.Common
{
    public class GoNode : Node
    {
        public GoNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        public override string getVarDefine()
        {
            //return $@"    self.{transform.name} = FindGameObject(t, ""{GameUtils.GetPath(root, transform)}"")";
            string go = $@"        private GameObject {VarDefine};";
            return go;
        }

        public override string getLocalFind()
        {
            string go = $@"        {VarDefine} = Finder.GameObject(transform,""{GameUtility.GetPath(root, transform)}"");";
            return go;
           
        }
    }
}