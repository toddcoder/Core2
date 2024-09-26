using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public abstract record Paragraph : Line
{
   public record SpecifiedParagraph(Specifiers Specifiers, string Line) : Line
   {
      public override Result<Unit> DocumentRender(ParsingState state, Document document)
      {
         try
         {
            var paragraph = document + Line;
            foreach (var specifier in Specifiers)
            {
               
            }
            return unit;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public override Result<Unit> HeaderRender(ParsingState state, HeaderFooter header) => unit;

      public override Result<Unit> FooterRender(ParsingState state, HeaderFooter footer) => unit;

      public override Result<Unit> TableRender(ParsingState state, Table table) => unit;
   }
}