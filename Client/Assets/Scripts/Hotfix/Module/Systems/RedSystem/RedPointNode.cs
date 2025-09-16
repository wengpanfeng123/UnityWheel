// RedPointNode.cs

using System;
using System.Collections.Generic;
using UnityEngine;

public class RedPointNode
{
    #region Properties
    public string Path { get; private set; }
    public RedPointNode parent;
    public List<RedPointNode> children = new List<RedPointNode>();
    public event Action<RedPointData> OnValueChanged;

    private int value;
    private RedPointType type;
    private int cachedChildValue;
    private RedPointType cachedChildType;
    private bool isDirty;
    #endregion

    #region Public Methods
    public void Init(string path)
    {
        Path = path;
        Reset();
    }

    public void SetValue(int newValue, RedPointType newType)
    {
        value = newValue;
        type = newType;
        PropagateChange();
    }

    public void UpdateFromChildren()
    {
        int total = 0;
        bool hasExclamation = false;

        foreach (var child in children)
        {
            total += child.EffectiveValue;
            hasExclamation |= child.EffectiveType == RedPointType.Exclamation;
        }

        cachedChildValue = total;
        cachedChildType = hasExclamation ? RedPointType.Exclamation : RedPointType.Number;
        PropagateChange();
    }

    public void MarkDirty()
    {
        if (!isDirty)
        {
            isDirty = true;
            RedPointSystem.AddDirtyNode(this);
        }
    }

    public RedPointData GetState()
    {
        return new RedPointData
        {
            totalValue = EffectiveValue,
            displayType = EffectiveType,
            isActive = EffectiveValue > 0 || EffectiveType == RedPointType.Exclamation
        };
    }

    public void Reset()
    {
        value = 0;
        type = RedPointType.Number;
        cachedChildValue = 0;
        cachedChildType = RedPointType.Number;
        parent = null;
        children.Clear();
        OnValueChanged = null;
        isDirty = false;
    }
    #endregion

    #region Helper Properties
    // 使用预计算值避免重复计算
    public int EffectiveValue => Mathf.Min(value + cachedChildValue, 99);
    
    public RedPointType EffectiveType
    {
        get
        {
            // 子节点有感叹号 或 自身数值>0 → 优先显示感叹号。否则，显示红点数量-----TODO:后续优化。
            if (type == RedPointType.Hybrid)
                return (cachedChildType == RedPointType.Exclamation || value > 0) ? 
                    RedPointType.Exclamation : 
                    RedPointType.Number;

            return type;
        }
    }

    public bool IsDirty => isDirty;
    public void ClearDirty() => isDirty = false;
    #endregion

    #region Private Methods
    private void PropagateChange()
    {
        var newState = GetState();
        if (!newState.Equals(prevState))
        {
            prevState = newState;
            OnValueChanged?.Invoke(newState);
        }
    }

    private RedPointData prevState;
    #endregion
}