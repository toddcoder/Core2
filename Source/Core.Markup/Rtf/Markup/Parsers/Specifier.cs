using Core.Monads;

namespace Core.Markup.Rtf.Markup.Parsers;

public abstract record Specifier
{
   public record Font(FontDescriptor FontDescriptor) : Specifier
   {
      public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + FontDescriptor;

      public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + FontDescriptor;

      public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + FontDescriptor;
   }

   public record Color(ColorDescriptor ColorDescriptor) : Specifier
   {
      public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + ColorDescriptor.Foreground;

      public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + ColorDescriptor.Foreground;

      public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + ColorDescriptor.Foreground;
   }

   public record BackgroundColor(ColorDescriptor ColorDescriptor) : Color(ColorDescriptor)
   {
      public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + ColorDescriptor.Background;

      public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + ColorDescriptor.Background;

      public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + ColorDescriptor.Background;
   }

   public record Style(Specifiers Specifiers) : Specifier
   {
      public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph)
      {
         return getStyle(state, Specifiers).Map(s => paragraph + s);
      }

      public override Result<Formatter> Render(ParsingState state, Formatter formatter)
      {
         return getStyle(state, Specifiers).Map(s => formatter + s);
      }

      public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style)
      {
         return getStyle(state, Specifiers).Map(s => style + s);
      }
   }

   public abstract record Feature : Specifier
   {
      public record None : Feature
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Feature.None;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Feature.None;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Feature.None;
      }

      public record Bold : Feature
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Feature.Bold;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Feature.Bold;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Feature.Bold;
      }

      public record Italic : Feature
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Feature.Italic;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Feature.Italic;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Feature.Italic;
      }

      public record Underline : Feature
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Feature.Underline;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Feature.Underline;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Feature.Underline;
      }

      public record Bullet : Feature
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Feature.Bullet;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Feature.Bullet;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Feature.Bullet;
      }

      public record NewPage : Feature
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Feature.NewPage;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Feature.NewPage;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Feature.NewPage;
      }

      public record NewPageAfter : Feature
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Feature.NewPageAfter;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Feature.NewPageAfter;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Feature.NewPageAfter;
      }
   }

   public abstract record Alignment : Specifier
   {
      public record Left : Alignment
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Alignment.Left;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Alignment.Left;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Alignment.Left;
      }

      public record Right : Alignment
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Alignment.Right;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Alignment.Right;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Alignment.Right;
      }

      public record Center : Alignment
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Alignment.Center;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Alignment.Center;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Alignment.Center;
      }

      public record Distributed : Alignment
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Alignment.Distributed;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Alignment.Distributed;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Alignment.Distributed;
      }

      public record FullyJustify : Alignment
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.Alignment.FullyJustify;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.Alignment.FullyJustify;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style + Rtf.Alignment.FullyJustify;
      }
   }

   public abstract record FieldType : Specifier
   {
      public record Page : FieldType
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.FieldType.Page;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.FieldType.Page;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style;
      }

      public record NumPages : FieldType
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.FieldType.NumPages;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.FieldType.NumPages;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style;
      }

      public record Date : FieldType
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.FieldType.Date;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.FieldType.Date;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style;
      }

      public record Time : FieldType
      {
         public override Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph) => paragraph + Rtf.FieldType.Time;

         public override Result<Formatter> Render(ParsingState state, Formatter formatter) => formatter + Rtf.FieldType.Time;

         public override Result<Rtf.Style> Render(ParsingState state, Rtf.Style style) => style;
      }
   }

   public abstract Result<Formatter> Render(ParsingState state, Rtf.Paragraph paragraph);

   public abstract Result<Formatter> Render(ParsingState state, Formatter formatter);

   public abstract Result<Rtf.Style> Render(ParsingState state, Rtf.Style style);

   protected Result<Rtf.Style> getStyle(ParsingState state, Specifiers specifiers)
   {
      var style = new Rtf.Style();
      foreach (var specifier in specifiers)
      {
         var _result = specifier.Render(state, style);
         if (_result is (true, var resultingStyle))
         {
            style = resultingStyle;
         }
         else
         {
            return _result;
         }
      }

      return style;
   }
}