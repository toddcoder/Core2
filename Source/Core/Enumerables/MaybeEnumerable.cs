using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables;

public class MaybeEnumerable<T>(IEnumerable<T> enumerable) : IEnumerable<T> where T : notnull
{
   protected T[] array = [.. enumerable];
   protected int index;

   public Maybe<T> Next()
   {
      if (More)
      {
         return array[index++];
      }
      else
      {
         return nil;
      }
   }

   public Maybe<T> Previous()
   {
      if (More)
      {
         return array[index--];
      }
      else
      {
         return nil;
      }
   }

   public bool More => index >= 0 && index < array.Length;

   public void Reset() => index = 0;

   public void ResetToLast() => index = array.Length - 1;

   public IEnumerator<T> GetEnumerator()
   {
      foreach (var item in array)
      {
         yield return item;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public IEnumerable<T> Rest() => array.Skip(index);

   public IEnumerable<T> RestFromLast() => array.Take(index).Reversed();
}