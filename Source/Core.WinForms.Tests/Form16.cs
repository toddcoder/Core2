using Core.WinForms.Controls;

namespace Core.WinForms.Tests;

public partial class Form16 : Form
{
   protected UiMenuAction menu = new();

   public Form16()
   {
      InitializeComponent();
      menu.TextItem("Alfa", () => Text = "A");
      menu.TextItem("Bravo", () => Text = "B");
      menu.Success("Menu");
      Controls.Add(menu);
      menu.Location = new Point(0, 0);
   }
}