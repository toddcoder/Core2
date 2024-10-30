﻿using Core.Monads;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public partial class Form7 : Form
{
   protected TestCycle cycle = new();
   protected UiAction uiAlignment = new();
   protected UiAction uiOne = new();
   protected UiAction uiTwo = new();
   protected UiAction uiThree = new();
   protected RectangleRow rectangleRow;
   protected Maybe<SubText> _one = nil;
   protected Maybe<SubText> _two = nil;
   protected Maybe<SubText> _three = nil;
   protected DoubleProgress dpProgress = new();

   public Form7()
   {
      InitializeComponent();

      uiAlignment.Alternate("Left", "Right", "Center", "Spread");
      uiAlignment.ClickOnAlternate += (_, e) =>
      {
         rectangleRow!.Alignment = e.RectangleIndex switch
         {
            0 => RectangleAlignment.Left,
            1 => RectangleAlignment.Right,
            2 => RectangleAlignment.Center,
            3 => RectangleAlignment.Spread,
            _ => rectangleRow.Alignment
         };
         rearrange();
      };

      uiOne.Message("one");
      panel.Controls.Add(uiOne);
      uiOne.Size = new Size(200, 60);
      uiTwo.Message("two");
      panel.Controls.Add(uiTwo);
      uiTwo.Size = new Size(200, 60);
      uiThree.Message("three");
      panel.Controls.Add(uiThree);
      uiThree.Size = new Size(200, 60);

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 80 + 100 + 80 + 100f;
      builder.SetUp();

      (builder + uiAlignment).Row();
      (builder + panel).Row();
      (builder + dpProgress).Row();

      rectangleRow = new RectangleRow(panel.ClientRectangle);
      rectangleRow.BeginUpdate();
      rectangleRow.Add(uiOne.ClientRectangle);
      rectangleRow.Add(uiTwo.ClientRectangle);
      rectangleRow.Add(uiThree.ClientRectangle);
      rectangleRow.EndUpdate();
      panel.BackColor = Color.RosyBrown;

      rearrange();

      dpProgress.TopMaximum = cycle.OuterLength;
      dpProgress.BottomMaximum = cycle.InnerLength;
   }

   protected void rearrange()
   {
      var queue = new Queue<Rectangle>(rectangleRow);

      var rectangle = queue.Dequeue();
      uiOne.Location = rectangle.Location;
      uiOne.RemoveSubText(_one);
      _one = uiOne.SubText(rectangle.ToString()).Set.MiniInverted().SubText;

      rectangle = queue.Dequeue();
      uiTwo.Location = rectangle.Location;
      uiTwo.RemoveSubText(_two);
      _two = uiTwo.SubText(rectangle.ToString()).Set.MiniInverted().SubText;

      rectangle = queue.Dequeue();
      uiThree.Location = rectangle.Location;
      uiThree.RemoveSubText(_three);
      _three = uiThree.SubText(rectangle.ToString()).Set.MiniInverted().SubText;
   }

   protected void Form7_Load(object sender, EventArgs e)
   {
      Show();
      Application.DoEvents();

      timer.Enabled = true;
   }

   protected void timer_Tick(object sender, EventArgs e)
   {
      var _next = cycle.Next();
      if (_next is (true, var (outer, inner, reset)))
      {
         if (reset)
         {
            dpProgress.ResetBottom();
         }
         dpProgress.Progress(outer, inner);
      }
      else
      {
         dpProgress.Done();
         timer.Enabled = false;
      }
   }
}