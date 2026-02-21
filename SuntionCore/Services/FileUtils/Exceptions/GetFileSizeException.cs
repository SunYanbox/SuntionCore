namespace SuntionCore.Services.FileUtils.Exceptions;

public class GetFileSizeException(string filePath, Exception inner): Exception($"获取文件({filePath})尺寸时出错: {inner.Message}", inner);