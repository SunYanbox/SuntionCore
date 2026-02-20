namespace SuntionCore.Services.I18NUtil.Exceptions;

public class NotLoadLanguageException(string msg = ""): Exception($"没有加载语言任何语言 {msg}");