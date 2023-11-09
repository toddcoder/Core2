using static Core.Monads.MonadFunctions;

namespace Core.Monads;

public class Monads
{
   protected static Maybe<Monads> _function;

   static Monads()
   {
      _function = nil;
   }

   public static Monads monads
   {
      get
      {
         if (!_function)
         {
            _function = new Monads();
         }

         return _function;
      }
   }

   public Maybe<T> maybe<T>() where T : notnull => nil;

   public Result<T> result<T>() where T : notnull => nil;

   public Optional<T> optional<T>() where T : notnull => nil;

   public Completion<T> completion<T>() where T : notnull => nil;
}