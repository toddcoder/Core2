using System.Windows.Forms.VisualStyles;
using Core.Monads;

namespace Core.WinForms.Controls;

public class CheckBoxWriter(UiAction uiAction, string[] alternates, bool autoSizeText, Maybe<int> _floor, Maybe<int> _ceiling)
   : AlternateWriter(uiAction, alternates, autoSizeText, _floor, _ceiling)
{
   public bool BoxChecked
   {
      get => selectedIndex != -1;
      set => selectedIndex = value ? 0 : -1;
   }

   protected override void drawUnselected(Graphics g, Pen pen, Rectangle rectangle, Color backColor)
   {
      g.HighQuality();
      CheckBoxRenderer.RenderMatchingApplicationState = true;
      var size = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.UncheckedNormal);
      var glyphRectangle = size.West(rectangle, 2);
      CheckBoxRenderer.DrawCheckBox(g, glyphRectangle.Location, CheckBoxState.UncheckedNormal);
   }

   protected override void drawSelected(Graphics g, Rectangle rectangle, Rectangle alternateRectangle, int index, Color foreColor, Color backColor,
      int penSize)
   {
      g.HighQuality();
      CheckBoxRenderer.RenderMatchingApplicationState = true;
      var size = CheckBoxRenderer.GetGlyphSize(g, CheckBoxState.CheckedNormal);
      var glyphRectangle = size.West(rectangle, 2);
      CheckBoxRenderer.DrawCheckBox(g, glyphRectangle.Location, CheckBoxState.CheckedNormal);
   }
}