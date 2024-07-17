using System.Collections.Generic;
using static Core.Collections.CollectionFunctions;

namespace Core.Collections;

public class StringHash<TValue> : Hash<string, TValue> where TValue : notnull
{
   public static StringHash<TValue> operator +(StringHash<TValue> hash, (string key, TValue value) tuple)
   {
      hash[tuple.key] = tuple.value;
      return hash;
   }

   protected bool ignoreCase;

   protected StringHash(bool ignoreCase) : base(stringComparer(ignoreCase))
   {
      this.ignoreCase = ignoreCase;
   }

   public StringHash() : base(stringComparer(false))
   {
      ignoreCase = true;
   }

   protected StringHash(bool ignoreCase, int capacity) : base(capacity, stringComparer(ignoreCase))
   {
      this.ignoreCase = ignoreCase;
   }

   public StringHash(int capacity) : base(capacity, stringComparer(false))
   {
      ignoreCase = true;
   }

   protected StringHash(bool ignoreCase, IDictionary<string, TValue> dictionary) : base(dictionary, stringComparer(ignoreCase))
   {
      this.ignoreCase = ignoreCase;
   }

   public StringHash(IDictionary<string, TValue> dictionary) : base(dictionary, stringComparer(false))
   {
      ignoreCase = true;
   }

   public bool IgnoreCase
   {
      get => ignoreCase;
      set => ignoreCase = value;
   }

   public Hash<string, TValue> AsHash => this;

   public StringHash<TValue> CaseAware() => new(false, this);

   public StringHash<TValue> CaseIgnore(bool ignoreCase) => new(ignoreCase, this);
}

public class StringHash : StringHash<string>
{
   public static StringHash operator +(StringHash hash, (string key, string value) tuple)
   {
      hash[tuple.key] = tuple.value;
      return hash;
   }

   protected StringHash(bool ignoreCase) : base(ignoreCase)
   {
   }

   public StringHash()
   {
      ignoreCase = true;
   }

   protected StringHash(bool ignoreCase, int capacity) : base(ignoreCase, capacity)
   {
   }

   public StringHash(int capacity) : base(capacity)
   {
   }

   protected StringHash(bool ignoreCase, IDictionary<string, string> dictionary) : base(ignoreCase, dictionary)
   {
   }

   public StringHash(IDictionary<string, string> dictionary) : base(dictionary)
   {
   }

   public new StringHash CaseAware() => new(false, this);

   public new StringHash CaseIgnore(bool ignored) => new(ignored, this);
}