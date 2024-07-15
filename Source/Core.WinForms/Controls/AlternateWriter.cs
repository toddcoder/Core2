using System.Windows.Forms.VisualStyles;
using Core.Arrays;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;

namespace Core.WinForms.Controls;

public class AlternateWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling)
{
   protected int selectedIndex;
   protected int disabledIndex = -1;
   protected Lazy<Font> disabledFont = new(() => new Font(uiAction.Font, FontStyle.Italic));
   protected Hash<int, Color> foreColors = [];
   protected Hash<int, Color> backColors = [];

   protected UiAction uiAction = uiAction;

   protected virtual int getPenSize(Rectangle rectangle)
   {
      var penSize = rectangle.Height / 40;
      return penSize <= 0 ? 1 : penSize;
   }

   protected virtual (int penSize, Rectangle textRectangle, Rectangle smallRectangle) splitRectangle(Rectangle rectangle)
   {
      var penSize = getPenSize(rectangle);

      var checkSize = rectangle.Height / 3;
      var checkSizeWithMargin = checkSize + 8;
      var checkRectangle = rectangle with
      {
         X = rectangle.X + 4,
         Y = (rectangle.Height - checkSize) / 2 + 2,
         Width = checkSize,
         Height = checkSize
      };
      var textRectangle = rectangle with
      {
         X = rectangle.X + checkSizeWithMargin,
         Width = rectangle.Width - checkSizeWithMargin,
         Height = rectangle.Height
      };

      return (penSize, textRectangle, checkRectangle);
   }

   public void SetForeColor(int index, Color color) => foreColors[index] = color;

   public Maybe<Color> GetForeColor(int index) => foreColors.Maybe[index];

   public void SetBackColor(int index, Color color) => backColors[index] = color;

   public Maybe<Color> GetBackColor(int index) => backColors.Maybe[index];

   public void SetColors(int index, UiActionType type)
   {
      SetForeColor(index, uiAction.GetForeColor(type));
      SetBackColor(index, uiAction.GetBackColor(type));
   }

   public int SelectedIndex
   {
      get => selectedIndex;
      set
      {
         if (disabledIndex != value)
         {
            if (value < 0)
            {
               selectedIndex = 0;
            }
            else if (value >= alternates.Length)
            {
               selectedIndex = alternates.Length - 1;
            }
            else
            {
               selectedIndex = value;
            }
         }
      }
   }

   public int DisabledIndex
   {
      get => disabledIndex;
      set
      {
         if (value < 0 || value >= alternates.Length)
         {
            disabledIndex = -1;
         }
         else
         {
            disabledIndex = value;
         }
      }
   }

   public string Alternate => alternates[selectedIndex];

   public string[] Alternates => alternates;

   public Maybe<string> GetAlternate(int index) => alternates.Of(index);

   protected static void fillRectangle(Graphics g, Rectangle rectangle, Color color)
   {
      using var brush = new SolidBrush(color);
      g.FillRectangle(brush, rectangle);
   }

   protected virtual void drawSelected(Graphics g, Rectangle rectangle, Rectangle alternateRectangle, int index, Color foreColor, Color backColor,
      int penSize)
   {
      g.HighQuality();
      RadioButtonRenderer.RenderMatchingApplicationState = true;
      var size = RadioButtonRenderer.GetGlyphSize(g, RadioButtonState.CheckedNormal);
      var glyphRectangle = size.West(rectangle, 2);
      RadioButtonRenderer.DrawRadioButton(g, glyphRectangle.Location, RadioButtonState.CheckedNormal);
   }

   protected virtual void drawUnselected(Graphics g, Pen pen, Rectangle rectangle, Color backColor)
   {
      g.HighQuality();
      RadioButtonRenderer.RenderMatchingApplicationState = true;
      var size = RadioButtonRenderer.GetGlyphSize(g, RadioButtonState.UncheckedNormal);
      var glyphRectangle = size.West(rectangle, 2);
      RadioButtonRenderer.DrawRadioButton(g, glyphRectangle.Location, RadioButtonState.UncheckedNormal);
   }

   protected void drawUnselected(Graphics g, Rectangle rectangle, Color foreColor, Color backColor, int penSize)
   {
      using var pen = new Pen(foreColor, penSize);
      drawUnselected(g, pen, rectangle, backColor);
   }

   public Color GetAlternateForeColor(int index)
   {
      var _foreColor = foreColors.Maybe[index];
      if (index == disabledIndex)
      {
         return Color.Black;
      }
      else if (index == selectedIndex)
      {
         return _foreColor | Color.White;
      }
      else
      {
         return _foreColor | Color.Black;
      }
   }

   public Color GetAlternateBackColor(int index)
   {
      var _backColor = backColors.Maybe[index];
      if (index == disabledIndex)
      {
         return Color.LightGray;
      }
      else if (index == selectedIndex)
      {
         return _backColor | Color.Teal;
      }
      else
      {
         return _backColor | Color.Wheat;
      }
   }

   protected virtual void onPaint(Graphics g, int index, Rectangle rectangle, UiActionWriter writer, string alternate)
   {
      var (penSize, textRectangle, smallRectangle) = splitRectangle(rectangle);

      if (index == disabledIndex)
      {
         var foreColor = GetAlternateForeColor(index);
         var backColor = GetAlternateBackColor(index);
         using var brush = new SolidBrush(backColor);
         g.FillRectangle(brush, rectangle);
         writer.Rectangle = textRectangle;
         writer.Font = disabledFont.Value;
         writer.Color = foreColor;
         if (index == selectedIndex)
         {
            drawSelected(g, smallRectangle, rectangle, index, Color.Black, Color.LightGray, penSize);
         }
         else
         {
            drawUnselected(g, smallRectangle, Color.Black, Color.LightGray, penSize);
         }
      }
      else
      {
         writer.Font = uiAction.Font;
         writer.Color = GetAlternateForeColor(index);
         var backColor = GetAlternateBackColor(index);
         fillRectangle(g, rectangle, backColor);

         if (index == selectedIndex)
         {
            drawSelected(g, smallRectangle, rectangle, index, Color.Black, Color.White, penSize);
         }
         else
         {
            drawUnselected(g, smallRectangle, Color.Black, Color.White, penSize);
         }

         writer.Rectangle = textRectangle;
      }

      writer.Write(g, alternate, uiAction.Type is UiActionType.NoStatus);
   }

   public void OnPaint(Graphics g)
   {
      var writer = new UiActionWriter(CardinalAlignment.Center, autoSizeText, _floor, _ceiling, UiActionButtonType.Normal);
      foreach (var (index, rectangle) in uiAction.Rectangles.Indexed())
      {
         onPaint(g, index, rectangle, writer, alternates[index]);
      }
   }
}