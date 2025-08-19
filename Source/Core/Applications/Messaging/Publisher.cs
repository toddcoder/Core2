using System;
using System.Threading.Tasks;
using Core.Collections;
using Core.DataStructures;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class Publisher<TPayload> where TPayload : notnull
{
   protected static Memo<string, Hash<Guid, Subscriber<TPayload>>> subscribers = new Memo<string, Hash<Guid, Subscriber<TPayload>>>.Function(_ => []);
   protected static MaybeQueue<(string name, Subscriber<TPayload> subscriber)> pendingSubscribers = [];

   public static void AddSubscriber(string name, Subscriber<TPayload> subscriber)
   {
      subscribers[name][subscriber.Id] = subscriber;
   }

   public static void QueueSubscriber(string name, Subscriber<TPayload> subscriber) => pendingSubscribers.Enqueue((name, subscriber));

   public static void RemoveSubscriber(string name, Subscriber<TPayload> subscriber)
   {
      if (subscribers.Find(name) is (true, var hash))
      {
         hash.Maybe[subscriber.Id] = nil;
         if (hash.Count == 0)
         {
            subscribers.Remove(name);
         }
      }
   }

   protected string name;
   protected object mutex = new();

   public Publisher(string name)
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
                  Task.Run(() => subscriber.InvokeTopic(new Publication<TPayload>(topic, payload)));
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
               subscriber.InvokeTopic(new Publication<TPayload>(topic, payload));
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
               await subscriber.InvokeTopicAsync(new Publication<TPayload>(topic, payload));
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

public class Publisher<TTopic, TPayload> where TTopic : notnull where TPayload : notnull
{
   protected static Memo<string, Hash<Guid, Subscriber<TTopic, TPayload>>> subscribers =
      new Memo<string, Hash<Guid, Subscriber<TTopic, TPayload>>>.Function(_ => []);
   protected static MaybeQueue<(string name, Subscriber<TTopic, TPayload> subscriber)> pendingSubscribers = [];

   public static void AddSubscriber(string name, Subscriber<TTopic, TPayload> subscriber)
   {
      subscribers[name][subscriber.Id] = subscriber;
   }

   public static void QueueSubscriber(string name, Subscriber<TTopic, TPayload> subscriber) => pendingSubscribers.Enqueue((name, subscriber));

   public static void RemoveSubscriber(string name, Subscriber<TTopic, TPayload> subscriber)
   {
      if (subscribers.Find(name) is (true, var hash))
      {
         hash.Maybe[subscriber.Id] = nil;
         if (hash.Count == 0)
         {
            subscribers.Remove(name);
         }
      }
   }

   protected string name;
   protected object mutex = new();

   public Publisher(string name)
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

   public void Publish(TTopic topic, TPayload payload)
   {
      lock (mutex)
      {
         foreach (var (publisherName, hash) in subscribers)
         {
            if (publisherName == name)
            {
               foreach (var (_, subscriber) in hash)
               {
                  Task.Run(() => subscriber.InvokeTopic(new Publication<TTopic, TPayload>(topic, payload)));
               }
            }
         }

         DequeuePendingSubscriptions();
      }
   }

   public static void Publish(string name, TTopic topic, TPayload payload)
   {
      var publisher = new Publisher<TTopic, TPayload>(name);
      publisher.Publish(topic, payload);
   }

   public void PublishSync(TTopic topic, TPayload payload)
   {
      foreach (var (publisherName, hash) in subscribers)
      {
         if (publisherName == name)
         {
            foreach (var (_, subscriber) in hash)
            {
               subscriber.InvokeTopic(new Publication<TTopic, TPayload>(topic, payload));
            }
         }
      }

      DequeuePendingSubscriptions();
   }

   public static void PublishSync(string name, TTopic topic, TPayload payload)
   {
      var publisher = new Publisher<TTopic, TPayload>(name);
      publisher.PublishSync(topic, payload);
   }

   public async Task PublishAsync(TTopic topic, TPayload payload)
   {
      foreach (var (publisherName, hash) in subscribers)
      {
         if (publisherName == name)
         {
            foreach (var (_, subscriber) in hash)
            {
               await subscriber.InvokeTopicAsync(new Publication<TTopic, TPayload>(topic, payload));
            }
         }
      }

      DequeuePendingSubscriptions();
   }

   public static async Task PublishAsync(string name, TTopic topic, TPayload payload)
   {
      var publisher = new Publisher<TTopic, TPayload>(name);
      await publisher.PublishAsync(topic, payload);
   }
}

public class Publisher
{
   protected static Memo<string, Hash<Guid, Subscriber>> subscribers = new Memo<string, Hash<Guid, Subscriber>>.Function(_ => []);
   protected static MaybeQueue<(string name, Subscriber subscriber)> pendingSubscribers = [];

   public static void AddSubscriber(string name, Subscriber subscriber)
   {
      subscribers[name][subscriber.Id] = subscriber;
   }

   public static void QueueSubscriber(string name, Subscriber subscriber) => pendingSubscribers.Enqueue((name, subscriber));

   public static void RemoveSubscriber(string name, Subscriber subscriber)
   {
      if (subscribers.Find(name) is (true, var hash))
      {
         hash.Maybe[subscriber.Id] = nil;
         if (hash.Count == 0)
         {
            subscribers.Remove(name);
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
                  Task.Run(() => subscriber.InvokeTopic(new Publication(topic)));
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
               subscriber.InvokeTopic(new Publication(topic));
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
               await subscriber.InvokeTopicAsync(new Publication(topic));
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