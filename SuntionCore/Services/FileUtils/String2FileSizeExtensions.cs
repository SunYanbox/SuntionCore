using JetBrains.Annotations;

namespace SuntionCore.Services.FileUtils;

public static class String2FileSizeExtensions
{
    /// <summary>
    /// 为String类型扩展的获取文件尺寸函数
    /// </summary>
    /// <param name="value">文件路径</param>
    /// <param name="decimalPlaces">保留的小数位数</param>
    /// <returns>文件尺寸</returns>
    [UsedImplicitly]
    public static string ToFileSize(this string? value, int decimalPlaces = 2)
    {
        return FileSizeUtil.GetFileSize(value ?? "[String.Empty]", decimalPlaces);
    }
    
    /// <summary>
    /// 为String类型扩展的计算文件尺寸函数
    /// </summary>
    /// <param name="value">文件路径</param>
    /// <returns>文件尺寸</returns>
    [UsedImplicitly]
    public static long CalFileSize(this string? value)
    {
        return FileSizeUtil.CalFileSize(value ?? "[String.Empty]");
    }
}