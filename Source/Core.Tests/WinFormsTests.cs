﻿using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Dates.DateIncrements;
using Core.Strings;
using Core.WinForms;
using Core.WinForms.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;
using Timer = System.Windows.Forms.Timer;

namespace Core.Tests;

[TestClass]
public class WinFormsTests
{
   [TestMethod]
   public void ConsoleScrollingTest()
   {
      var form = new Form();
      var richTextBox = new RichTextBox { Dock = DockStyle.Fill };
      form.Controls.Add(richTextBox);
      var console = new WinForms.Consoles.TextBoxConsole(form, richTextBox);
      using var writer = console.Writer();

      form.Show();

      for (var i = 0; i < 300; i++)
      {
         writer.WriteLine(i);
      }
   }

   [TestMethod]
   public void MessageProgressBusyTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
      uiAction.Busy("This message is in no way clickable!");
      form.ShowDialog();
   }

   [TestMethod]
   public void MessageProgressBusyIndefiniteTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
      uiAction.Busy(true);
      form.ShowDialog();
   }

   [TestMethod]
   public void ProgressDefiniteTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
      uiAction.Maximum = 50;
      var i = 0;

      var timer = new Timer();
      timer.Tick += (_, _) =>
      {
         if (i++ < 50)
         {
            uiAction.Progress("x".Repeat(i));
         }
         else
         {
            timer.Stop();
         }
      };
      timer.Start();
      form.ShowDialog();
   }

   [TestMethod]
   public void AutomaticMessageTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
      var stopwatch = new Stopwatch();
      uiAction.AutomaticMessage += (_, e) => { e.Text = stopwatch.Elapsed.ToString(); };
      uiAction.StartAutomatic();
      stopwatch.Start();
      form.ShowDialog();
   }

   [TestMethod]
   public void EnabledTest1()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 300, 30);
      uiAction.CheckStyle = CheckStyle.Checked;
      uiAction.Busy("working...");

      var checkBox = new CheckBox
      {
         Text = "Enabled",
         Checked = true
      };
      checkBox.Click += (_, _) => uiAction.Enabled = checkBox.Checked;
      checkBox.Location = new Point(0, 35);
      form.Controls.Add(checkBox);

      form.ShowDialog();
   }

   [TestMethod]
   public void ArrowMessageTest()
   {
      var form = new Form();
      var message = new UiAction() { Arrow = true };
      message.SetUp(4, 4, form.ClientSize.Width - 20, 100, AnchorStyles.Left);
      message.Message("Arrow");
      form.ShowDialog();
   }

   [TestMethod]
   public void StatusesTest()
   {
      var form = new Form();
      UiAction[] uiActions = [.. Enumerable.Range(0, 4).Select(_ => new UiAction())];
      var width = form.ClientSize.Width - 20;
      for (var i = 0; i < 4; i++)
      {
         uiActions[i].SetUp(4, 4 + i * 27, width, 27, AnchorStyles.Left);
      }

      uiActions[0].Success("Success");
      uiActions[1].Caution("Caution");
      uiActions[2].Failure("Failure");
      uiActions[3].Exception(fail("Exception"));

      form.ShowDialog();
   }

   [TestMethod]
   public void ImageTest()
   {
      var form = new Form();
      var uiAction = new UiAction()
      {
         Dock = DockStyle.Fill
      };

      var image = Image.FromFile(@"..\..\TestData\build.jpg");
      uiAction.Image = image;
      uiAction.Message("Build");
      form.ShowDialog();
   }

   [TestMethod]
   public void StretchImageTest()
   {
      var form = new Form();
      var uiAction = new UiAction()
      {
         Dock = DockStyle.Fill
      };

      var image = Image.FromFile(@"..\..\TestData\build.jpg");
      uiAction.Image = image;
      uiAction.StretchImage = true;
      uiAction.Message("Build");
      form.ShowDialog();
   }

   [TestMethod]
   public void SubTextTest()
   {
      var form = new Form();
      var uiAction = new UiAction()
      {
         Dock = DockStyle.Fill
      };
      uiAction.SubText("now").Set
         .GoToUpperLeft(8)
         .Outline()
         .ForeColor(Color.White)
         .BackColor(Color.Green)
         .Font("Verdana", 8);
      uiAction.Message("Message");
      form.ShowDialog();
   }

   [TestMethod]
   public void StopwatchTest()
   {
      var form = new Form();
      var uiAction = new UiAction() { Stopwatch = true };
      uiAction.SetUp(0, 0, 300, 27, AnchorStyles.Left | AnchorStyles.Right);
      uiAction.Busy("Timing");
      uiAction.StartStopwatch();
      form.ShowDialog();
   }

   [TestMethod]
   public void ExTextBoxTest()
   {
      var form = new Form();
      var textBox = new ExTextBox(form) { RefreshOnTextChange = true };
      textBox.Font = new Font("Consolas", 12);
      textBox.Width = form.ClientSize.Width;
      textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
      textBox.Location = new Point(0, 0);
      textBox.Paint += (_, e) =>
      {
         var pen = new Pen(Color.Green, 2);
         e.Graphics.DrawRectangle(pen, textBox.Bounds);
         foreach (var (rectangle, word) in textBox.RectangleWords(e.Graphics))
         {
            Console.WriteLine(word);
            textBox.DrawHighlight(e.Graphics, rectangle, Color.White, Color.Red, DashStyle.Dot);
         }
      };
      form.ShowDialog();
   }

   [TestMethod]
   public void ExTextBoxTest2()
   {
      var form = new Form();
      var textBox = new ExTextBox(form) { RefreshOnTextChange = true };
      textBox.Font = new Font("Consolas", 12);
      textBox.Width = form.ClientSize.Width;
      textBox.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
      textBox.Location = new Point(0, 0);
      textBox.Paint += (_, e) =>
      {
         var rectangle = new Rectangle(textBox.Bounds.Width - 16, 0, 16, textBox.Bounds.Height);
         using var brush = new HatchBrush(HatchStyle.DiagonalBrick, Color.White, Color.Green);
         e.Graphics.FillRectangle(brush, rectangle);
      };
      form.ShowDialog();
   }

   [TestMethod]
   public void ExTextBoxTest3()
   {
      var form = new Form();
      var textBox = new ExTextBox(form)
      {
         RefreshOnTextChange = true,
         Font = new Font("Consolas", 12),
         Width = form.ClientSize.Width,
         Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top,
         Location = new Point(0, 0),
         BackColor = SystemColors.Control
      };
      textBox.Paint += (_, e) =>
      {
         foreach (var (rectangle, word) in textBox.RectangleWords(e.Graphics))
         {
            Console.WriteLine(word);
            textBox.DrawHighlight(e.Graphics, rectangle, Color.Black, Color.CadetBlue);
         }
      };
      form.ShowDialog();
   }

   [TestMethod]
   public void ExTextBoxTest4()
   {
      var form = new Form();
      var textBox = new ExTextBox(form)
      {
         RefreshOnTextChange = true,
         ForeColor = Color.White,
         BackColor = Color.Blue
      };
      textBox.SetUp(0, 0, form.ClientSize.Width, 30);
      textBox.Paint += (_, e) =>
      {
         foreach (var (rectangle, _) in textBox.RectangleWords(e.Graphics))
         {
            textBox.DrawHighlight(e.Graphics, rectangle, Color.White, Color.White, DashStyle.Dot);
         }
      };
      form.ShowDialog();
   }

   [TestMethod]
   public void BackgroundTest()
   {
      var running = false;
      var text = "a";

      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 400, 40, AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right);
      uiAction.Message("Ready");
      uiAction.Click += (_, _) =>
      {
         if (running)
         {
            running = false;
            uiAction.Success("Done");
         }
         else
         {
            uiAction.ClickText = "Stop";
            running = true;
            uiAction.RunWorkerAsync();
         }
      };
      uiAction.ClickText = "Start";
      uiAction.DoWork += (_, _) =>
      {
         while (running)
         {
            uiAction.Do(() => uiAction.Busy(text));
            Application.DoEvents();
            text = text.Succ();
            Thread.Sleep(500.Milliseconds());
         }
      };

      form.ShowDialog();
   }

   [TestMethod]
   public void TextBoxLabelTest()
   {
      var form = new Form();
      var textBox = new TextBox();
      form.Controls.Add(textBox);
      textBox.Location = new Point(0, 30);
      textBox.Width = form.ClientSize.Width;

      var uiAction = new UiAction();
      uiAction.AttachTo("Name", textBox);
      uiAction.Click += (_, _) =>
      {
         textBox.Text = "";
         uiAction.IsDirty = true;
      };
      uiAction.ClickText = "Clear text";

      form.ShowDialog();
   }

   [TestMethod]
   public void TestBoxLabelTest2()
   {
      var form = new Form();
      var textBox = new TextBox();
      form.Controls.Add(textBox);
      textBox.Location = new Point(0, 30);
      textBox.Width = form.ClientSize.Width;

      var label = new UiAction();
      label.AttachTo("Name", textBox);
      label.Click += (_, _) => textBox.Text = "";
      label.ClickText = "Clear text";

      var status = new UiAction();
      status.AttachTo("Name", textBox, left: 100, stretch: true);
      status.Success("Success!");

      form.ShowDialog();
   }

   [TestMethod]
   public void UiActionButtonTest()
   {
      var form = new Form();
      var uiButton = new UiAction();
      uiButton.SetUp(0, 0, 200, 40);
      uiButton.Button("Push me");
      uiButton.Click += (_, _) => form.Text = "Clicked";
      uiButton.ClickText = "Click me";
      form.ShowDialog();
   }

   [TestMethod]
   public void UiActionWorkingTest()
   {
      var form = new Form();
      var uiButton = new UiAction();
      uiButton.SetUp(0, 0, 200, 40);
      uiButton.Message("Not Working");
      uiButton.Click += (_, _) =>
      {
         uiButton.Working = uiButton.Working ? nil : "working";
         uiButton.Message(uiButton.Working ? "Working" : "Not Working");
      };
      uiButton.ClickText = "Toggle working";
      form.ShowDialog();
   }

   [TestMethod]
   public void UiShowFocusTest()
   {
      var form = new Form();
      var textBox = new TextBox();
      form.Controls.Add(textBox);
      textBox.SetUp(0, 60, 200, 40);
      var uiButton = new UiAction() { ShowFocus = true };
      uiButton.SetUp(0, 0, 200, 40);
      uiButton.Message("Unfocused");
      uiButton.GotFocus += (_, _) => uiButton.Message("Focused");
      uiButton.LostFocus += (_, _) => uiButton.Message("Unfocused");
      uiButton.Click += (_, _) => { };
      uiButton.ClickText = "Does nothing";
      form.ShowDialog();
   }

   [TestMethod]
   public void DelayedButtonTest()
   {
      var form = new Form();
      var uiButton = new UiAction();
      uiButton.SetUp(0, 0, 200, 40);
      uiButton.Message("Not Working");
      uiButton.Click += (_, _) =>
      {
         uiButton.Working = uiButton.Working ? nil : "working";
         uiButton.Message(uiButton.Working ? "Working" : "Not Working");
         uiButton.SuccessLegendTemp("saved");
      };
      uiButton.ClickText = "Toggle working";
      form.ShowDialog();
   }

   [TestMethod]
   public void ValidateTest()
   {
      var form = new Form();
      var checkBox = new CheckBox { Text = "Toggle" };
      form.Controls.Add(checkBox);
      checkBox.SetUp(0, 0, 100, 40);
      var uiButton = new UiAction();
      uiButton.SetUp(0, 50, 200, 40);
      uiButton.ValidateText += (_, e) => e.Type = checkBox.Checked ? UiActionType.Success : UiActionType.Failure;
      uiButton.Uninitialized("toggled");
      checkBox.Click += (_, _) => uiButton.Validate("Toggled");
      form.ShowDialog();
   }

   [TestMethod]
   public void BottomDockTest()
   {
      var form = new Form();
      var panel = new Panel();
      form.Controls.Add(panel);
      panel.Location = new Point(0, 0);
      panel.Size = new Size(200, 200);

      var uiAction = new UiAction();
      uiAction.SetUpInPanel(panel, dockStyle: DockStyle.Bottom);
      uiAction.Height = 40;
      uiAction.Message("Testing");

      var label = new UiAction();
      panel.Controls.Add(label);
      label.AttachTo("label", uiAction);

      form.ShowDialog();
   }

   [TestMethod]
   public void EmptyTextTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 200, 40);
      uiAction.Failure("");
      form.ShowDialog();
      Console.WriteLine($"<{uiAction.Text}>");
   }

   [TestMethod]
   public void DirtyUiActionTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 200, 40);
      uiAction.Message("This is dirty");
      uiAction.IsDirty = true;
      uiAction.ClickText = "Click";
      form.ShowDialog();
   }

   [TestMethod]
   public void ChooserTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 200, 40);
      uiAction.Click += (_, _) => _ = uiAction.Choose("A,B,C").SizeToText(true).Choices("Alpha", "Bravo", "Charlie").Choose();
      uiAction.ClickText = "Select item";

      form.ShowDialog();
   }

   [TestMethod]
   public void Chooser2Test()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 200, 40);
      uiAction.Click += (_, _) =>
      {
         var _chosen = uiAction.Choose("A,B,C").Choices("Alpha", "Bravo", "Charlie").ModifyTitle(false).NilItem(nil).Choose();
         if (_chosen is (true, var chosen))
         {
            MessageBox.Show(chosen.Value);
         }
      };
      uiAction.ClickText = "Select item";

      form.ShowDialog();
   }

   [TestMethod]
   public void Chooser3Test()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 200, 40);
      uiAction.Click += (_, _) => _ = uiAction.Choose("A,B,C").Choices(("Alpha", "A"), ("Bravo", "B"), ("Charlie", "C")).Choose();
      uiAction.ClickText = "Select item";

      form.ShowDialog();
   }

   [TestMethod]
   public void Chooser4Test()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 200, 40);
      uiAction.Click += (_, _) => _ = uiAction.Choose("A,B,C", 800).Choices(("Alpha", "A"), ("Bravo", "B"), ("Charlie", "C")).Choose();
      uiAction.ClickText = "Select item";

      form.ShowDialog();
   }

   [TestMethod]
   public void ChooserEventTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 200, 40);
      uiAction.AppearanceOverride += (_, e) =>
      {
         e.ForeColor = Color.White;
         e.BackColor = Color.Red;
         e.Italic = true;
         e.Override = true;
      };
      uiAction.Click += (_, _) => _ = uiAction.Choose("A,B,C", 800).Choices(("Alpha", "A"), ("Bravo", "B"), ("Charlie", "C")).Choose();
      uiAction.ClickText = "Select item";

      form.ShowDialog();
   }

   [TestMethod]
   public void HttpTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 200, 60);
      uiAction.TextChanged += (_, _) => form.Text = uiAction.Text;
      uiAction.Http("http://google.com");
      form.ShowDialog();
   }

   [TestMethod]
   public void UiActionCornersTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 400, 80);
      uiAction.Message("Test");
      uiAction.SubText("UL").Set.GoToUpperLeft(8).Outline();
      uiAction.SubText("UR").Set.GoToUpperRight(8).Outline();
      uiAction.SubText("LL").Set.GoToLowerLeft(8).Outline();
      uiAction.SubText("LR").Set.GoToLowerRight(8).Outline();
      uiAction.SubText("ML").Set.GoToMiddleLeft(8).Outline();
      uiAction.SubText("MR").Set.GoToMiddleRight(8).Outline();
      form.ShowDialog();
   }

   [TestMethod]
   public void UiActionConsoleTest()
   {
      var pause = 500.Milliseconds();

      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 600, 500);
      uiAction.Click += (_, _) => uiAction.RunWorkerAsync();
      uiAction.ClickText = "Start test";
      uiAction.DoWork += (_, _) =>
      {
         for (var i = 0; i < 50; i++)
         {
            uiAction.Do(() => uiAction.WriteLine($"Line {i}"));
            Thread.Sleep(pause);
         }
      };
      form.ShowDialog();
   }

   [TestMethod]
   public void UiActionConsole2Test()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 600, 500);

      form.Show();
      for (var i = 0; i < 1000; i++)
      {
         uiAction.Do(() => uiAction.WriteLine($"Line {i}"));
         Application.DoEvents();
      }
   }

   [TestMethod]
   public void DisabledUiActionTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 400, 40);
      uiAction.Button("Test");
      uiAction.Enabled = false;
      form.ShowDialog();
   }

   protected class UiActionClickStore : EventHandlerStore<EventArgs>
   {
      public UiActionClickStore(Control control) : base(control, "Click")
      {
      }

      public override void Handler(object sender, EventArgs e)
      {
         var uiAction = (UiAction)sender;
         uiAction.Success(uniqueID());
         Unsubscribe();
      }
   }

   [TestMethod]
   public void EventHandlerStoreTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 400, 40);
      uiAction.Button("Test");
      uiAction.ClickText = "Test";

      var store = new UiActionClickStore(uiAction);
      store.Subscribe();
      form.ShowDialog();
   }

   [TestMethod]
   public void LegendChangingTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 400, 40);
      uiAction.Uninitialized("uninitialized");
      uiAction.Legend("pushable");
      uiAction.Click += (_, _) => uiAction.Failure("Failed");
      uiAction.ClickText = "test";

      form.ShowDialog();
   }


   [TestMethod]
   public void FloatingFailureTest()
   {
      var form = new Form();
      var uiAction = new UiAction();
      uiAction.SetUp(0, 0, 400, 40);

      uiAction.Button("Show Floating Failure");
      uiAction.Click += (_, _) => uiAction.FloatingFailure("Failed!");

      form.ShowDialog();
   }
}