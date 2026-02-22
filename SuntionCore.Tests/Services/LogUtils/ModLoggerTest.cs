#nullable enable
using System;
using System.Text;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuntionCore.Services.LogUtils;

namespace SuntionCore.Tests.Services.LogUtils;

[TestClass]
[TestSubject(typeof(ModLogger))]
public class ModLoggerTest
{

    [TestMethod]
    public void Method()
    {
        var loggerRaidRecord = ModLogger.GetOrCreateLogger("RaidRecord", ModLoggerStrategy.SingleFile, TestData.TestLogFolder);
        var loggerAllGoodsTrader = ModLogger.GetOrCreateLogger("AllGoodsTrader", ModLoggerStrategy.SingleFile, TestData.TestLogFolder);
        var loggerItemCreator = ModLogger.GetOrCreateLogger("ItemCreator", ModLoggerStrategy.MultiFile, TestData.TestLogFolder);
        
        Console.WriteLine(loggerRaidRecord.Info("日志初始化完毕"));

        Random random = new();
        
        Console.WriteLine(loggerRaidRecord.IsStreamReady(LogWriterStream.InfoStream));


        Console.WriteLine($"开始执行 500 次随机日志记录测试 (文件夹：{TestData.TestLogFolder})");

        for (var i = 0; i < 500; i++)
        {
            // 1. 随机选择日志级别 (Info: 30%, Warn: 40%, Error/Debug: 30%)
            double levelRoll = random.NextDouble();
    
            // 2. 随机生成内容 (长度在 10 到 200 字符之间波动)
            string message = $"[迭代 {i:D3}] " + GenerateRandomText(10, 200);
    
            // 3. 随机选择一个 Logger 实例
            ModLogger currentLogger = GetRandomLogger();
    
            // 4. 偶尔模拟异常 (10% 的概率附带 Exception)
            Exception? ex = null;
            if (random.NextDouble() < 0.1)
            {
                ex = new InvalidOperationException($"模拟的异常：索引 {i} 处的数据无效");
                message += " [包含异常堆栈]";
            }

            try
            {
                if (levelRoll < 0.3)
                {
                    // 30% 概率 -> Info
                    currentLogger.Info(message, ex);
                }
                else if (levelRoll < 0.7)
                {
                    // 40% 概率 (0.3 ~ 0.7) -> Warn
                    currentLogger.Warn(message, ex);
                }
                else
                {
                    // 30% 概率 (0.7 ~ 1.0) -> 混合 Debug 和 Error
                    if (random.NextDouble() < 0.5)
                    {
                        currentLogger.Debug(message, ex);
                    }
                    else
                    {
                        currentLogger.Error(message, ex);
                    }
                }
        
                // 每 50 条在控制台输出一次进度，避免刷屏
                if ((i + 1) % 50 == 0)
                {
                    Console.WriteLine($"... 已完成 {i + 1}/500 条日志记录");
                }
            }
            catch (Exception writeEx)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"写入日志失败 (迭代 {i}): {writeEx.Message}");
                Console.ResetColor();
            }
        }

        Console.WriteLine("测试完成！请检查日志文件夹验证文件生成情况。");

        foreach (ModLogger variable in new[] {loggerRaidRecord, loggerAllGoodsTrader, loggerItemCreator})
        {
            Console.WriteLine($" - {variable.ModName}: {variable.FolderPath}");
        }

        // // 可选：手动释放资源
        // loggerRaidRecord.Dispose();
        // loggerAllGoodsTrader.Dispose();
        // loggerItemCreator.Dispose();
        return;

        // 辅助方法：随机获取一个 Logger
        ModLogger GetRandomLogger()
        {
            ModLogger[] loggers = [loggerRaidRecord, loggerAllGoodsTrader, loggerItemCreator];
            return loggers[random.Next(loggers.Length)];
        }

        // 辅助方法：生成随机字符串
        string GenerateRandomText(int minLength, int maxLength)
        {
            // ReSharper disable once StringLiteralTypo
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_=+[]{}|;:',.<>?/!@#$%^&*() ";
            int length = random.Next(minLength, maxLength + 1);
            StringBuilder sb = new(length);
    
            for (int i = 0; i < length; i++)
            {
                sb.Append(chars[random.Next(chars.Length)]);
            }
            return sb.ToString();
        }
    }
}
