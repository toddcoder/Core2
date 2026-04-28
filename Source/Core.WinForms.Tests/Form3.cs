using Core.WinForms.Components;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form3 : Form
{
   protected class RandomNumbers(ListBox listBox, UiAction uiAction) : Background
   {
      protected Random random = new();
      protected List<int> numbers = [];

      public override void Initialize()
      {
         uiAction.Busy(true);
         numbers.Clear();
      }

      public override void DoWork()
      {
         for (var i = 0; i < 100; i++)
         {
            numbers.Add(random.Next(1, 100));
         }
      }

      public override void RunWorkerCompleted()
      {
         foreach (var number in numbers)
         {
            listBox.Items.Add(number);
         }

         uiAction.Success("Done");
      }
   }

   protected RandomNumbers randomNumbers;
   protected UiMenuAction uiChooserTop = new();
   protected UiMenuAction uiChooserBottom = new();
   protected UiAction uiResult = new();

   public Form3()
   {
      InitializeComponent();

      randomNumbers = new RandomNumbers(listBox1, uiResult);

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 40 + 100f + 40 + 40;
      builder.SetUp();

      uiChooserTop.RequestMenuItems.Handler = () => uiChooserTop.Choose(getChoices()).Then(chosen => uiChooserTop.Success(chosen));
      uiChooserTop.ClickText = "Select items";

      uiChooserBottom.RequestMenuItems.Handler = () => uiChooserBottom.Choose(getChoices()).Then(chosen => uiChooserBottom.Success(chosen));
      uiChooserBottom.ClickText = "Select items";

      uiResult.Button("Start");
      uiResult.Click += (_, _) => randomNumbers.RunWorkerAsync();

      (builder + uiChooserTop).Row();
      (builder + listBox1).Row();
      (builder + uiChooserBottom).Row();
      (builder + uiResult).Row();

      return;

      IEnumerable<string> getChoices() => ["alfa", "bravo", "charlie", "", "delta", "echo", "foxtrot"];
   }
}