namespace Core.WinForms.Controls;

public abstract record ColumnSize
{
   public sealed record Percent(float Amount) : ColumnSize;

   public sealed record Absolute(int Amount) : ColumnSize;
}