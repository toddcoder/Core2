﻿using System.Text;
using Core.Collections;
using Core.Matching;
using Core.Monads.Lazy;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.MonadFunctions;

namespace Core.Tests;

[TestClass]
public class StringClassTests
{
   [TestMethod]
   public void SlicerTest()
   {
      Slicer slicer = "Throw the old woman out!";

      Console.WriteLine(slicer[6, 13]);
      slicer[6, 13] = "Lucy";

      Console.WriteLine(slicer[0, 5]);
      slicer[0, 5] = "Toss";

      Console.WriteLine(slicer[23, 1]);
      slicer[23, 1] = "?!";
      Console.WriteLine(slicer);
   }

   [TestMethod]
   public void SlicedTest()
   {
      var source = "Throw the old woman out!";
      Slicer slicer = source;

      Console.WriteLine(slicer[6, 13]);
      slicer[6, 13] = "Lucy";

      Console.WriteLine(slicer[0, 5]);
      slicer[0, 5] = "Toss";

      Console.WriteLine(slicer[23, 1]);
      slicer[23, 1] = "?!";

      var builder = new StringBuilder(source);

      foreach (var (index, length, text) in slicer)
      {
         builder.Remove(index, length);
         builder.Insert(index, text);
      }

      Console.WriteLine(builder);
   }

   [TestMethod]
   public void ListStringTest()
   {
      var listString = new ListString("alpha", "; ", true) { Text = "bravo" };
      listString.Text = "charlie";
      Console.WriteLine(listString);
      listString.Text = "alpha";
      Console.WriteLine(listString);
   }

   [TestMethod]
   public void DestringifyAsSqlTest()
   {
      var source = "SELECT 'I can''t do this' from foobar --yes you can\r\nprint ''";
      var delimitedText = DelimitedText.AsSql();
      var parsed = delimitedText.Destringify(source);
      Console.WriteLine(parsed);
      Console.WriteLine(delimitedText.Restringify(parsed, RestringifyQuotes.SingleQuote));
   }

   [TestMethod]
   public void DestringifyAsSqlTest2()
   {
      Pattern.IsFriendly = false;
      var source = "UPDATE Foobar SET A = -A, B = 'This is a test' /*a test*/;";
      var delimitedText = DelimitedText.AsSql();
      var parsed = delimitedText.Destringify(source);
      Console.WriteLine(parsed);
      Console.WriteLine(delimitedText.Restringify(parsed, RestringifyQuotes.SingleQuote));

      var inOutside = new DelimitedText("'", "'", "''");
      foreach (var (text, _, _) in inOutside.Enumerable(source))
      {
         Console.WriteLine($"<{text}>");
      }
   }

   [TestMethod]
   public void FormatterTest()
   {
      var text = "Please refresh the DB on [{environment}] in preparation for {releaseType} {release} - deployment for testing";
      var formatter = new Formatter { ["environment"] = "estream01ua", ["releaseType"] = "Patch", ["release"] = "r-6.24.0" };
      var formatted = formatter.Format(text);
      Console.WriteLine(formatted);

      StringHash replacements = new() { ["environment"] = "estream01ua", ["releaseType"] = "Patch", ["release"] = "r-6.24.0" };
      formatter = new Formatter(replacements);
      formatted = formatter.Format(text);
      Console.WriteLine(formatted);

      text = "no replacements";
      formatted = formatter.Format(text);
      Console.WriteLine(formatted);
   }

   [TestMethod]
   public void FormatterSlashTest()
   {
      var formatter = new Formatter { ["remedy"] = "CRQ26075" };
      var text = "https://smartit.eprod.com/smartit/app/#/search/{remedy}";
      var formattedText = formatter.Format(text);
      Console.WriteLine(formattedText);

      text = "https://smartit.eprod.com/smartit/app/#/search//{remedy}";
      formattedText = formatter.Format(text);
      Console.WriteLine(formattedText);
   }

   [TestMethod]
   public void SourceLineTest()
   {
      var text = "alpha\rbravo\rcharlie\n\ndelta\r\necho";
      var source = new Source(text);
      LazyMaybe<string> _line = nil;
      while (_line.ValueOf(source.NextLine, true))
      {
         Console.WriteLine($"'{_line}'");
      }

      Console.WriteLine("---");
      var sourceLines = new SourceLines(text);
      foreach (var line in sourceLines.Lines())
      {
         Console.WriteLine($"'{line}'");
      }
   }

   [TestMethod]
   public void SourceLine2Test()
   {
      var text = ":alpha\n:bravo\n-charlie\n-delta\n-echo\nfoxtrot";
      var source = new Source(text);

      LazyMaybe<string> _line = nil;
      while (_line.ValueOf(source.NextLine("^ ':'"), true))
      {
         Console.WriteLine($": -> '{_line}'");
      }

      Console.WriteLine("========");

      while (_line.ValueOf(source.NextLine("^ '-'"), true))
      {
         Console.WriteLine($"- -> '{_line}'");
      }

      Console.WriteLine("========");

      while (_line.ValueOf(source.NextLine, true))
      {
         Console.WriteLine($"  -> '{_line}'");
      }

      Console.WriteLine("---");

      var sourceLines = new SourceLines(text);
      foreach (var line in sourceLines.While("^ ':'"))
      {
         Console.WriteLine($": -> '{line}'");
      }

      Console.WriteLine("========");

      foreach (var line in sourceLines.While("^ '-'"))
      {
         Console.WriteLine($"- -> '{line}'");
      }

      Console.WriteLine("========");

      foreach (var line in sourceLines.Lines())
      {
         Console.WriteLine($"  -> '{line}'");
      }
   }

   [TestMethod]
   public void SourceLine3Test()
   {
      var text = "[a -> alpha]This is line 1\n[b -> bravo]This is line 2\n[c -> charlie]This is line 3";
      Pattern pattern = "^ '[' /(/w) /s* '->' /s* /(/w+) ']'; f";
      var sourceLines = new SourceLines(text);
      foreach (var (result, line) in sourceLines.WhileMatches(pattern))
      {
         var (tag, value) = result;
         var remainder = line.Drop(result.Length);
         Console.WriteLine($"[ {tag} -> {value} ] {remainder}");
      }
   }

   [TestMethod]
   public void TableMakerTest()
   {
      var table = new TableMaker(("Name", Justification.Left), ("Value", Justification.Left))
      {
         Title = "Properties"
      };

      var obj = "Testing";

      table.Add("Value", obj);
      table.Add("Length", obj.Length);
      table.Add("Hash Code", obj.GetHashCode());

      Console.WriteLine(table);
   }

   [TestMethod]
   public void TableMaker2Test()
   {
      var table = new TableMaker(("Name", Justification.Left), ("Value", Justification.Left));

      var obj = "Testing";

      table.Add("Value", obj);
      table.Add("Length", obj.Length);
      table.Add("Hash Code", obj.GetHashCode());

      Console.WriteLine(table);
   }

   [TestMethod]
   public void StringVariantsTest()
   {
      var variants = new StringVariants
      {
         ["x"] = "This is X: {xValue}",
         ["y"] = "This is Y: {yValue} with {extra}",
         ["z"] = "This is Z: {xValue}"
      };
      variants
         .Alias("xx", "xValue", "X!")
         .Alias("yy1", "yValue", "Y!")
         .Alias("yy2", "extra", "Extra!")
         .Alias("zx", "xValue", "X!");
      LazyMaybe<string> _result = nil;
      if (_result.ValueOf(variants.TemplateName("x").Evaluate("xx"), true) is (true, var result1))
      {
         Console.WriteLine(result1);
      }

      if (_result.ValueOf(variants.TemplateName("y").Evaluate("yy1", "yy2"), true) is (true, var result2))
      {
         Console.WriteLine(result2);
      }

      if (_result.ValueOf(variants.TemplateName("z").Evaluate("zx"), true) is (true, var result3))
      {
         Console.WriteLine(result3);
      }
   }
}