using System.Security.Cryptography;
using System.Text;
using JetBrains.Annotations;

namespace SuntionCore.Services.HashUtils;

/// <summary>
/// 提供字符串和字节数组的哈希计算功能
/// </summary>
/// <remarks>
/// 这是一个静态工具类，封装了常用的哈希算法，提供了简便的哈希计算方法
/// 支持 SHA1、SHA256、MD5、SHA384、SHA512 等算法，默认使用 SHA256
/// </remarks>
public static class HashUtil
{
    /// <summary>
    /// 计算输入字符串的哈希值，并以十六进制字符串形式返回
    /// </summary>
    /// <param name="text">要进行哈希计算的输入字符串</param>
    /// <param name="hashAlgo">指定的哈希算法，默认为 <see cref="HashAlgo.SHA256"/></param>
    /// <returns>
    /// 返回表示哈希值的十六进制字符串
    /// </returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="text"/> 为 null 时抛出</exception>
    /// <exception cref="NotSupportedException">当指定的哈希算法不支持时抛出</exception>
    [UsedImplicitly]
    public static string Hash(string text, HashAlgo hashAlgo = HashAlgo.SHA256)
    {
        ArgumentNullException.ThrowIfNull(text);

        return Convert.ToHexString(Hash(Encoding.UTF8.GetBytes(text), hashAlgo));
    }

    /// <summary>
    /// 计算输入字节数组的哈希值，并以字节数组形式返回
    /// </summary>
    /// <param name="data">要进行哈希计算的输入字节数组</param>
    /// <param name="hashAlgo">指定的哈希算法，默认为 <see cref="HashAlgo.SHA256"/></param>
    /// <returns>
    /// 返回表示哈希值的字节数组
    /// </returns>
    /// <exception cref="ArgumentNullException">当 <paramref name="data"/> 为 null 时抛出</exception>
    /// <exception cref="NotSupportedException">当指定的哈希算法不支持时抛出</exception>
    /// <remarks>
    /// 此方法使用 .NET 内置的静态 HashData 方法（.NET 5+），
    /// 无需创建 HashAlgorithm 实例，性能更优且无需手动释放资源
    /// </remarks>
    public static byte[] Hash(byte[] data, HashAlgo hashAlgo = HashAlgo.SHA256)
    {
        ArgumentNullException.ThrowIfNull(data);

        return hashAlgo switch
        {
            HashAlgo.SHA1 => SHA1.HashData(data),
            HashAlgo.SHA256 => SHA256.HashData(data),
            HashAlgo.MD5 => MD5.HashData(data),
            HashAlgo.SHA384 => SHA384.HashData(data),
            HashAlgo.SHA512 => SHA512.HashData(data),
            
            // 理论上不会执行到这里，因为枚举已经覆盖所有可能的值
            // 但为了代码完整性保留此分支
            _ => throw new NotSupportedException(
                $"不支持的哈希算法: {hashAlgo}请确保使用了 {nameof(HashAlgo)} 中定义的算法")
        };
    }
}