using Core.Monads;

namespace Core.Markup.Rtf.Markup.Parsers;

public class Parser(PaperSize paperSize = PaperSize.Letter, PaperOrientation paperOrientation = PaperOrientation.Portrait, Lcid lcid = Lcid.English)
{
   protected Document document = new(paperSize, paperOrientation, lcid);

   public Result<string> Parse(string source)
   {
      try
      {
         return "";
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}