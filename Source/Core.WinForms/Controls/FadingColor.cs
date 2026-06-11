namespace Core.WinForms.Controls;

public struct FadingColor(Color color)
{
   public static implicit operator FadingColor(Color color) => new(color);

   public static implicit operator Color(FadingColor fadingColor) => fadingColor.Color;

   private int alpha = 255;

   public void Fade(int amount = 5) => alpha = Math.Max(0, alpha - amount);

   public void Reset() => alpha = 255;

   public Color Color => color.WithAlpha(alpha);

   public bool IsFading => alpha > 0;
}