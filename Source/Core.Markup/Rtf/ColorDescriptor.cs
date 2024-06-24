namespace Core.Markup.Rtf;

public class ColorDescriptor(int descriptor) : Descriptor(descriptor)
{
   public ForegroundColorDescriptor Foreground => new(descriptor);

   public BackgroundColorDescriptor Background => new(descriptor);
}