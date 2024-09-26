using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public record NullLine : Line
{
   public override Result<Unit> DocumentRender(ParsingState state, Document document) => unit;

   public override Result<Unit> HeaderRender(ParsingState state, HeaderFooter header) => unit;

   public override Result<Unit> FooterRender(ParsingState state, HeaderFooter footer) => unit;

   public override Result<Unit> TableRender(ParsingState state, Table table) => unit;
}