using Core.Matching;
using Core.Monads;
using Core.Strings;

namespace Core.Markdown;

public class StylesParser(string styles)
{
   public Optional<string> Parse()
   {
      try
      {
         using var writer = new StringWriter();

         writer.WriteLine("<html>");
         writer.WriteLine("<head>");
         writer.WriteLine("<meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" charset=\"utf-u\" />");
         writer.WriteLine("<style>");

         foreach (var line in styles.Lines())
         {
            if (line.Matches("^ /(-['[']+) '[' /(-[']']+) ']' $; f") is (true, var result))
            {
               var selector = result.FirstGroup;
               var specifiers = result.SecondGroup;

               writer.WriteLine($"  {selector} {{");

               if (specifiers.Matches("/(-['(']+) '(' /(-[')']+) ')'; f") is (true, var specifiersResult))
               {
                  foreach (var match in specifiersResult)
                  {
                     var name = match.FirstGroup;
                     var content = match.SecondGroup;
                     writer.WriteLine($"    {name}: {content};");
                  }
               }

               writer.WriteLine("  }");
            }
         }

         writer.WriteLine("</style>");
         writer.WriteLine("</head>");
         writer.WriteLine("<body />");
         writer.WriteLine("</html>");

         return writer.ToString();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}