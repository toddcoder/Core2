using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form9 : Form
{
   protected LabelUrl luUrl = new("url");

   public Form9()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 60 + 100f;
      builder.SetUp();

      (builder + luUrl).Row();
   }
}