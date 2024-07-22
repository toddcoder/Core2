using System.Collections.Generic;

namespace Core.Collections;

public class BackHash<TKey, TValue> : Hash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TValue, TKey> backHash = [];

   public BackHash()
   {
   }

   public BackHash(BackHash<TValue, TKey> backHash) : this()
   {
      foreach (var item in backHash)
      {
         this[item.Value] = item.Key;
      }
   }

   public BackHash(int capacity) : base(capacity)
   {
   }

   public BackHash(IEqualityComparer<TKey> comparer) : base(comparer)
   {
   }

   public BackHash(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
   {
   }

   public BackHash(IDictionary<TKey, TValue> dictionary) : base(dictionary)
   {
   }

   public BackHash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
   {
   }

   public new TValue this[TKey key]
   {
      get => base[key];
      set
      {
         base[key] = value;
         backHash[value] = key;
      }
   }

   public virtual TKey ValueToKey(TValue value) => backHash[value];

   public virtual Hash<TValue, TKey> Back => backHash;

   public new void Clear()
   {
      base.Clear();
      backHash.Clear();
   }

   protected override Hash<TKey, TValue> getNewHash() => new BackHash<TKey, TValue>(Comparer);

   public override Hash<TKey, TValue> Subset(params TKey[] keys)
   {
      var newHash = (BackHash<TKey, TValue>)getNewHash();
      foreach (var key in keys)
      {
         newHash[key] = this[key];
      }

      return newHash;
   }

   public void Deconstruct(out Hash<TKey, TValue> forward, out Hash<TValue, TKey> backward)
   {
      forward = [];
      backward = backHash;
   }
}