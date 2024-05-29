using System.Drawing.Drawing2D;
using Core.Lists;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiActionContainer : UserControl
{
   protected List<UiAction> uiActions = [];
   protected Maybe<int> _width = nil;
   protected Maybe<int> _height = nil;
   protected int padding = 3;
   protected UiActionDirection direction = UiActionDirection.Horizontal;
   protected bool showLastClicked = true;
   protected Maybe<int> _indexLastClicked = nil;

   public void Add(UiAction uiAction)
   {
      var index = uiActions.Count;

      uiActions.Add(uiAction);
      Controls.Add(uiAction);
      if (showLastClicked)
      {
         uiAction.Click += (_, _) =>
         {
            _indexLastClicked = index;
            Invalidate();
         };
      }
      resize();
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

   protected void setHeight()
   {
      var count = uiActions.Count;
      if (count == 0)
      {
         _height = nil;
      }
      else
      {
         _height = (clientHeight() - (count + 1) * padding) / count;
      }
   }

   protected int clientWidth() => ClientSize.Width;

   protected int clientHeight() => ClientSize.Height;

   protected void arrangeUiActions()
   {
      switch (direction)
      {
         case UiActionDirection.Horizontal when _width is (true, var width):
            arrangeHorizontal(width);
            break;
         case UiActionDirection.Vertical when _height is (true, var height):
            arrangeVertical(height);
            break;
      }
   }

   protected void arrangeHorizontal(int width)
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

   protected void arrangeVertical(int height)
   {
      var left = padding;
      var top = padding;
      var width = clientWidth() - 2 * padding;
      var size = new Size(width, height);

      foreach (var uiAction in uiActions)
      {
         uiAction.Location = new Point(left, top);
         uiAction.Size = size;
         top += height + padding;
         uiAction.Refresh();
      }
   }

   protected void resize()
   {
      setWidth();
      setHeight();
      arrangeUiActions();
   }

   public Maybe<UiAction> this[int index]
   {
      get => uiActions.Get(index);
      set
      {
         uiActions.Set(index, value);
         if (!value)
         {
            resize();
         }
      }
   }

   public new int Padding
   {
      get => padding;
      set
      {
         if (value.Between(0).And(10))
         {
            padding = value;
            resize();
         }
         else
         {
            throw fail("Padding value must be between 0 and 10");
         }
      }
   }

   public UiActionDirection Direction
   {
      get => direction;
      set
      {
         direction = value;
         resize();
      }
   }

   public bool ShowLastClicked
   {
      get => showLastClicked;
      set
      {
         if (padding > 0)
         {
            showLastClicked = value;
            resize();
         }
      }
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      resize();
   }

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      if (showLastClicked && _indexLastClicked is (true, var index))
      {
         var uiAction = uiActions[index];
         var location = uiAction.Location.Reposition(-1, -1);
         var size = uiAction.Size.Resize(2, 2);
         var rectangle = new Rectangle(location, size);
         using var pen = new Pen(Color.Black);
         pen.DashStyle = DashStyle.Dash;
         e.Graphics.DrawRectangle(pen, rectangle);
      }
   }
}