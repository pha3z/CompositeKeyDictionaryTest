using System;
using System.Collections.Concurrent;

namespace CompositeDictionaryStd
{
    public class NestedConcurrentDictionary<TKey1, TKey2, TValue> : ICompositeConcurrentDictionary<TKey1, TKey2, TValue>
    {
        readonly ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>> _dic = new ConcurrentDictionary<TKey1, ConcurrentDictionary<TKey2, TValue>>();

        public NestedConcurrentDictionary() { }

        public bool TryAdd(TKey1 key1, TKey2 key2, TValue value)
        {
            ConcurrentDictionary<TKey2, TValue> Values2 = null;
            if (!_dic.TryGetValue(key1, out Values2))
            {
                Values2 = new ConcurrentDictionary<TKey2, TValue>();
                _dic.TryAdd(key1, Values2);
            }

            return Values2.TryAdd(key2, value);
        }

        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue retValue)
        {
            ConcurrentDictionary<TKey2, TValue> Values2 = null;
            if (!_dic.TryGetValue(key1, out Values2))
            {
                retValue = default(TValue);
                return false;
            }

            return Values2.TryGetValue(key2, out retValue);
        }
    }
}
