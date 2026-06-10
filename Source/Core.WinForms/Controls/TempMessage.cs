namespace Core.WinForms.Controls;

public partial class TempMessage : UserControl
{
   protected int foreAlpha = 255;
   protected int backAlpha = 255;
   protected Color foreColor = Color.White;
   protected Color backColor = Color.Blue;
   protected Color workingForeColor = Color.White;
   protected Color workingBackColor = Color.Blue;
   protected string message = "";
   protected bool loweringBack;

   public TempMessage()
   {
      InitializeComponent();

      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
   }

   public bool AutoSizeText { get; set; } = true;

   public bool UseEmojis { get; set; } = true;

   public void Display(string message, Color foreColor, Color backColor)
   {
      timer.Enabled = false;
      this.message = message;
      this.foreColor = foreColor;
      this.backColor = backColor;
      foreAlpha = 255;
      backAlpha = 255;
      workingForeColor = foreColor.WithAlpha(foreAlpha);
      workingBackColor = backColor.WithAlpha(backAlpha);
      loweringBack = false;
      timer.Enabled = true;
      Invalidate();
   }

   public void Display(string message) => Display(message, Color.White, Color.Blue);

   public void Success(string message) => Display(message, Color.White, Color.Green);

   public void Failure(string message) => Display(message, Color.White, Color.Red);

   public void Exception(Exception exception) => Display(exception.Message, Color.White, Color.Gold);

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      if (!loweringBack)
      {
         writeMessage(e.Graphics);
      }
   }

   protected void writeMessage(Graphics g)
   {
      var writer = new ControlWriter
      {
         Color = workingForeColor,
         Font = Font,
         Rectangle = ClientRectangle
      };
      writer.Write(g, message);
   }

   protected override void OnPaintBackground(PaintEventArgs e)
   {
      base.OnPaintBackground(e);
      e.Graphics.HighQuality();

      if (loweringBack)
      {
         using var backBrush = new SolidBrush(workingBackColor);
         e.Graphics.FillRectangle(backBrush, ClientRectangle);
      }
      else
      {
         using var backBrush = new SolidBrush(backColor);
         e.Graphics.FillRectangle(backBrush, ClientRectangle);
      }
   }

   protected void timer_Tick(object sender, EventArgs e)
   {
      if (loweringBack)
      {
         if (backAlpha >= 0)
         {
            workingBackColor = backColor.WithAlpha(backAlpha);
            backAlpha -= 5;
         }
         else
         {
            timer.Enabled = false;
         }
      }
      else
      {
         if (foreAlpha >= 0)
         {
            workingForeColor = foreColor.WithAlpha(foreAlpha);
            workingBackColor = backColor;
            foreAlpha -= 5;
         }
         else
         {
            loweringBack = true;
         }
      }

      Invalidate();
   }
}