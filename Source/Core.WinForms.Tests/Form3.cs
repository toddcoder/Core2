using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form3 : Form
{
   protected UiAction uiChooserTop;
   protected UiAction uiChooserBottom;
   protected UiAction uiResult;

   public Form3()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 40 + 100f + 40 + 40;
      builder.SetUp();

      uiChooserTop = new UiAction(this) { ChooserGlyph = true };
      uiChooserTop.Click += (_, _) =>
      {
         var _chosen = uiChooserTop.Choose("Items").Choices(getChoices()).Choose();
         if (_chosen is (true, var chosen))
         {
            uiChooserTop.Success(chosen.Value);
         }
      };

      uiChooserBottom = new UiAction(this) { ChooserGlyph = true };
      uiChooserBottom.Click += (_, _) =>
      {
         var _chosen = uiChooserBottom.Choose("Items").Choices(getChoices()).FlyUp().Choose();
         if (_chosen is (true, var chosen))
         {
            uiChooserBottom.Success(chosen.Value);
         }
      };
      uiChooserBottom.ClickText = "Select items";

      uiResult = new UiAction(this);

      (builder + uiChooserTop).Row();
      (builder + uiChooserBottom).NextRow().Row();
      (builder + uiResult).Row();

      return;

      IEnumerable<string> getChoices() => ["alfa", "bravo", "charlie", "delta", "echo", "foxtrot"];
   }
}