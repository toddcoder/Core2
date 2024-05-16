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

   public Maybe<TValue> Memoize(TKey key, Func<TKey, Maybe<TValue>> defaultValue, bool alwaysUseDefaultValue = false)
   {
      return alwaysUseDefaultValue ? defaultValue(key) : Find(key, defaultValue, true);
   }
}