using Core.Collections;

namespace Core.Markdown;

public interface IMarkdownFrameOptions
{
   string Source { get; }

   bool Tidy { get; }

   StringHash ScalarReplacements { get; }

   StringHash<Replacements> MultipleReplacements { get; }

   StringSet Included { get; }

   StringHash Variables { get; }
}