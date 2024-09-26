using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public abstract record Context : Line
{
   public record DocumentContext : Context
   {
      public override Result<Unit> Render(ParsingState state)
      {
         state.Context = this;
         return unit;
      }

      public override Result<Unit> DocumentRender(ParsingState state, Document document)

      public override Result<Unit> HeaderRender(ParsingState state, HeaderFooter header) => TODO_IMPLEMENT_ME;

      public override Result<Unit> FooterRender(ParsingState state, HeaderFooter footer) => TODO_IMPLEMENT_ME;

      public override Result<Unit> TableRender(ParsingState state, Table table) => TODO_IMPLEMENT_ME;
   }

   public record HeaderContext : Context
   {
      public override Result<Unit> Render(ParsingState state)
      {
         state.Context = this;
         return unit;
      }
   }

   public record FooterContext : Context
   {
      public override Result<Unit> Render(ParsingState state)
      {
         state.Context = this;
         return unit;
      }
   }

   public record TableContext(float Size) : Context
   {
      public override Result<Unit> Render(ParsingState state)
      {
         state.Context = this;
         return unit;
      }
   }
}