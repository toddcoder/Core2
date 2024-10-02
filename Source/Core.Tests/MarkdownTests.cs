using Core.Markup.Markdown;
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

      tagged = MarkdownWriter.Tagify("span!blue>This is blue!");
      Console.WriteLine(tagged);

      tagged = MarkdownWriter.Tagify("span!text-align: right>This is blue!");
      Console.WriteLine(tagged);
   }

   [TestMethod]
   public void ClassRefTest()
   {
      var writer = new MarkdownWriter();
      writer.WriteTextLine("Word", ".fp");
      Console.WriteLine(writer);
   }
}