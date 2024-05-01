﻿using System.Collections.Generic;
using System.Linq;
using Core.Assertions;

namespace Core.Enumerables;

public static class TupleExtensions
{
   public static (T, T) ToTuple2<T>(this IEnumerable<T> enumerable)
   {
      List<T> list = [.. enumerable];
      list.Must().HaveCountOf(2).OrThrow();

      return (list[0], list[1]);
   }

   public static (T, T, T) ToTuple3<T>(this IEnumerable<T> enumerable)
   {
      List<T> list = [.. enumerable];
      list.Must().HaveCountOf(3).OrThrow();

      return (list[0], list[1], list[2]);
   }

   public static (T, T, T, T) ToTuple4<T>(this IEnumerable<T> enumerable)
   {
      List<T> list = [.. enumerable];
      list.Must().HaveCountOf(4).OrThrow();

      return (list[0], list[1], list[2], list[3]);
   }

   public static (IEnumerable<T1>, IEnumerable<T2>) Split<T1, T2>(this IEnumerable<(T1, T2)> enumerable)
   {
      (T1, T2)[] array = [.. enumerable];
      var enumerable1 = array.Select(t => t.Item1);
      var enumerable2 = array.Select(t => t.Item2);

      return (enumerable1, enumerable2);
   }

   public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>) Split<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable)
   {
      (T1, T2, T3)[] array = [.. enumerable];
      var enumerable1 = array.Select(t => t.Item1);
      var enumerable2 = array.Select(t => t.Item2);
      var enumerable3 = array.Select(t => t.Item3);

      return (enumerable1, enumerable2, enumerable3);
   }

   public static (IEnumerable<T1>, IEnumerable<T2>, IEnumerable<T3>, IEnumerable<T4>) Split<T1, T2, T3, T4>(
      this IEnumerable<(T1, T2, T3, T4)> enumerable)
   {
      (T1, T2, T3, T4)[] array = [.. enumerable];
      var enumerable1 = array.Select(t => t.Item1);
      var enumerable2 = array.Select(t => t.Item2);
      var enumerable3 = array.Select(t => t.Item3);
      var enumerable4 = array.Select(t => t.Item4);

      return (enumerable1, enumerable2, enumerable3, enumerable4);
   }
}