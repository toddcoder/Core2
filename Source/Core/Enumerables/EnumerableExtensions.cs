﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Assertions;
using Core.Collections;
using Core.Configurations;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables;

public static class EnumerableExtensions
{
   public static string ToString<T>(this IEnumerable<T> enumerable, string connector)
   {
      enumerable.Must().Not.BeNull().OrThrow();
      connector.Must().Not.BeNull().OrThrow();

      return string.Join(connector, enumerable.Select(i => i!.ToNonNullString()));
   }

   public static IEnumerable<IEnumerable<T>> Pivot<T>(this IEnumerable<IEnumerable<T>> source, Func<T> defaultValue)
   {
      T[][] array = [.. source.Select(row => (T[]) [.. row])];
      if (array.Length != 0)
      {
         var maxRowLen = array.Select(a => a.Length).Max();
         var minRowLen = array.Select(a => a.Length).Min();
         var squared = array;
         if (minRowLen != maxRowLen)
         {
            squared = [.. array.Select(row => row.Pad(maxRowLen, defaultValue()))];
         }

         return [.. 0.Until(maxRowLen).Select(i => squared.Select(row => row[i]))];
      }
      else
      {
         return array;
      }
   }

   public static IEnumerable<IEnumerable<T>> Pivot<T>(this IEnumerable<IEnumerable<T>> source, T defaultValue) => source.Pivot(() => defaultValue);

   public static Result<T[]> ToResultOfArray<T>(this IEnumerable<T> enumerable)
   {
      try
      {
         return (T[]) [.. enumerable];
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<List<T>> ToResultOfList<T>(this IEnumerable<T> enumerable)
   {
      try
      {
         return (List<T>) [.. enumerable];
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static IEnumerable<int> UpTo(this int from, int to, int by = 1)
   {
      for (var i = from; i <= to; i += by)
      {
         yield return i;
      }
   }

   public static IEnumerable<char> UpTo(this char from, char to, int by = 1)
   {
      for (var i = from; i <= to; i = (char)(i + by))
      {
         yield return i;
      }
   }

   public static IEnumerable<int> UpUntil(this int from, int to, int by = 1)
   {
      for (var i = from; i < to; i += by)
      {
         yield return i;
      }
   }

   public static IEnumerable<char> UpUntil(this char from, char to, int by = 1)
   {
      for (var i = from; i < to; i = (char)(i + by))
      {
         yield return i;
      }
   }

   public static IEnumerable<int> DownTo(this int from, int to, int by = -1)
   {
      for (var i = from; i >= to; i += by)
      {
         yield return i;
      }
   }

   public static IEnumerable<char> DownTo(this char from, char to, int by = -1)
   {
      for (var i = from; i >= to; i = (char)(i + by))
      {
         yield return i;
      }
   }

   public static IEnumerable<int> DownUntil(this int from, int to, int by = -1)
   {
      for (var i = from; i > to; i += by)
      {
         yield return i;
      }
   }

   public static IEnumerable<char> DownUntil(this char from, char to, int by = -1)
   {
      for (var i = from; i > to; i = (char)(i + by))
      {
         yield return i;
      }
   }

   public static IEnumerable<T> Then<T>(this T seed, Func<T, T> next, Predicate<T> stop)
   {
      var current = seed;

      yield return current;

      while (!stop(current))
      {
         current = next(current);
         yield return current;
      }
   }

   public static IEnumerable<T> Then<T>(this T seed, Func<T, T> next) => seed.Then(next, _ => false);

   public static IEnumerable<(int index, T item)> Indexed<T>(this IEnumerable<T> enumerable)
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         yield return (index++, item);
      }
   }

   public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> enumerable) where T : notnull
   {
      var first = enumerable.FirstOrDefault();

      if (first is null)
      {
         return nil;
      }
      else if (first.Equals(default))
      {
         return nil;
      }
      else
      {
         return first;
      }
   }

   public static Maybe<int> FirstIndexOrNone<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         if (predicate(item))
         {
            return index;
         }

         index++;
      }

      return nil;
   }

   public static Maybe<T> LastOrNone<T>(this IEnumerable<T> enumerable) where T : notnull
   {
      var last = enumerable.LastOrDefault();

      if (last is null)
      {
         return nil;
      }
      else if (last.Equals(default))
      {
         return nil;
      }
      else
      {
         return last;
      }
   }

   public static Result<T> FirstOrFailure<T>(this IEnumerable<T> enumerable, string failureMessage = "Default value") where T : notnull
   {
      try
      {
         return enumerable.FirstOrNone().Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> FirstOrFailure<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, string failureMessage = "Default value")
      where T : notnull
   {
      try
      {
         return enumerable.FirstOrNone(i => predicate(i)).Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<int> FirstIndexOrFailure<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, string failureMessage = "Default value")
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         if (predicate(item))
         {
            return index;
         }

         index++;
      }

      return fail(failureMessage);
   }

   public static Result<(T1, T2)> FirstOrFailure<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
      string failureMessage = "Default value")
   {
      try
      {
         return enumerable.FirstOrNone(i => predicate(i.Item1, i.Item2)).Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2, T3)> FirstOrFailure<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
      string failureMessage = "Default value")
   {
      try
      {
         return enumerable.FirstOrNone(i => predicate(i.Item1, i.Item2, i.Item3)).Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2, T3, T4)> FirstOrFailure<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate, string failureMessage = "Default value")
   {
      try
      {
         return enumerable.FirstOrNone(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4)).Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> FirstOrFailure<T>(this IEnumerable<T> enumerable, Func<string> failureMessage) where T : notnull
   {
      try
      {
         return enumerable.FirstOrNone().Result(failureMessage());
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> FirstOrFailure<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Func<string> failureMessage) where T : notnull
   {
      try
      {
         return enumerable.FirstOrNone(i => predicate(i)).Result(failureMessage());
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<int> FirstIndexOrFailure<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, Func<string> failureMessage)
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         if (predicate(item))
         {
            return index;
         }

         index++;
      }

      return fail(failureMessage());
   }

   public static Result<(T1, T2)> FirstOrFailure<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
      Func<string> failureMessage)
   {
      try
      {
         return enumerable.FirstOrNone(i => predicate(i.Item1, i.Item2)).Result(failureMessage());
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2, T3)> FirstOrFailure<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
      Func<string> failureMessage)
   {
      try
      {
         return enumerable.FirstOrNone(i => predicate(i.Item1, i.Item2, i.Item3)).Result(failureMessage());
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2, T3, T4)> FirstOrFailure<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate, Func<string> failureMessage)
   {
      try
      {
         return enumerable.FirstOrNone(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4)).Result(failureMessage());
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> LastOrFailure<T>(this IEnumerable<T> enumerable, string failureMessage = "Default value") where T : notnull
   {
      try
      {
         return enumerable.LastOrNone().Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> LastOrFailure<T>(this IEnumerable<T> enumerable, Predicate<T> predicate,
      string failureMessage = "Default value") where T : notnull
   {
      try
      {
         return enumerable.LastOrNone(i => predicate(i)).Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2)> LastOrFailure<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
      string failureMessage = "Default value")
   {
      try
      {
         return enumerable.LastOrNone(i => predicate(i.Item1, i.Item2)).Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2, T3)> LastOrFailure<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable,
      Func<T1, T2, T3, bool> predicate, string failureMessage = "Default value")
   {
      try
      {
         return enumerable.LastOrNone(i => predicate(i.Item1, i.Item2, i.Item3)).Result(failureMessage);
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage);
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> LastOrFailure<T>(this IEnumerable<T> enumerable, Func<string> failureMessage) where T : notnull
   {
      try
      {
         return enumerable.LastOrNone().Result(failureMessage());
      }
      catch (InvalidOperationException)
      {
         return fail(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> LastOrFailure<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Func<string> failureMessage) where T : notnull
   {
      try
      {
         return enumerable.LastOrNone(i => predicate(i)).Result(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2)> LastOrFailure<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
      Func<string> failureMessage)
   {
      try
      {
         return enumerable.LastOrNone(i => predicate(i.Item1, i.Item2)).Result(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2, T3)> LastOrFailure<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable,
      Func<T1, T2, T3, bool> predicate, Func<string> failureMessage)
   {
      try
      {
         return enumerable.LastOrNone(i => predicate(i.Item1, i.Item2, i.Item3)).Result(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<(T1, T2, T3, T4)> LastOrFailure<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate, Func<string> failureMessage)
   {
      try
      {
         return enumerable.LastOrNone(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4)).Result(failureMessage());
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Maybe<T> FirstOrNone<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : notnull
   {
      var first = enumerable.FirstOrDefault(predicate);

      if (first is null)
      {
         return nil;
      }
      else if (first.Equals(default))
      {
         return nil;
      }
      else
      {
         return first;
      }
   }

   public static Maybe<(T1, T2)> FirstOrNone<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
   {
      var first = enumerable.FirstOrDefault(i => predicate(i.Item1, i.Item2));

      if (first.AnyNull())
      {
         return nil;
      }
      else if (first.Equals(default))
      {
         return nil;
      }
      else
      {
         return first;
      }
   }

   public static Maybe<(T1, T2, T3)> FirstOrNone<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable,
      Func<T1, T2, T3, bool> predicate)
   {
      var first = enumerable.FirstOrDefault(i => predicate(i.Item1, i.Item2, i.Item3));

      if (first.AnyNull())
      {
         return nil;
      }
      else if (first.Equals(default))
      {
         return nil;
      }
      else
      {
         return first;
      }
   }

   public static Maybe<(T1, T2, T3, T4)> FirstOrNone<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate)
   {
      var first = enumerable.FirstOrDefault(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4));

      if (first.AnyNull())
      {
         return nil;
      }
      else if (first.Equals(default))
      {
         return nil;
      }
      else
      {
         return first;
      }
   }

   public static Maybe<T> LastOrNone<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : notnull
   {
      var last = enumerable.LastOrDefault(predicate);

      if (last is null)
      {
         return nil;
      }
      else if (last.Equals(default))
      {
         return nil;
      }
      else
      {
         return last;
      }
   }

   public static Maybe<(T1, T2)> LastOrNone<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
   {
      var last = enumerable.LastOrDefault(i => predicate(i.Item1, i.Item2));

      if (last.AnyNull())
      {
         return nil;
      }
      else if (last.Equals(default))
      {
         return nil;
      }
      else
      {
         return last;
      }
   }

   public static Maybe<(T1, T2, T3)> LastOrNone<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable,
      Func<T1, T2, T3, bool> predicate)
   {
      var last = enumerable.LastOrDefault(i => predicate(i.Item1, i.Item2, i.Item3));

      if (last.AnyNull())
      {
         return nil;
      }
      else if (last.Equals(default))
      {
         return nil;
      }
      else
      {
         return last;
      }
   }

   public static Maybe<(T1, T2, T3, T4)> LastOrNone<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate)
   {
      var last = enumerable.LastOrDefault(i => predicate(i.Item1, i.Item2, i.Item3, i.Item4));

      if (last.AnyNull())
      {
         return nil;
      }
      else if (last.Equals(default))
      {
         return nil;
      }
      else
      {
         return last;
      }
   }

   public static Optional<T> FirstOrEmpty<T>(this IEnumerable<T> enumerable) where T : notnull => enumerable.FirstOrNone().Optional();

   public static Optional<T> FirstOrEmpty<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : notnull
   {
      return enumerable.FirstOrNone(predicate).Optional();
   }

   public static Optional<int> FirstIndexOrEmpty<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         if (predicate(item))
         {
            return index;
         }

         index++;
      }

      return nil;
   }

   public static Optional<(T1, T2)> FirstOrEmpty<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
   {
      return enumerable.FirstOrNone(predicate).Optional();
   }

   public static Optional<(T1, T2, T3)> FirstOrEmpty<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate)
   {
      return enumerable.FirstOrNone(predicate).Optional();
   }

   public static Optional<(T1, T2, T3, T4)> FirstOrEmpty<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate)
   {
      return enumerable.FirstOrNone(predicate).Optional();
   }

   public static Optional<T> LastOrEmpty<T>(this IEnumerable<T> enumerable) where T : notnull => enumerable.LastOrNone().Optional();

   public static Optional<T> LastOrEmpty<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : notnull
   {
      return enumerable.LastOrNone(predicate).Optional();
   }

   public static Optional<(T1, T2)> LastOrEmpty<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
   {
      return enumerable.LastOrNone(predicate).Optional();
   }

   public static Optional<(T1, T2, T3)> LastOrEmpty<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate)
   {
      return enumerable.LastOrNone(predicate).Optional();
   }

   public static Optional<(T1, T2, T3, T4)> LastOrEmpty<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate)
   {
      return enumerable.LastOrNone(predicate).Optional();
   }

   public static Optional<T> FirstOrFail<T>(this IEnumerable<T> enumerable, string failMessage = "Default value") where T : notnull
   {
      return enumerable.FirstOrFailure(failMessage).Optional();
   }

   public static Optional<T> FirstOrFail<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, string failMessage = "Default value")
      where T : notnull
   {
      return enumerable.FirstOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<int> FirstIndexOrFail<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate, string failMessage = "Default value")
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         if (predicate(item))
         {
            return index;
         }

         index++;
      }

      return fail(failMessage);
   }

   public static Optional<(T1, T2)> FirstOrFail<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
      string failMessage = "Default value")
   {
      return enumerable.FirstOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2, T3)> FirstOrFail<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
      string failMessage = "Default value")
   {
      return enumerable.FirstOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<T> FirstOrFail<T>(this IEnumerable<T> enumerable, Func<string> failMessage) where T : notnull
   {
      return enumerable.FirstOrFailure(failMessage).Optional();
   }

   public static Optional<T> FirstOrFail<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Func<string> failMessage) where T : notnull
   {
      return enumerable.FirstOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2)> FirstOrFail<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate, Func<string> failMessage)
   {
      return enumerable.FirstOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2, T3)> FirstOrFail<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
      Func<string> failMessage)
   {
      return enumerable.FirstOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2, T3, T4)> FirstOrFail<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate, Func<string> failMessage)
   {
      return enumerable.FirstOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<T> LastOrFail<T>(this IEnumerable<T> enumerable, string failMessage = "Default value") where T : notnull
   {
      return enumerable.LastOrFailure(failMessage).Optional();
   }

   public static Optional<T> LastOrFail<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, string failMessage = "Default value")
      where T : notnull
   {
      return enumerable.LastOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2)> LastOrFail<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate,
      string failMessage = "Default value")
   {
      return enumerable.LastOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2, T3)> LastOrFail<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
      string failMessage = "Default value")
   {
      return enumerable.LastOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<T> LastOrFail<T>(this IEnumerable<T> enumerable, Func<string> failMessage) where T : notnull
   {
      return enumerable.LastOrFailure(failMessage).Optional();
   }

   public static Optional<T> LastOrFail<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Func<string> failMessage) where T : notnull
   {
      return enumerable.LastOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2)> LastOrFail<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate, Func<string> failMessage)
   {
      return enumerable.LastOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2, T3)> LastOrFail<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Func<T1, T2, T3, bool> predicate,
      Func<string> failMessage)
   {
      return enumerable.LastOrFailure(predicate, failMessage).Optional();
   }

   public static Optional<(T1, T2, T3, T4)> LastOrFail<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable,
      Func<T1, T2, T3, T4, bool> predicate, Func<string> failMessage)
   {
      return enumerable.LastOrFailure(predicate, failMessage).Optional();
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable) where TKey : notnull
      where TValue : notnull
   {
      return enumerable.ToHash(kv => kv.Key, kv => kv.Value);
   }

   public static Hash<TKey, TValue> ToHash<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> enumerable,
      IEqualityComparer<TKey> comparer) where TKey : notnull where TValue : notnull
   {
      return enumerable.ToHash(kv => kv.Key, kv => kv.Value, comparer);
   }

   public static IEnumerable<TResult> FlatMap<TSource, TResult>(this IEnumerable<IEnumerable<TSource>> enumerable,
      Func<TSource, TResult> mapFunc)
   {
      foreach (var outer in enumerable)
      {
         foreach (var inner in outer)
         {
            yield return mapFunc(inner);
         }
      }
   }

   public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable)
   {
      foreach (var outer in enumerable)
      {
         foreach (var inner in outer)
         {
            yield return inner;
         }
      }
   }

   public static TResult FoldLeft<TSource, TResult>(this IEnumerable<TSource> enumerable, TResult init,
      Func<TResult, TSource, TResult> foldFunc)
   {
      return enumerable.Aggregate(init, foldFunc);
   }

   public static TResult FoldRight<TSource, TResult>(this IEnumerable<TSource> enumerable, TResult init,
      Func<TSource, TResult, TResult> foldFunc)
   {
      List<TSource> list = [.. enumerable];
      var accumulator = init;
      for (var i = list.Count - 1; i >= 0; i--)
      {
         accumulator = foldFunc(list[i], accumulator);
      }

      return accumulator;
   }

   public static T FoldLeft<T>(this IEnumerable<T> enumerable, Func<T, T, T> foldFunc) => enumerable.Aggregate(foldFunc);

   public static T FoldRight<T>(this IEnumerable<T> enumerable, Func<T, T, T> foldFunc)
   {
      List<T> list = [.. enumerable];
      if (list.Count == 0)
      {
         throw fail("Enumerable can't be empty");
      }

      var accumulator = list[^1];
      for (var i = list.Count - 2; i >= 0; i--)
      {
         accumulator = foldFunc(list[i], accumulator);
      }

      return accumulator;
   }

   public static TResult Fold<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TResult, TSource, TResult> foldFunc, TResult defaultValue)
      where TResult : notnull
   {
      Maybe<TResult> _result = nil;
      foreach (var source in enumerable)
      {
         if (_result is (true, var result))
         {
            _result = foldFunc(result, source);
         }
      }

      return _result | defaultValue;
   }

   public static Hash<TKey, TValue[]> Group<TKey, TValue>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> groupingFunc) where TKey : notnull
   {
      Hash<TKey, List<TValue>> hash = [];
      foreach (var value in enumerable)
      {
         var key = groupingFunc(value);
         if (hash.TryGetValue(key, out var list))
         {
            list.Add(value);
         }
         else
         {
            hash[key] = [value];
         }
      }

      Hash<TKey, TValue[]> result = [];
      foreach (var (key, value) in hash)
      {
         result[key] = [.. value];
      }

      return result;
   }

   public static Hash<TKey, Set<TValue>> GroupToSet<TKey, TValue>(this IEnumerable<TValue> enumerable,
      Func<TValue, TKey> groupingFunc) where TKey : notnull
   {
      Hash<TKey, Set<TValue>> hash = [];
      foreach (var value in enumerable)
      {
         var key = groupingFunc(value);
         if (hash.TryGetValue(key, out var set))
         {
            set.Add(value);
         }
         else
         {
            hash[key] = [value];
         }
      }

      return hash;
   }

   public static Hash<TKey, StringSet> GroupToStringSet<TKey>(this IEnumerable<string> enumerable, Func<string, TKey> groupingFunc)
      where TKey : notnull
   {
      Hash<TKey, StringSet> hash = [];
      foreach (var value in enumerable)
      {
         var key = groupingFunc(value);
         if (hash.TryGetValue(key, out var stringSet))
         {
            stringSet.Add(value);
         }
         else
         {
            hash[key] = [value];
         }
      }

      return hash;
   }

   public static (IEnumerable<T> isTrue, IEnumerable<T> isFalse) Partition<T>(this IEnumerable<T> enumerable,
      Predicate<T> predicate)
   {
      List<T> isTrue = [];
      List<T> isFalse = [];
      foreach (var item in enumerable)
      {
         if (predicate(item))
         {
            isTrue.Add(item);
         }
         else
         {
            isFalse.Add(item);
         }
      }

      return (isTrue, isFalse);
   }

   public static Maybe<int> IndexOfMax<T>(this IEnumerable<T> enumerable) where T : IComparable<T>
   {
      Maybe<int> _index = nil;
      var currentIndex = 0;
      Maybe<T> _currentValue = nil;
      foreach (var item in enumerable)
      {
         if (_currentValue is (true, var currentValue))
         {
            if (item.ComparedTo(currentValue) > 0)
            {
               _index = currentIndex;
               _currentValue = item;
            }
         }
         else
         {
            _index = currentIndex;
            _currentValue = item;
         }

         currentIndex++;
      }

      return _index;
   }

   public static Maybe<int> IndexOfMax<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> mappingFunc)
      where TResult : IComparable<TResult>
   {
      Maybe<int> _index = nil;
      var currentIndex = 0;
      Maybe<TResult> _currentValue = nil;
      foreach (var item in enumerable)
      {
         var mappedItem = mappingFunc(item);
         if (_currentValue is (true, var currentValue))
         {
            if (mappedItem.ComparedTo(currentValue) > 0)
            {
               _index = currentIndex;
               _currentValue = mappedItem;
            }
         }
         else
         {
            _index = currentIndex;
            _currentValue = mappedItem;
         }

         currentIndex++;
      }

      return _index;
   }

   public static Maybe<int> IndexOfMin<T>(this IEnumerable<T> enumerable) where T : IComparable<T>
   {
      Maybe<int> _index = nil;
      var currentIndex = 0;
      Maybe<T> _currentValue = nil;
      foreach (var item in enumerable)
      {
         if (_currentValue is (true, var currentValue))
         {
            if (item.ComparedTo(currentValue) < 0)
            {
               _index = currentIndex;
               _currentValue = item;
            }
         }
         else
         {
            _index = currentIndex;
            _currentValue = item;
         }

         currentIndex++;
      }

      return _index;
   }

   public static Maybe<int> IndexOfMin<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> mappingFunc)
      where TResult : IComparable<TResult>
   {
      Maybe<int> _index = nil;
      var currentIndex = 0;
      Maybe<TResult> _currentValue = nil;
      foreach (var item in enumerable)
      {
         var mappedItem = mappingFunc(item);
         if (_currentValue is (true, var currentValue))
         {
            if (mappedItem.ComparedTo(currentValue) < 0)
            {
               _index = currentIndex;
               _currentValue = mappedItem;
            }
         }
         else
         {
            _index = currentIndex;
            _currentValue = mappedItem;
         }

         currentIndex++;
      }

      return _index;
   }

   public static IEnumerable<T> Reversed<T>(this IEnumerable<T> enumerable)
   {
      List<T> list = [.. enumerable];
      list.Reverse();

      return list;
   }

   public static Maybe<TResult> FirstOrNoneAs<T, TResult>(this IEnumerable<T> enumerable) where T : notnull where TResult : notnull
   {
      return enumerable.FirstOrNone(i => i is TResult).CastAs<TResult>();
   }

   public static Result<TResult> FirstOrFailAs<T, TResult>(this IEnumerable<T> enumerable) where T : notnull where TResult : notnull
   {
      return enumerable.FirstOrFailure(i => i is TResult).CastAs<TResult>();
   }

   public static bool AtLeastOne<T>(this IEnumerable<T> enumerable) where T : notnull => enumerable.FirstOrNone();

   public static bool AtLeastOne<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) where T : notnull => enumerable.FirstOrNone(predicate);

   public static IEnumerable<T> Do<T>(this IEnumerable<T> enumerable, Action<T> action)
   {
      foreach (var value in enumerable)
      {
         action(value);
         yield return value;
      }
   }

   public static IEnumerable<T> DoIf<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Action<T> action)
   {
      foreach (var value in enumerable)
      {
         if (predicate(value))
         {
            action(value);
         }

         yield return value;
      }
   }

   public static IEnumerable<T> DoIfElse<T>(this IEnumerable<T> enumerable, Predicate<T> predicate, Action<T> ifTrue, Action<T> ifFalse)
   {
      foreach (var value in enumerable)
      {
         if (predicate(value))
         {
            ifTrue(value);
         }
         else
         {
            ifFalse(value);
         }

         yield return value;
      }
   }

   public static bool AllMatch<T1, T2>(this IEnumerable<T1> leftEnumerable, IEnumerable<T2> rightEnumerable, Func<T1, T2, bool> matcher,
      bool mustBeSameLength = true) where T2 : notnull
   {
      T1[] left = [.. leftEnumerable];
      T2[] right = [.. rightEnumerable];

      if (mustBeSameLength && left.Length != right.Length)
      {
         return false;
      }

      foreach (var leftItem in left)
      {
         if (!right.AtLeastOne(r => matcher(leftItem, r)))
         {
            return false;
         }
      }

      return true;
   }

   public static IEnumerable<(T1, Maybe<T2>)> AllMatched<T1, T2>(this IEnumerable<T1> leftEnumerable, IEnumerable<T2> rightEnumerable,
      Func<T1, T2, bool> matcher) where T2 : notnull
   {
      T2[] rightArray = [.. rightEnumerable];
      foreach (var left in leftEnumerable)
      {
         yield return (left, rightArray.FirstOrNone(r => matcher(left, r)));
      }
   }

   public static IEnumerable<(int index, T item)> IndexedEnumerable<T>(this IEnumerable<T> enumerable)
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         yield return (index++, item);
      }
   }

   public static Set<T> ToSet<T>(this IEnumerable<T> enumerable) => [.. enumerable];

   public static StringSet ToStringSet(this IEnumerable<string> enumerable, bool ignoreCase)
   {
      StringSet set = [.. enumerable];
      return set.CaseIgnore(ignoreCase);
   }

   public static IEnumerable<T> For<T>(this IEnumerable<T> enumerable, Action<T> action)
   {
      foreach (var item in enumerable)
      {
         action(item);
         yield return item;
      }
   }

   public static IEnumerable<(T1, T2)> For<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Action<T1, T2> action)
   {
      foreach (var (item1, item2) in enumerable)
      {
         action(item1, item2);
         yield return (item1, item2);
      }
   }

   public static IEnumerable<(T1, T2, T3)> For<T1, T2, T3>(this IEnumerable<(T1, T2, T3)> enumerable, Action<T1, T2, T3> action)
   {
      foreach (var (item1, item2, item3) in enumerable)
      {
         action(item1, item2, item3);
         yield return (item1, item2, item3);
      }
   }

   public static IEnumerable<(T1, T2, T3, T4)> For<T1, T2, T3, T4>(this IEnumerable<(T1, T2, T3, T4)> enumerable, Action<T1, T2, T3, T4> action)
   {
      foreach (var (item1, item2, item3, item4) in enumerable)
      {
         action(item1, item2, item3, item4);
         yield return (item1, item2, item3, item4);
      }
   }

   public static IEnumerable<IEnumerable<T>> Cluster<T>(this IEnumerable<T> enumerable, int count)
   {
      T[] array = [.. enumerable];
      for (var i = 0; i < array.Length; i += count)
      {
         yield return array.Skip(i).Take(count);
      }
   }

   public static IEnumerable<T> SortByList<T>(this IEnumerable<T> enumerable, Func<T, string> keyMap, params string[] keys) where T : notnull
   {
      StringSet keySet = [.. keys];
      StringHash<T> matching = [];
      List<T> remainder = [];
      foreach (var item in enumerable)
      {
         var key = keyMap(item);
         if (keySet.Contains(key))
         {
            matching[key] = item;
         }
         else
         {
            remainder.Add(item);
         }
      }

      foreach (var key in keySet)
      {
         if (matching.Maybe[key] is (true, var item))
         {
            yield return item;
         }
      }

      foreach (var item in remainder.OrderBy(keyMap))
      {
         yield return item;
      }
   }

   public static IEnumerable<T> SortByList<T>(this IEnumerable<T> enumerable, Func<T, string> keyMap, IEnumerable<string> keys) where T : notnull
   {
      return enumerable.SortByList(keyMap, [.. keys]);
   }

   public static IEnumerable<T> SortByList<T>(this IEnumerable<T> enumerable, Func<T, string> keyMap, Func<T, T, int> compareFunc,
      params string[] keys)
   {
      var comparer = Comparer<T>.Create((x, y) => compareFunc(x, y));
      StringSet keySet = [.. keys];
      var matching = new AutoStringHash<SortedSet<T>>(_ => new SortedSet<T>(comparer), true);
      var remainder = new SortedSet<T>(comparer);

      foreach (var item in enumerable)
      {
         var key = keyMap(item);
         if (keySet.Contains(key))
         {
            matching[key].Add(item);
         }
         else
         {
            remainder.Add(item);
         }
      }

      foreach (var key in keySet)
      {
         if (matching.Maybe[key] is (true, var list))
         {
            foreach (var item in list)
            {
               yield return item;
            }
         }
      }

      foreach (var item in remainder)
      {
         yield return item;
      }
   }

   public static IEnumerable<T> SortByList<T>(this IEnumerable<T> enumerable, Func<T, string> keyMap, Func<T, T, int> compareFunc,
      IEnumerable<string> keys)
   {
      return enumerable.SortByList(keyMap, compareFunc, [.. keys]);
   }

   private static AutoHash<T, int> getPaired<T>(IEnumerable<T> orderItems, int defaultValue) where T : notnull
   {
      return orderItems.Select((v, i) => (value: v, index: i)).ToHash(i => i.value, i => i.index).ToAutoHash(defaultValue);
   }

   public static IOrderedEnumerable<T> OrderBy<T>(this IEnumerable<T> enumerable, IEnumerable<T> orderItems) where T : notnull
   {
      var paired = getPaired(orderItems, int.MaxValue);
      return enumerable.OrderBy(i => paired[i]);
   }

   public static IOrderedEnumerable<T> ThenBy<T>(this IOrderedEnumerable<T> enumerable, IEnumerable<T> orderItems) where T : notnull
   {
      var paired = getPaired(orderItems, int.MaxValue);
      return enumerable.ThenBy(i => paired[i]);
   }

   public static IOrderedEnumerable<T> OrderBy<T, TMember>(this IEnumerable<T> enumerable, Func<T, TMember> mapper,
      IEnumerable<TMember> orderItems) where TMember : notnull
   {
      var paired = getPaired(orderItems, int.MaxValue);
      return enumerable.OrderBy(i => paired[mapper(i)]);
   }

   public static IOrderedEnumerable<T> ThenBy<T, TMember>(this IOrderedEnumerable<T> enumerable, Func<T, TMember> mapper,
      IEnumerable<TMember> orderItems) where TMember : notnull
   {
      var paired = getPaired(orderItems, int.MaxValue);
      return enumerable.ThenBy(i => paired[mapper(i)]);
   }

   public static IOrderedEnumerable<T> OrderByDescending<T>(this IEnumerable<T> enumerable, IEnumerable<T> orderItems) where T : notnull
   {
      var paired = getPaired(orderItems, int.MinValue);
      return enumerable.OrderByDescending(i => paired[i]);
   }

   public static IOrderedEnumerable<T> ThenByDescending<T>(this IOrderedEnumerable<T> enumerable, IEnumerable<T> orderItems) where T : notnull
   {
      var paired = getPaired(orderItems, int.MinValue);
      return enumerable.ThenByDescending(i => paired[i]);
   }

   public static IOrderedEnumerable<T> OrderByDescending<T, TMember>(this IEnumerable<T> enumerable, Func<T, TMember> mapper,
      IEnumerable<TMember> orderItems) where TMember : notnull
   {
      var paired = getPaired(orderItems, int.MinValue);
      return enumerable.OrderByDescending(i => paired[mapper(i)]);
   }

   public static IOrderedEnumerable<T> ThenByDescending<T, TMember>(this IOrderedEnumerable<T> enumerable, Func<T, TMember> mapper,
      IEnumerable<TMember> orderItems) where TMember : notnull
   {
      var paired = getPaired(orderItems, int.MinValue);
      return enumerable.ThenByDescending(i => paired[mapper(i)]);
   }

   public static IEnumerable<TValue> Distinct<TValue, TKey>(this IEnumerable<TValue> enumerable, Func<TValue, TKey> keySelector)
   {
      var knownKeys = new HashSet<TKey>();
      foreach (var value in enumerable)
      {
         if (knownKeys.Add(keySelector(value)))
         {
            yield return value;
         }
      }
   }

   public static Maybe<int> Find<T>(this IEnumerable<T> items, T item, int startIndex = 0)
   {
      T[] array = [.. items];
      for (var i = startIndex; i < array.Length; i++)
      {
         if (array[i]!.Equals(item))
         {
            return i;
         }
      }

      return nil;
   }

   public static Maybe<int> Find<T>(this IEnumerable<T> items, Func<T, bool> predicate, int startIndex = 0)
   {
      T[] array = [.. items];
      for (var i = startIndex; i < array.Length; i++)
      {
         if (predicate(array[i]))
         {
            return i;
         }
      }

      return nil;
   }

   public static IEnumerable<int> FindAll<T>(this IEnumerable<T> items, T item, int startIndex = 0)
   {
      T[] array = [.. items];
      for (var i = startIndex; i < array.Length; i++)
      {
         if (array[i]!.Equals(item))
         {
            yield return i;
         }
      }
   }

   public static IEnumerable<TResult> Merge<T1, T2, TResult>(this IEnumerable<T1> left, IEnumerable<T2> right, Func<T1, T2, TResult> map)
      where T1 : notnull where T2 : notnull
   {
      var leftQueue = new EnumerableQueue<T1>(left);
      var rightQueue = new EnumerableQueue<T2>(right);

      while (leftQueue.Next() is (true, var leftValue) && rightQueue.Next() is (true, var rightValue))
      {
         yield return map(leftValue, rightValue);
      }
   }

   public static IEnumerable<(T1 left, T2 right)> Merge<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right) where T1 : notnull
      where T2 : notnull
   {
      return left.Merge<T1, T2, (T1, T2)>(right, (t1, t2) => (t1, t2));
   }

   public static string Andify<T>(this IEnumerable<T> enumerable) where T : notnull
   {
      T[] array = [.. enumerable];
      var length = array.Length;
      return length switch
      {
         0 => "",
         1 => array[0].ToString() ?? "",
         2 => $"{array[0]} and {array[1]}",
         _ => $"{array.Take(array.Length - 1).ToString(", ")}, and {array[^1]}"
      };
   }

   public static IEnumerator<int> GetEnumerator(this Range range)
   {
      var start = range.Start.Value;
      var end = range.End.Value;
      if (end > start)
      {
         for (var i = start; i < end; i++)
         {
            yield return i;
         }
      }
      else
      {
         for (var i = start; i > end; i--)
         {
            yield return i;
         }
      }
   }
}