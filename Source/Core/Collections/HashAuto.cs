using System;

namespace Core.Collections;

public class HashAuto<TKey, TValue>(Hash<TKey, TValue> hash) where TKey : notnull where TValue : notnull
{
   public HashAutoKey<TKey, TValue> this[TKey key] => new(hash, key);
}

public class HashAutoKey<TKey, TValue>(Hash<TKey, TValue> hash, TKey key) where TKey : notnull where TValue : notnull
{
   public TValue Memo(TValue defaultValue) => hash.Memoize(key, defaultValue);

   public TValue Memo(Func<TKey, TValue> defaultValue) => hash.Memoize(key, defaultValue);

   public TValue Find(TValue defaultValue) => hash.Find(key, defaultValue);

   public TValue Find(Func<TKey, TValue> defaultValue) => hash.Find(key, defaultValue);
}