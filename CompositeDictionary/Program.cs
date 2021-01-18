using System;
using BenchmarkDotNet;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using CompositeDictionaryStd;

namespace CompositeDictionary
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            
            BenchmarkRunner.Run<CompositeDictionaryBenchmarks>();
            return;

            //If you want to manually run methods in a typical debug fashion,
            //Comment out the Run() method and return; statement above
            //Run the code below and debug.
            var bm = new CompositeDictionaryBenchmarks();
            bm.UseNestedDic = false;
            bm.Key1_Type = KeyType.STR;
            bm.Key2_Type = KeyType.INT;
            bm.WhichTest = TestType.ADD;
            bm.Setup();
            bm.RunTest();

        }
    }
}
