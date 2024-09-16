using System.Runtime.InteropServices;

namespace Core.WinForms.Controls;

public class CoreDateTimePicker : DateTimePicker
{
   [StructLayout(LayoutKind.Sequential)]
   protected struct Rect
   {
      public int L, T, R, B;
   }

   [StructLayout(LayoutKind.Sequential)]
   protected struct DateTimePickerInfo
   {
      public int cbSize;
      public Rect rcCheck;
      public int stateCheck;
      public Rect rcButton;
      public int stateButton;
      public IntPtr hwndEdit;
      public IntPtr hwndUD;
      public IntPtr hwndDropDown;
   }

   protected const int WM_PAINT = 0xF;
   protected const int DTM_FIRST = 0x1000;
   protected const int DTM_GET_DATE_TIME_PICKER_INFO = DTM_FIRST + 14;

   [DllImport("user32.dll")]
   protected static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, ref DateTimePickerInfo info);

   public CoreDateTimePicker()
   {
      SetStyle(ControlStyles.ResizeRedraw | ControlStyles.OptimizedDoubleBuffer, true);
   }

   protected override void WndProc(ref Message m)
   {
      base.WndProc(ref m);

      if (m.Msg == WM_PAINT)
      {
         var info = new DateTimePickerInfo();
         info.cbSize = Marshal.SizeOf(info);
         SendMessage(Handle, DTM_GET_DATE_TIME_PICKER_INFO, IntPtr.Zero, ref info);
         using var g = Graphics.FromHwndInternal(Handle);
         var clientRect = new Rectangle(0, 0, Width, Height);
         var buttonWidth = info.rcButton.R - info.rcButton.L;
         var dropDownRect = new Rectangle(info.rcButton.L, info.rcButton.T,
            buttonWidth, clientRect.Height);
         if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
         {
            dropDownRect.X = clientRect.Width - dropDownRect.Right;
            dropDownRect.Width += 1;
         }

         var middle = new Point(dropDownRect.Left + dropDownRect.Width / 2,
            dropDownRect.Top + dropDownRect.Height / 2);
         var arrow = new[]
         {
            new Point(middle.X - 3, middle.Y - 2),
            new Point(middle.X + 4, middle.Y - 2),
            middle with { Y = middle.Y + 2 }
         };

         var borderAndButtonColor = SystemColors.Control;
         using (var pen = new Pen(borderAndButtonColor))
         {
            g.DrawRectangle(pen, 0, 0,
               clientRect.Width - 1, clientRect.Height - 1);
         }

         using (var brush = new SolidBrush(borderAndButtonColor))
         {
            g.FillRectangle(brush, dropDownRect);
         }

         g.FillPolygon(Brushes.Black, arrow);
      }
   }
}