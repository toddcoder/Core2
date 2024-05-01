﻿using System;
using System.Collections.Generic;
using Core.Assertions;
using Core.Enumerables;

namespace Core.Strings;

public class PadderArray
{
   protected int[] maximumLengths;

   public PadderArray(int count) => maximumLengths = new int[count];

   public int this[int index]
   {
      get => maximumLengths[index];
      set => maximumLengths[index] = value;
   }

   protected int withinBounds(int index) => index.Must().BeBetween(0).Until(maximumLengths.Length).Force();

   public int MaximumLengths(int index) => maximumLengths[withinBounds(index)];

   public void Evaluate(int index, string text) => Padder.Evaluate(text, ref maximumLengths[withinBounds(index)]);

   public void EvaluateAll(int index, params string[] text)
   {
      foreach (var item in text)
      {
         Evaluate(index, item);
      }
   }

   public void EvaluateAll(IEnumerable<string> enumerable)
   {
      foreach (var (i, item) in enumerable.Indexed())
      {
         Evaluate(i, item);
      }
   }

   public string PadLeft(int index, string text, char paddingCharacter = ' ')
   {
      index = withinBounds(index);

      if (text.IsNotEmpty())
      {
         return Padder.PadLeft(text, maximumLengths[index], paddingCharacter);
      }
      else
      {
         return Repeat(index, paddingCharacter.ToString());
      }
   }

   public string PadRight(int index, string text, char paddingCharacter = ' ')
   {
      index = withinBounds(index);

      if (text.IsNotEmpty())
      {
         return Padder.PadRight(text, maximumLengths[index], paddingCharacter);
      }
      else
      {
         return Repeat(index, paddingCharacter.ToString());
      }
   }

   public string PadCenter(int index, string text, char paddingCharacter = ' ')
   {
      index = withinBounds(index);

      if (text.IsNotEmpty())
      {
         return Padder.PadCenter(text, maximumLengths[index], paddingCharacter);
      }
      else
      {
         return Repeat(index, paddingCharacter.ToString());
      }
   }

   public string Pad(int index, string text, PadType type, char paddingCharacter = ' ')
   {
      return Padder.Pad(text, type, maximumLengths[withinBounds(index)], paddingCharacter);
   }

   public void Reset() => Array.Clear(maximumLengths, 0, maximumLengths.Length);

   public string Repeat(int index, string text) => Padder.Repeat(text, maximumLengths[withinBounds(index)]);
}