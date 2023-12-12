using System;

namespace Core.Collections;

internal class HashBuilder<TKey, TValue> where TKey : notnull where TValue : notnull
{
   internal static Hash<TKey, TValue> Create(ReadOnlySpan<(TKey, TValue)> values)
   {
      Hash<TKey, TValue> hash = [];
      foreach (var (key, value) in values)
      {
         hash[key] = value;
      }

      return hash;
   }
}