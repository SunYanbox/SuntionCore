using System.Reflection;
using System.Text.Json;
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
    private static readonly Dictionary<string, I18N> I18NStatics = new();
    /// <summary> 根据I18N的名称获取I18N实例 </summary>
    public static I18N? GetInstance(string name) => I18NStatics.GetValueOrDefault(name);
    /// <summary> 数据服务器 </summary>
    public static DatabaseServer? DatabaseServer { get; set; }
    /// <summary>
    /// 初始化DatabaseServer
    /// </summary>
    public static void Initialize(DatabaseServer databaseServer)
    {
        DatabaseServer ??= databaseServer;
    }
    #endregion

    #region 实例

    public I18N(string name)
    {
        Name = name;
        if (_i18n.ContainsKey(name))
        {
            throw new I18NNameAlreadyExistException(name);
        }

        I18NStatics.Add(name, this);
    }
    
    private readonly Dictionary<string, Dictionary<string, string>> _i18n = new();
    private string _currentLang = DefaultLang;
    private Dictionary<string, string>? _sptLocals;
    /// <summary> 名称 </summary>
    public string Name { get; private set; }
    /// <summary> 已加载的缓存字典 </summary>
    public Dictionary<string, string>? CurrentI18NCache { get; private set; }
    /// <summary> 获取当前语言对应的SPT译文数据库 </summary>
    public Dictionary<string, string>? SptLocals => _sptLocals ??= GetSptLocals();
    /// <summary> 只读属性, 查看支持的语言 </summary>
    public List<string> AvailableLang => _i18n.Keys.ToList();
    /// <summary> 当前语言 </summary>
    public string CurrentLang
    {
        get => _currentLang;
        set
        {
            if (value.Length != 2) throw new I18NNameAlreadyExistException(value);
            if (!_i18n.TryGetValue(value, out Dictionary<string, string>? cache)) return;
            _currentLang = value;
            CurrentI18NCache = cache;
        }
    }

    /// <summary>
    /// 加载指定文件夹下所有格式为'xx.json'格式的语言文件
    /// </summary>
    /// <exception cref="LoadLocalDBException"></exception>
    /// <exception cref="DirectoryNotFoundException"></exception>
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
        if (_i18n.TryGetValue(lang, out Dictionary<string, string>? value1)) return value1;
        value1 = new Dictionary<string, string>();
        _i18n.Add(lang, value1);
        return value1;
    }

    /// <summary>
    /// 为指定语言添加一条译文
    /// </summary>
    public void Add(string lang, string key, string value) => GetOrCreate(lang)[key] = value;

    /// <summary>
    /// 删除指定语言的指定一个键的翻译数据
    /// </summary>
    public void Remove(string lang, string key) => GetOrCreate(lang).Remove(key);
    
    /// <summary>
    /// 删除指定语言的所有数据
    /// </summary>
    public void Remove(string lang) => _i18n.Remove(lang);
    
    /// <summary>
    /// 通过字典扩展指定语言的翻译信息(覆盖已存在键)
    /// </summary>
    public HashSet<string> Expand(string lang, Dictionary<string, string> value)
    {
        HashSet<string> coverageKeys = [];
        if (value.Count == 0) return coverageKeys; // 快速跳过
        Dictionary<string, string> cache = GetOrCreate(lang);
        foreach ((string key, string item) in value)
        {
            if (cache.ContainsKey(key)) coverageKeys.Add(key);
            cache[key] = item;
        }
        return coverageKeys;
    }

    /// <summary>
    /// 为当前语言添加一条译文
    /// </summary>
    public void Add(string key, string value) => Add(CurrentLang, key, value);

    /// <summary>
    /// 获取带参数的格式化字符串
    /// <remarks>字符串中的参数必须是{{变量名}}格式</remarks>
    /// </summary>
    /// <param name="msg">有参数格式化字符串</param>
    /// <param name="args">参数对象</param>
    public string DumpFormatStrLocal(string msg, object args)
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
    /// 获取本地化文本（主要公共方法）
    /// </summary>
    /// <param name="key">本地化键</param>
    /// <param name="args">可选参数对象，属性将替换字符串中的{{属性名}}</param>
    /// <returns>本地化后的字符串</returns>
    public string Translate(string key, object? args = null)
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
            if (_i18n.TryGetValue(CurrentLang, out Dictionary<string, string>? cache))
            {
                CurrentI18NCache = cache;
            }
            else if (_i18n.TryGetValue(DefaultLang, out Dictionary<string, string>? chCache))
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
        return DatabaseServer?.GetTables().Locales.Global[CurrentLang].Value;
    }
    #endregion
}