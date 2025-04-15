using Core.Collections;
using Core.Computers;
using Core.Dates.DateIncrements;
using Core.Enumerables;
using Core.Json;
using Core.Matching;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.Documents;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form1 : Form
{
   protected Panel panel1;
   protected ExTextBox textBox;
   protected ExRichTextBox richTextBox;
   protected UiAction uiButton6 = new();
   protected UiAction uiDirty = new();

   public Form1()
   {
      InitializeComponent();

      var uiDivider = new UiAction();

      var menus = new FreeMenus { Form = this };
      menus.Menu("File");
      (menus + @"Show C:\Temp" + (() => uiButton6.RunWorkerAsync())).Menu();
      (menus + "ShowAndFade" + (() => uiDirty.ShowAndFadeOut())).Menu();
      (menus + "Tests").SubMenu();
      (menus + "Alpha" + (() => { })).Menu();
      (menus + "Bravo" + (() => { })).Menu();
      menus.GoUpTo("File");
      (menus + "Charlie" + (() => { })).Menu();
      menus.Separator();
      (menus + "Form2" + (() =>
      {
         var form2 = new Form2();
         form2.Show();
      }) + Keys.F2).Menu();
      (menus + "Form3" + (() =>
      {
         var form3 = new Form3();
         form3.Show();
      }) + Keys.F3).Menu();
      (menus + "Form4" + (() =>
      {
         var form4 = new Form4();
         form4.Show();
      }) + Keys.F4).Menu();
      (menus + "Form5" + (() =>
      {
         var form5 = new Form5();
         form5.Show();
      }) + Keys.F5).Menu();
      (menus + "Form6" + (() =>
      {
         var form6 = new Form6();
         form6.Show();
      }) + Keys.F6).Menu();
      (menus + "Form7" + (() =>
      {
         var form7 = new Form7();
         form7.Show();
      }) + Keys.F7).Menu();
      (menus + "Form8" + (() =>
      {
         var form8 = new Form8();
         form8.Show();
      }) + Keys.F8).Menu();
      (menus + "Form9" + (() =>
      {
         var form9 = new Form9();
         form9.Show();
      }) + Keys.F9).Menu();
      (menus + "Form10" + (() =>
      {
         var form10 = new Form10();
         form10.Show();
      }) + Keys.F10).Menu();
      (menus + "Form11" + (() =>
      {
         var form11 = new Form11();
         form11.Show();
      }) + Keys.F11).Menu();
      (menus + "Form12" + (() =>
      {
         var form12 = new Form12();
         form12.Show();
      }) + Keys.F12).Menu();
      (menus + "JSON" + retrieveJson + Keys.Control + Keys.J).Menu();
      (menus + "Start Flashing" + startFlashing + Keys.Control + Keys.F).Menu();
      (menus + "Stop Flashing" + stopFlashing + Keys.Control + Keys.Alt + Keys.F).Menu();
      menus.Menu("Edit");
      menus.StandardEditMenu();
      (menus + "Enabled" + (() => uiButton6.Enabled = !uiButton6.Enabled)).Menu();
      menus.RenderMainMenu();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 80f + 20f;
      _ = builder.Row * 11 * 60 + 100f;
      builder.SetUp();

      var uiAlternates = new UiAction { AutoSizeText = true };
      (builder + uiAlternates + (0, 0)).Next();
      uiAlternates.NoStatus("alternates/bar.action");
      uiAlternates.WorkingAlignment = CardinalAlignment.SouthEast;
      uiAlternates.Maximum = 100;
      uiAlternates.Click += (_, _) => uiAlternates.Success("alternates/bar.action");

      var uiButton1 = new UiAction();
      uiButton1.DefaultButton("alternates");
      (builder + uiButton1).Row();
      AcceptButton = uiButton1;
      uiButton1.Count = 3;
      uiButton1.Click += (_, _) =>
      {
         uiAlternates.Alternate("alpha", "bravo", "charlie");
         uiAlternates.SetColors(0, UiActionType.Message);
         uiAlternates.SetColors(1, UiActionType.Success);
         uiAlternates.SetColors(2, UiActionType.Failure);
      };
      uiButton1.ClickText = "alternates";

      var uiCheckBox = new UiAction { AutoSizeText = true };
      (builder + uiCheckBox).Next();
      uiCheckBox.NoStatus("checkbox action");

      var uiButton2 = new UiAction();
      uiButton2.CancelButton("checkbox");
      (builder + uiButton2).Row();
      CancelButton = uiButton2;
      uiButton2.Click += (_, _) => uiCheckBox.CheckBox("Multi-choice", false);
      uiButton2.ClickText = "Cancel";

      var uiWorking = new UiAction { AutoSizeText = true };
      (builder + uiWorking).Next();
      uiWorking.NoStatus("working action");

      var uiButton3 = new UiAction { AutoSizeText = true };
      //uiButton3.KeyMatch("progress", "busy");
      uiButton3.KeyDownCaption = new KeyDownCapture.ControlKey("Progress Working");
      uiButton3.Button("Busy Working");
      (builder + uiButton3).Row();
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
      (builder + panel1).Next();

      var stager = new UiStager(panel1);

      var uiButton4 = new UiAction();
      (builder + uiButton4).Row();
      uiButton4.Button("Test stager");
      uiButton4.Click += (_, _) => stager.AddRange("alpha", "bravo", "charlie");
      uiButton4.ClickText = "Test stager";

      var uiButton5 = new UiAction();
      (builder + uiButton5 + (1, -1)).Row();
      uiButton5.Button("Next stage");
      uiButton5.Click += (_, _) => stager.NextStage(UiActionType.Success);
      uiButton5.ClickText = "Next stage";

      (builder + uiDirty).Next();
      uiDirty.Success("Not Dirty");

      (builder + uiButton6).Row();
      uiButton6.Button("Dirty");
      uiButton6.Click += (_, _) =>
      {
         uiDivider.IsDirty = !uiDivider.IsDirty;
         uiDirty.IsDirty = !uiDirty.IsDirty;
         uiButton6.Status = uiDirty.IsDirty ? StatusType.Success : StatusType.Busy;
      };
      uiButton6.ClickText = "Dirty";
      uiButton6.Initialize += (_, e) =>
      {
         uiButton6.Status = StatusType.Busy;
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
      uiButton6.RunWorkerCompleted += (_, _) => uiButton6.Status = StatusType.Success;

      var uiChosen = new UiAction();
      (builder + uiChosen).Next();
      uiChosen.NoStatus("not chosen");

      string[] choices =
      [
         "alfa", "bravo", "charlie", "delta", "echo", "foxtrot", "-", "golf", "hotel", "india", "juliett", "kilo", "lima", "mike", "november", "oscar",
         "papa", "quebec", "romeo", "sierra", "tango", "uniform", "victor", "whiskey", "xray", "yankee", "zulu"
      ];

      string[] alternateChoices =
      [
         "alpha", "beta", "delta", "eta", "phi", "gamma", "iota", "kappa", "lambda", "mu", "nu", "omega", "pi", "rho", "sigma", "tau", "upsilon",
         "xi", "zeta"
      ];

      Set<Chosen> chosenSet = [];
      var counts = choices.ToStringHash(c => c, _ => 0);

      var uiButton7 = new UiAction { AutoSizeText = true, ChooserGlyph = true };
      (builder + uiButton7).Row();
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

      var uiSubText = new UiAction();
      (builder + uiSubText).Next();
      uiSubText.NoStatus("subtexts");

      var uiButton8 = new UiAction();
      (builder + uiButton8).Row();
      uiButton8.Button("SubTexts");
      //uiButton8.Status = UiActionType.NoStatus;
      uiButton8.Maximum = 20;
      uiButton8.Status = StatusType.Progress;
      uiButton8.Click += (_, _) =>
      {
         uiButton8.Status = StatusType.ProgressStep;
         uiSubText.ClearSubTexts();
         var subText1 = uiSubText.SubText("alfa").Set.FontSize(8).Invert().Alignment(CardinalAlignment.SouthWest).SubText;
         uiSubText.SubText("bravo").Set.FontSize(8).Invert().RightOf(subText1);
         uiSubText.Refresh();
         //uiButton8.Status = UiActionType.Done;
      };
      uiButton8.ClickText = "Create subTexts";

      var uiChoices = new UiAction();
      (builder + uiChoices).Next();
      uiChoices.NoStatus("choices");

      var uiButton9 = new UiAction { ChooserGlyph = true };
      (builder + uiButton9).Row();
      uiButton9.Button("Update Choices");
      var chooser = uiButton9.Choose("Update").AutoClose(false).Choices(choices).Chooser();
      uiButton9.Click += (_, _) => chooser.Open();
      uiButton9.ClickText = "Update choices";
      uiButton9.KeyDownCaption = new KeyDownCapture.ControlKey("Clear Choices");
      uiButton9.ChosenItemSelected += (_, _) =>
      {
         if (uiButton9.IsKeyDown)
         {
            chooser.ClearChoices();
         }
         else
         {
            chooser.Update(alternateChoices);
         }
      };

      (builder + uiDivider).SpanCol(2).Row();
      uiDivider.Divider("part 2");

      textBox = new ExTextBox(this) { BorderStyle = BorderStyle.None, AutoSelectAll = true };
      (builder + textBox).SpanCol(2).Row();
      textBox.Text = "test text";

      richTextBox = new ExRichTextBox();
      richTextBox.Shortcut(Keys.Control | Keys.D, _ => richTextBox.DuplicateCurrentLine());
      (builder + richTextBox).SpanCol(2).Next();
      richTextBox.Text = "test text";

      var textBoxMenu = new Menus();
      (textBoxMenu + textBox + "Open" + (() => { })).ContextMenu();
      textBoxMenu.ContextMenuSeparator();
      textBoxMenu.StandardContextEdit();
      textBoxMenu.CreateContextMenu();

      var richTextBoxMenu = new Menus();
      (richTextBoxMenu + richTextBox + "Open" + (() => { })).ContextMenu();
      richTextBoxMenu.ContextMenuSeparator();
      richTextBoxMenu.StandardContextEdit();
      richTextBoxMenu.CreateContextMenu();
   }

   protected void retrieveJson()
   {
      var url = textBox.Text;
      if (url.Matches("^ /(.+) '//_workitems//edit//' /(/d+)").Map(r => (prefix: r.FirstGroup, id: r.SecondGroup)) is (true, var (prefix, id)))
      {
         url = $"{prefix}/_apis/wit/workitems/{id}?$expand=all";
         textBox.Text = url;
      }

      richTextBox.Clear();
      var _retriever = JsonRetriever.FromUrl(url);
      if (_retriever is (true, var retriever))
      {
         foreach (var (propertyName, value) in retriever.RetrieveHash("System.Title", "System.WorkItemType", "Estream.Release.Target",
                     "Estream.ProdSupp.MergeStatus", "Estream.ProdSupp.MergedTo"))
         {
            richTextBox.AppendText($"{propertyName}: {value}\n");
         }
      }
   }

   protected void startFlashing() => this.StartFlashing();

   protected void stopFlashing() => this.StopFlashing();
}