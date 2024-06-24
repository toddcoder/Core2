namespace Core.Markup.Rtf;

public class CellMergeInfo(TableCell representative, int rowSpan, int columnSpan, int rowIndex, int columnIndex)
{
   protected int rowSpan = rowSpan;
   protected int columnSpan = columnSpan;
   protected int rowIndex = rowIndex;
   protected int columnIndex = columnIndex;
   protected TableCell representative = representative;

   public int RowSpan => rowSpan;

   public int ColumnSpan => columnSpan;

   public int RowIndex => rowIndex;

   public int ColumnIndex => columnIndex;

   public TableCell Representative => representative;
}