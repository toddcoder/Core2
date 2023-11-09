using System.Collections.Generic;
using Core.Strings;

namespace Core.Collections;

public class IgnoreCaseEqualityComparer : IEqualityComparer<string>
{
   public bool Equals(string? x, string? y) => x is not null && y is not null && x.Same(y);

   public int GetHashCode(string obj) => obj.ToUpper().GetHashCode();
}