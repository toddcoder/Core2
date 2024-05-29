using System;
using System.Collections.Generic;
using Core.Assertions;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.Lists;

public static class ListExtensions
{
   public static bool AllEqualTo<T>(this List<T> left, List<T> right) where T : IEquatable<T>
   {
      left.Must().Not.BeNullOrEmpty().OrThrow();
      right.Must().Not.BeNullOrEmpty().OrThrow();

      if (left.Count != right.Count)
      {
         return false;
      }
      else
      {
         for (var i = 0; i < left.Count; i++)
         {
            if (!left[i].Equals(right[i]))
            {
               return false;
            }
         }

         return true;
      }
   }

   public static Maybe<T> Get<T>(this List<T> list, int index) where T : notnull
   {
      return maybe<T>() & index.Between(0).Until(list.Count) & (() => list[index]);
   }

   public static void Set<T>(this List<T> list, int index, Maybe<T> _value) where T : notnull
   {
      if (index.Between(0).Until(list.Count))
      {
         if (_value is (true, var value))
         {
            list[index] = value;
         }
         else
         {
            list.RemoveAt(index);
         }
      }
   }
}