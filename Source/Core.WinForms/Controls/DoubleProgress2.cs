namespace Core.WinForms.Controls
{
   public partial class DoubleProgress2 : UserControl
   {
      protected DoubleProgressWriter writer = DoubleProgressWriter.Empty;

      public DoubleProgress2()
      {
         InitializeComponent();
      }

      public int OuterMaximum
      {
         get => writer.OuterMaximum;
         set => writer.OuterMaximum = value;
      }

      public void AdvanceOuter(string outerText, int innerMaximum)
      {
         writer.AdvanceOuter(outerText, innerMaximum);
         Invalidate();
      }

      public void AdvanceInner(string innerText)
      {
         writer.AdvanceInner(innerText);
         Invalidate();
      }

      protected override void OnResize(EventArgs e)
      {
         base.OnResize(e);

         writer.OnResize(ClientRectangle);

         Invalidate();
      }

      protected override void OnPaint(PaintEventArgs e)
      {
         base.OnPaint(e);

         writer.OnPaint(e);
      }
   }
}