namespace Core.WinForms.Controls
{
   public partial class DoubleProgress : UserControl
   {
      protected DoubleProgressWriter writer = DoubleProgressWriter.Empty;

      public DoubleProgress()
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

      public void Done()
      {
         writer.Status = new DoubleProgressStatus.Idle();
         Invalidate();
      }

      public void Failure(string message)
      {
         writer.Status = new DoubleProgressStatus.Failure(message);
         Invalidate();
      }

      public void Exception(Exception exception)
      {
         writer.Status = new DoubleProgressStatus.Error(exception);
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