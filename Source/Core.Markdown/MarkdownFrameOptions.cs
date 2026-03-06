using Core.Collections;

namespace Core.Markdown;

public struct MarkdownFrameOptions() : IMarkdownFrameOptions
{
   public string Source { get; set; } = "";

   public bool Tidy { get; set; } = true;

   public ScalarReplacements ScalarReplacements { get; set; } = new();

   public MultiReplacements MultipleReplacements { get; set; } = new();

   public StringHash Variables { get; set; } = [];
}