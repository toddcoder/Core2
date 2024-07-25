using System.Drawing.Text;
using Core.Monads;
using Core.Strings;
using Core.Strings.Emojis;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class SubText(ISubTextHost subTextHost, string text, int x, int y, Size size, bool clickGlyph, bool chooserGlyph, bool invert = false,
   bool transparentBackground = false) : IEquatable<SubText>
{
   protected const string POSITIVE = "✅";
   protected const string NEGATIVE = "❎";
   public const int GLYPH_WIDTH = 8;

   protected Maybe<Color> _foreColor = nil;
   protected Maybe<Color> _backColor = nil;
   protected Maybe<CardinalAlignment> _alignment = nil;
   protected int margin = 2;
   protected LocationLockStatus locationLockStatus = LocationLockStatus.Floating;
   protected Maybe<(SubText subText, int margin)> _rightSubText = nil;
   protected Maybe<(SubText subText, int margin)> _leftSubText = nil;
   protected bool useEmojis = subTextHost.UseEmojis;

   public event EventHandler<PaintEventArgs>? Painting;
   public event EventHandler<PaintEventArgs>? PaintingBackground;

   public string Text
   {
      get => text;
      set
      {
         text = value;
         ResetLock();
      }
   }

   public int X
   {
      get => x;
      set => x = value;
   }

   public int Y
   {
      get => y;
      set => y = value;
   }

   public Size Size => size;

   public SubTextOption Option { get; set; } = SubTextOption.None;

   public SubTextSet Set => new(this, subTextHost);

   public string FontName { get; set; } = "Consolas";

   public float FontSize { get; set; } = 12;

   public FontStyle FontStyle { get; set; } = FontStyle.Regular;

   public bool Outline { get; set; }

   public bool Invert
   {
      get => invert;
      set => invert = value;
   }

   public bool TransparentBackground
   {
      get => transparentBackground;
      set => transparentBackground = value;
   }

   public Guid Id { get; } = Guid.NewGuid();

   public Maybe<Color> ForeColor
   {
      get => _foreColor;
      set => _foreColor = value;
   }

   public Maybe<Color> BackColor
   {
      get => _backColor;
      set => _backColor = value;
   }

   public SubText SetFont(string fontName, float fontSize, FontStyle fontStyle)
   {
      FontName = fontName;
      FontSize = fontSize;
      FontStyle = fontStyle;

      return this;
   }

   public void SetAlignment(CardinalAlignment alignment)
   {
      _alignment = alignment;
      if (_alignment)
      {
         ResetLock();
      }
   }

   public void SetMargin(int margin) => this.margin = margin;

   public bool IncludeFloor { get; set; } = true;

   public bool IncludeCeiling { get; set; } = true;

   public bool SquareFirstCharacter { get; set; }

   public SubTextTransparency Transparency { get; set; }

   public int Alpha { get; set; } = 255;

   protected string withEmojis(string originalText) => useEmojis ? originalText.EmojiSubstitutions() : originalText;

   public (Size measuredSize, string text, TextFormatFlags flags, Font font) TextSize(Maybe<Graphics> _graphics)
   {
      var newText = withEmojis(text);
      var font = new Font(FontName, FontSize, FontStyle);
      var flags = TextFormatFlags.EndEllipsis | TextFormatFlags.NoPrefix | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter;
      var proposedSize = new Size(int.MaxValue, int.MaxValue);
      Size measuredSize;
      if (_graphics is (true, var graphics))
      {
         measuredSize = TextRenderer.MeasureText(graphics, newText, font, proposedSize, flags);
      }
      else
      {
         measuredSize = TextRenderer.MeasureText(newText, font, proposedSize, flags);
      }

      return (measuredSize, newText, flags, font);
   }

   public (int x, int y) LocationFromAlignment(Rectangle clientRectangle)
   {
      if (_alignment is (true, var alignment))
      {
         var (measuredSize, _, _, _) = TextSize(nil);

         locationLockStatus = LocationLockStatus.Locked;

         return alignment switch
         {
            CardinalAlignment.NorthWest => (nearX(), nearY()),
            CardinalAlignment.North => (centerX(), nearY()),
            CardinalAlignment.NorthEast => (farX(), nearY()),
            CardinalAlignment.East => (farX(), centerY()),
            CardinalAlignment.SouthEast => (farX(), farY()),
            CardinalAlignment.South => (centerX(), farY()),
            CardinalAlignment.SouthWest => (nearX(), farY()),
            CardinalAlignment.West => (nearX(), centerY()),
            CardinalAlignment.Center => (centerX(), centerY()),
            _ => (X, Y)
         };

         int centerX() => (clientRectangle.Width - measuredSize.Width) / 2 + clientRectangle.X;

         int centerY() => (clientRectangle.Height - measuredSize.Height) / 2 + clientRectangle.Y;

         int nearX() => clientRectangle.X + margin;

         int nearY() => clientRectangle.Y + margin;

         int farX() => clientRectangle.Right - measuredSize.Width - margin - (clickGlyph ? GLYPH_WIDTH : 0) - (chooserGlyph ? GLYPH_WIDTH : 0);

         int farY() => clientRectangle.Bottom - measuredSize.Height - margin;
      }
      else
      {
         locationLockStatus = LocationLockStatus.Floating;
         return (X, Y);
      }
   }

   public void SetLocation(Rectangle clientRectangle)
   {
      if (locationLockStatus != LocationLockStatus.Locked)
      {
         (X, Y) = LocationFromAlignment(clientRectangle);
      }
   }

   public void ResetLock()
   {
      if (_alignment)
      {
         locationLockStatus = LocationLockStatus.Unlocked;
      }
      else
      {
         locationLockStatus = LocationLockStatus.Floating;
      }
   }

   protected int alphaFromTransparency() => Transparency switch
   {
      SubTextTransparency.Quarter => 64,
      SubTextTransparency.Half => 128,
      SubTextTransparency.ThreeQuarters => 192,
      _ => Alpha
   };

   protected virtual SubText draw(Graphics g, Color foreColor, Color backColor)
   {
      var (measuredSize, sizedText, flags, font) = TextSize(g);

      try
      {
         g.HighQuality();
         g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

         var clientRectangle = Rectangle.Empty;

         PaintingBackground?.Invoke(this, new PaintEventArgs(g, clientRectangle));

         if (Painting is not null)
         {
            Painting.Invoke(this, new PaintEventArgs(g, clientRectangle));
            return this;
         }

         var location = new Point(X, Y);
         var rectangle = new Rectangle(location, measuredSize);

         var foreColorToUse = Invert ? backColor : foreColor;
         var backColorToUse = Invert ? foreColor : backColor;

         var alpha = alphaFromTransparency();

         foreColorToUse = Color.FromArgb(alpha, foreColorToUse);
         backColorToUse = Color.FromArgb(alpha, backColorToUse);

         if (transparentBackground)
         {
            TextRenderer.DrawText(g, sizedText, font, rectangle, foreColorToUse, flags);
         }
         else
         {
            using var brush = new SolidBrush(backColorToUse);
            g.FillRectangle(brush, rectangle);
            if (!invert && Outline)
            {
               using var pen = new Pen(foreColorToUse);
               g.DrawRectangle(pen, rectangle);
            }

            TextRenderer.DrawText(g, sizedText, font, rectangle, foreColorToUse, flags);
         }

         if (SquareFirstCharacter && sizedText.Length > 0)
         {
            var character = sizedText.Keep(1);
            var charSize = TextRenderer.MeasureText(g, character, font);
            var charLocation = rectangle.Location;
            var charRectangle = new Rectangle(charLocation, charSize).Reposition(2, 2).Resize(-6, -4);
            using var firstBrush = new SolidBrush(Color.FromArgb(64, Color.Wheat));
            g.FillRectangle(firstBrush, charRectangle);
            using var firstPen = new Pen(Color.Black);
            g.DrawRectangle(firstPen, charRectangle);
         }

         return this;
      }
      finally
      {
         font.Dispose();
      }
   }

   public virtual SubText Draw(Graphics graphics, Color foreColor, Color backColor)
   {
      foreColor = _foreColor | foreColor;
      backColor = _backColor | backColor;

      return draw(graphics, foreColor, backColor);
   }

   public virtual Maybe<SubText> Draw(Graphics graphics)
   {
      if (_foreColor is (true, var foreColor) && _backColor is (true, var backColor))
      {
         return draw(graphics, foreColor, backColor);
      }
      else
      {
         return nil;
      }
   }

   public void LeftOf(SubText rightSubText, int margin = 2)
   {
      X = rightSubText.X - TextSize(nil).measuredSize.Width - margin;
      Y = rightSubText.Y;

      if (!_rightSubText)
      {
         _rightSubText = (rightSubText, margin);
      }
   }

   public void LeftOf(Maybe<SubText> _rightSubText, int margin = 2)
   {
      if (_rightSubText is (true, var rightSubText))
      {
         LeftOf(rightSubText, margin);
      }
   }

   public void AdjustRightSubText()
   {
      if (_rightSubText is (true, var (rightSubText, subTextMargin)))
      {
         LeftOf(rightSubText, subTextMargin);
      }
   }

   public void RightOf(SubText leftSubText, int margin = 2)
   {
      X = leftSubText.X + leftSubText.TextSize(nil).measuredSize.Width + margin;
      Y = leftSubText.Y;

      if (!_leftSubText)
      {
         _leftSubText = (leftSubText, margin);
      }
   }

   public void RightOf(Maybe<SubText> _leftSubText, int margin = 2)
   {
      if (_leftSubText is (true, var leftSubText))
      {
         RightOf(leftSubText, margin);
      }
   }

   public void AdjustLeftSubText()
   {
      if (_leftSubText is (true, var (leftSubText, subTextMargin)))
      {
         RightOf(leftSubText, subTextMargin);
      }
   }

   public bool Equals(SubText? other) => other is not null && Id.Equals(other.Id);

   public override bool Equals(object? obj) => obj is SubText other && Equals(other);

   public override int GetHashCode() => Id.GetHashCode();

   public static bool operator ==(SubText left, SubText right) => Equals(left, right);

   public static bool operator !=(SubText left, SubText right) => !Equals(left, right);
}