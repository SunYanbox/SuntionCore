namespace SuntionCore.Services.LogUtils;

/// <summary> ModLogger实例记录日志的策略 </summary>
public enum ModLoggerStrategy
{
    /// <summary> 单文件日志 </summary>
    SingleFile,
    /// <summary> 多文件日志 </summary>
    MultiFile
}