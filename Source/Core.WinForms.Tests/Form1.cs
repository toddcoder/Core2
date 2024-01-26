using Core.WinForms.Controls;

namespace Core.WinForms.Tests;

public partial class Form1 : Form
{
   public Form1()
   {
      InitializeComponent();

      var uiAction = new UiAction(this) { AutoSizeText = true };
      uiAction.SetUpInTableLayoutPanel(tableLayoutPanel, 1, 0);
      uiAction.NoStatus("not set");

      var uiButton1 = new UiAction(this);
      uiButton1.DefaultButton("button 1");
      uiButton1.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 0);
      AcceptButton = uiButton1;
      uiButton1.Click += (_, _) => uiButton1.Success("OK");
      uiButton1.ClickText = "OK";

      var uiButton2 = new UiAction(this);
      uiButton2.CancelButton("button 2");
      uiButton2.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 1);
      CancelButton = uiButton2;
      uiButton2.Click += (_, _) => uiButton2.Success("Cancel");
      uiButton2.ClickText = "Cancel";

      var uiButton3 = new UiAction(this) { AutoSizeText = true };
      uiButton3.KeyMatch("progress", "busy");
      uiButton3.Button("Working");
      uiButton3.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 2);
      uiButton3.Click += (_, _) =>
      {
         uiAction.Working = "working";
         if (uiButton3.IsKeyDown)
         {
            uiAction.Busy("busy");
         }
         else
         {
            uiAction.Maximum = 100;
            for (var i = 0; i < 100; i++)
            {
               uiAction.Progress(i);
            }
         }
      };
      uiButton3.ClickText = "Working";

      var uiButton4 = new UiAction(this);
      uiButton4.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 3);
      uiButton4.Button("Pulse");
      uiButton4.Click += (_, _) => { uiAction.Pulse(); };
      uiButton4.ClickText = "Pulse";

      var stager = new UiStager(panel1);

      var uiButton5 = new UiAction(this);
      uiButton5.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 4);
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
      uiButton6.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 5);
      uiButton6.Button("Next stage");
      uiButton6.Click += (_, _) => { stager.NextStage(UiActionType.Success); };
      uiButton6.ClickText = "Next stage";
   }
}