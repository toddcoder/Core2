using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.WinForms.TableLayoutPanels.BuilderFunctions;

namespace Core.WinForms.Tests;

public partial class Form2 : Form
{
   protected UiActionContainer container1 = UiActionContainer.HorizontalContainer();
   protected UiActionContainer container2 = UiActionContainer.VerticalContainer();


   public Form2()
   {
      InitializeComponent();

      var builder = new Builder(tableLayoutPanel);
      _ = builder + 100.ColPercent();
      _ = builder + 60.RowPixels() + 180.RowPixels() + 100.RowPercent() + setup;

      _ = builder + container1 + (0, 0) + row;
      _ = builder + container2 + control;

      var uiAction = container1.Add("Alfa");
      uiAction.Click += (_, _) => Text = "Alfa";
      uiAction.ClickText = "Alfa";

      uiAction = container1.Add("Bravo");
      uiAction.Click += (_, _) => Text = "Bravo";
      uiAction.ClickText = "Bravo";

      uiAction = container1.Add("Charlie");
      uiAction.Click += (_, _) => Text = "Charlie";
      uiAction.ClickText = "Charlie";

      uiAction = container2.Add("Delta");
      uiAction.Click += (_, _) => Text = "Delta";
      uiAction.ClickText = "Delta";

      uiAction = container2.Add("Echo");
      uiAction.Click += (_, _) => Text = "Echo";
      uiAction.ClickText = "Echo";

      uiAction = container2.Add("Foxtrot");
      uiAction.Click += (_, _) => Text = "Foxtrot";
      uiAction.ClickText = "Foxtrot";
   }
}