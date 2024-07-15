using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form3 : Form
{
   protected UiAction uiChooserTop = new() { ChooserGlyph = true };
   protected UiAction uiChooserBottom = new() { ChooserGlyph = true };
   protected UiAction uiResult = new();

   public Form3()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 40 + 100f + 40 + 40;
      builder.SetUp();

      uiChooserTop.Click += (_, _) =>
      {
         var _chosen = uiChooserTop.Choose("Items").Choices(getChoices()).Choose();
         if (_chosen is (true, var chosen))
         {
            uiChooserTop.Success(chosen.Value);
         }
      };

      uiChooserBottom.Click += (_, _) =>
      {
         var _chosen = uiChooserBottom.Choose("Items").Choices(getChoices()).FlyUp().Choose();
         if (_chosen is (true, var chosen))
         {
            uiChooserBottom.Success(chosen.Value);
         }
      };
      uiChooserBottom.ClickText = "Select items";

      (builder + uiChooserTop).Row();
      (builder + uiChooserBottom).NextRow().Row();
      (builder + uiResult).Row();

      return;

      IEnumerable<string> getChoices() => ["alfa", "bravo", "charlie", "delta", "echo", "foxtrot"];
   }
}