using Core.Enumerables;
using Core.Enums;
using Core.Matching;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form5 : Form
{
   protected ExTextBox textBox1 = new() { BorderStyle = BorderStyle.None };
   protected ExTextBox textBox2 = new() { BorderStyle = BorderStyle.None, RefreshOnTextChange = true };
   protected UiAction uiAction1 = new();
   protected UiAction uiAction2 = new();
   protected UiAction uiAction3 = new();
   protected UiAction uiAction4 = new();
   protected Header header = new();
   protected ControlContainer<TextBox> textBoxContainer = ControlContainer<TextBox>.HorizontalContainer();

   public Form5()
   {
      textBox2.ReassignHandle();
      textBox2.Text = "Smith, John; Doe, Jane; Johnson, Emily";
      textBox2.Triggered.Handler = _ =>
      {
         var text = textBox2.Text;
         List<string> names = [];
         var anyToResolve = false;
         foreach (var fullName in text.Unjoin("/s* ';' /s*; f"))
         {
            var strippedName = fullName;
            if (strippedName.StartsWith('['))
            {
               strippedName = strippedName.Drop(1).Drop(-1);
            }
            else
            {
               anyToResolve = true;
            }

            var trimmedName = strippedName.Trim().NormalizeWhitespace();
            if (trimmedName.Matches("^/(-[',']+) ',' /(.+)$; f") is (true, var result))
            {
               var reorganizedName = $"[{fixedText(result.SecondGroup)} {fixedText(result.FirstGroup)}]";
               names.Add(reorganizedName);
            }
            else
            {
               names.Add($"[{trimmedName}]");
            }
         }

         if (anyToResolve)
         {
            textBox2.Text = names.ToString("; ");
         }

         return;

         string fixedText(string source) => source.Trim().NormalizeWhitespace().ToTitleCase();
      };
      textBox2.Paint += (_, e) =>
      {
         using var brush = new SolidBrush(Color.Cyan.WithAlpha(64));
         using var pen = new Pen(Color.Black);
         foreach (var rectangle in getRectangles())
         {
            e.Graphics.FillRectangle(brush, rectangle);
            e.Graphics.DrawRectangle(pen, rectangle);
         }

         return;

         IEnumerable<Rectangle> getRectangles()
         {
            foreach (var match in textBox2.Text.AllMatches("'[' /(-[']']+) ']'; f"))
            {
               var index = match.Groups[1].Index;
               var length = match.Groups[1].Length;
               yield return textBox2.RectangleFrom(e.Graphics, index, length);
            }
         }
      };
      textBox2.Idle = 2;

      uiAction1.Message("Starts with digits");

      uiAction2.Message("Two digits separated by colon");

      uiAction3.NoStatus("Digits only");

      uiAction4.Message("No digits");

      var enabler = new Enabler(textBox1)
      {
         ["u1"] = uiAction1,
         ["u2"] = uiAction2,
         ["u3"] = uiAction3,
         ["u4"] = uiAction4
      };

      enabler.HookTextChanged();
      enabler.Enable.Handler = p =>
      {
         if (p.EventTriggered is EventTriggered.TextChanged textChanged)
         {
            p.Enabled = p.Key switch
            {
               "u1" => textChanged.Text.IsMatch("^ /d+; f"),
               "u2" => textChanged.Text.IsMatch("^ /d+ ':' /d+; f"),
               "u3" => textChanged.Text.IsMatch("^ /d+ $; f"),
               "u4" => textChanged.Text.IsMatch("^ -/d+ $; f"),
               _ => p.Enabled
            };
         }
      };

      var columnSize = new ColumnSize.Percent(33.33f);
      (header + "a" + "alpha" + columnSize).HeaderColumn();
      (header + "b" + "bravo" + columnSize).HeaderColumn();
      (header + "c" + "charlie" + columnSize).HeaderColumn();
      header.Arrange();
      header.HeaderClick.Handler = p => Text = $"{p.Name}: {p.UiAction.Text}";
      header.ZeroOut();

      header.HeaderColumns["a"].Text = "alfa";
      header.Arrange();

      textBoxContainer.BeginUpdate();
      textBoxContainer.ShowLastFocus = false;
      foreach (var _ in Enumerable.Range(0, 3))
      {
         var control = new TextBox { Text = "" };
         control.ZeroOut();
         textBoxContainer.Add(control);
      }

      textBoxContainer.EndUpdate();

      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel1);
      _ = builder.Col + 50f + 50f;
      _ = builder.Row + 40 + 60 + 60 + 30 + 60 + 100f;
      builder.SetUp();

      (builder + textBox1).Next();
      (builder + textBox2).Row();

      (builder + uiAction1).Next();
      (builder + uiAction2).Row();

      (builder + uiAction3).Next();
      (builder + uiAction4).Row();

      (builder + header).SpanCol(2).Row();

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
            ForeColor = Color.Black,
            BackColor = Color.Bisque,
            BackgroundRestriction = new BackgroundRestriction.Restricted(alignment)
         };
         writer.Write(e.Graphics);
      }
   }

   protected void panel1_Click(object sender, EventArgs e)
   {
   }

   protected void pictureBox1_Click(object sender, EventArgs e)
   {
      textBoxContainer.BeginUpdate();
      textBoxContainer.Clear();
      textBoxContainer.ShowLastFocus = false;
      foreach (var _ in Enumerable.Range(0, 3))
      {
         var control = new TextBox { Text = "" };
         textBoxContainer.Add(control);
      }

      textBoxContainer.EndUpdate();
   }
}