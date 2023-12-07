using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core.Collections;

namespace Core.Strings.Text;

public class DifferenceModel
{
   public DifferenceModel()
   {
      OldDifferenceItems = [];
      NewDifferenceItems = [];
   }

   public List<DifferenceItem> OldDifferenceItems { get; }

   public IEnumerable<Difference> OldDifferences()
   {
      return OldDifferenceItems
         .Where(i => i.Type != DifferenceType.Unchanged && i.Type != DifferenceType.Imaginary)
         .Select(Difference.FromDifferenceItem);
   }

   public List<DifferenceItem> NewDifferenceItems { get; }

   public IEnumerable<Difference> NewDifferences()
   {
      return NewDifferenceItems
         .Where(i => i.Type != DifferenceType.Unchanged && i.Type != DifferenceType.Imaginary)
         .Select(Difference.FromDifferenceItem);
   }

   public IEnumerable<string> MergedDifferences()
   {
      var oldHash = OldDifferences().ToHash(d => d.Position, d => d.ToString());
      var newHash = NewDifferences().ToHash(d => d.Position, d => d.ToString());
      Set<int> keys = [];
      keys.AddRange(oldHash.Keys.Where(k => k > -1));
      keys.AddRange(newHash.Keys.Where(k => k > -1));

      var builder = new StringBuilder();

      foreach (var key in keys.OrderBy(k => k))
      {
         if (oldHash.Maybe[key] is (true, var oldValue))
         {
            builder.Append(oldValue);
         }

         if (newHash.Maybe[key] is (true, var newValue))
         {
            builder.Append(" <=> ");
            builder.Append(newValue);
         }

         yield return builder.ToString();

         builder.Clear();
      }
   }

   public override string ToString()
   {
      using var writer = new StringWriter();

      writer.WriteLine("old:");
      foreach (var item in OldDifferenceItems)
      {
         writer.WriteLine(item);
      }

      writer.WriteLine();

      writer.WriteLine("new:");
      foreach (var item in NewDifferenceItems)
      {
         writer.WriteLine(item);
      }

      return writer.ToString();
   }
}