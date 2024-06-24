using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Rtf;

public class PendingFormatter(Table table)
{
   public static PendingFormatter operator |(PendingFormatter pendingFormatter, string columnText)
   {
      return pendingFormatter.Table.ColumnPendingFormatter(columnText);
   }

   public static PendingFormatter operator +(PendingFormatter pendingFormatter, string columnText)
   {
      return pendingFormatter.Table.ColumnPendingFormatter(columnText);
   }

   public static PendingFormatter operator +(PendingFormatter formatter, Feature feature) => feature switch
   {
      Feature.Bold => formatter.Bold(),
      Feature.Italic => formatter.Italic(),
      Feature.Underline => formatter.Underline(),
      Feature.Bullet => formatter.Bullet(),
      Feature.NewPage => formatter.NewPage(),
      Feature.NewPageAfter => formatter.NewPageAfter(),
      _ => formatter
   };

   public static PendingFormatter operator +(PendingFormatter formatter, Alignment alignment) => formatter.Alignment(alignment);

   public static PendingFormatter operator +(PendingFormatter formatter, ForegroundColorDescriptor foregroundColor)
   {
      return formatter.ForegroundColor(foregroundColor);
   }

   public static PendingFormatter operator +(PendingFormatter formatter, BackgroundColorDescriptor backgroundColor)
   {
      return formatter.BackgroundColor(backgroundColor);
   }

   public static PendingFormatter operator +(PendingFormatter formatter, Hyperlink hyperlink) => formatter.Hyperlink(hyperlink);

   public static PendingFormatter operator +(PendingFormatter formatter, FontDescriptor font) => formatter.Font(font);

   public static PendingFormatter operator +(PendingFormatter formatter, float fontSize) => formatter.FontSize(fontSize);

   public static PendingFormatter operator +(PendingFormatter formatter, FirstLineIndent firstLineIndent)
   {
      return formatter.FirstLineIndent(firstLineIndent);
   }

   public static PendingFormatter operator +(PendingFormatter formatter, (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      return formatter.Margins(margins);
   }

   protected Set<Feature> features = [];
   protected Maybe<Alignment> _alignment = nil;
   protected Maybe<ForegroundColorDescriptor> _foregroundColor = nil;
   protected Maybe<BackgroundColorDescriptor> _backgroundColor = nil;
   protected Maybe<Hyperlink> _hyperlink = nil;
   protected Maybe<FontDescriptor> _font = nil;
   protected Maybe<float> _fontSize = nil;
   protected Maybe<FirstLineIndent> _firstLineIndent = nil;
   protected (Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins = (nil, nil, nil, nil);
   protected Maybe<Style> _style = nil;

   public Table Table => table;

   public virtual PendingFormatter Italic(bool on = true)
   {
      if (on)
      {
         features.Add(Feature.Italic);
      }
      else
      {
         features.Remove(Feature.Italic);
      }

      return this;
   }

   public virtual PendingFormatter Bold(bool on = true)
   {
      if (on)
      {
         features.Add(Feature.Bold);
      }
      else
      {
         features.Remove(Feature.Bold);
      }

      return this;
   }

   public virtual PendingFormatter Underline(bool on = true)
   {
      if (on)
      {
         features.Add(Feature.Underline);
      }
      else
      {
         features.Remove(Feature.Underline);
      }

      return this;
   }

   public virtual PendingFormatter Bullet()
   {
      features.Add(Feature.Bullet);
      return this;
   }

   public virtual PendingFormatter NewPage()
   {
      features.Add(Feature.NewPage);
      return this;
   }

   public virtual PendingFormatter NewPageAfter()
   {
      features.Add(Feature.NewPageAfter);
      return this;
   }

   public PendingFormatter Alignment(Alignment alignment)
   {
      _alignment = alignment;
      return this;
   }

   public PendingFormatter ForegroundColor(ForegroundColorDescriptor foregroundColor)
   {
      _foregroundColor = foregroundColor;
      return this;
   }

   public PendingFormatter BackgroundColor(BackgroundColorDescriptor backgroundColor)
   {
      _backgroundColor = backgroundColor;
      return this;
   }

   public PendingFormatter Hyperlink(Hyperlink hyperlink)
   {
      _hyperlink = hyperlink;
      return this;
   }

   public PendingFormatter Font(FontDescriptor font)
   {
      _font = font;
      return this;
   }

   public PendingFormatter FontSize(float fontSize)
   {
      _fontSize = fontSize;
      return this;
   }

   public PendingFormatter FirstLineIndent(FirstLineIndent firstLineIndent)
   {
      _firstLineIndent = firstLineIndent;
      return this;
   }

   public PendingFormatter Margins((Maybe<float>, Maybe<float>, Maybe<float>, Maybe<float>) margins)
   {
      this.margins = margins;
      return this;
   }

   public PendingFormatter Style(Style style)
   {
      _style = style;
      return this;
   }

   public Formatter Formatter(Paragraph paragraph, CharFormat format)
   {
      var formatter = _style.Map(s => s.Formatter(paragraph, format)) | (() => new Formatter(paragraph, format));

      foreach (var feature in features)
      {
         _ = formatter + feature;
      }

      if (_alignment is (true, var alignment))
      {
         formatter.Alignment(alignment);
      }

      if (_foregroundColor is (true, var foregroundColor))
      {
         formatter.ForegroundColor(foregroundColor);
      }

      if (_backgroundColor is (true, var backgroundColor))
      {
         formatter.BackgroundColor(backgroundColor);
      }

      if (_hyperlink is (true, var hyperlink))
      {
         formatter.Hyperlink(hyperlink);
      }

      if (_font is (true, var font))
      {
         formatter.Font(font);
      }

      if (_fontSize is (true, var fontSize))
      {
         formatter.FontSize(fontSize);
      }

      if (_firstLineIndent is (true, var firstLineIndent))
      {
         formatter.FirstLineIndent(firstLineIndent.Amount);
      }

      formatter.Margins(margins);

      return formatter;
   }
}