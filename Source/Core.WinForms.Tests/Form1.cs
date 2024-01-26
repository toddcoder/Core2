using Core.WinForms.Controls;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public partial class Form1 : Form
{
   public Form1()
   {
      var progressIndex = 1;

      InitializeComponent();

      var uiAction = new UiAction(this) { AutoSizeText = true };
      uiAction.SetUpInTableLayoutPanel(tableLayoutPanel, 1, 0);
      uiAction.NoStatus("not set");
      uiAction.WorkingAlignment = CardinalAlignment.SouthEast;
      uiAction.Maximum = 100;

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
         else if(progressIndex <= 100)
         {
            uiAction.Progress(progressIndex++);
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
         foreach (var text in (string[])["alpha", "bravo", "charlie"])
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

      var uiButton7 = new UiAction(this);
      uiButton7.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 6);
      uiButton7.Button("Busy");
      uiButton7.Click += (_, _) =>
      {
         if (uiAction.Stopwatch)
         {
            uiAction.StopStopwatch();
            uiAction.Stopwatch = false;
            uiAction.Success("Done");
            uiAction.Working = nil;
         }
         else
         {
            uiAction.Stopwatch = true;
            uiAction.StartStopwatch();
            uiAction.Busy("[r-6.56.6] | [r-6.56.6_Prod_6.56.6.17]");
            uiAction.Working = "building";
         }
      };
      uiButton7.ClickText = "Busy";
   }
}