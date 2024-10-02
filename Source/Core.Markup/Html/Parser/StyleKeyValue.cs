namespace Core.Markup.Html.Parser;

public class StyleKeyValue(string key, string value) : IEquatable<StyleKeyValue>
{
   public string Key => key;

   public string Value => value;

   public bool Equals(StyleKeyValue? other) => other is not null && key == other.Key;

   public override bool Equals(object? obj) => obj is StyleKeyValue other && Equals(other);

   public override int GetHashCode() => key.GetHashCode();

   public static bool operator ==(StyleKeyValue? left, StyleKeyValue? right) => Equals(left, right);

   public static bool operator !=(StyleKeyValue? left, StyleKeyValue? right) => !Equals(left, right);

   public void Deconstruct(out string outKey, out string outValue)
   {
      outKey = Key;
      outValue = Value;
   }
}