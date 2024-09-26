using Core.Matching;
using Core.Monads;

namespace Core.Markup.Rtf.Markup.Parsers;

public abstract class LineParser
{
   public const string WORD = "[/w:-]+";
   public const string HEX = "'0x' ['0-9a-f'] % 6";

   public abstract Pattern Pattern { get; }

   public abstract Optional<Line> Parse(string[] groups, ParsingState state);

   public IEnumerable<LineParser> Parsers()
   {
      yield return new FontDefinitionParser();
      yield return new ColorDefinitionParser();
      yield return new StyleDefinitionParser();
   }
}