using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.WinForms.TableLayoutPanels.BuilderFunctions;

namespace Core.WinForms.Tests;

public partial class Form1 : Form
{
   protected Panel panel1;

   public Form1()
   {
      InitializeComponent();

      panel1 = new Panel();

      var builder = new Builder(tableLayoutPanel);
      _ = builder + 50.ColPercent() + 30.ColPercent() + 20.ColPercent();
      _ = builder * 8 + 60.RowPixels();
      _ = builder + 100.RowPercent() + setup;

      _ = builder + panel1 + 0.At(0) + control;

      var uiDivider = new UiAction(this);
      _ = builder + uiDivider + 0.At(8) + 3.ColSpan() + control;
      uiDivider.Divider("part 2");

      var uiAction1 = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiAction1 + 1.At(0) + down;
      uiAction1.NoStatus("action 1");
      uiAction1.WorkingAlignment = CardinalAlignment.SouthEast;
      uiAction1.Maximum = 100;

      var uiAction2 = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiAction2 + down;
      uiAction2.NoStatus("action 2");

      var uiAction3 = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiAction3 + down;
      uiAction3.NoStatus("action 3");

      var uiButton1 = new UiAction(this);
      uiButton1.DefaultButton("alternates");
      _ = builder + uiButton1 + 2.At(0) + down;
      AcceptButton = uiButton1;
      uiButton1.Click += (_, _) => uiAction1.Alternate("alpha", "bravo", "charlie");
      uiButton1.ClickText = "alternates";

      var uiButton2 = new UiAction(this);
      uiButton2.CancelButton("checkbox");
      _ = builder + uiButton2 + down;
      CancelButton = uiButton2;
      uiButton2.Click += (_, _) => uiAction2.CheckBox("Fixed", false);
      uiButton2.ClickText = "Cancel";

      var uiButton3 = new UiAction(this) { AutoSizeText = true };
      uiButton3.KeyMatch("progress", "busy");
      uiButton3.Button("Working");
      _ = builder + uiButton3 + down;
      uiButton3.Click += (_, _) =>
      {
         uiAction3.WorkingAlignment = CardinalAlignment.SouthEast;
         uiAction3.Working = "working";
      };
      uiButton3.ClickText = "Working";

      var uiButton4 = new UiAction(this);
      _ = builder + uiButton4 + down;
      uiButton4.Button("Pulse");
      uiButton4.Click += (_, _) => uiAction1.Pulse();
      uiButton4.ClickText = "Pulse";

      var stager = new UiStager(panel1);

      var uiButton5 = new UiAction(this);
      _ = builder + uiButton5 + down;
      uiButton5.Button("Test stager");
      uiButton5.Click += (_, _) =>
      {
         foreach (var text in (string[]) ["alpha", "bravo", "charlie"])
         {
            var uiAction = new UiAction(this);
            stager.Add(uiAction, text);
         }
      };
      uiButton5.ClickText = "Test stager";

      var uiButton6 = new UiAction(this);
      _ = builder + uiButton6 + down;
      uiButton6.Button("Next stage");
      uiButton6.Click += (_, _) => stager.NextStage(UiActionType.Success);
      uiButton6.ClickText = "Next stage";

      var uiButton7 = new UiAction(this);
      _ = builder + uiButton7 + down;
      uiButton7.Button("Divider");
      uiButton7.Click += (_, _) => uiButton7.Divider("divider");
      uiButton7.ClickText = "Divider";

      var uiButton8 = new UiAction(this);
      _ = builder + uiButton8 + down;
      uiButton8.Button("Dirty");
      uiButton8.Click += (_, _) =>
      {
         uiDivider.IsDirty = !uiDivider.IsDirty;
         if (uiAction1.Type is not UiActionType.Success)
         {
            uiAction1.Success("Is Dirty");
         }

         uiAction1.IsDirty = !uiAction1.IsDirty;
      };
      uiButton8.ClickText = "Dirty";
   }
}