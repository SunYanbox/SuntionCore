## MagnitudeFormatter - Public API

#### MagnitudeFormatter
```csharp
/// <summary>
/// A general-purpose magnitude numeric formatter.
/// </summary>
public static class MagnitudeFormatter
{
    /// <param name="steps">
    /// Array of magnitude step bases.<br />
    /// Length should be units.Length - 1.
    /// </param>
    /// <param name="units">Array of units (e.g., ["B", "KB", "MB"] or ["s", "min", "h"])</param>
    /// <param name="decimalPlaces">Number of decimal places to retain</param>
    /// <param name="addSpace">Whether to add a space between the value and the unit (default: true)</param>
    /// <remarks>Values less than 1 will be returned directly!!!</remarks>
    public static string Format(
        double value,
        string[] units,
        double[] steps,
        int decimalPlaces = 2,
        bool addSpace = true);
    
    /// <param name="units">Array of units (e.g., ["B", "KB", "MB"] or ["s", "min", "h"])</param>
    /// <param name="decimalPlaces">Number of decimal places to retain</param>
    /// <param name="addSpace">Whether to add a space between the value and the unit (default: true)</param>
    /// <param name="fixedStep">
    /// The fixed step size used when the 'steps' array is not provided.
    /// This implementation prioritizes the 'steps' array; if 'steps' is empty, it falls back to 'fixedStep'.
    /// </param>
    /// <remarks>Values less than 1 will be returned directly!!!</remarks>
    public static string Format(
        double value,
        string[] units,
        int fixedStep = 1000,
        int decimalPlaces = 2,
        bool addSpace = true);
    
    /// <param name="magnitude">Integrated unit and step data</param>
    /// <param name="decimalPlaces">Number of decimal places to retain</param>
    /// <param name="addSpace">Whether to add a space between the value and the unit (default: true)</param>
    /// <remarks>Values less than 1 will be returned directly!!!</remarks>
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
    
    /// <summary> Units </summary>
    public string[] Units { get; init; }
    /// <summary> Variable steps </summary>
    public double[]? Steps { get; init; }
    /// <summary> Fixed step </summary>
    public double FixedStep { get; init; }
    /// <summary> Flag indicating whether variable steps are used </summary>
    public bool IsVariable { get; init; }

    /// <summary>
    /// [Factory Method] Creates a fixed-step system.
    /// </summary>
    public static MagnitudeConfig CreateFixed(string[] units, double step);

    /// <summary>
    /// [Factory Method] Creates a variable-step system.
    /// </summary>
    public static MagnitudeConfig CreateVariable(string[] units, double[] steps);
    
    public override string ToString();
}
```

#### MagnitudePreset

```csharp
/// <summary> Presets for common magnitude units </summary>
public static class MagnitudePreset
{
    /// <summary> File size units (Binary, base 1024) </summary>
    [UsedImplicitly] public static readonly MagnitudeConfig FileSizeBinary;

    /// <summary> Disk capacity / Network traffic units (Decimal, base 1000, compliant with SI standards) </summary>
    /// <remarks>Hard drive manufacturers and ISPs typically use base 1000 (KB=1000B), not 1024.</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig DataSizeDecimal;

    /// <summary> Time units (Variable steps: 60, 60, 24, 7) </summary>
    /// <remarks>Seconds -> Minutes -> Hours -> Days -> Weeks</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig TimeStandard;

    /// <summary> Time units (High precision, includes milliseconds) </summary>
    /// <remarks>Milliseconds -> Seconds -> Minutes -> Hours -> Days</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig TimeHighPrecision;

    /// <summary> Frequency units (Hertz, base 1000) </summary>
    [UsedImplicitly] public static readonly MagnitudeConfig Frequency;

    /// <summary> Count magnitude abbreviations (Thousand/Million/Billion) </summary>
    /// <remarks>Commonly used for displaying user counts, likes, etc. (e.g., 1.5K, 2.3M)</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig CountAbbreviation;

    /// <summary> Computer memory / Bandwidth rate (Bitrate, base 1000) </summary>
    /// <remarks>Network speeds typically use bps, Kbps, Mbps (base 1000)</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig Bitrate;
    
    /// <summary> Length units (Metric, base 1000) </summary>
    [UsedImplicitly] public static readonly MagnitudeConfig LengthMetric;
        
    /// <summary> Length units (Imperial, variable steps) </summary>
    /// <remarks>Inches -> Feet (12) -> Yards (3) -> Miles (1760)</remarks>
    [UsedImplicitly] public static readonly MagnitudeConfig LengthImperial;
}
```

## FileSizeUtil - Public API

```csharp
public static class FileSizeUtil
{
    /// <summary>
    /// Calculates the file size at the specified path.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    /// <exception cref="GetFileSizeException"></exception>
    public static long CalFileSize(string filePath);
    
    /// <summary>
    /// Formats a byte count into a human-readable file size.
    /// </summary>
    public static string GetFileSize(string filePath, int decimalPlaces = 2);
}
```

## I18N - Public API

```csharp
/// <summary>
/// An I18N class managing all language-to-translation dictionaries.
///
/// Requires setting the DatabaseServer object before first use.
/// </summary>
public class I18N
{
    #region Static
    /// <summary>
    /// The key for the connector string translation in a specified language.
    /// <remarks>e.g., uses '' (empty) for Chinese, ' ' (space) for English</remarks>
    /// </summary>
    public const string LinkTagKey = "LinkTag";
    /// <summary> Providing this symbol in a key allows concatenating multiple key values into a single translation. </summary>
    public const string LinkKeySymbol = "::";
    /// <summary> Gets an I18N instance by name. </summary>
    [UsedImplicitly]
    public static I18N? GetI18N(string name) => I18NStatics.GetValueOrDefault(name);
    /// <summary> Gets an I18N instance by name. <br /> Creates a new instance if one with the specified name does not exist. </summary>
    [UsedImplicitly]
    public static I18N GetOrCreateI18N(string name) => I18NStatics.GetValueOrDefault(name) ?? new I18N(name);
    /// <summary> Data Server </summary>
    public static DatabaseServer? DatabaseServer { get; set; }
    /// <summary>
    /// Initializes the DatabaseServer.
    /// </summary>
    [UsedImplicitly] public static void Initialize(DatabaseServer databaseServer);
    #endregion

    #region Instance

    public I18N(string name);
    /// <summary> Default separator for the I18N instance. </summary>
    public string DefaultLinkTag { get; [UsedImplicitly] set; } = " ";
    /// <summary> Separator for the current language. </summary>
    [UsedImplicitly] public string LinkTag => _linkTags.GetValueOrDefault(CurrentLang, DefaultLinkTag);
    /// <summary> Name. </summary>
    public string Name { get; private set; }
    /// <summary> Gets the SPT translation database corresponding to the current language. </summary>
    public Dictionary<string, string>? SptLocals => _sptLocals ??= GetSptLocals();
    /// <summary> Read-only property to view supported languages. </summary>
    [UsedImplicitly] public List<string> AvailableLang => _i18N.Keys.ToList();
    /// <summary> Current language. </summary>
    /// <exception cref="I18NNameAlreadyExistException"></exception>
    /// <exception cref="NotLoadLanguageException"></exception>
    public string CurrentLang { get; set; }

    /// <summary>
    /// Loads all language files with the format 'xx.json' from the specified folder.
    /// </summary>
    /// <exception cref="LoadLocalDBException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    [UsedImplicitly] public void LoadFolders(string path);

    /// <summary>
    /// Adds a translation entry for a specified language.
    /// </summary> 
    public void Add(string lang, string key, string value);

    /// <summary>
    /// Removes the translation data for a specific key in a specified language.
    /// </summary>
    [UsedImplicitly] public void Remove(string lang, string key);
    
    /// <summary>
    /// Removes all data for a specified language.
    /// </summary>
    [UsedImplicitly] public void Remove(string lang);
    
    /// <summary>
    /// Extends translation information for a specified language via a dictionary (overwrites existing keys).
    /// </summary>
    /// <returns>Set of overwritten keys</returns>
    /// <exception cref="IllegalTranslationKeyException"></exception>
    [UsedImplicitly] public HashSet<string> Expand(string lang, Dictionary<string, string> value);

    /// <summary>
    /// Adds a translation entry for the current language.
    /// </summary>
    [UsedImplicitly] public void Add(string key, string value);

    /// <summary>
    /// Gets a formatted string with parameters.
    /// <remarks>Parameters in the string must be in the {{variable_name}} format.</remarks>
    /// </summary>
    /// <param name="msg">Formatted string with parameters</param>
    /// <param name="args">Parameter object</param>
    public static string DumpFormatStrLocal(string msg, object args);
    
    /// <summary>
    /// Gets localized text.
    /// </summary>
    /// <param name="key">Localization key</param>
    /// <param name="args">Optional parameter object; properties will replace {{property_name}} in the string</param>
    /// <remarks>Multiple key translations can be concatenated using :: <br/> e.g., "key1::key2" </remarks>
    /// <returns>The localized string</returns>
    public string Translate(string key, object? args = null);
}
```

## ModLogger - Public API

```csharp
/// <summary> Strategy for ModLogger instance log recording. </summary>
public enum ModLoggerStrategy
{
    /// <summary> Single file logging. </summary>
    SingleFile,
    /// <summary> Multi-file logging. </summary>
    MultiFile
}
```

```csharp
/// <summary>
/// Enables mods to record detailed logs within the mod folder.
/// <br />
/// Facilitates testing/debugging.
/// Prevents cluttering the SPT server logs.
/// </summary>
public class ModLogger : IDisposable
{
    #region Static
    public const string DefaultLogFolderPath = "user/mods/SuntionCore/ModLogs";
    /// <summary> Sets the default maximum log file size. The original file is deleted when this limit is exceeded. <remarks>Used when the logFileMaxSize parameter is set to 0.</remarks> </summary>
    public static long TotalDefaultLogFileMaxSize { get; [UsedImplicitly] set; } = 1 * 1024 * 1024;
    [UsedImplicitly] public static ModLogger? GetLogger(string name);
    /// <summary> Gets a ModLogger instance by name. <br /> Creates a new instance if one with the specified name does not exist. </summary>
    [UsedImplicitly]
    public static ModLogger GetOrCreateLogger(string name, ModLoggerStrategy strategy = ModLoggerStrategy.SingleFile,
        string folderPath = DefaultLogFolderPath, long logFileMaxSize = 0);
    /// <summary> Number of registered logger instances. </summary>
    public static long LoggerCount { get {
        lock (StaticLock)
        {
            return Loggers.Count;
        }
    } }
    /// <summary> Iterates over registered logger instances. </summary>
    public static Dictionary<string, ModLogger> Items { get {
        lock (StaticLock)
        {
            return Loggers;
        }
    } }

    #endregion

    #region Instance

    /// <summary> Initializes the instance. </summary>
    /// <param name="modName">Unique mod name or Guid</param>
    /// <param name="strategy">Log recording strategy</param>
    /// <param name="folderPath">Folder path for recording logs</param>
    /// <param name="logFileMaxSize">Sets the maximum log file size; the original file is deleted when exceeded. Unit: Bytes</param>
    /// <exception cref="ArgumentNullException">Error thrown when modName is not provided</exception>
    public ModLogger(string modName, ModLoggerStrategy strategy = ModLoggerStrategy.SingleFile, 
        string folderPath = DefaultLogFolderPath, long logFileMaxSize = 0);
    
    /// <summary> Mod name. </summary>
    public string ModName { get; }
    /// <summary> Folder path for recording logs. </summary>
    public string FolderPath { get; [UsedImplicitly] set; }
    /// <summary> Sets the maximum log file size; the original file is deleted when exceeded. </summary>
    public long LogFileMaxSize { get; [UsedImplicitly] set; }
    /// <summary> Log recording strategy. </summary>
    public ModLoggerStrategy Strategy { get; [UsedImplicitly] set; }
    
    /// <summary> Records a local log entry, then returns the log entry prefixed with the mod name. </summary>
    [UsedImplicitly] public string Info(string message, Exception? ex = null) => $"[{ModName}] {LogMessage(LogWriterStream.InfoStream, message, ex)}";
    /// <summary> Records a local log entry, then returns the log entry prefixed with the mod name. </summary>
    [UsedImplicitly] public string Warn(string message, Exception? ex = null) => $"[{ModName}] {LogMessage(LogWriterStream.WarningStream, message, ex)}";
    /// <summary> Records a local log entry, then returns the log entry prefixed with the mod name. </summary>
    [UsedImplicitly] public string Debug(string message, Exception? ex = null) => $"[{ModName}] {LogMessage(LogWriterStream.DebugStream, message, ex)}";
    /// <summary> Records a local log entry, then returns the log entry prefixed with the mod name. </summary>
    [UsedImplicitly] public string Error(string message, Exception? ex = null) => $"[{ModName}] {LogMessage(LogWriterStream.ErrorStream, message, ex)}";
    
    public void Dispose();
    
    #endregion
}
```
## ModMailService - Public API

```csharp
[Injectable(InjectionType.Singleton)]
public class ModMailService(
    ItemHelper itemHelper,
    ProfileHelper profileHelper,
    PaymentService paymentService,
    MailSendService mailSendService,
    EventOutputHolder eventOutputHolder
)
{
    /// <summary> Maximum character limit for a single message. </summary>
    public const int SendLimit = 490;
    
    /// <summary>
    /// Splits a string by newlines to prevent client display issues with long texts.
    /// </summary>
    public static string[] SplitStringByNewlines(string str);
    
    /// <summary>
    /// Sends a message to the client associated with the given sessionId via the registered chat bot.
    /// </summary>
    public void SendMessage(string sessionId, string message, UserDialogInfo chatBot);

    /// <summary>
    /// Asynchronously sends bulk messages to the client via the registered chat bot.
    /// <br />
    /// Automatically handles splitting long text to ensure compatibility with the client chat window.
    /// </summary>
    [UsedImplicitly] public async Task SendAllMessageAsync(string sessionId, string message, UserDialogInfo chatBot);
    
    /// <summary>
    /// Deducts a specified amount of currency from the player.
    /// </summary>
    /// <param name="sessionId">Player account ID / Session ID.</param>
    /// <param name="moneyId">Template ID of the currency type.</param>
    /// <param name="amount">Amount to deduct.</param>
    /// <param name="pmcData">If provided, this instance is used directly (ignoring sessionId); otherwise, lookup is performed via sessionId.</param>
    /// <exception cref="ArgumentException">Thrown when the currency template ID is not one of the following: Euros, Rubles, GP Coins, or USD.</exception>
    /// <returns>Returns a list of warnings if the operation fails partially or completely; otherwise null.</returns>
    [UsedImplicitly] public List<Warning>? Payment(MongoId sessionId, MongoId moneyId, long amount, PmcData? pmcData = null);

    /// <summary>
    /// Sends money to the player with the FIR (Found In Raid) status (though FIR status is typically irrelevant for currency).
    /// </summary>
    /// <param name="sessionId">Player account ID / Session ID.</param>
    /// <param name="moneyId">Template ID of the currency type.</param>
    /// <param name="msg">Message attached to the transaction.</param>
    /// <param name="amount">Amount to send.</param>
    /// <exception cref="ArgumentException">Thrown when the currency template ID is not one of the following: Euros, Rubles, GP Coins, or USD.</exception>
    /// <returns>Returns a list of warnings if the operation fails; otherwise null.</returns>
    [UsedImplicitly] public List<Warning>? SendMoney(MongoId sessionId, MongoId moneyId, string msg, double amount);

    /// <summary>
    /// Sends items to the player via the "System" account.
    /// </summary>
    /// <param name="sessionId">Player Session ID.</param>
    /// <param name="msg">Notification message.</param>
    /// <param name="items">List of items to send.</param>
    /// <param name="modGiveIsFIR">Whether the granted items should have FIR (Found In Raid) status.</param>
    /// <param name="maxStorageTimeSeconds">Maximum storage time in seconds (default: 2 days / 172800s).</param>
    /// <returns>Returns a list of warnings if the operation fails; otherwise null.</returns>
    public List<Warning>? SendItemsToPlayer(
        MongoId sessionId,
        string msg,
        List<Item>? items,
        bool modGiveIsFIR = true,
        long? maxStorageTimeSeconds = 172800L);
}
```