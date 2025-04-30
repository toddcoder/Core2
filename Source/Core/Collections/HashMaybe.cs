using System;
using Core.Monads;
using Core.Monads.Lazy;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class HashMaybe<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash;

   public HashMaybe(Hash<TKey, TValue> hash)
   {
      this.hash = hash;
   }

   public Maybe<TValue> this[TKey key]
   {
      get
      {
         if (hash.TryGetValue(key, out var item))
         {
            return item;
         }
         else
         {
            return nil;
         }
      }
      set
      {
         if (value)
         {
            hash[key] = value;
         }
         else
         {
            hash.Remove(key);
         }
      }
   }

   public Maybe<TValue> Find(TKey key, Func<TKey, Maybe<TValue>> defaultValue, bool addIfNotFound)
   {
      LazyMaybe<TValue> _fromDefaultValue = nil;
      if (this[key] is (true, var result))
      {
         return result;
      }
      else if (_fromDefaultValue.ValueOf(defaultValue(key)) is (true, var value))
      {
         if (addIfNotFound)
         {
            hash[key] = value;
         }

         return value;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<TValue> GetValue(TKey key) => hash.ContainsKey(key) ? this[key] : nil;

   public Maybe<(TValue, TValue)> ValueOf(TKey key1, TKey key2)
   {
      return
         from value1 in GetValue(key1)
         from value2 in GetValue(key2)
         select (value1, value2);
   }

   public Maybe<(TValue, TValue, TValue)> ValueOf(TKey key1, TKey key2, TKey key3)
   {
      return
         from value1 in GetValue(key1)
         from value2 in GetValue(key2)
         from value3 in GetValue(key3)
         select (value1, value2, value3);
   }

   public Maybe<(TValue, TValue, TValue, TValue)> ValueOf(TKey key1, TKey key2, TKey key3, TKey key4)
   {
      return
         from value1 in GetValue(key1)
         from value2 in GetValue(key2)
         from value3 in GetValue(key3)
         from value4 in GetValue(key4)
         select (value1, value2, value3, value4);
   }

   public Maybe<(TValue, TValue, TValue, TValue, TValue)> ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5)
   {
      return
         from value1 in GetValue(key1)
         from value2 in GetValue(key2)
         from value3 in GetValue(key3)
         from value4 in GetValue(key4)
         from value5 in GetValue(key5)
         select (value1, value2, value3, value4, value5);
   }

   public Maybe<(TValue, TValue, TValue, TValue, TValue, TValue)> ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5, TKey key6)
   {
      return
         from value1 in GetValue(key1)
         from value2 in GetValue(key2)
         from value3 in GetValue(key3)
         from value4 in GetValue(key4)
         from value5 in GetValue(key5)
         from value6 in GetValue(key6)
         select (value1, value2, value3, value4, value5, value6);
   }

   public Maybe<(TValue, TValue, TValue, TValue, TValue, TValue, TValue)> ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5, TKey key6,
      TKey key7)
   {
      return
         from value1 in GetValue(key1)
         from value2 in GetValue(key2)
         from value3 in GetValue(key3)
         from value4 in GetValue(key4)
         from value5 in GetValue(key5)
         from value6 in GetValue(key6)
         from value7 in GetValue(key7)
         select (value1, value2, value3, value4, value5, value6, value7);
   }

   public Maybe<(TValue, TValue, TValue, TValue, TValue, TValue, TValue, TValue)> ValueOf(TKey key1, TKey key2, TKey key3, TKey key4, TKey key5,
      TKey key6, TKey key7, TKey key8)
   {
      return
         from value1 in GetValue(key1)
         from value2 in GetValue(key2)
         from value3 in GetValue(key3)
         from value4 in GetValue(key4)
         from value5 in GetValue(key5)
         from value6 in GetValue(key6)
         from value7 in GetValue(key7)
         from value8 in GetValue(key8)
         select (value1, value2, value3, value4, value5, value6, value7, value8);
   }

   public Maybe<(TValue, TValue, TValue, TValue, TValue, TValue, TValue, TValue, TValue)> ValueOf(TKey key1, TKey key2, TKey key3, TKey key4,
      TKey key5, TKey key6, TKey key7, TKey key8, TKey key9)
   {
      return
         from value1 in GetValue(key1)
         from value2 in GetValue(key2)
         from value3 in GetValue(key3)
         from value4 in GetValue(key4)
         from value5 in GetValue(key5)
         from value6 in GetValue(key6)
         from value7 in GetValue(key7)
         from value8 in GetValue(key8)
         from value9 in GetValue(key9)
         select (value1, value2, value3, value4, value5, value6, value7, value8, value9);
   }
}