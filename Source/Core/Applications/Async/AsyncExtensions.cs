﻿using System;
using System.Threading.Tasks;
using Core.Monads;

namespace Core.Applications.Async;

public static class AsyncExtensions
{
   public static async Task<Completion<TResult>> Map<T, TResult>(this Task<Completion<T>> task, Func<T, TResult> func) where T : notnull
      where TResult : notnull
   {
      return await Task.Run(() => task.Result.Map(func));
   }

   public static async Task<Completion<TResult>> Map<T, TResult>(this Task<Completion<T>> task, Func<T, Completion<TResult>> func) where T : notnull
      where TResult : notnull
   {
      return await Task.Run(() => task.Result.Map(func));
   }
}