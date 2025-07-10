using Core.Assertions;
using Core.Collections;
using Core.Enumerables;
using Core.Strings;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Strings.StringFunctions;

namespace Core.Tests;

[TestClass]
public class HashTest
{
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

   protected static int memoizationFunc(string key)
   {
      Console.WriteLine($"key:{key}");
      return key.Length;
   }

   protected static void write(int value) => Console.WriteLine(value);

   [TestMethod]
   public void MemoTest()
   {
      var memo = new Memo<string, int>.Function(memoizationFunc);
      write(memo["alpha"]);
      write(memo["zulu"]);
      write(memo["charlie"]);
      write(memo["alpha"]);
      write(memo["zulu"]);
      write(memo["romeo"]);
   }

   [TestMethod]
   public void MemoListTest()
   {
      var memo = new Memo<string, List<string>>.Function(_ => []);

      memo["alpha"].AddRange(["able", "apple"]);
      memo["bravo"].Add("baker");
      _ = memo["charlie"];

      foreach (var (key, list) in memo)
      {
         writeList(key, list);
      }

      return;

      void writeList(string key, List<string> list) => Console.WriteLine($"{key}: ({list.Select(i => $"'{i}'").ToString(", ")})");
   }

   [TestMethod]
   public void MemoCountTest()
   {
      var memo = new Memo<string, int>.Value(0);
      string[] keys = ["alpha", "bravo", "charlie", "bravo", "alpha", "alpha", "delta", "delta"];
      foreach (var key in keys)
      {
         memo[key]++;
      }

      foreach (var (key, count) in memo)
      {
         writeCount(key, count);
      }

      return;

      void writeCount(string key, int count) => Console.WriteLine($"{key}: ({count})");
   }

   [TestMethod]
   public void MemoInitializationTest()
   {
      var memo = new Memo<string, string>.Function(k => k.Reverse())
      {
         ["alpha"] = "A"
      };
      writeValue("alpha", memo["alpha"]);
      writeValue("bravo", memo["bravo"]);
      memo["charlie"] = "C";
      writeValue("charlie", memo["charlie"]);

      return;

      void writeValue(string key, string value) => Console.WriteLine($"{key}: ({value})");
   }

   [TestMethod]
   public void MemoFunction2Test()
   {
      var memo = new StateMemo<(string text, int multiplier), string, int>(s => s.text, s => s.text.Length * s.multiplier);

      Console.WriteLine(memo[("alfa", 10)]);
      Console.WriteLine(memo[("bravo", 10)]);
      Console.WriteLine(memo[("charlie", 20)]);
      Console.WriteLine(memo[("delta", 20)]);
   }

   [TestMethod]
   public void LazyMemoTest()
   {
      var memo = new LazyMemo<string, int>
      {
         DefaultValue = key => key.Length
      };

      Console.WriteLine(memo["alfa"]);
      Console.WriteLine(memo["bravo"]);
      Console.WriteLine(memo["charlie"]);
      Console.WriteLine(memo["delta"]);
   }

   protected class State
   {
      public Guid Id { get; set; } = Guid.NewGuid();

      public string Name { get; set; } = uniqueIDFromTime();

      public override string ToString() => $"{Id}: {Name}";
   }

   [TestMethod]
   public void StateMemoTest()
   {
      var memo = new StateMemo<State, Guid, string>(s => s.Id, _ => uniqueIDFromTime());

      var state = new State();
      memo[state] = state.Name;
      Console.WriteLine(state);

      state = new State();
      Console.WriteLine(memo[state]);

      foreach (var item in memo)
      {
         Console.WriteLine($"{item.Key}: {item.Value}");
      }
   }
}