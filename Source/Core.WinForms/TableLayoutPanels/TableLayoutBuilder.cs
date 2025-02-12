namespace Core.WinForms.TableLayoutPanels;

public class TableLayoutBuilder(TableLayoutPanel tableLayoutPanel)
{
   public static ControlInfo operator +(TableLayoutBuilder builder, Control control)
   {
      control.TabIndex = builder.tabIndex++;
      return new ControlInfo(builder, control);
   }

   protected AxisSetup columnsSetup = new();

   protected AxisSetup rowsSetup = new();

   protected int currentColumn;

   protected int currentRow;

   protected int tabIndex;

   public AxisSetup Col => columnsSetup;

   internal AxisSetup ColumnsSetup
   {
      set => columnsSetup = value;
   }

   public AxisSetup Row => rowsSetup;

   internal AxisSetup RowSetup
   {
      set => rowsSetup = value;
   }

   public TableLayoutPanel TableLayoutPanel => tableLayoutPanel;

   public int CurrentColumn
   {
      get => currentColumn;
      set => currentColumn = value;
   }

   public int CurrentRow
   {
      get => currentRow;
      set => currentRow = value;
   }

   public void SetUp()
   {
      tableLayoutPanel.ColumnStyles.Clear();
      tableLayoutPanel.ColumnCount = columnsSetup.Values.Count;
      foreach (var either in columnsSetup.Values)
      {
         var columnStyle = either.ToObject() switch
         {
            -1 => new ColumnStyle(SizeType.AutoSize),
            int absolute => new ColumnStyle(SizeType.Absolute, absolute),
            float percent => new ColumnStyle(SizeType.Percent, percent),
            _ => new ColumnStyle(SizeType.AutoSize)
         };
         tableLayoutPanel.ColumnStyles.Add(columnStyle);
      }

      tableLayoutPanel.RowStyles.Clear();
      tableLayoutPanel.RowCount = rowsSetup.Values.Count;
      foreach (var either in rowsSetup.Values)
      {
         var rowStyle = either.ToObject() switch
         {
            -1 => new RowStyle(SizeType.AutoSize),
            int absolute => new RowStyle(SizeType.Absolute, absolute),
            float percent => new RowStyle(SizeType.Percent, percent),
            _ => new RowStyle(SizeType.AutoSize)
         };
         tableLayoutPanel.RowStyles.Add(rowStyle);
      }

      tabIndex = 0;
   }

   public TableLayoutBuilder SkipCol(int count = 1)
   {
      currentColumn += count;
      return this;
   }

   public TableLayoutBuilder SkipRow(int count = 1)
   {
      currentRow += count;
      return this;
   }
}