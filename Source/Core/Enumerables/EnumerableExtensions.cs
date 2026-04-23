using System;
using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Collections;
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
      return string.Join(connector, enumerable.Select(i => i!.ToNonNullString()));
   }

   extension<T>(IEnumerable<T> enumerable)
   {
      public IEnumerable<T> SortBy(Func<T, int> sorter)
      {
         var hash = new Memo<int, List<T>>.Function(_ => []);
         foreach (var item in enumerable)
         {
            hash[sorter(item)].Add(item);
         }

         foreach (var key in hash.Keys.Order())
         {
            foreach (var item in hash[key].Order())
            {
               yield return item;
            }
         }
      }
   }

   extension<T>(IEnumerable<IEnumerable<T>> source)
   {
      public IEnumerable<IEnumerable<T>> Pivot(Func<T> defaultValue)
      {
         T[][] array = [.. source.Select(row => (T[])[.. row])];
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

      public IEnumerable<IEnumerable<T>> Pivot(T defaultValue) => source.Pivot(() => defaultValue);
   }

   extension<T>(IEnumerable<T> enumerable)
   {
      public Result<T[]> ToResultOfArray()
      {
         try
         {
            return (T[])[.. enumerable];
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Result<List<T>> ToResultOfList()
      {
         try
         {
            return (List<T>)[.. enumerable];
         }
         catch (Exception exception)
         {
            return exception;
         }
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

   extension<T>(T seed)
   {
      public IEnumerable<T> Then(Func<T, T> next, Predicate<T> stop)
      {
         var current = seed;

         yield return current;

         while (!stop(current))
         {
            current = next(current);
            yield return current;
         }
      }

      public IEnumerable<T> Then(Func<T, T> next) => seed.Then(next, _ => false);
   }

   public static IEnumerable<(int index, T item)> Indexed<T>(this IEnumerable<T> enumerable)
   {
      var index = 0;
      foreach (var item in enumerable)
      {
         yield return (index++, item);
      }
   }

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Maybe<int> IndexOf(T needle)
      {
         foreach (var (index, item) in enumerable.Indexed())
         {
            if (needle.Equals(item))
            {
               return index;
            }
         }

         return nil;
      }

      public Maybe<int> LastIndexOf(T needle)
      {
         foreach (var (index, item) in enumerable.Indexed().Reversed())
         {
            if (needle.Equals(item))
            {
               return index;
            }
         }

         return nil;
      }

      public Maybe<T> FirstOrNone()
      {
         var first = enumerable.FirstOrDefault();

         if (first is not null)
         {
            return first;
         }
         else
         {
            return nil;
         }
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Maybe<T> LastOrNone()
      {
         var last = enumerable.LastOrDefault();

         if (last is not null)
         {
            return last;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<T> AnyOrNone(Func<T, T, bool> predicate, params T[] needles)
      {
         T[] array = [.. enumerable];
         foreach (var needle in needles)
         {
            var _result = array.FirstOrNone(i => predicate(needle, i));
            if (_result is (true, var result))
            {
               return result;
            }
         }

         return nil;
      }

      public Maybe<T> AnyOrNone(Func<T, T, bool> predicate, Func<T, T, T> returnFunc, params T[] needles)
      {
         T[] array = [.. enumerable];
         foreach (var needle in needles)
         {
            var _result = array.FirstOrNone(i => predicate(needle, i));
            if (_result is (true, var result))
            {
               return returnFunc(needle, result);
            }
         }

         return nil;
      }

      public Result<T> FirstOrFailure(string failureMessage = "Default value")
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

      public Result<T> FirstOrFailure(Predicate<T> predicate, string failureMessage = "Default value")
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Result<T> FirstOrFailure(Func<string> failureMessage)
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

      public Result<T> FirstOrFailure(Predicate<T> predicate, Func<string> failureMessage)
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Result<T> LastOrFailure(string failureMessage = "Default value")
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

      public Result<T> LastOrFailure(Predicate<T> predicate,
         string failureMessage = "Default value")
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Result<T> LastOrFailure(Func<string> failureMessage)
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

      public Result<T> LastOrFailure(Predicate<T> predicate, Func<string> failureMessage)
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

      if (first is not null)
      {
         return first;
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<(T1, T2)> FirstOrNone<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
   {
      var first = enumerable.FirstOrDefault(i => predicate(i.Item1, i.Item2));

      if (first.AnyNull() || first.Equals(default))
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

      if (first.AnyNull() || first.Equals(default))
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

      if (first.AnyNull() || first.Equals(default))
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

      if (last is not null)
      {
         return last;
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<(T1, T2)> LastOrNone<T1, T2>(this IEnumerable<(T1, T2)> enumerable, Func<T1, T2, bool> predicate)
   {
      var last = enumerable.LastOrDefault(i => predicate(i.Item1, i.Item2));

      if (last.AnyNull() || last.Equals(default))
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

      if (last.AnyNull() || last.Equals(default))
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

      if (last.AnyNull() || last.Equals(default))
      {
         return nil;
      }
      else
      {
         return last;
      }
   }

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Optional<T> FirstOrEmpty() => enumerable.FirstOrNone().Optional();

      public Optional<T> FirstOrEmpty(Func<T, bool> predicate)
      {
         return enumerable.FirstOrNone(predicate).Optional();
      }
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Optional<T> LastOrEmpty() => enumerable.LastOrNone().Optional();

      public Optional<T> LastOrEmpty(Func<T, bool> predicate)
      {
         return enumerable.LastOrNone(predicate).Optional();
      }
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Optional<T> FirstOrFail(string failMessage = "Default value")
      {
         return enumerable.FirstOrFailure(failMessage).Optional();
      }

      public Optional<T> FirstOrFail(Predicate<T> predicate, string failMessage = "Default value")
      {
         return enumerable.FirstOrFailure(predicate, failMessage).Optional();
      }
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Optional<T> FirstOrFail(Func<string> failMessage)
      {
         return enumerable.FirstOrFailure(failMessage).Optional();
      }

      public Optional<T> FirstOrFail(Predicate<T> predicate, Func<string> failMessage)
      {
         return enumerable.FirstOrFailure(predicate, failMessage).Optional();
      }
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Optional<T> LastOrFail(string failMessage = "Default value")
      {
         return enumerable.LastOrFailure(failMessage).Optional();
      }

      public Optional<T> LastOrFail(Predicate<T> predicate, string failMessage = "Default value")
      {
         return enumerable.LastOrFailure(predicate, failMessage).Optional();
      }
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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Optional<T> LastOrFail(Func<string> failMessage)
      {
         return enumerable.LastOrFailure(failMessage).Optional();
      }

      public Optional<T> LastOrFail(Predicate<T> predicate, Func<string> failMessage)
      {
         return enumerable.LastOrFailure(predicate, failMessage).Optional();
      }
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

   extension<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> enumerable) where TKey : notnull where TValue : notnull
   {
      public Hash<TKey, TValue> ToHash()
      {
         return enumerable.ToHash(kv => kv.Key, kv => kv.Value);
      }

      public Hash<TKey, TValue> ToHash(IEqualityComparer<TKey> comparer)
      {
         return enumerable.ToHash(kv => kv.Key, kv => kv.Value, comparer);
      }
   }

   extension<TSource>(IEnumerable<IEnumerable<TSource>> enumerable)
   {
      public IEnumerable<TResult> FlatMap<TResult>(Func<TSource, TResult> mapFunc)
      {
         foreach (var outer in enumerable)
         {
            foreach (var inner in outer)
            {
               yield return mapFunc(inner);
            }
         }
      }

      public IEnumerable<TSource> Flatten()
      {
         foreach (var outer in enumerable)
         {
            foreach (var inner in outer)
            {
               yield return inner;
            }
         }
      }
   }

   extension<TSource>(IEnumerable<TSource> enumerable)
   {
      public TResult FoldLeft<TResult>(TResult init,
         Func<TResult, TSource, TResult> foldFunc)
      {
         return enumerable.Aggregate(init, foldFunc);
      }

      public TResult FoldRight<TResult>(TResult init,
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

      public TSource FoldLeft(Func<TSource, TSource, TSource> foldFunc) => enumerable.Aggregate(foldFunc);

      public TSource FoldRight(Func<TSource, TSource, TSource> foldFunc)
      {
         List<TSource> list = [.. enumerable];
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

      public TResult Fold<TResult>(Func<TResult, TSource, TResult> foldFunc, TResult defaultValue)
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

   extension<TSource>(IEnumerable<TSource> enumerable)
   {
      public Maybe<int> IndexOfMin<TResult>(Func<TSource, TResult> mappingFunc)
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

      public IEnumerable<TSource> Reversed()
      {
         List<TSource> list = [.. enumerable];
         list.Reverse();

         return list;
      }
   }

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public Maybe<TResult> FirstOrNoneAs<TResult>() where TResult : notnull
      {
         return enumerable.FirstOrNone(i => i is TResult).CastAs<TResult>();
      }

      public Result<TResult> FirstOrFailAs<TResult>() where TResult : notnull
      {
         return enumerable.FirstOrFailure(i => i is TResult).CastAs<TResult>();
      }

      public bool AtLeastOne() => enumerable.FirstOrNone();
      public bool AtLeastOne(Func<T, bool> predicate) => enumerable.FirstOrNone(predicate);
   }

   extension<T>(IEnumerable<T> enumerable)
   {
      public IEnumerable<T> Do(Action<T> action)
      {
         foreach (var value in enumerable)
         {
            action(value);
            yield return value;
         }
      }

      public IEnumerable<T> DoIf(Predicate<T> predicate, Action<T> action)
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

      public IEnumerable<T> DoIfElse(Predicate<T> predicate, Action<T> ifTrue, Action<T> ifFalse)
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

      public bool AllMatch<T2>(IEnumerable<T2> rightEnumerable, Func<T, T2, bool> matcher,
         bool mustBeSameLength = true) where T2 : notnull
      {
         T[] left = [.. enumerable];
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

      public IEnumerable<(T, Maybe<T2>)> AllMatched<T2>(IEnumerable<T2> rightEnumerable,
         Func<T, T2, bool> matcher) where T2 : notnull
      {
         T2[] rightArray = [.. rightEnumerable];
         foreach (var left in enumerable)
         {
            yield return (left, rightArray.FirstOrNone(r => matcher(left, r)));
         }
      }

      [Obsolete("Use native index")]
      public IEnumerable<(int index, T item)> IndexedEnumerable()
      {
         var index = 0;
         foreach (var item in enumerable)
         {
            yield return (index++, item);
         }
      }

      public Set<T> ToSet() => [.. enumerable];
   }

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

   extension<T>(IEnumerable<T> enumerable) where T : notnull
   {
      public IEnumerable<T> SortByList(Func<T, string> keyMap, params string[] keys)
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

      public IEnumerable<T> SortByList(Func<T, string> keyMap, IEnumerable<string> keys)
      {
         return enumerable.SortByList(keyMap, [.. keys]);
      }
   }

   extension<T>(IEnumerable<T> enumerable)
   {
      public IEnumerable<T> SortByList(Func<T, string> keyMap, Func<T, T, int> compareFunc,
         params string[] keys)
      {
         var comparer = Comparer<T>.Create((x, y) => compareFunc(x, y));
         StringSet keySet = [.. keys];
         Memo<string, SortedSet<T>> matching = new Memo<string, SortedSet<T>>.Function(_ => []);
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

         Hash<string, SortedSet<T>> matchingHash = matching;
         foreach (var key in keySet)
         {
            if (matchingHash.Maybe[key] is (true, var list))
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

      public IEnumerable<T> SortByList(Func<T, string> keyMap, Func<T, T, int> compareFunc,
         IEnumerable<string> keys)
      {
         return enumerable.SortByList(keyMap, compareFunc, [.. keys]);
      }
   }

   private static Memo<T, int> getPaired<T>(IEnumerable<T> orderItems, int defaultValue) where T : notnull
   {
      return orderItems.Select((v, i) => (value: v, index: i)).ToHash(i => i.value, i => i.index).Memo(defaultValue);
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

   extension<TValue>(IEnumerable<TValue> enumerable)
   {
      public IEnumerable<TValue> Distinct<TKey>(Func<TValue, TKey> keySelector)
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

      public Maybe<int> Find(TValue item, int startIndex = 0)
      {
         TValue[] array = [.. enumerable];
         for (var i = startIndex; i < array.Length; i++)
         {
            if (array[i]!.Equals(item))
            {
               return i;
            }
         }

         return nil;
      }

      public Maybe<int> Find(Func<TValue, bool> predicate, int startIndex = 0)
      {
         TValue[] array = [.. enumerable];
         for (var i = startIndex; i < array.Length; i++)
         {
            if (predicate(array[i]))
            {
               return i;
            }
         }

         return nil;
      }

      public IEnumerable<int> FindAll(TValue item, int startIndex = 0)
      {
         TValue[] array = [.. enumerable];
         for (var i = startIndex; i < array.Length; i++)
         {
            if (array[i]!.Equals(item))
            {
               yield return i;
            }
         }
      }
   }

   extension<T1>(IEnumerable<T1> left) where T1 : notnull
   {
      public IEnumerable<TResult> Merge<T2, TResult>(IEnumerable<T2> right, Func<T1, T2, TResult> map) where T2 : notnull
      {
         var leftQueue = new EnumerableQueue<T1>(left);
         var rightQueue = new EnumerableQueue<T2>(right);

         while (leftQueue.Next() is (true, var leftValue) && rightQueue.Next() is (true, var rightValue))
         {
            yield return map(leftValue, rightValue);
         }
      }

      public IEnumerable<(T1 left, T2 right)> Merge<T2>(IEnumerable<T2> right) where T2 : notnull
      {
         return left.Merge<T1, T2, (T1, T2)>(right, (t1, t2) => (t1, t2));
      }

      public string Andify()
      {
         T1[] array = [.. left];
         var length = array.Length;
         return length switch
         {
            0 => "",
            1 => array[0].ToString() ?? "",
            2 => $"{array[0]} and {array[1]}",
            _ => $"{array.Take(array.Length - 1).ToString(", ")}, and {array[^1]}"
         };
      }
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

   public static (Maybe<T> head, IEnumerable<T> tail) HeadAndTail<T>(this IEnumerable<T> enumerable) where T : notnull
   {
      using var enumerator = enumerable.GetEnumerator();
      if (enumerator.MoveNext())
      {
         var head = enumerator.Current;
         List<T> tail = [];
         while (enumerator.MoveNext())
         {
            tail.Add(enumerator.Current);
         }

         return (head, tail);
      }
      else
      {
         return (nil, []);
      }
   }

   public static IEnumerable<int> Range(this int count) => Enumerable.Range(0, count);
}