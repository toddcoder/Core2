using System.Collections.Generic;
using System.Linq;
using Core.Arrays;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;

namespace Core.Strings;

public class OldPadding
{
   protected const string DEFAULT_SPLIT_PATTERN = "/s* ',' /s*; f";
   protected const string LINE_SPLIT_PATTERN = "/r /n | /r | /n; f";

   protected PadType[] padTypes;
   protected string splitPattern;
   protected string columnSeparator;
   protected string text;
   protected int[] sizes;

   public OldPadding(params PadType[] padTypes)
   {
      this.padTypes = padTypes;
      splitPattern = DEFAULT_SPLIT_PATTERN;
      columnSeparator = " ";
      text = "";
      sizes = [];
   }

   public OldPadding(string padTypes)
   {
      this.padTypes = [.. padTypes.Unjoin(DEFAULT_SPLIT_PATTERN).Select(s => s.Maybe().Enumeration<PadType>()).SomeValue()];
      splitPattern = DEFAULT_SPLIT_PATTERN;
      columnSeparator = " ";
      text = "";
      sizes = [];
   }

   public OldPadding(int count, PadType padType)
   {
      padTypes = [.. Enumerable.Repeat(padType, count)];
      splitPattern = DEFAULT_SPLIT_PATTERN;
      columnSeparator = " ";
      text = "";
      sizes = [];
   }

   public string SplitPattern
   {
      get => splitPattern;
      set => splitPattern = value;
   }

   public string ColumnSeparator
   {
      get => columnSeparator;
      set => columnSeparator = value;
   }

   public string Text
   {
      get
      {
         if (text.IsNotEmpty())
         {
            string[][] lines = [.. text.Unjoin(LINE_SPLIT_PATTERN).Select(line => line.Unjoin(splitPattern))];
            return getText(lines);
         }
         else
         {
            return "";
         }
      }
      set => text = value;
   }

   protected string getText(string[][] lines)
   {
      if (lines.Length != 0)
      {
         IEnumerable<int>[] enumerable = [.. lines.Select(line => (int[]) [.. line.Select(column => column.Length)]).Pivot(() => 0)];
         sizes = [.. enumerable.Select(columns => columns.Max())];
         var sizesLength = sizes.Length;
         var paddedPadTypes = padTypes.Pad(sizesLength, PadType.Right);
         lines = [.. lines.Select(columns => columns.Pad(sizesLength, ""))];

         return lines
            .Select(columns => columns
               .Select((column, i) => column.Pad(paddedPadTypes[i], sizes[i]))
               .ToString(columnSeparator))
            .ToString("\r\n");
      }
      else
      {
         return "";
      }
   }

   public string ToString(IEnumerable<IEnumerable<string>> source) => getText([.. source.Select(columns => (string[]) [.. columns])]);

   public override string ToString() => Text;

   public string Header(params string[] headers)
   {
      if (sizes.Length == 0)
      {
         return getText([headers]);
      }
      else
      {
         var newHeaders = headers.LimitTo(sizes.Length, "");
         return newHeaders.Zip(sizes, (header, size) => header.PadCenter(size)).ToString(columnSeparator);
      }
   }
}