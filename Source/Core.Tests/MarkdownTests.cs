using Core.Markup.Markdown;

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
      writer.TextLine("Word", ".fp");
      Console.WriteLine(writer);
   }

   [TestMethod]
   public void MarkdownBuilderTest()
   {
      var builder = new MarkdownBuilder();
      builder
         .Header("Job 1", 1)
         .CheckList("Alpha", true)
         .CheckList("Bravo", false)
         .CheckList("Charlie", true)
         .Quote("note 1")
         .Quote("note 2");
      Console.WriteLine(builder);
   }

   public void MarkdownFrameTest()
   {

   }
}