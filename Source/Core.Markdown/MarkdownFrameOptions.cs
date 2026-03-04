using Core.Collections;

namespace Core.Markdown;

public struct MarkdownFrameOptions() : IMarkdownFrameOptions
{
   public string Source { get; set; } = "";

   public bool Tidy { get; set; } = true;

   public StringHash ScalarReplacements { get; set; } = [];

   public StringHash<Replacements> MultipleReplacements { get; set; } = [];

   public StringSet Included { get; set; } = [];

   public StringHash Variables { get; set; } = [];
}