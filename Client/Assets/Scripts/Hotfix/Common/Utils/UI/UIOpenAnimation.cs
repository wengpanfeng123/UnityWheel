// using System;
// using System.Collections;
// using System.Collections.Generic;
// using HsCore;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace HsJam
// {
//     [RequireComponent(typeof(Animation))]
//     public class UIOpenAnimation:UIBaseAnimation
//     {
//         private Animation _animation;
//         [Header("打开页面时的动画名称")]
//         private string AnimationName = "Anim_UIOpenScale";
//
//         private void Awake()
//         {
//             _animation = GetComponent<Animation>();
//         }
//
//         public void OnValidate()
//         {
//             GetComponent<Animation>().playAutomatically = false;
//         }
//
//         public override void OpenAnimation()
//         {
//             base.OpenAnimation();
//             if (string.IsNullOrEmpty(AnimationName))
//             {
//                 return;
//             }
//
//             if (!_animation.GetClip(AnimationName))
//             {
//                 Debug.LogError($"动画{AnimationName}不存在。检查动画列表");
//                 return;
//             }
//
//             _animation.Play(AnimationName);
//         }
//         
//         public override void Dispose()
//         {
//             base.Dispose();
//         }
//     }  
// }
//
