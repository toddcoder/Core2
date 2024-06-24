namespace Core.Markup.Rtf;

public class TableCellArgs(int rowIndex, int columnIndex, string text, TableCell tableCell) : EventArgs
{
   public int RowIndex => rowIndex;

   public int ColumnIndex => columnIndex;

   public string Text => text;

   public TableCell TableCell => tableCell;
}