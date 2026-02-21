using System.Text;
using JetBrains.Annotations;
using SuntionCore.Services.FileUtils;
using SuntionCore.Services.LogUtils.Exceptions;

namespace SuntionCore.Services.LogUtils;

enum LogWriterStream
{
    SingleFileStream,
    InfoStream,
    ErrorStream,
    WarningStream,
    DebugStream
}

/// <summary>
/// 用于使得模组在模组文件夹下记录详细日志
/// <br />
/// 方便测试 / 调试
/// 避免SPT服务器的日志过于繁琐
/// </summary>
public class ModLogger: IDisposable
{
    #region 静态
    private static readonly Lock StaticLock = new();
    public const string DefaultLogFolderPath = "user/mods/SuntionCore/ModLogs";
    private static readonly Dictionary<string, ModLogger> Loggers = new();
    /// <summary> 设置默认日志文件大小上限, 超出上限后删除原文件 <remarks>当logFileMaxSize参数设置为0时使用</remarks> </summary>
    public static long TotalDefaultLogFileMaxSize { get; [UsedImplicitly] set; } = 1 * 1024 * 1024;
    [UsedImplicitly]
    public static ModLogger? GetLogger(string name)
    {
        lock (StaticLock){
            return Loggers.GetValueOrDefault(name);
        }
    }
    /// <summary> 根据ModLogger的名称获取ModLogger实例 <br /> 如果指定名称的实例不存在时创建新的 </summary>
    [UsedImplicitly]
    public static ModLogger GetOrCreateLogger(string name, ModLoggerStrategy strategy = ModLoggerStrategy.SingleFile,
        string folderPath = DefaultLogFolderPath, long logFileMaxSize = 0)
    {
        lock (StaticLock)
        {
            return Loggers.GetValueOrDefault(name) ?? new ModLogger(name, strategy, folderPath, logFileMaxSize);
        }
    }
    /// <summary> 注册的日志实例数量 </summary>
    public static long LoggerCount { get {
        lock (StaticLock)
        {
            return Loggers.Count;
        }
    } }
    /// <summary> 遍历注册的日志实例 </summary>
    public static Dictionary<string, ModLogger> Items { get {
        lock (StaticLock)
        {
            return Loggers;
        }
    } }

    #endregion

    #region 实例

    /// <summary> 初始化实例 </summary>
    /// <param name="modName">模组唯一名称或者Guid</param>
    /// <param name="strategy">记录日志的策略</param>
    /// <param name="folderPath">记录日志的文件夹路径</param>
    /// <param name="logFileMaxSize">设置日志文件大小上限, 超出上限后删除原文件 单位: B</param>
    /// <exception cref="ArgumentNullException">没有提供modName时的错误</exception>
    public ModLogger(string modName, ModLoggerStrategy strategy = ModLoggerStrategy.SingleFile, 
        string folderPath = DefaultLogFolderPath, long logFileMaxSize = 0)
    {
        ModName = modName ?? throw new ArgumentNullException(nameof(modName));
        Strategy = strategy;
        LogFileMaxSize = logFileMaxSize > 0 ? logFileMaxSize : logFileMaxSize == 0 ? TotalDefaultLogFileMaxSize : -1;
        FolderPath = Path.GetFullPath(folderPath);
        if (Loggers.ContainsKey(modName)) throw new ModLoggerNameAlreadyExistException(modName);
        if (File.Exists(folderPath)) throw new InputPathIsFileException(folderPath);
        if (!Directory.Exists(FolderPath))
        {
            Directory.CreateDirectory(FolderPath);
        }
        Loggers.Add(modName, this);
        _disposed = false;
    }
    
    /// <summary> 模组名称 </summary>
    public string ModName { get; }
    /// <summary> 记录日志的文件夹路径 </summary>
    public string FolderPath { get; [UsedImplicitly] set; }
    /// <summary> 设置日志文件大小上限, 超出上限后删除原文件 </summary>
    public long LogFileMaxSize { get; [UsedImplicitly] set; }
    /// <summary> 日志记录策略 </summary>
    public ModLoggerStrategy Strategy { get; [UsedImplicitly] set; }
    
    /// <summary> 记录一条本地日志, 随后返回使用模组名称修饰的日志条目 </summary>
    [UsedImplicitly] public string Info(string message, Exception? ex = null) => $"[{ModName}] {LogMessage(LogWriterStream.InfoStream, message, ex)}";
    /// <summary> 记录一条本地日志, 随后返回使用模组名称修饰的日志条目 </summary>
    [UsedImplicitly] public string Warn(string message, Exception? ex = null) => $"[{ModName}] {LogMessage(LogWriterStream.WarningStream, message, ex)}";
    /// <summary> 记录一条本地日志, 随后返回使用模组名称修饰的日志条目 </summary>
    [UsedImplicitly] public string Debug(string message, Exception? ex = null) => $"[{ModName}] {LogMessage(LogWriterStream.DebugStream, message, ex)}";
    /// <summary> 记录一条本地日志, 随后返回使用模组名称修饰的日志条目 </summary>
    [UsedImplicitly] public string Error(string message, Exception? ex = null) => $"[{ModName}] {LogMessage(LogWriterStream.ErrorStream, message, ex)}";
    
    public void Dispose()
    {
        if (_disposed) return;

        lock (_lock)
        {
            foreach (StreamWriter writer in _logWriters.Values)
            {
                try
                {
                    writer.Flush();
                    writer.Close();
                    writer.Dispose();
                }
                catch { /* Ignore disposal errors */ }
            }
            _logWriters.Clear();
            
            // 从静态字典移除自身 (可选，视生命周期管理策略而定)
            lock (StaticLock)
            {
                Loggers.Remove(ModName);
            }
        }

        _disposed = true;
    }
    
    #endregion

    #region 私有方法

    private string LogMessage(LogWriterStream type, string msg, Exception? ex = null)
    {
        if (_disposed) return $"ModLogger({ModName})已销毁";
        if (type == LogWriterStream.SingleFileStream)
        {
            throw new ArgumentOutOfRangeInThereException(
                nameof(LogWriterStream.SingleFileStream),
                nameof(LogWriterStream.SingleFileStream),
                "ModLogger.LogMessage",
                "InfoStream+ErrorStream+WarningStream+DebugStream");
        }
        lock (_lock)
        {
            StreamWriter sw = GetLogWriter(type);
            msg += type == LogWriterStream.DebugStream ? "\n" : string.Empty;
            string message = msg + (ex is null ? "" : $"{ex.GetType().Name}({ex.Message}, {ex.StackTrace})");
            sw.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {GetLogType(type)} - {message}");
            sw.Flush();
            return message;
        }
    }

    private static string GetLogType(LogWriterStream type)
    {
        return type switch
        {
            LogWriterStream.SingleFileStream => nameof(LogWriterStream.SingleFileStream),
            LogWriterStream.InfoStream => "Info",
            LogWriterStream.ErrorStream => "Error",
            LogWriterStream.WarningStream => "Warning",
            LogWriterStream.DebugStream => "Debug",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }

    /// <summary> 如果文件尺寸过大则删除 </summary>
    private void DeleteIfFileTooBig(string filePath)
    {
        if (!File.Exists(filePath)) return;
        if (FileSizeUtil.CalFileSize(filePath) <= LogFileMaxSize) return;
        
        File.Delete(filePath);
    }

    /// <summary> 获取写入日志流 </summary>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private StreamWriter GetLogWriter(LogWriterStream streamType)
    {
        switch (Strategy)
        {
            case ModLoggerStrategy.SingleFile:
                lock (_lock)
                {
                    if (_logWriters.TryGetValue(LogWriterStream.SingleFileStream, out StreamWriter? value)) return value;
                    DeleteIfFileTooBig(SingleLogPath);
                    value = CreateLogWriter(SingleLogPath);
                    _logWriters[LogWriterStream.SingleFileStream] = value;
                    return value;
                }
            case ModLoggerStrategy.MultiFile:
                lock (_lock)
                {
                    if (_logWriters.TryGetValue(streamType, out StreamWriter? value)) return value;
                    string logPath;
                    switch (streamType)
                    {
                        case LogWriterStream.InfoStream:
                            logPath = InfoLogPath;
                            break;
                        case LogWriterStream.ErrorStream:
                            logPath = ErrorLogPath;
                            break;
                        case LogWriterStream.WarningStream:
                            logPath = WarningLogPath;
                            break;
                        case LogWriterStream.DebugStream:
                            logPath = DebugLogPath;
                            break;
                        default:
                            throw new ArgumentOutOfRangeInThereException(
                                nameof(streamType),
                                streamType.ToString(),
                                "ModLogger.GetLogWriter",
                                "InfoStream+ErrorStream+WarningStream+DebugStream");
                    }
                    DeleteIfFileTooBig(logPath);
                    value = CreateLogWriter(logPath);
                    _logWriters[streamType] = value;
                    return value;
                }
            default:
                throw new ArgumentOutOfRangeInThereException(
                    nameof(Strategy), 
                    Strategy.ToString(), 
                    $"ModLogger.GetLogWriter",
                    "SingleFile+MultiFile");
        }
    }

    private static StreamWriter CreateLogWriter(string logFilePath)
    {
        return new StreamWriter(
            new FileStream(logFilePath, FileMode.Append, FileAccess.Write, FileShare.Read), 
            new UTF8Encoding(), 
            1024, 
            false);
    }

    #endregion
    
    #region 私有属性
    private bool _disposed;
    private readonly Lock _lock = new();
    private readonly Dictionary<LogWriterStream, StreamWriter> _logWriters = new();
    private string SingleLogPath => Path.Combine(FolderPath, $"{ModName}.log");
    private string InfoLogPath => Path.Combine(FolderPath, $"{ModName}.info.log");
    private string ErrorLogPath => Path.Combine(FolderPath, $"{ModName}.error.log");
    private string WarningLogPath => Path.Combine(FolderPath, $"{ModName}.warn.log");
    private string DebugLogPath => Path.Combine(FolderPath, $"{ModName}.debug.log");
    
    #endregion

}