using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Matching.MultiMatching;

public class MultiMatcher<T> where T : notnull
{
   public class PatternAction(Pattern pattern, Func<MatchResult, T> func)
   {
      protected Pattern pattern = pattern;
      protected Func<MatchResult, T> func = func;

      public Pattern Pattern => pattern;

      public Func<MatchResult, T> Func => func;

      public void Deconstruct(out Pattern pattern, out Func<MatchResult, T> func)
      {
         pattern = Pattern;
         func = Func;
      }
   }

   public class Case(MultiMatcher<T> multiMatcher, Pattern pattern)
   {
      public static MultiMatcher<T> operator &(Case @case, Func<MatchResult, T> func) => @case.Then(func);

      protected MultiMatcher<T> multiMatcher = multiMatcher;

      public Pattern Pattern => pattern;

      public MultiMatcher<T> Then(Func<MatchResult, T> func)
      {
         multiMatcher.AddPattern(Pattern, func);
         return multiMatcher;
      }
   }

   public static Case operator &(MultiMatcher<T> multiMatcher, Pattern pattern) => multiMatcher.When(pattern);

   public static MultiMatcher<T> operator &(MultiMatcher<T> multiMatcher, Func<string, T> func) => multiMatcher.Else(func);

   protected List<PatternAction> patternActions = [];
   protected Maybe<Func<string, T>> _defaultResult = nil;

   public Case When(Pattern pattern) => new(this, pattern);

   internal void AddPattern(Pattern pattern, Func<MatchResult, T> func) => patternActions.Add(new PatternAction(pattern, func));

   public MultiMatcher<T> Else(Func<string, T> func)
   {
      if (!_defaultResult)
      {
         _defaultResult = func;
      }

      return this;
   }

   public Optional<T> Matches(string input)
   {
      foreach (var (pattern, func) in patternActions)
      {
         var _result = input.Matches(pattern);
         if (_result is (true, var result))
         {
            try
            {
               return func(result);
            }
            catch (Exception exception)
            {
               return exception;
            }
         }
      }

      if (_defaultResult is (true, var defaultResult))
      {
         try
         {
            return defaultResult(input);
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      return nil;
   }
}

public class MultiMatcher
{
   public class PatternAction(Pattern pattern, Action<MatchResult> action)
   {
      protected Pattern pattern = pattern;
      protected Action<MatchResult> action = action;

      public Pattern Pattern => pattern;

      public Action<MatchResult> Action => action;

      public void Deconstruct(out Pattern pattern, out Action<MatchResult> action)
      {
         pattern = Pattern;
         action = Action;
      }
   }

   public class Case(MultiMatcher multiMatcher, Pattern pattern)
   {
      public static MultiMatcher operator &(Case @case, Action<MatchResult> func) => @case.Then(func);

      protected MultiMatcher multiMatcher = multiMatcher;

      public Pattern Pattern => pattern;

      public MultiMatcher Then(Action<MatchResult> action)
      {
         multiMatcher.AddPattern(Pattern, action);
         return multiMatcher;
      }
   }

   public static Case operator &(MultiMatcher multiMatcher, Pattern pattern) => multiMatcher.When(pattern);

   public static MultiMatcher operator &(MultiMatcher multiMatcher, Action<string> action) => multiMatcher.Else(action);

   protected List<PatternAction> patternActions = [];
   protected Maybe<Action<string>> _defaultAction = nil;

   public Case When(Pattern pattern) => new(this, pattern);

   internal void AddPattern(Pattern pattern, Action<MatchResult> action) => patternActions.Add(new PatternAction(pattern, action));

   public MultiMatcher Else(Action<string> action)
   {
      if (!_defaultAction)
      {
         _defaultAction = action;
      }

      return this;
   }

   public Optional<Unit> Matches(string input)
   {
      foreach (var (pattern, action) in patternActions)
      {
         var _result = input.Matches(pattern);
         if (_result is (true, var result))
         {
            try
            {
               action(result);
               return unit;
            }
            catch (Exception exception)
            {
               return exception;
            }
         }
      }

      if (_defaultAction is (true, var defaultAction))
      {
         try
         {
            defaultAction(input);
            return unit;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      return nil;
   }
}