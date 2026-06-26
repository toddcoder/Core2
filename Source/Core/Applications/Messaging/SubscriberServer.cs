using System;
using System.Linq;
using System.Reflection;

namespace Core.Applications.Messaging;

public abstract class SubscriberServer<TPayload>(string name) : IDisposable where TPayload : notnull
{
   protected Subscriber<TPayload> subscriber = new(name);

   public virtual void Start()
   {
      var type = GetType();
      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith("On"));
      foreach (var method in methods)
      {
         var parameters = method.GetParameters();
         if (parameters.Length == 1 && parameters[0].ParameterType == typeof(TPayload))
         {
            var topic = method.Name[2..];
            subscriber.SetTopic(topic, payload => { subscriber[topic] = _ => method.Invoke(this, [new Publication<TPayload>(topic, payload)]); });
         }
      }
   }

   public void Suspend() => subscriber.Suspend();

   public void Resume() => subscriber.Resume();

   public void Dispose()
   {
      subscriber.Dispose();
   }
}

public abstract class SubscriberServer(string name) : IDisposable
{
   protected Subscriber subscriber = new(name);

   public virtual void Start()
   {
      var type = GetType();
      var methods = type.GetMethods(BindingFlags.Public).Where(m => m.Name.StartsWith("On"));
      foreach (var method in methods)
      {
         var parameters = method.GetParameters();
         if (parameters.Length == 1 && parameters[0].ParameterType == typeof(Publication))
         {
            var topic = method.Name[2..];
            subscriber[topic] = publication => method.Invoke(this, [publication]);
         }
      }
   }

   public void Suspend() => subscriber.Suspend();

   public void Resume() => subscriber.Resume();

   public void Dispose()
   {
      subscriber.Dispose();
   }
}