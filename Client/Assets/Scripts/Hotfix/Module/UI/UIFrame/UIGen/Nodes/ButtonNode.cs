using UnityEngine;

namespace Xicheng.Common
{
    
    public class ButtonNode : Node
    {
        public ButtonNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        public override string getVarDefine()
        {
    //         return $@"    self.{transform.name} = FindButton(t, ""{GameUtils.GetPath(root, transform)}"")
    // if self.On{transform.name}Click then
    //     self.{transform.name}.onClick:AddListener(function()
    //         self:On{transform.name}Click()
    //     end)
    // end";
            
            string button = $@"    private Button {VarDefine};";
            return button;
        }

        public override string getLocalFind()
        {
            string button = $@"        {VarDefine} = Finder.Button(transform,""{TransformUtil.GetPath(root, transform)}"");";
            return button;
            //return "local FindButton = CS.VirtualWorld.Utils.LuaUtils.FindButton";
        }
    }

}