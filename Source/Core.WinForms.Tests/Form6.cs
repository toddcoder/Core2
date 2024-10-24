using Core.Applications;
using Core.Enumerables;
using Core.Strings;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form6 : Form
{
   protected Image image;
   protected PictureBox pictureBox = new();
   protected LabelText ltText = new("text") { CanDirty = false };
   protected CoreDateTimePicker picker = new();
   protected UiAction uiAlternates = new();
   protected UiAction uiReadOnlyAlternates = new();
   protected UiAction uiApplication = new();
   protected UiAction uiDivider = new();

   public Form6()
   {
      var resources = new Resources<Form6>();
      using var stream = resources.Stream("Application.png");
      image = Image.FromStream(stream);

      InitializeComponent();

      ltText.TextChanged += (_, _) =>
      {
         var text = ltText.Text;
         ltText.Label.DividerValidation = new DividerValidation.None();
         _ = ltText.Label & (text.IsNotEmpty(), "text is empty") & (text is "alfa" or "bravo", "Expected alfa or bravo");
      };

      var uiBusy = new UiAction();
      uiBusy.Message("busy");
      uiBusy.Click += (_, _) => uiBusy.Busy(true);
      uiBusy.ClickText = "Set to busy";

      var uiAlfa = new UiAction();
      uiAlfa.Button("alfa");
      uiAlfa.Click += (_, _) => ltText.TextBox.SelectedText = "alfa";
      uiAlfa.ClickText = "Insert alfa";

      var uiBravo = new UiAction();
      uiBravo.Button("bravo");
      uiBravo.Click += (_, _) => ltText.TextBox.SelectedText = "bravo";
      uiBravo.ClickText = "Insert bravo";

      ltText.AddUiActions(uiBusy, uiAlfa, uiBravo);

      uiDivider.Divider("Divider");
      uiDivider.DividerMessage("bad!", UiActionType.Failure);

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 32 + 100f;
      _ = builder.Row + 80 + 60 + 60 + 60 + 32 + 32 + 60 + 100f;
      builder.SetUp();

      (builder + ltText).SpanCol(2).Row();

      (builder + picker).SpanCol(2).Row();

      (builder + uiAlternates).SpanCol(2).Row();

      uiReadOnlyAlternates.Dock = DockStyle.Fill;
      (builder + uiReadOnlyAlternates).SpanCol(2).Row();

      (builder + uiApplication).Row();

      (builder + pictureBox).Row();

      (builder + uiDivider).SpanCol(2).Row();

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

      uiReadOnlyAlternates.BeginUpdate();
      uiReadOnlyAlternates.AlternateReadOnly("alfa", "bravo", "charlie", "delta", "echo", "foxtrot");
      uiReadOnlyAlternates.SetColors(UiActionType.Message);
      uiReadOnlyAlternates.Refresh();
      uiReadOnlyAlternates.EndUpdate();

      uiApplication.StretchImage = true;
      uiApplication.Image = image;
      uiApplication.Refresh();

      pictureBox.Image = image;
      pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
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