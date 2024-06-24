namespace Core.Markup.Rtf;

public class PartialFormatter(Paragraph paragraph, CharFormat format, int begin, int end) : Formatter(paragraph, format)
{
   public int Begin => begin;

   public int End => end;

   public override Formatter Hyperlink(Hyperlink hyperlink)
   {
      paragraph.AddPendingHyperlink(hyperlink);
      return this;
   }
}