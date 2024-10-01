namespace Core.Markup.Markdown;

public class StyleItem(string key, string value) : IEquatable<StyleItem>
{
   public string Key => key;

   public string Value => value;

   public bool Equals(StyleItem? other) => other is not null && key == other.Key;

   public override bool Equals(object? obj) => obj is StyleItem otherStyleItem && Equals(otherStyleItem);

   public override int GetHashCode() => key.GetHashCode();

   public static bool operator ==(StyleItem? left, StyleItem? right) => Equals(left, right);

   public static bool operator !=(StyleItem? left, StyleItem? right) => !Equals(left, right);
}