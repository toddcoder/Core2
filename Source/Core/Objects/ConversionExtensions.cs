using Core.Strings;
using System.Globalization;
using System;
using System.Linq;
using Core.Matching;
using Core.Monads;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Objects;

public static class ConversionExtensions
{
   public class ValueConverter(string source)
   {
      public bool Boolean(bool defaultValue = false)
      {
         if (source.IsNotEmpty())
         {
            return source switch
            {
               "1" => true,
               "0" => false,
               _ => bool.TryParse(source, out var result) ? result : defaultValue
            };
         }
         else
         {
            return defaultValue;
         }
      }

      public static CultureInfo FormatProvider(NumberStyles numberStyles)
      {
         if ((numberStyles & NumberStyles.AllowCurrencySymbol) > 0)
         {
            return new CultureInfo("en-US");
         }
         else
         {
            return CultureInfo.InvariantCulture;
         }
      }

      public byte Byte(byte defaultValue = 0, NumberStyles numberStyles = NumberStyles.Integer)
      {
         if (source.IsNotEmpty())
         {
            return byte.TryParse(source, numberStyles, FormatProvider(numberStyles), out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public int Int32(int defaultValue = 0, NumberStyles numberStyles = NumberStyles.Integer)
      {
         if (source.IsNotEmpty())
         {
            return int.TryParse(source, numberStyles, FormatProvider(numberStyles), out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public long Int64(long defaultValue = 0, NumberStyles numberStyles = NumberStyles.Integer)
      {
         if (source.IsNotEmpty())
         {
            return long.TryParse(source, numberStyles, FormatProvider(numberStyles), out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public float Single(float defaultValue = 0, NumberStyles numberStyles = NumberStyles.Float)
      {
         if (source.IsNotEmpty())
         {
            return float.TryParse(source, numberStyles, FormatProvider(numberStyles), out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public double Double(double defaultValue = 0, NumberStyles numberStyles = NumberStyles.Float)
      {
         if (source.IsNotEmpty())
         {
            return double.TryParse(source, numberStyles, FormatProvider(numberStyles), out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public decimal Decimal(decimal defaultValue = 0, NumberStyles numberStyles = NumberStyles.Float)
      {
         if (source.IsNotEmpty())
         {
            return decimal.TryParse(source, numberStyles, FormatProvider(numberStyles), out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public DateTime DateTime(DateTime defaultValue)
      {
         if (source.IsNotEmpty())
         {
            return System.DateTime.TryParse(source, out var result) ? result : defaultValue;
         }
         else
         {
            return defaultValue;
         }
      }

      public DateTime DateTime() => DateTime(System.DateTime.MinValue);

      public Guid Guid()
      {
         if (source.IsNotEmpty())
         {
            return System.Guid.TryParse(source, out var guid) ? guid : System.Guid.Empty;
         }
         else
         {
            return System.Guid.Empty;
         }
      }

      public T Enumeration<T>(bool ignoreCase = true) where T : Enum
      {
         try
         {
            return (T)Enum.Parse(typeof(T), source, ignoreCase);
         }
         catch
         {
            return (T)Enum.ToObject(typeof(T), 0);
         }
      }

      public object Enumeration(Type type, bool ignoreCase = true) => Enum.Parse(type, source, ignoreCase);

      public TimeSpan TimeSpan(TimeSpan defaultValue) => getSpans(source).Recover(_ => defaultValue);

      public TimeSpan TimeSpan() => getSpans(source).ForceValue();

      public static T Cast<T>(object obj, Func<string> message)
      {
         try
         {
            return (T)obj;
         }
         catch (Exception exception)
         {
            var formatter = new Formatter { ["object"] = obj.ToString() ?? "", ["e"] = exception.Message };
            throw new ApplicationException(formatter.Format(message()));
         }
      }

      public static T Cast<T>(object obj) => (T)obj;
   }

   public class MaybeConverter(string source)
   {
      public Maybe<bool> Boolean()
      {
         if (source.IsNotEmpty())
         {
            return source switch
            {
               "1" => true,
               "0" => false,
               _ => bool.TryParse(source, out var result) ? result : nil
            };
         }
         else
         {
            return nil;
         }
      }

      public Maybe<byte> Byte(NumberStyles numberStyles = NumberStyles.Integer)
      {
         if (source.IsNotEmpty())
         {
            return byte.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<int> Int32(NumberStyles numberStyles = NumberStyles.Integer)
      {
         if (source.IsNotEmpty())
         {
            return int.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<long> Int64(NumberStyles numberStyles = NumberStyles.Integer)
      {
         if (source.IsNotEmpty())
         {
            return long.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<float> Single(NumberStyles numberStyles = NumberStyles.Float)
      {
         if (source.IsNotEmpty())
         {
            return float.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<double> Double(NumberStyles numberStyles = NumberStyles.Float)
      {
         if (source.IsNotEmpty())
         {
            return double.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<decimal> Decimal(NumberStyles numberStyles = NumberStyles.Float)
      {
         if (source.IsNotEmpty())
         {
            return decimal.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<DateTime> DateTime()
      {
         if (source.IsNotEmpty())
         {
            return System.DateTime.TryParse(source, out var result) ? result : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<Guid> Guid()
      {
         if (source.IsNotEmpty())
         {
            return System.Guid.TryParse(source, out var guid) ? guid : nil;
         }
         else
         {
            return nil;
         }
      }

      public Maybe<T> Enumeration<T>(bool ignoreCase = true) where T : struct, Enum
      {
         try
         {
            return Enum.TryParse<T>(source, ignoreCase, out var result) ? result : nil;
         }
         catch
         {
            return nil;
         }
      }

      public Maybe<object> Enumeration(Type type, bool ignoreCase = true)
      {
         try
         {
            return Enum.Parse(type, source, ignoreCase);
         }
         catch
         {
            return nil;
         }
      }

      public Maybe<TimeSpan> TimeSpan() => getSpans(source).Maybe();
   }

   public class MaybeCaster(object obj)
   {
      public Maybe<T> Cast<T>() where T : notnull => obj is T t ? t : nil;
   }

   public class ResultConverter(string source)
   {
      public Result<bool> Boolean() => tryTo(() => bool.Parse(source));

      public Result<byte> Byte(NumberStyles numberStyles = NumberStyles.Integer)
      {
         return tryTo(() => byte.Parse(source, numberStyles));
      }

      public Result<int> Int32(NumberStyles numberStyles = NumberStyles.Integer)
      {
         return tryTo(() => int.Parse(source, numberStyles));
      }

      public Result<long> Int64(NumberStyles numberStyles = NumberStyles.Integer)
      {
         return tryTo(() => long.Parse(source, numberStyles));
      }

      public Result<float> Single(NumberStyles numberStyles = NumberStyles.Float)
      {
         return tryTo(() => float.Parse(source, numberStyles));
      }

      public Result<double> Double(NumberStyles numberStyles = NumberStyles.Float)
      {
         return tryTo(() => double.Parse(source, numberStyles));
      }

      public Result<decimal> Decimal(NumberStyles numberStyles = NumberStyles.Float)
      {
         return tryTo(() => decimal.Parse(source, numberStyles));
      }

      public Result<DateTime> DateTime() => tryTo(() => System.DateTime.Parse(source));

      public Result<Guid> Guid() => tryTo(() => System.Guid.Parse(source));

      public Result<T> Enumeration<T>(bool ignoreCase = true) where T : struct, Enum
      {
         return tryTo(() => (T)Enum.Parse(typeof(T), source, ignoreCase));
      }

      public Result<object> Enumeration(Type type, bool ignoreCase = true)
      {
         return tryTo(() => Enum.Parse(type, source, ignoreCase));
      }

      public Result<TimeSpan> TimeSpan() => getSpans(source);
   }

   public class ResultCaster(object obj)
   {
      public Result<T> Cast<T>() where T : notnull => (T)obj;
   }

   public class OptionalConverter(string source)
   {
      public Optional<bool> Boolean()
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return source switch
               {
                  "1" => true,
                  "0" => false,
                  _ => bool.TryParse(source, out var result) ? result : nil
               };
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<byte> Byte(NumberStyles numberStyles = NumberStyles.Integer)
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return byte.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<int> Int32(NumberStyles numberStyles = NumberStyles.Integer)
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return int.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<long> Int64(NumberStyles numberStyles = NumberStyles.Integer)
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return long.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<float> Single(NumberStyles numberStyles = NumberStyles.Float)
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return float.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<double> Double(NumberStyles numberStyles = NumberStyles.Float)
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return double.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<decimal> Decimal(NumberStyles numberStyles = NumberStyles.Float)
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return decimal.TryParse(source, numberStyles, ValueConverter.FormatProvider(numberStyles), out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<DateTime> DateTime()
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return System.DateTime.TryParse(source, out var result) ? result : nil;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<Guid> Guid()
      {
         try
         {
            if (source.IsNotEmpty())
            {
               return System.Guid.TryParse(source, out var guid) ? guid : nil;
            }
            else
            {
               return nil;
            }
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<T> Enumeration<T>(bool ignoreCase = true) where T : struct, Enum
      {
         try
         {
            return Enum.TryParse<T>(source, ignoreCase, out var result) ? result : nil;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<object> Enumeration(Type type, bool ignoreCase = true)
      {
         try
         {
            return Enum.Parse(type, source, ignoreCase);
         }
         catch (Exception exception)
         {
            return exception;
         }
      }

      public Optional<TimeSpan> TimeSpan() => getSpans(source).Optional();
   }

   public class OptionalCaster(object obj)
   {
      public Optional<T> Cast<T>() where T : notnull
      {
         try
         {
            return obj is T t ? t : nil;
         }
         catch (Exception exception)
         {
            return exception;
         }
      }
   }

   public static ValueConverter Value(this string source) => new(source);

   public static MaybeConverter Maybe(this string source) => new(source);

   public static MaybeCaster Maybe(this object obj) => new(obj);

   public static ResultConverter Result(this string source) => new(source);

   public static ResultCaster Result(this object obj) => new(obj);

   public static OptionalConverter Optional(this string source) => new(source);

   public static OptionalCaster Optional(this object obj) => new(obj);

   private const string REGEX_TIMER_INTERVAL = "/(/d+) /s+ /(('milli')? 'sec' ('ond')? 's'? | 'min' ('ute')? 's'? | " +
      "'h' ('ou')? 'r' 's'? | 'days'?); f";

   private static Result<TimeSpan> getSpan(string source)
   {
      return
         from result in source.Matches(REGEX_TIMER_INTERVAL).Result($"Can't match {source}")
         from span in getSpan(result)
         select span;
   }

   private static Result<TimeSpan> getSpans(string source)
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