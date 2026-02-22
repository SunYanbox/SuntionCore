## SuntionCore

从我做过的模组中提取、优化后的一些常用方法，适合SPT服务端模组

当前版本包含**文件大小计算与格式化**和**多语言国际化 (I18N) 管理**以及**模块化日志记录系统**。

## 提供的功能

**关于如何使用这些功能可以参考SuntionCore.Tests项目中的测试代码**

- [类型扩展速览](#类型扩展速览)
  - 将多语言功能, 文件尺寸计算功能扩展到字符串的成员函数, 方便于使用
- [MagnitudeFormatter - 按单位格式化数值工具](#magnitudeformatter---按单位格式化数值工具)
  - 提供封住的单位类型`MagnitudeConfig`以及常用单位的预设`MagnitudePreset`
  - 支持根据步长格式化数值为格式化字符串: 如 12.12 KB 36.24 MB 等
- [FileSizeUtil - 文件尺寸工具](#filesizeutil---文件尺寸工具)
  - 直接使用静态类`FileSizeUtil`
  - 使用字符串扩展`ToFileSize`或`CalFileSize`
- [I18N - 国际化多语言管理](#i18n---国际化多语言管理)
  - 使用I18N.GetI18N(string name)获取实例(没有时返回空)
  - 使用I18N.GetOrCreateI18N(string name)获取实例
  - (使用本功能前需要调用至少一次I18N.Initialize(DatabaseServer databaseServer))自动根据当前语言跟踪SptLocals字典, 可以通过SptLocals属性获取当前语言对应的该字典
- [ModLogger - 模块化日志系统](#modlogger---模块化日志系统)
  - 使用ModLogger.GetLogger(string name)获取实例(没有时返回空)
  - 使用ModLogger.GetOrCreateLogger(string name, ModLoggerStrategy strategy = ModLoggerStrategy.SingleFile, string folderPath = DefaultLogFolderPath, long logFileMaxSize = 0)获取实例
  - 默认日志文件夹路径: `user/mods/SuntionCore/ModLogs`(存在模组使用该日志时才会在服务器加载完毕后输出日志文件夹路径->该路径下的模组的信息)

## 如何在你的项目引用这个库

1. 在模组项目文件夹下创建libs文件夹(可选)
2. 安装本模组到SPT(本模组Releases默认打包为适配通用模组安装方式的格式), 把`你的游戏文件夹\SPT\user\mods\SuntionCore\SuntionCore.dll"`复制到上述libs文件夹下
3. 在`你的项目名称.csproj`中写入以下内容
   ```css
    <ItemGroup>
        <Reference Include="SuntionCore.dll">
            <HintPath>libs\SuntionCore.dll</HintPath>
            <Private>False</Private>
        </Reference>
    </ItemGroup>
   ```
   > HintPath为你实际的dll文件相对于csproj文件的相对路径
   > Private必须设置为false以避免在打包输出时错误的包含所有引用过的库
4. 随后你的编译器就可以正确引用与使用该库中的内容了

## 类型扩展速览

```csharp
/// <summary>
/// 为String类型扩展的获取文件尺寸函数
/// </summary>
/// <param name="value">文件路径</param>
/// <param name="decimalPlaces">保留的小数位数</param>
/// <returns>文件尺寸</returns>
public static string ToFileSize(this string? value, int decimalPlaces = 2);

/// <summary>
/// 为String类型扩展的计算文件尺寸函数
/// </summary>
/// <param name="value">文件路径</param>
/// <returns>文件尺寸</returns>
public static long CalFileSize(this string? value);

/// <summary>
/// 为String类型扩展的翻译函数
/// </summary>
/// <param name="value">本地化键, 即该字符串自身</param>
/// <param name="local">I18N实例</param>
/// <param name="args">参数</param>
/// <remarks>参数格式: {{ArgName}}</remarks>
/// <returns>译文</returns>
[UsedImplicitly]
public static string Translate(this string? value, I18N local, object? args = null);
```

| 代码                                                     | 补充信息                                                     | 结果                             |
| -------------------------------------------------------- | ------------------------------------------------------------ | -------------------------------- |
| "一个合法文件路径".ToFileSize();                         |                                                              | (String)(61.23 KB)               |
| "一个合法文件路径".CalFileSize();                        |                                                              | (long)(612346)                   |
| "key1".Translate(i18N);                                  | i18N指I18N实例<br/>在翻译文件中key1对应的值为value1          | (String)(value1)                 |
| "key1::key2".Translate(i18N);                            | 在翻译文件中keyx对应的值为valuex<br/>当前语言默认LinkTag对应的值为- | (String)(value1-value2)          |
| "key1".Translate(i18N, new { Name = "a", Count = "b" }); | 在翻译文件中key1对应的值为"value1: Name={{Name}} Count={{Count}}" | (String)(value1: Name=a Count=b) |

---

## MagnitudeFormatter - 按单位格式化数值工具

位于 `public static class MagnitudeFormatter`, 提供静态方法用于根据单位与步长的配置格式化数值

[API参考](API_ZH.md#magnitudeformatter---公有接口)

### 使用示例

参考`SuntionCore.Tests/Services/MagnitudeFormatterTest.cs`

## FileSizeUtil - 文件尺寸工具

[API参考](API_ZH.md#filesizeutil---公有接口)

### 💡 使用示例

```csharp
string filePath = @"C:\Mods\MyMod\data.bin";

// 1. 获取原始字节数
long sizeInBytes = FileSizeUtil.CalFileSize(filePath);
Console.WriteLine($"文件大小：{sizeInBytes} bytes");

// 2. 获取格式化后的大小 (默认保留 2 位小数)
string readableSize = FileSizeUtil.GetFileSize(filePath); 
// 输出示例："10.25 MB"

// 3. 自定义小数位数
string preciseSize = FileSizeUtil.GetFileSize(filePath, decimalPlaces: 4);
// 输出示例："10.2500 MB"
```

### ⚠️ 异常处理
- **`GetFileSizeException`**: 当文件不存在、路径无效或无法访问时抛出。

---

## I18N - 国际化多语言管理

### 🚀 核心特性
- **单例/多实例管理**：通过名称管理多个独立的翻译上下文。
- **动态加载**：支持从文件夹批量加载 `xx.json` 格式的语言文件。
- **智能连接**：支持使用 `::` 符号将多个 Key 的译文拼接。
- **参数化替换**：支持 `{{变量名}}` 格式的字符串插值。
- **数据库集成**：可选集成 `DatabaseServer` 进行持久化存储。

[API参考](API_ZH.md#i18n---公有接口)

### 🛠️ 初始化与配置

在使用前，建议初始化全局数据库服务器（如果需要使用SptLocals时）:

```csharp
// 设置全局数据库服务器
I18N.DatabaseServer ??= injectDatabaseServerInstance;
// 或者使用初始化方法
I18N.Initialize(injectDatabaseServerInstance);
```

获取或创建实例：
```csharp
// 获取名为 "MyMod" 的实例，若不存在则创建
var i18n = I18N.GetOrCreateI18N("MyMod");

// 加载语言文件 (加载 path 目录下所有 xx.json 文件)
i18n.LoadFolders("./Languages");

// 设置当前语言 (例如设置为中文 "ch")
i18n.CurrentLang = "ch"; 
```

### 📝 常用操作

#### 1. 添加/修改/删除翻译
```csharp
// 为当前语言添加
i18n.Add("greeting", "你好，世界！");

// 为指定语言添加
i18n.Add("en", "greeting", "Hello World!");

// 批量扩展 (覆盖已存在的键)
var newTranslations = new Dictionary<string, string> { 
    { "farewell", "再见" } 
};
var overriddenKeys = i18n.Expand("zh", newTranslations);

// 删除单个键
i18n.Remove("zh", "old_key");

// 删除整个语言包
i18n.Remove("fr");
```

#### 2. 获取翻译文本
支持简单的 Key 查询、多 Key 拼接以及参数替换。

```csharp
// 简单查询
string text1 = i18n.Translate("greeting"); 
// 输出：你好，世界！

// 多键拼接 (使用 :: 连接)
// 假设 key1="欢迎", key2="来到", key3="SPT"
string text2 = i18n.Translate("key1::key2::key3"); 
// 输出：欢迎来到 SPT (自动根据当前语言的分隔符连接)

// 带参数替换 (使用 {{PropertyName}})
var args = new { UserName = "PlayerOne", Count = 5 };
string template = "玩家 {{UserName}} 获得了 {{Count}} 个物品";
// 注意：Translate 方法内部会处理 object 属性的反射替换
string text3 = i18n.Translate("reward_msg", args); 
// 输出：玩家 PlayerOne 获得了 5 个物品
```

### ⚠️ 异常提示
- **`NotLoadLanguageException`**: 尝试切换到未加载的语言时抛出。
- **`I18NNameAlreadyExistException`**: 语言代码长度不为 2 或命名冲突时抛出。
- **`IllegalTranslationKeyException`**: 批量导入时键格式非法。

---

## ModLogger - 模块化日志系统

`ModLogger` 专为模组开发设计，旨在将模组的详细日志与主服务器日志分离，便于调试且避免污染主日志流。

[API参考](API_ZH.md#modlogger---公有接口)

### 🌟 主要特点
- **独立文件**：每个模组拥有独立的日志文件或流。
- **策略灵活**：支持单文件轮转(只在模组加载时轮转一次, 不是很适配Fika之类一直开着的服务器)、按级别分流等多种策略 (`ModLoggerStrategy`)。
- **自动清理**：可配置文件大小上限，超出在启动服务器使用日志时自动删除旧日志。
- **线程安全**：静态注册表采用锁机制，确保多线程安全。

### 🚀 快速开始

#### 1. 获取 logger 实例
推荐使用静态方法获取或创建，确保全局唯一性。

```csharp
// 获取或创建名为 "MyAwesomeMod" 的日志器
// 策略：单文件模式
// 路径：默认 user/mods/SuntionCore/ModLogs
// 大小限制：1MB (默认)，设为 0 表示使用全局默认值
var logger = ModLogger.GetOrCreateLogger(
    "MyAwesomeMod", 
    strategy: ModLoggerStrategy.SingleFile,
    logFileMaxSize: 5 * 1024 * 1024 // 限制为 5MB
);
```

#### 2. 记录日志
所有记录方法都会返回一条带有 `[ModName]` 前缀的格式化字符串，方便追踪。

```csharp
// 信息日志
logger.Info("模组初始化成功...");

// 警告日志
logger.Warn("配置文件缺失，使用默认配置", exception: null);

// 错误日志 (附带异常堆栈)
try 
{
    // ... 可能出错的代码
}
catch (Exception ex)
{
    logger.Error("加载资源失败", ex);
}

// 调试日志 (仅在需要详细调试时开启)
logger.Debug($"当前内存占用：{GC.GetTotalMemory(false)}");
```

#### 3. 全局管理
```csharp
// 获取所有已注册的 Logger
var allLoggers = ModLogger.Items;

// 获取特定名称的 Logger (若不存在返回 null)
var specificLogger = ModLogger.GetLogger("OtherMod");

// 查看注册数量
long count = ModLogger.LoggerCount;
```

## 致谢

灵感来源：
- [MassivesoftCore](https://forge.sp-tarkov.com/mod/2587/massivesoftcore) | **关于制作SPT类库**
- [MassivesoftWeapons](https://forge.sp-tarkov.com/mod/2588/massivesoftweapons) | **关于引用SPT类库**
- [SPT Item Creator](https://forge.sp-tarkov.com/mod/2565/spt-item-creator) | **关于本地模组日志**
- [Raid Record](https://forge.sp-tarkov.com/mod/2341/raid-record) | **关于文件尺寸计算 + 关于国际化功能**
- [SuntionCore](https://forge.sp-tarkov.com/mod/2600/suntioncore) | **关于按照单位和步长划分数值**