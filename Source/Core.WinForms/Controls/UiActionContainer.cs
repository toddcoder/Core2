using Core.Lists;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiActionContainer : UserControl
{
   protected List<UiAction> uiActions = [];
   protected Maybe<int> _width = nil;
   protected int padding = 3;

   public void Add(UiAction uiAction)
   {
      uiActions.Add(uiAction);
      Controls.Add(uiAction);
      setWidth();
      arrangeUiActions();
      Refresh();
   }

   protected void setWidth()
   {
      var count = uiActions.Count;
      if (count == 0)
      {
         _width = nil;
      }
      else
      {
         _width = (clientWidth() - (count + 1) * padding) / count;
      }
   }

   protected int clientWidth() => ClientSize.Width;

   protected int clientHeight() => ClientSize.Height;

   protected void arrangeUiActions()
   {
      if (_width is (true, var width))
      {
         var left = padding;
         var top = padding;
         var height = clientHeight() - 2 * padding;
         var size = new Size(width, height);
         foreach (var uiAction in uiActions)
         {
            uiAction.Location = new Point(left, top);
            uiAction.Size = size;
            left += width + padding;
            uiAction.Refresh();
         }
      }
   }

   public Maybe<UiAction> this[int index]
   {
      get => uiActions.Get(index);
      set => uiActions.Set(index, value);
   }

   public new int Padding
   {
      get => padding;
      set
      {
         if (value.Between(0).And(10))
         {
            padding = value;
         }
         else
         {
            throw fail("Padding value must be between 0 and 10");
         }
      }
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      setWidth();
      arrangeUiActions();
   }
}