using System.Collections;
using System.Drawing.Drawing2D;
using Core.Monads;
using Core.Numbers;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ControlContainer<TControl> : UserControl, IEnumerable<TControl> where TControl : Control
{
   public static ControlContainer<TControl> HorizontalContainer() => [];

   public static ControlContainer<TControl> VerticalContainer() => new() { Direction = ControlDirection.Vertical };

   public static ControlContainer<TControl> ReadingContainer() => new() { Direction = ControlDirection.Reading };

   protected ObjectHash<TControl> objectHash = new();
   protected Maybe<int> _width = nil;
   protected Maybe<int> _height = nil;
   protected int padding = 3;
   protected ControlDirection direction = ControlDirection.Horizontal;
   protected bool showLastFocus = true;
   protected Maybe<long> _lastIdFocus = nil;
   protected bool isUpdating = true;
   protected int horizontalCount = 4;
   protected int verticalCount = 2;

   public new event EventHandler<ControlFocusArgs<TControl>>? GotFocus;

   protected (long id, bool firstTime) setControl(TControl control) => objectHash.GetIdWithFirstTime(control);

   public long Add(TControl control)
   {
      var (id, firstTime) = setControl(control);

      if (firstTime && showLastFocus)
      {
         control.GotFocus += (_, _) =>
         {
            _lastIdFocus = id;
            Invalidate();
            GotFocus?.Invoke(this, new ControlFocusArgs<TControl>(control, id));
         };
      }

      Controls.Add(control);

      resize();

      if (control is IHasObjectId hasObjectId)
      {
         hasObjectId.ObjectId = id;
      }

      return id;
   }

   public long Remove(TControl control)
   {
      var id = -1L;

      Controls.Remove(control);
      if (control is IHasObjectId hasObjectId)
      {
         remove(hasObjectId.ObjectId);
         id = hasObjectId.ObjectId;
      }

      resize();

      return id;
   }

   public void Rearrange()
   {
      setWidth();
      setHeight();
      arrangeControls();
   }

   protected void resize()
   {
      if (isUpdating)
      {
         setWidth();
         setHeight();
         arrangeControls();
      }
   }

   protected int clientWidth() => ClientSize.Width;

   protected int clientHeight() => ClientSize.Height;

   protected void setWidth()
   {
      var count = objectHash.Count;

      _width = direction switch
      {
         ControlDirection.Horizontal when count == 0 => nil,
         ControlDirection.Horizontal => (clientWidth() - (count + 1) * padding) / count,
         ControlDirection.Vertical => clientWidth() - 2 * padding,
         ControlDirection.Reading when count == 0 => nil,
         ControlDirection.Reading => (clientWidth() - (horizontalCount + 1) * padding) / horizontalCount,
         _ => nil
      };
   }

   protected void setHeight()
   {
      var count = objectHash.Count;

      _height = direction switch
      {
         ControlDirection.Horizontal => clientHeight() - 2 * padding,
         ControlDirection.Vertical when count == 0 => nil,
         ControlDirection.Vertical => (clientHeight() - (count + 1) * padding) / count,
         ControlDirection.Reading when count == 0 => nil,
         ControlDirection.Reading => (clientHeight() - (verticalCount + 1) * padding) / verticalCount,
         _ => nil
      };
   }

   protected Maybe<TControl> getControl(long id) => objectHash.IdToObjectMaybe[id];

   protected void arrangeHorizontal(int width)
   {
      var left = padding;
      var top = padding;
      var height = _height | (() => clientHeight() - 2 * padding);
      var size = new Size(width, height);

      foreach (var control in objectHash.Objects())
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

      foreach (var control in objectHash.Objects())
      {
         control.Location = new Point(left, top);
         control.Size = size;
         top += height + padding;
         control.Refresh();
      }
   }

   protected void arrangeReading(int width, int height)
   {
      var left = padding;
      var top = padding;

      var maxWidth = clientWidth();
      var maxHeight = clientHeight();

      foreach (var control in objectHash.Objects())
      {
         if (left + width > maxWidth)
         {
            left = padding;
            top += height;
         }

         if (top + height > maxHeight)
         {
            break;
         }

         control.Location = new Point(left, top);
         control.Size = new Size(width, height);
         left += width + padding;
      }
   }

   protected void arrangeControls()
   {
      switch (direction)
      {
         case ControlDirection.Horizontal when _width is (true, var width):
         {
            arrangeHorizontal(width);
            break;
         }
         case ControlDirection.Vertical when _height is (true, var height):
         {
            arrangeVertical(height);
            break;
         }
         case ControlDirection.Reading when _width is (true, var width) && _height is (true, var height):
         {
            arrangeReading(width, height);
            break;
         }
      }
   }

   public Maybe<TControl> this[long id]
   {
      get => objectHash.IdToObjectMaybe[id];
      set
      {
         if (value is (true, var control))
         {
            objectHash.IdToObject[id] = control;
            control.GotFocus += (_, _) =>
            {
               _lastIdFocus = id;
               Invalidate();
               GotFocus?.Invoke(this, new ControlFocusArgs<TControl>(control, id));
            };
            Controls.Add(control);
         }
         else if (objectHash.IdToObjectMaybe[id] is (true, var oldControl))
         {
            Controls.Remove(oldControl);
            objectHash.Remove(id);
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

   public int Count => objectHash.Count;

   public int HorizontalCount
   {
      get => horizontalCount;
      set => horizontalCount = value;
   }

   public int VerticalCount
   {
      get => verticalCount;
      set => verticalCount = value;
   }

   protected override void OnResize(EventArgs e)
   {
      base.OnResize(e);

      resize();
   }

   protected Maybe<TControl> getLastControlWithFocus()
   {
      return
         from lastId in _lastIdFocus
         from control in getControl(lastId)
         select control;
   }

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      if (isUpdating && showLastFocus && getLastControlWithFocus() is (true, var control))
      {
         var location = control.Location.Reposition(-1, -1);
         var size = control.Size.Resize(2, 2);
         var rectangle = new Rectangle(location, size);
         using var pen = new Pen(Color.Black);
         pen.DashStyle = DashStyle.Dash;
         e.Graphics.DrawRectangle(pen, rectangle);
      }
   }

   protected Maybe<TControl> remove(long id) => objectHash.Remove(id);

   protected override void OnVisibleChanged(EventArgs e)
   {
      base.OnVisibleChanged(e);

      foreach (var uiAction in objectHash.Objects())
      {
         uiAction.Visible = Visible;
      }
   }

   protected override void OnEnabledChanged(EventArgs e)
   {
      base.OnEnabledChanged(e);

      foreach (var uiAction in objectHash.Objects())
      {
         uiAction.Enabled = Enabled;
      }
   }

   public void Clear()
   {
      objectHash.Clear();
      Controls.Clear();
   }

   public IEnumerator<TControl> GetEnumerator()
   {
      foreach (var control in objectHash.Objects())
      {
         yield return control;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   [Obsolete("Use BeginUpdate")]
   public void BeginUpdating() => isUpdating = false;

   public void BeginUpdate() => isUpdating = false;

   [Obsolete("Use EndUpdate")]
   public void EndUpdating()
   {
      isUpdating = true;
      resize();
      Invalidate();
   }

   public void EndUpdate()
   {
      isUpdating = true;
      resize();
      Invalidate();
   }

   public void SetScroller(int maximum, uint page)
   {
      var scroller = new Scroller(Handle, maximum, page);
      scroller.Set();
   }
}