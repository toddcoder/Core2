﻿using Core.Assertions;
using Core.Matching;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Strings;

namespace Core.Tests;

[TestClass]
public class PatternTests
{
   protected static void matcherTest(Pattern pattern)
   {
      var _result = "tsqlcop.sql.format.options.xml".Matches(pattern);
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
   public void UMatcherTest()
   {
      matcherTest("(sql); u");
   }

   [TestMethod]
   public void FMatcherTest()
   {
      matcherTest("/('sql'); f");
   }

   [TestMethod]
   public void DualMatcherTest()
   {
      matcherTest("(sql); u");
      matcherTest("/('sql'); f");
   }

   protected static void matchOnlySubstitutions(Pattern pattern)
   {
      var result = "This is the full sentence with sql1 in it".Substitute(pattern, "sql-$1");
      Console.WriteLine(result);
      result.Must().Equal("This is the full sentence with sql-1 in it").OrThrow();
   }

   [TestMethod]
   public void UMatchOnlySubstitutionsTest()
   {
      Pattern.IsFriendly = false;
      matchOnlySubstitutions(@"sql(\d+); u");
   }

   [TestMethod]
   public void FMatchOnlySubstitutionsTest()
   {
      matchOnlySubstitutions("'sql' /(/d+); f");
   }

   /*protected static void matchPatternsTest(Pattern pattern1, Pattern pattern2, Pattern pattern3)
   {
      var _result = "foobar(foo,baz)".Matches(pattern1);
      if (_result is (true, var result))
      {
         Console.Write(result.FirstMatch);
         var lastResult = result;
         var _result2 = result.MatchedBy(pattern2);
         while (_result2 is (true, var result2))
         {
            Console.Write(result2.FirstMatch);
            lastResult = result2;
         }

         if (_result2.AnyException is (true, var exception))
         {
            Console.WriteLine($"Exception: {exception.Message}");
         }

         var _lastResult = lastResult.MatchedBy(pattern3);
         if (_lastResult is (true, var result3))
         {
            Console.WriteLine(result3.FirstMatch);
         }
      }
   }

   [TestMethod]
   public void UMatchPatternsTest()
   {
      matchPatternsTest(@"^\w+\(; u", @"\w+,; u", @"\w+\)l; u");
   }

   [TestMethod]
   public void FMatchPatternsTest()
   {
      matchPatternsTest("^ /w+ '('; f", "/w+ ','; f", "/w+ ')'; f");
   }*/

   [TestMethod]
   public void FQuoteTest()
   {
      Pattern pattern = "`quote /(-[`quote]+) `quote; f";
      var _result = "\"Fee fi fo fum\" said the giant.".Matches(pattern);
      if (_result is (true, var result))
      {
         Console.WriteLine(result.FirstGroup.Guillemetify());
      }
   }

   [TestMethod]
   public void RetainTest()
   {
      Pattern.IsFriendly = true;

      var source = "~foobar-foo?baz-boo!boo-yogi";
      var retained = source.Retain("[/w '-']; f");
      Console.WriteLine(retained);
   }

   [TestMethod]
   public void ScrubTest()
   {
      var source = "~foobar-foo?baz-boo!boo-yogi";
      var scrubbed = source.Scrub("[/w '-']; f");
      Console.WriteLine(scrubbed);
   }

   [TestMethod]
   public void BugTest()
   {
      var pattern = @"^ '\)'; f";
      var input = @"\)";
      if (input.Matches(pattern))
      {
         Console.WriteLine("Matched");
      }
      else
      {
         Console.WriteLine("Not matched");
      }
   }

   [TestMethod]
   public void MatchTextTest()
   {
      var input = "This is a test I'm testing";
      var _result = input.Matches("/b 'test' /w*; f");
      if (_result is (true, var result))
      {
         foreach (var match in result)
         {
            match.Text = $"<{match.Text}>";
         }

         Console.WriteLine(result);
      }
   }

   [TestMethod]
   public void UnfriendlyPatternExtractionTest()
   {
      Pattern pattern = "('foo' | 'calc'); f";
      var regex = pattern.Regex;
      Console.WriteLine(regex);
   }
}