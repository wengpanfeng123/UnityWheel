//- Created:       #xicheng#
// - CreateTime:      #CreateTime#
// - Description:   C#游戏工具类

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Xicheng.Common
{
    public static class TransformUtil
    {
        
        /// /////////////////////////Transform相关////////////////////////////
        /// <summary>
        /// 面向目标方向
        /// </summary>
        /// <param name="targetDirection">目标方向</param>
        /// <param name="transform">需要转向的对象</param>
        /// <param name="rotationSpeed">转向速度</param>
        public static void LookAtTarget(Vector3 targetDirection, Transform transform, float rotationSpeed)
        {
            if (targetDirection != Vector3.zero)
            {
                var targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed);
            }
        }

        /// <summary>
        /// 查找子物体（递归查找）
        /// </summary>
        /// <param name="trans">父物体</param>
        /// <param name="goName">子物体的名称</param>
        /// <returns>找到的相应子物体</returns>
        public static Transform FindChild(Transform trans, string goName)
        {
            Transform child = trans.Find(goName);
            if (child != null)
                return child;

            for (int i = 0; i < trans.childCount; i++)
            {
                child = trans.GetChild(i);
                var go = FindChild(child, goName);
                if (go != null)
                    return go;
            }
            return null;
        }
        
        public static string GetPath(Transform root, Transform cur)
        {
            StringBuilder sb = new StringBuilder();
            while (cur != root && cur != null)
            {
                sb.Insert(0, $"/{cur.name}");
                cur = cur.parent;
            }

            sb.Remove(0, 1);
            if(cur==null)
                throw new Exception("error root");
            return sb.ToString();
        }
        
        public static void CollectAllChildren(Transform cur, bool addSelf, ref List<Transform> children)
        {
            if (addSelf)
            {
                children.Add(cur);
            }

            for (int i = 0; i < cur.childCount; i++)
            {
                CollectAllChildren(cur.GetChild(i), true, ref children);
            }
        }
        
    }
}
