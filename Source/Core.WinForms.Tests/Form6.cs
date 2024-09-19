using Core.Enumerables;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form6 : Form
{
   protected UiAction uiTextDivider = new();
   protected ExTextBox textBox = new() { BorderStyle = BorderStyle.None };
   protected CoreDateTimePicker picker = new();
   protected UiAction uiAlternates = new();

   public Form6()
   {
      InitializeComponent();

      uiTextDivider.Divider("Text");

      textBox.TextChanged += (_, _) =>
      {
         var text = textBox.Text;
         uiTextDivider.DividerValidation = new DividerValidation.None();
         _ = uiTextDivider & (text.IsNotEmpty(), "text is empty") & (text is "alfa" or "bravo", "Expected alfa or bravo");
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col * 100f;
      _ = builder.Row + 40 + 40 + 60 + +60 + 100f;
      builder.SetUp();

      (builder + uiTextDivider + false).Row();

      (builder + textBox).Row();

      (builder + picker).Row();

      (builder + uiAlternates).Row();

      uiAlternates.RectangleCount = 6;
      uiAlternates.PaintOnRectangle += (_, e) =>
      {
         var text = e.RectangleIndex switch
         {
            0 => "alfa",
            1 => "bravo",
            2 => "charlie",
            3 => "delta",
            4 => "echo",
            5 => "foxtrot",
            _ => ""
         };
         var writer = new RectangleWriter(text, uiAlternates.Rectangles[e.RectangleIndex])
         {
            ForeColor = Color.White, BackColor = Color.Blue, Outline = true
         };
         writer.Write(e.Graphics);
         writer = new RectangleWriter(text.Keep(1), uiAlternates.Rectangles[e.RectangleIndex])
         {
            ForeColor = Color.Blue, BackColor = Color.White, BackgroundRestriction = new BackgroundRestriction.Restricted(CardinalAlignment.NorthWest)
         };
         writer.Write(e.Graphics);
      };

      foreach (var (index, letter) in getLetters().Indexed())
      {
         uiAlternates.SubText(letter, index, 0, 0);
      }

      uiAlternates.Refresh();
   }

   protected static IEnumerable<string> getLetters()
   {
      yield return "a";
      yield return "b";
      yield return "c";
      yield return "d";
      yield return "e";
      yield return "f";
   }
}