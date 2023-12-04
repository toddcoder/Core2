using Core.Monads;

namespace Core.WinForms.Controls;

public class TableLayoutPanelControlBuilder(Control control)
{
   public static TableLayoutPanelControlBuilder operator +(TableLayoutPanelControlBuilder builder, (Maybe<int>, Maybe<int>) spans)
   {
      var (_columnSpan, _rowSpan) = spans;
      switch (_columnSpan.ToObject(), _rowSpan.ToObject())
      {
         case (int columnSpan, int rowSpan):
            builder.ColumnSpan = columnSpan;
            builder.RowSpan = rowSpan;
            break;
         case (int columnSpan, Nil):
            builder.ColumnSpan = columnSpan;
            break;
         case (Nil, int rowSpan):
            builder.RowSpan = rowSpan;
            break;
         case (Nil, Nil):
            break;
      }

      return builder;
   }

   public static TableLayoutPanelControlBuilder operator +(TableLayoutPanelControlBuilder builder, string fontName)
   {
      builder.FontName = fontName;
      return builder;
   }

   public static TableLayoutPanelControlBuilder operator +(TableLayoutPanelControlBuilder builder, float fontSize)
   {
      builder.FontSize = fontSize;
      return builder;
   }

   public static TableLayoutPanelControlBuilder operator +(TableLayoutPanelControlBuilder builder, DockStyle dockStyle)
   {
      builder.DockStyle = dockStyle;
      return builder;
   }

   public static TableLayoutPanelControlBuilder operator +(TableLayoutPanelControlBuilder builder, TableLayoutPanelBuilder.Dimension dimension)
   {
      builder.NewRow = dimension is TableLayoutPanelBuilder.NewRow;
      return builder;
   }

   public int ColumnSpan { get; set; } = 1;

   public int RowSpan { get; set; } = 1;

   public string FontName { get; set; } = "Consolas";

   public float FontSize { get; set; } = 12f;

   public DockStyle DockStyle { get; set; } = DockStyle.Fill;

   public bool NewRow { get; set; } = false;

   public void SetUp(TableLayoutPanel tableLayoutPanel, int column, int row)
   {
      control.SetUpInTableLayoutPanel(tableLayoutPanel, column, row, ColumnSpan, RowSpan, FontName, FontSize, DockStyle);
   }
}