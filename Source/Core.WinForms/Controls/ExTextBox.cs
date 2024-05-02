﻿using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Core.Applications;
using Core.Collections;
using Core.DataStructures;
using Core.Matching;
using Core.Monads;
using Core.Strings;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ExTextBox : TextBox, ISubTextHost
{
   public class WindowExtender : NativeWindow
   {
      protected const int WM_PAINT = 15;

      protected ExTextBox baseControl;
      protected Bitmap? canvas;
      protected Graphics? bufferGraphics;
      protected Rectangle bufferClip;
      protected Graphics? controlGraphics;
      protected bool canRender;

      public WindowExtender(ExTextBox baseControl)
      {
         this.baseControl = baseControl;

         canRender = false;
         ReinitializeCanvas();
      }

      protected override void WndProc(ref Message message)
      {
         if (message.Msg == WM_PAINT)
         {
            baseControl.Invalidate();
            base.WndProc(ref message);
            onPerformPaint();
         }
         else
         {
            base.WndProc(ref message);
         }
      }

      protected void onPerformPaint()
      {
         if (canRender)
         {
            bufferGraphics!.Clear(Color.Transparent);
            baseControl.OnPaint(new PaintEventArgs(bufferGraphics, bufferClip));
            controlGraphics!.DrawImageUnscaled(canvas!, 0, 0);
         }
      }

      public void ReinitializeCanvas()
      {
         lock (this)
         {
            TearDown();
            canRender = baseControl is { Width: > 0, Height: > 0 };

            if (canRender)
            {
               canvas = new Bitmap(baseControl.Width, baseControl.Height);
               bufferGraphics = Graphics.FromImage(canvas);
               bufferClip = new Rectangle(0, 0, canvas.Width, canvas.Height);
               bufferGraphics.Clip = new Region(bufferClip);
               controlGraphics = Graphics.FromHwnd(baseControl.Handle);
            }
         }
      }

      public void TearDown()
      {
         controlGraphics?.Dispose();
         bufferGraphics?.Dispose();
         canvas?.Dispose();
      }
   }

   protected WindowExtender windowExtender;
   protected int updatingCount;
   protected Maybe<Func<string, bool>> _allow = nil;
   protected Maybe<Func<string, bool>> _trend = nil;
   protected Maybe<Func<string, bool>> _deny = nil;
   protected string allowMessage = "allowed";
   protected string trendMessage = "trending";
   protected string denyMessage = "denied";
   protected Hash<Guid, SubText> subTexts = [];
   protected MaybeStack<SubText> legends = [];
   protected Maybe<SubText> _lastSubText = nil;
   protected Maybe<int> _leftMargin = nil;
   protected string cueBanner = string.Empty;
   protected Maybe<SubText> _validateSubText = nil;
   protected bool validatesMessages;

   public new event EventHandler<PaintEventArgs>? Paint;
   public new event EventHandler<ValidatingArgs>? Validating;
   public event EventHandler<AllowArgs>? Allowing;
   public event EventHandler<DenyArgs>? Denying;
   public event EventHandler<TrendArgs>? Trending;

   public ExTextBox(Control control) : this()
   {
      control.Controls.Add(this);
   }

   public ExTextBox()
   {
      windowExtender = new WindowExtender(this);
      windowExtender.AssignHandle(Handle);

      TextChanged += (_, _) => validateText();
      GotFocus += (_, _) =>
      {
         if (AutoSelectAll)
         {
            SelectAll();
         }
      };
      MouseClick += (_, _) =>
      {
         if (AutoSelectAll)
         {
            SelectAll();
         }
      };
   }

   public bool AutoSelectAll { get; set; } = false;

   protected void validateText()
   {
      if (Validating is not null)
      {
         var status = ValidStatus;
         switch (status)
         {
            case AllowanceStatus.Allowed:
               Allowing?.Invoke(this, new AllowArgs(Text, true));
               break;
            case AllowanceStatus.Trending:
               Trending?.Invoke(this, new TrendArgs(Text, true));
               break;
            case AllowanceStatus.Denied:
               Denying?.Invoke(this, new DenyArgs(Text, true));
               break;
         }

         if (validatesMessages)
         {
            (var validateSubText, _validateSubText) = _validateSubText.Create(() =>
               SubText("", Color.White, Color.Black).Set.FontSize(8).Alignment(CardinalAlignment.NorthEast).SubText);

            switch (status)
            {
               case AllowanceStatus.Allowed:
                  validateSubText.Text = allowMessage;
                  validateSubText.ForeColor = Color.White;
                  validateSubText.BackColor = Color.Green;
                  break;
               case AllowanceStatus.Trending:
                  validateSubText.Text = trendMessage;
                  validateSubText.ForeColor = Color.Black;
                  validateSubText.BackColor = Color.Yellow;
                  break;
               case AllowanceStatus.Denied:
                  validateSubText.Text = denyMessage;
                  validateSubText.ForeColor = Color.White;
                  validateSubText.BackColor = Color.Red;
                  break;
            }
         }

         return;
      }

      Allowing?.Invoke(this, new AllowArgs(Text, IsAllowed));
      Denying?.Invoke(this, new DenyArgs(Text, IsDenied));
      Trending?.Invoke(this, new TrendArgs(Text, IsTrending));
   }

   public bool ValidateMessages
   {
      get => validatesMessages;
      set
      {
         validatesMessages = value;
         validateText();
      }
   }

   public Maybe<Func<string, bool>> Allow
   {
      get => _allow;
      set => _allow = value;
   }

   public Maybe<Func<string, bool>> Trend
   {
      get => _trend;
      set => _trend = value;
   }

   public Maybe<Func<string, bool>> Deny
   {
      get => _deny;
      set => _deny = value;
   }

   public AllowanceStatus ValidStatus
   {
      get
      {
         if (Validating is not null)
         {
            var args = new ValidatingArgs(Text, AllowanceStatus.None);
            Validating.Invoke(this, args);
            return args.Status;
         }
         else
         {
            return AllowanceStatus.None;
         }
      }
   }

   public bool IsAllowed => _allow.Map(allowed => allowed(Text)) | true;

   public bool IsDenied => _deny.Map(deny => deny(Text)) | false;

   public bool IsTrending => _trend.Map(trend => trend(Text)) | false;

   public string AllowMessage
   {
      get => allowMessage;
      set => allowMessage = value;
   }

   public string TrendMessage
   {
      get => trendMessage;
      set => trendMessage = value;
   }

   public string DenyMessage
   {
      get => denyMessage;
      set => denyMessage = value;
   }

   public Maybe<int> LeftMargin
   {
      get => _leftMargin;
      set => _leftMargin = value;
   }

   public int GetLeftMargin() => (int)User32.SendMessage(Handle, User32.Messages.GetMargins, User32.LEFT_MARGIN, 0);

   public SubText SubText(string text, Color foreColor, Color backColor)
   {
      var subText = new SubText(this, text, 0, 0, ClientSize, false)
         .Set
         .ForeColor(foreColor)
         .BackColor(backColor)
         .FontName("Consolas")
         .FontSize(Font.Size)
         .SubText;
      if (subTexts.Count == 0)
      {
         subText.SetAlignment(CardinalAlignment.NorthEast);
      }
      else if (_lastSubText is (true, var lastSubText))
      {
         subText.LeftOf(lastSubText);
         subText.SetLocation(ClientRectangle);
         _lastSubText = subText;
      }

      subText.SetLocation(ClientRectangle);
      var rightMargin = ClientSize.Width - subText.TextSize(nil).measuredSize.Width;
      User32.SendMessage(Handle, User32.Messages.SetMargins, User32.RIGHT_MARGIN, rightMargin - 2);
      subTexts[subText.Id] = subText;

      return subText;
   }

   public void RemoveSubText(Guid id)
   {
      subTexts.Remove(id);
      Refresh();
   }

   public void RemoveSubText(SubText subText) => RemoveSubText(subText.Id);

   public void ClearSubTexts()
   {
      subTexts.Clear();
      Refresh();
   }

   public SubText Legend(string text)
   {
      var legend = new SubText(this, text, 0, 0, ClientSize, false)
         .Set
         .FontName("Consolas")
         .FontSize(9)
         .ForeColor(BackColor)
         .BackColor(ForeColor)
         .Alignment(CardinalAlignment.NorthWest)
         .SubText;
      legends.Push(legend);

      (var leftMargin, _leftMargin) = _leftMargin.Create(() =>
      {
         using var g = CreateGraphics();
         var length = legend.TextSize(g).measuredSize.Width;
         return length + 2;
      });

      User32.SendMessage(Handle, User32.Messages.SetMargins, User32.LEFT_MARGIN, leftMargin);

      return legend;
   }

   public void Legend()
   {
      legends.Pop();
      Refresh();
   }

   public void ClearAllLegends()
   {
      legends.Clear();
      Refresh();
   }

   public Maybe<string> ShadowText { get; set; } = nil;

   public bool RefreshOnTextChange { get; set; }

   public void StopUpdating()
   {
      if (updatingCount == 0)
      {
         User32.SendMessage(Handle, User32.Messages.SetRedraw, false, 0);
      }

      updatingCount++;
   }

   public void ResumeUpdating()
   {
      if (--updatingCount == 0)
      {
         User32.SendMessage(Handle, User32.Messages.SetRedraw, true, 0);
      }
   }

   public void ResetUpdating() => updatingCount = 0;

   public (int start, int length) Selection
   {
      get => (SelectionStart, SelectionLength);
      set => Select(value.start, value.length);
   }

   protected override void Dispose(bool disposing)
   {
      windowExtender.ReleaseHandle();
      windowExtender.TearDown();

      base.Dispose(disposing);
   }

   public string CueBanner
   {
      get => cueBanner;
      set
      {
         cueBanner = value;
         User32.SendMessage(Handle, User32.Messages.SetCueBanner, true, cueBanner);
      }
   }

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
      e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
      e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
      e.Graphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
      e.Graphics.CompositingMode = CompositingMode.SourceCopy;

      if (ShadowText is (true, var shadowText))
      {
         var text = Text;
         shadowText = shadowText.Drop(text.Length);
         if (shadowText.Length > 0)
         {
            var textSize = text.IsNotEmpty() ? MeasureString(e.Graphics, text, Font) : Size.Empty;

            var shadowSize = MeasureString(e.Graphics, shadowText, Font);
            var bitmap = new Bitmap(shadowSize.Width, shadowSize.Height);
            using var g = Graphics.FromImage(bitmap);
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
            TextRenderer.DrawText(g, shadowText, Font, new Point(0, 0), SystemColors.GrayText, BackColor);

            var left = (legends.Peek().Map(l => l.TextSize(e.Graphics).measuredSize.Width + 2) | 0) + textSize.Width;
            var top = 0;
            e.Graphics.DrawImage(bitmap, left, top);
         }
      }

      drawAllSubTexts(e.Graphics, e.ClipRectangle);

      Paint?.Invoke(this, e);
   }

   protected void drawAllSubTexts(Graphics g, Rectangle clientRectangle)
   {
      g.CompositingQuality = CompositingQuality.HighQuality;
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.PixelOffsetMode = PixelOffsetMode.HighQuality;
      g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
      var _legend = legends.Peek();
      if (_legend is (true, var legend))
      {
         legend.Draw(g, legend.ForeColor | Color.White, legend.BackColor | Color.Black);
      }

      foreach (var subText in subTexts.Values)
      {
         subText.SetLocation(clientRectangle);
         subText.Draw(g, subText.ForeColor | Color.White, subText.BackColor | Color.Black);
      }
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      windowExtender.ReinitializeCanvas();
   }

   public void ReassignHandle() => windowExtender.AssignHandle(Handle);

   public static Size MeasureString(Graphics graphics, string text, Font font)
   {
      if (text.IsEmpty())
      {
         return new Size(0, 0);
      }
      else if (text.Contains(" "))
      {
         var size = graphics.MeasureString(text, font).ToSize();
         return size with { Width = size.Width + 8 };
      }
      else
      {
         var size = graphics.MeasureString(text, font);
         CharacterRange[] ranges = [new CharacterRange(0, text.Length)];
         var format = new StringFormat();
         format.SetMeasurableCharacterRanges(ranges);
         var regions = graphics.MeasureCharacterRanges(text, font, new RectangleF(0, 0, size.Width, size.Height), format);
         var rectangle = regions[0].GetBounds(graphics);

         return rectangle.Size.ToSize();
      }
   }

   public Rectangle RectangleFrom(Graphics graphics, int start, int length = -1, bool expand = false)
   {
      if (length == -1)
      {
         length = Text.Length;
      }

      var text = Text.Drop(start).Keep(length);
      var size = MeasureString(graphics, text, Font);
      if (expand)
      {
         size = size with { Width = Width };
      }

      var location = GetPositionFromCharIndex(start);
      if (expand)
      {
         location = new Point(0, location.X);
      }

      return new Rectangle(location, size);
   }

   public Rectangle RectangleFromCurrentSelection(Graphics graphics, bool expand = false)
   {
      return RectangleFrom(graphics, SelectionStart, SelectionLength, expand);
   }

   public IEnumerable<(Rectangle rectangle, string word)> RectangleWords(Graphics graphics)
   {
      var _result = Text.Matches("/w+; f");
      if (_result is (true, var result))
      {
         foreach (var (text, index, length) in result)
         {
            var rectangle = RectangleFrom(graphics, index, length);
            yield return (rectangle, text);
         }
      }
   }

   public IEnumerable<(int start, int length)> Words()
   {
      var _result = Text.Matches("/w+; f");
      if (_result is (true, var result))
      {
         foreach (var (_, index, length) in result)
         {
            yield return (index, length);
         }
      }
   }

   public Maybe<(int start, int length)> WordAtSelection()
   {
      var text = Text;
      var (start, _) = Selection;
      if (text.IsEmpty())
      {
         return nil;
      }
      else if (start < text.Length && char.IsLetterOrDigit(text, start))
      {
         var i = start;
         for (; i > -1 && char.IsLetterOrDigit(text, i); i--)
         {
         }

         i++;
         return maybe<(int start, int length)>() & text.Drop(i).Matches("/w+; f").Map(r => (r.Index + i, r.Length));
      }
      else
      {
         return nil;
      }
   }

   public IEnumerable<(char, Rectangle)> RectangleWhitespace(Graphics graphics)
   {
      var _result = Text.Matches("[' /t']; f");
      if (_result is (true, var result))
      {
         foreach (var (text, index, length) in result)
         {
            var rectangle = RectangleFrom(graphics, index, length);
            yield return (text[0], rectangle);
         }
      }
   }

   public Maybe<Rectangle> WordAtSelection(Graphics graphics, int start, int length)
   {
      if (length == 0)
      {
         return WordAtSelection(graphics, start);
      }
      else if (Text.IsEmpty() || start >= Text.Length)
      {
         return nil;
      }
      else
      {
         var segment = Text.Drop(start).Keep(length);
         return segment.Matches("/w+; f").Map(result => RectangleFrom(graphics, result.Index + start, result.Length));
      }
   }

   public Maybe<Rectangle> WordAtSelection(Graphics graphics, int start)
   {
      var text = Text;
      if (text.IsEmpty() || start >= text.Length)
      {
         return nil;
      }
      else if (char.IsLetterOrDigit(text, start))
      {
         var i = start;
         for (; i > -1 && char.IsLetterOrDigit(text, i); i--)
         {
         }

         i++;
         return text.Drop(i).Matches("/w+; f").Map(result => RectangleFrom(graphics, result.Index + i, result.Length));
      }
      else
      {
         return nil;
      }
   }

   public Maybe<Rectangle> WordAtCurrentSelection(Graphics graphics)
   {
      return WordAtSelection(graphics, SelectionStart, SelectionLength);
   }

   public void DrawHighlight(Graphics graphics, Rectangle rectangle, Color foreColor, Color backColor, DashStyle dashStyle = DashStyle.Solid)
   {
      using var brush = new SolidBrush(Color.FromArgb(30, backColor));
      graphics.FillRectangle(brush, rectangle);

      using var pen = new Pen(foreColor);
      pen.DashStyle = dashStyle;
      graphics.DrawRectangle(pen, rectangle);
   }

   public void DrawWavyUnderline(Graphics graphics, Rectangle rectangle, Color color)
   {
      using var brush = new HatchBrush(HatchStyle.ZigZag, color, Color.Transparent);
      using var pen = new Pen(brush, 2f);
      graphics.DrawLine(pen, rectangle.Left, rectangle.Bottom, rectangle.Right, rectangle.Bottom);
   }

   public void DrawStrikeout(Graphics graphics, Rectangle rectangle, Color color, DashStyle dashStyle = DashStyle.Solid, int alpha = 255,
      float penSize = 2f)
   {
      using var pen = new Pen(Color.FromArgb(alpha, color), penSize);
      pen.DashStyle = dashStyle;
      var y = (rectangle.Bottom - rectangle.Top) / 2;
      graphics.DrawLine(pen, rectangle.Left, y, rectangle.Right, y);
   }

   public void DrawLineOnTop(Graphics graphics, Rectangle rectangle, Color color, DashStyle dashStyle = DashStyle.Solid, int alpha = 255,
      float penSize = 2f)
   {
      using var pen = new Pen(Color.FromArgb(alpha, color), penSize);
      pen.DashStyle = dashStyle;
      var y = rectangle.Top;
      graphics.DrawLine(pen, rectangle.Left, y, rectangle.Right, y);
   }

   public void DrawLineOnBottom(Graphics graphics, Rectangle rectangle, Color color, DashStyle dashStyle = DashStyle.Solid, int alpha = 255,
      float penSize = 2f)
   {
      using var pen = new Pen(Color.FromArgb(alpha, color), penSize);
      pen.DashStyle = dashStyle;
      var y = rectangle.Bottom;
      graphics.DrawLine(pen, rectangle.Left, y, rectangle.Right, y);
   }

   public void DrawSideLines(Graphics graphics, Rectangle rectangle, Color color, DashStyle dashStyle = DashStyle.Solid, int alpha = 255,
      float penSize = 2f)
   {
      using var pen = new Pen(Color.FromArgb(alpha, color), penSize);
      pen.DashStyle = dashStyle;
      graphics.DrawLine(pen, rectangle.Left - 2, rectangle.Top, rectangle.Left - 2, rectangle.Bottom);
      graphics.DrawLine(pen, rectangle.Right + 2, rectangle.Top, rectangle.Right + 2, rectangle.Bottom);
   }

   public void BoxRectangle(Graphics graphics, Rectangle rectangle, Color boxColor, Color backColor, DashStyle dashStyle = DashStyle.Solid,
      int alpha = 64, float penSize = 2f, int widthOffset = 4, int heightOffset = 2)
   {
      using var brush = new SolidBrush(Color.FromArgb(alpha, backColor));
      rectangle = rectangle.Resize(widthOffset, heightOffset).OffsetX(-widthOffset / 2).OffsetY(heightOffset / 2);
      graphics.FillRectangle(brush, rectangle);
      if (penSize > 0)
      {
         using var pen = new Pen(boxColor, penSize);
         pen.DashStyle = dashStyle;
         graphics.DrawRectangle(pen, rectangle);
      }
   }

   public void DrawWhitespace(Graphics graphics)
   {
      var brush = Brushes.Gray;
      var format = new StringFormat
      {
         LineAlignment = StringAlignment.Center,
         Alignment = StringAlignment.Center
      };
      foreach (var (ch, rectangle) in RectangleWhitespace(graphics))
      {
         graphics.DrawString(ch == '\t' ? "→" : "°", Font, brush, rectangle, format);
      }
   }

   public IEnumerable<Rectangle> RectanglesFromSelection(Graphics graphics, int start, int length)
   {
      if (Text.IsNotEmpty())
      {
         var text = Text.Drop(start).Keep(length);
         if (text.IsEmpty())
         {
            yield break;
         }

         if (text.Contains("\n"))
         {
            var offset = 0;

            foreach (var i in text.FindAll("\n"))
            {
               var strLength = i - offset - 2;
               yield return RectangleFrom(graphics, start + offset, strLength);

               offset = i + 1;
            }

            yield return RectangleFrom(graphics, start + offset, text.Length - offset - 2);
         }
         else
         {
            yield return RectangleFrom(graphics, start, length);
         }
      }
   }

   protected override void OnTextChanged(EventArgs e)
   {
      if (RefreshOnTextChange)
      {
         Refresh();
      }

      base.OnTextChanged(e);
   }

   protected override void OnMouseClick(MouseEventArgs e)
   {
      base.OnMouseClick(e);

      if (RefreshOnTextChange)
      {
         Refresh();
      }
   }

   protected override void OnKeyPress(KeyPressEventArgs e)
   {
      base.OnKeyPress(e);

      if (RefreshOnTextChange)
      {
         Refresh();
      }
   }

   protected override void OnKeyDown(KeyEventArgs e)
   {
      base.OnKeyDown(e);

      if (RefreshOnTextChange)
      {
         Refresh();
      }
   }

   protected override void OnKeyUp(KeyEventArgs e)
   {
      base.OnKeyUp(e);

      if (RefreshOnTextChange)
      {
         Refresh();
      }
   }

   public Maybe<SubText> CurrentLegend => legends.Peek();
}