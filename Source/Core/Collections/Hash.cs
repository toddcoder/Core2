using System;
using System.Collections.Generic;
using System.Threading;
using Core.Applications.Messaging;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class Hash<TKey, TValue> : Dictionary<TKey, TValue>, IHash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   public static Hash<TKey, TValue> operator +(Hash<TKey, TValue> hash, (TKey key, TValue value) tuple)
   {
      hash[tuple.key] = tuple.value;
      return hash;
   }

   protected ReaderWriterLockSlim locker = new(LockRecursionPolicy.SupportsRecursion);

   public MessageEvent<HashArgs<TKey, TValue>> Updated = new();
   public MessageEvent<HashArgs<TKey, TValue>> Removed = new();

   public Hash()
   {
   }

   public Hash(int capacity) : base(capacity)
   {
   }

   public Hash(IEqualityComparer<TKey> comparer) : base(comparer)
   {
   }

   public Hash(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
   {
   }

   public Hash(IDictionary<TKey, TValue> dictionary) : base(dictionary)
   {
   }

   public Hash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
   {
   }

   public Hash(IEnumerable<(TKey key, TValue value)> tuples)
   {
      foreach (var (key, value) in tuples)
      {
         this[key] = value;
      }
   }

   public new void Add(TKey key, TValue value)
   {
      base.Add(key, value);
      Updated.Invoke(new HashArgs<TKey, TValue>(key, value));
   }

   public void Add((TKey key, TValue value) item) => Add(item.key, item.value);

   public void Add(KeyValuePair<TKey, TValue> item) => this[item.Key] = item.Value;

   public new bool Remove(TKey key)
   {
      if (ContainsKey(key))
      {
         var value = this[key];
         var result = base.Remove(key);
         Removed.Invoke(new HashArgs<TKey, TValue>(key, value));

         return result;
      }
      else
      {
         return false;
      }
   }

   public new TValue this[TKey key]
   {
      get
      {
         if (ContainsKey(key))
         {
            try
            {
               locker.EnterReadLock();
               return base[key];
            }
            finally
            {
               locker.ExitReadLock();
            }
         }
         else
         {
            return default!;
         }
      }
      set
      {
         try
         {
            locker.EnterWriteLock();
            if (ContainsKey(key))
            {
               base[key] = value;
               Updated.Invoke(new HashArgs<TKey, TValue>(key, value));
            }
            else
            {
               Add(key, value);
            }
         }
         finally
         {
            locker.ExitWriteLock();
         }
      }
   }

   public IEnumerable<TValue> ValuesFromKeys(IEnumerable<TKey> keys)
   {
      foreach (var key in keys)
      {
         if (Maybe[key] is (true, var value))
         {
            yield return value;
         }
      }
   }

   public Hash<TKey, TValue> GetHash() => this;

   public HashInterfaceMaybe<TKey, TValue> Items => new(this);

   public TValue Find(TKey key, Func<TKey, TValue> defaultValue, bool addIfNotFound = false)
   {
      var _result = Maybe[key];
      if (_result is (true, var result))
      {
         return result;
      }
      else
      {
         var value = defaultValue(key);
         if (addIfNotFound)
         {
            try
            {
               locker.EnterWriteLock();
               Add(key, value);
            }
            catch
            {
            }
            finally
            {
               locker.ExitWriteLock();
            }
         }

         return value;
      }
   }

   public TValue Memoize(TKey key, Func<TKey, TValue> defaultValue, bool alwaysUseDefaultValue = false)
   {
      return alwaysUseDefaultValue ? defaultValue(key) : Find(key, defaultValue, true);
   }

   public TKey[] KeyArray()
   {
      var keys = new TKey[Count];
      Keys.CopyTo(keys, 0);

      return keys;
   }

   public TValue[] ValueArray()
   {
      var values = new TValue[Count];
      Values.CopyTo(values, 0);

      return values;
   }

   public void SetValueTo(TValue value, params TKey[] keys)
   {
      foreach (var key in keys)
      {
         this[key] = value;
      }
   }

   public HashTrying<TKey, TValue> TryTo => new(this);

   public KeyValuePair<TKey, TValue>[] ItemsArray() => [.. this];

   public IEnumerable<(TKey key, TValue value)> Tuples()
   {
      foreach (var (key, value) in this)
      {
         yield return (key, value);
      }
   }

   public void Copy(Hash<TKey, TValue> other)
   {
      foreach (var (key, value) in other)
      {
         this[key] = value;
      }
   }

   public Hash<TKey, TValue> Merge(Hash<TKey, TValue> otherHash)
   {
      Hash<TKey, TValue> result = [];
      foreach (var (key, value) in this)
      {
         result[key] = value;
      }

      foreach (var (key, value) in otherHash)
      {
         result[key] = value;
      }

      return result;
   }

   public Maybe<TValue> Replace(TKey key, TValue newValue)
   {
      var oldValue = Items[key];
      this[key] = newValue;

      return oldValue;
   }

   public Maybe<TValue> OneTime(TKey key)
   {
      if (ContainsKey(key))
      {
         var value = this[key];
         Remove(key);
         return value;
      }
      else
      {
         return nil;
      }
   }

   public void Move(TKey oldKey, TKey newKey)
   {
      var _value = Maybe[oldKey];
      if (_value is (true, var value))
      {
         this[newKey] = value;
         Maybe[oldKey] = nil;
      }
   }

   protected virtual Hash<TKey, TValue> getNewHash() => new(Comparer);

   public virtual Hash<TKey, TValue> Subset(params TKey[] keys)
   {
      var newHash = getNewHash();

      foreach (var key in keys)
      {
         newHash[key] = this[key];
      }

      return newHash;
   }

   public virtual Hash<TKey, TValue> Subset(IEnumerable<TKey> keys) => Subset([.. keys]);

   public HashMaybe<TKey, TValue> Maybe => new(this);

   public void AddKeys(IEnumerable<TKey> keys, TValue value)
   {
      foreach (var key in keys)
      {
         this[key] = value;
      }
   }

   public TValue ValueOrNew(TKey key, Func<TKey, TValue> initializer)
   {
      if (TryGetValue(key, out var value))
      {
         return value;
      }
      else
      {
         value = initializer(key);
         this[key] = value;

         return value;
      }
   }

   public TValue GetValue(TKey key, TValue defaultValue)
   {
      if (ContainsKey(key))
      {
         return this[key];
      }
      else
      {
         return defaultValue;
      }
   }

   public (TValue, TValue) ValueOf(TKey key1, TKey key2, TValue defaultValue)
   {
      var value1 = GetValue(key1, defaultValue);
      var value2 = GetValue(key2, defaultValue);

      return (value1, value2);
   }

   public (TValue, TValue, TValue) ValueOf(TKey key1, TKey key2, TKey key3, TValue defaultValue)
   {
      var value1 = GetValue(key1, defaultValue);
      var value2 = GetValue(key2, defaultValue);
      var value3 = GetValue(key3, defaultValue);

      return (value1, value2, value3);
   }

   public (TValue, TValue, TValue, TValue) ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TValue defaultValue)
   {
      var value1 = GetValue(key1, defaultValue);
      var value2 = GetValue(key2, defaultValue);
      var value3 = GetValue(key3, defaultValue);
      var value4 = GetValue(key4, defaultValue);

      return (value1, value2, value3, value4);
   }

   public (TValue, TValue, TValue, TValue, TValue) ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5, TValue defaultValue)
   {
      var value1 = GetValue(key1, defaultValue);
      var value2 = GetValue(key2, defaultValue);
      var value3 = GetValue(key3, defaultValue);
      var value4 = GetValue(key4, defaultValue);
      var value5 = GetValue(key5, defaultValue);

      return (value1, value2, value3, value4, value5);
   }

   public (TValue, TValue, TValue, TValue, TValue, TValue) ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5, TKey key6,
      TValue defaultValue)
   {
      var value1 = GetValue(key1, defaultValue);
      var value2 = GetValue(key2, defaultValue);
      var value3 = GetValue(key3, defaultValue);
      var value4 = GetValue(key4, defaultValue);
      var value5 = GetValue(key5, defaultValue);
      var value6 = GetValue(key6, defaultValue);

      return (value1, value2, value3, value4, value5, value6);
   }

   public (TValue, TValue, TValue, TValue, TValue, TValue, TValue) ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5, TKey key6,
      TKey key7, TValue defaultValue)
   {
      var value1 = GetValue(key1, defaultValue);
      var value2 = GetValue(key2, defaultValue);
      var value3 = GetValue(key3, defaultValue);
      var value4 = GetValue(key4, defaultValue);
      var value5 = GetValue(key5, defaultValue);
      var value6 = GetValue(key6, defaultValue);
      var value7 = GetValue(key7, defaultValue);

      return (value1, value2, value3, value4, value5, value6, value7);
   }

   public (TValue, TValue, TValue, TValue, TValue, TValue, TValue, TValue) ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5, TKey key6,
      TKey key7, TKey key8, TValue defaultValue)
   {
      var value1 = GetValue(key1, defaultValue);
      var value2 = GetValue(key2, defaultValue);
      var value3 = GetValue(key3, defaultValue);
      var value4 = GetValue(key4, defaultValue);
      var value5 = GetValue(key5, defaultValue);
      var value6 = GetValue(key6, defaultValue);
      var value7 = GetValue(key7, defaultValue);
      var value8 = GetValue(key8, defaultValue);

      return (value1, value2, value3, value4, value5, value6, value7, value8);
   }

   public (TValue, TValue, TValue, TValue, TValue, TValue, TValue, TValue, TValue) ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5,
      TKey key6, TKey key7, TKey key8, TKey key9, TValue defaultValue)
   {
      var value1 = GetValue(key1, defaultValue);
      var value2 = GetValue(key2, defaultValue);
      var value3 = GetValue(key3, defaultValue);
      var value4 = GetValue(key4, defaultValue);
      var value5 = GetValue(key5, defaultValue);
      var value6 = GetValue(key6, defaultValue);
      var value7 = GetValue(key7, defaultValue);
      var value8 = GetValue(key8, defaultValue);
      var value9 = GetValue(key9, defaultValue);

      return (value1, value2, value3, value4, value5, value6, value7, value8, value9);
   }
}