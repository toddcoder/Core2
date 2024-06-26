using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms;

public static class ListViewExtensions
{
   public static void AutoSizeColumns(this ListView listView)
   {
      listView.BeginUpdate();

      listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
      var headerWidths = new int[listView.Columns.Count];

      for (var i = 0; i < listView.Columns.Count; i++)
      {
         headerWidths[i] = listView.Columns[i].Width;
      }

      listView.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);

      for (var i = 0; i < headerWidths.Length; i++)
      {
         listView.Columns[i].Width = Math.Max(listView.Columns[i].Width, headerWidths[i]);
      }

      listView.EndUpdate();
   }

   public static IEnumerable<ListViewItem> AllItems(this ListView listView)
   {
      foreach (ListViewItem item in listView.Items)
      {
         yield return item;
      }
   }

   public static IEnumerable<ListViewItem> AllCheckedItems(this ListView listView)
   {
      foreach (ListViewItem item in listView.CheckedItems)
      {
         yield return item;
      }
   }

   public static Maybe<ListViewItem> SelectedItem(this ListView listView)
   {
      if (listView.SelectedIndices.Count > 0)
      {
         var index = listView.SelectedIndices[0];
         if (index > -1)
         {
            return listView.Items[index];
         }
      }

      return nil;
   }

   public static Maybe<(ListViewItem item, int index)> SelectedItemWithIndex(this ListView listView)
   {
      if (listView.SelectedIndices.Count > 0)
      {
         var index = listView.SelectedIndices[0];
         if (index > -1)
         {
            return (listView.Items[index], index);
         }
      }

      return nil;
   }

   public static Maybe<(string text, int index)> SelectedText(this ListView listView)
   {
      if (listView.SelectedIndices.Count > 0)
      {
         var index = listView.SelectedIndices[0];
         if (index > -1)
         {
            return (listView.Items[index].Text, index);
         }
      }

      return nil;
   }

   public static Maybe<ListViewItem> FindItem(this ListView listView, Func<ListViewItem, bool> predicate)
   {
      foreach (ListViewItem item in listView.Items)
      {
         if (predicate(item))
         {
            return item;
         }
      }

      return nil;
   }

   public static bool AnyChecked(this ListView listView) => listView.CheckedItems.Count > 0;

   public static Maybe<ListViewItem> Add(this ListView listView, params string[] subItemTexts)
   {
      if (subItemTexts.Length > 0)
      {
         var item = listView.Items.Add(subItemTexts[0]);
         string[] remainingText = [.. subItemTexts.Skip(1)];
         var length = remainingText.Length.MinOf(item.SubItems.Count - 1);
         foreach (var i in ..length)
         {
            item.SubItems.Add(remainingText[i]);
         }

         return item;
      }
      else
      {
         return nil;
      }
   }

   public static Maybe<ListViewItem> AddGrouped(this ListView listView, string groupName, params string[] subItemTexts)
   {
      if (listView.Add(subItemTexts) is (true, var item))
      {
         var _group = listView.Groups[groupName].NotNull();
         if (_group is (true, var group))
         {
            item.Group = group;
         }
         else
         {
            group = listView.Groups.Add(groupName, groupName);
            item.Group = group;
         }
         return item;
      }
      else
      {
         return nil;
      }
   }
}