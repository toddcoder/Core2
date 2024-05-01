﻿using System;
using System.Collections.Generic;
using System.Linq;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Core.Objects;
using Core.Strings;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Dates;

public static class TimeSpanExtensions
{
   private const string REGEX_TIMER_INTERVAL = "/(/d+) /s+ /(('milli')? 'sec' ('ond')? 's'? | 'min' ('ute')? 's'? | " +
      "'h' ('ou')? 'r' 's'? | 'days'?); f";

   public static string ToLongString(this TimeSpan span, bool includeMilliseconds) => Time.ToLongString(span, includeMilliseconds);

   public static string ToString(this TimeSpan span, bool includeMilliseconds)
   {
      List<string> list = [];

      if (span.Days > 0)
      {
         list.Add(span.Days == 1 ? "1 day" : $"{span.Days} days");
      }

      if (span.Hours > 0)
      {
         list.Add(span.Hours == 1 ? "1 hour" : $"{span.Hours} hours");
      }

      if (span.Minutes > 0)
      {
         list.Add(span.Minutes == 1 ? "1 minute" : $"{span.Minutes} minutes");
      }

      if (span.Seconds > 0)
      {
         if (includeMilliseconds && span.Milliseconds > 0)
         {
            list.Add($"{span.Seconds}.{span.Milliseconds:D3} seconds");
         }
         else
         {
            list.Add(span.Seconds == 1 ? "1 second" : $"{span.Seconds} seconds");
         }
      }
      else if (includeMilliseconds && span.Milliseconds > 0)
      {
         list.Add($"0.{span.Milliseconds:D3} seconds");
      }
      else
      {
         list.Add("0 seconds");
      }

      return list.Andify();
   }

   public static string ToShortString(this TimeSpan span, bool includeMilliseconds)
   {
      return Time.ToShortString(span, includeMilliseconds);
   }

   public static string ToVeryShortString(this TimeSpan span) => Time.ToVeryShortString(span);

   public static Result<TimeSpan> TimeSpan(this string source)
   {
      var intervals = source.Unjoin("/s* (',' | 'and') /s*; f");
      var spans = intervals.Where(i => i.IsNotEmpty()).Select(getSpan);
      var newSpan = new TimeSpan(0, 0, 0, 0);

      foreach (var _span in spans)
      {
         if (_span)
         {
            newSpan = newSpan.Add(_span);
         }
         else
         {
            return _span.Exception;
         }
      }

      return newSpan;
   }

   private static Result<TimeSpan> getSpan(string source)
   {
      return
         from result in source.Matches(REGEX_TIMER_INTERVAL).Result($"Can't match {source}")
         from span in getSpan(result)
         select span;
   }

   private static Result<TimeSpan> getSpan(MatchResult result)
   {
      var value = result.FirstGroup;
      var unit = result.SecondGroup;

      return
         from intValue in value.Result().Int32()
         from span in tryTo(() =>
         {
            if (unit.IsMatch("'millisec' ('ond')? 's'?; f"))
            {
               return new TimeSpan(0, 0, 0, 0, intValue);
            }
            else if (unit.IsMatch("'sec' ('ond') 's'?; f"))
            {
               return new TimeSpan(0, 0, 0, intValue, 0);
            }
            else if (unit.IsMatch("'min' ('ute')? 's'?; f"))
            {
               return new TimeSpan(0, intValue, 0);
            }
            else if (unit.IsMatch("'h' ('ou')? 'r' 's'?; f"))
            {
               return new TimeSpan(0, intValue, 0, 0);
            }
            else if (unit.IsMatch("'days'?; f"))
            {
               return new TimeSpan(intValue, 0, 0, 0);
            }
            else
            {
               throw fail($"Couldn't determine unit from {unit}");
            }
         })
         select span;
   }
}