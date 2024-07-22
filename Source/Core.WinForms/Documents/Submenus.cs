using System.Collections;
using Core.Collections;

namespace Core.WinForms.Documents;

public class Submenus : IHash<string, ToolStripMenuItem>, IEnumerable<ToolStripMenuItem>
{
   protected ToolStripMenuItem parent;
   protected string parentText;

   internal Submenus(ToolStripMenuItem parent)
   {
      this.parent = parent;
      parentText = parent.Text ?? "";
   }

   public bool ContainsKey(string key) => parent.DropDownItems.ContainsKey(Menus.SubmenuName(parentText, key));

   public Hash<string, ToolStripMenuItem> GetHash() => [];

   public HashInterfaceMaybe<string, ToolStripMenuItem> Items => new(this);

   ToolStripMenuItem IHash<string, ToolStripMenuItem>.this[string text]
   {
      get
      {
         var submenuName = Menus.SubmenuName(parentText, text);
         var item = parent.DropDownItems[submenuName];

         return item is not null ? (ToolStripMenuItem)item : new ToolStripMenuItem();
      }
   }

   public IEnumerator<ToolStripMenuItem> GetEnumerator()
   {
      foreach (var dropDownItem in parent.DropDownItems)
      {
         if (dropDownItem is ToolStripMenuItem item)
         {
            yield return item;
         }
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}