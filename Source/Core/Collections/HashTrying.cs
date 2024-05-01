﻿using System;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public class HashTrying<TKey, TValue> where TKey : notnull where TValue : notnull
{
   protected Hash<TKey, TValue> hash;

   public HashTrying(Hash<TKey, TValue> hash) => this.hash = hash;

   public Result<TValue> this[TKey key] => hash.Must().HaveKeyOf(key).OrFailure().Map(d => d[key].Success());

   public Result<TValue> Find(TKey key, Func<TKey, Result<TValue>> defaultValue, bool addIfNotFound = false)
   {
      var result = this[key];
      if (result)
      {
         return result;
      }
      else
      {
         var _value = defaultValue(key);
         if (_value is (true, var value))
         {
            if (addIfNotFound)
            {
               hash.Add(key, value);
            }

            return value;
         }
         else
         {
            return _value.Exception;
         }
      }
   }

   public Result<TValue> Map(TKey key, string notFoundMessage)
   {
      var _value = hash.Maybe[key];
      if (_value is (true, var value))
      {
         return value;
      }
      else
      {
         return fail(notFoundMessage);
      }
   }

   public Result<TValue> Map(TKey key, Func<string> notFoundMessage)
   {
      var _value = hash.Maybe[key];
      if (_value is (true, var value))
      {
         return value;
      }
      else
      {
         return fail(notFoundMessage());
      }
   }
}