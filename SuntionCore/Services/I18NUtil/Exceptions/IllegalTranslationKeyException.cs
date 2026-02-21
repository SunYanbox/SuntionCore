namespace SuntionCore.Services.I18NUtil.Exceptions;

public class IllegalTranslationKeyException(string key, string linkKeySymbol = I18N.LinkKeySymbol)
    : Exception($"非法的键: \"{key}\", 创建键时禁止包含\"{linkKeySymbol}\"符号或空格符号");