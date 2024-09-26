using Core.Matching;
using Core.Monads;

namespace Core.Markup.Rtf.Markup.Parsers;

public  abstract class SliceParser
{
   public abstract Pattern Pattern { get; }

   public abstract Optional<int> Parse(string line, string[] groups, ParsingState state);
}