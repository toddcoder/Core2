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

public class HashMemoFunction<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash;
   protected Func<TKey, TValue> defaultValue;

   public HashMemoFunction(Hash<TKey, TValue> hash, Func<TKey, TValue> defaultValue)
   {
      this.hash = hash;
      this.defaultValue = defaultValue;
   }

   public HashMemoKeyFunction<TKey, TValue> this[TKey key] => new(hash, key, defaultValue);

   public Hash<TKey, TValue> Hash => hash;
}

public class HashMemoKeyFunction<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash;
   protected TKey key;
   protected Func<TKey, TValue> defaultValue;

   public HashMemoKeyFunction(Hash<TKey, TValue> hash, TKey key, Func<TKey, TValue> defaultValue)
   {
      this.hash = hash;
      this.key = key;
      this.defaultValue = defaultValue;
   }

   public TValue Value
   {
      get => hash.Memoize(key, defaultValue);
      set => hash[key] = value;
   }

   public Hash<TKey, TValue> Hash => hash;
}

public class HashMemoValue<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash;
   protected TValue defaultValue;

   public HashMemoValue(Hash<TKey, TValue> hash, TValue defaultValue)
   {
      this.hash = hash;
      this.defaultValue = defaultValue;
   }

   public HashMemoKeyValue<TKey, TValue> this[TKey key] => new(hash, key, defaultValue);

   public Hash<TKey, TValue> Hash => hash;
}

public class HashMemoKeyValue<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash;
   protected TKey key;
   protected TValue defaultValue;

   public HashMemoKeyValue(Hash<TKey, TValue> hash, TKey key, TValue defaultValue)
   {
      this.hash = hash;
      this.key = key;
      this.defaultValue = defaultValue;
   }

   public TValue Value
   {
      get => hash.Memoize(key, defaultValue);
      set => hash[key] = value;
   }

   public Hash<TKey, TValue> Hash => hash;
}