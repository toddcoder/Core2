using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public class StyleDefinitionParser : LineParser
{
   public override Pattern Pattern => $"^ '//style' /s+ /({WORD}) /s+ /(.+)$; f";

   public override Optional<Line> Parse(string[] groups, ParsingState state)
   {
      try
      {
         var name = groups[0];
         var value = groups[1];

         var parser = new SpecifierParser();
         var _specifiers = parser.Parse(value, state);
         if (_specifiers is (true, var specifiers))
         {
            var styleDefinition = new Definition.Style(specifiers);
            state.Definitions[name] = styleDefinition;

            return new NullLine();
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}