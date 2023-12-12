using System.Collections;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables;

public class KeyValuePairEnumerable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>> where TKey : notnull where TValue : notnull
{
   public static KeyValuePairEnumerable<TKey, TValue> operator |(KeyValuePairEnumerable<TKey, TValue> enumerable, TKey key)
   {
      return enumerable.AddKey(key);
   }

   public static KeyValuePairEnumerable<TKey, TValue> operator |(KeyValuePairEnumerable<TKey, TValue> enumerable, TValue value)
   {
      return enumerable.AddValue(value);
   }

   protected Maybe<TKey> _currentKey = nil;
   protected List<KeyValuePair<TKey, TValue>> keyValuePairs = [];

   public KeyValuePairEnumerable<TKey, TValue> AddKey(TKey key)
   {
      _currentKey = key;
      return this;
   }

   public KeyValuePairEnumerable<TKey, TValue> AddValue(TValue value)
   {
      if (_currentKey is (true, var key))
      {
         keyValuePairs.Add(new KeyValuePair<TKey, TValue>(key, value));
      }

      return this;
   }

   public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => keyValuePairs.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}