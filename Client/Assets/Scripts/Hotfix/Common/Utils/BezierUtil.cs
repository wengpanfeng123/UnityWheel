using System;
using UnityEngine;

namespace HsJam
{
    /* 1阶  p0,p1 是一条直线
     * 2阶  抛物线  p0,p1,p2
     * 3阶  曲线/S线 p0,p1,p2,p3
     */
    public static class BezierUtil
    {
        #region 2阶贝塞尔曲线离散点序列

        /// <summary>
        /// 2阶贝塞尔路劲点
        /// </summary>
        /// <param name="startPos">起始位置</param>
        /// <param name="endPos">终点位置</param>
        /// <param name="ctrlPointPos">控制点位置</param>
        /// <param name="pointCount">路径点数量</param>
        /// <param name="debug">编辑器模式下,绘制曲线路径。</param>
        /// <returns>路径点集合</returns>
        public static Vector3[] GetBezier2Path(Vector3 startPos, Vector3 endPos, Vector3 ctrlPointPos, int pointCount)
        {
            return Bezier2Path(startPos, ctrlPointPos, endPos, pointCount);
        }


        /// <summary>
        /// 2阶贝塞尔路劲点
        /// </summary>
        /// <param name="startPos">起始位置</param>
        /// <param name="endPos">终点位置</param>
        /// <param name="centerOffset">相对于中心点的偏移位置</param>
        /// <param name="pointCount">路径点数量</param>
        /// <returns></returns>
        public static Vector3[] GetBezier2PathByOffset(Vector3 startPos, Vector3 endPos, Vector3 centerOffset,
            int pointCount)
        {
            Vector3 ctrlPoint = GetBezierCenterPoint(startPos, endPos, centerOffset);
            return Bezier2Path(startPos, ctrlPoint, endPos, pointCount);
        }

        private static Vector3 GetBezierCenterPoint(Vector3 startPos, Vector3 endPos, Vector3 centerPosOffset)
        {
            Vector3 normalized = (endPos - startPos).normalized;
            float distance = Vector3.Distance(startPos, endPos);
            float percent = 0.5f;
            Vector3 centerPoint = normalized * (distance * percent) + startPos;
            Vector3 centerPointOffset = centerPoint + (startPos.x > endPos.x ? centerPosOffset * -1 : centerPosOffset);
            return centerPointOffset;
        }

        /// <summary>
        /// 获取两点之间距离一定百分比的一个点
        /// </summary>
        /// <param name="start">起始点</param>
        /// <param name="end">结束点</param>
        /// <param name="percent">起始点到目标点距离百分比</param>
        /// <returns></returns>
        private static Vector3 GetBetweenPoint(Vector3 start, Vector3 end, float percent = 0.5f)
        {
            Vector3 normal = (end - start).normalized;
            float distance = Vector3.Distance(start, end);
            return normal * (distance * percent) + start;
        }

        /// <summary>
        /// 生成贝塞尔曲线上的离散点序列。
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="ctrlPoint"></param>
        /// <param name="endPos"></param>
        /// <param name="pointCount"></param>
        /// <returns></returns>
        private static Vector3[] Bezier2Path(Vector3 startPos, Vector3 ctrlPoint, Vector3 endPos, int pointCount)
        {
            if (pointCount < 2)
            {
                Debug.LogError("Point count must be at least 2 to generate a Bezier path.");
                return Array.Empty<Vector3>();
            }

            Vector3[] path = new Vector3[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                float t = (float)i / (pointCount - 1);
                path[i] = CalculateBezier2(startPos, ctrlPoint, endPos, t);
            }
            return path;
        }

        // 2阶贝塞尔曲线
        private static Vector3 CalculateBezier2(Vector3 startPos, Vector3 ctrlPoint, Vector3 endPos, float t)
        {
            return (1 - t) * (1 - t) * startPos + 2 * t * (1 - t) * ctrlPoint + t * t * endPos;
        }

        #endregion

        #region 3阶贝塞尔曲线离散点序列

        /// <summary>
        /// 3阶贝塞尔曲线的路径点集合
        /// </summary>
        /// <param name="startPos">起点位置</param>
        /// <param name="ctrlPos1">第1个中间控制点</param>
        /// <param name="ctrlPos2">第2个中间控制点</param>
        /// <param name="endPos">终点</param>
        /// <param name="pointCount">生成的点数量</param>
        /// <returns>路径点数组</returns>
        public static Vector3[] GetBezier3Path(Vector3 startPos, Vector3 ctrlPos1, Vector3 ctrlPos2,
            Vector3 endPos, int pointCount)
        {
            if (pointCount < 2) // 至少2个点
            {
                Debug.LogError("Point count must be at least 2 to generate a Bezier path.");
                return Array.Empty<Vector3>();
            }
            
            Vector3[] path = new Vector3[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(pointCount - 1);
                // 三阶贝塞尔公式：
                // B(t) = (1-t)^3*P0 + 3*(1-t)^2*t*P1 + 3*(1-t)*t^2*P2 + t^3*P3
                float oneMinusT = 1f - t;
                path[i] = oneMinusT * oneMinusT * oneMinusT * startPos +
                            3f * oneMinusT * oneMinusT * t * ctrlPos1 +
                            3f * oneMinusT * t * t * ctrlPos2 +
                            t * t * t * endPos;
            }
            return path;
        }
        
        #endregion
        
        #region 4阶贝塞尔曲线离散点序列
        /// <summary>
        /// 4阶贝塞尔曲线路径点集合
        /// </summary>
        /// <param name="startPos">起点</param>
        /// <param name="endPos">终点</param>
        /// <param name="ctrlArr">控制点集合(数量必须为3)</param>
        /// <param name="pointCount">采样点数量</param>
        /// <returns>路径点数组</returns>
        public static Vector3[] GetBezier4Path(Vector3 startPos, Vector3 endPos,Vector3[] ctrlArr,int pointCount)
        {
            if (pointCount < 2) 
                pointCount = 2;

            Vector3 controlPos1 = ctrlArr[0];
            Vector3 controlPos2 = ctrlArr[1];
            Vector3 controlPos3 = ctrlArr[2];
                
            Vector3[] points = new Vector3[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(pointCount - 1);
                float oneMinusT = 1f - t;
                points[i] = Mathf.Pow(oneMinusT, 4) * startPos
                            + 4f * Mathf.Pow(oneMinusT, 3) * t * controlPos1
                            + 6f * Mathf.Pow(oneMinusT, 2) * Mathf.Pow(t, 2) * controlPos2
                            + 4f * oneMinusT * Mathf.Pow(t, 3) * controlPos3
                            + Mathf.Pow(t, 4) * endPos;
            }
            return points;
        }
        
        #endregion
    }
}