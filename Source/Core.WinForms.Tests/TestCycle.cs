using System.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public class TestCycle : IEnumerable<(string outerValue, string innerValue)>
{
   protected string[] outer = ["Alfa", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot"];
   protected string[] inner = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
   protected int outerIndex;
   protected int innerIndex;

   public int OuterLength => outer.Length;

   public int InnerLength => inner.Length;

   public Maybe<(string outerValue, string innerValue)> Next()
   {
      if (innerIndex < 9)
      {
         var tuple = (outer[outerIndex], inner[innerIndex]);
         innerIndex++;

         return tuple;
      }
      else if (outerIndex < 6)
      {
         var tuple = (outer[outerIndex], inner[innerIndex]);
         outerIndex++;
         innerIndex = 0;

         return tuple;
      }
      else
      {
         return nil;
      }
   }

   public IEnumerator<(string outerValue, string innerValue)> GetEnumerator()
   {
      while (Next() is (true, var tuple))
      {
         yield return tuple;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}