﻿using System;
using System.Collections.Generic;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Enumerables;

public class EnumerableQueue<T> where T : notnull
{
   protected IEnumerator<T> enumerator;

   public EnumerableQueue(IEnumerable<T> enumerable)
   {
      enumerator = enumerable.GetEnumerator();
   }

   public Optional<T> Next()
   {
      try
      {
         if (enumerator.MoveNext())
         {
            try
            {
               return enumerator.Current;
            }
            catch (Exception innerException)
            {
               return innerException;
            }
         }
         else
         {
            return nil;
         }
      }
      catch (Exception outerException)
      {
         return outerException;
      }
   }
}