using Core.Applications.Messaging;

namespace Core.Tests;

[TestClass]
public class PublishSubscribeTests
{
   protected class AlphabetSubscriberServer() : SubscriberServer<string>("alphabet")
   {
      public void OnAlpha(Publication<string> publication)
      {
         Console.WriteLine($"alpha: {publication.Payload}");
      }

      public void OnBeta(Publication<string> publication)
      {
         Console.WriteLine($"beta: {publication.Payload}");
      }

      public void OnCharlie(Publication<string> publication)
      {
         Console.WriteLine($"charlie: {publication.Payload}");
      }
   }

   protected class AlphabetPublishingCenter() : PublishingCenter<string>("alphabet")
   {
      public void PublishAlpha(string payload) => publish("Alpha", payload);

      public void PublishBeta(string payload) => publish("Beta", payload);

      public void PublishCharlie(string payload) => publish("Charlie", payload);
   }

   [TestMethod]
   public void SubscriberServerTest()
   {
      using var subscriberServer = new AlphabetSubscriberServer();
      subscriberServer.Start();

      var center = new AlphabetPublishingCenter();
      center.PublishAlpha("A");
      center.PublishBeta("B");
      center.PublishCharlie("C");
   }
}