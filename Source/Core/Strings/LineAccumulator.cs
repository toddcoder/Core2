using System.Text;

namespace Core.Strings;

public class LineAccumulator
{
   public static LineAccumulator operator +(LineAccumulator accumulator, string line) => accumulator.Add(line);

   protected StringBuilder builder = new();

   public LineAccumulator Add(string line)
   {
      builder.AppendLine(line);
      return this;
   }

   public override string ToString() => builder.ToString();
}