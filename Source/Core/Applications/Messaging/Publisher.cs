using System;
using System.Linq;
using System.Threading.Tasks;
using Core.Assertions;
using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public class Publisher<TPayload> where TPayload : notnull
{
   protected static Hash<Guid, Subscriber<TPayload>> subscribers = [];

   public static void AddSubscriber(Subscriber<TPayload> subscriber) => subscribers[subscriber.Id] = subscriber;

   public static void RemoveSubscriber(Subscriber<TPayload> subscriber) => subscribers.Maybe[subscriber.Id] = nil;

   protected Maybe<string> _topic = nil;
   protected object mutex = new();

   public Publisher(string topic)
   {
      _topic = topic;
   }

   public Publisher()
   {
      _topic = nil;
   }

   public void Publish(TPayload payload)
   {
      if (_topic is (true, var topic))
      {
         Publish(topic, payload);
      }
   }

   public void Publish(string topic, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         lock (mutex)
         {
            Task.Run(() => subscriber.Received.Invoke(new Publication<TPayload>(topic, payload)));
         }
      }
   }

   public void Publish(string topic, string reader, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         lock (mutex)
         {
            Task.Run(() => subscriber.Received.Invoke(new Publication<TPayload>(topic, payload)));
         }
      }
   }

   public void PublishSync(TPayload payload)
   {
      if (_topic is (true, var topic))
      {
         PublishSync(topic, payload);
      }
   }

   public void PublishSync(string topic, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         lock (mutex)
         {
            subscriber.Received.Invoke(new Publication<TPayload>(topic, payload));
            System.Threading.Thread.Sleep(500);
         }
      }
   }

   public void PublishSync(string topic, string reader, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         lock (mutex)
         {
            subscriber.Received.Invoke(new Publication<TPayload>(topic, payload));
            System.Threading.Thread.Sleep(500);
         }
      }
   }

   public async Task PublishAsync(TPayload payload)
   {
      if (_topic is (true, var topic))
      {
         await PublishAsync(topic, payload);
      }
   }

   public async Task PublishAsync(string topic, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         await subscriber.Received.InvokeAsync(new Publication<TPayload>(topic, payload));
      }
   }

   public async Task PublishAsync(string topic, string reader, TPayload payload)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         await subscriber.Received.InvokeAsync(new Publication<TPayload>(topic, payload));
      }
   }
}

public class Publisher : Publisher<Unit>
{
   public void Publish()
   {
      if (_topic is (true, var topic))
      {
         Publish(topic);
      }
   }

   public void Publish(string topic)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         lock (mutex)
         {
            Task.Run(() => subscriber.Received.Invoke(new Publication(topic)));
         }
      }
   }

   public void Publish(string topic, string reader)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         lock (mutex)
         {
            Task.Run(() => subscriber.Received.Invoke(new Publication(topic)));
         }
      }
   }

   public void PublishSync()
   {
      if (_topic is (true, var topic))
      {
         PublishSync(topic);
      }
   }

   public void PublishSync(string topic)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         lock (mutex)
         {
            subscriber.Received.Invoke(new Publication(topic));
            System.Threading.Thread.Sleep(500);
         }
      }
   }

   public void PublishSync(string topic, string reader)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         lock (mutex)
         {
            subscriber.Received.Invoke(new Publication(topic));
            System.Threading.Thread.Sleep(500);
         }
      }
   }

   public async Task PublishAsync()
   {
      if (_topic is (true, var topic))
      {
         await PublishAsync(topic);
      }
   }

   public async Task PublishAsync(string topic)
   {
      foreach (var (_, subscriber) in subscribers)
      {
         await subscriber.Received.InvokeAsync(new Publication(topic));
      }
   }

   public async Task PublishAsync(string topic, string reader)
   {
      foreach (var (_, subscriber) in subscribers.Where(i => i.Value.Reader == reader))
      {
         await subscriber.Received.InvokeAsync(new Publication(topic));
      }
   }
}