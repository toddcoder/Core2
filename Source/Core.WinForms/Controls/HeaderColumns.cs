using System.Collections;
using Core.Collections;

namespace Core.WinForms.Controls;

public class HeaderColumns : IHash<string, HeaderColumn>, IEnumerable<(string name, HeaderColumn headerColumn)>
{
   protected StringHash<HeaderColumn> headerColumns = [];
   protected Hash<int, string> indexToKeys = [];

   public event EventHandler<EventArgs>? Updated;

   public HeaderColumn this[string key]
   {
      get => headerColumns[key];
      set
      {
         value.Index = headerColumns.Count;
         headerColumns[key] = value;
         indexToKeys[value.Index] = key;
         Updated?.Invoke(this, EventArgs.Empty);
      }
   }

   public bool ContainsKey(string key) => headerColumns.ContainsKey(key);

   public Hash<string, HeaderColumn> GetHash() => headerColumns;

   public HashInterfaceMaybe<string, HeaderColumn> Items => headerColumns.Items;

   public HashMaybe<string, HeaderColumn> Maybe => new(headerColumns);

   public IEnumerator<(string name, HeaderColumn headerColumn)> GetEnumerator()
   {
      foreach (var index in indexToKeys.Keys.Order())
      {
         var name = indexToKeys[index];
         yield return (name, headerColumns[name]);
      }
   }

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}