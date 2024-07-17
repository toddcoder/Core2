using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Core.Applications;
using Core.Assertions;
using Core.Collections;
using Core.Computers;
using Core.Configurations;
using Core.DataStructures;
using Core.Dates.DateIncrements;
using Core.Enumerables;
using Core.Json;
using Core.Json.Building;
using Core.Monads;
using Core.Monads.Lazy;
using Core.Strings;
using Core.Strings.Text;
using Core.WinForms;
using static Core.Monads.MonadFunctions;
using static Core.Strings.StringFunctions;

namespace Core.Tests;

[TestClass]
public class SettingTests
{
   protected enum TestEnum
   {
      Alpha,
      Bravo,
      Charlie
   }

   protected class Test
   {
      public Test()
      {
         StringValue = "";
         File = nil;
         Doubles = [];
         Escape = "";
      }

      public TestEnum Enum { get; set; }

      public int IntValue { get; set; }

      public string StringValue { get; set; }

      public Maybe<FileName> File { get; set; }

      public double[] Doubles { get; set; }

      public bool IsTrue { get; set; }

      public string Escape { get; set; }

      public override string ToString()
      {
         return $"{Enum}; {IntValue}; {StringValue}; {File | ""}; {Doubles.Select(d => d.ToString()).ToString(", ")}; {IsTrue}; {Escape}";
      }
   }

   protected class Container
   {
      public Container()
      {
         Tests = [];
      }

      public Test[] Tests { get; set; }
   }

   protected class ReleaseTarget
   {
      public string[] ReleaseTargets { get; set; } = [];
   }

   protected class BinaryPackage : IEquatable<BinaryPackage>
   {
      public byte[] Payload { get; set; } = [];

      public bool Equals(BinaryPackage? other)
      {
         return other is not null && (ReferenceEquals(this, other) || Payload.Zip(other.Payload, (b1, b2) => b1 == b2).All(b => b));
      }

      public override bool Equals(object? obj) => obj is BinaryPackage binaryPackage && Equals(binaryPackage);

      public override int GetHashCode() => Payload.GetHashCode();
   }

   protected Maybe<(string server, string database)> getServerDatabase(Setting setting)
   {
      return
         from connections in setting.Maybe.Setting("connections")
         from connection1 in connections.Maybe.Setting("connection1")
         from server in connection1.Maybe.String("server")
         from database in connection1.Maybe.String("database")
         select (server, database);
   }

   [TestMethod]
   public void BasicTest()
   {
      var resources = new Resources<SettingTests>();
      var source = resources.String("TestData.connections.txt");

      var _setting = new LazyResult<Setting>(() => Setting.FromString(source));
      var _serverDatabase = _setting.Then(setting => getServerDatabase(setting).Result("Failed"));

      if (_serverDatabase is (true, var (server, database)))
      {
         Console.WriteLine($"server: {server}");
         Console.WriteLine($"database: {database}");
      }
   }

   [TestMethod]
   public void FlatTest()
   {
      var resources = new Resources<SettingTests>();
      var source = resources.String("TestData.connections2.txt");

      var _setting = new LazyResult<Setting>(() => Setting.FromString(source));
      var _serverDatabase = _setting.Then(setting => getServerDatabase(setting).Result("Failed"));

      if (_serverDatabase is (true, var (server, database)))
      {
         Console.WriteLine($"server: {server}");
         Console.WriteLine($"database: {database}");
      }
   }

   [TestMethod]
   public void MultilineArrayTest()
   {
      var resources = new Resources<SettingTests>();
      var source = resources.String("TestData.Arrays.txt");
      var _setting = Setting.FromString(source);
      if (_setting is (true, var setting))
      {
         Console.WriteLine(setting);
      }
      else
      {
         Console.WriteLine(_setting.Exception.Message);
      }
   }

   [TestMethod]
   public void ToStringTest()
   {
      var resources = new Resources<SettingTests>();
      var source = resources.String("TestData.connections.txt");
      var _setting = Setting.FromString(source);
      if (_setting is (true, var setting))
      {
         Console.Write(setting);
      }
      else
      {
         Console.WriteLine(_setting.Exception.Message);
      }
   }

   [TestMethod]
   public void SerializationTest()
   {
      var test = new Test
      {
         Enum = TestEnum.Bravo,
         IntValue = 153,
         StringValue = "foobar",
         File = (FileName)@"C:\temp\temp.txt",
         Doubles = [1.0, 5.0, 3.0],
         IsTrue = true,
         Escape = "\r \t \\ foobar"
      };
      var _setting = Setting.Serialize(test, true, "test");
      if (_setting is (true, var setting))
      {
         Console.Write(setting);
      }
      else
      {
         Console.WriteLine(_setting.Exception.Message);
      }
   }

   [TestMethod]
   public void BinarySerializationTest()
   {
      var resources = new Resources<SettingTests>();
      var binary = resources.Bytes("TestData.guids.pdf");
      var package = new BinaryPackage { Payload = binary };
      var _newPackage =
         from setting in Setting.Serialize(package, true, "guids").OnSuccess(Console.WriteLine)
         from binaryPackage in setting.Deserialize<BinaryPackage>()
         select binaryPackage;
      if (_newPackage is (true, var newPackage))
      {
         package.Must().Equal(newPackage).OrThrow();
      }
      else
      {
         Console.WriteLine(_newPackage.Exception.Message);
      }
   }

   [TestMethod]
   public void DeserializationTest()
   {
      var source = @"enum: Bravo; intValue: 153; stringValue: foobar; file: C:\temp\temp.txt; doubles: 1.0, 5.0, 3.0; isTrue: true; " +
         @"escape: ""`r `t \ foobar""";
      var _object =
         from setting in Setting.FromString(source)
         from obj in setting.Deserialize<object>()
         select obj;
      if (_object is (true, var @object))
      {
         Console.WriteLine(@object);
      }
      else
      {
         Console.WriteLine(_object.Exception.Message);
      }
   }

   [TestMethod]
   public void ComplexSerializationDeserializationTest()
   {
      FileName file = @"C:\Temp\temp.txt";

      var container = new Container
      {
         Tests =
         [
            new Test
            {
               Enum = TestEnum.Alpha, Doubles = [1, 2, 3], Escape = "`1", File = file, IntValue = 123, IsTrue = true,
               StringValue = "foo"
            },
            new Test
            {
               Enum = TestEnum.Bravo, Doubles = [1.0, 5, 3], Escape = "`2", File = file, IntValue = 153, IsTrue = false,
               StringValue = "bar"
            }
         ]
      };

      var _container =
         from setting in Setting.Serialize(container, true, "data").OnSuccess(Console.WriteLine)
         from deserializedContainer in setting.Deserialize<Container>()
         select deserializedContainer;
      if (_container is (true, var containerValue))
      {
         foreach (var test in containerValue.Tests)
         {
            Console.WriteLine(test);
         }
      }
      else
      {
         Console.WriteLine(_container.Exception.Message);
      }
   }

   [TestMethod]
   public void HashToConfigurationTest()
   {
      StringHash hash = new()
      {
         ["alpha"] = "Alpha",
         ["bravo"] = "Beta",
         ["charlie"] = "Kappa",
         ["delta"] = "Delta",
         ["echo"] = "Eta",
         ["foxtrot"] = "Phi"
      };
      var _setting = hash.ToSetting();
      if (_setting is (true, var setting))
      {
         Console.WriteLine(setting);
      }
      else
      {
         Console.WriteLine($"Exception: {_setting.Exception.Message}");
      }
   }

   [TestMethod]
   public void SerializeStringHashTest()
   {
      StringHash hash = new()
      {
         ["alpha"] = "Alpha",
         ["bravo"] = "Beta",
         ["charlie"] = "Kappa",
         ["delta"] = "Delta",
         ["echo"] = "Eta",
         ["foxtrot"] = "Phi"
      };
      var _stringHash =
         from setting in hash.ToSetting()
         let file = (FileName)$@"C:\Temp\{uniqueID()}.txt"
         from _ in file.TryTo.SetText(setting.ToString())
         from source in file.TryTo.Text
         from setting2 in Setting.FromString(source)
         select setting.ToStringHash();
      if (_stringHash is (true, var stringHash))
      {
         foreach (var (key, value) in stringHash)
         {
            Console.WriteLine($"{key}: {value}");
         }
      }
      else
      {
         Console.WriteLine($"Exception: {_stringHash.Exception.Message}");
      }
   }

   [TestMethod]
   public void EmptyStringItemTest()
   {
      StringHash hash = new() { ["release"] = "", ["build"] = "http" };
      var _setting = hash.ToSetting();
      if (_setting is (true, var setting))
      {
         var source = setting.ToString();
         Console.WriteLine(source);
         Console.WriteLine(setting["release"]);
         Console.WriteLine(setting["build"]);

         _setting = Setting.FromString(source);
         if (_setting is (true, var setting2))
         {
            Console.WriteLine(setting2["release"]);
            Console.WriteLine(setting2["build"]);
         }
         else
         {
            Console.WriteLine($"Exception: {_setting.Exception.Message}");
         }
      }
      else
      {
         Console.WriteLine($"Exception: {_setting.Exception.Message}");
      }
   }

   [TestMethod]
   public void WritingTest()
   {
      var setting = new Setting
      {
         ["repository"] = @"\\vmdvw10estm57",
         ["server"] = ".",
         ["database"] = "local_tebennett"
      };

      Console.WriteLine(setting);
   }

   [TestMethod]
   public void ArrayTest()
   {
      using var writer = new StringWriter();
      writer.WriteLine("[");
      writer.WriteLine("   value1: 111");
      writer.WriteLine("   value2: \"$1\"");
      writer.WriteLine("]");
      writer.WriteLine("[");
      writer.WriteLine("   value1: 123");
      writer.WriteLine("   value2: \"$2\"");
      writer.WriteLine("]");
      writer.WriteLine("[");
      writer.WriteLine("   value1: 153");
      writer.WriteLine("   value2: \"$3\"");
      writer.WriteLine("]");
      var source = writer.ToString();
      var _setting = Setting.FromString(source);
      if (_setting is (true, var setting))
      {
         foreach (var (key, innerSetting) in setting.Settings())
         {
            Console.WriteLine($"{key} [");
            Console.WriteLine($"   value1: {innerSetting.Value.String("value1")}");
            Console.WriteLine($"   value2: {innerSetting.Value.String("value2")}");
            Console.WriteLine("]");
         }

         Console.WriteLine("=".Repeat(80));

         Console.WriteLine(setting);
      }
      else
      {
         throw _setting.Exception;
      }

      var builder = JsonBuilder.WithObject();
      _ = builder.Object("o1") + ("value1", 111) + ("value2", "$1") + nil;
      _ = builder.Object("o2") + ("value1", 123) + ("value2", "$2") + nil;
      _ = builder.Object("o3") + ("value1", 153) + ("value2", "$3") + nil;
      Console.WriteLine(builder);
   }

   [TestMethod]
   public void AnonymousTest()
   {
      using var writer = new StringWriter();
      writer.WriteLine("[");
      writer.WriteLine("   alpha");
      writer.WriteLine("   bravo");
      writer.WriteLine("   charlie");
      writer.WriteLine("]");
      var source = writer.ToString();

      if (Setting.FromString(source) is (true, var setting))
      {
         var _innerSetting = setting.Settings().FirstOrFailure("No outer group");
         if (_innerSetting is (true, var (_, innerSetting)))
         {
            foreach (var (key, value) in innerSetting.Items())
            {
               Console.WriteLine($"{key}: \"{value}\"");
            }
         }
      }

      var builder = JsonBuilder.WithObject();
      _ = builder.Array("items") + ["alpha", "bravo", "charlie"];
      Console.WriteLine(builder);
   }

   [TestMethod]
   public void AnonymousTest2()
   {
      using var writer = new StringWriter();
      writer.WriteLine("alpha");
      writer.WriteLine("bravo");
      writer.WriteLine("charlie");
      var source = writer.ToString();

      if (Setting.FromString(source) is (true, var setting))
      {
         foreach (var (key, value) in setting.Items())
         {
            Console.WriteLine($"{key}: \"{value}\"");
         }
      }
   }

   [TestMethod]
   public void QuoteTest()
   {
      using var writer = new StringWriter();
      writer.WriteLine("[");
      writer.WriteLine("""
                       [
                          bravo: "^(Enqueuing task `"\[)[^\]]+(\]`").+$; u"
                       ]
                       """);
      var source = writer.ToString();
      _ = (Setting)Setting.FromString(source);
   }

   [TestMethod]
   public void MultiLineArraySavingTest()
   {
      using var writer = new StringWriter();
      writer.WriteLine("releaseTargets: {");
      writer.WriteLine("   Monthly - 6.34 - March");
      writer.WriteLine("   Monthly - 6.35 - April");
      writer.WriteLine("}");
      var source = writer.ToString();

      var _setting = Setting.FromString(source);
      if (_setting is (true, var setting))
      {
         Console.WriteLine(setting);
      }
      else
      {
         Console.WriteLine(_setting.Exception.Message);
         return;
      }

      LazyResult<ReleaseTarget> _releaseTarget = nil;
      LazyResult<Setting> _serialized = nil;
      if (_releaseTarget.ValueOf(setting.Deserialize<ReleaseTarget>()) is (true, var releaseTarget))
      {
         if (_serialized.ValueOf(Setting.Serialize(typeof(ReleaseTarget), releaseTarget, true)) is (true, var serialized))
         {
            Console.WriteLine(serialized);
         }
         else
         {
            Console.WriteLine(_serialized.Exception.Message);
         }
      }
      else
      {
         Console.WriteLine(_releaseTarget.Exception);
      }
   }

   protected class NonConformanceInfo
   {
      public NonConformanceInfo()
      {
         Message = "";
         Rule = "";
      }

      public int Index { get; set; }

      public int Length { get; set; }

      public int Line { get; set; }

      public int Column { get; set; }

      public int EndLine { get; set; }

      public int EndColumn { get; set; }

      public string Message { get; set; } = "";

      public string Rule { get; set; } = "";

      public override string ToString() => $"{Message}: {Rule} ({Index}, {Length})";
   }

   protected class NonConformanceInfoContainer
   {
      public NonConformanceInfoContainer()
      {
         NonConformanceInfos = [];
      }

      public NonConformanceInfo[] NonConformanceInfos { get; set; }
   }

   [TestMethod]
   public void Bug1Test()
   {
      var resources = new Resources<SettingTests>();
      var source = resources.String("usesForeignKey.txt");
      LazyResult<Setting> _setting = nil;
      LazyResult<NonConformanceInfoContainer> _container = nil;
      if (_setting.ValueOf(Setting.FromString(source)) is (true, var setting))
      {
         Console.WriteLine(setting);
         if (_container.ValueOf(setting.Deserialize<NonConformanceInfoContainer>()) is (true, var container))
         {
            foreach (var info in container.NonConformanceInfos)
            {
               Console.WriteLine(info);
            }
         }
      }
   }

   [TestMethod]
   public void SerializationToJsonTest()
   {
      var resources = new Resources<SettingTests>();
      var json = resources.String("testSetting.json");
      var deserializer = new Deserializer(json);
      var _setting = deserializer.Deserialize();
      if (_setting)
      {
         var serializer = new Serializer(_setting);
         var _json = serializer.Serialize();
         if (_json is (true, var jsonValue))
         {
            Console.WriteLine();
            FileName tempFile = @"C:\Temp\testSetting.json";
            tempFile.Text = _json;

            var oldLines = json.Lines();
            var newLines = jsonValue.Lines();
            var diff = new Differentiator(oldLines, newLines, true, false);
            var _model = diff.BuildModel();
            if (_model is (true, var model))
            {
               var oldDifferences = model.OldDifferences();
               var newDifferences = model.NewDifferences();
               var mergedDifferences = model.MergedDifferences();

               Console.WriteLine("old differences:");
               foreach (var difference in oldDifferences)
               {
                  Console.WriteLine(difference);
               }

               Console.WriteLine();

               Console.WriteLine("new differences:");
               foreach (var difference in newDifferences)
               {
                  Console.WriteLine(difference);
               }

               Console.WriteLine();

               Console.WriteLine("merged differences:");
               foreach (var difference in mergedDifferences)
               {
                  Console.WriteLine(difference);
               }
            }
         }
         else
         {
            throw _json.Exception;
         }
      }
      else
      {
         throw _setting.Exception;
      }
   }

   [TestMethod]
   public void SetTest()
   {
      var setting = new Setting();
      setting.Set("index").Int32 = 153;
      setting.Set("name").String = "foobar";
      setting.Set("now").DateTime = DateTime.Now;
      setting.Set("data").StringHash = new StringHash() { ["alpha"] = "a", ["bravo"] = "b", ["charlie"] = "c" };

      Console.Write(setting.ToString());

      var serializer = new Serializer(setting);
      if (serializer.Serialize() is (true, var json))
      {
         Console.WriteLine(json);

         var deserializer = new Deserializer(json);
         var _setting = deserializer.Deserialize();
         if (_setting is (true, var newSetting))
         {
            var hash = newSetting.Value.StringHash("data");
            foreach (var (hKey, hValue) in hash)
            {
               Console.WriteLine($"{hKey}: {hValue}");
            }
         }
      }

      var builder = JsonBuilder.WithObject();
      _ = builder.Outer + ("index", 153) + ("name", "foobar") + ("now", DateTime.Now);
      _ = builder.Outer.Object("data") + ("alpha", "a") + ("bravo", "b") + ("charlie", "c");
      Console.WriteLine(builder);
   }

   [TestMethod]
   public void NestedSettingsTest()
   {
      var setting = new Setting();
      string[] keys = ["alfa", "bravo", "charlie", "delta", "echo", "foxtrot"];
      MaybeQueue<string> keysQueue = [.. keys];
      string[] values1 = ["a", "b", "c", "d", "e", "f"];
      MaybeQueue<string> values1Queue = [.. values1];
      string[] values2 = ["alpha", "beta", "kappa", "delta", "eta", "phi"];
      MaybeQueue<string> values2Queue = [.. values2];

      while (keysQueue.Dequeue() is (true, var key) && values1Queue.Dequeue() is (true, var value1) && values2Queue.Dequeue() is (true, var value2))
      {
         var subSetting = new Setting(key);
         setting.Set(key).Setting = subSetting;

         subSetting.Set("value1").String = value1;
         subSetting.Set("value2").String = value2;
      }

      if (Serializer.Serialize(setting) is (true, var json))
      {
         Console.WriteLine(json);
      }
   }

   [TestMethod]
   public void NestedBuilderSettingsTest()
   {
      var builder = JsonBuilder.WithObject();
      string[] keys = ["alfa", "bravo", "charlie", "delta", "echo", "foxtrot"];
      MaybeQueue<string> keysQueue = [.. keys];
      string[] values1 = ["a", "b", "c", "d", "e", "f"];
      MaybeQueue<string> values1Queue = [.. values1];
      string[] values2 = ["alpha", "beta", "kappa", "delta", "eta", "phi"];
      MaybeQueue<string> values2Queue = [.. values2];

      while (keysQueue.Dequeue() is (true, var key) && values1Queue.Dequeue() is (true, var value1) && values2Queue.Dequeue() is (true, var value2))
      {
         var obj = builder.Object(key);
         _ = obj + ("value1", value1) + ("value2", value2) + nil;
      }

      Console.WriteLine(builder);
   }

   [TestMethod]
   public void Array2Test()
   {
      var setting = new Setting();
      setting.Set("array").Setting = new Setting { Array = ["alpha", "bravo", "charlie"] };

      var serializer = new Serializer(setting);
      var _json = serializer.Serialize();
      if (_json is (true, var json))
      {
         Console.WriteLine(json);
      }

      var builder = JsonBuilder.WithObject();
      _ = builder.Array(Setting.ROOT_NAME) + ["alpha", "bravo", "charlie"];
      Console.WriteLine(builder);
   }

   [TestMethod]
   public void EmptyArrayTest()
   {
      var setting = new Setting();
      setting.Set("array").Setting = new Setting { Array = [] };

      var serializer = new Serializer(setting);
      var _json = serializer.Serialize();
      if (_json is (true, var json))
      {
         Console.WriteLine(json);
      }
   }

   [TestMethod]
   public void Array3Test()
   {
      var setting = new Setting();
      setting.Set("array").Array = ["alfa", "bravo", "charlie"];

      var serializer = new Serializer(setting);
      var _json = serializer.Serialize();
      if (_json is (true, var json))
      {
         Console.WriteLine(json);
      }
      else
      {
         Console.WriteLine($"Exception: {_json.Exception.Message}");
      }

      var builder = JsonBuilder.WithObject();
      _ = builder.Array("array") + ["alfa", "bravo", "charlie"];
      Console.WriteLine(builder);
   }

   protected class TestClass
   {
      public string Name { get; set; } = "";

      public string Letter { get; set; } = "";

      public int Number { get; set; }
   }

   [TestMethod]
   public void Array4Test()
   {
      TestClass[] testClasses =
      [
         new TestClass { Name = "alfa", Letter = "a", Number = 0 },
         new TestClass { Name = "bravo", Letter = "b", Number = 1 },
         new TestClass { Name = "charlie", Letter = "c", Number = 2 }
      ];
      var _json =
         from setting in testClasses.ToSetting(tc => tc.Name, "foobar")
         from serialized in Serializer.Serialize(setting)
         select serialized;
      if (_json is (true, var json))
      {
         Console.WriteLine(json);
      }
      else
      {
         Console.WriteLine(_json.Exception.Message);
      }

      var builder = JsonBuilder.WithObject();
      foreach (var testClass in testClasses)
      {
         _ = builder.Outer.Object(testClass.Name) + ("name", testClass.Name) + ("letter", testClass.Letter) + ("number", testClass.Number) + nil;
      }

      Console.WriteLine(builder);
   }

   protected Setting getTestClassesSetting()
   {
      TestClass[] testClasses =
      [
         new TestClass { Name = "alfa", Letter = "a", Number = 0 },
         new TestClass { Name = "bravo", Letter = "b", Number = 1 },
         new TestClass { Name = "charlie", Letter = "c", Number = 2 },
         new TestClass { Name = "delta", Letter = "d", Number = 3 },
         new TestClass { Name = "echo", Letter = "e", Number = 4 },
         new TestClass { Name = "foxtrot", Letter = "f", Number = 5 }
      ];

      var setting = new Setting();
      var _subSetting = testClasses.ToSetting(tc => tc.Letter, "nato");
      if (_subSetting is (true, var subSetting))
      {
         setting.Set(subSetting.Key).Setting = subSetting;
      }

      return setting;
   }

   [TestMethod]
   public void Array5Test()
   {
      var setting = getTestClassesSetting();

      var _json = Serializer.Serialize(setting);
      if (_json is (true, var json))
      {
         Console.WriteLine(json);
         Console.WriteLine();
         Console.WriteLine(setting);
      }
      else
      {
         Console.WriteLine(_json.Exception.Message);
      }
   }

   [TestMethod]
   public void SettingPathTest()
   {
      var setting = getTestClassesSetting();

      Console.WriteLine("All names");
      foreach (var item in setting.SelectItems("nato..name"))
      {
         Console.WriteLine(item.Text);
      }

      Console.WriteLine();

      Console.WriteLine("First name");
      foreach (var item in setting.SelectItems("nato..name[1]"))
      {
         Console.WriteLine(item.Text);
      }

      Console.WriteLine();

      Console.WriteLine("Skip 2, take 3");
      foreach (var item in setting.SelectItems("nato..letter(2:3)"))
      {
         Console.Write(item.Text);
      }

      Console.WriteLine();
      Console.WriteLine();

      Console.WriteLine("Text ends in o");
      foreach (var item in setting.SelectItems("nato..t/'o' $; f/"))
      {
         Console.WriteLine($"{item.Key}: {item.Text}");
      }
   }

   [TestMethod]
   public void SettingPathFromJsonTest()
   {
      using var writer = new JsonWriter();
      writer.BeginObject();
      writer.Write("count", 1);

      writer.BeginArray("value");
      writer.BeginObject();
      writer.Write("id", 112449);

      writer.BeginObject("project");
      writer.Write("id", Guid.NewGuid());
      writer.Write("name", "Foobar");
      writer.EndObject();

      var now = DateTime.Now;
      writer.Write("startedDate", now - 45.Minutes());
      writer.Write("completedDate", now);

      writer.BeginObject("testCase");
      writer.Write("name", "Bad");
      writer.EndObject();

      writer.EndObject();
      writer.EndArray();

      writer.EndObject();

      var json = writer.ToString();

      Console.WriteLine(json);
      Console.WriteLine();

      var _setting = Deserializer.Deserialize(json);
      if (_setting is (true, var setting))
      {
         foreach (var item in setting.SelectItems("value..testCase.name"))
         {
            Console.WriteLine(item.Text);
         }

         if (setting.SelectItems("value..startedDate").FirstOrNone() is (true, var firstItem))
         {
            Console.WriteLine($"{firstItem.Key}: {firstItem.Text}");
         }
      }
   }

   [TestMethod]
   public void SettingExtensionsTest()
   {
      var setting = new Setting();

      var point = new Point(10, 20);
      setting.Set("location").Point(point);

      var rectangle = new Rectangle(111, 123, 153, 200);
      setting.Set("bounds").Rectangle(rectangle);

      var _json = Serializer.Serialize(setting);
      if (_json is (true, var json))
      {
         Console.WriteLine(json);
         var _setting2 = Deserializer.Deserialize(json);
         if (_setting2 is (true, var setting2))
         {
            var location = setting2.Value.Point("location");
            Console.WriteLine($"location: {location}");
            var bounds = setting2.Value.Rectangle("bounds");
            Console.WriteLine($"bounds: {bounds}");
         }
      }
      else
      {
         Console.WriteLine(_json.Exception.Message);
      }
   }
}