using Core.Monads;
using Core.WinForms.Controls;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.TableLayoutPanels;

public class ControlInfo(TableLayoutBuilder builder, Control control)
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

   public static ControlInfo operator +(ControlInfo controlInfo, FontStyle fontStyle)
   {
      controlInfo.fontStyle = fontStyle;
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

   public static ControlInfo operator +(ControlInfo controlInfo, bool tabStop)
   {
      controlInfo.Control.TabStop = tabStop;
      return controlInfo;
   }

   protected Maybe<int> _column = nil;
   protected Maybe<int> _row = nil;
   protected int columnSpan = 1;
   protected int rowSpan = 1;
   protected string fontName = "Consolas";
   protected float fontSize = 12f;
   protected FontStyle fontStyle = FontStyle.Regular;
   protected DockStyle dockStyle = DockStyle.Fill;

   public TableLayoutBuilder Builder => builder;

   public Control Control => control;

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

   public FontStyle FontStyle
   {
      get => fontStyle;
      set => fontStyle = value;
   }

   public DockStyle DockStyle
   {
      get => dockStyle;
      set => dockStyle = value;
   }

   [Obsolete("Use SpanCol or SpanRow")]
   public ControlInfo Span(int column = 1, int row = 1)
   {
      columnSpan = column;
      rowSpan = row;

      return this;
   }

   public ControlInfo SpanCol(int columnSpan)
   {
      this.columnSpan = columnSpan;
      return this;
   }

   public ControlInfo SpanRow(int rowSpan)
   {
      this.rowSpan = rowSpan;
      return this;
   }

   protected void setUp()
   {
      var column = _column | builder.CurrentColumn;
      var row = _row | builder.CurrentRow;
      control.SetUpInTableLayoutPanel(builder.TableLayoutPanel, column, row, columnSpan, rowSpan, fontName, fontSize, fontStyle, dockStyle);
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

   public ControlInfo NextCol(int increment = 1)
   {
      var column = _column | builder.CurrentColumn;
      builder.CurrentColumn = column + increment;

      reset();

      return this;
   }

   public ControlInfo NextRow(int increment = 1)
   {
      var row = _row | builder.CurrentRow;
      builder.CurrentRow = row + increment;
      builder.CurrentColumn = 0;

      reset();

      return this;
   }
}