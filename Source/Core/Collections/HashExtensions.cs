using System;
using System.Collections.Generic;
using Core.Computers;
using Core.Configurations;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Collections;

public static class HashExtensions
{
   public static TValue Value<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key) where TKey : notnull where TValue : notnull
   {
      return hash.Value(key, $"Value for key {key} not found");
   }

   public static TValue Value<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, string message) where TKey : notnull where TValue : notnull
   {
      if (hash.ContainsKey(key))
      {
         return hash[key];
      }
      else
      {
         throw fail(message);
      }
   }

   public static Result<TValue> Require<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key) where TKey : notnull where TValue : notnull
   {
      return hash.Require(key, $"Value for key {key} not found");
   }

   public static Result<TValue> Require<TKey, TValue>(this IHash<TKey, TValue> hash, TKey key, string message) where TKey : notnull
      where TValue : notnull
   {
      if (hash.ContainsKey(key))
      {
         return hash[key];
      }
      else
      {
         return fail(message);
      }
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this Hash<TKey, TValue> hash, TValue defaultValue) where TKey : notnull
      where TValue : notnull
   {
      return new AutoHash<TKey, TValue>(hash, hash.Comparer) { DefaultValue = defaultValue };
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this StringHash<TValue> hash, TValue defaultValue) where TValue : notnull
   {
      return new AutoStringHash<TValue>(hash) { DefaultValue = defaultValue };
   }

   public static AutoStringHash ToAutoStringHash(this StringHash hash, string defaultValue)
   {
      return new AutoStringHash(hash) { DefaultValue = defaultValue };
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this Hash<TKey, TValue> hash,
      Func<TKey, TValue> defaultLambda) where TKey : notnull where TValue : notnull
   {
      return new AutoHash<TKey, TValue>(hash, hash.Comparer) { DefaultLambda = defaultLambda };
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this StringHash<TValue> hash, Func<string, TValue> defaultLambda)
      where TValue : notnull
   {
      return new AutoStringHash<TValue>(hash) { DefaultLambda = defaultLambda };
   }

   public static AutoStringHash ToAutoStringHash(this StringHash hash, Func<string, string> defaultLambda)
   {
      return new AutoStringHash(hash) { DefaultLambda = defaultLambda };
   }

   public static string Format(this Hash<string, string> hash, string format) => new Formatter(hash).Format(format);

   public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> item, out TKey key, out TValue value)
   {
      key = item.Key;
      value = item.Value;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this Dictionary<TKey, TValue> dictionary) where TKey : notnull where TValue : notnull
   {
      return new Hash<TKey, TValue>(dictionary);
   }

   public static StringHash<TValue> ToStringHash<TValue>(this Dictionary<string, TValue> dictionary) where TValue : notnull
   {
      return new StringHash<TValue>(dictionary);
   }

   public static StringHash ToStringHash(this Dictionary<string, string> dictionary)
   {
      return new StringHash(dictionary);
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
      where TKey : notnull where TValue : notnull
   {
      return new Hash<TKey, TValue>(dictionary, comparer);
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> keySelector) where TKey : notnull
      where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this Span<TValue> span, Func<TValue, TKey> keySelector) where TKey : notnull
      where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this ReadOnlySpan<TValue> span, Func<TValue, TKey> keySelector) where TKey : notnull
      where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue>(this IEnumerable<TValue> enumerable, Func<TValue, string> keySelector)
      where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue>(this Span<TValue> enumerable, Func<TValue, string> keySelector)
      where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue>(this ReadOnlySpan<TValue> enumerable, Func<TValue, string> keySelector)
      where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static StringHash ToStringHash<TValue>(this IEnumerable<TValue> enumerable, Func<TValue, string> keySelector,
      Func<TValue, string> valueSelector)
   {
      StringHash result = [];
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static StringHash ToStringHash<TValue>(this Span<TValue> span, Func<TValue, string> keySelector,
      Func<TValue, string> valueSelector)
   {
      StringHash result = [];
      foreach (var item in span)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static StringHash ToStringHash<TValue>(this ReadOnlySpan<TValue> span, Func<TValue, string> keySelector,
      Func<TValue, string> valueSelector)
   {
      StringHash result = [];
      foreach (var item in span)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> keySelector,
      IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this Span<TValue> span, Func<TValue, TKey> keySelector,
      IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this ReadOnlySpan<TValue> span, Func<TValue, TKey> keySelector,
      IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         result[keySelector(item)] = item;
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this Span<T> span, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this ReadOnlySpan<T> span, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue, T>(this IEnumerable<T> enumerable, Func<T, string> keySelector,
      Func<T, TValue> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue, T>(this Span<T> span, Func<T, string> keySelector,
      Func<T, TValue> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in span)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue, T>(this ReadOnlySpan<T> span, Func<T, string> keySelector,
      Func<T, TValue> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in span)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in enumerable)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this Span<T> span, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue, T>(this ReadOnlySpan<T> span, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         result[keySelector(item)] = valueSelector(item);
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<(TKey, TValue)> enumerable) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this Span<(TKey, TValue)> span) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this ReadOnlySpan<(TKey, TValue)> span) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this IEnumerable<(TKey, TValue)> enumerable, Func<TKey, TValue> defaultLambda,
      bool autoAddDefault = false) where TKey : notnull where TValue : notnull
   {
      var result = new AutoHash<TKey, TValue>(defaultLambda, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this Span<(TKey, TValue)> span, Func<TKey, TValue> defaultLambda,
      bool autoAddDefault = false) where TKey : notnull where TValue : notnull
   {
      var result = new AutoHash<TKey, TValue>(defaultLambda, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this ReadOnlySpan<(TKey, TValue)> span, Func<TKey, TValue> defaultLambda,
      bool autoAddDefault = false) where TKey : notnull where TValue : notnull
   {
      var result = new AutoHash<TKey, TValue>(defaultLambda, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this IEnumerable<(TKey, TValue)> enumerable, TValue defaultValue,
      bool autoAddDefault = false) where TKey : notnull where TValue : notnull
   {
      var result = new AutoHash<TKey, TValue>(defaultValue, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this Span<(TKey, TValue)> span, TValue defaultValue,
      bool autoAddDefault = false) where TKey : notnull where TValue : notnull
   {
      var result = new AutoHash<TKey, TValue>(defaultValue, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoHash<TKey, TValue> ToAutoHash<TKey, TValue>(this ReadOnlySpan<(TKey, TValue)> span, TValue defaultValue,
      bool autoAddDefault = false) where TKey : notnull where TValue : notnull
   {
      var result = new AutoHash<TKey, TValue>(defaultValue, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToStringHash(this IEnumerable<(string, string)> enumerable)
   {
      StringHash result = [];
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToStringHash(this Span<(string, string)> span)
   {
      StringHash result = [];
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToStringHash(this ReadOnlySpan<(string, string)> span)
   {
      StringHash result = [];
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToAutoStringHash(this IEnumerable<(string, string)> enumerable, Func<string, string> defaultLambda,
      bool autoAddDefault = false)
   {
      var result = new AutoStringHash(defaultLambda, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToAutoStringHash(this Span<(string, string)> span, Func<string, string> defaultLambda,
      bool autoAddDefault = false)
   {
      var result = new AutoStringHash(defaultLambda, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToAutoStringHash(this ReadOnlySpan<(string, string)> span, Func<string, string> defaultLambda,
      bool autoAddDefault = false)
   {
      var result = new AutoStringHash(defaultLambda, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToAutoStringHash(this IEnumerable<(string, string)> enumerable, string defaultValue, bool autoAddDefault = false)
   {
      var result = new AutoStringHash(defaultValue, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToAutoStringHash(this Span<(string, string)> span, string defaultValue, bool autoAddDefault = false)
   {
      var result = new AutoStringHash(defaultValue, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash ToAutoStringHash(this ReadOnlySpan<(string, string)> span, string defaultValue, bool autoAddDefault = false)
   {
      var result = new AutoStringHash(defaultValue, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue>(this IEnumerable<(string, TValue)> enumerable) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue>(this Span<(string, TValue)> span) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static StringHash<TValue> ToStringHash<TValue>(this ReadOnlySpan<(string, TValue)> span) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this IEnumerable<(string, TValue)> enumerable, Func<string, TValue> defaultLambda,
      bool autoAddDefault = false) where TValue : notnull
   {
      var result = new AutoStringHash<TValue>(defaultLambda, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this Span<(string, TValue)> span, Func<string, TValue> defaultLambda,
      bool autoAddDefault = false) where TValue : notnull
   {
      var result = new AutoStringHash<TValue>(defaultLambda, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this ReadOnlySpan<(string, TValue)> span, Func<string, TValue> defaultLambda,
      bool autoAddDefault = false) where TValue : notnull
   {
      var result = new AutoStringHash<TValue>(defaultLambda, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this IEnumerable<(string, TValue)> enumerable, TValue defaultValue,
      bool autoAddDefault = false) where TValue : notnull
   {
      var result = new AutoStringHash<TValue>(defaultValue, autoAddDefault);
      foreach (var (key, value) in enumerable)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this Span<(string, TValue)> span, TValue defaultValue,
      bool autoAddDefault = false) where TValue : notnull
   {
      var result = new AutoStringHash<TValue>(defaultValue, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static AutoStringHash<TValue> ToAutoStringHash<TValue>(this ReadOnlySpan<(string, TValue)> span, TValue defaultValue,
      bool autoAddDefault = false) where TValue : notnull
   {
      var result = new AutoStringHash<TValue>(defaultValue, autoAddDefault);
      foreach (var (key, value) in span)
      {
         result[key] = value;
      }

      return result;
   }

   public static BackHash<TKey, TValue> ToBackHash<TKey, TValue>(this Hash<TKey, TValue> hash) where TKey : notnull where TValue : notnull
   {
      BackHash<TKey, TValue> result = [];
      foreach (var (key, value) in hash)
      {
         result[key] = value;
      }

      return result;
   }

   public static BackHash<string, TValue> ToBackHash<TValue>(this StringHash<TValue> hash) where TValue : notnull
   {
      BackHash<string, TValue> result = [];
      foreach (var (key, value) in hash)
      {
         result[key] = value;
      }

      return result;
   }

   public static BackHash<string, string> ToBackHash(this StringHash hash)
   {
      BackHash<string, string> result = [];
      foreach (var (key, value) in hash)
      {
         result[key] = value;
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
      Func<T, Result<TValue>> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in enumerable)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this Span<T> span, Func<T, TKey> keySelector,
      Func<T, Result<TValue>> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this ReadOnlySpan<T> span, Func<T, TKey> keySelector,
      Func<T, Result<TValue>> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this IEnumerable<T> enumerable, Func<T, string> keySelector,
      Func<T, Result<TValue>> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in enumerable)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this Span<T> span, Func<T, string> keySelector,
      Func<T, Result<TValue>> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in span)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this ReadOnlySpan<T> span, Func<T, string> keySelector,
      Func<T, Result<TValue>> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in span)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
      Func<T, Result<TValue>> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in enumerable)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this Span<T> span, Func<T, TKey> keySelector,
      Func<T, Result<TValue>> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this ReadOnlySpan<T> span, Func<T, TKey> keySelector,
      Func<T, Result<TValue>> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         var _selector = valueSelector(item);
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, Result<TKey>> keySelector,
      Func<T, Result<TValue>> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in enumerable)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this Span<T> span, Func<T, Result<TKey>> keySelector,
      Func<T, Result<TValue>> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this ReadOnlySpan<T> span, Func<T, Result<TKey>> keySelector,
      Func<T, Result<TValue>> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this IEnumerable<T> enumerable, Func<T, Result<string>> keySelector,
      Func<T, Result<TValue>> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in enumerable)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this Span<T> span, Func<T, Result<string>> keySelector,
      Func<T, Result<TValue>> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in span)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this ReadOnlySpan<T> span, Func<T, Result<string>> keySelector,
      Func<T, Result<TValue>> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in span)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, Result<TKey>> keySelector,
      Func<T, Result<TValue>> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in enumerable)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this Span<T> span, Func<T, Result<TKey>> keySelector,
      Func<T, Result<TValue>> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this ReadOnlySpan<T> span, Func<T, Result<TKey>> keySelector,
      Func<T, Result<TValue>> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         var _value = new LazyResult<TValue>(() => valueSelector(item));
         var _key = _value.Then(_ => keySelector(item));
         if (_key)
         {
            result[_key] = _value;
         }
         else
         {
            return _key.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in enumerable)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this Span<T> span, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this ReadOnlySpan<T> span, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> result = [];
      foreach (var item in span)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this IEnumerable<T> enumerable, Func<T, string> keySelector,
      Func<T, TValue> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in enumerable)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this Span<T> span, Func<T, string> keySelector,
      Func<T, TValue> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in span)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<StringHash<TValue>> TryToStringHash<TValue, T>(this ReadOnlySpan<T> span, Func<T, string> keySelector,
      Func<T, TValue> valueSelector) where TValue : notnull
   {
      StringHash<TValue> result = [];
      foreach (var item in span)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this IEnumerable<T> enumerable, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in enumerable)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this Span<T> span, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Hash<TKey, TValue>> TryToHash<TKey, TValue, T>(this ReadOnlySpan<T> span, Func<T, TKey> keySelector,
      Func<T, TValue> valueSelector, IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      var result = new Hash<TKey, TValue>(comparer);
      foreach (var item in span)
      {
         var _selector = tryTo(() => valueSelector(item));
         if (_selector is (true, var selector))
         {
            result[keySelector(item)] = selector;
         }
         else
         {
            return _selector.Exception;
         }
      }

      return result;
   }

   public static Result<Setting> ToSetting<TKey, TValue>(this IHash<TKey, TValue> hash, string name = Setting.ROOT_NAME) where TKey : notnull where TValue : notnull
   {
      try
      {
         var internalHash = hash.GetHash();
         var toSetting = new Setting(name);
         foreach (var (key, value) in internalHash)
         {
            var keyAsString = key.ToString()!;
            toSetting.SetItem(keyAsString, new Item(keyAsString, value.ToString() ?? ""));
         }

         return toSetting;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Configuration> ToConfiguration<TKey, TValue>(this IHash<TKey, TValue> hash, FileName file, string name = Setting.ROOT_NAME,
      bool save = false) where TKey : notnull where TValue : notnull
   {
      var _configuration = hash.ToSetting().Map(setting => new Configuration(file, setting.items, name));
      if (_configuration is (true, var configuration))
      {
         if (save)
         {
            return configuration.Save().Map(_ => configuration);
         }
         else
         {
            return configuration;
         }
      }
      else
      {
         return _configuration.Exception;
      }
   }

   public static Result<Hash<TKey, TValue[]>> GroupToHash<T, TKey, TValue>(this IEnumerable<T> enumerable, Func<T, TKey> keyFunc,
      Func<T, TValue> valueFunc) where TKey : notnull
   {
      try
      {
         var hash = new AutoHash<TKey, List<TValue>>(_ => [], true);
         foreach (var item in enumerable)
         {
            var key = keyFunc(item);
            var value = valueFunc(item);
            hash[key].Add(value);
         }

         Hash<TKey, TValue[]> result = [];
         foreach (var (key, value) in hash)
         {
            result[key] = [.. value];
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, TValue[]>> GroupToHash<T, TKey, TValue>(this Span<T> span, Func<T, TKey> keyFunc,
      Func<T, TValue> valueFunc) where TKey : notnull
   {
      try
      {
         var hash = new AutoHash<TKey, List<TValue>>(_ => [], true);
         foreach (var item in span)
         {
            var key = keyFunc(item);
            var value = valueFunc(item);
            hash[key].Add(value);
         }

         Hash<TKey, TValue[]> result = [];
         foreach (var (key, value) in hash)
         {
            result[key] = [.. value];
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, TValue[]>> GroupToHash<T, TKey, TValue>(this ReadOnlySpan<T> span, Func<T, TKey> keyFunc,
      Func<T, TValue> valueFunc) where TKey : notnull
   {
      try
      {
         var hash = new AutoHash<TKey, List<TValue>>(_ => [], true);
         foreach (var item in span)
         {
            var key = keyFunc(item);
            var value = valueFunc(item);
            hash[key].Add(value);
         }

         Hash<TKey, TValue[]> result = [];
         foreach (var (key, value) in hash)
         {
            result[key] = [.. value];
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, T[]>> GroupToHash<T, TKey>(this IEnumerable<T> enumerable, Func<T, TKey> keyFunc) where TKey : notnull
   {
      try
      {
         var hash = new AutoHash<TKey, List<T>>(_ => [], true);
         foreach (var item in enumerable)
         {
            var key = keyFunc(item);
            hash[key].Add(item);
         }

         Hash<TKey, T[]> result = [];
         foreach (var (key, value) in hash)
         {
            result[key] = [.. value];
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, T[]>> GroupToHash<T, TKey>(this Span<T> span, Func<T, TKey> keyFunc) where TKey : notnull
   {
      try
      {
         var hash = new AutoHash<TKey, List<T>>(_ => [], true);
         foreach (var item in span)
         {
            var key = keyFunc(item);
            hash[key].Add(item);
         }

         Hash<TKey, T[]> result = [];
         foreach (var (key, value) in hash)
         {
            result[key] = [.. value];
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, T[]>> GroupToHash<T, TKey>(this ReadOnlySpan<T> span, Func<T, TKey> keyFunc) where TKey : notnull
   {
      try
      {
         var hash = new AutoHash<TKey, List<T>>(_ => [], true);
         foreach (var item in span)
         {
            var key = keyFunc(item);
            hash[key].Add(item);
         }

         Hash<TKey, T[]> result = [];
         foreach (var (key, value) in hash)
         {
            result[key] = [.. value];
         }

         return result;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, TValue>> GroupToHash<T, TKey, TValue>(this IEnumerable<T> enumerable, Func<T, TKey> keyFunc,
      Func<Maybe<TValue>, T, TValue> valueFunc) where TKey : notnull where TValue : notnull
   {
      try
      {
         Hash<TKey, TValue> hash = [];
         foreach (var item in enumerable)
         {
            var key = keyFunc(item);
            var _value = hash.Maybe[key];
            var value = valueFunc(_value, item);
            hash[key] = value;
         }

         return hash;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, TValue>> GroupToHash<T, TKey, TValue>(this Span<T> span, Func<T, TKey> keyFunc,
      Func<Maybe<TValue>, T, TValue> valueFunc) where TKey : notnull where TValue : notnull
   {
      try
      {
         Hash<TKey, TValue> hash = [];
         foreach (var item in span)
         {
            var key = keyFunc(item);
            var _value = hash.Maybe[key];
            var value = valueFunc(_value, item);
            hash[key] = value;
         }

         return hash;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Hash<TKey, TValue>> GroupToHash<T, TKey, TValue>(this ReadOnlySpan<T> span, Func<T, TKey> keyFunc,
      Func<Maybe<TValue>, T, TValue> valueFunc) where TKey : notnull where TValue : notnull
   {
      try
      {
         Hash<TKey, TValue> hash = [];
         foreach (var item in span)
         {
            var key = keyFunc(item);
            var _value = hash.Maybe[key];
            var value = valueFunc(_value, item);
            hash[key] = value;
         }

         return hash;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Maybe<TaggedValue<MatchResult>> Matches(this Hash<string, Pattern> patterns, string input)
   {
      foreach (var (name, pattern) in patterns)
      {
         var _result = input.Matches(pattern);
         if (_result is (true, var result))
         {
            return new TaggedValue<MatchResult>(name, result);
         }
      }

      return nil;
   }

   public static IHashMaybe<TKey, TValue> Maybe<TKey, TValue>(this IHash<TKey, TValue> hash) where TKey : notnull where TValue : notnull => new(hash);

   [Obsolete("Use kv function")]
   public static KeyValuePair<TKey, TValue> at<TKey, TValue>(this TKey key, TValue value) => new(key, value);
}