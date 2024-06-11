using System.Collections;
using System.Drawing.Drawing2D;
using Core.Enumerables;
using Core.Lists;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiActionContainer : UserControl, IEnumerable<UiAction>
{
   public static UiActionContainer HorizontalContainer() => new();

   public static UiActionContainer VerticalContainer() => new() { Direction = UiActionDirection.Vertical };

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

   public UiAction Add(string caption)
   {
      var uiAction = new UiAction(this);
      uiAction.Button(caption);

      Add(uiAction);

      return uiAction;
   }

   public UiAction Add(string caption, bool isChecked)
   {
      var uiAction = new UiAction(this);
      uiAction.CheckBox(caption, isChecked);

      Add(uiAction);

      return uiAction;
   }

   protected void setWidth()
   {
      var count = uiActions.Count;

      _width = direction switch
      {
         UiActionDirection.Horizontal when count == 0 => nil,
         UiActionDirection.Horizontal => (clientWidth() - (count + 1) * padding) / count,
         UiActionDirection.Vertical => clientWidth() - 2 * padding,
         _ => nil
      };
   }

   protected void setHeight()
   {
      var count = uiActions.Count;

      _height = direction switch
      {
         UiActionDirection.Horizontal => clientHeight() - 2 * padding,
         UiActionDirection.Vertical when count == 0 => nil,
         UiActionDirection.Vertical => (clientHeight() - (count + 1) * padding) / count,
         _ => nil
      };
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
      var height = _height | (() => clientHeight() - 2 * padding);
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
      var width = _width | (() => clientWidth() - 2 * padding);
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

   public Maybe<UiAction> this[string caption] => uiActions.FirstOrNone(a => a.Text == caption);

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

   public IEnumerator<UiAction> GetEnumerator() => uiActions.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public int Count => uiActions.Count;

   public Maybe<UiAction> Remove(string caption)
   {
      for (var i = 0; i < uiActions.Count; i++)
      {
         if (uiActions[i].Text == caption)
         {
            var uiAction = uiActions[i];
            uiActions.RemoveAt(i);
            resize();

            return uiAction;
         }
      }

      return nil;
   }

   public Maybe<UiAction> RemoveAt(int index)
   {
      var _uiAction = uiActions.Get(index);
      if (_uiAction)
      {
         uiActions.RemoveAt(index);
         resize();
      }

      return _uiAction;
   }

   public void Insert(int index, UiAction uiAction)
   {
      uiActions.Insert(index, uiAction);
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

   public UiAction Insert(int index, string caption)
   {
      var uiAction = new UiAction(this);
      uiAction.Button(caption);

      Insert(index, uiAction);

      return uiAction;
   }

   protected override void OnVisibleChanged(EventArgs e)
   {
      base.OnVisibleChanged(e);

      foreach (var uiAction in uiActions)
      {
         uiAction.Visible = Visible;
      }
   }

   protected override void OnEnabledChanged(EventArgs e)
   {
      base.OnEnabledChanged(e);

      foreach (var uiAction in uiActions)
      {
         uiAction.Enabled = Enabled;
      }
   }
}