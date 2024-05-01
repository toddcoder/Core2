using System;
using System.Collections.Generic;
using static Core.Collections.CollectionFunctions;

namespace Core.Collections;

public class AutoStringHash<TValue> : AutoHash<string, TValue> where TValue : notnull
{
   public static implicit operator StringHash<TValue>(AutoStringHash<TValue> autoStringHash)
   {
      StringHash<TValue> stringHash = [];
      stringHash = stringHash.CaseIgnore(autoStringHash.IgnoreCase);

      foreach (var (key, value) in autoStringHash)
      {
         stringHash[key] = value;
      }

      return stringHash;
   }

   protected AutoStringHash(bool ignoreCase) : base(stringComparer(ignoreCase))
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash()
   {
      IgnoreCase = true;
   }

   protected AutoStringHash(bool ignoreCase, int capacity) : base(capacity, stringComparer(ignoreCase))
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash(int capacity) : this(true, capacity)
   {
   }

   protected AutoStringHash(bool ignoreCase, IDictionary<string, TValue> dictionary) : base(dictionary, stringComparer(ignoreCase))
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash(IDictionary<string, TValue> dictionary) : this(true, dictionary)
   {
   }

   protected AutoStringHash(bool ignoreCase, Func<string, TValue> defaultLambda, bool autoAddDefault = false) :
      base(defaultLambda, stringComparer(ignoreCase), autoAddDefault)
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash(Func<string, TValue> defaultLambda, bool autoAddDefault = false) : this(true, defaultLambda, autoAddDefault)
   {
   }

   protected AutoStringHash(bool ignoreCase, TValue defaultValue, bool autoAddDefault = false) :
      base(defaultValue, autoAddDefault, stringComparer(ignoreCase))
   {
      IgnoreCase = ignoreCase;
   }

   public AutoStringHash(TValue defaultValue, bool autoAddDefault = false) : this(true, defaultValue, autoAddDefault)
   {
   }

   public bool IgnoreCase { get; }

   public AutoStringHash<TValue> CaseAware()
   {
      var hash = new AutoStringHash<TValue>(false, this);
      if (_defaultValue is (true, var defaultValue))
      {
         hash.DefaultValue = defaultValue;
      }
      else if (_defaultLambda is (true, var defaultLambda))
      {
         hash.DefaultLambda = defaultLambda;
      }

      return hash;
   }
}

public class AutoStringHash : AutoStringHash<string>
{
   public static implicit operator StringHash(AutoStringHash autoStringHash)
   {
      StringHash stringHash = [];
      if (!autoStringHash.IgnoreCase)
      {
         stringHash = stringHash.CaseAware();
      }

      foreach (var (key, value) in autoStringHash)
      {
         stringHash[key] = value;
      }

      return stringHash;
   }

   protected AutoStringHash(bool ignoreCase) : base(ignoreCase)
   {
   }

   public AutoStringHash() : this(true)
   {
   }

   protected AutoStringHash(bool ignoreCase, int capacity) : base(ignoreCase, capacity)
   {
   }

   public AutoStringHash(int capacity) : this(true, capacity)
   {
   }

   protected AutoStringHash(bool ignoreCase, IDictionary<string, string> dictionary) : base(ignoreCase, dictionary)
   {
   }

   public AutoStringHash(IDictionary<string, string> dictionary) : this(true, dictionary)
   {
   }

   protected AutoStringHash(bool ignoreCase, Func<string, string> defaultLambda, bool autoAddDefault = false) : base(ignoreCase, defaultLambda,
      autoAddDefault)
   {
   }

   public AutoStringHash(Func<string, string> defaultLambda, bool autoAddDefault = false) : this(true, defaultLambda, autoAddDefault)
   {
   }

   protected AutoStringHash(bool ignoreCase, string defaultValue, bool autoAddDefault = false) : base(ignoreCase, defaultValue, autoAddDefault)
   {
   }

   public AutoStringHash(string defaultValue, bool autoAddDefault = false) : this(true, defaultValue, autoAddDefault)
   {
   }

   public new AutoStringHash CaseAware()
   {
      var hash = new AutoStringHash(false, this);
      if (_defaultValue is (true, var defaultValue))
      {
         hash.DefaultValue = defaultValue;
      }
      else if (_defaultLambda is (true, var defaultLambda))
      {
         hash.DefaultLambda = defaultLambda;
      }

      return hash;
   }
}