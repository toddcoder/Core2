namespace Core.WinForms.Controls;

public class HeaderColumnBuilder(string name, HeaderColumns headerColumns)
{
   public static HeaderColumnBuilder operator +(HeaderColumnBuilder builder, string text)
   {
      builder.text = text;
      return builder;
   }

   public static HeaderColumnBuilder operator +(HeaderColumnBuilder builder, CardinalAlignment alignment)
   {
      builder.alignment = alignment;
      return builder;
   }

   public static HeaderColumnBuilder operator +(HeaderColumnBuilder builder, AutoSizeText autoSizeText)
   {
      builder.autoSizeText = autoSizeText is AutoSizeText.True;
      return builder;
   }

   public static HeaderColumnBuilder operator +(HeaderColumnBuilder builder, UseEmojis useEmojis)
   {
      builder.useEmojis = useEmojis is UseEmojis.True;
      return builder;
   }

   public static HeaderColumnBuilder operator +(HeaderColumnBuilder builder, FontInfo fontInfo)
   {
      switch (fontInfo)
      {
         case FontInfo.Name name:
            builder.fontName = name.Value;
            break;
         case FontInfo.Size size:
            builder.fontSize = size.Value;
            break;
         case FontInfo.Style style:
            builder.fontStyle = style.Value;
            break;
      }

      return builder;
   }

   public static HeaderColumnBuilder operator +(HeaderColumnBuilder builder, ColumnSize columnSize)
   {
      builder.columnSize = columnSize;
      return builder;
   }

   public static HeaderColumnBuilder operator +(HeaderColumnBuilder builder, HeaderColumnColor headerColumnColor)
   {
      switch (headerColumnColor)
      {
         case HeaderColumnColor.BackColor backColor:
            builder.backColor = backColor.Color;
            break;
         case HeaderColumnColor.ForeColor foreColor:
            builder.foreColor = foreColor.Color;
            break;
      }

      return builder;
   }

   protected string name = name;
   protected string text = name;
   protected CardinalAlignment alignment = CardinalAlignment.Center;
   protected bool autoSizeText;
   protected bool useEmojis = true;
   protected Color foreColor = Color.White;
   protected Color backColor = Color.CadetBlue;
   protected string fontName = "Consolas";
   protected float fontSize = 12f;
   protected FontStyle fontStyle = FontStyle.Regular;
   protected ColumnSize columnSize = new ColumnSize.Percent(100);

   public HeaderColumn HeaderColumn()
   {
      var headerColumn = new HeaderColumn(text)
      {
         Alignment = alignment,
         AutoSizeText = autoSizeText,
         UseEmojis = useEmojis,
         ForeColor = foreColor,
         BackColor = backColor,
         FontName = fontName,
         FontSize = fontSize,
         FontStyle = fontStyle,
         ColumnSize = columnSize
      };
      headerColumns[name] = headerColumn;
      return headerColumn;
   }
}