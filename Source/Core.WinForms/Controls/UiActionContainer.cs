using System.Collections;
using System.Drawing.Drawing2D;
using Core.Collections;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class UiActionContainer(string fontName = "Consolas", float fontSize = 12f, FontStyle fontStyle = FontStyle.Regular, bool autoSizeText = false)
   : UserControl, IEnumerable<UiAction>
{
   public static UiActionContainer HorizontalContainer() => [];

   public static UiActionContainer VerticalContainer() => new() { Direction = UiActionDirection.Vertical };

   protected StringHash<UiAction> uiActions = [];
   protected Hash<int, string> indexes = [];
   protected StringHash<int> keys = [];
   protected Maybe<int> _width = nil;
   protected Maybe<int> _height = nil;
   protected int padding = 3;
   protected UiActionDirection direction = UiActionDirection.Horizontal;
   protected bool showLastClicked = true;
   protected Maybe<int> _indexLastClicked = nil;
   protected Font font = new(fontName, fontSize, fontStyle);

   public new event EventHandler<UiActionContainerClickArgs>? Click;

   protected void setUiAction(UiAction uiAction)
   {
      var key = uiAction.Text.ToUpper();
      if (!uiActions.ContainsKey(key))
      {
         uiActions[key] = uiAction;
         var index = indexes.Count;
         indexes[index] = key;
         keys[key] = index;
         Controls.Add(uiAction);
      }
   }

   public void Add(UiAction uiAction)
   {
      var index = uiActions.Count;

      uiAction.Font = font;
      uiAction.AutoSizeText = autoSizeText;

      setUiAction(uiAction);

      if (showLastClicked)
      {
         uiAction.Click += (_, _) =>
         {
            _indexLastClicked = index;
            Invalidate();
            Click?.Invoke(this, new UiActionContainerClickArgs(uiAction, index));
         };
      }

      resize();
   }

   public UiAction Add(string caption)
   {
      var uiAction = new UiAction();
      uiAction.Button(caption);

      Add(uiAction);

      return uiAction;
   }

   public UiAction Add(string caption, bool isChecked)
   {
      var uiAction = new UiAction();
      uiAction.CheckBox(caption, isChecked);

      Add(uiAction);

      return uiAction;
   }

   public UiAction Add(string caption, UiActionType type)
   {
      var uiAction = new UiAction();
      uiAction.ShowMessage(caption, type);

      Add(uiAction);

      return uiAction;
   }

   public void AddRange(params string[] captions)
   {
      foreach (var caption in captions)
      {
         Add(caption);
      }
   }

   public void AddRange(params (string caption, bool isChecked)[] args)
   {
      foreach (var (caption, isChecked) in args)
      {
         Add(caption, isChecked);
      }
   }

   public void AddRange(params (string caption, UiActionType type)[] args)
   {
      foreach (var (caption, type) in args)
      {
         Add(caption, type);
      }
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

   protected Maybe<UiAction> getUiAction(int index)
   {
      return
         from key in indexes.Maybe[index]
         from uiAction in uiActions.Maybe[key]
         select uiAction;
   }

   protected Maybe<Unit> setUiAction(int index, Maybe<UiAction> _uiAction)
   {
      if (_uiAction is (true, var uiAction))
      {
         var uiActionKey = uiAction.Text.ToUpper();
         var _key = indexes.Maybe[index];
         if (_key is (true, var key))
         {
            uiActions.Maybe[key] = nil;
            uiActions[uiActionKey] = uiAction;
            indexes[index] = uiActionKey;

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

   protected IEnumerable<UiAction> uiActionEnumerable()
   {
      for (var i = 0; i < indexes.Count; i++)
      {
         if (getUiAction(i) is (true, var uiAction))
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

      foreach (var uiAction in uiActionEnumerable())
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

      foreach (var uiAction in uiActionEnumerable())
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
      get => getUiAction(index);
      set
      {
         if (!setUiAction(index, value))
         {
            resize();
         }
      }
   }

   public Maybe<UiAction> this[string caption] => uiActions.Maybe[caption.ToUpper()];

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

      if (showLastClicked && _indexLastClicked is (true, var index) && getUiAction(index) is (true, var uiAction))
      {
         var location = uiAction.Location.Reposition(-1, -1);
         var size = uiAction.Size.Resize(2, 2);
         var rectangle = new Rectangle(location, size);
         using var pen = new Pen(Color.Black);
         pen.DashStyle = DashStyle.Dash;
         e.Graphics.DrawRectangle(pen, rectangle);
      }
   }

   public IEnumerator<UiAction> GetEnumerator()
   {
      foreach (var uiAction in uiActionEnumerable())
      {
         yield return uiAction;
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

   public int Count => uiActions.Count;

   protected Maybe<UiAction> remove(int index, string key)
   {
      StringHash<UiAction> newUiActions = [];
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

      var _uiAction = uiActions.Maybe[key];
      if (_uiAction is (true, var uiAction))
      {
         Controls.Remove(uiAction);
      }

      uiActions = newUiActions;
      indexes = newIndexes;
      keys = newKeys;

      resize();

      return _uiAction;

      void setHashes(int currentIndex)
      {
         var key = indexes[currentIndex];
         var uiAction = uiActions[key];

         newUiActions[key] = uiAction;
         newIndexes[currentIndex] = key;
         newKeys[key] = currentIndex;
      }
   }

   public Maybe<UiAction> Remove(string caption)
   {
      var captionKey = caption.ToUpper();
      var _index = keys.Maybe[captionKey];

      return _index.Map(i => remove(i, captionKey));
   }

   public Maybe<UiAction> RemoveAt(int index)
   {
      var _key = indexes.Maybe[index];
      return _key.Map(k => remove(index, k));
   }

   public void Insert(int index, UiAction uiAction)
   {
      var captionKey = uiAction.Text.ToUpper();
      if (!uiActions.ContainsKey(captionKey) && index <= indexes.Count)
      {
         uiAction.Click += (_, _) =>
         {
            _indexLastClicked = index;
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
         uiActions[captionKey] = uiAction;

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

   public UiAction Insert(int index, string caption)
   {
      var uiAction = new UiAction();
      Controls.Add(uiAction);
      uiAction.Button(caption);

      Insert(index, uiAction);
      return uiAction;
   }

   protected override void OnVisibleChanged(EventArgs e)
   {
      base.OnVisibleChanged(e);

      foreach (var uiAction in uiActionEnumerable())
      {
         uiAction.Visible = Visible;
      }
   }

   protected override void OnEnabledChanged(EventArgs e)
   {
      base.OnEnabledChanged(e);

      foreach (var uiAction in uiActionEnumerable())
      {
         uiAction.Enabled = Enabled;
      }
   }

   public void Clear()
   {
      uiActions.Clear();
      indexes.Clear();
      keys.Clear();
   }
}