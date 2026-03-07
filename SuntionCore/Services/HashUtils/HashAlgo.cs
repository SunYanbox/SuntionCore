// ReSharper disable InconsistentNaming
namespace SuntionCore.Services.HashUtils;
/// <summary>
/// 指定支持的哈希算法类型，用于数据完整性校验和加密操作
/// </summary>
/// <remarks>
/// 包含SHA和MD系列算法，其中：
/// <list type="bullet">
/// <item><description><see cref="SHA256"/>、<see cref="SHA384"/>、<see cref="SHA512"/> 是目前推荐的加密安全算法</description></item>
/// <item><description><see cref="SHA1"/> 和 <see cref="MD5"/> 仅应用于兼容旧系统或非安全场景（如文件校验）</description></item>
/// </list>
/// </remarks>
public enum HashAlgo
{
    /// <summary>
    /// SHA-1 安全哈希算法（160位）
    /// </summary>
    /// <remarks>
    /// 警告：此算法已被证实存在碰撞攻击风险，不推荐用于安全敏感场景
    /// 输出长度：160位（20字节）
    /// </remarks>
    SHA1,

    /// <summary>
    /// SHA-256 安全哈希算法（256位）
    /// </summary>
    /// <remarks>
    /// 推荐：目前广泛使用的安全哈希算法，属于SHA-2家族
    /// 输出长度：256位（32字节）
    /// 适用于：数字签名、证书、区块链、文件完整性校验
    /// </remarks>
    SHA256,

    /// <summary>
    /// MD5 消息摘要算法（128位）
    /// </summary>
    /// <remarks>
    /// 警告：此算法存在严重安全漏洞，极易产生碰撞，仅适用于非安全场景
    /// 输出长度：128位（16字节）
    /// 适用场景：文件下载完整性校验（非安全要求）、旧系统兼容
    /// </remarks>
    MD5,

    /// <summary>
    /// SHA-384 安全哈希算法（384位）
    /// </summary>
    /// <remarks>
    /// 推荐：SHA-2家族成员，提供更高的安全性
    /// 输出长度：384位（48字节）
    /// 特点：基于SHA-512算法截断得到，安全性高于SHA-256
    /// </remarks>
    SHA384,

    /// <summary>
    /// SHA-512 安全哈希算法（512位）
    /// </summary>
    /// <remarks>
    /// 推荐：SHA-2家族中输出最长的算法，安全性最高
    /// 输出长度：512位（64字节）
    /// 适用场景：高安全性要求的加密应用、大型文件哈希
    /// </remarks>
    SHA512
}