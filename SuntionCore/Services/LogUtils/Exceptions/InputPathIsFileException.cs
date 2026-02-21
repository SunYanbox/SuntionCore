namespace SuntionCore.Services.LogUtils.Exceptions;

public class InputPathIsFileException(string path)
    : Exception($"传入的路径不是文件夹路径, 而是一个文件: \"{path}\"");