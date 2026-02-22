using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuntionCore.Services.MagnitudeUtils;

namespace SuntionCore.Tests.Services;

[TestClass]
[TestSubject(typeof(MagnitudeFormatter))]
public class MagnitudeFormatterTest
{
    [TestMethod]
    public void Method()
    {
        Random random = new();
        string[] sizeUnits = ["B", "KB", "MB", "GB", "TB", "PB", "EB"];

        for (var i = 0; i < 9; i++)
        {
            Console.WriteLine(MagnitudeFormatter.Format(random.NextDouble() * Math.Pow(1024, i), MagnitudePreset.FileSizeBinary));
        }
        
        Console.WriteLine(MagnitudeFormatter.Format(1, sizeUnits, 1024));
        
        string[] sizeUnits1 = ["", "MB", "TB"];
        
        for (var i = 0; i < 9; i++)
        {
            Console.WriteLine(MagnitudeFormatter.Format(random.NextDouble() * Math.Pow(1024, i), sizeUnits1, (int)Math.Pow(1024, 2)));
        }
        
        string[] sizeUnits2 = [" 卢布", "K 卢布", "M 卢布", "B 卢布"];
        
        for (var i = 0; i < 9; i++)
        {
            Console.WriteLine(MagnitudeFormatter.Format(random.NextDouble() * Math.Pow(1024, i), sizeUnits2, addSpace: false));
        }
        
        string[] sizeUnits3 = ["", "A", "B", "C", "D"];
        double[] steps3 = [0.1, 0.3, 0.6, 1];

        for (var i = 0; i < 9; i++)
        {
            Console.WriteLine(MagnitudeFormatter.Format(random.NextDouble(), sizeUnits3, steps3));
        }
        
        Console.WriteLine(MagnitudeFormatter.Format(1, sizeUnits3, steps3));
    }
}