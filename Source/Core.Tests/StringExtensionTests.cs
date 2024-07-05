using Core.Assertions;
using Core.Enumerables;
using Core.Matching;
using Core.Numbers;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class StringExtensionTests
{
   [TestMethod]
   public void PluralTest()
   {
      var message = "There (is,are) # book(s)";
      Console.WriteLine(1.Plural(message));
      Console.WriteLine(2.Plural(message));

      message = "child(ren)";
      Console.WriteLine(1.Plural(message));
      Console.WriteLine(2.Plural(message));

      message = @"\#G(OO,EE)SE";
      Console.WriteLine(1.Plural(message));
      Console.WriteLine(2.Plural(message));
   }

   protected static void test(string name, string camelResult, string pascalResult)
   {
      var camel = name.ToCamel();
      var pascal = name.ToPascal();

      Console.WriteLine($"camel:  {camel}");
      Console.WriteLine($"pascal: {pascal}");
      camel.Must().Equal(camelResult).OrThrow();
      pascal.Must().Equal(pascalResult).OrThrow();
   }

   [TestMethod]
   public void CamelAndPascalCaseTest()
   {
      test("SetSQL_nameForUser_ID", "setSQLNameForUserId", "SetSQLNameForUserId");
      test("TARGET", "target", "Target");
      test("TARGET_REPORT", "targetReport", "TargetReport");
      test("C1", "c1", "C1");
      test("Internal Result", "internalResult", "InternalResult");
   }

   [TestMethod]
   public void EllipticalTest()
   {
      var text = "'We had joy, we had fun, we had seasons in the sun' -- the second most depressing song in the world";
      foreach (var limit in 80.DownTo(-10, -10))
      {
         Console.WriteLine(limit);
         Console.WriteLine(text.Elliptical(limit, ' '));
         Console.WriteLine();
      }

      Console.WriteLine();

      text = "Now Is The Time For All Good Men To Come To The Aid Of Their Party";
      foreach (var limit in 80.DownTo(-10, -10))
      {
         Console.WriteLine(limit);
         Console.WriteLine(text.Elliptical(limit, " ABCDEFGHIJKLMNOPQRSTUVWXYZ"));
         Console.WriteLine();
      }
   }

   [TestMethod]
   public void TruncateTest()
   {
      var text = "'We had joy, we had fun, we had seasons in the sun' -- the second most depressing song in the world";
      foreach (var limit in 80.DownTo(-10, -10))
      {
         Console.WriteLine(limit);
         Console.WriteLine(text.Truncate(limit));
         Console.WriteLine();
      }
   }

   [TestMethod]
   public void JustificationTest()
   {
      var text = "Apples and oranges";
      Console.WriteLine($"|{text.LeftJustify(80)}|");
      Console.WriteLine($"|{text.RightJustify(80)}|");
      Console.WriteLine($"|{text.Center(80)}|");
   }

   [TestMethod]
   public void NormalizeWhitespaceTest()
   {
      var text = "foobar  \t \r\n \tis okay";
      Console.WriteLine(text.NormalizeWhitespace());
   }

   [TestMethod]
   public void ExactlyTest()
   {
      var empty = string.Empty;
      var tooLittle = "the cat jumped over the dog";
      var tooMuch = $"{tooLittle}, but the dog didn't care.";
      var needsNormalization = "the cat\tthe dog\r\nThey need help.";

      Console.WriteLine(" 0000000001111111111222222222233333333334");
      Console.WriteLine(" 1234567890123456789012345678901234567890");
      Console.WriteLine($"|{empty.Exactly(40)}|");
      Console.WriteLine($"|{tooLittle.Exactly(40)}|");
      Console.WriteLine($"|{tooMuch.Exactly(40)}|");
      Console.WriteLine($"|{needsNormalization.Exactly(40)}|");
   }

   [TestMethod]
   public void LinesTest()
   {
      var text = "foobar\r\nfoobaz\rfoo\nbar";
      var lines = text.Lines();
      foreach (var line in lines)
      {
         Console.WriteLine(line);
      }
   }

   protected static void quoteString(string text, bool indent = true)
   {
      var indentation = indent ? "  " : "";
      Console.WriteLine($"{indentation}\"{text}\"");
   }

   protected void writeWords(string text)
   {
      quoteString(text, false);
      foreach (var word in text.Words())
      {
         quoteString(word);
      }

      Console.WriteLine("=".Repeat(80));
   }

   [TestMethod]
   public void WordsTest()
   {
      foreach (var text in (string[]) ["maryMaryContrary", "ExamplesRemarksConstructorsProperties", "alfa-bravo-charlie"])
      {
         writeWords(text);
      }

      writeWords("LEFT OUTER JOIN");
   }

   [TestMethod]
   public void SuccTest()
   {
      var text = "a";
      for (var i = 0; i < 20; i++)
      {
         Console.WriteLine(text);
         text = text.Succ();
      }
   }

   [TestMethod]
   public void FixTest()
   {
      var haystack = "alfa, beta, charlie";
      if (haystack.PrefixOf("alfa") is (true, var (result1, remainder1)))
      {
         Console.WriteLine($"<{result1}><{remainder1}>");
      }

      if (haystack.SuffixOf("charlie") is (true, var (remainder2, result2)))
      {
         Console.WriteLine($"<{remainder2}><{result2}>");
      }

      if (haystack.InfixOf("beta") is (true, var (left, result, right)))
      {
         Console.WriteLine($"<{left}><{result}><{right}>");
      }
   }

   [TestMethod]
   public void FixTrimTest()
   {
      var haystack = "alfa    beta     charlie";
      if (haystack.PrefixOf("alfa", trim: true) is (true, var (result1, remainder1)))
      {
         Console.WriteLine($"<{result1}><{remainder1}>");
      }

      if (haystack.SuffixOf("charlie") is (true, var (remainder2, result2)))
      {
         Console.WriteLine($"<{remainder2}><{result2}>");
      }

      if (haystack.InfixOf("beta", trimLeft: true, trimRight: true) is (true, var (left, result, right)))
      {
         Console.WriteLine($"<{left}><{result}><{right}>");
      }
   }

   [TestMethod]
   public void CamelTest()
   {
      string[] items =
      [
         " (fix) ", "Conflict-Release", "r-6.59.0-grp1", "none\u003CString\u003E", "fix-49380f1f-commit", "LEFT OUTER JOIN",
         "foobar\r\nfoobaz\rfoo\nbar", "In UAT"
      ];
      foreach (var item in items)
      {
         Console.WriteLine($"<{item}>\t<{item.ToCamel()}>");
      }
   }

   [TestMethod]
   public void EmptyUnjoinTest()
   {
      var array = "".Unjoin("/s* ',' /s*; f");
      Console.WriteLine(array.Select(i => $"'{i}'").ToString(", "));
   }

   [TestMethod]
   public void KeepCharacterTypeTest()
   {
      var source = "Test 1";
      var result = source.Keep(CharacterType.Letter);
      Console.WriteLine($"<{result}>");
   }

   [TestMethod]
   public void DropCharacterTypeTest()
   {
      var source = "Test 1!";
      var result = source.Drop(CharacterType.Letter).Drop(CharacterType.Whitespace).Drop(CharacterType.Numeric);
      Console.WriteLine($"<{result}>");
   }
}