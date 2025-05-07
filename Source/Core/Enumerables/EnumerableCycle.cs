using System.Collections;
using System.Collections.Generic;

namespace Core.Enumerables;

public class EnumerableCycle<T> : IEnumerable<T>
{
   protected List<T> list = [];
   protected int index;
   protected int length;
   protected int cycle;

   public void Add(T item)
   {
      list.Add(item);
      length = list.Count;
   }

   public T Next()
   {
      if (index < length)
      {
         var next = list[index];
         index++;

         return next;
      }
      else
      {
         index = 0;
         cycle++;

         return list[index];
      }
   }

   public int Index => index;

   public int Length => length;

   public int Cycle => cycle;

   public bool Cycling { get; set; } = true;

   public IEnumerator<T> GetEnumerator()
   {
      while (Cycling)
      {
         yield return Next();
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}