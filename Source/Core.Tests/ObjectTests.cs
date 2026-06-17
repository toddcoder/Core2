using Core.Objects;
using static Core.Lambdas.LambdaFunctions;
using static Core.Objects.GetHashCodeGenerator;

namespace Core.Tests;

[TestClass]
public class ObjectTests
{
   [TestMethod]
   public void GetHashCodeTest()
   {
      int hash = hashCode() + 153 + "foobar" + true;
      Console.WriteLine(hash);
      int hash2 = hashCode() + 153 + "foobar" + true;
      Assert.AreEqual(hash, hash2);
      hash2 = hashCode() + 154 + "foobaz" + false;
      Console.WriteLine(hash2);
   }

   [TestMethod]
   public void LateLazyTest()
   {
      var i = new LateLazy<int>();
      i.ActivateWith(() => 153);
      Console.WriteLine(i * 2);

      i.Reset();
      Console.WriteLine(i * 10 + 4);
   }

   [TestMethod]
   public void CachedValueTest()
   {
      /*CachedValue<int> cachedValue = (Func<int>)(() =>
      {
         Console.WriteLine("Creating");
         return 153;
      });

      var x = cachedValue;
      var y = cachedValue;

      Console.WriteLine(x + y);*/
      var cachedValue = new CachedValue<int>(() =>
      {
         Console.WriteLine("Creating");
         return 153;
      });
      var x = cachedValue.Value;
      var y = cachedValue.Value;

      Console.WriteLine(x + y);
   }
}