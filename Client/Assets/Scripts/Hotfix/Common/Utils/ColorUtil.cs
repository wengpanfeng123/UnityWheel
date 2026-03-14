using UnityEngine;

/// <summary>
///颜色工具
/// </summary>
public static class ColorUtil
{
 
    /// <summary>
    /// 十六进制->Color（支持带#或不带#的格式）
    /// </summary>
    /// <param name="hex">16进制颜色串</param>
    /// <returns></returns>
    public static Color HexToColor(string hex)
    {
        hex = hex.Replace("#", "");
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return new Color(r / 255f, g / 255f, b / 255f);
    }
}