using System.Drawing.Drawing2D;
using Core.Applications.Messaging;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public partial class TempMessage : UserControl
{
   public enum PaintStatus
   {
      Initialize,
      Fore,
      Back,
      Busy
   }

   protected Color foreColor = Color.White;
   protected Color backColor = Color.Blue;
   protected FadingColor fadingForeColor = Color.White;
   protected FadingColor fadingBackColor = Color.Blue;
   protected FadingColor initializeForeColor = SystemColors.ControlText;
   protected string message = "";
   protected PaintStatus paintStatus = PaintStatus.Initialize;
   protected Maybe<BusyTextProcessor> _busyTextProcessor = nil;
   protected Rectangle textRectangle = Rectangle.Empty;

   public readonly MessageEvent<PaintStatus> PaintStatusChanged = new();

   public TempMessage()
   {
      InitializeComponent();

      SetStyle(ControlStyles.UserPaint, true);
      SetStyle(ControlStyles.DoubleBuffer, true);
      SetStyle(ControlStyles.AllPaintingInWmPaint, true);

      PaintStatusChanged.Invoke(paintStatus);
   }

   public bool AutoSizeText { get; set; } = true;

   public bool UseEmojis { get; set; } = true;

   public bool IsBusy
   {
      get => paintStatus == PaintStatus.Busy;
      set
      {
         paintStatus = value ? PaintStatus.Busy : PaintStatus.Fore;
         PaintStatusChanged.Invoke(paintStatus);
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
      fadingForeColor.Reset();
      fadingBackColor.Reset();
      textRectangle = ClientRectangle;
      paintStatus = PaintStatus.Fore;
      PaintStatusChanged.Invoke(paintStatus);
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
         case PaintStatus.Initialize when initializeForeColor.IsFading:
         {
            var smallerRectangle = ClientRectangle.Shrink(10);
            using var pen = new Pen(initializeForeColor);
            pen.DashStyle = DashStyle.Dash;
            e.Graphics.DrawRectangle(pen, smallerRectangle);
            break;
         }
         case PaintStatus.Fore:
            writeMessage(e.Graphics);
            break;
         case PaintStatus.Busy:
            fadingForeColor = foreColor;
            fadingBackColor = backColor;
            writeMessage(e.Graphics);
            break;
      }
   }

   protected void writeMessage(Graphics g)
   {
      var writer = new ControlWriter
      {
         Color = fadingForeColor,
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
         case PaintStatus.Initialize:
         {
            using var brush = new SolidBrush(SystemColors.Control);
            e.Graphics.FillRectangle(brush, ClientRectangle);
            break;
         }
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
            using var backBrush = new SolidBrush(fadingBackColor);
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
         case PaintStatus.Initialize when initializeForeColor.IsFading:
            initializeForeColor.Fade();
            break;
         case PaintStatus.Fore when fadingForeColor.IsFading:
            fadingForeColor.Fade();
            fadingBackColor = backColor;
            break;
         case PaintStatus.Fore:
            paintStatus = PaintStatus.Back;
            PaintStatusChanged.Invoke(paintStatus);
            break;
         case PaintStatus.Back when fadingBackColor.IsFading:
            fadingBackColor.Fade();
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