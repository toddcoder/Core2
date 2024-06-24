using static Core.Objects.GetHashCodeGenerator;

namespace Core.Markup.Rtf;

public class Border
{
   private BorderStyle style = BorderStyle.None;
   private float width = 0.5F;
   private ColorDescriptor colorDescriptor = new(0);

   public override bool Equals(object? obj) => obj is Border border && Style == border.Style && Width == border.Width;

   public override int GetHashCode() => hashCode() + width + style;

   public BorderStyle Style
   {
      get => style;
      set => style = value;
   }

   public float Width
   {
      get => width;
      set => width = value;
   }

   public ColorDescriptor Color
   {
      get => colorDescriptor;
      set => colorDescriptor = value;
   }
}