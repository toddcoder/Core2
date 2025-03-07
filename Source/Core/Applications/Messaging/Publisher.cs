using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Collections;
using Core.DataStructures;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class Publisher<TPayload> where TPayload : notnull
{
   protected static Hash<Guid, Subscriber<TPayload>> subscribers = [];
   protected static MaybeQueue<Subscriber<TPayload>> pendingSubscribers = [];

   public static void AddSubscriber(Subscriber<TPayload> subscriber) => subscribers[subscriber.Id] = subscriber;

   public static void QueueSubscriber(Subscriber<TPayload> subscriber) => pendingSubscribers.Enqueue(subscriber);

   public static void RemoveSubscriber(Subscriber<TPayload> subscriber) => subscribers.Maybe[subscriber.Id] = nil;

   protected Maybe<string> _topic = nil;
   protected object mutex = new();

   protected Publisher(string topic)
   {
      _topic = topic;
   }

   protected Publisher()
   {
      _topic = nil;
   }

   protected void dequeuePendingSubscriptions()
   {
      while (pendingSubscribers.Dequeue() is (true, var subscriber))
      {
         AddSubscriber(subscriber);
      }
   }

   protected void publish(TPayload payload)
   {
      if (_topic is (true, var topic))
      {
         publish(topic, payload);
      }
   }

   public static void Publish(TPayload payload)
   {
      var publisher = new Publisher<TPayload>();
      publisher.publish(payload);
   }

   protected void publish(string topic, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         lock (mutex)
         {
            Task.Run(() => subscriber.Received.Invoke(new Publication<TPayload>(topic, payload)));
         }
      }

      dequeuePendingSubscriptions();
   }

   protected void publish(string topic, string reader, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         lock (mutex)
         {
            Task.Run(() => subscriber.Received.Invoke(new Publication<TPayload>(topic, payload)));
         }
      }

      dequeuePendingSubscriptions();
   }

   public static void Publish(string topic, TPayload payload)
   {
      var publisher = new Publisher<TPayload>(topic);
      publisher.publish(topic, payload);
   }

   protected void publishSync(TPayload payload)
   {
      if (_topic is (true, var topic))
      {
         publishSync(topic, payload);
      }
   }

   public static void PublishSync(TPayload payload)
   {
      var publisher = new Publisher<TPayload>();
      publisher.publishSync(payload);
   }

   protected void publishSync(string topic, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         lock (mutex)
         {
            subscriber.Received.Invoke(new Publication<TPayload>(topic, payload));
            System.Threading.Thread.Sleep(500);
         }
      }

      dequeuePendingSubscriptions();
   }

   public static void PublishSync(string topic, TPayload payload)
   {
      var publisher = new Publisher<TPayload>(topic);
      publisher.publishSync(topic, payload);
   }

   protected void publishSync(string topic, string reader, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         lock (mutex)
         {
            subscriber.Received.Invoke(new Publication<TPayload>(topic, payload));
            System.Threading.Thread.Sleep(500);
         }
      }

      dequeuePendingSubscriptions();
   }

   public static void PublishSync(string topic, string reader, TPayload payload)
   {
      var publisher = new Publisher<TPayload>(topic);
      publisher.publishSync(topic, reader, payload);
   }

   protected async Task publishAsync(TPayload payload)
   {
      if (_topic is (true, var topic))
      {
         await publishAsync(topic, payload);
      }
   }

   public static async Task PublishAsync(TPayload payload)
   {
      var publisher = new Publisher<TPayload>();
      await publisher.publishAsync(payload);
   }

   protected async Task publishAsync(string topic, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         await subscriber.Received.InvokeAsync(new Publication<TPayload>(topic, payload));
      }

      dequeuePendingSubscriptions();
   }

   public static async Task PublishAsync(string topic, TPayload payload)
   {
      var publisher = new Publisher<TPayload>(topic);
      await publisher.publishAsync(topic, payload);
   }

   protected async Task publishAsync(string topic, string reader, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         await subscriber.Received.InvokeAsync(new Publication<TPayload>(topic, payload));
      }

      dequeuePendingSubscriptions();
   }

   public static async Task PublishAsync(string topic, string reader, TPayload payload)
   {
      var publisher = new Publisher<TPayload>(topic);
      await publisher.publishAsync(topic, reader, payload);
   }
}