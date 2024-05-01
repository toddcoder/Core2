﻿using System;
using System.Linq;
using Core.Collections;
using Core.DataStructures;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Matching;

public class Scraper : IHash<string, string>
{
   protected static MaybeStack<Scraper> scraperStack;

   static Scraper()
   {
      scraperStack = [];
   }

   protected Source source;
   protected StringHash variables;

   public Scraper(string source)
   {
      this.source = new Source(source);

      variables = [];
   }

   public Optional<Scraper> Match(Pattern pattern, params string[] names)
   {
      if (!source.More)
      {
         return nil;
      }

      var _result = pattern.MatchedBy(source.Current);
      if (_result is (true, var result))
      {
         string[] groups = [.. result.Groups(0).Skip(1)];
         var minLength = groups.Length.MinOf(names.Length);
         for (var i = 0; i < minLength; i++)
         {
            variables[names[i]] = groups[i];
         }

         source.Advance(result.Length);
         return this;
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public Optional<Scraper> Match(Pattern pattern, Func<string, string> nameFunc)
   {
      if (!source.More)
      {
         return nil;
      }

      var _result = pattern.MatchedBy(source.Current);
      if (_result is (true, var result))
      {
         foreach (var group in result.Groups(0).Skip(1))
         {
            var name = nameFunc(group);
            variables[name] = group;
         }

         source.Advance(result.Length);
         return this;
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public Optional<Scraper> Split(Pattern pattern, Func<string, string> nameFunc)
   {
      if (!source.More)
      {
         return nil;
      }

      var split = source.Current.Unjoin(pattern);
      foreach (var value in split)
      {
         variables[nameFunc(value)] = value;
      }

      source.Advance(source.Current.Length);
      return this;
   }

   public Optional<Scraper> Skip(Pattern pattern)
   {
      if (!source.More)
      {
         return nil;
      }

      var _result = pattern.MatchedBy(source.Current);
      if (_result is (true, var result))
      {
         source.Advance(result.Length);
         return this;
      }
      else if (_result.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public Optional<Scraper> Skip(int count)
   {
      if (!source.More)
      {
         return nil;
      }

      source.Advance(count);
      return this;
   }

   public string this[string key] => variables[key];

   public bool ContainsKey(string key) => variables.ContainsKey(key);

   public Hash<string, string> GetHash() => variables;

   public HashInterfaceMaybe<string, string> Items => new(this);

   public Optional<Scraper> Push(Pattern pattern)
   {
      var _length = pattern.MatchedBy(source.Current).Map(r => r.Length);
      if (_length)
      {
         var scraper = new Scraper(source.Current.Keep(_length));
         source.Advance(_length);
         scraperStack.Push(this);

         return scraper;
      }
      else if (_length.Exception is (true, var exception))
      {
         return exception;
      }
      else
      {
         return nil;
      }
   }

   public Optional<Scraper> Pop()
   {
      var _scraper = scraperStack.Pop();
      if (_scraper is (true, var scraper))
      {
         var scraperVariables = scraper.GetHash();
         var stringHash = scraperVariables.ToStringHash();
         foreach (var (variable, value) in stringHash)
         {
            scraperVariables[variable] = value;
         }

         return scraper;
      }
      else
      {
         return fail("Scraper stack empty");
      }
   }
}