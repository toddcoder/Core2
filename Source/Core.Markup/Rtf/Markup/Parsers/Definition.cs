using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf.Markup.Parsers;

public abstract record Definition : Line
{
   public record Font(FontDescriptor FontDescriptor) : Definition
   {
      public override Result<Unit> Render(ParsingState state) => unit;
   }

   public record Color(ColorDescriptor ColorDescriptor) : Definition
   {
      public override Result<Unit> Render(ParsingState state) => unit;
   }

   public record Style(Specifiers Specifiers) : Definition
   {
      public override Result<Unit> Render(ParsingState state) => unit;
   }
}