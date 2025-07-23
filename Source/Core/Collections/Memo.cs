using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public abstract class Memo<TKey, TValue> : IDictionary<TKey, TValue>, IHash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   public class Function : Memo<TKey, TValue>
   {
      protected Func<TKey, TValue> defaultValue;

      public Function(Hash<TKey, TValue> hash, Func<TKey, TValue> defaultValue)
      {
         this.hash = hash;
         this.defaultValue = defaultValue;
      }

      public Function(Func<TKey, TValue> defaultValue) : this(new Hash<TKey, TValue>(), defaultValue)
      {
      }

      public Function(Func<TKey, TValue> defaultValue, IDictionary<TKey, TValue> dictionary) : this(defaultValue)
      {
         foreach (var item in dictionary)
         {
            hash[item.Key] = item.Value;
         }
      }

      public override TValue GetValue(TKey key) => defaultValue(key);
   }

   public class Value : Memo<TKey, TValue>
   {
      protected TValue defaultValue;

      public Value(Hash<TKey, TValue> hash, TValue defaultValue)
      {
         this.hash = hash;
         this.defaultValue = defaultValue;
      }

      public Value(TValue defaultValue) : this(new Hash<TKey, TValue>(), defaultValue)
      {
      }

      public Value(TValue defaultValue, IDictionary<TKey, TValue> dictionary) : this(defaultValue)
      {
         foreach (var item in dictionary)
         {
            hash[item.Key] = item.Value;
         }
      }

      public override TValue GetValue(TKey key) => defaultValue;
   }

   public static implicit operator Hash<TKey, TValue>(Memo<TKey, TValue> memo) => memo.hash;

   protected Hash<TKey, TValue> hash = [];

   public abstract TValue GetValue(TKey key);

   public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => hash.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Add(KeyValuePair<TKey, TValue> item) => hash[item.Key] = item.Value;

   public void Clear() => hash.Clear();

   public bool Contains(KeyValuePair<TKey, TValue> item) => hash.ContainsKey(item.Key);

   public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) => ((IDictionary<TKey, TValue>)hash).CopyTo(array, arrayIndex);

   public bool Remove(KeyValuePair<TKey, TValue> item) => hash.Remove(item.Key);

   public int Count => hash.Count;

   public bool IsReadOnly => false;

   public void Add(TKey key, TValue value) => hash[key] = value;

   bool IDictionary<TKey, TValue>.ContainsKey(TKey key) => hash.ContainsKey(key);

   public Hash<TKey, TValue> GetHash() => hash;

   public HashInterfaceMaybe<TKey, TValue> Items => new(hash);

   public bool Remove(TKey key) => hash.Remove(key);

   public bool TryGetValue(TKey key, [MaybeNullWhen(false)] out TValue value)
   {
      value = this[key];
      return true;
   }

   public TValue this[TKey key]
   {
      get
      {
         if (hash.TryGetValue(key, out var value))
         {
            return value;
         }
         else
         {
            value = GetValue(key);
            hash[key] = value;

            return value;
         }
      }
      set => hash[key] = value;
   }

   bool IHash<TKey, TValue>.ContainsKey(TKey key) => hash.ContainsKey(key);

   public ICollection<TKey> Keys => hash.Keys;

   public ICollection<TValue> Values => hash.Values;

   public bool ContainsKey(TKey key) => hash.ContainsKey(key);

   public Maybe<TValue> Find(TKey key)
   {
      if (hash.TryGetValue(key, out var value))
      {
         return value;
      }
      else
      {
         return nil;
      }
   }

   public IEnumerable<(TKey key, TValue value)> Tuples
   {
      get
      {
         foreach (var key in Keys)
         {
            if (Find(key) is (true, var value))
            {
               yield return (key, value);
            }
         }
      }
   }

   public IEnumerable<TValue> UpdateFromKeys(params TKey[] keys)
   {
      foreach (var key in keys)
      {
         yield return this[key];
      }
   }
}