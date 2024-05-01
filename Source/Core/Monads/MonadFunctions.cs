using System;

namespace Core.Monads;

public static class MonadFunctions
{
   public static readonly Nil nil = new();

   public static readonly Unit unit = new();

   public static Exception fail(string message) => new ApplicationException(message);

   public static Maybe<TParent> some<TChild, TParent>(TChild? value) where TChild : TParent where TParent : notnull
   {
      if (value is not null)
      {
         return new Some<TParent>(value);
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<TParent> someAs<TChild, TParent>(TChild value) where TChild : class, TParent where TParent : notnull
   {
      return new Some<TParent>(value);
   }

   public static Result<T> success<T>(T value) where T : notnull => new Success<T>(value);

   public static Result<TParent> successAs<TChild, TParent>(TChild value) where TChild : class, TParent where TParent : class
   {
      return new Success<TParent>(value);
   }

   [Obsolete("Use maybe function")]
   public static Maybe<T> maybe<T>(bool test, Func<T> ifTrue) where T : notnull => test ? ifTrue().Some() : nil;

   [Obsolete("Use maybe function")]
   public static Maybe<T> maybe<T>(bool test, Func<Maybe<T>> ifTrue) where T : notnull => test ? ifTrue() : nil;

   [Obsolete("Use nil")]
   public static Completion<T> cancelled<T>() where T : notnull => new Cancelled<T>();

   [Obsolete("Use exception")]
   public static Completion<T> interrupted<T>(Exception exception) where T : notnull => new Interrupted<T>(exception);

   public static Result<T> assert<T>(bool test, Func<T> ifTrue, Func<string> ifFalse) where T : notnull
   {
      try
      {
         return test ? ifTrue().Success() : ifFalse().Failure<T>();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<T> assert<T>(bool test, Func<Result<T>> ifTrue, Func<string> ifFalse) where T : notnull
   {
      try
      {
         return test ? ifTrue() : ifFalse().Failure<T>();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Result<Unit> assert(bool test, Func<string> ifFalse)
   {
      try
      {
         return test ? unit : ifFalse().Failure<Unit>();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public static Maybe<T>.If maybe<T>() where T : notnull => new(true);

   public static Result<T>.If result<T>() where T : notnull => new(true, nil, nil);

   public static Optional<T>.If optional<T>() where T : notnull => new(true);
}