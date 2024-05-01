using Core.Dates;
using Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class TimeSpanTests
{
   [TestMethod]
   public void ToStringTest()
   {
      var timeSpan = new TimeSpan(1, 2, 3, 4, 53);
      Console.WriteLine(timeSpan.ToString(true));
   }

   [TestMethod]
   public void TimeToGoTest()
   {
      var startTime = DateTime.Now;
      foreach (var percent in 1.To(100))
      {
         var elapsed = DateTime.Now - startTime;
         var timeToGo = elapsed.DescriptionToGo(percent);
         Console.WriteLine(timeToGo);
         Thread.Sleep(1000);
      }
      /*var now = DateTime.Now;
      DateIncrementer incrementer = now;
      NowServer.SetToTest(incrementer);
      foreach (var percent in 1.To(100))
      {
         var elapsed = incrementer - now;
         Console.WriteLine(elapsed.DescriptionToGo(percent));
         incrementer += 30.Seconds();
      }*/
   }
}