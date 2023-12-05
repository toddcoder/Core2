using Core.Arrays;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Arrays.ArrayFunctions;

namespace Core.Tests;

[TestClass]
public class ArrayTests
{
   [TestMethod]
   public void AndifyTest()
   {
      var oneItem = array("See no evil");
      var twoItems = array("See no evil", "hear no evil");
      var threeItems = array("See no evil", "hear no evil", "speak no evil");

      Console.WriteLine(oneItem.Andify());
      Console.WriteLine(twoItems.Andify());
      Console.WriteLine(threeItems.Andify());
   }

   [TestMethod]
   public void ResizableArrayTest()
   {
      var array = new ResizableMatrix<int>(3, 3, -1, Enumerable.Range(0, 5));
      foreach (var i in array)
      {
         Console.WriteLine(i);
      }

      foreach (var tuple in array.WithCoordinates())
      {
         Console.WriteLine(tuple);
      }

      array.Resize(2, 2);

      foreach (var tuple in array.WithCoordinates())
      {
         Console.WriteLine(tuple);
      }
   }
}