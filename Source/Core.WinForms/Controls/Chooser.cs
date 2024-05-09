﻿using System.Runtime.InteropServices;
using Core.Collections;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings.Emojis;
using Core.WinForms.Drawing;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public partial class Chooser : Form
{
   [Serializable, StructLayout(LayoutKind.Sequential)]
   protected struct ScrollInfo
   {
      public uint size;
      public uint mask;
      public int min;
      public int max;
      public uint page;
      public int pos;
      public int trackPos;
   }

   protected const int WM_NCACTIVATE = 0x86;
   protected const int SB_VERTICAL = 0x1;
   protected const int SIF_RANGE = 0x1;
   protected const int SIF_PAGE = 0x2;

   public Maybe<Chosen> Open()
   {
      ShowDialog();
      return Choice;
   }

   [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
   [return: MarshalAs(UnmanagedType.Bool)]
   protected static extern bool ShowScrollBar(IntPtr hWnd, int wBar, [MarshalAs(UnmanagedType.Bool)] bool bShow);

   [DllImport("user32", CallingConvention = CallingConvention.Winapi)]
   protected static extern bool SetScrollInfo(IntPtr hWnd, int nBar, ref ScrollInfo info, bool redraw);

   [DllImport("user32.dll")]
   protected static extern bool GetScrollInfo(IntPtr hWnd, int fnBar, ref ScrollInfo info);

   protected string title;
   protected UiAction uiAction;
   protected StringHash choices = [];
   protected Maybe<Color> _foreColor = nil;
   protected Maybe<Color> _backColor = nil;
   protected Maybe<string> _nilItem = "none";
   protected bool modifyTitle = true;
   protected string emptyTitle = "";
   protected bool sizeToText;
   protected Maybe<int> _maximumWidth = nil;
   protected bool working;
   protected ChooserSorting sorting = ChooserSorting.None;
   protected Maybe<Func<string, string>> _customSorter = nil;
   protected bool autoSizeText;
   protected bool multiChoice;
   protected bool autoClose = true;
   protected Set<Chosen> chosenSet = [];
   protected bool isCheckingLocked;
   protected Guid choicesGuid = Guid.NewGuid();
   protected bool isHooked;

   public event EventHandler<AppearanceOverrideArgs>? AppearanceOverride;
   /*public event EventHandler<ChosenArgs>? ChosenItemChecked;
   public event EventHandler<ChosenArgs>? ChosenItemSelected;
   public event EventHandler<EventArgs>? ChooserOpened;
   public event EventHandler<EventArgs>? ChooserClosed;*/

   public Chooser(string title, UiAction uiAction, Maybe<int> _width)
   {
      this.title = title;
      this.uiAction = uiAction;

      InitializeComponent();

      Choice = nil;

      AutoScaleMode = AutoScaleMode.Inherit;

      if (_width)
      {
         Width = _width;
      }
   }

   public UiAction UiAction => uiAction;

   public ChooserSet Set => new(this);

   public string Title
   {
      get => title;
      set => title = value;
   }

   public StringHash Choices
   {
      get => choices;
      set => choices = value;
   }

   public Set<Chosen> ChosenSet => chosenSet;

   public Color ChoiceForeColor
   {
      get => _foreColor | Color.Black;
      set => _foreColor = value;
   }

   public Color ChoiceBackColor
   {
      get => _backColor | Color.Gold;
      set => _backColor = value;
   }

   public Maybe<string> NilItem
   {
      get => _nilItem;
      set => _nilItem = value;
   }

   public bool ModifyTitle
   {
      get => modifyTitle;
      set => modifyTitle = value;
   }

   public string EmptyTitle
   {
      get => emptyTitle;
      set => emptyTitle = value;
   }

   public bool SizeToText
   {
      get => sizeToText;
      set => sizeToText = value;
   }

   public bool Working
   {
      get => working;
      set => working = value;
   }

   public ChooserSorting Sorting
   {
      get => sorting;
      set => sorting = value;
   }

   public Maybe<Func<string, string>> CustomSorter
   {
      get => _customSorter;
      set => _customSorter = value;
   }

   public bool AutoSizeText
   {
      get => autoSizeText;
      set => autoSizeText = value;
   }

   public bool MultiChoice
   {
      get => multiChoice;
      set
      {
         multiChoice = value;
         listViewItems.CheckBoxes = true;
      }
   }

   public bool AutoClose
   {
      get => autoClose;
      set => autoClose = value;
   }

   public Guid ChoicesGuid
   {
      get => choicesGuid;
      set => choicesGuid = value;
   }

   public Maybe<Chosen> Choice { get; set; }

   public IEnumerable<Chosen> AllChosen => listViewItems.AllCheckedItems().Select(getChosen).WhereIsSome();

   protected void addItem(string text, Color foreColor, Color backColor)
   {
      Maybe<Font> _font = nil;

      text = text.EmojiSubstitutions();

      if (overrideAppearance(text, foreColor, backColor) is (true, var tuple))
      {
         (text, foreColor, backColor, _font) = tuple;
      }

      var item = listViewItems.Items.Add(text);
      item.UseItemStyleForSubItems = true;
      item.ForeColor = foreColor;
      item.BackColor = backColor;

      updateText(item, text, _font);
   }

   protected void updateItem(ListViewItem item, string text, Color foreColor, Color backColor)
   {
      Maybe<Font> _font = nil;

      text = text.EmojiSubstitutions();

      if (overrideAppearance(text, foreColor, backColor) is (true, var tuple))
      {
         (text, foreColor, backColor, _font) = tuple;
      }

      item.Text = text;
      item.UseItemStyleForSubItems = true;
      item.ForeColor = foreColor;
      item.BackColor = backColor;

      updateText(item, text, _font);
   }

   protected Maybe<(string text, Color foreColor, Color backColor, Maybe<Font> _font)> overrideAppearance(string text, Color foreColor,
      Color backColor)
   {
      if (AppearanceOverride is not null)
      {
         var args = new AppearanceOverrideArgs(text, foreColor, backColor);
         AppearanceOverride.Invoke(this, args);
         if (args.Override)
         {
            text = args.Text;
            foreColor = args.ForeColor;
            backColor = args.BackColor;

            Bits32<FontStyle> style = FontStyle.Regular;
            var modified = false;
            if (args.Italic)
            {
               style[FontStyle.Italic] = true;
               modified = true;
            }

            if (args.Bold)
            {
               style[FontStyle.Bold] = true;
               modified = true;
            }

            var _font = maybe<Font>() & modified & (() => new Font(listViewItems.Font, style));

            return (text, foreColor, backColor, _font);
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

   protected void updateText(ListViewItem item, string text, Maybe<Font> _font)
   {
      if (autoSizeText)
      {
         using var g = listViewItems.CreateGraphics();
         var _smallFont = AutoSizingWriter.AdjustedFont(g, text, listViewItems.Font, listViewItems.ClientSize.Width, 6, 12,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
         if (_smallFont is (true, var smallFont))
         {
            item.UseItemStyleForSubItems = true;
            item.Font = smallFont;
         }
      }
      else if (sizeToText)
      {
         var width = TextRenderer.MeasureText(text, _font | listViewItems.Font, Size.Empty).Width;
         if (_maximumWidth is (true, var maximumWidth))
         {
            if (width > maximumWidth)
            {
               _maximumWidth = width;
            }
         }
         else
         {
            _maximumWidth = width;
         }
      }

      if (_font is (true, var font))
      {
         item.Font = font;
      }
   }

   protected void locate()
   {
      var screenArea = Screen.GetWorkingArea(this);
      var location = Cursor.Position;
      var height = screenArea.Height - location.Y - 32;

      var xPlusWidth = location.X + Width;
      if (xPlusWidth > screenArea.Width)
      {
         var amount = xPlusWidth - screenArea.Width;
         Location = location with { X = location.X - amount };
      }
      else
      {
         Location = location;
      }

      Height = height;
   }

   protected void Chooser_Load(object sender, EventArgs e)
   {
      locate();
      LoadChoices();
   }

   public void LoadChoices()
   {
      listViewItems.Items.Clear();

      if (_nilItem is (true, var nilItem) && !multiChoice && autoClose)
      {
         addItem(nilItem, _foreColor | Color.White, _backColor | Color.Blue);
      }

      var foreColor = _foreColor | Color.White;
      var backColor = _backColor | Color.Green;

      switch (sorting)
      {
         case ChooserSorting.None:
            foreach (var choice in choices.Keys)
            {
               addItem(choice, foreColor, backColor);
            }

            break;
         case ChooserSorting.Ascending:
            foreach (var choice in choices.Keys.Order())
            {
               addItem(choice, foreColor, backColor);
            }

            break;
         case ChooserSorting.Descending:
            foreach (var choice in choices.Keys.OrderDescending())
            {
               addItem(choice, foreColor, backColor);
            }

            break;
         case ChooserSorting.Custom when _customSorter is (true, var customSorter):
            foreach (var choice in choices.Keys.OrderBy(customSorter))
            {
               addItem(choice, foreColor, backColor);
            }

            break;
         default:
            goto case ChooserSorting.None;
      }

      listViewItems.Columns[0].Width = ClientSize.Width;
      listViewItems.Columns[0].Text = title;

      var lastItem = listViewItems.Items[^1];
      var bottom = lastItem.Bounds.Bottom;
      if (listViewItems.ClientRectangle.Height > bottom)
      {
         Height = bottom + lastItem.Bounds.Height;
      }

      if (_maximumWidth is (true, var maximumWidth))
      {
         columnHeader1.Width = maximumWidth;
         Width = maximumWidth + 8;
      }

      ShowScrollBar(listViewItems.Handle, SB_VERTICAL, true);

      var info = new ScrollInfo
      {
         mask = SIF_RANGE | SIF_PAGE,
         size = (uint)Marshal.SizeOf(typeof(ScrollInfo))
      };
      GetScrollInfo(listViewItems.Handle, SB_VERTICAL, ref info);
      info.max = listViewItems.Items.Count - 1;
      info.page = (uint)info.max / 10;
      SetScrollInfo(listViewItems.Handle, SB_VERTICAL, ref info, true);

      CheckFromChosenSet();

      choicesGuid = Guid.NewGuid();

      uiAction.OnChooserOpened();
   }

   protected void Chooser_MouseDown(object sender, MouseEventArgs e)
   {
      if (!ClientRectangle.Contains(Cursor.Position))
      {
         Close();
      }
   }

   protected bool returnSome(int index) => _nilItem.Map(_ => index > 0) | (() => index > -1);

   protected Maybe<Chosen> getChosen(ListViewItem item)
   {
      return choices.Items[item.Text].Map(value => new Chosen(value, item));
   }

   protected void listViewItems_SelectedIndexChanged(object sender, EventArgs e)
   {
      Choice = listViewItems.SelectedItem().Map(item => maybe<Chosen>() & returnSome(item.Index) & (() => getChosen(item)));
      if (autoClose)
      {
         if (!multiChoice)
         {
            Close();
         }
      }
      else if (Choice is (true, var chosen))
      {
         var key = chosen.Key;
         var value = choices.Maybe[key] | key;
         var args = new ChosenArgs(chosen, chosenSet);

         var originalGuid = choicesGuid;

         uiAction.OnChosenItemSelected(args);

         if (originalGuid == choicesGuid)
         {
            chosen = args.Chosen;
            var item = listViewItems.Items[chosen.Index];
            if (key != chosen.Key)
            {
               choices.Maybe[key] = nil;
               choices[chosen.Key] = chosen.Value;
            }
            else if (value != chosen.Value)
            {
               choices[key] = chosen.Value;
            }

            updateItem(item, chosen.Key, chosen.ForeColor, chosen.BackColor);
         }
      }
   }

   protected override void WndProc(ref Message m)
   {
      base.WndProc(ref m);

      if (m.Msg == WM_NCACTIVATE && Visible && !RectangleToScreen(DisplayRectangle).Contains(Cursor.Position))
      {
         modifyTitle = false;
         Close();
      }
   }

   protected void listViewItems_ItemChecked(object sender, ItemCheckedEventArgs e)
   {
      if (!isCheckingLocked)
      {
         Choice = getChosen(e.Item);
         if (Choice is (true, var chosen))
         {
            if (chosen.IsChecked)
            {
               chosenSet.Add(chosen);
            }
            else
            {
               chosenSet.Remove(chosen);
            }

            uiAction.OnChosenItemChecked(new ChosenArgs(chosen, chosenSet));
         }
      }
   }

   public void CheckFromChosenSet()
   {
      if (multiChoice)
      {
         try
         {
            isCheckingLocked = true;

            var hash = chosenSet.ToStringHash(c => c.Key);
            foreach (var (key, _) in hash)
            {
               if (listViewItems.FindItem(i => i.Text == key) is (true, var item))
               {
                  item.Checked = true;
               }
            }
         }
         finally
         {
            isCheckingLocked = false;
         }
      }
   }

   protected void Chooser_FormClosed(object sender, FormClosedEventArgs e) => uiAction.OnChooserClosed();

   public void Update(StringHash choices)
   {
      this.choices = choices;
      LoadChoices();
   }

   public void Update(params string[] choices)
   {
      this.choices = choices.ToStringHash(c => c, c => c);
      LoadChoices();
   }

   public void Update(params (string key, string value)[] choices)
   {
      this.choices = choices.ToStringHash();
      LoadChoices();
   }

   public void Update(IEnumerable<string> choices)
   {
      Update([.. choices]);
      LoadChoices();
   }

   public void ClearChoices()
   {
      choices.Clear();
      LoadChoices();
   }
}