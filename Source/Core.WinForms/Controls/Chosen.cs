namespace Core.WinForms.Controls;

public class Chosen(string value, string key, int index, Color foreColor, Color backColor, bool isChecked) : IEquatable<Chosen>
{
   public Chosen(string value, ListViewItem item) : this(value, item.Text, item.Index, item.ForeColor, item.BackColor, item.Checked)
   {
   }

   public string Key => key;

   public string Value => value;

   public int Index => index;

   public Color ForeColor => foreColor;

   public Color BackColor => backColor;

   public bool IsChecked => isChecked;

   public void Deconstruct(out string outKey, out string outValue, out int outIndex, out Color outForeColor, out Color outBackColor, out bool outIsChecked)
   {
      outKey = Key;
      outValue = Value;
      outIndex = Index;
      outForeColor = ForeColor;
      outBackColor = BackColor;
      outIsChecked = IsChecked;
   }

   public bool Equals(Chosen? other)
   {
      return other is not null && Key == other.Key && Value == other.Value && Index == other.Index && ForeColor == other.ForeColor &&
         BackColor == other.BackColor && IsChecked == other.IsChecked;
   }

   public override bool Equals(object? obj) => obj is Chosen other && Equals(other);

   public override int GetHashCode() => HashCode.Combine(Key, Value, Index, ForeColor, BackColor, IsChecked);

   public static bool operator ==(Chosen? left, Chosen? right) => Equals(left, right);

   public static bool operator !=(Chosen? left, Chosen? right) => !Equals(left, right);
}