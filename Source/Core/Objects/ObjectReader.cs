using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using static System.Reflection.BindingFlags;
using static System.Reflection.MemberTypes;

namespace Core.Objects;

public class ObjectReader
{
   public static Result<ObjectReader> ReadObject(object obj) =>
      from nonNull in obj.Must().Not.BeNull().OrFailure()
      from type in obj.GetType().Success()
      from values in getValues(obj, type)
      select new ObjectReader(values);

   protected static Result<Hash<string, object>> getValues(object obj, Type type) =>
      from members in getMembers(type)
      from values in getValues(obj, members)
      select values;

   protected static Result<IEnumerable<MemberInfo>> getMembers(Type type)
   {
      const MemberTypes memberTypes = Field | Property;
      const BindingFlags bindingFlags = BindingFlags.Instance | GetField | GetProperty | NonPublic | Public;

      return
         from members in type.GetMembers(bindingFlags).Where(member => (member.MemberType & memberTypes) != 0).Success()
         select members;
   }

   protected static Result<object> getValue(object obj, MemberInfo memberInfo)
   {
      return memberInfo switch
      {
         FieldInfo fieldInfo => fieldInfo.GetValue(obj)!,
         PropertyInfo propertyInfo => propertyInfo.GetValue(obj)!,
         _ => MonadFunctions.fail($"{memberInfo.Name} is neither a field nor a property")
      };
   }

   protected static Result<Hash<string, object>> getValues(object obj, IEnumerable<MemberInfo> memberInfos)
   {
      Hash<string, object> hash = [];

      foreach (var info in memberInfos)
      {
         var _value = getValue(obj, info);
         if (_value is (true, var value))
         {
            hash[info.Name] = value;
         }
         else
         {
            return _value.Exception;
         }
      }

      return hash;
   }

   protected Hash<string, object> values;

   protected ObjectReader(Hash<string, object> values) => this.values = values;

   protected TResult invoke<TResult>(LambdaExpression expression) where TResult : notnull
   {
      object[] arguments =
      [
         .. expression.Parameters
            .Select(p => p.Name)
            .Where(name => values.ContainsKey(name!))
            .Select(name => values[name!])
      ];

      return (TResult)expression.Compile().DynamicInvoke(arguments)!;
   }

   protected void _do(LambdaExpression expression)
   {
      object[] arguments =
      [
         .. expression.Parameters
            .Select(p => p.Name)
            .Where(name => values.ContainsKey(name!))
            .Select(name => values[name!])
      ];

      expression.Compile().DynamicInvoke(arguments);
   }

   public TResult Invoke<T, TResult>(Expression<Func<T, TResult>> expression) where TResult : notnull => invoke<TResult>(expression);

   public TResult Invoke<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression) where TResult : notnull
   {
      return invoke<TResult>(expression);
   }

   public TResult Invoke<T1, T2, T3, TResult>(Expression<Func<T1, T2, T3, TResult>> expression) where TResult : notnull
   {
      return invoke<TResult>(expression);
   }

   public TResult Invoke<T1, T2, T3, T4, TResult>(Expression<Func<T1, T2, T3, T4, TResult>> expression) where TResult : notnull
   {
      return invoke<TResult>(expression);
   }

   public TResult Invoke<T1, T2, T3, T4, T5, TResult>(Expression<Func<T1, T2, T3, T4, T5, TResult>> expression) where TResult : notnull
   {
      return invoke<TResult>(expression);
   }

   public TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression) where TResult : notnull
   {
      return invoke<TResult>(expression);
   }

   public void Do<T>(Expression<Action<T>> expression) => _do(expression);

   public void Do<T1, T2>(Expression<Action<T1, T2>> expression) => _do(expression);

   public void Do<T1, T2, T3>(Expression<Action<T1, T2, T3>> expression) => _do(expression);

   public void Do<T1, T2, T3, T4>(Expression<Action<T1, T2, T3, T4>> expression) => _do(expression);

   public void Do<T1, T2, T3, T4, T5>(Expression<Action<T1, T2, T3, T4, T5>> expression) => _do(expression);

   public void Do<T1, T2, T3, T4, T5, T6>(Expression<Action<T1, T2, T3, T4, T5, T6>> expression) => _do(expression);

   public T Assign<T>(string name) => (T)values[name];

   public ObjectReaderTrying TryTo => new(this);
}