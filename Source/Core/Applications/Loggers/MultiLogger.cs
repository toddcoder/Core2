﻿using System.IO;
using System.Linq;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Loggers;

public class MultiLogger
{
   // ReSharper disable once CollectionNeverUpdated.Global
   protected Memo<string, Logger> loggers;
   protected StringSet keys;
   protected Maybe<string> _key;

   public MultiLogger(int indentation = 2)
   {
      keys = [];
      loggers = new Memo<string, Logger>.Function(key =>
      {
         keys.Add(key);
         return new Logger(indentation) { Key = key };
      });
      _key = nil;
   }

   public Logger this[string key]
   {
      get
      {
         _key = key;
         return loggers[_key];
      }
   }

   public override string ToString()
   {
      using var writer = new StringWriter();
      foreach (var logger in keys.Select(key => loggers[key]))
      {
         var key = logger.Key | "";
         var minDateTime = logger.MinDateTime.Map(dt => $"{dt:O} ") | "";
         var maxDateTime = logger.MaxDateTime.Map(dt => $" {dt:O}") | "";

         writer.WriteLine($"[{key}--------{minDateTime}-{maxDateTime}]");
         logger.Flush(writer);
      }

      return writer.ToString();
   }
}