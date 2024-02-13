using Core.WinForms.Drawing;

namespace Core.WinForms.Controls;

public class ProgressMiniProcessor(Rectangle clientRectangle)
{
   protected ColorFader colorFader = new(Color.Coral, Color.CadetBlue);

   public void OnPaintBackground(Graphics g, int index, int maximum) => colorFader.OnPaint(g, clientRectangle, index, maximum);
}