using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public partial class TempMessage : UserControl
{
   protected enum PaintStatus
   {
      Fore,
      Back,
      Busy
   }

   protected int foreAlpha = 255;
   protected int backAlpha = 255;
   protected Color foreColor = Color.White;
   protected Color backColor = Color.Blue;
   protected Color workingForeColor = Color.White;
   protected Color workingBackColor = Color.Blue;
   protected string message = "";
   protected PaintStatus paintStatus = PaintStatus.Fore;
   protected Maybe<BusyTextProcessor> _busyTextProcessor = nil;
   protected Rectangle textRectangle = Rectangle.Empty;

   public TempMessage()
   {
      InitializeComponent();

      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);
   }

   public bool AutoSizeText { get; set; } = true;

   public bool UseEmojis { get; set; } = true;

   public bool IsBusy
   {
      get => paintStatus == PaintStatus.Busy;
      set
      {
         paintStatus = value ? PaintStatus.Busy : PaintStatus.Fore;
         if (paintStatus is PaintStatus.Fore)
         {
            reset();
         }

         Invalidate();
      }
   }

   public void Display(string message, Color foreColor, Color backColor)
   {
      timer.Enabled = false;
      this.message = message;
      this.foreColor = foreColor;
      this.backColor = backColor;
      reset();
      timer.Enabled = true;
      Invalidate();
   }

   protected void reset()
   {
      foreAlpha = 255;
      backAlpha = 255;
      workingForeColor = foreColor.WithAlpha(foreAlpha);
      workingBackColor = backColor.WithAlpha(backAlpha);
      textRectangle = ClientRectangle;
      paintStatus = PaintStatus.Fore;
   }

   public void Display(string message) => Display(message, Color.White, Color.Blue);

   public void Success(string message) => Display(message, Color.White, Color.Green);

   public void Failure(string message) => Display(message, Color.Black, Color.Gold);

   public void Exception(Exception exception) => Display(exception.Message, Color.White, Color.Red);

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      switch (paintStatus)
      {
         case PaintStatus.Fore:
            writeMessage(e.Graphics);
            break;
         case PaintStatus.Busy:
            workingForeColor = foreColor;
            workingBackColor = backColor;
            writeMessage(e.Graphics);
            break;
      }
   }

   protected void writeMessage(Graphics g)
   {
      var writer = new ControlWriter
      {
         Color = workingForeColor,
         Font = Font,
         Rectangle = textRectangle,
         UseEmojis = UseEmojis,
         AutoSizeText = AutoSizeText
      };

      writer.Write(g, message);
   }

   protected override void OnPaintBackground(PaintEventArgs e)
   {
      base.OnPaintBackground(e);
      e.Graphics.HighQuality();

      switch (paintStatus)
      {
         case PaintStatus.Fore:
         {
            textRectangle = ClientRectangle;
            using var backBrush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(backBrush, textRectangle);
            break;
         }
         case PaintStatus.Back:
         {
            textRectangle = ClientRectangle;
            using var backBrush = new SolidBrush(workingBackColor);
            e.Graphics.FillRectangle(backBrush, textRectangle);
            break;
         }
         case PaintStatus.Busy:
         {
            using var backBrush = new SolidBrush(backColor);
            e.Graphics.FillRectangle(backBrush, ClientRectangle);
            (var busyTextProcessor, _busyTextProcessor) = _busyTextProcessor.Create(getBusyTextProcessor);
            textRectangle = busyTextProcessor.TextRectangle;
            busyTextProcessor.OnPaint(e);
            break;
         }
      }
   }

   protected BusyTextProcessor getBusyTextProcessor() => new(Color.White, ClientRectangle);

   protected void timer_Tick(object sender, EventArgs e)
   {
      switch (paintStatus)
      {
         case PaintStatus.Fore when foreAlpha >= 0:
            workingForeColor = foreColor.WithAlpha(foreAlpha);
            workingBackColor = backColor;
            foreAlpha -= 5;
            break;
         case PaintStatus.Fore:
            paintStatus = PaintStatus.Back;
            break;
         case PaintStatus.Back when backAlpha >= 0:
            workingBackColor = backColor.WithAlpha(backAlpha);
            backAlpha -= 5;
            break;
         case PaintStatus.Back:
            timer.Enabled = false;
            break;
         case PaintStatus.Busy:
         {
            (var busyTextProcessor, _busyTextProcessor) = _busyTextProcessor.Create(getBusyTextProcessor);
            busyTextProcessor.OnTick();
            break;
         }
      }

      Invalidate();
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      if (ClientRectangle.Width != 0 && ClientRectangle.Height != 0)
      {
         textRectangle = ClientRectangle;
         if (_busyTextProcessor)
         {
            (var busyTextProcessor, _busyTextProcessor) = _busyTextProcessor.Create(getBusyTextProcessor);
            textRectangle = busyTextProcessor.TextRectangle;
         }
      }
   }
}