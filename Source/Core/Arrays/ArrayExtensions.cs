using System;
using System.Collections.Generic;
using System.Linq;
using Core.Assertions;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using static System.Math;
using static Core.Monads.MonadFunctions;
using static Core.Objects.ObjectFunctions;

namespace Core.Arrays;

public static class ArrayExtensions
{
   private enum BalanceType
   {
      AwaitingStart,
      AwaitingStop
   }

   public static T[] Augment<T>(this T[] source, params T[] items)
   {
      var targetLength = source.Length + items.Length;
      var target = new T[targetLength];
      source.CopyTo(target, 0);
      items.CopyTo(target, source.Length);

      return target;
   }

   public static T[] Pad<T>(this T[] array, int newLength, T fillValue)
   {
      var length = array.Length;
      if (length == newLength)
      {
         return array;
      }
      else
      {
         newLength.Must().BeGreaterThan(length).OrThrow("New length must be greater than the source array's length");

         var newArray = new T[newLength];
         Array.Copy(array, newArray, length);
         for (var i = length; i < newLength; i++)
         {
            newArray[i] = fillValue;
         }

         return newArray;
      }
   }

   public static T[] LimitTo<T>(this T[] array, int limitingSize, T fillValue)
   {
      var length = array.Length;
      if (length == limitingSize)
      {
         return array;
      }
      else if (limitingSize == 0)
      {
         return [];
      }
      else if (limitingSize > length)
      {
         return array.Pad(limitingSize, fillValue);
      }
      else
      {
         var newArray = new T[limitingSize];
         Array.Copy(array, newArray, limitingSize);

         return newArray;
      }
   }

   public static T[] Shuffle<T>(this T[] array) => shuffle(array, new Random());

   public static T[] Shuffle<T>(this T[] array, int seed) => shuffle(array, new Random(seed));

   private static T[] shuffle<T>(T[] array, Random random)
   {
      var list = array.Select(value => new { Index = random.Next(), Value = value });
      var sortedList = list.OrderBy(i => i.Index).Select(i => i.Value);

      return [..sortedList];
   }

   public static bool IsEmpty<T>(this T[] array) => array.Length == 0;

   public static bool IsNotEmpty<T>(this T[] array) => array is { Length: > 0 };

   public static T[] RangeOf<T>(this T[] array, int start, int stop)
   {
      start.Must().BeLessThanOrEqual(stop).OrThrow();
      start.Must().BeBetween(0).Until(array.Length).OrThrow();
      stop.Must().BeBetween(0).Until(array.Length).OrThrow();

      var result = new T[stop - start + 1];
      var index = 0;
      for (var i = start; i <= stop; i++)
      {
         result[index++] = array[i];
      }

      return result;
   }

   public static Slice<T> From<T>(this T[] array, int startIndex) where T : notnull => new(array, startIndex);

   public static T[] Pick<T>(this T[] array, string columnIndexes)
   {
      return [.. columnIndexes.PickIndexes().Where(i => i.Between(0).Until(array.Length)).Select(index => array[index])];
   }

   public static T[] Pick<T>(this T[] array, params int[] columnIndexes)
   {
      return [.. columnIndexes.Where(i => i.Between(0).Until(array.Length)).Select(index => array[index])];
   }

   public static (T value, int index)[] PickIndexes<T>(this T[] array, string columnIndexes)
   {
      return [.. columnIndexes.PickIndexes().Where(i => i.Between(0).Until(array.Length)).Select(index => (array[index], index))];
   }

   public static int[] PickIndexes(this string columnIndexes)
   {
      List<int> indexes = [];

      foreach (var group in columnIndexes.RemoveWhitespace().Unjoin("','; f"))
      {
         var _result = group.Matches("/(/d+) '-' /(/d+); f");
         if (_result is (true, var (startIndex, stopIndex)))
         {
            var intStart = startIndex.Value().Int32();
            var intStop = stopIndex.Value().Int32();
            if (intStart > intStop)
            {
               swap(ref intStart, ref intStop);
            }

            for (var i = intStart; i <= intStop; i++)
            {
               indexes.Add(i);
            }
         }
         else
         {
            indexes.Add(group.Value().Int32());
         }
      }

      indexes.Sort();
      return [.. indexes];
   }

   public static bool ContainsValue<T>(this T[] array, T value) => Array.IndexOf(array, value) > -1;

   public static T Maybe<T>(this T[] array, int index, T defaultValue)
   {
      return index.Between(0).Until(array.Length) ? array[index] : defaultValue;
   }

   public static Maybe<T> Maybe<T>(this T[] array, int index) where T : notnull
   {
      return maybe<T>() & index.Between(0).Until(array.Length) & (() => array[index]);
   }

   public static T First<T>(this T[] array, T defaultValue) => array.IsEmpty() ? defaultValue : array[0];

   public static Maybe<T> First<T>(this T[] array) where T : notnull => maybe<T>() & array.IsNotEmpty() & (() => array[0]);

   public static T Last<T>(this T[] array, T defaultValue) => array.IsEmpty() ? defaultValue : array[^1];

   public static Maybe<T> Last<T>(this T[] array) where T : notnull => maybe<T>() & array.IsNotEmpty() & (() => array[^1]);

   public static T[] Tail<T>(this T[] array) => array.IsEmpty() ? [] : [.. array.Skip(1)];

   public static T[] AllButLast<T>(this T[] array) => array.IsEmpty() ? [] : [.. array.Take(array.Length - 1)];

   public static Maybe<Slice<T>> Balanced<T>(this T[] array, Predicate<T> startCondition, Predicate<T> stopCondition, int startIndex = 0) where T : notnull
   {
      if (array.IsEmpty() || startIndex >= array.Length)
      {
         return nil;
      }
      else
      {
         var count = 0;
         var type = BalanceType.AwaitingStart;
         var index = -1;

         for (var i = startIndex; i < array.Length; i++)
         {
            var element = array[i];
            switch (type)
            {
               case BalanceType.AwaitingStart:
                  if (startCondition(element))
                  {
                     count++;
                     type = BalanceType.AwaitingStop;
                     index = i;
                  }

                  break;
               case BalanceType.AwaitingStop:
                  if (startCondition(element))
                  {
                     count++;
                  }
                  else if (stopCondition(element))
                  {
                     if (--count == 0)
                     {
                        return array.From(index).To(i);
                     }
                  }

                  break;
            }
         }

         return nil;
      }
   }

   public static Maybe<(T[] array, T element)> Pop<T>(this T[] array)
   {
      if (!array.IsEmpty())
      {
         var length = array.Length - 1;
         var bottom = array[length];
         var newArray = new T[length];

         Array.Copy(array, newArray, length);

         return (newArray, bottom);
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<T[]> Push<T>(this T[] array, T element)
   {
      if (!array.IsEmpty())
      {
         var length = array.Length + 1;
         var newArray = new T[length];

         Array.Copy(array, newArray, length - 1);
         newArray[length - 1] = element;

         return newArray;
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<(T[] array, T element)> Shift<T>(this T[] array)
   {
      if (!array.IsEmpty())
      {
         var length = array.Length - 1;
         var top = array[0];
         var newArray = new T[length];

         Array.Copy(array, 1, newArray, 0, length);

         return (newArray, top);
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<T[]> Unshift<T>(this T[] array, T element)
   {
      if (!array.IsEmpty())
      {
         var length = array.Length + 1;
         var newArray = new T[length];

         Array.Copy(array, 0, newArray, 1, length - 1);
         newArray[0] = element;

         return newArray;
      }
      else
      {
         return nil;
      }
   }

   public static (T1, T2)[] Zip<T1, T2>(this T1[] leftArray, T2[] rightArray)
   {
      List<(T1, T2)> list = [];
      var length = Min(leftArray.Length, rightArray.Length);
      for (var i = 0; i < length; i++)
      {
         list.Add((leftArray[i], rightArray[i]));
      }

      return [.. list];
   }

   public static (T1, T2)[] Zip<T1, T2>(this T1[] leftArray, Func<T1, int, T2> generatorFunc)
   {
      List<(T1, T2)> list = [];
      var length = leftArray.Length;
      for (var i = 0; i < length; i++)
      {
         var left = leftArray[i];
         list.Add((left, generatorFunc(left, i)));
      }

      return [.. list];
   }

   public static T3[] Zip<T1, T2, T3>(this T1[] leftArray, T2[] rightArray, Func<T1, T2, T3> mappingFunc)
   {
      List<T3> list = [];
      var length = Min(leftArray.Length, rightArray.Length);
      for (var i = 0; i < length; i++)
      {
         list.Add(mappingFunc(leftArray[i], rightArray[i]));
      }

      return [.. list];
   }

   public static T3[] Zip<T1, T2, T3>(this T1[] leftArray, Func<T1, int, T2> generatorFunc,
      Func<T1, T2, T3> mappingFunc)
   {
      List<T3> list = [];
      var length = leftArray.Length;
      for (var i = 0; i < length; i++)
      {
         var left = leftArray[i];
         var right = generatorFunc(left, i);
         var item = mappingFunc(left, right);
         list.Add(item);
      }

      return [.. list];
   }

   public static Maybe<(T1, T2)>[] ZipUnevenly<T1, T2>(this T1[] leftArray, T2[] rightArray)
   {
      List<Maybe<(T1, T2)>> list = [];

      var leftLength = leftArray.Length;
      var rightLength = rightArray.Length;
      var length = Max(leftLength, rightLength);

      for (var i = 0; i < length; i++)
      {
         if (i.Between(0).Until(leftLength) && i.Between(0).Until(rightLength))
         {
            list.Add((leftArray[i], rightArray[i]).Some());
         }
      }

      return [.. list];
   }

   public static T[] Repeat<T>(this int count, T value) => [.. Enumerable.Repeat(value, count)];

   public static T[] Repeat<T>(this int count, Func<T> func) => [.. 0.Until(count).Select(_ => func())];

   public static T[] Repeat<T>(this T[] array, int count)
   {
      List<T> result = [];
      for (var i = 0; i < count; i++)
      {
         result.AddRange(array);
      }

      return [.. result];
   }

   public static Result<int> Assign<T>(this T[] array, out T var0, out T var1)
   {
      var0 = var1 = default!;
      if (array.Length >= 2)
      {
         var0 = array[0];
         var1 = array[1];

         return 2;
      }
      else
      {
         return fail("Array too small");
      }
   }

   public static Result<int> Assign<T>(this T[] array, out T var0, out T var1, out T var2)
   {
      var2 = default!;
      var _index = Assign(array, out var0, out var1);
      if (_index is (true, var index) && array.Length >= index + 1)
      {
         var2 = array[2];
         return 3;
      }
      else
      {
         return _index;
      }
   }

   public static Result<int> Assign<T>(this T[] array, out T var0, out T var1, out T var2, out T var3)
   {
      var3 = default!;
      var _index = Assign(array, out var0, out var1, out var2);
      if (_index is (true, var index) && array.Length >= index + 1)
      {
         var3 = array[3];
         return 4;
      }
      else
      {
         return _index;
      }
   }

   public static Result<int> Assign<T>(this T[] array, out T var0, out T var1, out T var2, out T var3,
      out T var4)
   {
      var4 = default!;
      var _index = Assign(array, out var0, out var1, out var2, out var3);
      if (_index is (true, var index) && array.Length >= index + 1)
      {
         var4 = array[4];
         return 5;
      }
      else
      {
         return _index;
      }
   }

   public static Result<int> Assign<T>(this T[] array, out T var0, out T var1, out T var2, out T var3,
      out T var4, out T var5)
   {
      var5 = default!;
      var _index = Assign(array, out var0, out var1, out var2, out var3, out var4);
      if (_index is (true, var index) && array.Length >= index + 1)
      {
         var5 = array[5];
         return 6;
      }
      else
      {
         return _index;
      }
   }

   public static Result<int> Assign<T>(this T[] array, out T var0, out T var1, out T var2, out T var3,
      out T var4, out T var5, out T var6)
   {
      var6 = default!;
      var _index = Assign(array, out var0, out var1, out var2, out var3, out var4, out var5);
      if (_index is (true, var index) && array.Length >= index + 1)
      {
         var6 = array[6];
         return 7;
      }
      else
      {
         return _index;
      }
   }

   /*public static Maybe<int> Index<T>(this T[] array, T item, int startIndex = 0)
   {
      var index = Array.IndexOf(array, item, startIndex);
      return maybe<int>() & index > -1 & (() => index);
   }*/

   /*public static Maybe<int[]> Indexes<T>(this T[] array, T item, int startIndex = 0)
   {
      List<int> list = [];
      var index = Array.IndexOf(array, item, startIndex);
      if (index != -1)
      {
         while (index > -1)
         {
            list.Add(index);
            index = Array.IndexOf(array, item, index + 1);
         }

         return (int[]) [.. list];
      }
      else
      {
         return nil;
      }
   }*/

   [Obsolete("Use native index")]
   public static IEnumerable<(int index, T value)> WithIndexes<T>(this T[] array)
   {
      for (var i = 0; i < array.Length; i++)
      {
         yield return (i, array[i]);
      }
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this (TKey, TValue)[] array) where TKey : notnull where TValue : notnull
   {
      Hash<TKey, TValue> hash = [];
      foreach (var (key, value) in array)
      {
         hash[key] = value;
      }

      return hash;
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2)
   {
      e1 = array[0];
      e2 = array[1];
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2, out T e3)
   {
      e1 = array[0];
      e2 = array[1];
      e3 = array[2];
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2, out T e3, out T e4)
   {
      e1 = array[0];
      e2 = array[1];
      e3 = array[2];
      e4 = array[3];
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2, out T e3, out T e4, out T e5)
   {
      e1 = array[0];
      e2 = array[1];
      e3 = array[2];
      e4 = array[3];
      e5 = array[4];
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2, out T e3, out T e4, out T e5, out T e6)
   {
      e1 = array[0];
      e2 = array[1];
      e3 = array[2];
      e4 = array[3];
      e5 = array[4];
      e6 = array[5];
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2, out T e3, out T e4, out T e5, out T e6, out T e7)
   {
      e1 = array[0];
      e2 = array[1];
      e3 = array[2];
      e4 = array[3];
      e5 = array[4];
      e6 = array[5];
      e7 = array[6];
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2, out T e3, out T e4, out T e5, out T e6, out T e7, out T e8)
   {
      e1 = array[0];
      e2 = array[1];
      e3 = array[2];
      e4 = array[3];
      e5 = array[4];
      e6 = array[5];
      e7 = array[6];
      e8 = array[7];
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2, out T e3, out T e4, out T e5, out T e6, out T e7, out T e8,
      out T e9)
   {
      e1 = array[0];
      e2 = array[1];
      e3 = array[2];
      e4 = array[3];
      e5 = array[4];
      e6 = array[5];
      e7 = array[6];
      e8 = array[7];
      e9 = array[8];
   }

   public static void Deconstruct<T>(this T[] array, out T e1, out T e2, out T e3, out T e4, out T e5, out T e6, out T e7, out T e8,
      out T e9, out T e10)
   {
      e1 = array[0];
      e2 = array[1];
      e3 = array[2];
      e4 = array[3];
      e5 = array[4];
      e6 = array[5];
      e7 = array[6];
      e8 = array[7];
      e9 = array[8];
      e10 = array[9];
   }

   public static bool AllEqualTo<T>(this T[] left, T[] right) where T : IEquatable<T>
   {
      if (left.Length != right.Length)
      {
         return false;
      }
      else
      {
         for (var i = 0; i < left.Length; i++)
         {
            if (!left[i].Equals(right[i]))
            {
               return false;
            }
         }

         return true;
      }
   }

   public static T[] RemoveAt<T>(this T[] array, int index) => [.. array.Indexed().Where(t => t.index != index).Select(t => t.item)];
}