using UnityEngine;
using UnityEngine.UI;
using Xicheng.Common;

namespace Xicheng.UIsystem.gen
{
    public class SliderNode : Node
    {
        public SliderNode(Transform transform, Transform root) : base(transform, root)
        {
        }

        public override string getVarDefine()
        {
    //         return $@"    self.{transform.name} = FindSlider(t, ""{GameUtils.GetPath(root, transform)}"")
    // if self.On{transform.name}ValueChanged then
    //     self.{transform.name}.onValueChanged:AddListener(function(value)
    //         self:On{transform.name}ValueChanged(value)
    //     end)
    // end";
            
            string slider = $@"private Slider {VarDefine};";
            return slider;
        }

        public override string getLocalFind()
        {
            string slider = $@"        {VarDefine} = Finder.Slider(transform,""{GameUtility.GetPath(root, transform)}"");";
            return slider;
        }
    }
}