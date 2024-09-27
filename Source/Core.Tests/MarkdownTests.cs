﻿using Core.Markup.Markdown;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class MarkdownTests
{
   [TestMethod]
   public void TagifyTest()
   {
      var tagged = MarkdownWriter.Tagify("p>foo.bold>bar.bold.p");
      Console.WriteLine(tagged);

      tagged = MarkdownWriter.Tagify("span?blue>This is blue!.span");
      Console.WriteLine(tagged);

      tagged = MarkdownWriter.Tagify("span!blue>This is blue!.span");
      Console.WriteLine(tagged);
   }
}