namespace Core.WinForms.TableLayoutPanels;

using BuilderSetup = (Setup setup, Axis axis, float amount);

public static class BuilderFunctions
{
   public static readonly Terminator control = Terminator.Control;

   public static readonly Terminator setup = Terminator.Setup;

   public static readonly Terminator row = Terminator.Row;

   public static readonly Terminator down = Terminator.Down;

   public static readonly Terminator skip = Terminator.Skip;

   public static BuilderSetup colAutoSize() => (Setup.AutoSize, Axis.Column, 0);

   public static BuilderSetup rowAutoSize() => (Setup.AutoSize, Axis.Row, 0);
}