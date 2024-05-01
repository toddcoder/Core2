namespace Core.Enumerables;

public static class EnumerableFunctions
{
   public static KeyValuePairEnumerable<TKey, TValue> kv<TKey, TValue>() where TKey : notnull where TValue : notnull => new();

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