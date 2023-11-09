using System;

namespace Core.Strings.Text;

internal class Modification
{
   public Modification(string[] rawData)
   {
      RawData = rawData;

      HashedItems = Array.Empty<int>();
      RawData = Array.Empty<string>();
      Modifications = Array.Empty<bool>();
      Items = Array.Empty<string>();
   }

   public int[] HashedItems { get; set; }

   public string[] RawData { get; }

   public bool[] Modifications { get; set; }

   public string[] Items { get; set; }
}