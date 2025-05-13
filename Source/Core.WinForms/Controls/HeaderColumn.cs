namespace Core.WinForms.Controls;

public class HeaderColumn(string name, string text) : IEquatable<HeaderColumn>
{
   protected string name = name;
   protected string text = text;

   public string Text
   {
      get => text;
      set => text = value;
   }

   public int Index { get; set; } = -1;

   public CardinalAlignment Alignment { get; set; } = CardinalAlignment.Center;

   public bool AutoSizeText { get; set; }

   public bool UseEmojis { get; set; } = true;

   public Color ForeColor { get; set; } = Color.White;

   public Color BackColor { get; set; } = Color.CadetBlue;

   public string FontName { get; set; } = "Consolas";

   public float FontSize { get; set; } = 12f;

   public FontStyle FontStyle { get; set; } = FontStyle.Regular;

   public ColumnSize ColumnSize { get; set; } = new ColumnSize.Percent(100);

   public UiAction UiAction()
   {
      var uiAction = new UiAction
      {
         NonNullFont = new Font(FontName, FontSize, FontStyle),
         AutoSizeText = AutoSizeText,
         UseEmojis = UseEmojis,
         CardinalAlignment = Alignment
      };

      uiAction.Display(text, ForeColor, BackColor);
      uiAction.ZeroOut();
      return uiAction;
   }

   public bool Equals(HeaderColumn? other) => other is not null && other.name == name;

   public override bool Equals(object? obj) => obj is HeaderColumn other && Equals(other);

   public override int GetHashCode() => HashCode.Combine(name);

   public static bool operator ==(HeaderColumn? left, HeaderColumn? right) => Equals(left, right);

   public static bool operator !=(HeaderColumn? left, HeaderColumn? right) => !Equals(left, right);
}