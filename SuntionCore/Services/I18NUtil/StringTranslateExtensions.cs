using JetBrains.Annotations;

namespace SuntionCore.Services.I18NUtil;

public static class StringTranslateExtensions
{
    /// <summary>
    /// 为String类型扩展的翻译函数
    /// </summary>
    /// <param name="value">本地化键, 即该字符串自身</param>
    /// <param name="local">I18N实例</param>
    /// <param name="args">参数</param>
    /// <remarks>参数格式: {{ArgName}}</remarks>
    /// <returns>译文</returns>
    [UsedImplicitly]
    public static string Translate(this string? value, I18N local, object? args = null)
    {
        return local.Translate(value ?? "[String.Empty]", args);
    }
}