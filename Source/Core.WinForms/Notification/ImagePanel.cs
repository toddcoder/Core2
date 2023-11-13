using System.Drawing.Drawing2D;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Notification;

public class ImagePanel : Panel
{
   protected Maybe<Image> _image;

   public ImagePanel()
   {
      SetStyle(ControlStyles.SupportsTransparentBackColor, true);
      SetStyle(ControlStyles.UserPaint, true);
      _image = nil;
   }

   public Image Image
   {
      get => _image.Required("Image not set");
      set
      {
         _image = value;
         RecreateHandle();
      }
   }

   protected override void OnPaintBackground(PaintEventArgs e)
   {
   }

   protected override CreateParams CreateParams
   {
      get
      {
         var cp = base.CreateParams;
         cp.ExStyle |= 0x20;

         return cp;
      }
   }

   protected override void OnPaint(PaintEventArgs e)
   {
      if (_image is (true, var image))
      {
         var imageWidth = image.Width;
         var imageHeight = image.Height;

         e.Graphics.CompositingQuality = CompositingQuality.HighQuality;

         e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
         e.Graphics.CompositingMode = CompositingMode.SourceOver;
         e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
         using var brush = new SolidBrush(BackColor);
         e.Graphics.FillRectangle(brush, ClientRectangle);

         var left = (Width - imageWidth) / 2;
         var top = (Height - imageHeight) / 2;
         e.Graphics.DrawImage(_image, left, top, imageWidth, imageHeight);
      }
   }
}