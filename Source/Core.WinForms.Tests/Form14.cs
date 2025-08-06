using Core.Enumerables;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form14 : Form
{
   protected ControlContainer<LabelText> labelTexts = ControlContainer<LabelText>.ReadingContainer();

   public Form14()
   {
      InitializeComponent();

      labelTexts.HorizontalCount = 5;
      foreach (var i in 5.Range())
      {
         labelTexts.Add(new LabelText($"Edit {i + 1}"));
      }

      labelTexts.ControlHeight = labelTexts[0].Map(c => c.Height);

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row * 2 * 50f;
      builder.SetUp();

      (builder + labelTexts).Row();
   }
}