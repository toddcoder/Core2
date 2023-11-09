using System;
using System.Reflection;
using Core.Assertions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public class Invoker
{
   protected const BindingFlags BASE_BINDINGS = BindingFlags.Public | BindingFlags.Instance;
   protected const BindingFlags METHOD_BINDINGS = BASE_BINDINGS | BindingFlags.InvokeMethod;
   protected const BindingFlags GET_PROPERTY_BINDINGS = BASE_BINDINGS | BindingFlags.GetProperty;
   protected const BindingFlags SET_PROPERTY_BINDINGS = BASE_BINDINGS | BindingFlags.SetProperty;
   protected const BindingFlags GET_FIELD_BINDINGS = BASE_BINDINGS | BindingFlags.GetField;
   protected const BindingFlags SET_FIELD_BINDINGS = BASE_BINDINGS | BindingFlags.SetField;

   public static Result<Invoker> From(object obj) =>
      from nonNull in obj.Must().Not.BeNull().OrFailure()
      from type in nonNull.GetType().Success()
      select new Invoker(nonNull, type);

   protected object obj;
   protected Type type;

   protected Invoker(object obj, Type type)
   {
      this.obj = obj;
      this.type = type;
   }

   public Invoker(object obj)
   {
      this.obj = obj;
      type = obj.GetType();
   }

   protected Optional<object> invokeMember(string name, BindingFlags bindings, object[] args)
   {
      try
      {
         var result = type.InvokeMember(name, bindings, null, obj, args);
         if (result is not null)
         {
            return result;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public Optional<T> Invoke<T>(string name, params object[] args) where T : notnull
   {
      try
      {
         var _result = invokeMember(name, METHOD_BINDINGS, args);
         if (_result is (true, var result))
         {
            return (T)result;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public void Invoke(string name, params object[] args) => invokeMember(name, METHOD_BINDINGS, args);

   public Optional<T> GetProperty<T>(string name, params object[] args) where T : class
   {
      try
      {
         var _result = invokeMember(name, GET_PROPERTY_BINDINGS, args);
         if (_result is (true, var result))
         {
            return (T)result;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public void SetProperty(string name, params object[] args) => invokeMember(name, SET_PROPERTY_BINDINGS, args);

   public Optional<T> GetField<T>(string name, params object[] args) where T : class
   {
      try
      {
         var _result = invokeMember(name, GET_FIELD_BINDINGS, args);
         if (_result is (true, var result))
         {
            return (T)result;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   public void SetField(string name, params object[] args) => invokeMember(name, SET_FIELD_BINDINGS, args);
}