using Core.Assertions;
using Core.Collections;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Tests;

[TestClass]
public class HashTest
{
   protected static void test(AutoHash<string, int> autoHash, string message, Func<string, int> newLambda)
   {
      string[] keys = ["alpha", "bravo", "charlie"];

      Console.WriteLine("-".Repeat(80));
      Console.WriteLine(message.LeftJustify(80, '-'));
      foreach (var key in keys)
      {
         Console.WriteLine($"{key}: {autoHash[key]}");
      }

      autoHash.DefaultLambda = newLambda;

      Console.WriteLine("-".Repeat(80));
      foreach (var key in keys)
      {
         Console.WriteLine($"{key}: {autoHash[key]}");
      }
   }

   protected static void test(Hash<string, int> hash, string message, int defaultValue, Func<string, int> newLambda1, bool add)
   {
      string[] keys = ["alpha", "bravo", "charlie"];

      Console.WriteLine("-".Repeat(80));
      Console.WriteLine(message.LeftJustify(80, '-'));
      foreach (var key in keys)
      {
         Console.WriteLine($"{key}: {hash.Find(key, defaultValue, add)}");
      }

      Console.WriteLine("-".Repeat(80));
      foreach (var key in keys)
      {
         Console.WriteLine($"{key}: {hash.Find(key, newLambda1, add)}");
      }
   }

   [TestMethod]
   public void AutoHashTest()
   {
      test(new AutoHash<string, int>(_ => -1), "No auto-add", _ => -2);
      test(new AutoHash<string, int>(_ => -1, true), "Auto-add", _ => -2);

      var autoHash = new AutoHash<string, int>(k => k.Length);
      autoHash.AddKeys(["alpha", "bravo"]);
      test(autoHash, "AddKeys", k => -k.Length);
   }

   [TestMethod]
   public void StringHashTest()
   {
      var hash = new StringHash<int>() + ("alpha", 0) + ("bravo", 1) + ("charlie", 2);
      hash.Must().HaveKeyOf("Bravo").OrThrow();
   }

   [TestMethod]
   public void GroupToHashTest()
   {
      var random = new Random(153);
      int[] array = [.. Enumerable.Range(0, 1000).Select(_ => random.Next(10))];
      var _hash = array.GroupToHash(i => i < 5 ? "lower" : "upper");
      if (_hash is (true, var hash))
      {
         foreach (var (key, value) in hash)
         {
            Console.WriteLine($"key {key}: {value.Length}");
         }
      }
   }

   [TestMethod]
   public void CollectionBuilderTest()
   {
      var hash = new Hash<char, int>() + ('a', 0) + ('b', 1);
      Console.Write(hash['a']);
   }

   [TestMethod]
   public void OneShotHashTest()
   {
      OneShotHash<char, int> hash = [];
      hash['a'] = 0;
      hash['b'] = 1;
      hash['a'] = 1;

      foreach (var (key, value) in hash)
      {
         Console.WriteLine($"{key}: {value}");
      }
   }

   [TestMethod]
   public void TwoValueHashTest()
   {
      var hash = new TwoValueHash<string, string, bool>(_ => "", _ => false);
      hash.SetValue1("alfa", "A");
      hash.SetValue1("bravo", "B");
      hash["charlie"] = ("C", true);

      foreach (var (key, (value1, value2)) in hash)
      {
         Console.WriteLine($"{key}: ({value1}, {value2})");
      }
   }

   [TestMethod]
   public void BackHashTest()
   {
      var stringHash = new StringHash() + ("a", "alfa") + ("b", "bravo") + ("c", "charlie");
      var backHash = stringHash.ToBackHash();

      if (backHash.Maybe["a"] is (true, var value1))
      {
         Console.WriteLine(value1);
      }

      if (backHash.Back.Maybe["bravo"] is (true, var value2))
      {
         Console.WriteLine(value2);
      }
   }

   [TestMethod]
   public void MemoizationTest()
   {
      Func<string, int> memoizationFunc = key =>
      {
         Console.WriteLine($"key:{key}");
         return key.Length;
      };

      var hash = new Hash<string, int>();
      write(hash.Memoize("alpha", memoizationFunc));
      write(hash.Memoize("zulu", memoizationFunc));
      write(hash.Memoize("charlie", memoizationFunc));
      write(hash.Memoize("alpha", memoizationFunc));
      write(hash.Memoize("zulu", memoizationFunc));
      write(hash.Memoize("romeo", memoizationFunc));

      return;

      void write(int value) => Console.WriteLine(value);
   }
}