using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public class ContextParser : LineParser
{
   public override Pattern Pattern => "^ '&' /('document' | 'header' | 'footer') $; f";

   public override Optional<Line> Parse(string[] groups, ParsingState state)
   {
      try
      {
         return groups[0] switch
         {
            "document" => new Context.DocumentContext(),
            "header" => new Context.HeaderContext(),
            "footer" => new Context.FooterContext(),
            _ => nil
         };
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}