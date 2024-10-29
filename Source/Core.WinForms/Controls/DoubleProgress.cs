using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Controls;

public partial class DoubleProgress : UserControl
{
   protected UiAction uiTop = new() { Visible = false };
   protected UiAction uiBottom = new() { Visible = false };

   public DoubleProgress()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row * 2 * 50f;
      builder.SetUp();

      (builder + uiTop).Row();
      (builder + uiBottom).Row();
   }

   public int TopMaximum
   {
      get => uiTop.Maximum;
      set => uiTop.Maximum = value;
   }

   public int BottomMaximum
   {
      get => uiBottom.Maximum;
      set => uiBottom.Maximum = value;
   }

   protected static void visible(UiAction uiAction)
   {
      if (!uiAction.Visible)
      {
         uiAction.Visible = true;
      }
   }

   protected void topVisible() => visible(uiTop);

   protected void bottomVisible() => visible(uiBottom);

   public void Progress(string topText, string bottomText)
   {
      if (topText != uiTop.NonNullText)
      {
         uiTop.Progress(topText);
         topVisible();
      }

      uiBottom.Progress(bottomText);
      bottomVisible();
   }

   public void ProgressOnTop(string text)
   {
      uiTop.Progress(text);
      topVisible();
   }

   public void ProgressOnBottom(string text)
   {
      uiBottom.Progress(text);
      bottomVisible();
   }

   public void Done()
   {
      uiTop.Visible = false;
      uiBottom.Visible = false;
   }

   public void ResetTop() => uiTop.ResetProgress();

   public void ResetBottom() => uiBottom.ResetProgress();

   public void Reset()
   {
      ResetTop();
      ResetBottom();
   }
}