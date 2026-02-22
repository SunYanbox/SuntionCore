using JetBrains.Annotations;
using SuntionCore.Models;

namespace SuntionCore.Services.MagnitudeUtils;

/// <summary>
/// 通用的量级数值格式化器
/// </summary>
public static class MagnitudeFormatter
{
    /// <summary>
    /// 通用的量级数值格式化器（支持变步长和空格控制）
    /// </summary>
    /// <param name="value">原始数值 (支持 int, long, float, double 等)</param>
    /// <param name="steps">
    /// 量级间隔基数数组。<br />
    /// 长度应为 units.Length - 1。
    /// </param>
    /// <param name="units">单位数组 (例如：["B", "KB", "MB"] 或 ["s", "min", "h"])</param>
    /// <param name="decimalPlaces">保留小数位数</param>
    /// <param name="addSpace">数值和单位之间是否添加空格 (默认 true)</param>
    /// <param name="fixedStep">
    /// 当 steps 未提供时使用的固定步长。
    /// 本实现优先使用 steps 数组，若 steps 为空则回退到 fixedStep。
    /// </param>
    /// <remarks>小于1的数会直接返回!!!</remarks>
    /// <exception cref="ArgumentException"></exception>
    /// <returns>格式化后的字符串</returns>
    [UsedImplicitly]
    private static string CommonFormat(
        double value, 
        double[]? steps = null, 
        string[]? units = null, 
        int decimalPlaces = 2, 
        bool addSpace = true,
        double fixedStep = 1000)
    {
        if (value <= 1) return value.ToString($"F{decimalPlaces}");
        
        if (units == null || units.Length == 0)
            throw new ArgumentException("单位字符串数组不能为空", nameof(units));

        if (steps is { Length: > 0 } && steps.Any(x => x <= 1))
            throw new ArgumentException("可变步长必须每一项都大于1", nameof(steps));

        // 确定步长策略
        bool useVariableSteps = steps is { Length: > 0 };
        
        // 如果使用变步长，步长数组长度必须等于 单位数 - 1
        if (useVariableSteps && steps!.Length != units.Length - 1)
        {
            throw new ArgumentException($"步长数组长度({steps.Length})必须等于({units.Length-1}).", nameof(steps));
        }

        // 处理 0 或负数情况
        if (value <= 0)
        {
            string space = addSpace && !string.IsNullOrEmpty(units[0]) ? " " : "";
            return $"0{space}{units[0]}";
        }

        // 计算量级索引 (Magnitude)
        var magnitude = 0;
        double remainingValue = value;

        if (useVariableSteps && steps is not null)
        {
            // 遍历步长数组，看当前值是否大于下一个阈值
            foreach (int t in steps)
            {
                if (remainingValue >= t)
                {
                    remainingValue /= t;
                    magnitude++;
                }
                else
                {
                    break;
                }
            }
        }
        else
        {
            // 固定步长逻辑  防止除以零
            if (fixedStep <= 1) fixedStep = 1000;
            
            magnitude = (int)Math.Floor(Math.Log(value, fixedStep));
        }

        // 限制最大索引，防止超出单位数组范围
        magnitude = Math.Min(magnitude, units.Length - 1);

        // 计算最终缩放后的数值
        // 如果是变步长，我们在循环中已经逐步除以了步长，remainingValue 就是结果
        // 如果是固定步长，我们需要重新计算（因为上面 log 只是算索引）
        double adjustedValue;
        if (useVariableSteps)
        {
            adjustedValue = remainingValue;
        }
        else
        {
            adjustedValue = value / Math.Pow(fixedStep, magnitude);
        }

        // 4. 格式化输出
        string spaceChar = addSpace && !string.IsNullOrEmpty(units[magnitude]) ? " " : "";
        var formatString = $"F{decimalPlaces}";
        
        return $"{adjustedValue.ToString(formatString)}{spaceChar}{units[magnitude]}";
    }

    public static string Format(
        double value,
        string[] units,
        double[] steps,
        int decimalPlaces = 2,
        bool addSpace = true) => CommonFormat(value, steps, units, decimalPlaces, addSpace);
    
    public static string Format(
        double value,
        string[] units,
        int fixedStep = 1000,
        int decimalPlaces = 2,
        bool addSpace = true) => CommonFormat(value, units: units, decimalPlaces: decimalPlaces, addSpace:addSpace,fixedStep:fixedStep);

    public static string Format(
        double value,
        MagnitudeConfig magnitude,
        int decimalPlaces = 2,
        bool addSpace = true) => CommonFormat(value, steps: magnitude.Steps, units: magnitude.Units,
        decimalPlaces: decimalPlaces, addSpace: addSpace, fixedStep: magnitude.FixedStep);
}