﻿using Core.Enums;
using Core.Matching;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form5 : Form
{
   protected ExTextBox textBox = new() { BorderStyle = BorderStyle.None };
   protected UiAction uiAction1 = new();
   protected UiAction uiAction2 = new();
   protected UiAction uiAction3 = new();
   protected UiAction uiAction4 = new();
   protected ControlContainer<TextBox> textBoxContainer = ControlContainer<TextBox>.HorizontalContainer();

   public Form5()
   {
      uiAction1.Message("Starts with digits");

      uiAction2.Message("Two digits separated by color");

      uiAction3.NoStatus("Digits only");

      uiAction4.Message("No digits");

      var enabler = new Enabler(textBox)
      {
         ["u1"] = uiAction1,
         ["u2"] = uiAction2,
         ["u3"] = uiAction3,
         ["u4"] = uiAction4
      };

      enabler.HookTextChanged();
      enabler.Enable += (_, e) =>
      {
         if (e.EventTriggered is EventTriggered.TextChanged textChanged)
         {
            e.Enabled = e.Key switch
            {
               "u1" => textChanged.Text.IsMatch("^ /d+; f"),
               "u2" => textChanged.Text.IsMatch("^ /d+ ':' /d+; f"),
               "u3" => textChanged.Text.IsMatch("^ /d+ $; f"),
               "u4" => textChanged.Text.IsMatch("^ -/d+ $; f"),
               _ => e.Enabled
            };
         }
      };

      foreach (var _ in Enumerable.Range(0, 3))
      {
         var control = new TextBox { Text = "" };
         textBoxContainer.Add(control);
      }

      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel1);
      _ = builder.Col + 50f + 50f;
      _ = builder.Row + 40 + 60 + 60 + 60 + 100f;
      builder.SetUp();

      (builder + textBox).Row();

      (builder + uiAction1).Next();
      (builder + uiAction2).Row();

      (builder + uiAction3).Next();
      (builder + uiAction4).Row();

      (builder + textBoxContainer).SpanCol(2).Row();

      (builder + pictureBox1).SpanCol(2).Row();
   }

   protected void pictureBox1_Paint(object sender, PaintEventArgs e)
   {
      /*var rectangle = new Rectangle(0, 0, 100, 40);
      var writer = new RectangleWriter("Location 1", rectangle) { BackColor = Color.White };
      writer.Write(e.Graphics);

      rectangle = rectangle.RightOf(rectangle, 10);
      writer = new RectangleWriter("Location 2", rectangle) { BackColor = Color.Green, ForeColor = Color.White };
      writer.Write(e.Graphics);

      rectangle = rectangle.BottomOf(rectangle, 10).Resize(100, 0);
      writer = new RectangleWriter("Underneath the second one", rectangle) { ForeColor = Color.White, BackColor = Color.Red, Outline = true };
      writer.Write(e.Graphics);*/

      var rectangle = e.ClipRectangle;
      foreach (var alignment in EnumFunctions.enumEnumerable<CardinalAlignment>())
      {
         var writer = new RectangleWriter(alignment.ToString(), rectangle, alignment)
         {
            ForeColor = Color.Black, BackColor = Color.Bisque, BackgroundRestriction = new BackgroundRestriction.Restricted(alignment)
         };
         writer.Write(e.Graphics);
      }
   }
}