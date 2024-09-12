using System.Runtime.InteropServices;

namespace Core.WinForms.Controls;

public class Scroller(IntPtr handle, int maximum, uint page)
{
   [Serializable, StructLayout(LayoutKind.Sequential)]
   protected struct ScrollInfo
   {
      public uint size;
      public uint mask;
      public int min;
      public int max;
      public uint page;
      public int pos;
      public int trackPos;
   }

   protected const int SB_VERTICAL = 0x1;
   protected const int SIF_RANGE = 0x1;
   protected const int SIF_PAGE = 0x2;

   [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
   [return: MarshalAs(UnmanagedType.Bool)]
   protected static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

   [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
   protected static extern bool SetScrollInfo(IntPtr hWnd, int nBar, ref ScrollInfo info, bool redraw);

   [DllImport("user32.dll")]
   protected static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, ref ScrollInfo info);

   public void Set()
   {
      ShowScrollBar(handle, SB_VERTICAL, true);

      var info = new ScrollInfo
      {
         mask = SIF_RANGE | SIF_PAGE,
         size = (uint)Marshal.SizeOf(typeof(ScrollInfo))
      };
      GetScrollInfo(handle, SB_VERTICAL, ref info);
      info.max = maximum;
      info.page = page;
      SetScrollInfo(handle, SB_VERTICAL, ref info, true);
   }
}