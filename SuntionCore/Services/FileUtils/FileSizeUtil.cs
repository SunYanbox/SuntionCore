using SuntionCore.Services.FileUtils.Exceptions;

namespace SuntionCore.Services.FileUtils;

public static class FileSizeUtil
{
    private static readonly string[] SizeUnits = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];

    /// <summary>
    /// 计算路径的文件的尺寸
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="GetFileSizeException"></exception>
    public static long CalFileSize(string filePath)
    {
        long bytes;
        
        try
        {
            bytes = new FileInfo(filePath).Length;
        }
        catch (Exception e)
        {
            throw new GetFileSizeException(filePath, e);
        }
        
        return bytes;
    }
    
    /// <summary>
    /// 将字节数格式化为可读的文件大小
    /// </summary>
    public static string GetFileSize(string filePath, int decimalPlaces = 2)
    {
        var emptySize = $"0 {SizeUnits[0]}";
        
        if (!File.Exists(filePath)) return emptySize;

        long bytes = CalFileSize(filePath);
        
        if (bytes == 0) return emptySize;
        
        var magnitude = (int)Math.Floor(Math.Log(bytes, 1024));
        magnitude = Math.Min(magnitude, SizeUnits.Length - 1);
        
        var adjustedSize = bytes / Math.Pow(1024, magnitude);
        return $"{adjustedSize.ToString($"F{decimalPlaces}")} {SizeUnits[magnitude]}";
    }
}