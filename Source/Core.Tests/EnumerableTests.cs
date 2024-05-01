using Core.Assertions;
using Core.Enumerables;
using Core.Monads;
using Core.Numbers;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class EnumerableTests
{
   protected class ItemToSort
   {
      public ItemToSort(string key)
      {
         Key = key;
         Ordinal = key[0];
      }

      public string Key { get; }

      public int Ordinal { get; }

      public override string ToString() => $"{Key}: {Ordinal}";
   }

   protected class SortableItem
   {
      public SortableItem()
      {
         Branch = string.Empty;
         Order = string.Empty;
      }

      public string Branch { get; set; }

      public string Order { get; set; }
   }

   [TestMethod]
   public void FirstOrNoneTest()
   {
      var testArray = 'f'.DownTo('a');
      var _char = testArray.FirstOrNone();
      if (_char is (true, var @char))
      {
         @char.ToString().Must().Equal("f").OrThrow();
         Console.WriteLine($"{@char} == 'f'");
      }
   }

   [TestMethod]
   public void FirstOrFailTest()
   {
      int[] testArray = [.. 0.UpUntil(10)];
      var _first = testArray.FirstOrFailure("Not found");
      if (_first is (true, var first))
      {
         Console.WriteLine(first);
      }
      else
      {
         Console.WriteLine(_first.Exception.Message);
      }

      testArray = [];
      _first = testArray.FirstOrFailure("Not found");
      if (_first is (true, var first2))
      {
         Console.WriteLine(first2);
      }
      else
      {
         Console.WriteLine(_first.Exception.Message);
      }
   }

   [TestMethod]
   public void IndexesOfMinMaxTest()
   {
      string[] source = ["alpha", "apples", "brat", "IP"];

      Console.WriteLine("Index of max");
      Console.WriteLine(source.IndexOfMax().Map(i => source[i]) | "empty");

      Console.WriteLine();

      Console.WriteLine("Index of max length");
      Console.WriteLine(source.IndexOfMax(s => s.Length).Map(i => source[i]) | "empty");

      Console.WriteLine();

      Console.WriteLine("Index of min");
      Console.WriteLine(source.IndexOfMin().Map(i => source[i]) | "empty");

      Console.WriteLine();

      Console.WriteLine("Index of min length");
      Console.WriteLine(source.IndexOfMin(s => s.Length).Map(i => source[i]) | "empty");
   }

   [TestMethod]
   public void MonadMinMaxTest()
   {
      string[] strings = ["foobar", "foo", "a", "bar"];
      var _max = strings.MaxOrNone();
      if (_max)
      {
         Console.WriteLine($"Max value: {_max}");
      }

      _max = strings.MaxOrNone(s => s.Length);
      if (_max)
      {
         Console.WriteLine($"Max length: {_max}");
      }
   }

   [TestMethod]
   public void AllMatchTest()
   {
      string[] left = ["foobar", "foo", "a", "bar"];
      int[] right = [6, 3, 1, 3];
      left.AllMatch(right, (s, i) => s.Length == i).Must().BeTrue().OrThrow();

      int[] right2 = [5, 3, 1, 3];
      left.AllMatch(right2, (s, i) => s.Length == i).Must().BeTrue().OrThrow();
   }

   [TestMethod]
   public void IntegerEnumerableTest1()
   {
      foreach (var i in 10.Times())
      {
         Console.Write($"{i,3}");
      }
   }

   [TestMethod]
   public void IntegerEnumerableTest2()
   {
      foreach (var i in 10.To(20))
      {
         Console.Write($"{i,3}");
      }
   }

   [TestMethod]
   public void IntegerEnumerableTest3()
   {
      foreach (var i in 20.To(10).By(2))
      {
         Console.Write($"{i,3}");
      }
   }

   [TestMethod]
   public void ByTest()
   {
      int[] numbers = [.. 1.To(10)];
      foreach (var by2 in numbers.Cluster(2))
      {
         Console.WriteLine("----------");
         foreach (var value in by2)
         {
            Console.WriteLine(value);
         }
      }
   }

   [TestMethod]
   public void SortByListTest()
   {
      ItemToSort[] array = [new("z"), new("a"), new("b"), new("c"), new("d"), new("e"), new("f"), new("y"), new("x")];
      var enumerable = array.SortByList(i => i.Key, "f", "e", "b", "a", "c", "d");
      foreach (var itemToSort in enumerable)
      {
         Console.WriteLine(itemToSort);
      }
   }

   [TestMethod]
   public void ListOrderByTest()
   {
      SortableItem[] array =
      [
         new() { Branch = "alpha", Order = "Good" }, new() { Branch = "bravo", Order = "Good" },
         new() { Branch = "charlie", Order = "Ugly" }, new() { Branch = "delta", Order = "Bad" },
         new() { Branch = "echo", Order = "Good" }, new() { Branch = "foxtrot", Order = "Ugly" }
      ];
      var sorted = array.OrderBy(i => i.Order, ["Good", "Bad", "Ugly"]);
      foreach (var sortableItem in sorted)
      {
         Console.WriteLine($"{sortableItem.Branch}: {sortableItem.Order}");
      }

      Console.WriteLine("=".Repeat(80));

      sorted = array.OrderByDescending(i => i.Order, ["Good", "Bad", "Ugly"]).ThenByDescending(i => i.Branch);
      foreach (var sortableItem in sorted)
      {
         Console.WriteLine($"{sortableItem.Branch}: {sortableItem.Order}");
      }
   }

   [TestMethod]
   public void LazyListTest()
   {
      LazyList<int> list =
      [
         .. (int[]) [1, 2, 3, 4, 5],
         111,
         153,
         .. (int[]) [555, 111, 222]
      ];
      foreach (var item in list)
      {
         Console.WriteLine(item);
      }
   }

   [TestMethod]
   public void MergeTest()
   {
      int[] left = [1, 2, 3, 4];
      string[] right = ["a", "b", "c"];

      foreach (var paired in left.Merge(right, (l, r) => l + r))
      {
         Console.WriteLine(paired);
      }
   }
}