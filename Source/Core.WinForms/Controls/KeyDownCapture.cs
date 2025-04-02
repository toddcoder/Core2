namespace Core.WinForms.Controls;

public abstract record KeyDownCapture
{
   public sealed record None : KeyDownCapture
   {
      public override bool IsDown => false;
   }

   public sealed record ControlKey(string Caption) : KeyDownCapture
   {
      public override bool IsDown => Control.ModifierKeys == Keys.Control;
   }

   public sealed record ShiftKey(string Caption) : KeyDownCapture
   {
      public override bool IsDown => Control.ModifierKeys == Keys.Shift;
   }

   public sealed record AltKey(string Caption) : KeyDownCapture
   {
      public override bool IsDown => Control.ModifierKeys == Keys.Alt;
   }

   public abstract bool IsDown { get; }
}