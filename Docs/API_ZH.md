## MagnitudeFormatter - 公有接口

#### MagnitudeFormatter
```csharp
/// <summary>
/// 通用的量级数值格式化器
/// </summary>
public static class MagnitudeFormatter
{
    /// <param name="steps">
    /// 量级间隔基数数组。<br />
    /// 长度应为 units.Length - 1。
    /// </param>
    /// <param name="units">单位数组 (例如：["B", "KB", "MB"] 或 ["s", "min", "h"])</param>
    /// <param name="decimalPlaces">保留小数位数</param>
    /// <param name="addSpace">数值和单位之间是否添加空格 (默认 true)</param>
    /// <remarks>小于1的数会直接返回!!!</remarks>
    public static string Format(
        double value,
        string[] units,
        double[] steps,
        int decimalPlaces = 2,
        bool addSpace = true);
    
    /// <param name="units">单位数组 (例如：["B", "KB", "MB"] 或 ["s", "min", "h"])</param>
    /// <param name="decimalPlaces">保留小数位数</param>
    /// <param name="addSpace">数值和单位之间是否添加空格 (默认 true)</param>
    /// <param name="fixedStep">
    /// 当 steps 未提供时使用的固定步长。
    /// 本实现优先使用 steps 数组，若 steps 为空则回退到 fixedStep。
    /// </param>
    /// <remarks>小于1的数会直接返回!!!</remarks>
    public static string Format(
        double value,
        string[] units,
        int fixedStep = 1000,
        int decimalPlaces = 2,
        bool addSpace = true);
    
    /// <param name="magnitude">整合的单位与步长数据</param>
    /// <param name="decimalPlaces">保留小数位数</param>
    /// <param name="addSpace">数值和单位之间是否添加空格 (默认 true)</param>
    /// <remarks>小于1的数会直接返回!!!</remarks>
    public static string Format(
        double value,
        MagnitudeConfig magnitude,
        int decimalPlaces = 2,
        bool addSpace = true);
}
```

#### MagnitudeConfig

```csharp
public record MagnitudeConfig
{
    private MagnitudeConfig(string[] units, double[]? steps, double fixedStep, bool isVariable);
    
    /// <summary> 单位 </summary>
    public string[] Units { get; init; }
    /// <summary> 可变步长 </summary>
    public double[]? Steps { get; init; }
    /// <summary> 固定步长 </summary>
    public double FixedStep { get; init; }
    /// <summary> 是否是可变步长的标志 </summary>
    public bool IsVariable { get; init; }

    /// <summary>
    /// 【工厂方法】创建固定步长系统
    /// </summary>
    public static MagnitudeConfig CreateFixed(string[] units, double step);

    /// <summary>
    /// 【工厂方法】创建可变步长系统
    /// </summary>
    public static MagnitudeConfig CreateVariable(string[] units, double[] steps);
    
    public override string ToString();
}
```

#### MagnitudePreset

```csharp
/// <summary> 常用数量级单位预设 </summary>
public static class MagnitudePreset
{
    /// <summary> 文件尺寸单位 (二进制，1024) </summary>
    [UsedImplicitly] public static readonly MagnitudeConfig FileSizeBinary;

    /// <summary> 磁盘容量/网络流量单位 (十进制，1000，符合 SI 标准) </summary>
    /// <remarks>硬盘厂商和网络运营商通常使用 1000 进制 (KB=1000B)，而非 1024。</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig DataSizeDecimal;

    /// <summary> 时间单位 (变步长：60, 60, 24, 7) </summary>
    /// <remarks>秒 -> 分 -> 时 -> 天 -> 周</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig TimeStandard;

    /// <summary> 时间单位 (高精度，含毫秒) </summary>
    /// <remarks>毫秒 -> 秒 -> 分 -> 时 -> 天</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig TimeHighPrecision;

    /// <summary> 频率单位 (赫兹，1000) </summary>
    [UsedImplicitly] public static readonly MagnitudeConfig Frequency;

    /// <summary> 数据数量级缩写 (千/百万/十亿) </summary>
    /// <remarks>常用于显示用户数、点赞数等 (e.g., 1.5K, 2.3M)</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig CountAbbreviation;

    /// <summary> 计算机内存/带宽速率 (比特率，1000) </summary>
    /// <remarks>网络速度通常用 bps, Kbps, Mbps (1000 进制)</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig Bitrate;
    
    /// <summary> 长度单位 (公制，1000) </summary>
    [UsedImplicitly] public static readonly MagnitudeConfig LengthMetric;
        
    /// <summary> 长度单位 (英制，变步长) </summary>
    /// <remarks>英寸 -> 英尺 (12) -> 码 (3) -> 英里 (1760)</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig LengthImperial;
}
```

## FileSizeUtil - 公有接口

```csharp
public static class FileSizeUtil
{
    /// <summary>
    /// 计算路径的文件的尺寸
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="GetFileSizeException"></exception>
    public static long CalFileSize(string filePath);
    
    /// <summary>
    /// 将字节数格式化为可读的文件大小
    /// </summary>
    public static string GetFileSize(string filePath, int decimalPlaces = 2);
}
```

## I18N - 公有接口

```csharp
/// <summary>
/// 一个I18N类管理所有语言->翻译字典
///
/// 首次使用需要设置DatabaseServer对象
/// </summary>
public class I18N
{
    #region 静态
    /// <summary>
    /// 指定语言中用于连接的字符串译文的键
    /// <remarks>例如中文用''分隔, 英文用' '分隔</remarks>
    /// </summary>
    public const string LinkTagKey = "LinkTag";
    /// <summary> 在键中提供该符号, 可以连接多个键的值为一条译文 </summary>
    public const string LinkKeySymbol = "::";
    /// <summary> 根据I18N的名称获取I18N实例 </summary>
    [UsedImplicitly]
    public static I18N? GetI18N(string name) => I18NStatics.GetValueOrDefault(name);
    /// <summary> 根据I18N的名称获取I18N实例 <br /> 如果指定名称的实例不存在时创建新的 </summary>
    [UsedImplicitly]
    public static I18N GetOrCreateI18N(string name) => I18NStatics.GetValueOrDefault(name) ?? new I18N(name);
    /// <summary> 数据服务器 </summary>
    public static DatabaseServer? DatabaseServer { get; set; }
    /// <summary>
    /// 初始化DatabaseServer
    /// </summary>
    [UsedImplicitly] public static void Initialize(DatabaseServer databaseServer);
    #endregion

    #region 实例

    public I18N(string name);
    /// <summary> I18N实例默认分隔符 </summary>
    public string DefaultLinkTag { get; [UsedImplicitly] set; } = " ";
    /// <summary> 当前语言的分隔符 </summary>
    [UsedImplicitly] public string LinkTag => _linkTags.GetValueOrDefault(CurrentLang, DefaultLinkTag);
    /// <summary> 名称 </summary>
    public string Name { get; private set; }
    /// <summary> 获取当前语言对应的SPT译文数据库 </summary>
    public Dictionary<string, string>? SptLocals => _sptLocals ??= GetSptLocals();
    /// <summary> 只读属性, 查看支持的语言 </summary>
    [UsedImplicitly] public List<string> AvailableLang => _i18N.Keys.ToList();
    /// <summary> 当前语言 </summary>
    /// <exception cref="I18NNameAlreadyExistException"></exception>
    /// <exception cref="NotLoadLanguageException"></exception>
    public string CurrentLang { get; set; }

    /// <summary>
    /// 加载指定文件夹下所有格式为'xx.json'格式的语言文件
    /// </summary>
    /// <exception cref="LoadLocalDBException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    [UsedImplicitly] public void LoadFolders(string path);

    /// <summary>
    /// 为指定语言添加一条译文
    /// </summary> public void Add(string lang, string key, string value);

    /// <summary>
    /// 删除指定语言的指定一个键的翻译数据
    /// </summary>
    [UsedImplicitly] public void Remove(string lang, string key);
    
    /// <summary>
    /// 删除指定语言的所有数据
    /// </summary>
    [UsedImplicitly] public void Remove(string lang);
    
    /// <summary>
    /// 通过字典扩展指定语言的翻译信息(覆盖已存在键)
    /// </summary>
    /// <returns>覆盖过的键的集合</returns>
    /// <exception cref="IllegalTranslationKeyException"></exception>
    [UsedImplicitly] public HashSet<string> Expand(string lang, Dictionary<string, string> value);

    /// <summary>
    /// 为当前语言添加一条译文
    /// </summary>
    [UsedImplicitly] public void Add(string key, string value);

    /// <summary>
    /// 获取带参数的格式化字符串
    /// <remarks>字符串中的参数必须是{{变量名}}格式</remarks>
    /// </summary>
    /// <param name="msg">有参数格式化字符串</param>
    /// <param name="args">参数对象</param>
    public static string DumpFormatStrLocal(string msg, object args);
    
    /// <summary>
    /// 获取本地化文本
    /// </summary>
    /// <param name="key">本地化键</param>
    /// <param name="args">可选参数对象，属性将替换字符串中的{{属性名}}</param>
    /// <remarks>可以使用 :: 连接多个键的译文 <br/> 例如 "key1::key2" </remarks>
    /// <returns>本地化后的字符串</returns>
    public string Translate(string key, object? args = null);
}
```

## ModLogger - 公有接口

```csharp
/// <summary> ModLogger实例记录日志的策略 </summary>
public enum ModLoggerStrategy
{
    /// <summary> 单文件日志 </summary>
    SingleFile,
    /// <summary> 多文件日志 </summary>
    MultiFile
}
```

```csharp
/// <summary>
/// 用于使得模组在模组文件夹下记录详细日志
/// <br />
/// 方便测试 / 调试
/// 避免SPT服务器的日志过于繁琐
/// </summary>
public class ModLogger: IDisposable
{
    #region 静态
    public const string DefaultLogFolderPath = "user/mods/SuntionCore/ModLogs";
    /// <summary> 设置默认日志文件大小上限, 超出上限后删除原文件 <remarks>当logFileMaxSize参数设置为0时使用</remarks> </summary>
    public static long TotalDefaultLogFileMaxSize { get; [UsedImplicitly] set; } = 1 * 1024 * 1024;
    [UsedImplicitly] public static ModLogger? GetLogger(string name);
    /// <summary> 根据ModLogger的名称获取ModLogger实例 <br /> 如果指定名称的实例不存在时创建新的 </summary>
    [UsedImplicitly]
    public static ModLogger GetOrCreateLogger(string name, ModLoggerStrategy strategy = ModLoggerStrategy.SingleFile,
        string folderPath = DefaultLogFolderPath, long logFileMaxSize = 0);
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
        string folderPath = DefaultLogFolderPath, long logFileMaxSize = 0);
    
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
    
    public void Dispose();
    
    #endregion
}
```
