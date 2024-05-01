using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.TableLayoutPanels;

public class ColumnBuilder(Control control, Builder builder)
{
   public static ColumnBuilder operator +(ColumnBuilder builder, (int column, int row) location)
   {
      builder.Column = maybe<int>() & location.column > -1 & location.column;
      builder.Row = maybe<int>() & location.row > -1 & location.row;

      return builder;
   }

   public static ColumnBuilder operator -(ColumnBuilder builder, (int columnSpan, int rowSpan) span)
   {
      if (span.columnSpan > 0)
      {
         builder.ColumnSpan = span.columnSpan;
      }

      if (span.rowSpan > 0)
      {
         builder.RowSpan = span.rowSpan;
      }

      return builder;
   }

   public static ColumnBuilder operator +(ColumnBuilder builder, (Axis axis, int amount) location)
   {
      switch (location.axis)
      {
         case Axis.Column:
            builder.Column = location.amount;
            break;
         case Axis.Row:
            builder.Row = location.amount;
            break;
      }

      return builder;
   }

   public static ColumnBuilder operator +(ColumnBuilder builder, (Span span, int amount) span)
   {
      switch (span.span)
      {
         case Span.Column:
            builder.ColumnSpan = span.amount;
            break;
         case Span.Row:
            builder.RowSpan = span.amount;
            break;
      }

      return builder;
   }

   public static ColumnBuilder operator +(ColumnBuilder builder, string fontName)
   {
      builder.FontName = fontName;
      return builder;
   }

   public static ColumnBuilder operator +(ColumnBuilder builder, float fontSize)
   {
      builder.FontSize = fontSize;
      return builder;
   }

   public static ColumnBuilder operator +(ColumnBuilder builder, DockStyle dockStyle)
   {
      builder.DockStyle = dockStyle;
      return builder;
   }

   public static Builder operator +(ColumnBuilder builder, Terminator terminator) => terminator switch
   {
      Terminator.Control => builder.Builder.AddColumn(builder, true),
      Terminator.Row => builder.Builder.NextRow(builder),
      Terminator.Down => builder.Builder.Down(builder),
      Terminator.Skip => builder.Builder.Skip(),
      _ => builder.Builder
   };

   public Control Control => control;

   public Builder Builder => builder;

   public Maybe<int> Column { get; set; } = nil;

   public Maybe<int> Row { get; set; } = nil;

   public Maybe<int> ColumnSpan { get; set; } = nil;

   public Maybe<int> RowSpan { get; set; } = nil;

   public string FontName { get; set; } = "Consolas";

   public float FontSize { get; set; } = 12f;

   public DockStyle DockStyle { get; set; } = DockStyle.Fill;
}