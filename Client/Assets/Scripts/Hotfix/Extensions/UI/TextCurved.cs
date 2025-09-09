/*************************************************************************
 * Copyright  xicheng. All rights reserved.
 *------------------------------------------------------------------------
 * File     : ArcText.cs
 * Author   : xicheng
 * Date     : 2025-09-04 14:00
 * Tips     : xicheng知识库
 * Description : 实现TextMeshPro字体的弧形效果
 *************************************************************************/

using UnityEngine;
using System.Collections;
using TMPro;

namespace HsJam
{
    [ExecuteInEditMode]
    public class TextCurved : MonoBehaviour
    {
        private TMP_Text _textComponent;

        [Tooltip("垂直向曲线")]
        public AnimationCurve vertexCurve = new(new Keyframe(0, 0), new Keyframe(0.5f, 2.0f), new Keyframe(1, 0));

        [Tooltip("垂直向曲线")] public float curveScale = 1.0f;

        void Awake()
        {
            _textComponent = gameObject.GetComponent<TMP_Text>();
        }

        void Start()
        {
            StartCoroutine(WarpText());
        }


        private AnimationCurve CopyCurve(AnimationCurve curve)
        {
            var newCurve = new AnimationCurve
            {
                keys = curve.keys
            };
            return newCurve;
        }


        /// <summary>
        ///  按照Unity动画曲线弯曲文本。
        /// </summary>
        IEnumerator WarpText()
        {
            //设置前后曲线的模式为Clamp
            vertexCurve.preWrapMode = WrapMode.Clamp;
            vertexCurve.postWrapMode = WrapMode.Clamp;

            //强制标记文本属性已更改，确保TextMeshPro对象被更新
            _textComponent.havePropertiesChanged = true;
            float oldCurveScale = curveScale;
            AnimationCurve oldCurve = CopyCurve(vertexCurve);

            while (true)
            {
                // 检查文本属性是否未更改且曲线参数未变化
                if (!_textComponent.havePropertiesChanged && Mathf.Approximately(oldCurveScale, curveScale) &&
                    Mathf.Approximately(oldCurve.keys[1].value, vertexCurve.keys[1].value))
                {
                    yield return null;
                    continue;
                }

                // 更新旧的曲线缩放值和曲线值
                oldCurveScale = curveScale;
                oldCurve = CopyCurve(vertexCurve);

                //强制更新网格，生成网格并填充textInfo数据
                _textComponent.ForceMeshUpdate();

                //获取字符数据
                TMP_TextInfo textInfo = _textComponent.textInfo;
                int characterCount = textInfo.characterCount;

                if (characterCount == 0)
                    continue;

                //获取文本边界的最小、最大值
                float boundsMinX = _textComponent.bounds.min.x; //textInfo.meshInfo[0].mesh.bounds.min.x;
                float boundsMaxX = _textComponent.bounds.max.x; //textInfo.meshInfo[0].mesh.bounds.max.x;

                for (int i = 0; i < characterCount; i++)
                {
                    //跳过不可见字符
                    if (!textInfo.characterInfo[i].isVisible)
                        continue;
                    // 获取当前字符的顶点索引
                    int vertexIndex = textInfo.characterInfo[i].vertexIndex;
                    // 获取当前字符使用的材质索引
                    int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    // 获取对应材质的顶点数组
                    var vertices = textInfo.meshInfo[materialIndex].vertices;
                    // 计算每个字符的基线中点偏移量
                    Vector3 offsetToMidBaseline =
                        new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2,
                            textInfo.characterInfo[i].baseLine);
                    //float offsetY = VertexCurve.Evaluate((float)i / characterCount + loopCount / 50f); // Random.Range(-0.25f, 0.25f);

                    //调整顶点位置，使其相对于基线中点
                    vertices[vertexIndex + 0] += -offsetToMidBaseline;
                    vertices[vertexIndex + 1] += -offsetToMidBaseline;
                    vertices[vertexIndex + 2] += -offsetToMidBaseline;
                    vertices[vertexIndex + 3] += -offsetToMidBaseline;

                    // 根据动画曲线计算每个字符的旋转角度
                    float x0 = (offsetToMidBaseline.x - boundsMinX) /
                               (boundsMaxX - boundsMinX); // 字符相对于网格边界的位置。
                    // 计算稍微偏移的位置，用于计算切线
                    float x1 = x0 + 0.0001f;
                    // 根据曲线计算Y轴偏移量
                    float y0 = vertexCurve.Evaluate(x0) * curveScale;
                    float y1 = vertexCurve.Evaluate(x1) * curveScale;

                    Vector3 horizontal = new Vector3(1, 0, 0);
                    // 计算切线向量
                    Vector3 tangent = new Vector3(x1 * (boundsMaxX - boundsMinX) + boundsMinX, y1) -
                                      new Vector3(offsetToMidBaseline.x, y0);
                    // 计算向量夹角
                    float dot = Mathf.Acos(Vector3.Dot(horizontal, tangent.normalized)) * 57.2957795f;
                    Vector3 cross = Vector3.Cross(horizontal, tangent);
                    // 根据叉积Z值确定旋转角度
                    float angle = cross.z > 0 ? dot : 360 - dot;
                    // 创建变换矩阵，包含位移、旋转和缩放
                    var matrix = Matrix4x4.TRS(new Vector3(0, y0, 0), Quaternion.Euler(0, 0, angle), Vector3.one);
                    // 应用变换矩阵到字符的四个顶点
                    vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
                    vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
                    vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
                    vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);

                    // 将顶点位置移回原来的基线中点位置
                    vertices[vertexIndex + 0] += offsetToMidBaseline;
                    vertices[vertexIndex + 1] += offsetToMidBaseline;
                    vertices[vertexIndex + 2] += offsetToMidBaseline;
                    vertices[vertexIndex + 3] += offsetToMidBaseline;
                }

                //更新顶点数据
                _textComponent.UpdateVertexData();

                yield return new WaitForSeconds(0.025f);
            }
        }
    }
}