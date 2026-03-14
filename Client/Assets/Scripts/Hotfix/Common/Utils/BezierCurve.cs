using System.Collections.Generic;
using UnityEngine;

namespace HsJam
{
    /// <summary>
    /// 支持任意阶的贝塞尔曲线
    /// </summary>
    public class BezierCurve
    {
        /// <summary>
        /// 控制点列表。存储所有控制点的列表。贝塞尔曲线的形状完全由这些点决定，阶数=控制点数-1。
        /// </summary>
        private readonly List<Vector3> _ctrlPoints;

        //允许你直接传入任意数量的控制点来创建曲线
        public BezierCurve(params Vector3[] points)
        {
            _ctrlPoints = new List<Vector3>(points);
        }

        /// <summary>
        /// 获取贝塞尔曲线上t（0~1）处的点。
        /// </summary>
        public Vector3 Evaluate(float t)
        {
            List<Vector3> temp = new List<Vector3>(_ctrlPoints);
            int n = temp.Count;
            for (int k = 1; k < n; k++)
            {
                for (int i = 0; i < n - k; i++)
                {
                    temp[i] = (1 - t) * temp[i] + t * temp[i + 1];
                }
            }

            return temp[0];
        }

        /// <summary>
        /// 获取贝塞尔曲线上t处的切线向量
        /// </summary>
        public Vector3 GetTangent(float t)
        {
            // 对所有相邻控制点求差,得到n-1阶贝塞尔曲线，并在t处采样
            List<Vector3> tangents = new List<Vector3>();
            for (int i = 0; i < _ctrlPoints.Count - 1; i++)
            {
                tangents.Add(_ctrlPoints[i + 1] - _ctrlPoints[i]);
            }

            // 递归降阶
            var tangentCurve = new BezierCurve(tangents.ToArray());
            return tangentCurve.Evaluate(t) * (_ctrlPoints.Count - 1);
        }

        /// <summary>
        /// 获取采样点
        /// </summary>
        /// <param name="pointCount">采样点数量</param>
        /// <returns></returns>
        public Vector3[] Sample(int pointCount)
        {
            if (pointCount < 2)
                pointCount = 2;
            Vector3[] points = new Vector3[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(pointCount - 1);
                points[i] = Evaluate(t);
            }
            return points;
        }

        /// <summary>
        /// 在Scene视图可视化曲线
        /// </summary>
        public void DrawDebug(int pointCount = 32, Color? color = null, float duration = 0, bool depthTest = false)
        {
            var points = Sample(pointCount);
            Color c = color ?? Color.yellow;
            for (int i = 0; i < points.Length - 1; i++)
            {
                Debug.DrawLine(points[i], points[i + 1], c, duration, depthTest);
            }
        }

        /// <summary>
        /// 添加控制点
        /// </summary>
        public void AddPoint(Vector3 point) => _ctrlPoints.Add(point);

        /// <summary>
        /// 移除最后一个控制点
        /// </summary>
        public void RemoveLastPoint()
        {
            if (_ctrlPoints.Count > 0)
                _ctrlPoints.RemoveAt(_ctrlPoints.Count - 1);
        }

        /// <summary>
        /// 获取曲线阶数
        /// </summary>
        public int Degree => _ctrlPoints.Count - 1;
    }
}