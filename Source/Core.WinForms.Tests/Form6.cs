using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form6 : Form
{
   protected UiAction uiTextDivider = new();
   protected ExTextBox textBox = new() { BorderStyle = BorderStyle.None };

   public Form6()
   {
      InitializeComponent();

      uiTextDivider.Divider("Text");

      textBox.TextChanged += (_, _) =>
      {
         var text = textBox.Text;
         uiTextDivider.DividerValidation = text is "Todd" or "Bennett" ? new DividerValidation.Valid() : new DividerValidation.Invalid("Expected Todd or Bennett");
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col * 100f;
      _ = builder.Row + 40 + 40 + 100f;
      builder.SetUp();

      (builder + uiTextDivider + false).Row();

      (builder + textBox).Row();
   }
}