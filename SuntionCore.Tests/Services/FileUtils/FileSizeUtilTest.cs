using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuntionCore.Services.FileUtils;

namespace SuntionCore.Tests.Services.FileUtils;

[TestClass]
[TestSubject(typeof(FileSizeUtil))]
public class FileSizeUtilTest
{

    [TestMethod]
    public void Method()
    {
        Console.WriteLine($"{TestData.TestDataFilePath}: {TestData.TestDataFilePath.ToFileSize(3)}");
        Console.WriteLine($"{TestData.TestDataFilePath}: {FileSizeUtil.GetFileSize(TestData.TestDataFilePath)}");
    }
}