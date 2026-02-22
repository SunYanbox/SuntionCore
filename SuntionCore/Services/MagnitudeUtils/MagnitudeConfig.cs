namespace SuntionCore.Models;

public record MagnitudeConfig
{
    // 主构造函数参数即属性
    // units: 单位数组
    // steps: 步长数组 (如果是固定步长，这里存 null 或单元素数组，建议用 nullable array)
    // fixedStep: 固定步长值 (如果是可变步长，这里忽略)
    // isVariable: 标记当前模式
    private MagnitudeConfig(string[] units, double[]? steps, double fixedStep, bool isVariable)
    {
        Units = units;
        Steps = steps;
        FixedStep = fixedStep;
        IsVariable = isVariable;

        // 校验逻辑
        if (Units == null || Units.Length == 0)
            throw new ArgumentException("单位数组禁止为空", nameof(units));

        if (IsVariable)
        {
            if (Steps == null || Steps.Length != Units.Length - 1)
                throw new ArgumentException(
                    $"可变步长数组长度必须等于 单位数组长度-1 即: {Units.Length - 1}", 
                    nameof(steps));
            
            if (Steps.Any(s => s <= 1))
                throw new ArgumentException("可变步长数组中任意步长必须大于1", nameof(steps));
        }
        else
        {
            if (FixedStep <= 1)
                throw new ArgumentException("固定步长必须大于1", nameof(fixedStep));
        }
        
        // 构造时克隆数组
        Units = (string[])units.Clone();
        if (Steps is not null)
        {
            Steps = (double[])Steps.Clone();
        }
    }
    /// <summary> 单位 </summary>
    public string[] Units { get; init; }
    /// <summary> 可变步长 </summary>
    public double[]? Steps { get; init; }
    /// <summary> 固定步长 </summary>
    public double FixedStep { get; init; }
    /// <summary> 是否是可变步长的标志 </summary>
    public bool IsVariable { get; init; }

    /// <summary>
    /// 【工厂方法】创建固定步长系统
    /// </summary>
    public static MagnitudeConfig CreateFixed(string[] units, double step)
    {
        return new MagnitudeConfig(units, null, step, false);
    }

    /// <summary>
    /// 【工厂方法】创建可变步长系统
    /// </summary>
    public static MagnitudeConfig CreateVariable(string[] units, double[] steps)
    {
        return new MagnitudeConfig(units, steps, 0, true);
    }
    
    public override string ToString() => $"MagnitudeConfig({(IsVariable && Steps is not null ? $"可变步长: [{string.Join(", ", Steps)}]" : $"固定步长:{FixedStep}")}, 单位=[{string.Join(", ", Units)}])";
}