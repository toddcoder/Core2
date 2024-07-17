using System;

namespace Core.Enumerables;

public static class EnumerableFunctions
{
   [Obsolete("Use + operator")]
   public static KeyValuePairEnumerable<TKey, TValue> kv<TKey, TValue>() where TKey : notnull where TValue : notnull => new();

   [Obsolete("Use + operator")]
   public static KeyValuePairEnumerable<TKey, TValue> kv<TKey, TValue>(params (TKey, TValue)[] items) where TKey : notnull where TValue : notnull
   {
      var enumerable = new KeyValuePairEnumerable<TKey, TValue>();
      foreach (var (key, value) in items)
      {
         enumerable.AddKey(key).AddValue(value);
      }

      return enumerable;
   }
}