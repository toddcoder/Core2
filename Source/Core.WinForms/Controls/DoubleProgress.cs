namespace Core.WinForms.Controls;

public partial class DoubleProgress : UserControl
{
   protected UiAction uiTop = new() { Visible = false, Padding = new Padding(0), Margin = new Padding(0) };
   protected UiAction uiBottom = new() { Visible = false, Padding = new Padding(0), Margin = new Padding(0) };

   public DoubleProgress()
   {
      InitializeComponent();

      Controls.Add(uiTop);
      Controls.Add(uiBottom);

      arrange();
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

   public void BusyOnBottom(string text)
   {
      uiBottom.Busy(text);
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

   protected void arrange()
   {
      var halfHeight = ClientRectangle.Height / 2;

      uiTop.Location = Point.Empty;
      uiTop.Width = ClientRectangle.Width;
      uiTop.Height = halfHeight;

      uiBottom.Location = new Point(0, halfHeight + 1);
      uiBottom.Width = ClientRectangle.Width;
      uiBottom.Height = halfHeight;
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);
      arrange();
   }
}