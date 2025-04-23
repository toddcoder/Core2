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

   public TValue Value
   {
      get => hash[key];
      set => hash[key] = value;
   }
}

public class MemoFunction<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash;
   protected Func<TKey, TValue> defaultValue;

   public MemoFunction(Hash<TKey, TValue> hash, Func<TKey, TValue> defaultValue)
   {
      this.hash = hash;
      this.defaultValue = defaultValue;
   }

   public TValue this[TKey key]
   {
      get => hash.Memoize(key, defaultValue);
      set => hash[key] = value;
   }

   public Hash<TKey, TValue> Hash => hash;
}

public class MemoValue<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash;
   protected TValue defaultValue;

   public MemoValue(Hash<TKey, TValue> hash, TValue defaultValue)
   {
      this.hash = hash;
      this.defaultValue = defaultValue;
   }

   public TValue this[TKey key]
   {
      get => hash.Memoize(key, defaultValue);
      set => hash[key] = value;
   }

   public Hash<TKey, TValue> Hash => hash;
}