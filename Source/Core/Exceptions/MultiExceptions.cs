﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Monads;

namespace Core.Exceptions;

public class MultiExceptions : Exception, IList<Exception>
{
   protected List<Exception> exceptions;

   public MultiExceptions() => exceptions = [];

   public void Add(Exception exception) => exceptions.Add(exception);

   public void Add(string exceptionMessage) => Add(new ApplicationException(exceptionMessage));

   public void Add<T>(Result<T> result) where T : notnull
   {
      if (!result)
      {
         Add(result.Exception);
      }
   }

   public void AddRange(IEnumerable<Exception> exceptions) => this.exceptions.AddRange(exceptions);

   public IEnumerator<Exception> GetEnumerator() => exceptions.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public bool Remove(Exception item) => exceptions.Remove(item);

   public int Count => exceptions.Count;

   public bool IsReadOnly => false;

   public int IndexOf(Exception item) => exceptions.IndexOf(item);

   public void Insert(int index, Exception item) => exceptions.Insert(index, item);

   public void RemoveAt(int index) => exceptions.RemoveAt(index);

   public Exception this[int index]
   {
      get => exceptions[index];
      set => exceptions[index] = value;
   }

   public void Clear() => exceptions.Clear();

   public bool Contains(Exception item) => exceptions.Contains(item);

   public void CopyTo(Exception[] array, int arrayIndex) => exceptions.CopyTo(array, arrayIndex);

   public override string Message => exceptions.Select(e => e.Message).ToString("\r\n");

   public IEnumerable<TException> Exceptions<TException>() where TException : Exception
   {
      return [.. exceptions.Select(e => (TException)e)];
   }
}