using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using Common;
using static System.Console;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Engines;

namespace CompositeDictionaryStd
{
    public enum TestType : int
    {
        LOOKUP,
        ADD
    }

    public enum KeyType : int
    {
        INT,
        STR
    }

    [SimpleJob(RunStrategy.ColdStart, launchCount: 0, warmupCount: 0, targetCount: 1, invocationCount: 1)]
    public class CompositeDictionaryBenchmarks
    {
        object dic;


        [Params(true, false)]
        public bool UseNestedDic;

        [Params(KeyType.INT, KeyType.STR)]
        public KeyType Key1_Type;

        [Params(KeyType.INT, KeyType.STR)]
        public KeyType Key2_Type;

        [Params(TestType.LOOKUP, TestType.ADD)]
        public TestType WhichTest;

        FastShuffledRange _values;
        FastShuffledRange _secondKeys;

        bool Key1_IsString => Key1_Type == KeyType.STR;
        bool Key2_IsString => Key2_Type == KeyType.STR;

        bool _keysHaveBeenAdded = true;
        bool _hasWarmupRun = false;

        [GlobalSetup]
        public void Setup()
        {
            _keysHaveBeenAdded = false;

            int dataSize = 2000000;
            var rnd = new Random();
            _values = new FastShuffledRange(0, dataSize, rnd);
            _secondKeys = new FastShuffledRange(0, 10, rnd);

            if (UseNestedDic)
            {
                if (Key1_IsString && Key2_IsString)
                    dic = new NestedConcurrentDictionary<string, string, int>();
                else if (!Key1_IsString && !Key2_IsString)
                    dic = new NestedConcurrentDictionary<int, int, int>();
                if (Key1_IsString && !Key2_IsString)
                    dic = new NestedConcurrentDictionary<string, int, int>();
                if (!Key1_IsString && Key2_IsString)
                    dic = new NestedConcurrentDictionary<int, string, int>();
            }
            else
            {
                if (Key1_IsString && Key2_IsString)
                    dic = new CompositeConcurrentDictionary<string, string, int>();
                else if (!Key1_IsString && !Key2_IsString)
                    dic = new CompositeConcurrentDictionary<int, int, int>();
                if (Key1_IsString && !Key2_IsString)
                    dic = new CompositeConcurrentDictionary<string, int, int>();
                if (!Key1_IsString && Key2_IsString)
                    dic = new CompositeConcurrentDictionary<int, string, int>();
            }

            //For lookup tests, add the keys first so that adding isn't part of the timed test.
            if (WhichTest == TestType.LOOKUP)
                DoAddKeys();

            WriteLine("Global Setup Completed");

        }

        [Benchmark(Baseline = false, Description = "", OperationsPerInvoke = 1)]
        public void RunTest()
        {
            //This is incredibly stupid.
            //I could not figure out how to stop benchmarkdotnet
            //from invoking the RunTest() method in a warm-up fashion
            //before invoking it for the actual test
            //As a result of this, the dictionary elements were being added twice druing an ADD test
            //So we bail out of the warmup run using a flag.
            if(!_hasWarmupRun)
            {
                _hasWarmupRun = true;
                return;
            }

            _values.SetPosition(0);
            _secondKeys.SetPosition(0);

            if (WhichTest == TestType.ADD)
                DoAddKeys();
            else
                DoLookup();

            string testType = UseNestedDic ? "Nested Dic" : "Composite Dic";
            WriteLine($"Completed test {testType}. Params: {WhichTest} | {Key1_Type }, {Key2_Type }");
        }

        private void DoAddKeys()
        {
            if (_keysHaveBeenAdded)
                throw new InvalidOperationException("Duplicate call to DoAddKeys during a test");

            int key1;
            int key2;

            for (int i = 0; i < _values.Length; i++)
            {
                key1 = _values.NextValue();

                if (!_secondKeys.HasValuesRemaining)
                    _secondKeys.SetPosition(0);

                key2 = _secondKeys.NextValue();

                if (Key1_IsString && Key2_IsString)
                    ((ICompositeConcurrentDictionary<string, string, int>)dic)
                        .TryAdd(key1.ToString(), key2.ToString(), key1);

                else if (!Key1_IsString && !Key2_IsString)
                    ((ICompositeConcurrentDictionary<int, int, int>)dic)
                        .TryAdd(key1, key2, key1);

                else if (!Key1_IsString && Key2_IsString)
                    ((ICompositeConcurrentDictionary<int, string, int>)dic)
                        .TryAdd(key1, key2.ToString(), key1);

                else if (Key1_IsString && !Key2_IsString)
                    ((ICompositeConcurrentDictionary<string, int, int>)dic)
                        .TryAdd(key1.ToString(), key2, key1);
            }

            _keysHaveBeenAdded = true;
        }

        private void DoLookup()
        {
            int key1;
            int key2;
            int output;

            for (int i = 0; i < _values.Length; i++)
            {
                key1 = _values.NextValue();

                if (!_secondKeys.HasValuesRemaining)
                    _secondKeys.SetPosition(0);
                
                key2 = _secondKeys.NextValue();

                if (Key1_IsString && Key2_IsString)
                    ((ICompositeConcurrentDictionary<string, string, int>)dic)
                        .TryGetValue(key1.ToString(), key2.ToString(), out output);

                else if (!Key1_IsString && !Key2_IsString)
                    ((ICompositeConcurrentDictionary<int, int, int>)dic)
                        .TryGetValue(key1, key2, out output);

                else if (!Key1_IsString && Key2_IsString)
                    ((ICompositeConcurrentDictionary<int, string, int>)dic)
                        .TryGetValue(key1, key2.ToString(), out output);

                else if (Key1_IsString && !Key2_IsString)
                    ((ICompositeConcurrentDictionary<string, int, int>)dic)
                        .TryGetValue(key1.ToString(), key2, out output);

            }
        }
    }
}
