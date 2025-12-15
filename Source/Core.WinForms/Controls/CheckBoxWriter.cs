using System.Windows.Forms.VisualStyles;
using Core.Arrays;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;

namespace Core.WinForms.Controls;

public class CheckBoxWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling, bool useEmojis)
   : IAlternateWriter
{
   protected readonly Color defaultForeColor = Color.White;
   protected readonly Color defaultBackColor = Color.CadetBlue;
   protected const FontStyle DEFAULT_FONT_STYLE = FontStyle.Regular;

   protected Hash<int, Color> foreColors = [];
   protected Hash<int, Color> backColors = [];
   protected Hash<int, FontStyle> fontStyles = [];

   public bool BoxChecked { get; set; }

   public Maybe<Color> GetForeColor(int index) => foreColors.Maybe[index];

   public Color GetAlternateForeColor(int index) => foreColors.Maybe[index] | defaultForeColor;

   public void SetForeColor(int index, Color color) => foreColors[index] = color;

   public Maybe<Color> GetBackColor(int index) => backColors.Maybe[index];

   public Color GetAlternateBackColor(int index) => backColors.Maybe[index] | defaultBackColor;

   public void SetBackColor(int index, Color color) => backColors[index] = color;

   public FontStyle GetFontStyle(int index) => fontStyles.Maybe[index] | DEFAULT_FONT_STYLE;

   public void SetFontStyle(int index, FontStyle style) => fontStyles[index] = style;

   public Maybe<string> GetAlternate(int index) => alternates.Maybe(index);

   public void SetAlternate(int index, string alternate)
   {
      if (index.Between(0).Until(alternates.Length))
      {
         alternates[index] = alternate;
      }
   }

   public int SelectedIndex { get; set; }

   public int DisabledIndex { get; set; }

   public string Alternate => alternates.Maybe(SelectedIndex);

   protected void onPaint(Graphics g, int index, Rectangle rectangle, UiActionWriter writer, string alternate)
   {
      g.HighQuality();

      writer.Font = getFont();
      writer.Color = GetAlternateForeColor(index);
      var backColor = GetAlternateBackColor(index);
      using var brush = new SolidBrush(backColor);
      g.FillRectangle(brush, rectangle);

      if (BoxChecked)
      {
         drawChecked(rectangle);
      }
      else
      {
         drawUnchecked(rectangle);
      }

      return;

      Font getFont() => new(uiAction.NonNullFont, GetFontStyle(index));

      void drawUnchecked(Rectangle rectangle)
      {
         CheckBoxRenderer.RenderMatchingApplicationState = true;
         var size = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
         var glyphRectangle = size.West(rectangle, 4);
         CheckBoxRenderer.DrawCheckBox(g, glyphRectangle.Location, CheckBoxState.UncheckedNormal);
         var width = glyphRectangle.Width;
         writer.Rectangle = rectangle with { X = rectangle.X + width, Width = rectangle.Width - width };
         writer.Write(g, alternate, false);
      }

      void drawChecked(Rectangle rectangle)
      {
         CheckBoxRenderer.RenderMatchingApplicationState = true;
         var size = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.CheckedNormal);
         var glyphRectangle = size.West(rectangle, 4);
         CheckBoxRenderer.DrawCheckBox(g, glyphRectangle.Location, CheckBoxState.CheckedNormal);
         var width = glyphRectangle.Width;
         writer.Rectangle = rectangle with { X = rectangle.X + width, Width = rectangle.Width - width };
         writer.Write(g, alternate, false);
      }
   }

   public void OnPaint(Graphics g)
   {
      var writer = new UiActionWriter(CardinalAlignment.Center, autoSizeText, _floor, _ceiling, UiActionButtonType.Normal, useEmojis, false);
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         onPaint(g, index, rectangle, writer, alternates[index]);
      }
   }

   public string[] Alternates => alternates;
}