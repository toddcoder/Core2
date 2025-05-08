using Core.Monads;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class StateMemo<TState, TKey, TValue> : IDictionary<TKey, TValue>, IHash<TKey, TValue> where TState : notnull where TKey : notnull
   where TValue : notnull
{
   public static implicit operator Hash<TKey, TValue>(StateMemo<TState, TKey, TValue> memo) => memo.hash;

   protected Hash<TKey, TValue> hash = [];
   protected Func<TState, TKey> keyFunc;
   protected Func<TState, TValue> defaultFunc;

   public StateMemo(Hash<TKey, TValue> hash, Func<TState, TKey> keyFunc, Func<TState, TValue> defaultFunc)
   {
      this.hash = hash;
      this.keyFunc = keyFunc;
      this.defaultFunc = defaultFunc;
   }

   public StateMemo(Func<TState, TKey> keyFunc, Func<TState, TValue> defaultFunc) : this(new Hash<TKey, TValue>(), keyFunc, defaultFunc)
   {
   }

   public StateMemo(Func<TState, TKey> keyFunc, Func<TState, TValue> defaultFunc, IDictionary<TKey, TValue> dictionary) : this(keyFunc, defaultFunc)
   {
      foreach (var item in dictionary)
      {
         hash[item.Key] = item.Value;
      }
   }

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
            value = default!;
            hash[key] = value;

            return value;
         }
      }
      set => hash[key] = value;
   }

   public TValue this[TState state]
   {
      get
      {
         var key = keyFunc(state);
         if (hash.TryGetValue(key, out var value))
         {
            return value;
         }
         else
         {
            value = defaultFunc(state);
            hash[key] = value;

            return value;
         }
      }
      set => hash[keyFunc(state)] = value;
   }

   bool IHash<TKey, TValue>.ContainsKey(TKey key) => hash.ContainsKey(key);

   public ICollection<TKey> Keys => hash.Keys;

   public ICollection<TValue> Values => hash.Values;

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
}