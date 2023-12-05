using System.Collections;
using Core.Collections;

namespace Core.WinForms.Controls;

public class Spans : IEnumerable<Spans.SpanLocation>
{
   protected record Coordinate(int StartColumn, int StopColumn, int StartRow, int StopRow);

   public record SpanLocation(int Control, int X, int ColumnSpan, int Y, int RowSpan);

   protected Hash<int, Coordinate> coordinates = new();
   protected int index;

   public void Add(int columnSpan = 1, int rowSpan = 1)
   {
      if (columnSpan <= 0)
      {
         columnSpan = 1;
      }

      if (rowSpan <= 0)
      {
         rowSpan = 1;
      }



      index++;
   }

   public IEnumerator<SpanLocation> GetEnumerator()
   {
      foreach (var (i, (startColumn, stopColumn, startRow, stopRow)) in coordinates)
      {
         yield return new SpanLocation(i, startColumn, stopColumn - startColumn, startRow, stopRow - startRow);
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}