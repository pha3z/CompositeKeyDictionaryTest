using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace CompositeDictionaryStd
{
    public class CompositeConcurrentDictionary<TKey1, TKey2, TValue> : ICompositeConcurrentDictionary<TKey1, TKey2, TValue>
    {
        readonly ConcurrentDictionary<(TKey1, TKey2), TValue> _dic = new ConcurrentDictionary<(TKey1, TKey2), TValue>();

        public bool TryAdd(TKey1 key1, TKey2 key2, TValue value)
            => _dic.TryAdd((key1, key2), value);

        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue retValue)
            => _dic.TryGetValue((key1, key2), out retValue);
    }
}
