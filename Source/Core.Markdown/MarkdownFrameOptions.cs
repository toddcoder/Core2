using Core.Collections;

namespace Core.Markdown;

public class MarkdownFrameOptions(string source) : IMarkdownFrameOptions
{
   public string Source => source;

   public bool Tidy { get; set; }

   public StringHash ScalarReplacements { get; set; } = [];

   public StringHash<Replacements> MultipleReplacements { get; set; } = [];

   public StringSet Included { get; set; } = [];
}