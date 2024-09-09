using System.Diagnostics;
using Core.Assertions;
using Core.Collections;
using Core.Collections.Infix;
using Core.DataStructures;
using Core.Dates;
using Core.Monads.Lazy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.MonadFunctions;

namespace Core.Tests;

[TestClass]
public class CollectionTest
{
   [TestMethod]
   public void InfixListWithInfixDataTest()
   {
      var list = new InfixList<int, char> { { 1, '+' }, { 2, '-' }, 3 };
      Console.WriteLine(list);

      var intStack = new Stack<int>();
      var charStack = new Stack<char>();

      foreach (var (number, _op) in list)
      {
         intStack.Push(number);

         if (_op is (true, var op))
         {
            charStack.Push(op);
         }
      }

      while (charStack.Count > 0)
      {
         var y = intStack.Pop();
         var x = intStack.Pop();
         var op = charStack.Pop();
         switch (op)
         {
            case '+':
               intStack.Push(x + y);
               break;
            case '-':
               intStack.Push(x - y);
               break;
         }
      }

      var result = intStack.Pop();
      result.Must().Equal(result).OrThrow();
   }

   [TestMethod]
   public void PriorityQueueTest()
   {
      PriorityQueue<int> queue = [1, 5, 3, 6, 9];

      LazyMaybe<int> _item = nil;
      while (_item.ValueOf(queue.Dequeue(), true) is (true, var item))
      {
         Console.WriteLine(item);
      }
   }

   protected static string time(Func<Func<string>, string> func)
   {
      var stopwatch = new Stopwatch();
      stopwatch.Start();

      var result = func(() => stopwatch.Elapsed.ToString(true));

      stopwatch.Stop();
      return result;
   }

   [TestMethod]
   public void MaybeStackItemTest()
   {
      MaybeStack<string> stack = ["alpha", "bravo", "charlie"];

      LazyResult<string> _item = nil;
      if (_item.ValueOf(stack.Item(0), true) is (true, var item))
      {
         Console.WriteLine(item);
      }
      else
      {
         Console.WriteLine(_item.Exception.Message);
      }

      if (_item.ValueOf(stack.Item(-1), true) is (true, var item2))
      {
         Console.WriteLine(item2);
      }
      else
      {
         Console.WriteLine(_item.Exception.Message);
      }
   }

   [TestMethod]
   public void RingTest()
   {
      var ring = new Ring<string>("alpha", "bravo", "charlie");
      var index = 0;

      foreach (var item in ring)
      {
         Console.WriteLine($"{index}: {item}");
         if (index++ >= 10)
         {
            break;
         }
      }
   }
}