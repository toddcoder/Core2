using Core.Enumerables;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form3 : Form
{
   protected UiAction uiChooser;
   protected UiAction uiResult;

   public Form3()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 50f + 50f;
      _ = builder.Row + 60 + 100f;
      builder.SetUp();

      uiChooser = new UiAction(this) { ChooserGlyph = true };
      (builder + uiChooser).Next();
      uiChooser.ChosenItemChecked += (_, e) => { uiResult!.Message(e.ChosenSet.ToString(", ")); };
      uiChooser.Click += (_, _) =>
      {
         var chooser = uiChooser.Choose("Items").Choices("alfa", "bravo", "charlie", "delta", "echo", "foxtrot").MultiChoice(true).Chooser();
      };
      uiChooser.ClickText = "Select items";

      uiResult = new UiAction(this);
      (builder + uiResult).Row();
   }
}