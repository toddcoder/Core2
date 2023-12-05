using Core.Arrays;

namespace Core.WinForms.Controls;

public class TableLayoutPanelBuilder
{
   protected TableLayoutPanel tableLayoutPanel;
   protected List<TableLayoutPanelStyle> columnStyles;
   protected List<TableLayoutPanelStyle> rowStyles;
   protected ResizableMatrix<bool> blocked;

   public TableLayoutPanelBuilder(TableLayoutPanel tableLayoutPanel)
   {
      this.tableLayoutPanel = tableLayoutPanel;

      this.tableLayoutPanel.ColumnStyles.Clear();
      this.tableLayoutPanel.RowStyles.Clear();

      columnStyles = new List<TableLayoutPanelStyle>();
      rowStyles = new List<TableLayoutPanelStyle>();

      blocked = new ResizableMatrix<bool>(0, 0, false);
   }

   public TableLayoutPanelStyle Column
   {
      get
      {
         var column = new TableLayoutPanelStyle(this);
         columnStyles.Add(column);

         return column;
      }
   }

   public TableLayoutPanelStyle Row
   {
      get
      {
         var row = new TableLayoutPanelStyle(this);
         rowStyles.Add(row);

         return row;
      }
   }

   protected bool fits(int x, int y, int columnSpan, int rowSpan)
   {
      return false;
   }

   public void SetUp(Control control, int columnSpan = 1, int rowSpan = 1, string fontName = "Consolas", float fontSize = 12f,
      DockStyle dockStyle = DockStyle.Fill)
   {
      var rowCount = tableLayoutPanel.RowCount;
      var columnCount = tableLayoutPanel.ColumnCount;

      blocked.Resize(rowCount, columnCount);
      for (var row = 0; row < rowCount; row++)
      {
         for (var column = 0; column < columnCount; column++)
         {

         }
      }
   }
}