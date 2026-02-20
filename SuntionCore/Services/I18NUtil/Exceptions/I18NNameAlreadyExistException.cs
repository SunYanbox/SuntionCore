namespace SuntionCore.Services.I18NUtil.Exceptions;

public class I18NNameAlreadyExistException(string name) : Exception(HandleName(name))
{
    private static string HandleName(string name) => $"I18N已存在 '{name}' 名称的实例, 无法重新注册";
}