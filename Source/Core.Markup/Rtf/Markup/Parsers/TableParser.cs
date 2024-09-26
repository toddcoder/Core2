using Core.Matching;
using Core.Monads;
using Core.Objects;

namespace Core.Markup.Rtf.Markup.Parsers;

public class TableParser : LineParser
{
   public override Pattern Pattern => "^ 'table' /s+ '^' /(/d+) $; f";

   public override Optional<Line> Parse(string[] groups, ParsingState state)
   {
      try
      {
         return groups[0].Optional().Single().Map(Line (s) => new Context.TableContext(s));
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}