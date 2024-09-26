using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public class FontDefinitionParser : LineParser
{
   public override Pattern Pattern => $"^ '//font' /s+ /({WORD}) /s* /(.*)$; f";

   public override Optional<Line> Parse(string[] groups, ParsingState state)
   {
      try
      {
         var name = groups[0];
         var value = groups[1];
         if (value.IsEmpty())
         {
            value = name.ToTitleCase();
         }

         var fontDescriptor = state.Document.Font(value);
         state.Definitions[name] = new Definition.Font(fontDescriptor);

         return new NullLine();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}