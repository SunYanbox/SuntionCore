using JetBrains.Annotations;
using SuntionCore.Models;

namespace SuntionCore.Services.MagnitudeUtils;

/// <summary> 常用数量级单位预设 </summary>
public static class MagnitudePreset
{
    /// <summary> 文件尺寸单位 (二进制，1024) </summary>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig FileSizeBinary = MagnitudeConfig.CreateFixed([
            "B", "KB", "MB", "GB", "TB", "PB", "EB"
        ], 
        1024);

    /// <summary> 磁盘容量/网络流量单位 (十进制，1000，符合 SI 标准) </summary>
    /// <remarks>硬盘厂商和网络运营商通常使用 1000 进制 (KB=1000B)，而非 1024。</remarks>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig DataSizeDecimal = MagnitudeConfig.CreateFixed([
            "B", "KB", "MB", "GB", "TB", "PB", "EB"
        ], 
        1000);

    /// <summary> 时间单位 (变步长：60, 60, 24, 7) </summary>
    /// <remarks>秒 -> 分 -> 时 -> 天 -> 周</remarks>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig TimeStandard = MagnitudeConfig.CreateVariable([
            "s", "m", "h", "d", "w"
        ], 
        [60, 60, 24, 7]);

    /// <summary> 时间单位 (高精度，含毫秒) </summary>
    /// <remarks>毫秒 -> 秒 -> 分 -> 时 -> 天</remarks>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig TimeHighPrecision = MagnitudeConfig.CreateVariable([
            "ms", "s", "m", "h", "d"
        ], 
        [1000, 60, 60, 24]);

    /// <summary> 频率单位 (赫兹，1000) </summary>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig Frequency = MagnitudeConfig.CreateFixed([
            "Hz", "kHz", "MHz", "GHz", "THz"
        ], 
        1000);

    /// <summary> 数据数量级缩写 (千/百万/十亿) </summary>
    /// <remarks>常用于显示用户数、点赞数等 (e.g., 1.5K, 2.3M)</remarks>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig CountAbbreviation = MagnitudeConfig.CreateFixed([
            "", "K", "M", "B", "T"
        ], 
        1000);

    /// <summary> 计算机内存/带宽速率 (比特率，1000) </summary>
    /// <remarks>网络速度通常用 bps, Kbps, Mbps (1000 进制)</remarks>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig Bitrate = MagnitudeConfig.CreateFixed([
            "bps", "Kbps", "Mbps", "Gbps", "Tbps"
        ], 
        1000);
    
    /// <summary> 长度单位 (公制，1000) </summary>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig LengthMetric = MagnitudeConfig.CreateFixed([
            "mm", "m", "km"
        ], 
        1000);
        
    /// <summary> 长度单位 (英制，变步长) </summary>
    /// <remarks>英寸 -> 英尺 (12) -> 码 (3) -> 英里 (1760)</remarks>
    [UsedImplicitly] 
    public static readonly MagnitudeConfig LengthImperial = MagnitudeConfig.CreateVariable([
            "in", "ft", "yd", "mi"
        ], 
        [12, 3, 1760]);
}