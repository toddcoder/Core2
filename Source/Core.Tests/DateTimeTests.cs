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

   [TestMethod]
   public void DescriptionFromNowShortTest()
   {
      var beginningDate = "06/01/2021 6:30:01 AM".Value().DateTime();
      DateIncrementer incrementer = beginningDate;
      NowServer.SetToTest(incrementer);

      checkJustNowShort();
      checkMinuteAgoShort();
      checkHourAgoShort();
      checkYesterdayShort();
      checkDayBeforeYesterdayShort();
      checkThreeDaysAgoShort();
      check5DaysAgoShort();
      check7DaysAgoShort();
      check8DaysAgoShort();
      check30DaysAgoShort();
      checkAYearAgoShort();
   }

   protected static void checkDate(string message, DateTime dateTime, bool longForm)
   {
      Console.WriteLine($"{message}: [{dateTime}, {dateTime.DayOfWeek}] = {dateTime.DescriptionFromNow(longForm)}");
   }

   protected static void checkJustNow() => checkDate("Just now", NowServer.Now - 1.Millisecond(), true);

   protected static void checkJustNowShort() => checkDate("now", NowServer.Now - 1.Millisecond(), false);

   protected static void checkMinuteAgo() => checkDate("1 minute ago", NowServer.Now - 1.Minute(), true);

   protected static void checkMinuteAgoShort() => checkDate("1 min", NowServer.Now - 1.Minute(), false);

   protected static void checkHourAgo() => checkDate("1 hour ago", NowServer.Now - 1.Hour(), true);

   protected static void checkHourAgoShort() => checkDate("1 hr", NowServer.Now - 1.Hour(), false);

   protected static void checkYesterday() => checkDate("Yesterday", NowServer.Today - 1.Day(), true);

   protected static void checkYesterdayShort() => checkDate("yest.", NowServer.Today - 1.Day(), false);

   protected static void checkDayBeforeYesterday() => checkDate("Sunday", NowServer.Now - 2.Days(), true);

   protected static void checkDayBeforeYesterdayShort() => checkDate("sun", NowServer.Now - 2.Days(), false);

   protected static void checkThreeDaysAgo() => checkDate("Last Saturday", NowServer.Now - 3.Days(), true);

   protected static void checkThreeDaysAgoShort() => checkDate("last sat", NowServer.Now - 3.Days(), false);

   protected static void check5DaysAgo() => checkDate("Last Thursday", NowServer.Today - 5.Days(), true);

   protected static void check5DaysAgoShort() => checkDate("lat thu", NowServer.Today - 5.Days(), false);

   protected static void check7DaysAgo() => checkDate("Last Tuesday", NowServer.Today - 7.Days(), true);

   protected static void check7DaysAgoShort() => checkDate("last tue", NowServer.Today - 7.Days(), false);

   protected static void check8DaysAgo() => checkDate("May 24", NowServer.Today - 8.Days(), true);

   protected static void check8DaysAgoShort() => checkDate("yay 24", NowServer.Today - 8.Days(), false);

   protected static void check30DaysAgo() => checkDate("May 2", NowServer.Today - 30.Days(), true);

   protected static void check30DaysAgoShort() => checkDate("may 2", NowServer.Today - 30.Days(), false);

   protected static void checkAYearAgo() => checkDate("June 1, 2020", NowServer.Today - 365.Days(), true);

   protected static void checkAYearAgoShort() => checkDate("jun 1, 20", NowServer.Today - 365.Days(), false);

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