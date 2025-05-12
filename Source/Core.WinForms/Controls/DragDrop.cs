using Core.Applications.Messaging;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class DragDrop<TControl> where TControl : Control
{
   public record PresentData(string DataFormat, string Text);

   public static DragDrop<TControl> operator +(DragDrop<TControl> dragDrop, string dataFormat)
   {
      dragDrop.Add(dataFormat);
      return dragDrop;
   }

   protected TControl control;
   protected StringSet dataFormats = [];
   protected string firstDataFormat = "";

   public readonly MessageEvent<(TControl control, string dataFormat)> DraggedOver = new();
   public readonly MessageEvent<(TControl control, string[] dataFormats)> NotDraggedOver = new();
   public readonly MessageEvent<(TControl control, PresentData presentData)> DragDropped = new();
   public readonly MessageEvent<TControl> DataNotPresent = new();

   public DragDrop(TControl control)
   {
      this.control = control;
      this.control.AllowDrop = true;
   }

   public void Add(string dataFormat)
   {
      dataFormats.Add(dataFormat);
      if (dataFormats.Count == 1)
      {
         firstDataFormat = dataFormat;
      }
   }

   public void Wire()
   {
      switch (dataFormats.Count)
      {
         case 1:
            control.DragOver += (_, e) =>
            {
               if (dataPresent(e, firstDataFormat))
               {
                  e.Effect = DragDropEffects.Copy;
                  DraggedOver.Invoke((control, firstDataFormat));
               }
               else
               {
                  e.Effect = DragDropEffects.None;
                  NotDraggedOver.Invoke((control, getFormats(e)));
               }
            };
            control.DragDrop += (_, e) =>
            {
               if (dataPresent(e, firstDataFormat))
               {
                  var dataPresent = new PresentData(firstDataFormat, getData(e, firstDataFormat));
                  DragDropped.Invoke((control, dataPresent));
               }
               else
               {
                  DataNotPresent.Invoke(control);
               }
            };
            break;
         case > 1:
            control.DragOver += (_, e) =>
            {
               var _dataFormat = dataFormats.FirstOrNone(format => dataPresent(e, format));
               if (_dataFormat is (true, var dataFormat))
               {
                  e.Effect = DragDropEffects.Copy;
                  DraggedOver.Invoke((control, dataFormat));
                  return;
               }

               e.Effect = DragDropEffects.None;
               NotDraggedOver.Invoke((control, getFormats(e)));
            };
            control.DragDrop += (_, e) =>
            {
               foreach (var dataFormat in dataFormats.Where(dataFormat => dataPresent(e, dataFormat)))
               {
                  DragDropped.Invoke((control, new PresentData(dataFormat, getData(e, dataFormat))));
                  return;
               }

               DataNotPresent.Invoke(control);
            };
            break;
      }

      return;

      bool dataPresent(DragEventArgs e, string dataFormat) => e.Data?.GetDataPresent(dataFormat) ?? false;

      Maybe<string> getData(DragEventArgs e, string dataFormat)
      {
         if (e.Data is not null)
         {
            var data = e.Data.GetData(dataFormat);
            if (data is not null)
            {
               return data.ToString() ?? "";
            }
         }

         return nil;
      }

      string[] getFormats(DragEventArgs e) => e.Data?.GetFormats() ?? [];
   }
}