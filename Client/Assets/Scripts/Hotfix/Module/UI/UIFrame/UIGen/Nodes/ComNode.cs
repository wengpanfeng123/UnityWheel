using UnityEngine;

namespace Xicheng.Common
{
    public class ComNode : Node
    {
        public ComNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        //TODO:待修改
        public override string getVarDefine()
        {
            return $@"    self.{transform.name} = FindGameObject(t, ""{TransformUtil.GetPath(root, transform)}"")";
        }

        public override string getLocalFind()
        {
            return "local FindGameObject = CS.VirtualWorld.Utils.LuaUtils.FindGameObject";
        }
        
        public   string getVarDefin1e()
        {
            //return $@"    self.{transform.name} = FindGameObject(t, ""{GameUtils.GetPath(root, transform)}"")";
            string go = $@"        private GameObject {VarDefine};";
            return go;
        }

        public  string getLocalFin1d()
        {
            string go = $@"        {VarDefine} = Finder.GameObject(transform,""{TransformUtil.GetPath(root, transform)}"");";
            return go;
           
        }
    }
}