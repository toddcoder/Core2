using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.WinForms.TableLayoutPanels.BuilderFunctions;

namespace Core.WinForms.Tests;

public partial class Form2 : Form
{
   protected UiActionContainer container = new();
   protected UiAction uiAction1;
   protected UiAction uiAction2;
   protected UiAction uiAction3;

   public Form2()
   {
      InitializeComponent();

      var builder = new Builder(tableLayoutPanel);
      _ = builder + 100.ColPercent();
      _ = builder + 60.RowPixels() + 100.RowPercent() + setup;

      _ = builder + container + (0, 0) + control;

      uiAction1 = new UiAction(container);
      container.Add(uiAction1);
      uiAction1.Button("Alfa");
      uiAction1.Click += (_, _) => Text = "Alfa";
      uiAction1.ClickText = "Alfa";

      uiAction2 = new UiAction(container);
      container.Add(uiAction2);
      uiAction2.Button("Bravo");
      uiAction2.Click += (_, _) => Text = "Bravo";
      uiAction2.ClickText = "Bravo";

      uiAction3 = new UiAction(container);
      container.Add(uiAction3);
      uiAction3.Button("Charlie");
      uiAction3.Click += (_, _) => Text = "Charlie";
      uiAction3.ClickText = "Charlie";
   }
}