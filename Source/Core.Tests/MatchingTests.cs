﻿using Core.Assertions;
using Core.Collections;
using Core.Enumerables;
using Core.Matching;
using Core.Objects;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Matching.MultiMatching.MultiMatchingFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Tests;

[TestClass]
public class MatchingTests
{
   [TestMethod]
   public void MatcherTest()
   {
      var _result = "tsqlcop.sql.format.options.xml".Matches("(sql); u");
      if (_result is (true, var result))
      {
         foreach (var match in result)
         {
            match.FirstGroup = "style";
         }

         Console.WriteLine(result);
         result.ToString().Must().Equal("tstylecop.style.format.options.xml").OrThrow();
      }
   }

   [TestMethod]
   public void MatchOnlySubstitutions()
   {
      var result = "This is the full sentence with sql1 in it".Substitute("'sql' /(/d); f", "sql-$1");
      Console.WriteLine(result);
      result.Must().Equal("This is the full sentence with sql-1 in it").OrThrow();
   }

   [TestMethod]
   public void QuoteTest()
   {
      var pattern = "`quote /(-[`quote]+) `quote; f";
      var _result = "\"Fee fi fo fum\" said the giant.".Matches(pattern);
      if (_result is (true, var result))
      {
         Console.WriteLine(result.FirstGroup.Guillemetify());
      }
   }

   [TestMethod]
   public void ScraperTest()
   {
      static string getVariables(Hash<string, string> hash, string prefix)
      {
         var keys = hash.Keys
            .Where(k => k.StartsWith(prefix))
            .Select(k => (key: k, index: k.DropUntil(":") + 1))
            .OrderBy(t => t.index)
            .Select(t => t.key);
         return hash.ValuesFromKeys(keys).ToString(", ");
      }

      var scraper = new Scraper("foo(a, b, c)\r\nbar(x,y , z); f");
      var index1 = 0;
      var index2 = 0;
      var _scraper =
         from name1 in scraper.Match("^ /(/w+) '('; f", "name1")
         from pushed1 in name1.Push("^ -[')']+; f")
         from split1 in pushed1.Split("/s* ',' /s*; f", s => $"var0_{s}:{index1++}")
         from popped1 in split1.Pop()
         from skipped1 in popped1.Skip(1)
         from skippedCrLf in skipped1.Skip("^ (/r /n)+; f")
         from name2 in scraper.Match("^ /(/w+) '('; f", "name2")
         from pushed2 in name2.Push("^ -[')']+; f")
         from split2 in pushed2.Split("/s* ',' /s*; f", s => $"var1_{s}:{index2++}")
         from popped2 in split2.Pop()
         select popped2;
      if (_scraper.Map(s => s.GetHash()) is (true, var hash))
      {
         var func1 = $"{hash["name1"]}({getVariables(hash, "var0_")})";
         var func2 = $"{hash["name2"]}({getVariables(hash, "var1_")})";
         Console.WriteLine(func1);
         Console.WriteLine(func2);
      }
      else if (_scraper.Exception is (true, var exception))
      {
         Console.WriteLine($"Exception: {exception.Message}");
      }
      else
      {
         Console.WriteLine("Not matched");
      }
   }

   [TestMethod]
   public void RetainTest()
   {
      var source = "~foobar-foo?baz-boo!boo-yogi";
      var retained = source.Retain("[/w '-']; f");
      Console.WriteLine(retained);
   }

   [TestMethod]
   public void ScrubTest()
   {
      var source = "~foobar-foo?baz-boo!boo-yogi";
      var retained = source.Scrub("[/w '-']; f");
      Console.WriteLine(retained);
   }

   [TestMethod]
   public void MultiMatcherTest()
   {
      var match = match<string>()
         & "^ 'foobaz' $; f" & (_ => "1. Foobaz")
         & "^ 'foo' /(.3) $; f" & (r => $"2. Foo{r.FirstGroup}")
         & (_ => "3. No match");
      string[] list = ["foobar", "foobaz", "???"];
      foreach (var input in list)
      {
         var _text = match.Matches(input);
         if (_text)
         {
            Console.WriteLine($"This was the match result: {_text}");
         }
         else if (_text.Exception is (true, var exception))
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }
         else
         {
            Console.WriteLine("Default happened");
         }
      }

      Pattern pattern1 = "^ 'foobaz' $; f";

      foreach (var input in list)
      {
         if (pattern1 & input)
         {
            Console.WriteLine("1. Foobaz");
         }
         else if (((Pattern)"^ 'foo' /(.3) $; f" & input) is (true, var result))
         {
            Console.WriteLine($"2. Foo{result.FirstGroup}");
         }
         else
         {
            Console.WriteLine("3. No match");
         }
      }
   }

   [TestMethod]
   public void MultiMatcherActionTest()
   {
      var matched = match()
         & "^ 'foobaz' $; f" & (_ => Console.WriteLine("1. Foobaz"))
         & "^ 'foo' /(.3) $; f" & (r => Console.WriteLine($"2. Foo{r.FifthGroup}"))
         & (_ => Console.WriteLine("3. No match"));
      string[] list = ["foobar", "foobaz", "???"];
      foreach (var input in list)
      {
         matched.Matches(input);
      }
   }

   [TestMethod]
   public void LeadingMatchesTest()
   {
      var input = "111 apples, 123 books, 153 chairs";
      var _result = input.Matches("['a-z']+");
      if (_result is (true, var result))
      {
         foreach (var item in result.Matches.LeadingMatches(input, true))
         {
            switch (item)
            {
               case ((true, var slice), _):
                  Console.WriteLine($"Leading: <{slice.Text}>");
                  break;
               case (_, (true, var match)):
                  Console.WriteLine($"Match: <{match.Text}>");
                  break;
            }
         }
      }
   }

   [TestMethod]
   public void ReplacerTest()
   {
      var source = "1.2.3 4.5.6";
      var replacer = new Replacer("/(/d+) '.' /(/d+) '.' /(/d+) /s*; f");
      var _replaced = replacer.Replace(source, (m, g, s) =>
      {
         Console.WriteLine($"{m}, {g}");
         if (m != 1 || g != 2)
         {
            return s.RightJustify(10, '0');
         }
         else
         {
            return nil;
         }
      });

      if (_replaced is (true, var replaced))
      {
         Console.WriteLine(replaced);
      }
      else if (_replaced.Exception is (true, var exception))
      {
         Console.WriteLine(exception.Message);
      }
      else
      {
         Console.WriteLine("Not matched");
      }
   }

   [TestMethod]
   public void ReplacerGroupsTest()
   {
      var replacer = new Replacer("/(/d+) '.' /(/d+) '.' /(/d+); f");

      var _replaced = replacer.ReplaceAllGroups("65.66.67", h =>
      {
         h["one"] = h["one"].Maybe().Int32().Map(i => (char)i).Map(c => c.ToString()) | "?";
         h["two"] = h["two"].Succ();
         h["three"] = h["three"].PadLeft(10, '0');
      }, "one", "two", "three");

      if (_replaced is (true, var replaced))
      {
         Console.WriteLine(replaced);
      }
      else if (_replaced.Exception is (true, var exception))
      {
         Console.WriteLine(exception.Message);
      }
      else
      {
         Console.WriteLine("Not matched");
      }
   }
}