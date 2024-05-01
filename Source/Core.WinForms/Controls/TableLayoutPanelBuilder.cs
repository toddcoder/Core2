namespace Core.WinForms.Controls;

public class TableLayoutPanelBuilder(TableLayoutPanel tableLayoutPanel)
{
   public abstract class Dimension;

   public class Column : Dimension;

   public class Row : Dimension;

   public class NewRow : Dimension;

   public static TableLayoutPanelBuilder operator +(TableLayoutPanelBuilder builder, float value)
   {
      builder.styles.Add((SizeType.Percent, value));
      return builder;
   }

   public static TableLayoutPanelBuilder operator +(TableLayoutPanelBuilder builder, int value)
   {
      switch (value)
      {
         case 0:
            builder.styles.Add((SizeType.AutoSize, 0));
            break;
         default:
            builder.styles.Add((SizeType.Absolute, value));
            break;
      }

      return builder;
   }

   public static TableLayoutPanelBuilder operator +(TableLayoutPanelBuilder builder, Dimension dimension)
   {
      if (builder.isFirst)
      {
         builder.TableLayoutPanel.ColumnStyles.Clear();
         builder.TableLayoutPanel.RowStyles.Clear();
         builder.isFirst = false;
      }

      switch (dimension)
      {
         case Column:
            builder.addColumns();
            break;
         case Row:
            builder.addRows();
            break;
      }

      return builder;
   }

   public static TableLayoutPanelControlBuilder operator +(TableLayoutPanelBuilder builder, Control control)
   {
      var controlBuilder = new TableLayoutPanelControlBuilder(control);
      builder.controls.Add(controlBuilder);

      return controlBuilder;
   }

   protected bool isFirst = true;
   protected List<(SizeType sizeType, object value)> styles = new();
   protected List<TableLayoutPanelControlBuilder> controls = new();

   public TableLayoutPanel TableLayoutPanel => tableLayoutPanel;

   protected void addColumns()
   {
      tableLayoutPanel.ColumnCount = styles.Count;

      foreach (var (styleType, value) in styles)
      {
         switch (styleType, value)
         {
            case (SizeType.Percent, float percent):
               tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(styleType, percent));
               break;
            case (SizeType.Absolute, int absolute):
               tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(styleType, absolute));
               break;
            case (SizeType.AutoSize, _):
               tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(styleType));
               break;
         }
      }

      styles.Clear();
   }

   protected void addRows()
   {
      tableLayoutPanel.RowCount = styles.Count;

      foreach (var (styleType, value) in styles)
      {
         switch (styleType, value)
         {
            case (SizeType.Percent, float percent):
               tableLayoutPanel.RowStyles.Add(new RowStyle(styleType, percent));
               break;
            case (SizeType.Absolute, int absolute):
               tableLayoutPanel.RowStyles.Add(new RowStyle(styleType, absolute));
               break;
            case (SizeType.AutoSize, _):
               tableLayoutPanel.RowStyles.Add(new RowStyle(styleType));
               break;
         }
      }

      styles.Clear();
   }

   public void Build()
   {
      var columnIndex = 0;
      var rowIndex = 0;

      foreach (var builder in controls)
      {
         builder.SetUp(tableLayoutPanel, columnIndex, rowIndex);
         if (builder.NewRow)
         {
            rowIndex++;
            columnIndex = 0;
         }
         else
         {
            columnIndex++;
         }
      }
   }
}