using Core.Collections;
using Core.Markdown;

namespace Core.WinForms.Tests;

public class MarkdownFrameTestOptions(string source, bool tidy, StringHash scalarReplacements, StringHash<IEnumerable<string>> multipleReplacements,
   StringSet included) : IMarkdownFrameOptions
{
   public string Source => source;

   public bool Tidy => tidy;

   public StringHash ScalarReplacements => scalarReplacements;

   public StringHash<IEnumerable<string>> MultipleReplacements => multipleReplacements;

   public StringSet Included => included;
}