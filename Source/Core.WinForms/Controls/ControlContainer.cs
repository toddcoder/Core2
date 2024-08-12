using System.Collections;
using System.Drawing.Drawing2D;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ControlContainer<TControl> : UserControl, IEnumerable<TControl> where TControl : Control
{
   public static ControlContainer<TControl> HorizontalContainer() => [];

   public static ControlContainer<TControl> VerticalContainer() => new() { Direction = ControlDirection.Vertical };

   protected StringHash<TControl> controls = [];
   protected Hash<int, string> indexes = [];
   protected StringHash<int> keys = [];
   protected Maybe<int> _width = nil;
   protected Maybe<int> _height = nil;
   protected int padding = 3;
   protected ControlDirection direction = ControlDirection.Horizontal;
   protected bool showLastFocus = true;
   protected Maybe<int> _lastIndexFocus = nil;

   public new event EventHandler<ControlFocusArgs<TControl>>? GotFocus;

   protected void setControl(TControl control)
   {
      var key = control.Text.ToUpper();
      if (!controls.ContainsKey(key))
      {
         controls[key] = control;
         var index = indexes.Count;
         indexes[index] = key;
         keys[key] = index;
         Controls.Add(control);
      }
   }

   public void Add(TControl control)
   {
      var index = controls.Count;

      setControl(control);

      if (showLastFocus)
      {
         control.GotFocus += (_, _) =>
         {
            _lastIndexFocus = index;
            Invalidate();
            GotFocus?.Invoke(this, new ControlFocusArgs<TControl>(control, index));
         };
      }

      resize();
   }

   protected void resize()
   {
      setWidth();
      setHeight();
      arrangeControls();
   }

   protected int clientWidth() => ClientSize.Width;

   protected int clientHeight() => ClientSize.Height;

   protected void setWidth()
   {
      var count = controls.Count;

      _width = direction switch
      {
         ControlDirection.Horizontal when count == 0 => nil,
         ControlDirection.Horizontal => (clientWidth() - (count + 1) * padding) / count,
         ControlDirection.Vertical => clientWidth() - 2 * padding,
         _ => nil
      };
   }

   protected void setHeight()
   {
      var count = controls.Count;

      _height = direction switch
      {
         ControlDirection.Horizontal => clientHeight() - 2 * padding,
         ControlDirection.Vertical when count == 0 => nil,
         ControlDirection.Vertical => (clientHeight() - (count + 1) * padding) / count,
         _ => nil
      };
   }

   protected Maybe<TControl> getControl(int index)
   {
      return
         from key in indexes.Maybe[index]
         from control in controls.Maybe[key]
         select control;
   }

   protected Maybe<Unit> setControl(int index, Maybe<TControl> _control)
   {
      if (_control is (true, var control))
      {
         var controlKey = control.Text.ToUpper();
         var _key = indexes.Maybe[index];
         if (_key is (true, var key))
         {
            controls.Maybe[key] = nil;
            controls[controlKey] = control;
            indexes[index] = controlKey;

            return unit;
         }
         else
         {
            return nil;
         }
      }
      else
      {
         return nil;
      }
   }

   protected IEnumerable<TControl> controlEnumerable()
   {
      for (var i = 0; i < indexes.Count; i++)
      {
         if (getControl(i) is (true, var uiAction))
         {
            yield return uiAction;
         }
      }
   }

   protected void arrangeHorizontal(int width)
   {
      var left = padding;
      var top = padding;
      var height = _height | (() => clientHeight() - 2 * padding);
      var size = new Size(width, height);

      foreach (var control in controlEnumerable())
      {
         control.Location = new Point(left, top);
         control.Size = size;
         left += width + padding;
         control.Refresh();
      }
   }

   protected void arrangeVertical(int height)
   {
      var left = padding;
      var top = padding;
      var width = _width | (() => clientWidth() - 2 * padding);
      var size = new Size(width, height);

      foreach (var control in controlEnumerable())
      {
         control.Location = new Point(left, top);
         control.Size = size;
         top += height + padding;
         control.Refresh();
      }
   }

   protected void arrangeControls()
   {
      switch (direction)
      {
         case ControlDirection.Horizontal when _width is (true, var width):
            arrangeHorizontal(width);
            break;
         case ControlDirection.Vertical when _height is (true, var height):
            arrangeVertical(height);
            break;
      }
   }

   public Maybe<TControl> this[int index]
   {
      get => getControl(index);
      set
      {
         if (!setControl(index, value))
         {
            resize();
         }
      }
   }

   public Maybe<TControl> this[string key] => controls.Maybe[key.ToUpper()];

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

   public ControlDirection Direction
   {
      get => direction;
      set
      {
         direction = value;
         resize();
      }
   }

   public bool ShowLastFocus
   {
      get => showLastFocus;
      set
      {
         if (padding > 0)
         {
            showLastFocus = value;
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

      if (showLastFocus && _lastIndexFocus is (true, var index) && getControl(index) is (true, var uiAction))
      {
         var location = uiAction.Location.Reposition(-1, -1);
         var size = uiAction.Size.Resize(2, 2);
         var rectangle = new Rectangle(location, size);
         using var pen = new Pen(Color.Black);
         pen.DashStyle = DashStyle.Dash;
         e.Graphics.DrawRectangle(pen, rectangle);
      }
   }

   protected Maybe<TControl> remove(int index, string key)
   {
      StringHash<TControl> newControls = [];
      Hash<int, string> newIndexes = [];
      StringHash<int> newKeys = [];

      for (var i = 0; i < index; i++)
      {
         setHashes(i);
      }

      for (var i = index + 1; i < indexes.Count; i++)
      {
         setHashes(i);
      }

      var _control = controls.Maybe[key];
      if (_control is (true, var uiAction))
      {
         Controls.Remove(uiAction);
      }

      controls = newControls;
      indexes = newIndexes;
      keys = newKeys;

      resize();

      return _control;

      void setHashes(int currentIndex)
      {
         var key = indexes[currentIndex];
         var uiAction = controls[key];

         newControls[key] = uiAction;
         newIndexes[currentIndex] = key;
         newKeys[key] = currentIndex;
      }
   }

   public Maybe<TControl> Remove(string caption)
   {
      var captionKey = caption.ToUpper();
      var _index = keys.Maybe[captionKey];

      return _index.Map(i => remove(i, captionKey));
   }

   public Maybe<TControl> RemoveAt(int index)
   {
      var _key = indexes.Maybe[index];
      return _key.Map(k => remove(index, k));
   }

   public void Insert(int index, TControl control)
   {
      var captionKey = control.Text.ToUpper();
      if (!controls.ContainsKey(captionKey) && index <= indexes.Count)
      {
         control.Click += (_, _) =>
         {
            _lastIndexFocus = index;
            Invalidate();
         };

         Hash<int, string> newIndexes = [];
         StringHash<int> newKeys = [];

         for (var i = 0; i < index; i++)
         {
            newIndexes[i] = indexes[i];
            newKeys[indexes[i]] = i;
         }

         newIndexes[index] = captionKey;
         newKeys[captionKey] = index;
         controls[captionKey] = control;

         for (var i = index + 1; i < indexes.Count + 1; i++)
         {
            newIndexes[i] = indexes[index - 1];
            newKeys[indexes[i - 1]] = i;
         }

         indexes = newIndexes;
         keys = newKeys;

         resize();
      }
   }

   protected override void OnVisibleChanged(EventArgs e)
   {
      base.OnVisibleChanged(e);

      foreach (var uiAction in controlEnumerable())
      {
         uiAction.Visible = Visible;
      }
   }

   protected override void OnEnabledChanged(EventArgs e)
   {
      base.OnEnabledChanged(e);

      foreach (var uiAction in controlEnumerable())
      {
         uiAction.Enabled = Enabled;
      }
   }

   public void Clear()
   {
      controls.Clear();
      indexes.Clear();
      keys.Clear();
   }

   public IEnumerator<TControl> GetEnumerator()
   {
      foreach (var control in controlEnumerable())
      {
         yield return control;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}