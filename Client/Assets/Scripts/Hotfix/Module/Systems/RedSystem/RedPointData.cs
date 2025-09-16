// RedPointType.cs
public enum RedPointType
{
    /// <summary>
    /// 数字类型
    /// </summary>
    Number,
    /// <summary>
    /// 感叹号类型
    /// </summary>
    Exclamation,
    /// <summary>
    /// 仅红点
    /// </summary>
    OnlyRed,
    /// <summary>
    /// 混合类型
    /// </summary>
    Hybrid
}

// RedPointData.cs
public struct RedPointData
{
    public int totalValue;
    public RedPointType displayType;
    public bool isActive;

    public bool Equals(RedPointData other)
    {
        return totalValue == other.totalValue &&
               displayType == other.displayType &&
               isActive == other.isActive;
    }
}