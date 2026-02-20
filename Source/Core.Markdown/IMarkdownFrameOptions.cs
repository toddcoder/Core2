using Core.Collections;

namespace Core.Markdown;

public interface IMarkdownFrameOptions
{
   string Source { get; }

   bool Tidy { get; }

   StringHash ScalarReplacements { get; }

   StringHash<IEnumerable<string>> MultipleReplacements { get; }

   StringSet Included { get; }
}