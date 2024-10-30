using System.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public class TestCycle : IEnumerable<(string outerValue, string innerValue, bool reset)>
{
   protected string[] outer = ["Alfa", "Bravo", "Charlie", "Delta", "Echo", "Foxtrot"];
   protected string[] inner = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
   protected int outerIndex;
   protected int innerIndex;

   public int OuterLength => outer.Length;

   public int InnerLength => inner.Length;

   public Maybe<(string outerValue, string innerValue, bool reset)> Next()
   {
      if (innerIndex < 9)
      {
         return (outer[outerIndex], inner[innerIndex++], false);
      }
      else if (outerIndex + 1 < 6)
      {
         innerIndex = 0;
         return (outer[++outerIndex], inner[innerIndex], true);
      }
      else
      {
         return nil;
      }
   }

   public IEnumerator<(string outerValue, string innerValue, bool reset)> GetEnumerator()
   {
      while (Next() is (true, var tuple))
      {
         yield return tuple;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}