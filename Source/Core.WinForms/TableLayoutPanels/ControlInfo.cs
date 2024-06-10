using Core.Monads;
using Core.WinForms.Controls;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.TableLayoutPanels;

public class ControlInfo(Builder builder, Control control)
{
   public static ControlInfo operator +(ControlInfo controlInfo, (int column, int row) tuple)
   {
      if (tuple.column > -1)
      {
         controlInfo._column = tuple.column;
      }

      if (tuple.row > -1)
      {
         controlInfo._row = tuple.row;
      }

      return controlInfo;
   }

   public static ControlInfo operator +(ControlInfo controlInfo, string fontName)
   {
      controlInfo.fontName = fontName;
      return controlInfo;
   }

   public static ControlInfo operator +(ControlInfo controlInfo, float fontSize)
   {
      controlInfo.fontSize = fontSize;
      return controlInfo;
   }

   public static ControlInfo operator +(ControlInfo controlInfo, DockStyle dockStyle)
   {
      controlInfo.dockStyle = dockStyle;
      return controlInfo;
   }

   public static ControlInfo operator +(ControlInfo controlInfo, Control control)
   {
      controlInfo.Next();
      return new ControlInfo(controlInfo.Builder, control);
   }

   protected Maybe<int> _column = nil;
   protected Maybe<int> _row = nil;
   protected int columnSpan = 1;
   protected int rowSpan = 1;
   protected string fontName = "Consolas";
   protected float fontSize = 12f;
   protected DockStyle dockStyle = DockStyle.Fill;

   public Builder Builder => builder;

   public int ColumnSpan
   {
      get => columnSpan;
      set => columnSpan = value;
   }

   public int RowSpan
   {
      get => rowSpan;
      set => rowSpan = value;
   }

   public string FontName
   {
      get => fontName;
      set => fontName = value;
   }

   public float FontSize
   {
      get => fontSize;
      set => fontSize = value;
   }

   public DockStyle DockStyle
   {
      get => dockStyle;
      set => dockStyle = value;
   }

   public ControlInfo Span(int column = 1, int row = 1)
   {
      columnSpan = column;
      rowSpan = row;

      return this;
   }

   protected void setUp()
   {
      var column = _column | builder.CurrentColumn;
      var row = _row | builder.CurrentRow;
      control.SetUpInTableLayoutPanel(builder.TableLayoutPanel, column, row, columnSpan, rowSpan, fontName, fontSize, dockStyle);
   }

   protected void reset()
   {
      columnSpan = 1;
      rowSpan = 1;
   }

   public void Next()
   {
      setUp();

      var column = _column | builder.CurrentColumn;
      builder.CurrentColumn = column + columnSpan;

      reset();
   }

   public void Row(int increment = 1)
   {
      setUp();

      var row = _row | builder.CurrentRow;
      builder.CurrentRow = row + increment;
      builder.CurrentColumn = 0;

      reset();
   }

   public void Down(int increment = 1)
   {
      setUp();

      var row = _row | builder.CurrentRow;
      builder.CurrentRow = row + increment;

      reset();
   }
}