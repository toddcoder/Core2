using System;
using System.Threading.Tasks;
using Core.Collections;
using Core.DataStructures;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class Publisher<TPayload> where TPayload : notnull
{
   protected static StringHash<Hash<Guid, Subscriber<TPayload>>> subscribers = [];
   protected static MaybeQueue<(string name, Subscriber<TPayload>subscriber)> pendingSubscribers = [];

   public static void AddSubscriber(string name, Subscriber<TPayload> subscriber)
   {
      var hash = subscribers.Memoize(name, _ => new Hash<Guid, Subscriber<TPayload>>());
      hash[subscriber.Id] = subscriber;
   }

   public static void QueueSubscriber(string name, Subscriber<TPayload> subscriber) => pendingSubscribers.Enqueue((name, subscriber));

   public static void RemoveSubscriber(string name, Subscriber<TPayload> subscriber)
   {
      if (subscribers.Maybe[name] is (true, var hash))
      {
         hash.Maybe[subscriber.Id] = nil;
         if (hash.Count == 0)
         {
            subscribers.Maybe[name] = nil;
         }
      }
   }

   protected string name;
   protected object mutex = new();

   protected Publisher(string name)
   {
      this.name = name;
   }

   public void DequeuePendingSubscriptions()
   {
      while (pendingSubscribers.Dequeue() is (true, var (pendingName, subscriber)))
      {
         AddSubscriber(pendingName, subscriber);
      }
   }

   public void Publish(string topic, TPayload payload)
   {
      lock (mutex)
      {
         foreach (var (publisherName, hash) in subscribers)
         {
            if (publisherName == name)
            {
               foreach (var (_, subscriber) in hash)
               {
                  Task.Run(() => subscriber.Received.Invoke(new Publication<TPayload>(topic, payload)));
               }
            }
         }

         DequeuePendingSubscriptions();
      }
   }

   public static void Publish(string name, string topic, TPayload payload)
   {
      var publisher = new Publisher<TPayload>(name);
      publisher.Publish(topic, payload);
   }

   public void PublishSync(string topic, TPayload payload)
   {
      foreach (var (publisherName, hash) in subscribers)
      {
         if (publisherName == name)
         {
            foreach (var (_, subscriber) in hash)
            {
               subscriber.Received.Invoke(new Publication<TPayload>(topic, payload));
            }
         }
      }

      DequeuePendingSubscriptions();
   }

   public static void PublishSync(string name, string topic, TPayload payload)
   {
      var publisher = new Publisher<TPayload>(name);
      publisher.PublishSync(topic, payload);
   }

   public async Task PublishAsync(string topic, TPayload payload)
   {
      foreach (var (publisherName, hash) in subscribers)
      {
         if (publisherName == name)
         {
            foreach (var (_, subscriber) in hash)
            {
               await subscriber.Received.InvokeAsync(new Publication<TPayload>(topic, payload));
            }
         }
      }

      DequeuePendingSubscriptions();
   }

   public static async Task PublishAsync(string name, string topic, TPayload payload)
   {
      var publisher = new Publisher<TPayload>(name);
      await publisher.PublishAsync(topic, payload);
   }
}

public class Publisher
{
   protected static StringHash<Hash<Guid, Subscriber>> subscribers = [];
   protected static MaybeQueue<(string name, Subscriber subscriber)> pendingSubscribers = [];

   public static void AddSubscriber(string name, Subscriber subscriber)
   {
      var hash = subscribers.Memoize(name, _ => new Hash<Guid, Subscriber>());
      hash[subscriber.Id] = subscriber;
   }

   public static void QueueSubscriber(string name, Subscriber subscriber) => pendingSubscribers.Enqueue((name, subscriber));

   public static void RemoveSubscriber(string name, Subscriber subscriber)
   {
      if (subscribers.Maybe[name] is (true, var hash))
      {
         hash.Maybe[subscriber.Id] = nil;
         if (hash.Count == 0)
         {
            subscribers.Maybe[name] = nil;
         }
      }
   }

   protected string name;
   protected object mutex = new();

   protected Publisher(string name)
   {
      this.name = name;
   }

   public void DequeuePendingSubscriptions()
   {
      while (pendingSubscribers.Dequeue() is (true, var (pendingName, subscriber)))
      {
         AddSubscriber(pendingName, subscriber);
      }
   }

   public void Publish(string topic)
   {
      lock (mutex)
      {
         foreach (var (publisherName, hash) in subscribers)
         {
            if (publisherName == name)
            {
               foreach (var (_, subscriber) in hash)
               {
                  Task.Run(() => subscriber.Received.Invoke(new Publication(topic)));
               }
            }
         }

         DequeuePendingSubscriptions();
      }
   }

   public static void Publish(string name, string topic)
   {
      var publisher = new Publisher(name);
      publisher.Publish(topic);
   }

   public void PublishSync(string topic)
   {
      foreach (var (publisherName, hash) in subscribers)
      {
         if (publisherName == name)
         {
            foreach (var (_, subscriber) in hash)
            {
               subscriber.Received.Invoke(new Publication(topic));
            }
         }
      }

      DequeuePendingSubscriptions();
   }

   public static void PublishSync(string name, string topic)
   {
      var publisher = new Publisher(name);
      publisher.PublishSync(topic);
   }

   public async Task PublishAsync(string topic)
   {
      foreach (var (publisherName, hash) in subscribers)
      {
         if (publisherName == name)
         {
            foreach (var (_, subscriber) in hash)
            {
               await subscriber.Received.InvokeAsync(new Publication(topic));
            }
         }
      }

      DequeuePendingSubscriptions();
   }

   public static async Task PublishAsync(string name, string topic)
   {
      var publisher = new Publisher(name);
      await publisher.PublishAsync(topic);
   }
}