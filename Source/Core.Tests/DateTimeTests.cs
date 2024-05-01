using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Dates;
using Core.Dates.DateIncrements;
using Core.Dates.Now;
using Core.Objects;
using Timeout = Core.Dates.Timeout;

namespace Core.Tests;

[TestClass]
public class DateTimeTests
{
   [TestMethod]
   public void DescriptionFromNowTest()
   {
      var beginningDate = "06/01/2021 6:30:01 AM".Value().DateTime();
      DateIncrementer incrementer = beginningDate;
      NowServer.SetToTest(incrementer);

      checkJustNow();
      checkMinuteAgo();
      checkHourAgo();
      checkYesterday();
      checkDayBeforeYesterday();
      checkThreeDaysAgo();
      check5DaysAgo();
      check7DaysAgo();
      check8DaysAgo();
      check30DaysAgo();
      checkAYearAgo();
   }

   protected static void checkDate(string message, DateTime dateTime)
   {
      Console.WriteLine($"{message}: [{dateTime}, {dateTime.DayOfWeek}] = {dateTime.DescriptionFromNow()}");
   }

   protected static void checkJustNow() => checkDate("Just now", NowServer.Now - 1.Millisecond());

   protected static void checkMinuteAgo() => checkDate("1 Minute", NowServer.Now - 1.Minute());

   protected static void checkHourAgo() => checkDate("1 Hour", NowServer.Now - 1.Hour());

   protected static void checkYesterday() => checkDate("Yesterday", NowServer.Today - 1.Day());

   protected static void checkDayBeforeYesterday() => checkDate("Day before yesterday", NowServer.Now - 2.Days());

   protected static void checkThreeDaysAgo() => checkDate("3 days ago", NowServer.Now - 3.Days());

   protected static void check5DaysAgo() => checkDate("5 days ago", NowServer.Today - 5.Days());

   protected static void check7DaysAgo() => checkDate("7 days ago", NowServer.Today - 7.Days());

   protected static void check8DaysAgo() => checkDate("8 days ago", NowServer.Today - 8.Days());

   protected static void check30DaysAgo() => checkDate("30 days ago", NowServer.Today - 30.Days());

   protected static void checkAYearAgo() => checkDate("1 year ago", NowServer.Today - 365.Days());

   [TestMethod]
   public void IncrementalDateDescriptionTest()
   {
      var beginningDate = "06/01/2021".Value().DateTime();
      DateIncrementer incrementer = beginningDate;
      NowServer.SetToTest(incrementer);

      for (var i = 0; i <= 30; i++)
      {
         var date = NowServer.Now - i.Days();
         Console.WriteLine($"{date:d} [{date.DayOfWeek}] -> {date.DescriptionFromNow()}");
      }
   }

   [TestMethod]
   public void ZuluDateTimeTest()
   {
      var now = DateTime.Now;
      var zulu = now.Zulu();
      Console.WriteLine(zulu);
   }

   [TestMethod]
   public void ZuluToDateTimeTest()
   {
      var source = "2022-11-18T15:00:18.253Z";
      var _date = source.Maybe().DateTime();
      if (_date.Map(d => d.Zulu()) is (true, var zulu))
      {
         Console.WriteLine(zulu);
      }
   }

   [TestMethod]
   public void WorkingTest()
   {
      Timeout timeout = 10.Seconds();
      while (timeout.IsPending())
      {
         Console.WriteLine(timeout.Elapsed.ToLongString(true));
      }
   }

   [TestMethod]
   public void ToFromZuluTest()
   {
      var localTime = "04/10/2024 10:30:00 AM".Value().DateTime();
      Console.WriteLine($"Local time: {localTime}");

      var zuluTime = localTime.ToZulu();
      Console.WriteLine($"Zulu time: {zuluTime}");

      localTime = zuluTime.FromZulu();
      Console.WriteLine($"Local time: {localTime}");
   }
}