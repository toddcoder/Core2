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

      var builder = new Builder(tableLayoutPanel);
      _ = builder + 80.ColPercent() + 20.ColPercent();
      _ = builder * 8 + 60.RowPixels();
      _ = builder + 100.RowPercent() + setup;

      var uiDivider = new UiAction(this);
      _ = builder + uiDivider + (0, 8) - (2, 0) + control;
      uiDivider.Divider("part 2");

      var uiAlternates = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiAlternates + (0, 0) + control;
      uiAlternates.NoStatus("alternates action");
      uiAlternates.WorkingAlignment = CardinalAlignment.SouthEast;
      uiAlternates.Maximum = 100;

      var uiButton1 = new UiAction(this);
      uiButton1.DefaultButton("alternates");
      _ = builder + uiButton1 + row;
      AcceptButton = uiButton1;
      uiButton1.Click += (_, _) => uiAlternates.Alternate("alpha", "bravo", "charlie");
      uiButton1.ClickText = "alternates";

      var uiCheckBox = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiCheckBox + control;
      uiCheckBox.NoStatus("checkbox action");

      var uiButton2 = new UiAction(this);
      uiButton2.CancelButton("checkbox");
      _ = builder + uiButton2 + row;
      CancelButton = uiButton2;
      uiButton2.Click += (_, _) => uiCheckBox.CheckBox("Fixed", false);
      uiButton2.ClickText = "Cancel";

      var uiWorking = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiWorking + control;
      uiWorking.NoStatus("working action");

      var uiButton3 = new UiAction(this) { AutoSizeText = true };
      uiButton3.KeyMatch("progress", "busy");
      uiButton3.Button("Working");
      _ = builder + uiButton3 + row;
      uiButton3.Click += (_, _) =>
      {
         if (uiWorking.Working)
         {
            uiWorking.Pulse();
         }
         else
         {
            uiWorking.WorkingAlignment = CardinalAlignment.SouthEast;
            uiWorking.Working = "working";
         }
      };
      uiButton3.ClickText = "Working";

      panel1 = new Panel();
      _ = builder + panel1 + control;

      var stager = new UiStager(panel1);

      var uiButton4 = new UiAction(this);
      _ = builder + uiButton4 + row;
      uiButton4.Button("Test stager");
      uiButton4.Click += (_, _) =>
      {
         foreach (var text in (string[]) ["alpha", "bravo", "charlie"])
         {
            var uiAction = new UiAction(this);
            stager.Add(uiAction, text);
         }
      };
      uiButton4.ClickText = "Test stager";

      var uiButton5 = new UiAction(this);
      _ = builder + uiButton5 + (1, -1) + row;
      uiButton5.Button("Next stage");
      uiButton5.Click += (_, _) => stager.NextStage(UiActionType.Success);
      uiButton5.ClickText = "Next stage";

      var uiDirty = new UiAction(this);
      _ = builder + uiDirty + control;
      uiDirty.Success("Not Dirty");

      var uiButton6 = new UiAction(this);
      _ = builder + uiButton6 + down;
      uiButton6.Button("Dirty");
      uiButton6.Click += (_, _) =>
      {
         uiDivider.IsDirty = !uiDivider.IsDirty;
         if (uiDirty.Type is not UiActionType.Success)
         {
            uiDirty.Success("Is Dirty");
         }

         uiDirty.IsDirty = !uiDirty.IsDirty;
      };
      uiButton6.ClickText = "Dirty";
   }
}