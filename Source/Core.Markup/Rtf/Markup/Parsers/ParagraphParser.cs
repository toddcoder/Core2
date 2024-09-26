using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public class ParagraphParser : LineParser
{
   public override Pattern Pattern => "";

   public override Optional<Line> Parse(string[] groups, ParsingState state)
   {
      try
      {
         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}