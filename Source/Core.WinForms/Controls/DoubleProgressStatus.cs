namespace Core.WinForms.Controls;

public abstract record DoubleProgressStatus
{
   public sealed record Idle : DoubleProgressStatus
   {
      public override Color ForeColor => Color.LightGray;

      public override Color BackColor => Color.LightGray;
   }

   public sealed record Running : DoubleProgressStatus
   {
      public override Color ForeColor => Color.LightSteelBlue;

      public override Color BackColor => Color.Coral;
   }

   public sealed record Failure(string Message) : DoubleProgressStatus
   {
      public override Color ForeColor => Color.Black;

      public override Color BackColor => Color.Yellow;
   }

   public sealed record Error(Exception Exception) : DoubleProgressStatus
   {
      public override Color ForeColor => Color.White;

      public override Color BackColor => Color.Red;
   }

   public abstract Color ForeColor { get; }

   public abstract Color BackColor { get; }
}