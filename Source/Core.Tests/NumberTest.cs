using Core.Collections;
using Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class NumberTest
{
   [TestMethod]
   public void WordsTest()
   {
      double[] numbers = [5.0, 58.8, 153.69, 1_964.5, 12_345.77, 138_444.0, 1_234_567.89];
      foreach (var number in numbers)
      {
         var _words = number.ToWords();
         if (_words is (true, var words))
         {
            Console.WriteLine(words);
         }
         else
         {
            Console.WriteLine(_words.Exception.Message);
         }
      }
   }

   [TestMethod]
   public void RandomExtensionTest()
   {
      var random = new Random();
      var memo = new Memo<int, int>.Value(0);
      for (var i = 0; i < 1000; i++)
      {
         var number = random.Next(0, 740, 20);
         memo[number]++;
      }

      foreach (var key in memo.Keys.Order())
      {
         var value = memo[key];
         Console.WriteLine($"{key}: {value}");
      }
   }
}