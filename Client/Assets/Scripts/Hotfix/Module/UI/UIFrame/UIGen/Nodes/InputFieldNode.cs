using UnityEngine;

namespace xicheng.common
{
    public class InputFieldNode:Node
    {
        public InputFieldNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        public override string getVarDefine()
        {
    //         return $@"    self.{transform.name} = FindInputField(t, ""{GameUtils.GetPath(root, transform)}"")
    // if self.On{transform.name}EndEdit then
    //     self.{transform.name}.onEndEdit:AddListener(function(input)
    //         self:On{transform.name}EndEdit(input)
    //     end)
    // end
    // if self.On{transform.name}ValueChanged then
    //     self.{transform.name}.onValueChanged:AddListener(function(input)
    //         self:On{transform.name}ValueChanged(input)
    //     end)
    // end";
            string inputFiled = $@"    private InputField {VarDefine};";
            return inputFiled;
        }

        public override string getLocalFind()
        {
            string inputFiled = $@"        {VarDefine} = Finder.InputField(transform,""{GameUtility.GetPath(root, transform)}"");";
            return inputFiled;
        }
    }
}