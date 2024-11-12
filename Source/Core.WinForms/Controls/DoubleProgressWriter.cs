namespace Core.WinForms.Controls;

public class DoubleProgressWriter(Rectangle clientRectangle, Font font)
{
   protected Color backColor = Color.Coral;
   protected Color foreColor = Color.LightSteelBlue;

   public static DoubleProgressWriter Empty => new(Rectangle.Empty, new Font("Consolas", 12f));

   protected int outerMaximum;

   protected int innerMaximum;

   protected Rectangle clientRectangle = clientRectangle;

   protected Rectangle pieRectangle;

   protected Rectangle nonPieRectangle;

   protected Rectangle textRectangle;

   protected Rectangle percentRectangle;

   protected float sweepAngle;

   protected int outerIndex = 1;

   protected int innerIndex = 1;

   protected string outerText = "";

   protected string innerText = "";

   protected bool isRunning;

   public int OuterMaximum
   {
      get => outerMaximum;
      set => outerMaximum = value;
   }

   protected Rectangle getPercentRectangle(double percent)
   {
      return textRectangle with { Width = (int)(percent * textRectangle.Width) };
   }

   protected void setRectangles()
   {
      var width = Math.Min(clientRectangle.Width / 2, clientRectangle.Height);
      pieRectangle = clientRectangle with { Width = width };

      nonPieRectangle = clientRectangle with { X = clientRectangle.X + width, Width = clientRectangle.Width - width };

      var left = pieRectangle.Width / 2 - 4;
      width = clientRectangle.Width - left - 4;
      var height = (int)(.4 * clientRectangle.Height);
      var top = (clientRectangle.Height - height) / 2;
      textRectangle = new Rectangle(left, top, width, height);
   }

   public void AdvanceOuter(string outerText, int innerMaximum)
   {
      isRunning = true;

      this.outerText = outerText;
      this.innerMaximum = innerMaximum;

      if (outerIndex <= outerMaximum)
      {
         sweepAngle = (float)outerIndex / outerMaximum * 360;
         outerIndex++;
         innerIndex = 1;
      }
   }

   public void AdvanceInner(string innerText)
   {
      this.innerText = innerText;

      if (innerIndex <= innerMaximum)
      {
         percentRectangle = getPercentRectangle(getInnerPercent());
         innerIndex++;
      }
   }

   public void Done()
   {
      isRunning = false;
   }

   public void OnResize(Rectangle clientRectangle)
   {
      this.clientRectangle = clientRectangle;
      setRectangles();
   }

   public void OnPaint(PaintEventArgs e)
   {
      e.Graphics.HighQuality();

      drawInner(e.Graphics);
      drawOuter(e.Graphics);
   }

   protected double getInnerPercent()
   {
      return innerMaximum == 0 ? 0 : (double)innerIndex / innerMaximum;
   }

   protected Color getForeColor() => isRunning ? foreColor : Color.LightGray;

   protected Color getBackColor() => isRunning ? backColor : Color.LightGray;

   protected void drawOuter(Graphics g)
   {
      using var circleBrush = new SolidBrush(getBackColor());
      g.FillEllipse(circleBrush, pieRectangle);
      using var pieBrush = new SolidBrush(getForeColor());
      g.FillPie(pieBrush, pieRectangle, 0, sweepAngle);

      if (isRunning)
      {
         var writer = new RectangleWriter(outerText, nonPieRectangle, CardinalAlignment.NorthWest)
         {
            Font = font,
            ForeColor = Color.Black,
            BackgroundRestriction = new BackgroundRestriction.UseWriterAlignment(4, 4)
         };
         writer.Write(g);
      }
   }

   protected void drawInner(Graphics g)
   {
      using var backBrush = new SolidBrush(getBackColor());
      g.FillRectangle(backBrush, textRectangle);

      using var foreBrush = new SolidBrush(getForeColor());
      g.FillRectangle(foreBrush, percentRectangle);

      if (isRunning)
      {
         var writer = new RectangleWriter(innerText, textRectangle)
         {
            Font = font,
            ForeColor = Color.White,
            AutoSizeText = true
         };
         writer.Write(g);
      }
   }
}