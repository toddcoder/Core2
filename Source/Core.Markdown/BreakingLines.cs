using Core.Monads;

namespace Core.Markdown;

public class BreakingLines(Maybe<int> _pageBreak)
{
   protected List<string> lines = [];

   public void Add(string line)
   {
      lines.Add(line);

      if (_pageBreak is (true, var pageBreak) && lines.Count % pageBreak == 0)
      {
         lines.Add("""<div style="page-break-after: always;"></div><br></br>""");
      }
   }

   public void AddRange(IEnumerable<string> strings)
   {
      foreach (var line in strings)
      {
         Add(line);
      }
   }

   public override string ToString() => string.Join(Environment.NewLine, lines);
}