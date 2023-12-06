using System;
using System.Linq;
using Core.Collections;
using Core.Enumerables;

namespace Core.Exceptions;

public class HashExceptions : Exception, IHash<string, Exception>
{
   protected StringHash<Exception> exceptions;

   public HashExceptions(bool ignoreCase)
   {
      exceptions = [];
      exceptions = exceptions.CaseIgnore(ignoreCase);
   }

   public Exception this[string key]
   {
      get => exceptions[key];
      set => exceptions[key] = value;
   }

   public bool ContainsKey(string key) => exceptions.ContainsKey(key);

   public Hash<string, Exception> GetHash() => exceptions;

   public HashInterfaceMaybe<string, Exception> Items => exceptions.Items;

   public int Count => exceptions.Count;

   public void Clear() => exceptions.Clear();

   public override string Message => exceptions.Select(i => $"{i.Key}: {i.Value.Message}").ToString("; ");
}