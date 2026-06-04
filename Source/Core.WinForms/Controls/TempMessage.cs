using System.Drawing.Text;

namespace Core.WinForms.Controls;

public partial class TempMessage : UserControl
{
   protected int foreAlpha = 100;
   protected int backAlpha = 100;
   protected Color foreColor = Color.White;
   protected Color backColor = Color.Blue;
   protected Color workingForeColor = Color.White;
   protected Color workingBackColor = Color.Blue;
   protected string message = "";
   protected Lazy<Font> boldFont;
   protected bool loweringBack;

   public TempMessage()
   {
      InitializeComponent();

      boldFont = new Lazy<Font>(() => new Font(Font, FontStyle.Bold));

      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
   }

   public void Display(string message, Color foreColor, Color backColor)
   {
      timer.Enabled = false;
      this.message = message;
      this.foreColor = foreColor;
      this.backColor = backColor;
      foreAlpha = 100;
      backAlpha = 100;
      workingForeColor = foreColor.WithAlpha(foreAlpha);
      workingBackColor = backColor.WithAlpha(backAlpha);
      loweringBack = false;
      timer.Enabled = true;
      Invalidate();
   }

   public void Display(string message)
   {
      foreColor = Color.White;
      backColor = Color.Blue;
      Display(message, foreColor, backColor);
   }

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      if (!loweringBack)
      {
         e.Graphics.HighQuality();
         e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
         e.Graphics.TextContrast = 0;

         using var foreBrush = new SolidBrush(workingForeColor);
         using var sf = new StringFormat();
         sf.Alignment = StringAlignment.Center;
         sf.LineAlignment = StringAlignment.Center;
         e.Graphics.DrawString(message, boldFont.Value, foreBrush, ClientRectangle, sf);
      }
   }

   protected override void OnPaintBackground(PaintEventArgs e)
   {
      base.OnPaintBackground(e);

      if (loweringBack)
      {
         e.Graphics.HighQuality();
         using var backBrush = new SolidBrush(workingBackColor);
         e.Graphics.FillRectangle(backBrush, ClientRectangle);
      }
   }

   protected void timer_Tick(object sender, EventArgs e)
   {
      if (foreAlpha <= 0)
      {
         loweringBack = true;
      }
      else if (backAlpha <= 0)
      {
         foreAlpha = 100;
         backAlpha = 100;
         message = "";
         loweringBack = false;
         timer.Enabled = false;
      }
      else
      {
         if (loweringBack)
         {
            workingBackColor = backColor.WithAlpha(backAlpha);
            backAlpha -= 5;
         }
         else
         {
            workingForeColor = foreColor.WithAlpha(foreAlpha);
            foreAlpha -= 5;
         }

         Invalidate();
      }
   }
}