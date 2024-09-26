using Core.Monads;

namespace Core.Markup.Rtf.Markup.Parsers;

public abstract record Line
{
   public abstract Result<Unit> DocumentRender(ParsingState state, Document document);

   public abstract Result<Unit> HeaderRender(ParsingState state, HeaderFooter header);

   public abstract Result<Unit> FooterRender(ParsingState state, HeaderFooter footer);

   public abstract Result<Unit> TableRender(ParsingState state, Table table);
}