﻿using Core.Applications;
using Core.Collections;
using Core.Matching;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Objects;
using Core.Strings;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.WinForms.Documents;

public class Menus : IHash<string, ToolStripMenuItem>
{
   public static MenuBuilder operator +(Menus menus, string text) => menus.Add().Text(text);

   public static MenuBuilder operator +(Menus menus, Func<string> textFunc) => menus.Add().Text(textFunc);

   public static MenuBuilder operator +(Menus menus, Func<Result<string>> textFunc) => menus.Add().Text(textFunc);

   public static MenuBuilder operator +(Menus menus, EventHandler handler) => menus.Add().Handler(handler);

   public static MenuBuilder operator +(Menus menus, Action handler) => menus.Add().Handler(handler);

   public static MenuBuilder operator +(Menus menus, Keys keys) => menus.Add().Keys(keys);

   public static MenuBuilder operator +(Menus menus, bool enabled) => menus.Add().Enabled(enabled);

   public static MenuBuilder operator +(Menus menus, MenuBuilder.BuilderIsChecked _) => menus.Add().IsChecked(true);

   public static MenuBuilder operator +(Menus menus, ToolStripMenuItem item) => menus.Add(item);

   public static MenuBuilder operator +(Menus menus, (string parentText, string menuText) texts) => menus.Add(texts.parentText).Text(texts.menuText);

   public static MenuBuilder operator +(Menus menus, (string parentText, Func<string> menuText) texts)
   {
      return menus.Add(texts.parentText).Text(texts.menuText);
   }

   public static MenuBuilder operator +(Menus menus, (string parentText, Func<Result<string>> menuText) texts)
   {
      return menus.Add(texts.parentText).Text(texts.menuText);
   }

   public static Menus operator +(Menus menus, MenuBuilder.Separator _)
   {
      menus.Separator();
      return menus;
   }

   public static Menus operator +(Menus menus, Control control)
   {
      menus.SetContextMenuControl(control);
      return menus;
   }

   public static MenuBuilder operator +(Menus menus, MenuBuilder.ContextMenuItem _) => menus.Add();

   protected StringHash<ToolStripItem> menuItems = [];
   protected Hash<ToolStripItem, MenuText> dynamicTextItems = [];
   protected StringHash<int> tabIndexes = [];
   protected int tabIndex;
   protected Maybe<string> _currentMenu = nil;
   protected Maybe<ToolStripMenuItem> _currentItem = nil;
   protected Maybe<Control> _contextMenuControl = nil;
   protected bool contextControlIsRichTextBox;

   protected void setParent(string text)
   {
      _currentMenu = text;
      _currentItem = nil;
   }

   protected void setParent(ToolStripMenuItem item)
   {
      _currentMenu = nil;
      _currentItem = item;
   }

   public MenuBuilder Add(string parentText)
   {
      setParent(parentText);
      return new MenuBuilder(this, parentText);
   }

   public MenuBuilder Add(ToolStripMenuItem parentItem)
   {
      setParent(parentItem);
      return new MenuBuilder(this, parentItem);
   }

   public MenuBuilder Add() => new(this);

   public ToolStripMenuItem Menu(string text, string shortcut = "")
   {
      var item = new ToolStripMenuItem(text) { Name = MenuName(text) };
      if (shortcut.IsNotEmpty())
      {
         var _keys = shortcutKeys(shortcut);
         if (_keys is (true, var keys))
         {
            item.ShortcutKeys = keys;
         }
      }

      menuItems[item.Name] = item;
      tabIndexes[item.Name] = tabIndex++;
      setParent(text);

      return item;
   }

   public ToolStripMenuItem ContextMenu(string text, EventHandler handler, string shortcut = "", bool isChecked = false, bool enabled = true,
      Keys keys = Keys.None)
   {
      var item = new ToolStripMenuItem(text) { Name = MenuName(text), Checked = isChecked, Enabled = enabled };
      item.Click += handler;
      setShortcut(item, shortcut, keys);

      menuItems[item.Name] = item;
      tabIndexes[item.Name] = tabIndex++;

      return item;
   }

   public ToolStripMenuItem ContextMenu(Func<string> textFunc, EventHandler handler, string shortcut = "", bool isChecked = false,
      bool enabled = true, Keys keys = Keys.None)
   {
      var text = textFunc();
      var item = new ToolStripMenuItem(text) { Name = MenuName(text), Checked = isChecked, Enabled = enabled };
      item.Click += handler;
      setShortcut(item, shortcut, keys);

      menuItems[item.Name] = item;
      tabIndexes[item.Name] = tabIndex++;

      return item;
   }

   public ToolStripMenuItem ContextMenu(Func<Result<string>> textFunc, EventHandler handler, string shortcut = "", bool isChecked = false,
      bool enabled = true, Keys keys = Keys.None)
   {
      var _text = textFunc();
      var text = _text | (e => e.Message);
      var item = new ToolStripMenuItem(text) { Name = MenuName(text), Checked = isChecked, Enabled = enabled };
      item.Click += handler;
      setShortcut(item, shortcut, keys);

      menuItems[item.Name] = item;
      tabIndexes[item.Name] = tabIndex++;

      return item;
   }

   public static string MenuName(string text) => $"menu{makeIdentifier(text)}";

   protected static string makeIdentifier(string text) => text.Replace("&", "").Substitute("/s+; f", "_").SnakeToCamelCase(false);

   public static string SubmenuName(string parentText, string text) => MenuName(parentText) + makeIdentifier(text);

   protected ToolStripMenuItem getParent(string text)
   {
      var name = MenuName(text);
      var parent = menuItems.Value(name);

      return (ToolStripMenuItem)parent;
   }

   protected static void setShortcut(ToolStripMenuItem item, string shortcut, Keys keyValue)
   {
      if (keyValue is not Keys.None)
      {
         item.ShortcutKeys = keyValue;
      }
      else if (shortcut.IsNotEmpty())
      {
         var _keys = shortcutKeys(shortcut);
         if (_keys is (true, var keys))
         {
            item.ShortcutKeys = keys;
         }
      }
   }

   public ToolStripMenuItem Menu(string parentText, string text, EventHandler handler, string shortcut = "", bool isChecked = false, int index = -1,
      bool enabled = true, Keys keys = Keys.None)
   {
      setParent(parentText);

      var parent = getParent(parentText);
      var item = new ToolStripMenuItem(text) { Name = SubmenuName(parentText, text), Checked = isChecked, Enabled = enabled };
      item.Click += handler;
      setShortcut(item, shortcut, keys);
      if (index == -1)
      {
         parent.DropDownItems.Add(item);
      }
      else
      {
         parent.DropDownItems.Insert(index, item);
      }

      return item;
   }

   public ToolStripMenuItem SubMenu(string parentText, string text, int index = -1)
   {
      setParent(parentText);

      var parentItem = getParent(parentText);
      var item = new ToolStripMenuItem(text) { Name = SubmenuName(parentText, text) };
      if (index == -1)
      {
         parentItem.DropDownItems.Add(item);
      }
      else
      {
         parentItem.DropDownItems.Insert(index, item);
      }

      setParent(item);

      return item;
   }

   public void GoUpTo(string parentText)
   {
      var parentItem = getParent(parentText);
      setParent(parentItem);
   }

   public ToolStripMenuItem Menu(ToolStripMenuItem parentItem, string text, EventHandler handler, string shortcut = "", bool isChecked = false,
      int index = -1, bool enabled = true, Keys keys = Keys.None)
   {
      setParent(parentItem);

      var item = new ToolStripMenuItem(text) { Name = SubmenuName(parentItem.Text ?? "", text), Checked = isChecked, Enabled = enabled };
      item.Click += handler;
      setShortcut(item, shortcut, keys);
      if (index == -1)
      {
         parentItem.DropDownItems.Add(item);
      }
      else
      {
         parentItem.DropDownItems.Insert(index, item);
      }

      return item;
   }

   public ToolStripMenuItem SubMenu(ToolStripMenuItem parentItem, string text, int index = -1)
   {
      setParent(parentItem);

      var item = new ToolStripMenuItem(text) { Name = SubmenuName(parentItem.Text ?? "", text) };
      if (index == -1)
      {
         parentItem.DropDownItems.Add(item);
      }
      else
      {
         parentItem.DropDownItems.Insert(index, item);
      }

      setParent(item);

      return item;
   }

   public Maybe<ToolStripMenuItem> Menu(string text, EventHandler handler, string shortcut = "", bool isChecked = false, int index = -1,
      bool enabled = true, Keys keys = Keys.None)
   {
      if (_currentMenu is (true, var currentMenu))
      {
         return Menu(currentMenu, text, handler, shortcut, isChecked, index, enabled, keys);
      }
      else if (_currentItem is (true, var currentItem))
      {
         return Menu(currentItem, text, handler, shortcut, isChecked, index, enabled, keys);
      }
      else
      {
         return nil;
      }
   }

   public Maybe<ToolStripMenuItem> SubMenu(string text, int index = -1)
   {
      if (_currentMenu is (true, var currentMenu))
      {
         return SubMenu(currentMenu, text, index);
      }
      else if (_currentItem is (true, var currentItem))
      {
         return SubMenu(currentItem, text, index);
      }
      else
      {
         return nil;
      }
   }

   public ToolStripMenuItem Menu(string parentText, Func<string> textFunc, EventHandler handler, string shortcut = "", bool isChecked = false,
      int index = -1, bool enabled = true, Keys keys = Keys.None)
   {
      setParent(parentText);

      var parent = getParent(parentText);
      var text = textFunc();
      var item = new ToolStripMenuItem(text) { Name = SubmenuName(parentText, text), Checked = isChecked, Enabled = enabled };
      item.Click += handler;
      setShortcut(item, shortcut, keys);
      if (index == -1)
      {
         parent.DropDownItems.Add(item);
      }
      else
      {
         parent.DropDownItems.Insert(index, item);
      }

      dynamicTextItems[item] = textFunc;
      return item;
   }

   public ToolStripMenuItem Menu(ToolStripMenuItem parentItem, Func<string> textFunc, EventHandler handler, string shortcut = "",
      bool isChecked = false, int index = -1, bool enabled = true, Keys keys = Keys.None)
   {
      setParent(parentItem);

      var text = textFunc();
      var item = new ToolStripMenuItem(text) { Name = SubmenuName(parentItem.Text ?? "", text), Checked = isChecked, Enabled = enabled };
      item.Click += handler;
      setShortcut(item, shortcut, keys);
      if (index == -1)
      {
         parentItem.DropDownItems.Add(item);
      }
      else
      {
         parentItem.DropDownItems.Insert(index, item);
      }

      dynamicTextItems[item] = textFunc;
      return item;
   }

   public Maybe<ToolStripMenuItem> Menu(Func<string> textFunc, EventHandler handler, string shortcut = "", bool isChecked = false, int index = -1,
      bool enabled = true, Keys keys = Keys.None)
   {
      if (_currentMenu is (true, var currentMenu))
      {
         return Menu(currentMenu, textFunc, handler, shortcut, isChecked, index, enabled, keys);
      }
      else if (_currentItem is (true, var currentItem))
      {
         return Menu(currentItem, textFunc, handler, shortcut, isChecked, index, enabled, keys);
      }
      else
      {
         return nil;
      }
   }

   public ToolStripMenuItem Menu(string parentText, Func<Result<string>> textFunc, EventHandler handler, string shortcut = "", bool isChecked = false,
      int index = -1, bool checkOnResult = false, Keys keys = Keys.None)
   {
      setParent(parentText);

      var _text = textFunc();
      ToolStripMenuItem item;
      if (_text is (true, var text))
      {
         item = Menu(parentText, text, handler, shortcut, isChecked || checkOnResult, index, keys: keys);
      }
      else
      {
         item = Menu(parentText, _text.Exception.Message, handler, shortcut, isChecked, index, false, keys);
      }

      dynamicTextItems[item] = textFunc;
      return item;
   }

   public ToolStripMenuItem Menu(ToolStripMenuItem parentItem, Func<Result<string>> textFunc, EventHandler handler, string shortcut = "",
      bool isChecked = false, int index = -1, bool checkOnResult = false, Keys keys = Keys.None)
   {
      setParent(parentItem);

      var _text = textFunc();
      ToolStripMenuItem item;
      if (_text is (true, var text))
      {
         item = Menu(parentItem, text, handler, shortcut, isChecked || checkOnResult, index, keys: keys);
      }
      else
      {
         item = Menu(parentItem, _text.Exception.Message, handler, shortcut, isChecked, index, false, keys);
      }

      dynamicTextItems[item] = textFunc;
      return item;
   }

   public ToolStripMenuItem Menu(Func<Result<string>> textFunc, EventHandler handler, string shortcut = "", bool isChecked = false, int index = -1,
      bool checkOnResult = false, Keys keys = Keys.None)
   {
      var _text = textFunc();
      ToolStripMenuItem item;
      if (_text is (true, var text))
      {
         item = Menu(text, handler, shortcut, isChecked || checkOnResult, index, keys: keys);
      }
      else
      {
         item = Menu(_text.Exception.Message, handler, shortcut, isChecked, index, false, keys);
      }

      dynamicTextItems[item] = textFunc;
      return item;
   }

   public void AddHandler(string parentText, string text, EventHandler handler)
   {
      var _toolStripMenuItem =
         from submenus in Submenus(parentText)
         from toolStripMenuItemText in submenus.Maybe()[text]
         select toolStripMenuItemText;
      if (_toolStripMenuItem is (true, var toolStripMenuItem))
      {
         toolStripMenuItem.Click += handler;
      }
   }

   public Maybe<Delegate> ReplaceHandler(string parentText, string text, EventHandler handler)
   {
      var _submenus = new LazyMaybe<Submenus>(() => Submenus(parentText));
      var _item = _submenus.Then(submenus => submenus.Items[text]);
      var _delegate = _item.Then(item => item.ClearEvent("Click"));
      if (_delegate is (true, var @delegate) && _item is (true, var item))
      {
         item.Click += handler;
         return @delegate;
      }
      else
      {
         return nil;
      }
   }

   public Maybe<Delegate> ReplaceHandler(string text, EventHandler handler)
   {
      return _currentMenu.Map(currentMenu => ReplaceHandler(currentMenu, text, handler));
   }

   public void Separator(string parentText)
   {
      var parent = getParent(parentText);
      var item = new ToolStripSeparator();
      parent.DropDownItems.Add(item);
   }

   public void Separator(ToolStripMenuItem parentItem)
   {
      var item = new ToolStripSeparator();
      parentItem.DropDownItems.Add(item);
   }

   public void Separator()
   {
      if (_currentMenu is (true, var currentMenu))
      {
         Separator(currentMenu);
      }
      else if (_currentItem is (true, var currentItem))
      {
         Separator(currentItem);
      }
   }

   public void ContextMenuSeparator()
   {
      ToolStripItem item = new ToolStripSeparator();
      item.Name = MenuName(uniqueID());
      menuItems[item.Name] = item;
      tabIndexes[item.Name] = tabIndex++;
   }

   public void RemoveMenu(string parentText, string text)
   {
      setParent(parentText);

      var parent = getParent(parentText);
      var name = SubmenuName(parentText, text);

      var item = parent.DropDownItems[name];
      if (item is not null)
      {
         if (dynamicTextItems.ContainsKey(item))
         {
            dynamicTextItems.Remove(item);
         }

         parent.DropDownItems.Remove(item);
      }
   }

   public void RemoveMenu(ToolStripMenuItem parentItem, string text)
   {
      setParent(parentItem);

      var name = SubmenuName(parentItem.Text ?? "", text);

      var item = parentItem.DropDownItems[name];
      if (item is not null)
      {
         if (dynamicTextItems.ContainsKey(item))
         {
            dynamicTextItems.Remove(item);
         }

         parentItem.DropDownItems.Remove(item);
      }
   }

   public void RemoveMenu(string text)
   {
      if (_currentMenu is (true, var currentMenu))
      {
         RemoveMenu(currentMenu, text);
      }
      else if (_currentItem is (true, var currentItem))
      {
         RemoveMenu(currentItem, text);
      }
   }

   public void RemoveMenu(string parentText, int index)
   {
      var parent = getParent(parentText);

      var item = parent.DropDownItems[index];
      if (dynamicTextItems.ContainsKey(item))
      {
         dynamicTextItems.Remove(item);
      }

      parent.DropDownItems.RemoveAt(index);
   }

   public void RemoveMenu(ToolStripMenuItem parentItem, int index)
   {
      var item = parentItem.DropDownItems[index];
      if (dynamicTextItems.ContainsKey(item))
      {
         dynamicTextItems.Remove(item);
      }

      parentItem.DropDownItems.RemoveAt(index);
   }

   public void RemoveMenu(int index)
   {
      if (_currentMenu is (true, var currentMenu))
      {
         RemoveMenu(currentMenu, index);
      }
      else if (_currentItem is (true, var currentItem))
      {
         RemoveMenu(currentItem, index);
      }
   }

   protected static Result<Keys> shortcutKeys(string text)
   {
      if (text.Matches("^ /(['^%|']+)? /(/w+) $; f") is (true, var result))
      {
         var keys = (Keys)0;
         var prefix = result[0, 1];
         if (prefix.IsNotEmpty())
         {
            foreach (var sign in prefix)
            {
               switch (sign)
               {
                  case '^':
                     keys |= Keys.Control;
                     break;
                  case '%':
                     keys |= Keys.Alt;
                     break;
                  case '|':
                     keys |= Keys.Shift;
                     break;
               }
            }
         }

         return result[0, 2].Result().Enumeration<Keys>().Map(k => keys | k).ExceptionMessage($"Couldn't translate {result[0, 2]} into a key");
      }
      else
      {
         return fail($"Didn't understand key {text}");
      }
   }

   public void CreateMainMenu(Form form)
   {
      var menuStrip = new MenuStrip { Name = "menuMain", Dock = DockStyle.Top };
      var items = getMenuItems();
      menuStrip.Items.AddRange(items);
      form.MainMenuStrip = menuStrip;
      form.Controls.Add(menuStrip);
   }

   protected ToolStripItem[] getMenuItems() =>
   [
      .. menuItems.Values.Select(i => new { MenuItem = i, Index = tabIndexes[i.Name ?? ""] }).OrderBy(a => a.Index).Select(a => a.MenuItem)
   ];

   public void SetContextMenuControl(Control control)
   {
      _contextMenuControl = control;
      contextControlIsRichTextBox = control is RichTextBox;
   }

   public void CreateContextMenu(Control control)
   {
      SetContextMenuControl(control);
      CreateContextMenu();
   }

   public void CreateContextMenu()
   {
      if (_contextMenuControl is (true, var control))
      {
         contextControlIsRichTextBox = control is RichTextBox;
         var menuContext = new ContextMenuStrip { Name = $"menuContext{control.Name}" };
         var items = getMenuItems();
         menuContext.Items.AddRange(items);
         control.ContextMenuStrip = menuContext;
      }
   }

   public void StandardContextEdit(Document document)
   {
      ContextMenu("Undo", (_, _) => document.Undo(), "^Z");
      ContextMenu("Redo", (_, _) => document.Redo());
      ContextMenuSeparator();
      ContextMenu("Cut", (_, _) => document.Cut(), "^X");
      ContextMenu("Copy", (_, _) => document.Copy(), "^C");
      ContextMenu("Paste", (_, _) => document.Paste(), "^V");
      ContextMenu("Delete", (_, _) => document.Delete());
      ContextMenuSeparator();
      ContextMenu("Select All", (_, _) => document.SelectAll(), "^A");
   }

   public ToolStripMenuItem this[string text] => (ToolStripMenuItem)menuItems[MenuName(text)];

   public bool ContainsKey(string key) => menuItems.ContainsKey(MenuName(key));

   public Hash<string, ToolStripMenuItem> GetHash() => menuItems.ToHash(i => i.Key, i => (ToolStripMenuItem)i.Value);

   public HashInterfaceMaybe<string, ToolStripMenuItem> Items => new(this);

   public Maybe<Submenus> Submenus(string parentText) => Items[parentText].Map(p => new Submenus(p));

   public void RefreshText()
   {
      foreach (var (item, menuText) in dynamicTextItems)
      {
         item.Text = menuText.Text;
         item.Enabled = menuText.Enabled;
      }
   }

   public virtual void StandardContextEdit()
   {
      ContextMenu("Undo", (_, _) =>
      {
         if (_contextMenuControl is (true, TextBoxBase { CanUndo: true } textBox))
         {
            textBox.Undo();
            textBox.ClearUndo();
         }
      }, "^Z");

      if (contextControlIsRichTextBox)
      {
         ContextMenu("Redo", (_, _) =>
         {
            if (_contextMenuControl is (true, RichTextBox { CanRedo: true } richTextBox))
            {
               richTextBox.Redo();
            }
         });
      }

      ContextMenuSeparator();

      ContextMenu("Cut", (_, _) =>
      {
         if (_contextMenuControl is (true, TextBoxBase { SelectionLength: > 0 } textBox))
         {
            textBox.Cut();
         }
      }, "^X");

      ContextMenu("Copy", (_, _) =>
      {
         if (_contextMenuControl is (true, TextBoxBase { SelectionLength: > 0 } textBox))
         {
            textBox.Copy();
         }
      }, "^C");

      ContextMenu("Paste", (_, _) =>
      {
         var dataObject = Clipboard.GetDataObject();
         if (dataObject is not null && dataObject.GetDataPresent(DataFormats.Text) && _contextMenuControl is (true, TextBoxBase textBox))
         {
            textBox.Paste();
         }
      }, "^V");

      ContextMenu("Delete", (_, _) =>
      {
         if (_contextMenuControl is (true, TextBoxBase { SelectionLength: > 0 } textBox))
         {
            textBox.SelectedText = "";
         }
      });

      ContextMenuSeparator();

      ContextMenu("Select All", (_, _) =>
      {
         if (_contextMenuControl is (true, TextBoxBase textBox))
         {
            textBox.SelectAll();
         }
      }, "^A");
   }
}