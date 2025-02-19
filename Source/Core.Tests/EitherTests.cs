using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Tests;

[TestClass]
public class EitherTests
{
   [TestMethod]
   public void CreatingTest()
   {
      Either<char, string> _left = 'a';
      Either<char, string> _right = "a";
      var (_char, _) = _left;
      if (_char)
      {
         Console.WriteLine($"char {_char}");
      }

      var (_, _string) = _right;
      if (_string)
      {
         Console.WriteLine($"string {_string}");
      }
   }

   [TestMethod]
   public void MappingTest()
   {
      Either<int, double> _left = 10;
      var dLeft = _left.Map(i => (double)i, d => (int)d);
      var (_double, _int) = dLeft;
      if (_double is (true, var @double))
      {
         Console.WriteLine($"double {@double} is good");
      }
      else if (_int is (true, var @int))
      {
         Console.WriteLine($"int {@int} is good");
      }

      Either<int, double> _right = 7.0;
      var iRight = _right.Map(i => i / 2.0, d => (int)d / 2);
      var (_double2, _int2) = iRight;
      if (_double2 is (true, var double2))
      {
         Console.WriteLine($"double {double2} is good");
      }
      else if (_int2 is (true, var int2))
      {
         Console.WriteLine($"int {int2} is good");
      }
   }

   [TestMethod]
   public void DefaultTest()
   {
      var left = 'a'.Either<int, char>() | 'a';
      Console.WriteLine($"{left}: {left.GetType().Name}");
   }

   [TestMethod]
   public void ImplicitTest()
   {
      Either<int, char> either = 'a';
      Console.WriteLine(either);

      either = 10;
      Console.WriteLine(either);
   }

   [TestMethod]
   public void NilTest()
   {
      Either<int, char> either = nil;
      switch (either)
      {
         case ((true, var asInt), _):
            Console.WriteLine($"{nameof(asInt)}: {asInt}");
            break;
         case (_, (true, var asChar)):
            Console.WriteLine($"{nameof(asChar)}: {asChar}");
            break;
         default:
            Console.WriteLine($"{nameof(either)}: {either}");
            break;
      }
   }
}