﻿using System;
using System.Collections.Generic;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Collections;

public class StringVariants : IHash<string, string>
{
   public class KeyValue : IEquatable<KeyValue>
   {
      public KeyValue(string key, string value)
      {
         Key = key;
         Value = value;
      }

      public string Key { get; }

      public string Value { get; }

      public bool Equals(KeyValue? other) => other is not null && Key == other.Key && Value == other.Value;

      public override bool Equals(object? obj) => obj is KeyValue other && Equals(other);

      public override int GetHashCode() => hashCode() + Key + Value;
   }

   protected StringHash templates;
   protected StringHash<KeyValue> keyValues;
   protected Maybe<string> _templateName;

   public StringVariants()
   {
      templates = [];
      keyValues = [];
      _templateName = nil;
   }

   public StringVariants(Dictionary<string, string> dictionary) : this()
   {
      foreach (var item in dictionary)
      {
         this[item.Key] = item.Value;
      }
   }

   public string this[string key]
   {
      get => templates[key];
      set => templates[key] = value;
   }

   public bool ContainsKey(string key) => templates.ContainsKey(key);

   public Hash<string, string> GetHash() => templates;

   public HashInterfaceMaybe<string, string> Items => new(this);

   public StringVariants Alias(string alias, string key, string value)
   {
      keyValues[alias] = new KeyValue(key, value);
      return this;
   }

   public StringVariants Alias(string key, string value) => Alias($"{key}.{value}", key, value);

   public StringVariants TemplateName(string templateName)
   {
      _templateName = templateName;
      return this;
   }

   public Maybe<string> Evaluate(params string[] aliases)
   {
      var _template =
         from templateName in _templateName
         from mappedTemplate in Items[templateName]
         select mappedTemplate;
      if (_template is (true, var template))
      {
         var formatter = new Formatter();
         foreach (var alias in aliases)
         {
            var _keyValue = keyValues.Maybe[alias];
            if (_keyValue is (true, var keyValue))
            {
               formatter[keyValue.Key] = keyValue.Value;
            }
         }

         return formatter.Format(template);
      }
      else
      {
         return nil;
      }
   }
}