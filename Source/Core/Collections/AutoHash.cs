﻿using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

[Obsolete("Use memos")]
public class AutoHash<TKey, TValue> : Hash<TKey, TValue> where TKey : notnull where TValue : notnull
{
   public static AutoHash<TKey, TValue> operator +(AutoHash<TKey, TValue> hash, (TKey key, TValue value) tuple)
   {
      hash[tuple.key] = tuple.value;
      return hash;
   }

   protected Maybe<Func<TKey, TValue>> _defaultLambda = nil;
   protected Maybe<TValue> _defaultValue = nil;

   public AutoHash()
   {
   }

   public AutoHash(int capacity) : base(capacity)
   {
   }

   public AutoHash(IEqualityComparer<TKey> comparer) : base(comparer)
   {
   }

   public AutoHash(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
   {
   }

   public AutoHash(IDictionary<TKey, TValue> dictionary) : base(dictionary)
   {
   }

   public AutoHash(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(dictionary, comparer)
   {
   }

   public AutoHash(Func<TKey, TValue> defaultLambda, bool autoAddDefault = false)
   {
      _defaultLambda = defaultLambda;
      AutoAddDefault = autoAddDefault;
   }

   public AutoHash(Func<TKey, TValue> defaultLambda, IEqualityComparer<TKey> comparer, bool autoAddDefault = false) : this(comparer)
   {
      _defaultLambda = defaultLambda;
      AutoAddDefault = autoAddDefault;
   }

   public AutoHash(Func<TKey, TValue> defaultLambda, bool autoAddDefault, IEqualityComparer<TKey> comparer) : this(comparer)
   {
      _defaultLambda = defaultLambda;
      AutoAddDefault = autoAddDefault;

      _defaultValue = nil;
   }

   public AutoHash(TValue defaultValue, bool autoAddDefault = false)
   {
      _defaultValue = defaultValue;
      AutoAddDefault = autoAddDefault;

      _defaultLambda = nil;
   }

   public AutoHash(TValue defaultValue, bool autoAddDefault, IEqualityComparer<TKey> comparer) : this(comparer)
   {
      DefaultValue = defaultValue;
      AutoAddDefault = autoAddDefault;

      _defaultLambda = nil;
   }

   public DefaultType Default
   {
      get
      {
         if (_defaultLambda)
         {
            return DefaultType.Lambda;
         }
         else if (_defaultValue)
         {
            return DefaultType.Value;
         }
         else
         {
            return DefaultType.None;
         }
      }
   }

   public TValue DefaultValue
   {
      set => _defaultValue = value;
   }

   public Func<TKey, TValue> DefaultLambda
   {
      set => _defaultLambda = value;
   }

   public bool AutoAddDefault { get; set; }

   public TValue GetValue(TKey key)
   {
      if (ContainsKey(key))
      {
         return base[key];
      }
      else if (_defaultLambda is (true, var defaultLambda))
      {
         return defaultLambda(key);
      }
      else if (_defaultValue is (true, var defaultValue))
      {
         return defaultValue;
      }
      else
      {
         return base[key];
      }
   }

   public new TValue this[TKey key]
   {
      get
      {
         if (ContainsKey(key))
         {
            return base[key];
         }
         else if (_defaultLambda is (true, var defaultLambda))
         {
            var result = defaultLambda(key);
            if (AutoAddDefault)
            {
               this[key] = result;
            }

            return result;
         }
         else if (_defaultValue is (true, var defaultValue))
         {
            if (AutoAddDefault)
            {
               this[key] = defaultValue;
            }

            return defaultValue;
         }
         else
         {
            return base[key];
         }
      }
      set => base[key] = value;
   }

   protected override Hash<TKey, TValue> getNewHash()
   {
      if (_defaultLambda is (true, var defaultLambda))
      {
         return new AutoHash<TKey, TValue>(defaultLambda, Comparer, AutoAddDefault);
      }
      else if (_defaultValue is (true, var defaultValue))
      {
         return new AutoHash<TKey, TValue>(defaultValue, AutoAddDefault, Comparer);
      }
      else
      {
         return new AutoHash<TKey, TValue>(Comparer);
      }
   }

   public void AddKeys(IEnumerable<TKey> keys)
   {
      foreach (var key in keys)
      {
         this[key] = GetValue(key);
      }
   }
}