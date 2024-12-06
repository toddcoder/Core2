using System.Runtime.InteropServices;
using System.Windows.Forms.VisualStyles;
using Core.Collections;
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

   protected const int SB_VERTICAL = 0x1;
   protected const int SIF_RANGE = 0x1;
   protected const int SIF_PAGE = 0x2;

   public Maybe<Chosen> Open()
   {
      UpdateZOrder();
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
   protected bool useEmojis;
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
   protected Maybe<Func<string, string>> _customKeySorter = nil;
   protected Maybe<Func<string, string>> _customValueSorter = nil;
   protected bool autoSizeText;
   protected bool multiChoice;
   protected bool autoClose = true;
   protected Set<Chosen> chosenSet = [];
   protected bool isCheckingLocked;
   protected Guid choicesGuid = Guid.NewGuid();

   public Chooser(string title, UiAction uiAction, Maybe<int> _width)
   {
      this.title = title;
      this.uiAction = uiAction;
      useEmojis = this.uiAction.UseEmojis;

      InitializeComponent();

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

   public Maybe<Func<string, string>> CustomKeySorter
   {
      get => _customKeySorter;
      set => _customKeySorter = value;
   }

   public Maybe<Func<string, string>> CustomValueSorter
   {
      get => _customValueSorter;
      set => _customValueSorter = value;
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
         listViewItems.CheckBoxes = multiChoice;
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

   public Maybe<Chosen> Choice { get; set; } = nil;

   public IEnumerable<Chosen> AllChosen => listViewItems.AllCheckedItems().Select(getChosen).WhereIsSome();

   public bool CheckBoxes
   {
      get => listViewItems.CheckBoxes;
      set => listViewItems.CheckBoxes = value;
   }

   public bool FlyUp { get; set; }

   protected string withEmojis(string text) => useEmojis ? text.EmojiSubstitutions() : text;

   protected void addItem(string text, Color foreColor, Color backColor)
   {
      Maybe<Font> _font = nil;

      text = withEmojis(text);

      if (overrideAppearance(listViewItems.Items.Count, text, foreColor, backColor) is (true, var tuple))
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

      text = withEmojis(text);

      if (overrideAppearance(item.Index, text, foreColor, backColor) is (true, var tuple))
      {
         (text, foreColor, backColor, _font) = tuple;
      }

      item.Text = text;
      item.UseItemStyleForSubItems = true;
      item.ForeColor = foreColor;
      item.BackColor = backColor;

      updateText(item, text, _font);
   }

   protected Maybe<(string text, Color foreColor, Color backColor, Maybe<Font> _font)> overrideAppearance(int index, string text, Color foreColor,
      Color backColor)
   {
      var args = new AppearanceOverrideArgs(index, text, foreColor, backColor);
      uiAction.OnAppearanceOverride(args);
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
      if (!FlyUp)
      {
         Height = screenArea.Height - location.Y - 32;
      }

      var xPlusWidth = location.X + Width;
      var amount = xPlusWidth > screenArea.Width ? xPlusWidth - screenArea.Width : 0;
      if (FlyUp)
      {
         Location = location with { X = location.X - amount, Y = location.Y - Height };
      }
      else
      {
         Location = location with { X = location.X - amount };
      }
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
         case ChooserSorting.CustomKey when _customKeySorter is (true, var customKeySorter):
         {
            foreach (var choice in choices.Keys.OrderBy(customKeySorter))
            {
               addItem(choice, foreColor, backColor);
            }

            break;
         }
         case ChooserSorting.CustomKeyDescending when _customKeySorter is (true, var customKeySorter):
         {
            foreach (var choice in choices.Keys.OrderByDescending(customKeySorter))
            {
               addItem(choice, foreColor, backColor);
            }

            break;
         }
         case ChooserSorting.CustomValue when _customValueSorter is (true, var customValueSorter):
         {
            foreach (var choice in choices.OrderBy(t => customValueSorter(t.Value)))
            {
               addItem(choice.Key, foreColor, backColor);
            }

            break;
         }
         case ChooserSorting.CustomValueDescending when _customValueSorter is (true, var customValueSorter):
         {
            foreach (var choice in choices.OrderByDescending(t => customValueSorter(t.Value)))
            {
               addItem(choice.Key, foreColor, backColor);
            }

            break;
         }
         default:
            goto case ChooserSorting.None;
      }

      listViewItems.Columns[0].Width = ClientSize.Width;
      listViewItems.Columns[0].Text = title;

      var lastItem = listViewItems.Items[^1];
      var bottom = lastItem.Bounds.Bottom;
      if (!FlyUp && listViewItems.ClientRectangle.Height > bottom)
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

   protected void listViewItems_DrawItem(object sender, DrawListViewItemEventArgs e)
   {
      if (multiChoice)
      {
         e.DrawBackground();
         var checkBoxState = e.Item.Checked ? CheckBoxState.CheckedNormal : CheckBoxState.UncheckedPressed;
         var glyphSize = CheckBoxRenderer.GetGlyphSize(e.Graphics, checkBoxState);
         var glyphRectangle = glyphSize.West(e.Bounds, 2);
         var textRectangle = e.Bounds.RightOf(glyphSize);
         CheckBoxRenderer.DrawParentBackground(e.Graphics, glyphRectangle, this);
         CheckBoxRenderer.DrawCheckBox(e.Graphics, glyphRectangle.Location, checkBoxState);
         using var font = new Font("Consolas", 12f);
         TextRenderer.DrawText(e.Graphics, e.Item.Text, font, textRectangle, e.Item.ForeColor, e.Item.BackColor, TextFormatFlags.Left);
      }
      else
      {
         e.DrawDefault = true;
      }
   }

   protected void Chooser_KeyUp(object sender, KeyEventArgs e)
   {
      if (e.KeyCode is Keys.Escape)
      {
         e.Handled = true;
         Close();
      }
   }
}