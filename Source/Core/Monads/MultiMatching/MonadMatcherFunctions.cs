namespace Core.Monads.MultiMatching;

public static class MonadMatcherFunctions
{
   public static MaybeMatcher<T, TResult> maybeMatcher<T, TResult>() where T : notnull where TResult : notnull => new();

   public static MaybeMatcher<T> maybeMatcher<T>() where T : notnull => new();
}