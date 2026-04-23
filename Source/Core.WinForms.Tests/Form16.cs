using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form16 : Form
{
   protected UiMenuAction menu = new();

   public Form16()
   {
      InitializeComponent();
      menu.TextItem("Alfa (A or alpha)", text => menu.Success(text));
      menu.TextItem("Bravo (B or beta)", text => menu.Success(text));
      menu.Success("Menu");

      var builder = new TableLayoutBuilder(tableLayoutPanel1);
      _ = builder.Col + 200 + 100f;
      _ = builder.Row + 60 + 100f;
      builder.SetUp();

      (builder + menu).Row();
   }
}