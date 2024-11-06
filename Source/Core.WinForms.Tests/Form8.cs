using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form8 : Form
{
   protected TableLayoutPanel tableLayoutPanel = new();

   public Form8()
   {
      Color[] colors = [Color.Red, Color.Blue, Color.Green];

      InitializeComponent();

      var size = new Size(100, 60);
      var row = new RectangleRow(ClientRectangle with { Height = 64 });
      for (var i = 0; i < 20; i++)
      {
         if (!row.MayContain(size))
         {
            row = row.NextRow();
         }

         row.Add(size);
         var rectangle = row[^1];
         var uiAction = new UiAction();
         uiAction.Display(i.ToString(), Color.White, colors[i % 3]);
         Controls.Add(uiAction);
         uiAction.Location = rectangle.Location;
         uiAction.Size = size;
      }

      Controls.Add(tableLayoutPanel);
      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col * 5 * 20f;
      _ = builder.Row + 100f;
      builder.SetUp();

      var uiAlfa = new UiAction();
      uiAlfa.CheckBox("Schema Change", false);
      (builder + uiAlfa).Next();
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      tableLayoutPanel.MoveTo(ClientRectangle.SouthWest(tableLayoutPanel.Width, tableLayoutPanel.Height));
   }
}