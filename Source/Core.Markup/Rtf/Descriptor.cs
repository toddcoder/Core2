namespace Core.Markup.Rtf;

public class Descriptor(int descriptor)
{
   protected int descriptor = descriptor;

   public int Value => descriptor;
}