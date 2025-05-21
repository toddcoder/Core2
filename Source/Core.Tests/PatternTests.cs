using Core.Assertions;
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

   [TestMethod]
   public void WithOptionsTest()
   {
      Pattern pattern = "/('foo' | 'bar'); f";
      pattern = pattern.WithIgnoreCase(true);
      if (pattern.MatchedBy("Foo"))
      {
         Console.WriteLine("matches");
      }
      else
      {
         Console.WriteLine("not matches");
      }
   }
}