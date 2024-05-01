using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.TableLayoutPanels;

using BuilderSetup = (Setup setup, Axis axis, float amount);

public class Builder(TableLayoutPanel tableLayoutPanel)
{
   protected List<BuilderSetup> setups = [];
   protected int currentColumn;
   protected int currentRow;
   protected Maybe<int> _repeat = nil;

   public static Builder operator +(Builder builder, BuilderSetup setup) => setup.setup switch
   {
      Setup.Percent when setup.axis is Axis.Column => builder.AddColumnPercent(setup.amount),
      Setup.Pixels when setup.axis is Axis.Column => builder.AddColumnPixels(setup.amount),
      Setup.AutoSize when setup.axis is Axis.Column => builder.AddColumnAutoSize(),
      Setup.Percent when setup.axis is Axis.Row => builder.AddRowPercent(setup.amount),
      Setup.Pixels when setup.axis is Axis.Row => builder.AddRowPixels(setup.amount),
      Setup.AutoSize when setup.axis is Axis.Row => builder.AddRowAutoSize(),
      _ => builder
   };

   public static Builder operator *(Builder builder, int times)
   {
      builder.Repeat = times;
      return builder;
   }

   public static ColumnBuilder operator +(Builder builder, Control control) => new(control, builder);

   public static Builder operator +(Builder builder, Terminator terminator) => terminator switch
   {
      Terminator.Setup => builder.SetUp(),
      Terminator.Control => builder,
      _ => builder
   };

   public Maybe<int> Repeat
   {
      get => _repeat;
      set => _repeat = value;
   }

   public Builder AddColumnPercent(float amount)
   {
      if (_repeat is (true, var repeat))
      {
         for (var i = 0; i < repeat; i++)
         {
            setups.Add((Setup.Percent, Axis.Column, amount));
         }

         _repeat = nil;
      }
      else
      {
         setups.Add((Setup.Percent, Axis.Column, amount));
      }

      return this;
   }

   public Builder AddColumnPixels(float amount)
   {
      if (_repeat is (true, var repeat))
      {
         for (var i = 0; i < repeat; i++)
         {
            setups.Add((Setup.Pixels, Axis.Column, amount));
         }

         _repeat = nil;
      }
      else
      {
         setups.Add((Setup.Pixels, Axis.Column, amount));
      }

      return this;
   }

   public Builder AddColumnAutoSize()
   {
      if (_repeat is (true, var repeat))
      {
         for (var i = 0; i < repeat; i++)
         {
            setups.Add((Setup.AutoSize, Axis.Column, 0));
         }

         _repeat = nil;
      }
      else
      {
         setups.Add((Setup.AutoSize, Axis.Column, 0));
      }

      return this;
   }

   public Builder AddRowPercent(float amount)
   {
      if (_repeat is (true, var repeat))
      {
         for (var i = 0; i < repeat; i++)
         {
            setups.Add((Setup.Percent, Axis.Row, amount));
         }

         _repeat = nil;
      }
      else
      {
         setups.Add((Setup.Percent, Axis.Row, amount));
      }

      return this;
   }

   public Builder AddRowPixels(float amount)
   {
      if (_repeat is (true, var repeat))
      {
         for (var i = 0; i < repeat; i++)
         {
            setups.Add((Setup.Pixels, Axis.Row, amount));
         }

         _repeat = nil;
      }
      else
      {
         setups.Add((Setup.Pixels, Axis.Row, amount));
      }

      return this;
   }

   public Builder AddRowAutoSize()
   {
      if (_repeat is (true, var repeat))
      {
         for (var i = 0; i < repeat; i++)
         {
            setups.Add((Setup.AutoSize, Axis.Row, 0));
         }

         _repeat = nil;
      }
      else
      {
         setups.Add((Setup.AutoSize, Axis.Row, 0));
      }

      return this;
   }

   public Builder SetUp()
   {
      tableLayoutPanel.ColumnStyles.Clear();
      tableLayoutPanel.ColumnCount = setups.Count(s => s.axis == Axis.Column);
      foreach (var (setup, _, amount) in setups.Where(s => s.axis == Axis.Column))
      {
         Maybe<ColumnStyle> _columnStyle = setup switch
         {
            Setup.Percent => new ColumnStyle(SizeType.Percent, amount),
            Setup.Pixels => new ColumnStyle(SizeType.Absolute, amount),
            Setup.AutoSize => new ColumnStyle(SizeType.AutoSize),
            _ => nil
         };
         if (_columnStyle is (true, var columnStyle))
         {
            tableLayoutPanel.ColumnStyles.Add(columnStyle);
         }
      }

      tableLayoutPanel.RowStyles.Clear();
      tableLayoutPanel.RowCount = setups.Count(s => s.axis == Axis.Row);
      foreach (var (setup, _, amount) in setups.Where(s => s.axis == Axis.Row))
      {
         Maybe<RowStyle> _rowStyle = setup switch
         {
            Setup.Percent => new RowStyle(SizeType.Percent, amount),
            Setup.Pixels => new RowStyle(SizeType.Absolute, amount),
            Setup.AutoSize => new RowStyle(SizeType.AutoSize),
            _ => nil
         };
         if (_rowStyle is (true, var rowStyle))
         {
            tableLayoutPanel.RowStyles.Add(rowStyle);
         }
      }

      return this;
   }

   public Builder AddColumn(ColumnBuilder columnBuilder, bool incrementColumn)
   {
      var control = columnBuilder.Control;
      control.Dock = columnBuilder.DockStyle;

      if (columnBuilder.Column is (true, var column))
      {
         currentColumn = column;
      }

      if (columnBuilder.Row is (true, var row))
      {
         currentRow = row;
      }

      tableLayoutPanel.Controls.Add(control, currentColumn, currentRow);

      if (columnBuilder.ColumnSpan is (true, var columnSpan))
      {
         tableLayoutPanel.SetColumnSpan(control, columnSpan);
      }

      if (columnBuilder.RowSpan is (true, var rowSpan))
      {
         tableLayoutPanel.SetRowSpan(control, rowSpan);
      }

      control.Font = new Font(columnBuilder.FontName, columnBuilder.FontSize);

      if (incrementColumn)
      {
         currentColumn++;
      }

      return this;
   }

   public Builder NextRow(ColumnBuilder builder)
   {
      AddColumn(builder, false);
      currentRow++;
      currentColumn = 0;

      return this;
   }

   public Builder Down(ColumnBuilder builder)
   {
      AddColumn(builder, false);
      currentRow++;

      return this;
   }

   public Builder Skip()
   {
      currentColumn++;
      return this;
   }
}