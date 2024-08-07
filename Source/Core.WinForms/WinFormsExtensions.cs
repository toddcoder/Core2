﻿namespace Core.WinForms;

public static class WinFormsExtensions
{
   public static void Do(this Control control, Action action)
   {
      try
      {
         if (!control.IsDisposed)
         {
            if (control.InvokeRequired)
            {
               control.Invoke(action);
            }
            else
            {
               action();
            }
         }
      }
      catch
      {
      }
   }

   public static T Get<T>(this Control control, Func<T> func)
   {
      try
      {
         if (!control.IsDisposed)
         {
            if (control.InvokeRequired)
            {
               return control.Invoke(func);
            }
            else
            {
               return func();
            }
         }
         else
         {
            return func();
         }
      }
      catch
      {
         return func();
      }
   }

   public static void Tuck(this Form form, Control bottommostControl, int margin = 3)
   {
      var bottom = bottommostControl.Bottom;
      form.ClientSize = form.ClientSize with { Height = bottom + margin };
   }
}