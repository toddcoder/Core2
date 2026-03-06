using Core.Collections;
using Core.Markdown;

namespace Core.Tests;

public class MarkdownFrameTestOptions(string source) : IMarkdownFrameOptions
{
   public string Source => source;

   public bool Tidy { get; set; } = false;

   public ScalarReplacements ScalarReplacements { get; set; } = new();

   public MultiReplacements MultipleReplacements { get; set; } = new();

   public StringHash Variables { get; set; } = [];
}