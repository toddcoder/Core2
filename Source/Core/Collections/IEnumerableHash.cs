using System.Collections.Generic;

namespace Core.Collections;

public interface IEnumerableHash<TKey, TValue> : IHash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   IEnumerable<(TKey, TValue)> Enumerable();
}