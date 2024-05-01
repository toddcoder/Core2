using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Core.Collections;

public class TwoValueHash<TKey, TValue1, TValue2>(Func<TKey, TValue1> default1, Func<TKey, TValue2> default2)
   : IHash<TKey, (TValue1, TValue2)>, IEnumerable<(TKey, (TValue1, TValue2))> where TKey : notnull where TValue1 : notnull where TValue2 : notnull
{
   protected Hash<TKey, TValue1> hash1 = [];
   protected Hash<TKey, TValue2> hash2 = [];

   protected TValue1 getValue1(TKey key) => hash1.Maybe[key] | (() => default1(key));

   protected TValue2 getValue2(TKey key) => hash2.Maybe[key] | (() => default2(key));

   public (TValue1, TValue2) this[TKey key]
   {
      get => (getValue1(key), getValue2(key));
      set
      {
         var (value1, value2) = value;
         hash1[key] = value1;
         hash2[key] = value2;
      }
   }

   public bool ContainsKey(TKey key) => hash1.ContainsKey(key);

   public Hash<TKey, (TValue1, TValue2)> GetHash()
   {
      var hash = new Hash<TKey, (TValue1, TValue2)>();
      foreach (var key in hash1.Keys)
      {
         hash[key] = this[key];
      }

      return hash;
   }

   public Hash<TKey, TValue1> GetHash1() => hash1;

   public Hash<TKey, TValue2> GetHash2() => hash2;

   public HashInterfaceMaybe<TKey, (TValue1, TValue2)> Items => new(this);

   public void SetValue1(TKey key, TValue1 value1)
   {
      hash1[key] = value1;
      if (!hash2.ContainsKey(key))
      {
         hash2[key] = getValue2(key);
      }
   }

   public void SetValue2(TKey key, TValue2 value2)
   {
      hash2[key] = value2;
      if (!hash1.ContainsKey(key))
      {
         hash1[key] = getValue1(key);
      }
   }

   public IEnumerator<(TKey, (TValue1, TValue2))> GetEnumerator()
   {
      return hash1.Keys.Select(key => (key, (getValue1(key), getValue2(key)))).GetEnumerator();
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void Clear()
   {
      hash1.Clear();
      hash2.Clear();
   }

   public int Count => hash1.Count;
}