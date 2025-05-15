using System.Collections;
using System.Drawing.Drawing2D;
using Core.Applications.Messaging;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ControlContainer<TControl> : UserControl, IEnumerable<TControl> where TControl : Control
{
   public static ControlContainer<TControl> HorizontalContainer() => new();

   public static ControlContainer<TControl> VerticalContainer() => new() { Direction = ControlDirection.Vertical };

   public static ControlContainer<TControl> ReadingContainer() => new() { Direction = ControlDirection.Reading };

   protected Hash<TControl, int> controlToIndex = [];
   protected Hash<int, TControl> indexToControl = [];
   protected Maybe<int> _width = nil;
   protected Maybe<int> _height = nil;
   protected int padding = 3;
   protected ControlDirection direction = ControlDirection.Horizontal;
   protected bool showLastFocus = true;
   protected Maybe<int> _lastIndexFocus = nil;
   protected bool isUpdating = true;
   protected int horizontalCount = 4;
   protected int verticalCount = 2;

   public readonly MessageEvent<ControlFocusArgs<TControl>> AfterFocus = new();
   public readonly MessageEvent<ControlArrangingArgs<TControl>> AfterArranged = new();
   public readonly MessageEvent NextRow = new();

   public int Add(TControl control)
   {
      var index = Controls.Count;

      if (!isUpdating)
      {
         control.Visible = false;
      }

      if (showLastFocus)
      {
         control.GotFocus += (_, _) =>
         {
            _lastIndexFocus = index;
            Invalidate();
            AfterFocus.Invoke(new ControlFocusArgs<TControl>(control, index));
         };
      }

      Controls.Add(control);
      controlToIndex[control] = index;
      indexToControl[index] = control;

      resize();

      if (control is IHasObjectId hasObjectId)
      {
         hasObjectId.ObjectId = index;
      }

      return index;
   }

   protected Maybe<int> getControlIndex(TControl control) => controlToIndex.Maybe[control];

   public Maybe<int> Remove(TControl control)
   {
      if (Controls.Count == 0)
      {
         return nil;
      }

      var _index = getControlIndex(control);

      if (_index is (true, var index))
      {
         Controls.Remove(control);
         controlToIndex.Clear();
         indexToControl.Clear();

         for (var i = 0; i < Controls.Count; i++)
         {
            controlToIndex[(TControl)Controls[i]] = i;
            indexToControl[i] = (TControl)Controls[i];
         }

         resize();

         return index;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<int> RemoveLast()
   {
      if (Controls.Count > 0)
      {
         return Remove((TControl)Controls[^1]);
      }
      else
      {
         return nil;
      }
   }

   public Maybe<TControl> LastControl()
   {
      if (Controls.Count > 0)
      {
         return (TControl)Controls[^1];
      }
      else
      {
         return nil;
      }
   }

   public Maybe<int> ControlHeight { get; set; } = nil;

   public Maybe<int> ControlWidth { get; set; } = nil;

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
      var count = Controls.Count;

      _width = direction switch
      {
         ControlDirection.Horizontal when count == 0 => nil,
         ControlDirection.Horizontal when ControlWidth is (true, var width) => width,
         ControlDirection.Horizontal => (clientWidth() - (count + 1) * padding) / count,
         ControlDirection.Vertical when ControlWidth is (true, var width) => width,
         ControlDirection.Vertical => clientWidth() - 2 * padding,
         ControlDirection.Reading when count == 0 => nil,
         ControlDirection.Reading when ControlWidth is (true, var width) => width,
         ControlDirection.Reading => (clientWidth() - (horizontalCount + 1) * padding) / horizontalCount,
         _ => nil
      };
   }

   protected void setHeight()
   {
      var count = Controls.Count;

      _height = direction switch
      {
         ControlDirection.Horizontal when ControlHeight is (true, var height) => height,
         ControlDirection.Horizontal => clientHeight() - 2 * padding,
         ControlDirection.Vertical when count == 0 => nil,
         ControlDirection.Vertical when ControlHeight is (true, var height) => height,
         ControlDirection.Vertical => (clientHeight() - (count + 1) * padding) / count,
         ControlDirection.Reading when count == 0 => nil,
         ControlDirection.Reading when ControlHeight is (true, var height) => height,
         ControlDirection.Reading => (clientHeight() - (verticalCount + 1) * padding) / verticalCount,
         _ => nil
      };
   }

   protected Maybe<TControl> getControl(int index) => maybe<TControl>() & index.Between(0).Until(Controls.Count) & (() => (TControl)Controls[index]);

   protected void arrangeHorizontal(int width)
   {
      var left = padding;
      var top = padding;
      var height = _height | (() => clientHeight() - 2 * padding);
      var size = new Size(width, height);

      var index = 0;
      foreach (var control in this)
      {
         control.Location = new Point(left, top);
         control.Size = size;
         left += width + padding;
         control.Visible = true;
         control.Refresh();

         AfterArranged.Invoke(new ControlArrangingArgs<TControl>(control, size, index++));
      }
   }

   protected void arrangeVertical(int height)
   {
      var left = padding;
      var top = padding;
      var width = _width | (() => clientWidth() - 2 * padding);
      var size = new Size(width, height);

      var index = 0;
      foreach (var control in this)
      {
         control.Location = new Point(left, top);
         control.Size = size;
         top += height + padding;
         control.Visible = true;
         control.Refresh();

         AfterArranged.Invoke(new ControlArrangingArgs<TControl>(control, size, index++));
      }
   }

   protected void arrangeReading(int width, int height)
   {
      var left = padding;
      var top = padding;

      var maxWidth = clientWidth();
      var maxHeight = clientHeight();

      var index = 0;
      foreach (var control in this)
      {
         if (left + width > maxWidth)
         {
            left = padding;
            top += height;
            NextRow.Invoke();
         }

         if (top + height > maxHeight)
         {
            break;
         }

         control.Location = new Point(left, top);
         var size = new Size(width, height);
         control.Size = size;
         control.Visible = true;
         left += width + padding;

         AfterArranged.Invoke(new ControlArrangingArgs<TControl>(control, size, index++));
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

   public Maybe<TControl> this[int index]
   {
      get => getControl(index);
      set
      {
         if (value is (true, var newControl) && indexToControl.Maybe[index] is (true, var oldControl))
         {
            indexToControl.Maybe[index] = nil;
            controlToIndex.Maybe[oldControl] = nil;
            indexToControl[index] = newControl;
            controlToIndex[newControl] = index;

            Controls.Clear();
            foreach (var control in this)
            {
               Controls.Add(control);
            }

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

   public int Count => Controls.Count;

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
         from lastId in _lastIndexFocus
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

   protected override void OnVisibleChanged(EventArgs e)
   {
      base.OnVisibleChanged(e);

      foreach (var control in this)
      {
         control.Visible = Visible;
      }
   }

   protected override void OnEnabledChanged(EventArgs e)
   {
      base.OnEnabledChanged(e);

      foreach (var control in this)
      {
         control.Enabled = Enabled;
      }
   }

   public void Clear()
   {
      controlToIndex.Clear();
      Controls.Clear();
   }

   public IEnumerator<TControl> GetEnumerator()
   {
      foreach (var control in controlToIndex.OrderBy(i => i.Value).Select(i => i.Key))
      {
         yield return control;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public void BeginUpdate() => isUpdating = false;

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

   public Maybe<int> IndexOf(TControl control) => controlToIndex.Maybe[control];

   public Maybe<TControl> Find(Func<TControl, bool> predicate)
   {
      foreach (var control in this)
      {
         if (predicate(control))
         {
            return control;
         }
      }

      return nil;
   }

   public IEnumerable<TControl> FindAll(Func<TControl, bool> predicate)
   {
      foreach (var control in this)
      {
         if (predicate(control))
         {
            yield return control;
         }
      }
   }
}