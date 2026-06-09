using Core.Collections;
using Core.Monads;

namespace Core.Markdown;

public interface IMarkdownFrameOptions
{
   string Source { get; }

   bool Tidy { get; }

   ScalarReplacements ScalarReplacements { get; }

   MultiReplacements MultipleReplacements { get; }

   StringHash Variables { get; }

   Maybe<int> PageBreak { get; }
}