using System;
using System.Collections.Generic;
using System.Text;

namespace CompositeDictionaryStd
{
    public interface ICompositeConcurrentDictionary<TKey1, TKey2, TValue>
    {
        bool TryAdd(TKey1 key1, TKey2 key2, TValue value);
        bool TryGetValue(TKey1 key1, TKey2 key2, out TValue retValue);
    }
}
