using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuntionCore.Services.FileUtils;
using SuntionCore.Services.I18NUtil;

namespace SuntionCore.Tests.Services.I18NUtil;

[TestClass]
[TestSubject(typeof(I18N))]
public class I18NTest
{

    [TestMethod]
    public void Method()
    {
        var i18N = I18N.GetOrCreateI18N("test"); // 创建 / 获取
        foreach (string file in Directory.GetFiles(TestData.TranslatesFolder))
        {
            Console.WriteLine($"- {file} {file.ToFileSize()}");
        }
        i18N.LoadFolders(TestData.TranslatesFolder); // 批量加载数据
        
        Console.WriteLine($"已加载语言: [{string.Join(", ", i18N.AvailableLang)}], 当前语言: {i18N.CurrentLang}");
        
        Console.WriteLine("common::add::hello::abc".Translate(i18N, new
        {
            Name = "World",
            A = "AAA",
            B = "BBB",
            C = "CCC"
        }));

        i18N.CurrentLang = "en"; // 动态更改语言

        Console.WriteLine("common::add::hello::abc".Translate(i18N, new
        {
            Name = "World",
            A = "AAA",
            B = "BBB",
            C = "CCC"
        })); // 连接多个键 与 参数
        
        i18N.Expand("ch", new Dictionary<string, string>
        {
            { "kkk", "sfg" },
            { "jjj", "1567" }
        });  // 批量加载数据

        try
        {
            Console.WriteLine("kkk::jjj".Translate(i18N));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        i18N.CurrentLang = "ch";
        
        Console.WriteLine("kkk::jjj".Translate(i18N));
        
        i18N.CurrentLang = "en";
        
        i18N.Add("kkk", " En(kkk) "); // 手动添加译文
        i18N.Add("en", "jjj", " En(jjj) ");
        
        Console.WriteLine("kkk::jjj".Translate(i18N));
        
        // 由于没有注入数据服务器, 此处为空
        Console.WriteLine($"SptLocals: {i18N.SptLocals is null}");
    }
}