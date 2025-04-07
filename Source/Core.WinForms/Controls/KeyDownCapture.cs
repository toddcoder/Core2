using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public abstract record KeyDownCapture
{
   public sealed record None : KeyDownCapture
   {
      public override bool IsDown => false;

      public override string Representation => "";
   }

   public sealed record ControlKey(string Caption) : KeyDownCapture
   {
      public override bool IsDown => Control.ModifierKeys == Keys.Control;

      public override string Representation => "Ctl";
   }

   public sealed record ShiftKey(string Caption) : KeyDownCapture
   {
      public override bool IsDown => Control.ModifierKeys == Keys.Shift;

      public override string Representation => "Shft";
   }

   public sealed record AltKey(string Caption) : KeyDownCapture
   {
      public override bool IsDown => Control.ModifierKeys == Keys.Alt;

      public override string Representation => "Alt";
   }

   public abstract bool IsDown { get; }

   public abstract string Representation { get; }

   public Maybe<SubText> SubText { get; set; } = nil;
}