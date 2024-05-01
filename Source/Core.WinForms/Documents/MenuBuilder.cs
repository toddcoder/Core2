﻿using Core.Monads;
using Core.Numbers;
using Core.Objects;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Documents;

public class MenuBuilder(Menus menus)
{
   public class MenuEnd;

   public class BuilderSubMenu;

   public class BuilderIsChecked;

   public class Separator;

   public class ContextMenuItem;

   public static MenuBuilder operator +(MenuBuilder builder, string text) => builder.Text(text);

   public static MenuBuilder operator +(MenuBuilder builder, Func<string> textFunc) => builder.Text(textFunc);

   public static MenuBuilder operator +(MenuBuilder builder, Func<Result<string>> textFunc) => builder.Text(textFunc);

   public static MenuBuilder operator +(MenuBuilder builder, EventHandler handler) => builder.Handler(handler);

   public static MenuBuilder operator +(MenuBuilder builder, Action handler) => builder.Handler(handler);

   public static MenuBuilder operator +(MenuBuilder builder, Keys keys) => builder.Keys(keys);

   public static MenuBuilder operator +(MenuBuilder builder, bool enabled) => builder.Enabled(enabled);

   public static MenuBuilder operator +(MenuBuilder builder, BuilderIsChecked _) => builder.IsChecked(true);

   public static ToolStripMenuItem operator +(MenuBuilder builder, MenuEnd _) => builder.Menu();

   public static ToolStripMenuItem operator +(MenuBuilder builder, BuilderSubMenu _) => builder.SubMenu();

   public static ToolStripMenuItem operator +(MenuBuilder builder, ContextMenuItem _) => builder.ContextMenu();

   protected MenuText menuText = MenuText.Empty;
   protected EventHandler handler = (_, _) => { };
   protected string shortcut = "";
   protected bool isChecked;
   protected int index = -1;
   protected bool enabled = true;
   protected Bits32<Keys> keys = System.Windows.Forms.Keys.None;
   protected Maybe<string> _parentText = nil;
   protected Maybe<ToolStripMenuItem> _parentItem = nil;

   public MenuBuilder(Menus menus, string parentText) : this(menus)
   {
      _parentText = parentText;
   }

   public MenuBuilder(Menus menus, ToolStripMenuItem parentItem, string parentText = "") : this(menus)
   {
      _parentItem = parentItem;
      _parentText = parentText.NotEmpty();
   }

   public MenuBuilder Text(string text)
   {
      menuText = text;
      return this;
   }

   public MenuBuilder Text(Func<string> textFunc)
   {
      menuText = textFunc;
      return this;
   }

   public MenuBuilder Text(Func<Result<string>> textFunc)
   {
      menuText = textFunc;
      return this;
   }

   public MenuBuilder Handler(EventHandler handler)
   {
      this.handler = handler;
      return this;
   }

   public MenuBuilder Handler(Action action)
   {
      handler = (_, _) => action();
      return this;
   }

   public MenuBuilder Shortcut(string shortcut)
   {
      this.shortcut = shortcut;
      return this;
   }

   public MenuBuilder Control()
   {
      keys[System.Windows.Forms.Keys.Control] = true;
      return this;
   }

   public MenuBuilder Control(string key) => Control().Key(key);

   public MenuBuilder Alt()
   {
      keys[System.Windows.Forms.Keys.Alt] = true;
      return this;
   }

   public MenuBuilder Alt(string key) => Alt().Key(key);

   public MenuBuilder Shift()
   {
      keys[System.Windows.Forms.Keys.Shift] = true;
      return this;
   }

   public MenuBuilder Shift(string key) => Shift().Key(key);

   public MenuBuilder Key(string key)
   {
      if (key.Maybe().Enumeration<Keys>() is (true, var keyValue))
      {
         keys[keyValue] = true;
      }

      return this;
   }

   public MenuBuilder Keys(Keys keys)
   {
      this.keys[keys] = true;
      return this;
   }

   public MenuBuilder IsChecked(bool isChecked)
   {
      this.isChecked = isChecked;
      return this;
   }

   public MenuBuilder Enabled(bool enabled)
   {
      this.enabled = enabled;
      return this;
   }

   protected ToolStripMenuItem menu(string parentText) => menuText.ToObject() switch
   {
      string text => menus.Menu(parentText, text, handler, shortcut, isChecked, index, enabled, keys),
      Func<string> func => menus.Menu(parentText, func, handler, shortcut, isChecked, index, enabled, keys),
      Func<Result<string>> func => menus.Menu(parentText, func, handler, shortcut, isChecked, index, enabled, keys),
      _ => throw fail("Unexpected item")
   };

   protected ToolStripMenuItem menu(ToolStripMenuItem parentItem) => menuText.ToObject() switch
   {
      string text => menus.Menu(parentItem, text, handler, shortcut, isChecked, index, enabled, keys),
      Func<string> func => menus.Menu(parentItem, func, handler, shortcut, isChecked, index, enabled, keys),
      Func<Result<string>> func => menus.Menu(parentItem, func, handler, shortcut, isChecked, index, enabled, keys),
      _ => throw fail("Unexpected item")
   };

   protected ToolStripMenuItem menu() => menuText.ToObject() switch
   {
      string text => menus.Menu(text, handler, shortcut, isChecked, index, enabled, keys),
      Func<string> func => menus.Menu(func, handler, shortcut, isChecked, index, enabled, keys),
      Func<Result<string>> func => menus.Menu(func, handler, shortcut, isChecked, index, enabled, keys),
      _ => throw fail("Unexpected item")
   };

   public ToolStripMenuItem Menu()
   {
      if (_parentText is (true, var parentText))
      {
         return menu(parentText);
      }
      else if (_parentItem is (true, var parentItem))
      {
         return menu(parentItem);
      }
      else
      {
         return menu();
      }
   }

   public ToolStripMenuItem SubMenu() => menuText.ToObject() switch
   {
      string text => menus.SubMenu(text, index),
      _ => throw fail("Unexpected item")
   };

   public ToolStripMenuItem ContextMenu() => menuText.ToObject() switch
   {
      string text => menus.ContextMenu(text, handler, shortcut, isChecked, enabled, keys),
      Func<string> textFunc => menus.ContextMenu(textFunc, handler, shortcut, isChecked, enabled, keys),
      Func<Result<string>> textFunc => menus.ContextMenu(textFunc, handler, shortcut, isChecked, enabled, keys),
      _ => throw fail("Unexpected item")
   };
}