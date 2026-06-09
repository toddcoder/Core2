using Core.Collections;
using Core.Markdown;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public class MarkdownFrameTestOptions(string source, bool tidy, ScalarReplacements scalarReplacements, MultiReplacements multipleReplacements,
   StringHash variables) : IMarkdownFrameOptions
{
   public string Source => source;

   public bool Tidy => tidy;

   public ScalarReplacements ScalarReplacements => scalarReplacements;

   public MultiReplacements MultipleReplacements => multipleReplacements;

   public StringHash Variables => variables;

   public Maybe<int> PageBreak { get; set; } = nil;
}