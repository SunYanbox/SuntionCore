using System.Reflection;
using System.Text.Json;
using JetBrains.Annotations;
using SPTarkov.Common.Extensions;
using SPTarkov.Server.Core.Servers;
using SuntionCore.Services.I18NUtil.Exceptions;

namespace SuntionCore.Services.I18NUtil;

/// <summary>
/// 一个I18N类管理所有语言->翻译字典
///
/// 首次使用需要设置DatabaseServer对象
/// </summary>
public class I18N
{
    #region 静态
    private const string DefaultLang = "ch";
    /// <summary>
    /// 指定语言中用于连接的字符串译文的键
    /// <remarks>例如中文用''分隔, 英文用' '分隔</remarks>
    /// </summary>
    public const string LinkTagKey = "LinkTag";
    /// <summary> 在键中提供该符号, 可以连接多个键的值为一条译文 </summary>
    public const string LinkKeySymbol = "::";
    private static readonly Dictionary<string, I18N> I18NStatics = new();
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
    [UsedImplicitly]
    public static void Initialize(DatabaseServer databaseServer)
    {
        DatabaseServer ??= databaseServer;
    }
    /// <summary> 是否是创建时的非法键 <remarks>使用时可以用</remarks>> </summary>
    private static bool IsIllegalKey(string key) => key.Contains(LinkKeySymbol) || key.Contains(' ');
    #endregion

    #region 实例

    public I18N(string name)
    {
        Name = name;
        if (_i18N.ContainsKey(name))
        {
            throw new I18NNameAlreadyExistException(name);
        }

        I18NStatics.Add(name, this);
    }
    
    private readonly Dictionary<string, Dictionary<string, string>> _i18N = new();
    /// <summary> I18N实例默认分隔符 </summary>
    public string DefaultLinkTag { get; [UsedImplicitly] set; } = " ";
    /// <summary> 语言->语言分隔符  </summary>
    private readonly Dictionary<string, string> _linkTags = new();
    private string _currentLang = DefaultLang;
    private Dictionary<string, string>? _sptLocals;
    /// <summary> 当前语言的分隔符 </summary>
    [UsedImplicitly] public string LinkTag => _linkTags.GetValueOrDefault(CurrentLang, DefaultLinkTag);
    /// <summary> 名称 </summary>
    public string Name { get; private set; }
    /// <summary> 已加载的缓存字典 </summary>
    private Dictionary<string, string>? CurrentI18NCache { get; set; }
    /// <summary> 获取当前语言对应的SPT译文数据库 </summary>
    public Dictionary<string, string>? SptLocals => _sptLocals ??= GetSptLocals();
    /// <summary> 只读属性, 查看支持的语言 </summary>
    [UsedImplicitly]
    public List<string> AvailableLang => _i18N.Keys.ToList();
    /// <summary>
    /// 当前语言
    /// </summary>
    /// <exception cref="I18NNameAlreadyExistException"></exception>
    /// <exception cref="NotLoadLanguageException"></exception>
    public string CurrentLang
    {
        get => _currentLang;
        [UsedImplicitly]
        set
        {
            if (value.Length != 2) throw new I18NNameAlreadyExistException(value);
            if (!_i18N.TryGetValue(value, out Dictionary<string, string>? cache))
            {
                throw new NotLoadLanguageException($"(语言 '{value}' 没有加载)");
            }
            _currentLang = value;
            _sptLocals = null;
            CurrentI18NCache = cache;
        }
    }

    /// <summary>
    /// 加载指定文件夹下所有格式为'xx.json'格式的语言文件
    /// </summary>
    /// <exception cref="LoadLocalDBException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
    [UsedImplicitly]
    public void LoadFolders(string path)
    {
        if (Directory.Exists(path))
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = false, // 大小写敏感
                ReadCommentHandling = JsonCommentHandling.Skip, // 允许 JSON 中有注释
                AllowTrailingCommas = true // 允许末尾逗号
            };
            
            foreach (string file in Directory.GetFiles(path))
            {
                // eg: ch.json

                string fileName = Path.GetFileNameWithoutExtension(file);
                if (!file.EndsWith(".json") || fileName.Length != 2) continue;
                try
                {
                    string jsonContent = File.ReadAllText(file);
                    var trans = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonContent, options);

                    if (trans is not null)
                    {
                        Expand(fileName, trans);
                    }
                }
                catch (Exception e)
                {
                    throw new LoadLocalDBException(fileName, e);
                }
            }
        }
        else
        {
            throw new DirectoryNotFoundException(path);
        }
    }

    /// <summary> 如果语言不存在则创建新实例 </summary>
    private Dictionary<string, string> GetOrCreate(string lang)
    {
        if (_i18N.TryGetValue(lang, out Dictionary<string, string>? value1)) return value1;
        value1 = new Dictionary<string, string>();
        _i18N.Add(lang, value1);
        return value1;
    }

    /// <summary>
    /// 为指定语言添加一条译文
    /// </summary>
    public void Add(string lang, string key, string value)
    {
        if (IsIllegalKey(key))
            throw new IllegalTranslationKeyException(key);
        GetOrCreate(lang)[key] = value;
        if (key == LinkTagKey)
        {
            _linkTags[lang] = value;
        }
    }

    /// <summary>
    /// 删除指定语言的指定一个键的翻译数据
    /// </summary>
    [UsedImplicitly]
    public void Remove(string lang, string key) => GetOrCreate(lang).Remove(key);
    
    /// <summary>
    /// 删除指定语言的所有数据
    /// </summary>
    [UsedImplicitly]
    public void Remove(string lang) => _i18N.Remove(lang);
    
    /// <summary>
    /// 通过字典扩展指定语言的翻译信息(覆盖已存在键)
    /// </summary>
    /// <returns>覆盖过的键的集合</returns>
    /// <exception cref="IllegalTranslationKeyException"></exception>
    [UsedImplicitly]
    public HashSet<string> Expand(string lang, Dictionary<string, string> value)
    {
        HashSet<string> coverageKeys = [];
        if (value.Count == 0) return coverageKeys; // 快速跳过
        Dictionary<string, string> cache = GetOrCreate(lang);
        if (value.TryGetValue(LinkTagKey, out string? linkTag))
        {
            _linkTags[lang] = linkTag;
        }

        HashSet<string> illegalKeys = [];
        foreach ((string key, string item) in value)
        {
            if (IsIllegalKey(key))
            {
                illegalKeys.Add(key);
                continue;
            }
            if (cache.ContainsKey(key)) coverageKeys.Add(key);
            cache[key] = item;
        }
        return illegalKeys.Count > 0 
            ? throw new IllegalTranslationKeyException(string.Join(", ", illegalKeys)) 
            : coverageKeys;
    }

    /// <summary>
    /// 为当前语言添加一条译文
    /// </summary>
    [UsedImplicitly]
    public void Add(string key, string value) => Add(CurrentLang, key, value);

    /// <summary>
    /// 获取带参数的格式化字符串
    /// <remarks>字符串中的参数必须是{{变量名}}格式</remarks>
    /// </summary>
    /// <param name="msg">有参数格式化字符串</param>
    /// <param name="args">参数对象</param>
    public static string DumpFormatStrLocal(string msg, object args)
    {
        string localMsg = msg;
        PropertyInfo[] typeProperties = args.GetType().GetProperties();

        foreach (PropertyInfo propertyInfo in typeProperties)
        {
            // 获取JSON属性名（支持[JsonProperty]特性）
            string localizedName = $"{{{{{propertyInfo.GetJsonName()}}}}}";
            if (localMsg.Contains(localizedName))
            {
                localMsg = localMsg.Replace(
                    localizedName,
                    propertyInfo.GetValue(args)?.ToString() ?? string.Empty
                );
            }
        }
        
        return localMsg;
    }
    
    /// <summary>
    /// 获取本地化文本
    /// </summary>
    /// <param name="key">本地化键</param>
    /// <param name="args">可选参数对象，属性将替换字符串中的{{属性名}}</param>
    /// <remarks>可以使用 :: 连接多个键的译文 <br/> 例如 "key1::key2" </remarks>
    /// <returns>本地化后的字符串</returns>
    public string Translate(string key, object? args = null)
    {
        if (!key.Contains(LinkKeySymbol))
        {
            return UnitTranslate(key, args);
        }

        string[] keys = key.Replace(" ", "").Split([LinkKeySymbol], StringSplitOptions.RemoveEmptyEntries);

        // 获取当前语言的分隔符
        string separator = _linkTags.GetValueOrDefault(CurrentLang, DefaultLinkTag);

        // 连接所有译文片段
        string fullText = string.Join(separator, keys.Select(GetLocalisedValue));

        // 如果有参数，对连接后的整句进行格式化
        if (args != null)
        {
            fullText = DumpFormatStrLocal(fullText, args);
        }
            
        return fullText;
    }

    /// <summary>
    /// 获取本地化文本
    /// </summary>
    /// <param name="key">本地化键</param>
    /// <param name="args">可选参数对象，属性将替换字符串中的{{属性名}}</param>
    /// <returns>本地化后的字符串</returns>
    private string UnitTranslate(string key, object? args = null)
    {
        return args is null ? GetLocalisedValue(key) : GetLocalised(key, args);
    }
    
    /// <summary>
    /// 处理带参数的本地化字符串
    /// 将{{属性名}}替换为参数对象的属性值
    /// </summary>
    private string GetLocalised(string key, object? args)
    {
        string rawLocalizedString = GetLocalisedValue(key);
        return args == null ? rawLocalizedString : DumpFormatStrLocal(rawLocalizedString, args);
    }
    
    /// <summary>
    /// 获取本地化值的核心方法
    /// 实现多级回退：当前语言 -> 回退到中文 -> 获取SPT译文 -> 报错加key的值
    /// </summary>
    /// <param name="key">译文的键</param>
    /// <exception cref="NotLoadLanguageException"></exception>
    private string GetLocalisedValue(string key)
    {
        var errorKey = $"[Error {key}]";
        if (CurrentI18NCache is null)
        {
            if (_i18N.TryGetValue(CurrentLang, out Dictionary<string, string>? cache))
            {
                CurrentI18NCache = cache;
            }
            else if (_i18N.TryGetValue(DefaultLang, out Dictionary<string, string>? chCache))
            {
                CurrentI18NCache = chCache;
            }
            else
            {
                if (SptLocals is not null && SptLocals.TryGetValue(key, out string? translation))
                {
                    return translation;
                }
                
                throw new NotLoadLanguageException();
            }
        }

        if (CurrentI18NCache?.TryGetValue(key, out string? value) ?? false)
        {
            return value;
        }
        return errorKey;
    }
    
    /// <summary>
    /// 获取Spt本地化字典
    /// </summary>
    private Dictionary<string, string>? GetSptLocals()
    {
        _sptLocals = DatabaseServer?.GetTables().Locales.Global[CurrentLang].Value;
        return _sptLocals;
    }
    #endregion
}