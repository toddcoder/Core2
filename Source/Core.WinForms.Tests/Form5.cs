using Core.Enums;
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
   protected Header header = new();
   protected ControlContainer<TextBox> textBoxContainer = ControlContainer<TextBox>.HorizontalContainer();

   public Form5()
   {
      uiAction1.Message("Starts with digits");

      uiAction2.Message("Two digits separated by colon");

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

      textBoxContainer.BeginUpdate();
      textBoxContainer.ShowLastFocus = false;
      foreach (var _ in Enumerable.Range(0, 3))
      {
         var control = new TextBox { Text = "" };
         textBoxContainer.Add(control);
      }

      textBoxContainer.EndUpdate();

      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel1);
      _ = builder.Col + 50f + 50f;
      _ = builder.Row + 40 + 60 + 60 + 30 + 60 + 100f;
      builder.SetUp();

      (builder + textBox).Row();

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