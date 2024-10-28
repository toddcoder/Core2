using Core.WinForms.Controls;

namespace Core.WinForms.Tests;

public partial class Form8 : Form
{
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
   }
}