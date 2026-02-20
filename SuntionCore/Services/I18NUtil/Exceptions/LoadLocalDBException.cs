namespace SuntionCore.Services.I18NUtil.Exceptions;

public class LoadLocalDBException(string fileName, Exception inner): Exception($"加载本地化数据库'{fileName}'时出错", inner);