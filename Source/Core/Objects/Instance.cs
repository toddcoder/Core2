﻿using System;
using System.Reflection;
using Core.Monads;
using Core.Strings;
using static Core.Monads.AttemptFunctions;

namespace Core.Objects;

public static class Instance
{
   public static T Create<T>() => Activator.CreateInstance<T>();

   public static Result<T> TryCreate<T>() where T : notnull => tryTo(Create<T>);

   public static T Create<T>(params object[] args) => (T)typeof(T).Create(args)!;

   public static Result<T> TryCreate<T>(params object[] args) where T : notnull => tryTo(() => Create<T>(args));

   public static object? Create(this Type type) => Activator.CreateInstance(type);

   public static Result<object> TryCreate(this Type type) => tryTo(() => Create(type)!);

   public static Result<T> TryCreate<T>(this Type type) where T : notnull, new()
   {
      return
         from obj in tryTo(() => type.Create()!)
         from cast in obj.Result().Cast<T>()
         select cast;
   }

   public static object? Create(this Type type, params object[] args)
   {
      if (type.IsPrimitive || type.FullName == "System.DateTime")
      {
         args[0] = args[0].ToNonNullString();
         return type.InvokeMember("Parse", BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public,
            null, null, args, null, null, null);
      }
      else
      {
         switch (type.FullName)
         {
            case "System.String":
               args[0] = args[0].ToNonNullString().ToCharArray();
               break;
            case "System.DBNull":
            case "System.Empty":
               return type.InvokeMember("Value", BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public,
                  null, null, null, null, null, null);
         }

         return Activator.CreateInstance(type, BindingFlags.CreateInstance, null, args, null);
      }
   }

   public static Result<object> TryCreate(this Type type, params object[] args) => tryTo(() => Create(type, args)!);

   public static Result<T> TryCreate<T>(this Type type, params object[] args) where T : notnull
   {
      return
         from obj in tryTo(() => type.Create(args)!)
         from cast in obj.Result().Cast<T>()
         select cast;
   }

   public static object? Create(this string typeName) => Type.GetType(typeName, true, true)?.Create();

   public static Result<object> TryCreate(this string typeName) => tryTo(() => Create(typeName)!);

   public static Result<T> TryCreate<T>(this string typeName) where T : notnull, new()
   {
      return
         from obj in tryTo(() => typeName.Create()!)
         from cast in obj.Result().Cast<T>()
         select cast;
   }

   public static object? Create(this string typeName, params object[] args)
   {
      return Type.GetType(typeName, true, true)?.Create(args);
   }

   public static Result<object> TryCreate(this string typeName, params object[] args) => tryTo(() => Create(typeName, args)!);

   public static Result<T> TryCreate<T>(this string typeName, params object[] args) where T : notnull
   {
      return
         from obj in tryTo(() => typeName.Create(args)!)
         from cast in obj.Result().Cast<T>()
         select cast;
   }
}