using System.Runtime.InteropServices;
using Core.Applications.Messaging;

namespace Core.WinForms;

public static class FormExtensions
{
   [StructLayout(LayoutKind.Sequential)]
   private struct FlashWindowInfo
   {
      public uint cbSize;
      public IntPtr hwnd;
      public uint dwFlags;
      public uint uCount;
      public uint dwTimeout;
   }

   [DllImport("user32.dll")]
   private static extern bool FlashWindowEx(ref FlashWindowInfo fwi);

   public const uint FLASHW_STOP = 0;
   public const uint FLASHW_CAPTION = 1;
   public const uint FLASHW_TRAY = 2;
   public const uint FLASHW_ALL = 3;
   public const uint FLASHW_TIMER = 4;
   public const uint FLASHW_TIMERNOFG = 12;

   private static FlashWindowInfo createFlashWindowInfo(IntPtr handle, uint flags, uint count, uint timeout)
   {
      var fi = new FlashWindowInfo();
      fi.cbSize = Convert.ToUInt32(Marshal.SizeOf(fi));
      fi.hwnd = handle;
      fi.dwFlags = flags;
      fi.uCount = count;
      fi.dwTimeout = timeout;

      return fi;
   }

   extension(Form form)
   {
      public bool Flash()
      {
         var fi = createFlashWindowInfo(form.Handle, FLASHW_ALL | FLASHW_TIMERNOFG, uint.MaxValue, 0);
         return FlashWindowEx(ref fi);
      }

      public bool Flash(uint count)
      {
         var fi = createFlashWindowInfo(form.Handle, FLASHW_ALL, count, 0);
         return FlashWindowEx(ref fi);
      }

      public bool StartFlashing() => form.Flash(uint.MaxValue);

      public bool StopFlashing()
      {
         var fi = createFlashWindowInfo(form.Handle, FLASHW_STOP, uint.MaxValue, 0);
         return FlashWindowEx(ref fi);
      }
   }

   extension<TPayload>(Subscriber<TPayload> subscriber) where TPayload : notnull
   {
      public void UnsubscribeOnClose(Form form)
      {
         form.FormClosed += (_, _) => subscriber.Unsubscribe();
      }
   }

   extension<TPayload>(SubscriberServer<TPayload> subscriber) where TPayload : notnull
   {
      public void UnsubscribeOnClose(Form form)
      {
         form.FormClosed += (_, _) => subscriber.Dispose();
      }
   }

   extension<TTopic, TPayload>(Subscriber<TTopic, TPayload> subscriber) where TTopic : notnull
      where TPayload : notnull
   {
      public void UnsubscribeOnClose(Form form)
      {
         form.FormClosed += (_, _) => subscriber.Unsubscribe();
      }
   }

   extension(Subscriber subscriber)
   {
      public void UnsubscribeOnClose(Form form)
      {
         form.FormClosed += (_, _) => subscriber.Unsubscribe();
      }
   }

   extension(SubscriberServer subscriber)
   {
      public void UnsubscribeOnClose(Form form)
      {
         form.FormClosed += (_, _) => subscriber.Dispose();
      }
   }

   extension(XSubscriber subscriber)
   {
      public void StopListeningOnClose(Form form)
      {
         form.FormClosed += (_, _) => subscriber.StopListening();
      }
   }
}