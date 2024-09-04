using System;
using System.Collections.Generic;

namespace Core.Enums;

public static class EnumFunctions
{
   public static IEnumerable<T> enumEnumerable<T>() where T : struct, Enum
   {
      foreach (var value in (T[])Enum.GetValues<T>())
      {
         yield return value;
      }
   }
}