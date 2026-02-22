## SuntionCore

[中文](Docs/README_ZH.md)

Some commonly used methods extracted and optimized from the mods I've created, suitable for SPT server mods.

The current version includes **File Size Calculation and Formatting**, **Internationalization (I18N) Management**, and a **Modular Logging System**.

## Provided Features

**For usage examples of these features, please refer to the test code in the `SuntionCore.Tests` project.**

- [Type Extension Overview](#type-extension-overview)
  - Extends string member functions with multi-language support and file size calculation features for easier usage.
- [MagnitudeFormatter - Unit-based Numeric Formatting Tool](#magnitudeformatter---unit-based-numeric-formatting-tool)
  - Provides the encapsulated unit type `MagnitudeConfig` and predefined common units via `MagnitudePreset`.
  - Supports formatting numeric values into strings based on step sizes, e.g., 12.12 KB, 36.24 MB, etc.
- [FileSizeUtil - File Size Utility](#filesizeutil---file-size-utility)
    - Use the static class `FileSizeUtil` directly
    - Use the string extensions `ToFileSize` or `CalFileSize`
- [I18N - Internationalization Management](#i18n---internationalization-management)
    - Use `I18N.GetI18N(string name)` to get an instance (returns null if not found)
    - Use `I18N.GetOrCreateI18N(string name)` to get or create an instance
    - (Before using this feature, `I18N.Initialize(DatabaseServer databaseServer)` needs to be called at least once) Automatically tracks the SptLocals dictionary based on the current language. The dictionary corresponding to the current language can be accessed via the `SptLocals` property.
- [ModLogger - Modular Logging System](#modlogger---modular-logging-system)
    - Use `ModLogger.GetLogger(string name)` to get an instance (returns null if not found)
    - Use `ModLogger.GetOrCreateLogger(string name, ModLoggerStrategy strategy = ModLoggerStrategy.SingleFile, string folderPath = DefaultLogFolderPath, long logFileMaxSize = 0)` to get or create an instance
    - Default log folder path: `user/mods/SuntionCore/ModLogs` (The folder path for the mod will be output after the server finishes loading only when a mod uses this logging system -> information for mods under that path)
- [ModMailService: A utility for sending bulk messages, processing payments, and handling claims on the server](#modmailservice-a-utility-for-bulk-messaging-payments-and-claims)
    - Provides `SendMessage` and `SendAllMessageAsync` for message delivery.
    - Includes `SplitStringByNewlines` to split long strings into client-friendly lists (automatically handled within `SendAllMessageAsync`).
    - Utilizes `Payment` and `SendMoney` for deducting costs and issuing rewards, respectively.
    - Offers `SendItemsToPlayer` for item delivery, featuring automatic batching based on stack limits across all payment, reward, messaging, and item transfer operations.

## How to Reference This Library in Your Project

1. Create a `libs` folder within your mod project directory (optional).
2. Install this mod to SPT (the releases are packaged in a format compatible with standard mod installers by default), then copy `YourGameFolder\SPT\user\mods\SuntionCore\SuntionCore.dll` to the `libs` folder created in the previous step.
3. Add the following content to your `YourProjectName.csproj` file:
   ```xml
   <ItemGroup>
       <Reference Include="SuntionCore.dll">
           <HintPath>libs\SuntionCore.dll</HintPath>
           <Private>False</Private>
       </Reference>
   </ItemGroup>
   ```
   > **Note:**
   > *   `HintPath` should be the relative path from your `.csproj` file to the actual DLL file.
   > *   `Private` must be set to `False` to prevent all referenced libraries from being incorrectly included during the build output packaging.
4. Your compiler will now be able to correctly reference and use the contents of this library.

## Type Extension Overview

```csharp
/// <summary>
/// Extension function for String type to get the file size
/// </summary>
/// <param name="value">File path</param>
/// <param name="decimalPlaces">Number of decimal places to retain</param>
/// <returns>File size as a formatted string</returns>
public static string ToFileSize(this string? value, int decimalPlaces = 2);

/// <summary>
/// Extension function for String type to calculate the file size
/// </summary>
/// <param name="value">File path</param>
/// <returns>File size in bytes as a long</returns>
public static long CalFileSize(this string? value);

/// <summary>
/// Extension function for String type to translate the string
/// </summary>
/// <param name="value">The localization key, i.e., the string itself</param>
/// <param name="local">I18N instance</param>
/// <param name="args">Parameters</param>
/// <remarks>Parameter format: {{ArgName}}</remarks>
/// <returns>Translated text</returns>
[UsedImplicitly]
public static string Translate(this string? value, I18N local, object? args = null);
```

| Code                                                  | Supplementary Information                                    | Result                            |
| ----------------------------------------------------- | ------------------------------------------------------------ | --------------------------------- |
| `"a_valid_file_path".ToFileSize();`                   |                                                              | (String) `(61.23 KB)`             |
| `"a_valid_file_path".CalFileSize();`                  |                                                              | (long) `612346`                   |
| `"key1".Translate(i18N);`                              | `i18N` refers to an I18N instance.<br/>In the translation file, the value corresponding to `key1` is `value1`. | (String) `value1`                 |
| `"key1::key2".Translate(i18N);`                        | In the translation file, the value corresponding to `keyx` is `valuex`.<br/>The default `LinkTag` for the current language corresponds to `-`. | (String) `value1-value2`          |
| `"key1".Translate(i18N, new { Name = "a", Count = "b" });` | In the translation file, the value corresponding to `key1` is `"value1: Name={{Name}} Count={{Count}}"`. | (String) `value1: Name=a Count=b` |

---

## MagnitudeFormatter - Unit-based Numeric Formatting Tool

Located in `public static class MagnitudeFormatter`, this class provides static methods to format numeric values according to configured units and step sizes.

[API Reference](Docs/API_EN.md#magnitudeformatter---public-api)

### Usage Examples

Refer to `SuntionCore.Tests/Services/MagnitudeFormatterTest.cs`.

## FileSizeUtil - File Size Utility

[API Reference](Docs/API_EN.md#filesizeutil---public-api)

### 💡 Usage Examples

```csharp
string filePath = @"C:\Mods\MyMod\data.bin";

// 1. Get the raw size in bytes
long sizeInBytes = FileSizeUtil.CalFileSize(filePath);
Console.WriteLine($"File size: {sizeInBytes} bytes");

// 2. Get the formatted size (default 2 decimal places)
string readableSize = FileSizeUtil.GetFileSize(filePath); 
// Example output: "10.25 MB"

// 3. Customize the number of decimal places
string preciseSize = FileSizeUtil.GetFileSize(filePath, decimalPlaces: 4);
// Example output: "10.2500 MB"
```

### ⚠️ Exception Handling

- **`GetFileSizeException`**: Thrown when the file does not exist, the path is invalid, or the file is inaccessible.

---

## I18N - Internationalization Management

The `I18N` class is a powerful multilingual management system that supports dynamic loading of language packs, key concatenation, parameterized replacement, and more.

### 🚀 Core Features

- **Singleton/Multi-instance Management**: Manage multiple independent translation contexts by name.
- **Dynamic Loading**: Supports batch loading of language files in `xx.json` format from folders.
- **Smart Concatenation**: Supports concatenating translations from multiple keys using the `::` symbol.
- **Parameterized Replacement**: Supports string interpolation using the `{{variableName}}` format.
- **Database Integration**: Optional integration with `DatabaseServer` for persistent storage.

[API Reference](Docs/API_EN.md#i18n---public-api)

### 🛠️ Initialization and Configuration

Before use, it is recommended to initialize the global database server (if `SptLocals` is required).

```csharp
// Set the global database server
I18N.DatabaseServer ??= injectDatabaseServerInstance;
// Or use the initialization method
I18N.Initialize(injectDatabaseServerInstance);
```

Getting or creating an instance:

```csharp
// Get an instance named "MyMod"; create it if it doesn't exist
var i18n = I18N.GetOrCreateI18N("MyMod");

// Load language files (loads all xx.json files in the ./Languages directory)
i18n.LoadFolders("./Languages");

// Set the current language (e.g., set to Chinese "ch")
i18n.CurrentLang = "ch"; 
```

### 📝 Common Operations

#### 1. Adding/Modifying/Deleting Translations

```csharp
// Add a translation for the current language
i18n.Add("greeting", "Hello World!");

// Add a translation for a specific language
i18n.Add("es", "greeting", "¡Hola Mundo!");

// Bulk extend (overwrites existing keys)
var newTranslations = new Dictionary<string, string> { 
    { "farewell", "Goodbye" } 
};
var overriddenKeys = i18n.Expand("en", newTranslations);

// Delete a single key
i18n.Remove("en", "old_key");

// Delete an entire language pack
i18n.Remove("fr");
```

#### 2. Getting Translated Text

Supports simple key queries, multi-key concatenation, and parameter substitution.

```csharp
// Simple query
string text1 = i18n.Translate("greeting"); 
// Output: Hello World!

// Multi-key concatenation (using ::)
// Assuming key1="Welcome", key2="to", key3="SPT"
string text2 = i18n.Translate("key1::key2::key3"); 
// Output: Welcome to SPT (automatically concatenated using the current language's separator)

// Parameterized replacement (using {{PropertyName}})
var args = new { UserName = "PlayerOne", Count = 5 };
string template = "Player {{UserName}} has obtained {{Count}} items";
// Note: The Translate method internally handles reflection-based property replacement on the object
string text3 = i18n.Translate("reward_msg", args); 
// Output: Player PlayerOne has obtained 5 items
```

### ⚠️ Exception Notes

- **`NotLoadLanguageException`**: Thrown when trying to switch to a language that hasn't been loaded.
- **`I18NNameAlreadyExistException`**: Thrown when the language code length is not 2 or there is a naming conflict.
- **`IllegalTranslationKeyException`**: Thrown when keys are in an illegal format during batch import.

---

## ModLogger - Modular Logging System

`ModLogger` is specifically designed for mod development, aiming to separate detailed mod logs from the main server log, facilitating debugging and avoiding pollution of the main log stream.

### 🌟 Main Features

- **Independent Files**: Each mod has its own independent log file or stream.
- **Flexible Strategies**: Supports various strategies like single file rotation (rotates only once when the mod loads, not very suitable for servers like Fika that run continuously) and stream separation by log level (`ModLoggerStrategy`).
- **Automatic Cleanup**: Configurable file size limit. When the limit is exceeded, old logs are automatically deleted when the server starts using the logs.
- **Thread Safety**: The static registry uses locking mechanisms to ensure multi-threading safety.

[API Reference](Docs/API_EN.md#modlogger---public-api)

### 🚀 Quick Start

#### 1. Get a Logger Instance

It is recommended to use the static methods to get or create a logger, ensuring global uniqueness.

```csharp
// Get or create a logger named "MyAwesomeMod"
// Strategy: Single file mode
// Path: Default user/mods/SuntionCore/ModLogs
// Size limit: 1MB (default), set to 0 to use the global default value
var logger = ModLogger.GetOrCreateLogger(
    "MyAwesomeMod", 
    strategy: ModLoggerStrategy.SingleFile,
    logFileMaxSize: 5 * 1024 * 1024 // Limit to 5MB
);
```

#### 2. Log Messages

All logging methods return a formatted string prefixed with `[ModName]` for easy tracking.

```csharp
// Info log
logger.Info("Mod initialized successfully...");

// Warning log
logger.Warn("Configuration file missing, using default configuration", exception: null);

// Error log (includes exception stack trace)
try 
{
    // ... code that might throw an exception
}
catch (Exception ex)
{
    logger.Error("Failed to load resource", ex);
}

// Debug log (only enable when detailed debugging is needed)
logger.Debug($"Current memory usage: {GC.GetTotalMemory(false)}");
```

#### 3. Global Management

```csharp
// Get all registered loggers
var allLoggers = ModLogger.Items;

// Get a specific logger by name (returns null if not exists)
var specificLogger = ModLogger.GetLogger("OtherMod");

// Check the number of registered loggers
long count = ModLogger.LoggerCount;
```

## ModMailService: A Utility for Bulk Messaging, Payments, and Claims

[API Reference](Docs/API_EN.md#modmailservice---public-api)

Primary testing and implementation examples can be found in the **RaidRecord** mod.

## Credits

Inspiration Source:
- [MassivesoftCore](https://forge.sp-tarkov.com/mod/2587/massivesoftcore) | **About Creating SPT Libraries**
- [MassivesoftWeapons](https://forge.sp-tarkov.com/mod/2588/massivesoftweapons) | **About Referencing SPT Libraries**
- [SPT Item Creator](https://forge.sp-tarkov.com/mod/2565/spt-item-creator) | **About Local Mod Logging**
- [Raid Record](https://forge.sp-tarkov.com/mod/2341/raid-record) | 
  - **About File Size Calculation**
  - **About Internationalization Features**
  - **Payment Processing | Reward Distribution | Message Delivery | Item Transfer**
- [SuntionCore](https://forge.sp-tarkov.com/mod/2600/suntioncore) | **About formatting numeric values based on units and steps**