using System.Collections.Generic;

namespace Core.Collections;

public class OneShotHash<TKey, TValue> : Hash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   public OneShotHash()
   {
   }

   public OneShotHash(IDictionary<TKey, TValue> dictionary)
   {
      foreach (var (key, value) in dictionary)
      {
         this[key] = value;
      }
   }

   public OneShotHash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(comparer)
   {
      foreach (var (key, value) in dictionary)
      {
         this[key] = value;
      }
   }

   public new TValue this[TKey key]
   {
      get => base[key];
      set
      {
         if (!ContainsKey(key))
         {
            base[key] = value;
         }
      }
   }

   public new void Add(TKey key, TValue value)
   {
      if (!ContainsKey(key))
      {
         base.Add(key, value);
      }
   }
}