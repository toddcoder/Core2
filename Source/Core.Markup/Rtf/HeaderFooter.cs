using System.Text;

namespace Core.Markup.Rtf;

public class HeaderFooter(HeaderFooterType type) : BlockList(true, false, true, true, false)
{
   protected HeaderFooterType type = type;

   public override string Render()
   {
      var result = new StringBuilder();

      switch (type)
      {
         case HeaderFooterType.Header:
            result.AppendLine(@"{\header");
            break;
         case HeaderFooterType.Footer:
            result.AppendLine(@"{\footer");
            break;
         default:
            throw new Exception("Invalid HeaderFooterType");
      }

      result.AppendLine();

      foreach (var block in blocks)
      {
         block.DefaultCharFormat.CopyFrom(defaultCharFormat);
         result.AppendLine(block.Render());
      }

      result.AppendLine("}");
      return result.ToString();
   }
}