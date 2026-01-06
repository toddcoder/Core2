using System.Text;

namespace Core.Quirk;

public class Parser(string input)
{
   protected string[] lines = input.Split(["\r\n", "\n", "\r"], StringSplitOptions.TrimEntries);

   public IEnumerable<TagInfo> Parse()
   {
      var builder = new StringBuilder();
      foreach (var line in lines)
      {
         
      }
   }
}