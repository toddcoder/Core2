using System.Collections;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Controls;

public class ReadingContainer : UserControl, IEnumerable<IEnumerable<Control>>
{
   protected TableLayoutPanel panel = new() { Dock = DockStyle.Fill };
   protected List<TableLayoutPanel> rows = [];
   protected int verticalHeight;

   public ReadingContainer(int verticalCount)
   {
      Controls.Add(panel);
   }

   public AxisSetup columnsSetup = new();

   public AxisSetup Col => columnsSetup;

   public void Add(params IEnumerable<Control> controls)
   {
      var row = new TableLayoutPanel();
      var builder = new TableLayoutBuilder(row);
      _ = builder.ColumnsSetup = columnsSetup;
      _ = builder.Row + 100f;
      builder.SetUp();

      foreach (var control in controls)
      {
         (builder + control).Next();
      }

      rows.Add(row);
   }

   public IEnumerator<IEnumerable<Control>> GetEnumerator()
   {
      foreach (var controls in rows.Select(getControls))
      {
         yield return controls;
      }

      yield break;

      IEnumerable<Control> getControls(TableLayoutPanel panel)
      {
         foreach (var control in panel.Controls)
         {
            if (control is Control controlToYield)
            {
               yield return controlToYield;
            }
         }
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}