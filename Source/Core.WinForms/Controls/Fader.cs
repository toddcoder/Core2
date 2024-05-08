using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace Core.WinForms.Controls;

public class Fader
{
   protected const int GWL_EXSTYLE = -20;
   protected const int WS_EX_LAYERED = 0x80000;
   protected const long LWA_ALPHA = 0x2L;

   protected const int RDW_INVALIDATE = 0x0001;
   protected const int RDW_ALLCHILDREN = 0x0080;
   protected const int RDW_UPDATENOW = 0x0100;
   protected const int RDW_FRAME = 0x0400;

   [DllImport("user32.dll", SetLastError = true)]
   protected static extern int GetWindowLong(IntPtr hWnd, int nIndex);

   [DllImport("user32.dll")]
   protected static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

   [DllImport("user32.dll")]
   protected static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

   [DllImport("user32.dll")]
   [return: MarshalAs(UnmanagedType.Bool)]
   protected static extern bool RedrawWindow(IntPtr hWnd, IntPtr ptr1Update, IntPtr ptr2Update, uint flags);

   protected IntPtr handle;
   protected bool isLayered;
   protected int interval = 300;
   protected byte currentAlpha = 255;
   protected byte destinationAlpha = 255;
   protected int start;
   protected int increment = 5;
   protected int totalSteps = 10;
   protected bool up = true;
   protected Timer timer = new();

   public event EventHandler? FadeComplete;

   public Fader(Control control)
   {
      handle = control.Handle;
      timer.Interval = interval;
      timer.Tick += (_, _) =>
      {
         if (up)
         {
            if (start + increment < destinationAlpha)
            {
               start += increment;
            }
            else
            {
               start = destinationAlpha;
            }

            UpdateAlpha((byte)start, true);
         }
         else
         {
            if (start + increment > destinationAlpha)
            {
               start += increment;
            }
            else
            {
               start = destinationAlpha;
            }

            UpdateAlpha((byte)start, true);
         }

         if (currentAlpha == destinationAlpha)
         {
            timer.Stop();
            FadeComplete?.Invoke(this, EventArgs.Empty);
         }
      };
   }

   public bool IsLayered => isLayered;

   public int Interval
   {
      get => interval;
      set
      {
         interval = value;
         timer.Interval = interval;
      }
   }

   public byte DestinationAlpha
   {
      get => destinationAlpha;
      set => destinationAlpha = value;
   }

   public int DestinationPercent
   {
      get => destinationAlpha * 100 / 255;
      set => destinationAlpha = (byte)(255 * value / 100);
   }

   public int CurrentAlpha => currentAlpha;

   public int TotalSteps
   {
      get => totalSteps;
      set => totalSteps = value;
   }

   public void UpdateAlpha(byte alpha, bool forceRefresh)
   {
      if (isLayered)
      {
         SetLayeredWindowAttributes(handle, 0, alpha, (uint)LWA_ALPHA);
         currentAlpha = alpha;

         if (forceRefresh)
         {
            Refresh();
         }
      }
   }

   public void UpdatePercentage(int percentage, bool forceRefresh)
   {
      if (isLayered)
      {
         var alpha = (byte)(255 * percentage / 100);
         UpdateAlpha(alpha, forceRefresh);
      }
   }

   public void Refresh() => RedrawWindow(handle, IntPtr.Zero, IntPtr.Zero, RDW_FRAME | RDW_UPDATENOW | RDW_INVALIDATE | RDW_ALLCHILDREN);

   public void SetTransparentLayeredWindow()
   {
      if (!isLayered)
      {
         SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
         SetLayeredWindowAttributes(handle, 0, currentAlpha, (uint)LWA_ALPHA);

         isLayered = true;
      }
   }

   public void ClearTransparentLayeredWindow()
   {
      if (isLayered)
      {
         SetWindowLong(handle, GWL_EXSTYLE, GetWindowLong(handle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
         isLayered = false;
      }
   }

   protected void StartToDestination()
   {
      if (isLayered)
      {
         if (currentAlpha < destinationAlpha)
         {
            up = true;
         }
         else if (currentAlpha > destinationAlpha)
         {
            up = false;
         }
         else
         {
            return;
         }

         increment = (destinationAlpha - currentAlpha) / totalSteps;

         timer.Interval = interval;
         timer.Start();
      }
   }

   public void Start(byte destinationAlpha)
   {
      if (isLayered)
      {
         this.destinationAlpha = destinationAlpha;
         start = currentAlpha;
         StartToDestination();
      }
   }

   public void StartPercent(int percent)
   {
      if (isLayered)
      {
         destinationAlpha = (byte)(255 * percent / 100);
         start = currentAlpha;
         StartToDestination();
      }
   }
}