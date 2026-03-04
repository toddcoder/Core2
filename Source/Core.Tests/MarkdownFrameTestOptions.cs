using Core.Collections;
using Core.Markdown;

namespace Core.Tests;

public class MarkdownFrameTestOptions(string source) : IMarkdownFrameOptions
{
   public string Source => source;

   public bool Tidy { get; set; } = false;

   public StringHash ScalarReplacements { get; set; } = [];

   public StringHash<Replacements> MultipleReplacements { get; set; } = [];

   public StringSet Included { get; set; } = [];

   public StringHash Variables { get; set; } = [];
}