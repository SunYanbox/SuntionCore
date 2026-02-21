namespace SuntionCore.Services.LogUtils.Exceptions;

public class ArgumentOutOfRangeInThereException(string param, string currValue, string where, string range)
    : Exception($"{where}位置的参数{param}(当前值: {currValue})超出范围, 应该在{range}");