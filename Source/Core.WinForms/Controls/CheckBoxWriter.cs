using System.Windows.Forms.VisualStyles;
using Core.Arrays;
using Core.Collections;
using Core.Monads;

namespace Core.WinForms.Controls;

public class CheckBoxWriter(UiAction uiAction, string[] alternates, Maybe<int> _floor, Maybe<int> _ceiling, bool useEmojis)
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
         var glyphRectangle = size.West(rectangle, 2);
         CheckBoxRenderer.DrawCheckBox(g, glyphRectangle.Location, CheckBoxState.UncheckedNormal);
         writer.Rectangle = rectangle with { Width = rectangle.Width - glyphRectangle.Width };
         writer.Write(g, alternate, false);
      }

      void drawChecked(Rectangle rectangle)
      {
         CheckBoxRenderer.RenderMatchingApplicationState = true;
         var size = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.CheckedNormal);
         var glyphRectangle = size.West(rectangle, 2);
         CheckBoxRenderer.DrawCheckBox(g, glyphRectangle.Location, CheckBoxState.CheckedNormal);
         writer.Rectangle = rectangle with { Width = rectangle.Width - glyphRectangle.Width };
         writer.Write(g, alternate, false);
      }
   }

   public void OnPaint(Graphics g)
   {
      var writer = new UiActionWriter(CardinalAlignment.Center, true, _floor, _ceiling, UiActionButtonType.Normal, useEmojis);
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         onPaint(g, index, rectangle, writer, alternates[index]);
      }
   }

   public string[] Alternates => alternates;
}