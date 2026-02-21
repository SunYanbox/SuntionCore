namespace SuntionCore.Services.LogUtils.Exceptions;

public class ModLoggerNameAlreadyExistException(string modName) : Exception(HandleName(modName))
{
    private static string HandleName(string name) => $"ModLogger已存在 '{name}' 名称的实例, 无法重新注册";
}