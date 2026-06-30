using Core.Applications.Messaging;

namespace Core.Tests;

[TestClass]
public class PublishSubscribeTests
{
   protected class AlphabetSubscriberServer() : Channel<string, byte>("alphabet0")
   {
      public void OnAlpha(string payload)
      {
         Console.WriteLine($"alpha: {payload}");
      }

      public void OnBeta(string payload)
      {
         Console.WriteLine($"beta: {payload}");
      }

      public void OnCharlie(string payload)
      {
         Console.WriteLine($"charlie: {payload}");
      }

      public void OnDelta(Func<string> payload)
      {
         Console.WriteLine($"delta: {payload()}");
      }

      public void PublishAlpha(string payload) => Send("Alpha", payload);

      public void PublishBeta(string payload) => Send("Beta", payload);

      public void PublishCharlie(string payload) => Send("Charlie", payload);

      public void PublishDelta(string payload) => Send("Delta", payload);
   }

   [TestMethod]
   public void SubscriberServerTest()
   {
      /*using var subscriberServer = new AlphabetSubscriberServer();
      //subscriberServer.OnDelta(()=>Console.WriteLine($"delta: {payload()}"));
      subscriberServer.Start();

      var center = new AlphabetPublishingCenter();
      center.PublishAlpha("A");
      center.PublishBeta("B");
      center.PublishCharlie("C");
      center.PublishDelta("D");*/
      using var center = new AlphabetSubscriberServer();
      center.Start();
      center.PublishAlpha("A");
      center.PublishBeta("B");
      center.PublishCharlie("C");
      center.PublishDelta("D");
   }

   protected class AlphabetChannel() : Channel<string, string>("alphabet1")
   {
      public string QueryA(string query) => "Alpha";

      public string QueryB(string query) => "Bravo";

      public string QueryC(string query) => "Charlie";

      public void ResponseA(string response) => Console.WriteLine($"ResponseA: {response}");

      public void ResponseB(string response) => Console.WriteLine($"ResponseB: {response}");

      public void ResponseC(string response) => Console.WriteLine($"ResponseC: {response}");

      public void SendA(string query) => Send("A", query);

      public void SendB(string query) => Send("B", query);

      public void SendC(string query) => Send("C", query);
   }

   protected class StringToIntChannel() : Channel<string, int>("string-to-int")
   {
      public int QueryLength(string query) => query.Length;

      public int QueryLengthOfLength(string query) => query.Length.ToString().Length;

      public int QueryParse(string query) => int.Parse(query);

      public void ResponseLength(int response) => Console.WriteLine($"ResponseLength: {response}");

      public void ResponseLengthOfLength(int response) => Console.WriteLine($"ResponseLengthOfLength: {response}");

      public void ResponseParse(int response) => Console.WriteLine($"ResponseParse: {response}");

      public void Length(string query) => Send("Length", query);

      public void LengthOfLength(string query) => Send("LengthOfLength", query);

      public void Parse(string query) => Send("Parse", query);
   }

   [TestMethod]
   public void ChannelTest()
   {
      using var alphabetChannel = new AlphabetChannel();
      alphabetChannel.Start();
      alphabetChannel.SendA("A");
      alphabetChannel.SendB("B");
      alphabetChannel.SendC("C");

      using var stringToIntChannel = new StringToIntChannel();
      stringToIntChannel.Start();
      stringToIntChannel.Length("Hello");
      stringToIntChannel.LengthOfLength("Hello");
      stringToIntChannel.Parse("123");
   }

   public abstract record Arguments
   {
      public sealed record Integer(int I) : Arguments;

      public sealed record Double(double D) : Arguments;

      public sealed record Str(string S) : Arguments;
   }

   public class ArgumentsChannel() : Channel<Arguments>("arguments")
   {
      public Arguments QueryInteger(Arguments.Integer query) => new Arguments.Integer(query.I + 1);

      public Arguments QueryDouble(Arguments.Double query) => new Arguments.Double(query.D * 2);

      public Arguments QueryStr(Arguments.Str query) => new Arguments.Str(query.S.ToUpper());

      public void ResponseInteger(Arguments.Integer response) => Console.WriteLine($"ResponseInteger: {response.I}");

      public void ResponseDouble(Arguments.Double response) => Console.WriteLine($"ResponseDouble: {response.D}");

      public void ResponseStr(Arguments.Str response) => Console.WriteLine($"ResponseStr: {response.S}");

      public void SendInteger(int i) => Send("Integer", new Arguments.Integer(i));

      public void SendDouble(double d) => Send("Double", new Arguments.Double(d));

      public void SendStr(string s) => Send("Str", new Arguments.Str(s));
   }

   [TestMethod]
   public void SubTypeTest()
   {
      using var argumentsChannel = new ArgumentsChannel();
      argumentsChannel.Start();
      argumentsChannel.SendInteger(1);
      argumentsChannel.SendDouble(2.0);
      argumentsChannel.SendStr("hello");
   }
}