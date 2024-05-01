namespace Core.Matching.MultiMatching;

public static class MultiMatchingFunctions
{
   public static MultiMatcher<T> match<T>() where T : notnull => new();

   public static MultiMatcher match() => new();
}