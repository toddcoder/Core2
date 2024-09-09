using System;
using System.Collections;
using System.Collections.Generic;

namespace Core.Collections;

public class Ring<T> : IEnumerable<T>
{
   protected List<T> list;
   protected int index;

   public Ring()
   {
      list = [];
      index = 0;
   }

   public Ring(IEnumerable<T> enumerable) : this()
   {
      list.AddRange(enumerable);
   }

   public Ring(params T[] args) : this()
   {
      list.AddRange(args);
   }

   public Ring(Span<T> span) : this()
   {
      list.AddRange(span);
   }

   public void Add(T item) => list.Add(item);

   public void AddRange(IEnumerable<T> collection) => list.AddRange(collection);

   public int Count => list.Count;

   [Obsolete("Use enumerable interface")]
   public T Next()
   {
      var result = list[index];
      if (++index >= list.Count)
      {
         index = 0;
      }

      return result;
   }

   public IEnumerator<T> GetEnumerator()
   {
      index = 0;
      while (true)
      {
         var result = list[index];
         if (++index >= list.Count)
         {
            index = 0;
         }

         yield return result;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}