using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SuntionCore.Services.HashUtils;

namespace SuntionCore.Tests.Services.HashUtils;

[TestClass]
[TestSubject(typeof(HashUtil))]
public class HashUtilTest
{

    [TestMethod]
    public void Method()
    {
        string[] tests =
        [
            "Hello World",
            "Hello World",
            "Hello World.",
            "666"
        ];

        foreach (HashAlgo hashAlgo in Enum.GetValues<HashAlgo>())
        {
            Console.WriteLine($"{hashAlgo}:");
            foreach (string test in tests)
            {
                Console.WriteLine($" - {test} --hash-> {HashUtil.Hash(test, hashAlgo)}");
            }
        }
    }
}