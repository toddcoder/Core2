using System;
using System.Linq;
using System.Reflection;

namespace Core.Applications.Messaging;

public abstract class SubscriberChannel<TPayload>(string name) : IDisposable where TPayload : notnull
{
   protected Subscriber<TPayload> receiver = new(name);
   protected Publisher<TPayload> sender = new(name);

   public virtual void Start()
   {
      var type = GetType();

      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith("Receive"));
      foreach (var method in methods)
      {
         var parameters = method.GetParameters();
         if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Publication<TPayload>))
         {
            var topic = $"receive-{method.Name[7..]}";
            receiver[topic] = publication => method.Invoke(this, [publication]);
         }
      }

      var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.Name.StartsWith("Receive"));
      foreach (var propertyInfo in properties)
      {
         if (propertyInfo.PropertyType == typeof(Action<Publication<TPayload>>))
         {
            var topic = $"receive-{propertyInfo.Name[7..]}";
            receiver[topic] = publication => ((Action<Publication<TPayload>>)propertyInfo.GetValue(this)!)?.Invoke(publication);
         }
      }

      methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith("Send"));
      foreach (var method in methods)
      {
         var parameters = method.GetParameters();
         if (parameters.Length == 0)
         {
            var topic = $"send-{method.Name[4..]}";
            var result = method.Invoke(this, null);
            if (result is TPayload payload)
            {
               sender.Publish(topic, payload);
            }
         }
      }

      properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.Name.StartsWith("Send"));
      foreach (var propertyInfo in properties)
      {
         if (propertyInfo.PropertyType == typeof(Action<Publication<TPayload>>))
         {
            var topic = $"send-{propertyInfo.Name[4..]}";
            sender[topic] = publication => ((Action<Publication<TPayload>>)propertyInfo.GetValue(this)!)?.Invoke(publication);
         }
      }
   }

   public void ReceiverSuspend() => receiver.Suspend();

   public void ReceiverResume() => receiver.Resume();

   public void SenderSuspend() => receiver.Suspend();

   public void SenderResume() => receiver.Resume();

   public void Suspend()
   {
      ReceiverSuspend();
      SenderSuspend();
   }

   public void Resume()
   {
      ReceiverResume();
      SenderResume();
   }

   public void Dispose()
   {
      receiver.Unsubscribe();
      sender.Unsubscribe();
   }
}

public abstract class SubscriberChannel(string name) : IDisposable
{
   protected Subscriber receiver = new($"{name}-receiver");
   protected Subscriber sender = new($"{name}-sender");

   public virtual void Start()
   {
      var type = GetType();

      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith("Receive"));
      foreach (var method in methods)
      {
         var parameters = method.GetParameters();
         if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Publication))
         {
            var topic = $"receive-{method.Name[7..]}";
            receiver[topic] = publication => method.Invoke(this, [publication]);
         }
      }

      var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.Name.StartsWith("Receive"));
      foreach (var propertyInfo in properties)
      {
         if (propertyInfo.PropertyType == typeof(Action<Publication>))
         {
            var topic = $"receive-{propertyInfo.Name[7..]}";
            receiver[topic] = publication => ((Action<Publication>)propertyInfo.GetValue(this)!)?.Invoke(publication);
         }
      }

      methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith("Send"));
      foreach (var method in methods)
      {
         var parameters = method.GetParameters();
         if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Publication))
         {
            var topic = $"send-{method.Name[4..]}";
            sender[topic] = publication => method.Invoke(this, [publication]);
         }
      }

      properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.Name.StartsWith("Send"));
      foreach (var propertyInfo in properties)
      {
         if (propertyInfo.PropertyType == typeof(Action<Publication>))
         {
            var topic = $"send-{propertyInfo.Name[4..]}";
            sender[topic] = publication => ((Action<Publication>)propertyInfo.GetValue(this)!)?.Invoke(publication);
         }
      }
   }

   public void ReceiverSuspend() => receiver.Suspend();

   public void ReceiverResume() => receiver.Resume();

   public void SenderSuspend() => receiver.Suspend();

   public void SenderResume() => receiver.Resume();

   public void Suspend()
   {
      ReceiverSuspend();
      SenderSuspend();
   }

   public void Resume()
   {
      ReceiverResume();
      SenderResume();
   }

   public void Dispose()
   {
      receiver.Unsubscribe();
      sender.Unsubscribe();
   }
}