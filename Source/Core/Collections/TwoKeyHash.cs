using System.Collections.Generic;

namespace Core.Collections;

public class TwoKeyHash<TKey1, TKey2, TValue> : Hash<TKey1, TValue> where TKey1 : notnull where TKey2 : notnull where TValue : notnull
{
   protected BackHash<TKey1, TKey2> backHash = new();

   public TwoKeyHash()
   {
   }

   public TwoKeyHash(int capacity) : base(capacity)
   {
   }

   public TwoKeyHash(IEqualityComparer<TKey1> comparer) : base(comparer)
   {
   }

   public TwoKeyHash(int capacity, IEqualityComparer<TKey1> comparer) : base(capacity, comparer)
   {
   }

   public TwoKeyHash(IDictionary<TKey1, TValue> dictionary) : base(dictionary)
   {
   }

   public TwoKeyHash(IDictionary<TKey1, TValue> dictionary, IEqualityComparer<TKey1> comparer) : base(dictionary, comparer)
   {
   }

   public TValue this[TKey1 key1, TKey2 key2]
   {
      get => base[key1];
      set
      {
         base[key1] = value;
         backHash[key1] = key2;
      }
   }

   public TValue this[TKey2 alternateKey] => base[backHash.Back[alternateKey]];

   public bool ContainsAlternateKey(TKey2 key) => backHash.Back.ContainsKey(key);

   public new void Clear()
   {
      base.Clear();
      backHash.Clear();
   }
}