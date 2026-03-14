using System;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Arranges child objects in a 3D grid.
/// </summary>
[AddComponentMenu("Layout/GridLayoutGroup 3D")]
[ExecuteInEditMode] // 使脚本在编辑器模式下也能运行，方便实时预览
public class GridLayoutGroup3D : MonoBehaviour
{
    public enum ConstraintType
    {
        FixedColumnCount,
        FixedRowCount
    }
    
    public enum LayoutPlane { XY, XZ, ZY }

    public LayoutPlane layoutPlane = LayoutPlane.XZ;
    public ConstraintType constraint = ConstraintType.FixedColumnCount;
    [Min(1)]
    public int constraintCount = 4;
    public Vector3 cellSize = new Vector3(1f, 1f, 1f);
    public Vector3 spacing = new Vector3(0f, 0f, 0f);
    public TextAnchor childAnchor = TextAnchor.MiddleCenter;
    public TextAnchor childAlignment = TextAnchor.MiddleCenter;
    public bool updateOnStart = true;
    public bool autoUpdateInEditor = true;


    // 在游戏启动时调用
    void Start()
    {
        if (Application.isPlaying && updateOnStart)
        {
            UpdateLayout();
        }
    }

#if UNITY_EDITOR
    // 当在Inspector中修改脚本属性时调用
    private void OnValidate()
    {
        if (autoUpdateInEditor)
        {
            UnityEditor.EditorApplication.delayCall += EditorUpdateLayout;
        }
    }

    // 当父对象的子对象变换发生变化时调用
    private void OnTransformChildrenChanged()
    {
        if (autoUpdateInEditor)
        {
            UnityEditor.EditorApplication.delayCall += EditorUpdateLayout;
        }
    }

    private void EditorUpdateLayout()
    {
        UnityEditor.EditorApplication.delayCall -= EditorUpdateLayout;
        
        if (!this || !this.gameObject)
        {
            return;
        }
        
        if (!enabled) return;
        
        UpdateLayout();
    }
    
#endif

    /// <summary>
    /// Public method to manually trigger a layout update.
    /// Can be called from other scripts.
    /// </summary>
    [ContextMenu("Update Layout")]
    public void UpdateLayout()
    {
        // 获取所有激活的子对象
        List<Transform> activeChildren = new List<Transform>();
        foreach (Transform child in transform)
        {
            if (child != null && child.gameObject.activeInHierarchy)
            {
                activeChildren.Add(child);
            }
        }

        if (activeChildren.Count == 0) return;

        int columnCount = 0;
        int rowCount = 0;
        int childCount = activeChildren.Count;

        // 根据约束条件计算行数和列数
        if (constraint == ConstraintType.FixedColumnCount)
        {
            columnCount = Mathf.Max(1, constraintCount);
            rowCount = Mathf.CeilToInt((float)childCount / columnCount);
        }
        else // FixedRowCount
        {
            rowCount = Mathf.Max(1, constraintCount);
            columnCount = Mathf.CeilToInt((float)childCount / rowCount);
        }
        
        int childIndex = 0;
        for (int y = 0; y < rowCount; y++)
        {
            for (int x = 0; x < columnCount; x++)
            {
                if (childIndex >= childCount) break;

                Transform child = activeChildren[childIndex];

                child.localPosition = GetLocalPosition(rowCount, columnCount, x, y);

                childIndex++;
            }
        }
    }

    private Vector3 GetLocalPosition(int rowCount, int columnCount, int x, int y)
    {
        // 计算每个子对象的位置
        // 注意：Y轴是负的，这样排列顺序会从上到下，更符合UI习惯
        float posX = x * (cellSize.x + spacing.x);
        float posY = -y * (cellSize.y + spacing.y);
        //float posZ = 0; // 默认在XY平面上排列。如果需要Z轴排列，可以修改这里

        switch (childAnchor)
        {
            case TextAnchor.UpperLeft:
                break;
            case TextAnchor.UpperCenter:
                break;
            case TextAnchor.UpperRight:
                break;
            case TextAnchor.MiddleLeft:
                break;
            case TextAnchor.MiddleCenter:
                posX += cellSize.x * 0.5f;
                posY -= cellSize.y * 0.5f;
                break;
            case TextAnchor.MiddleRight:
                break;
            case TextAnchor.LowerLeft:
                break;
            case TextAnchor.LowerCenter:
                break;
            case TextAnchor.LowerRight:
                posX += cellSize.x;
                posY -= cellSize.y;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        // 1. 计算网格总尺寸
        float totalWidth = (columnCount * cellSize.x) + (Mathf.Max(0, columnCount - 1) * spacing.x);
        float totalHeight = (rowCount * cellSize.y) + (Mathf.Max(0, rowCount - 1) * spacing.y);
        
        float offsetX = 0;
        float offsetY = 0;

        switch (childAlignment)
        {
            case TextAnchor.UpperLeft:
                break;
            case TextAnchor.UpperCenter:
                break;
            case TextAnchor.UpperRight:
                break;
            case TextAnchor.MiddleLeft:
                break;
            case TextAnchor.MiddleCenter:
                offsetX = -totalWidth / 2;
                offsetY = totalHeight / 2;
                break;
            case TextAnchor.MiddleRight:
                break;
            case TextAnchor.LowerLeft:
                break;
            case TextAnchor.LowerCenter:
                break;
            case TextAnchor.LowerRight:
                offsetX = -totalWidth;
                offsetY = totalHeight;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        posX += offsetX;
        posY += offsetY;

        Vector3 localPosition;

        switch (layoutPlane)
        {
            case LayoutPlane.XY:
                localPosition = new Vector3(posX, posY, 0f);
                break;
            case LayoutPlane.XZ:
                localPosition = new Vector3(posX, 0, posY);
                break;
            case LayoutPlane.ZY:
                localPosition = new Vector3(0, posX, posY);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        return localPosition;
    }
}