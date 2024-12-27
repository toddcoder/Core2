﻿using Core.Applications;
using Core.Applications.CommandProcessing;
using Core.Applications.ConsoleProcessing;
using Core.Collections;
using Core.Computers;
using Core.Enumerables;
using Core.Matching;
using Core.Monads;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Core.Monads.MonadFunctions;

namespace Core.Tests;

internal class Parameters
{
   public Parameters()
   {
      Recursive = true;
      Text = string.Empty;
      AttributeTargets = AttributeTargets.All;
      Array = [];
   }

   public bool Push { get; set; }

   public bool Pull { get; set; }

   public bool Show { get; set; }

   public int Code { get; set; }

   public double Amount { get; set; }

   public AttributeTargets AttributeTargets { get; set; }

   public string Text { get; set; }

   public bool Recursive { get; set; }

   public string[] Array { get; set; }
}

internal class Program : CommandLineInterface
{
   [EntryPoint(EntryPointType.Parameters)]
   public void Main(bool push = false, bool pull = false, bool show = false, int code = 0, double amount = 0,
      AttributeTargets attributeTargets = AttributeTargets.All, string text = "", bool recursive = true, string[]? array = null)
   {
      array ??= [];

      var command = "?";
      if (pull)
      {
         command = "pull";
      }
      else if (push)
      {
         command = "push";
      }
      else if (show)
      {
         command = "show";
      }

      Console.WriteLine($"command: '{command}'");
      Console.WriteLine($"code: {code}");
      Console.WriteLine($"amount: {amount}");
      Console.WriteLine($"attribute targets: {attributeTargets}");
      Console.WriteLine($"text: '{text}'");
      Console.WriteLine($"recursive: {recursive.ToString().ToLower()}");
      Console.WriteLine($"array: {array.ToString(", ")}");
   }
}

internal class ObjectProgram : CommandLineInterface
{
   [EntryPoint(EntryPointType.Object)]
   public void Main(Parameters parameters)
   {
      var command = "?";
      if (parameters.Pull)
      {
         command = "pull";
      }
      else if (parameters.Push)
      {
         command = "push";
      }
      else if (parameters.Show)
      {
         command = "show";
      }

      Console.WriteLine($"command: '{command}'");
      Console.WriteLine($"code: {parameters.Code}");
      Console.WriteLine($"amount: {parameters.Amount}");
      Console.WriteLine($"attribute targets: {parameters.AttributeTargets}");
      Console.WriteLine($"text: '{parameters.Text}'");
      Console.WriteLine($"recursive: {parameters.Recursive.ToString().ToLower()}");
      Console.WriteLine($"array: {parameters.Array.ToString(", ")}");
   }
}

internal class ThisProgram : CommandLineInterface
{
   public ThisProgram()
   {
      Recursive = true;
      Text = string.Empty;
      AttributeTargets = AttributeTargets.All;
      Array = [];
   }

   public bool Push { get; set; }

   public bool Pull { get; set; }

   public bool Show { get; set; }

   public int Code { get; set; }

   public double Amount { get; set; }

   public AttributeTargets AttributeTargets { get; set; }

   public string Text { get; set; }

   public bool Recursive { get; set; }

   public string[] Array { get; set; }

   [EntryPoint(EntryPointType.This)]
   public void Main()
   {
      var command = "?";
      if (Pull)
      {
         command = "pull";
      }
      else if (Push)
      {
         command = "push";
      }
      else if (Show)
      {
         command = "show";
      }

      Console.WriteLine($"command: '{command}'");
      Console.WriteLine($"code: {Code}");
      Console.WriteLine($"amount: {Amount}");
      Console.WriteLine($"attribute targets: {AttributeTargets}");
      Console.WriteLine($"text: '{Text}'");
      Console.WriteLine($"recursive: {Recursive.ToString().ToLower()}");
      Console.WriteLine($"array: {Array.ToString(", ")}");
   }
}

internal class TestProgram : CommandProcessor
{
   public TestProgram() : base("test")
   {
      File = nil;
      Folder = nil;
      Text = "";
      Pattern = "; f";
      Replacement = "";
   }

   [Command("find", "Find text using pattern", "$source$pattern$count?")]
   public void Find()
   {
      var _result = Text.Matches(Pattern);
      if (_result is (true, var result))
      {
         Console.WriteLine($"{Count}: {result.FirstMatch}");
      }
      else
      {
         Console.WriteLine("No match");
      }
   }

   [Command("replace", "Replace text using pattern", "$source$pattern$replacement;$source$pattern$ignore-case?")]
   public void Replace()
   {
   }

   [Command("replace2", "Replace text using pattern", "$source$pattern$replacement;$source$pattern$ignore-case?",
      replacements: "pattern:Unfriendly matching pattern")]
   public void Replace2()
   {
   }

   [Command("scan", "Scan for data", "$folder;$file")]
   public void Scan()
   {
      if (File)
      {
         Console.WriteLine($"Scanning file {File}");
      }
      else if (Folder)
      {
         Console.WriteLine($"Scanning folder {Folder}");
      }
      else
      {
         Console.WriteLine("Scanning nothing");
      }
   }

   [Switch("source", "string", "Source input from user")]
   public string Text { get; set; }

   [Switch("pattern", "pattern", "Friendly matching pattern", "p")]
   public string Pattern { get; set; }

   [Switch("count", "int", "Number of matches to consume", "c")]
   public int Count { get; set; }

   [Switch("ignore-case", "boolean", "ignore case", "i")]
   public bool IgnoreCase { get; set; }

   [Switch("replacement", "string", "String pattern for replacement", "r")]
   public string Replacement { get; set; }

   [Switch("file", "file", "File to use", "f")]
   public Maybe<FileName> File { get; set; }

   [Switch("folder", "folder", "Folder to use", "f2")]
   public Maybe<FolderName> Folder { get; set; }

   public override StringHash GetConfigurationDefaults() => [];

   public override StringHash GetConfigurationHelp() => [];

   public override void Initialize()
   {
      Text = string.Empty;
      Pattern = string.Empty;
      Count = 0;
      IgnoreCase = false;
   }
}

[TestClass]
public class ApplicationTests
{
   [TestMethod]
   public void RunTest()
   {
      var program = new Program();
      program.Run("foo.exe pull /code: 153 /amount: 153.69 /attribute-targets: all /array: [a, b, c]", "/", ":");

      Console.WriteLine();

      var objectProgram = new ObjectProgram();
      objectProgram.Run("foo.exe pull /code: 153 /amount: 153.69 /attribute-targets: all /array: [a, b, c]", "/", ":");

      Console.WriteLine();

      var thisProgram = new ThisProgram();
      thisProgram.Run("foo.exe pull /code: 153 /amount: 153.69 /attribute-targets: all /array: [a, b, c]", "/", ":");
   }

   [TestMethod]
   public void GreedyTest()
   {
      var program = new Program();
      program.Run("\"foo.exe\" push /code: 153 /amount: 153.69 /attribute-targets: all /text: 'foo' /recursive: false /array: [a, b, c]", "/",
         ":");

      Console.WriteLine();

      var objectProgram = new ObjectProgram();
      objectProgram.Run("\"foo.exe\" push /code: 153 /amount: 153.69 /attribute-targets: all /text: 'foo' /recursive: false /array: [a, b, c]",
         "/", ":");

      Console.WriteLine();

      var thisProgram = new ThisProgram();
      thisProgram.Run("\"foo.exe\" push /code: 153 /amount: 153.69 /attribute-targets: all /text: 'foo' /recursive: false /array: [a, b, c]", "/",
         ":");
   }

   [TestMethod]
   public void AlternateSyntax()
   {
      var program = new Program();
      program.Run("show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive- --array [a,b,c]", "--", " ");

      Console.WriteLine();

      var objectProgram = new ObjectProgram();
      objectProgram.Run("show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive- --array [a,b,c]", "--", " ");

      Console.WriteLine();

      var thisProgram = new ThisProgram { Shortcuts = "C = code; A = amount; R = recursive; a = array" };
      thisProgram.Run("show -C 153 -A 153.69 --attribute-targets all --text 'foo' -R -a [a, b, c]", "--", " ");
   }

   [TestMethod]
   public void Aliases()
   {
      var program = new Program { Application = "test" };
      program.Run("alias std show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive", "--", " ");
      program.Run("std", "--", " ");

      Console.WriteLine();

      var objectProgram = new ObjectProgram { Application = "test.object" };
      objectProgram.Run("alias std show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive", "--", " ");
      objectProgram.Run("std", "--", " ");

      Console.WriteLine();

      var thisProgram = new ThisProgram { Application = "test.this" };
      thisProgram.Run("alias std show --code 153 --amount 153.69 --attribute-targets all --text 'foo' --recursive", "--", " ");
      thisProgram.Run("std", "--", " ");
   }

   [TestMethod]
   public void ResourceTest()
   {
      var resources = new Resources<ApplicationTests>();
      var result = resources.String("Tests.Foobar.txt");
      Console.WriteLine(result);
   }

   [TestMethod]
   public void CommandProcessorTest()
   {
      var processor = new TestProgram();
      processor.Run("find -p \"/(-/s+)\" -c 153 --ignore-case --source foobar");
   }

   [TestMethod]
   public void CommandHelpTest()
   {
      var processor = new TestProgram();
      processor.Run("help");
   }

   [TestMethod]
   public void CommandHelpOnFindTest()
   {
      var processor = new TestProgram();
      processor.Run("help find");
   }

   [TestMethod]
   public void CommandHelpOnReplaceTest()
   {
      var processor = new TestProgram();
      processor.Run("help replace");
   }

   [TestMethod]
   public void CommandHelpOnReplace2Test()
   {
      var processor = new TestProgram();
      processor.Run("help replace2");
   }

   [TestMethod]
   public void CommandHelpOnScanTest()
   {
      var processor = new TestProgram();
      processor.Run("help scan");
   }

   [TestMethod]
   public void CommandHelpOnConfigTest()
   {
      var processor = new TestProgram();
      processor.Run("help config");
   }

   [TestMethod]
   public void ConfigurationTest()
   {
      var processor = new TestProgram();
      processor.Run("config --set foobar \"x\"");
      processor.Run("config -g foobar");

      Console.WriteLine(processor.Arguments);
   }

   [TestMethod]
   public void ScanFileTest()
   {
      var processor = new TestProgram();
      processor.Run(@"scan -f C:\Temp\max.txt");

      processor = new TestProgram();
      processor.Run(@"scan -f2 C:\Temp");

      processor = new TestProgram();
      processor.Run("scan");
   }

   [TestMethod]
   public void UnknownCommandTest()
   {
      var processor = new TestProgram();
      processor.Run(@"foobar -f C:\Temp\max.txt");
   }

   protected static IEnumerable<string> source()
   {
      yield return "alpha";
      yield return "bravo";
      yield return "charlie";
   }

   [TestMethod]
   public void EventSourceTest()
   {
      var eventSource = new EventSource<string>(source);
      eventSource.EventRaised += (_, e) => Console.WriteLine(e.Value);
      eventSource.Evaluate();
   }

   protected static IEnumerable<Either<string, int>> source2()
   {
      yield return "delta";
      yield return 42;
      yield return "echo";
   }

   [TestMethod]
   public void EventSource2Test()
   {
      var eventSource = new EventSource<Either<string, int>>(source2);
      eventSource.EventRaised += (_, e) =>
      {
         switch (e.Value.ToObject())
         {
            case string s:
               Console.WriteLine($"string: {s}");
               break;
            case int i:
               Console.WriteLine($"int: {i}");
               break;
         }
      };
      eventSource.Evaluate();
   }

   [TestMethod]
   public void ConsoleProcessorTest()
   {
      var processor = new ConsoleProcessor();

      var fileCommand = processor + "file" + file;
      (fileCommand + "source" + typeof(FileName) + "Source file").Parameter();
      (fileCommand + "target" + typeof(FolderName) + true + "Target folder").Parameter();
      fileCommand.Command();

      var formatCommand = processor + "format" + format;
      (formatCommand + "initialize" + typeof(bool) + true + 'i' + "Initialize disk").Parameter();
      (formatCommand + "drive" + typeof(string) + 'd' + "Drive to format").Parameter();
      formatCommand.Command();

      var _result = processor.Execute(@"file --source P:\Temp\angular.txt");
      if (!_result)
      {
         Console.WriteLine(_result.Exception.Map(e => e.Message) | "Not found");
      }

      Console.WriteLine();
      _result = processor.Execute(@"file --source P:\Temp\angular.txt --target P:\Temp");
      if (!_result)
      {
         Console.WriteLine(_result.Exception.Map(e => e.Message) | "Not found");
      }

      Console.WriteLine();
      _result = processor.Execute(@"format -i --drive C");
      if (!_result)
      {
         Console.WriteLine(_result.Exception.Map(e => e.Message) | "Not found");
      }

      Console.WriteLine();
      _result = processor.Execute(@"format -i --drive");
      if (!_result)
      {
         Console.WriteLine(_result.Exception.Map(e => e.Message) | "Not found");
      }

      Console.WriteLine();
      _result = processor.Execute(@"format");
      if (!_result)
      {
         Console.WriteLine(_result.Exception.Map(e => e.Message) | "Not found");
      }
   }

   protected static Optional<Unit> file(CommandValue fileCommand)
   {
      try
      {
         if (fileCommand["source"] is (true, FileParameterValue fileParameterValue))
         {
            var sourceFile = fileParameterValue.Value;
            var targetFolder = fileCommand["target"].CastAs<FolderParameterValue>().Map(f => f.Value) | @"C:\Temp";
            Console.WriteLine($"copied {sourceFile.FullPath} -> {targetFolder.FullPath}");

            return unit;
         }
         else
         {
            return nil;
         }
      }
      catch (Exception exception)
      {
         return exception;
      }
   }

   protected static Optional<Unit> format(CommandValue formatCommand)
   {
      try
      {
         if (formatCommand["initialize"] is (true, BoolParameterValue { Value: true }))
         {
            Console.WriteLine("initialized");
         }

         var drive = formatCommand["drive"].CastAs<StringParameterValue>().Required("Drive not provided").Value;
         Console.WriteLine($"Using drive {drive} ");

         return unit;
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}