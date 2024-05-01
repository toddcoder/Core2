using Core.Matching;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Markup.Html;

public class Selector(string name) : IEquatable<Selector>
{
   public static implicit operator Selector(string source)
   {
      var _result = source.Matches("^ /(-['{']+) /s* '{' (/s* /(.+))? $; f");
      if (_result is (true, var (name, styleSource)))
      {
         var selector = new Selector(name);

         if (styleSource.IsNotEmpty())
         {
            Style style = styleSource;
            selector.styles.Add(style);
         }

         return selector;
      }
      else
      {
         throw fail($"Didn't understand selector {source}");
      }
   }

   public static Selector operator +(Selector selector, Style style)
   {
      selector.Add(style);
      return selector;
   }

   protected List<Style> styles = [];

   public string Name => name;

   public void Add(Style style) => styles.Add(style);

   public bool Equals(Selector? other) => other is not null && Equals(styles, other.styles) && name == other.Name;

   public override bool Equals(object? obj) => obj is Selector other && Equals(other);

   public override int GetHashCode() => hashCode() + styles + name;

   public static bool operator ==(Selector left, Selector right) => Equals(left, right);

   public static bool operator !=(Selector left, Selector right) => !Equals(left, right);

   public override string ToString()
   {
      using var writer = new StringWriter();
      writer.WriteLine();
      writer.WriteLine($"   {name} {{");
      foreach (var style in styles)
      {
         writer.WriteLine($"      {style};");
      }

      writer.WriteLine("   }");

      return writer.ToString();
   }
}