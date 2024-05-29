using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.WinForms.TableLayoutPanels.BuilderFunctions;

namespace Core.WinForms.Tests;

public partial class Form2 : Form
{
   protected UiActionContainer container1 = new();
   protected UiActionContainer container2 = new() { Direction = UiActionDirection.Vertical };
   protected UiAction uiAction1;
   protected UiAction uiAction2;
   protected UiAction uiAction3;
   protected UiAction uiAction4;
   protected UiAction uiAction5;
   protected UiAction uiAction6;


   public Form2()
   {
      InitializeComponent();

      var builder = new Builder(tableLayoutPanel);
      _ = builder + 100.ColPercent();
      _ = builder + 60.RowPixels() + 180.RowPixels() + 100.RowPercent() + setup;

      _ = builder + container1 + (0, 0) + row;
      _ = builder + container2 + control;

      uiAction1 = new UiAction(container1);
      container1.Add(uiAction1);
      uiAction1.Button("Alfa");
      uiAction1.Click += (_, _) => Text = "Alfa";
      uiAction1.ClickText = "Alfa";

      uiAction2 = new UiAction(container1);
      container1.Add(uiAction2);
      uiAction2.Button("Bravo");
      uiAction2.Click += (_, _) => Text = "Bravo";
      uiAction2.ClickText = "Bravo";

      uiAction3 = new UiAction(container1);
      container1.Add(uiAction3);
      uiAction3.Button("Charlie");
      uiAction3.Click += (_, _) => Text = "Charlie";
      uiAction3.ClickText = "Charlie";

      uiAction4 = new UiAction(container2);
      container2.Add(uiAction4);
      uiAction4.Button("Delta");
      uiAction4.Click += (_, _) => Text = "Delta";
      uiAction4.ClickText = "Delta";

      uiAction5 = new UiAction(container2);
      container2.Add(uiAction5);
      uiAction5.Button("Echo");
      uiAction5.Click += (_, _) => Text = "Echo";
      uiAction5.ClickText = "Echo";

      uiAction6 = new UiAction(container2);
      container2.Add(uiAction6);
      uiAction6.Button("Foxtrot");
      uiAction6.Click += (_, _) => Text = "Foxtrot";
      uiAction6.ClickText = "Foxtrot";
   }
}