﻿using System.Collections;

namespace Core.Io.Delimited;

public class DelimitedTextEnumerator : IEnumerator<string[]>
{
   protected DelimitedTextReader reader;
   protected string[] current;

   public DelimitedTextEnumerator(DelimitedTextReader reader)
   {
      this.reader = reader;
      current = new string[reader.FieldCount];
   }

   public void Dispose() => reader.Dispose();

   public bool MoveNext()
   {
      if (reader.ReadRecord())
      {
         reader.CopyFields(current);

         return true;
      }
      else
      {
         return false;
      }
   }

   public void Reset()
   {
   }

   public string[] Current => current;

   object IEnumerator.Current => Current;
}