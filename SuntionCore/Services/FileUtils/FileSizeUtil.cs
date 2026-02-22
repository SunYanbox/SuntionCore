using SuntionCore.Services.FileUtils.Exceptions;
using SuntionCore.Services.MagnitudeUtils;

namespace SuntionCore.Services.FileUtils;

public static class FileSizeUtil
{
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
        var emptySize = $"0 {MagnitudePreset.FileSizeBinary.Units[0]}";
        
        if (!File.Exists(filePath)) return emptySize;

        long bytes = CalFileSize(filePath);

        return MagnitudeFormatter.Format(bytes, MagnitudePreset.FileSizeBinary, decimalPlaces);
    }
}