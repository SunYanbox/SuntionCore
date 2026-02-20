namespace SuntionCore.Services.I18NUtil.Exceptions;

public class LangNameNotMatchSPTException(string lang) : Exception($"语言名称 '{lang}' 不匹配SPT的Locals字典");