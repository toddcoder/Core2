using Core.Collections;
using Core.Computers;
using Core.Dates.DateIncrements;
using Core.Enumerables;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.Documents;
using Core.WinForms.TableLayoutPanels;
using static Core.WinForms.Documents.MenuBuilderFunctions;
using static Core.WinForms.TableLayoutPanels.BuilderFunctions;

namespace Core.WinForms.Tests;

public partial class Form1 : Form
{
   protected Panel panel1;
   protected ExTextBox textBox;
   protected ExRichTextBox richTextBox;
   protected UiAction uiButton6;

   public Form1()
   {
      InitializeComponent();

      var menus = new FreeMenus { Form = this };
      menus.Menu("File");
      _ = menus + "&Open" + (() => uiButton6!.RunWorkerAsync()) + menu;
      _ = menus + "Tests" + subMenu;
      _ = menus + "Alpha" + (() => { }) + menu;
      _ = menus + "Bravo" + (() => { }) + menu;
      menus.GoUpTo("File");
      _ = menus + "Charlie" + (() => { }) + menu;
      menus.Menu("Edit");
      menus.StandardEditMenu();
      menus.RenderMainMenu();

      var builder = new Builder(tableLayoutPanel);
      _ = builder + 80.ColPercent() + 20.ColPercent();
      _ = builder * 10 + 60.RowPixels();
      _ = builder + 100.RowPercent() + setup;

      var uiDivider = new UiAction(this);
      _ = builder + uiDivider + (0, 8) - (2, 0) + control;
      uiDivider.Divider("part 2");

      textBox = new ExTextBox(this) { BorderStyle = BorderStyle.None, AutoSelectAll = true };
      _ = builder + textBox + (0, 9) - (2, 0) + control;
      textBox.Text = "test text";

      richTextBox = new ExRichTextBox();
      _ = builder + richTextBox + (0, 10) - (2, 0) + control;
      richTextBox.Text = "test text";

      var textBoxMenu = new Menus();
      _ = textBoxMenu + textBox + "Open" + (() => { }) + contextMenu;
      textBoxMenu.ContextMenuSeparator();
      textBoxMenu.StandardContextEdit();
      textBoxMenu.CreateContextMenu();

      var richTextBoxMenu = new Menus();
      _ = richTextBoxMenu + richTextBox + "Open" + (() => { }) + contextMenu;
      richTextBoxMenu.ContextMenuSeparator();
      richTextBoxMenu.StandardContextEdit();
      richTextBoxMenu.CreateContextMenu();

      var uiAlternates = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiAlternates + (0, 0) + control;
      uiAlternates.NoStatus("alternates action");
      uiAlternates.WorkingAlignment = CardinalAlignment.SouthEast;
      uiAlternates.Maximum = 100;

      var uiButton1 = new UiAction(this);
      uiButton1.DefaultButton("alternates");
      _ = builder + uiButton1 + row;
      AcceptButton = uiButton1;
      uiButton1.Click += (_, _) =>
      {
         uiAlternates.Alternate("alpha", "bravo", "charlie");
         uiAlternates.SetColors(0, UiActionType.Message);
         uiAlternates.SetColors(1, UiActionType.Success);
         uiAlternates.SetColors(2, UiActionType.Failure);
      };
      uiButton1.ClickText = "alternates";

      var uiCheckBox = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiCheckBox + control;
      uiCheckBox.NoStatus("checkbox action");

      var uiButton2 = new UiAction(this);
      uiButton2.CancelButton("checkbox");
      _ = builder + uiButton2 + row;
      CancelButton = uiButton2;
      uiButton2.Click += (_, _) => uiCheckBox.CheckBox("Multi-choice", false);
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

      uiButton6 = new UiAction(this);
      _ = builder + uiButton6 + row;
      uiButton6.Button("Dirty");
      uiButton6.Click += (_, _) =>
      {
         uiDivider.IsDirty = !uiDivider.IsDirty;
         uiDirty.IsDirty = !uiDirty.IsDirty;
         uiButton6.Status = uiDirty.IsDirty ? UiActionType.Success : UiActionType.Busy;
      };
      uiButton6.ClickText = "Dirty";
      uiButton6.Initialize += (_, e) =>
      {
         uiButton6.Status = UiActionType.Busy;
         FolderName folder = @"C:\Temp";
         e.Argument = folder;
      };
      uiButton6.DoWork += (_, e) =>
      {
         if (e.Argument is FolderName folder)
         {
            var timeSpan = 3.Seconds();
            foreach (var file in folder.Files)
            {
               uiDivider.Message(file.NameExtension);
               Thread.Sleep(timeSpan);
            }
         }
      };
      uiButton6.RunWorkerCompleted += (_, _) => uiButton6.Status = UiActionType.Success;

      var uiChosen = new UiAction(this);
      _ = builder + uiChosen + control;
      uiChosen.NoStatus("not chosen");

      string[] choices =
      [
         "alfa", "bravo", "charlie", "delta", "echo", "foxtrot", "golf", "hotel", "india", "juliett", "kilo", "lima", "mike", "november", "oscar",
         "papa", "quebec", "romeo", "sierra", "tango", "uniform", "victor", "whiskey", "xray", "yankee", "zulu"
      ];

      Set<Chosen> chosenSet = [];
      var counts = choices.ToStringHash(c => c, _ => 0);

      var uiButton7 = new UiAction(this) { AutoSizeText = true };
      _ = builder + uiButton7 + row;
      uiButton7.NoStatus("choose");
      uiButton7.Click += (_, _) =>
      {
         if (uiDirty.IsDirty)
         {
            uiButton7.Choose("Non-Auto-Close").AutoClose(false).Choices(choices).Choose();
         }
         else if (uiCheckBox.BoxChecked)
         {
            uiButton7.Choose("Multi-Choose").MultiChoice(chosenSet).Choices(choices).Choose();
         }
         else
         {
            var _chosen = uiButton7.Choose("Choose").Choices(choices).Choose();
            if (_chosen is (true, var chosen))
            {
               uiChosen.Success(chosen.Value);
            }
         }
      };
      uiButton7.ChosenItemChecked += (_, e) => chosenSet = e.ChosenSet;
      uiButton7.ChooserClosed += (_, _) => uiButton7.Success(chosenSet.Select(c => c.Key.Keep(1)).Order().ToString(""));
      uiButton7.ChosenItemSelected += (_, e) =>
      {
         var count = (counts.Maybe[e.Chosen.Value] | 0) + 1;
         counts[e.Chosen.Value] = count;
         e.Chosen.Key = $"{e.Chosen.Value} ({count})";
      };
      uiButton7.ClickText = "Choose";

      var uiSubText = new UiAction(this);
      _ = builder + uiSubText + control;
      uiSubText.NoStatus("subtexts");

      var uiButton8 = new UiAction(this);
      _ = builder + uiButton8 + row;
      uiButton8.Button("SubTexts");
      uiButton8.Status = UiActionType.NoStatus;
      uiButton8.Click += (_, _) =>
      {
         uiSubText.ClearSubTexts();
         var subText1 = uiSubText.SubText("alfa").Set.FontSize(8).Invert().Alignment(CardinalAlignment.SouthWest).SubText;
         uiSubText.SubText("bravo").Set.FontSize(8).Invert().RightOf(subText1);
         uiSubText.Refresh();
         uiButton8.Status = UiActionType.Success;
      };
      uiButton8.ClickText = "Create subTexts";
   }
}