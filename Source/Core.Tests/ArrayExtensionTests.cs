using Core.Enumerables;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class ArrayExtensionTests
{
   [TestMethod]
   public void AndifyTest()
   {
      string[] oneItem = ["See no evil"];
      string[] twoItems = ["See no evil", "hear no evil"];
      string[] threeItems = ["See no evil", "hear no evil", "speak no evil"];

      Console.WriteLine(oneItem.Andify());
      Console.WriteLine(twoItems.Andify());
      Console.WriteLine(threeItems.Andify());
   }
}