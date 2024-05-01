using Core.DataStructures;
using Core.Numbers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class DataStructureTest
{
   [TestMethod]
   public void StackEnumerableTest()
   {
      MaybeStack<int> stack = [.. 0.Until(10)];

      foreach (var item in stack.Popping())
      {
         Console.WriteLine(item);
      }

      Console.WriteLine("---");

      for (var i = 0; i < 10; i++)
      {
         stack.Push(i);
      }

      foreach (var item in stack)
      {
         Console.WriteLine(item);
      }
   }

   [TestMethod]
   public void StackWhileTest()
   {
      MaybeStack<int> stack = [.. 0.Until(10)];

      while (stack.More())
      {
         if (stack.Last is (true, var last))
         {
            Console.WriteLine(last);
         }
      }
   }
}