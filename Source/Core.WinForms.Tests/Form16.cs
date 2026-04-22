using Core.WinForms.Controls;

namespace Core.WinForms.Tests;

public partial class Form16 : Form
{
   protected UiMenuAction menu = new();

   public Form16()
   {
      InitializeComponent();
      menu.TextItem("Alfa (A or alpha)", text => Text = text);
      menu.TextItem("Bravo (B or beta)", text => Text = text);
      menu.Success("Menu");
      Controls.Add(menu);
      menu.Location = new Point(0, 0);
   }
}