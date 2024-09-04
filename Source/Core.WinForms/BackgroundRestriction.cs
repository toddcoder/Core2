using Core.WinForms.Controls;

namespace Core.WinForms;

public abstract record BackgroundRestriction()
{
   public sealed record Fill : BackgroundRestriction;

   public sealed record Restricted(CardinalAlignment Alignment, int XMargin = 0, int YMargin = 0): BackgroundRestriction;

   public sealed record UseWriterAlignment(int XMargin = 0, int YMargin = 0) : BackgroundRestriction;
}