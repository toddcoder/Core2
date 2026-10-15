using System;
using System.Linq;
using System.Reflection;

namespace Core.Applications.Messaging;

public abstract class Channel<TQuery, TResponse> : IDisposable where TQuery : notnull where TResponse : notnull
{
   protected const string QUERY = "Query";
   protected const string ON = "On";
   protected const string RESPONSE = "Response";

   protected readonly Subscriber<TQuery> subscriberQuery;
   protected readonly Subscriber<TResponse> subscriberResponse;
   protected readonly Publisher<TQuery> publisherQuery;
   protected bool autoStarted;

   protected Channel(string name, bool autoStart = true, bool autoSubscribe = true)
   {
      subscriberQuery = new Subscriber<TQuery>($"query-{name}", autoSubscribe);
      subscriberResponse = new Subscriber<TResponse>($"response-{name}", autoSubscribe);
      publisherQuery = new Publisher<TQuery>($"query-{name}");

      if (autoStart)
      {
         Start();
      }
   }

   public void Subscribe()
   {
      subscriberQuery.Subscribe();
      subscriberResponse.Subscribe();
   }

   protected void setUpQuery(Type type)
   {
      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith(QUERY));
      foreach (var methodInfo in methods)
      {
         var returnType = methodInfo.ReturnType;
         var parameters = methodInfo.GetParameters();
         if (returnType.IsAssignableTo(typeof(TResponse)) && parameters.Length == 1)
         {
            var topic = methodInfo.Name[QUERY.Length..];
            var parameterType = parameters[0].ParameterType;
            if (parameterType.IsAssignableTo(typeof(TQuery)))
            {
               subscriberQuery.SetTopic(topic, (TQuery query) =>
               {
                  var response = (TResponse)methodInfo.Invoke(this, [query])!;
                  subscriberResponse.InvokeTopic(new Publication<TResponse>(topic, response));
               });
            }
            else if (parameterType.IsAssignableTo(typeof(Func<TQuery>)))
            {
               subscriberQuery.SetTopic(topic, func =>
               {
                  var response = (TResponse)methodInfo.Invoke(this, [func()])!;
                  subscriberResponse.InvokeTopic(new Publication<TResponse>(topic, response));
               });
            }
         }
      }

      methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith(ON));
      foreach (var methodInfo in methods)
      {
         var returnType = methodInfo.ReturnType;
         var parameters = methodInfo.GetParameters();
         if (returnType == typeof(void) && parameters.Length == 1)
         {
            var topic = methodInfo.Name[ON.Length..];
            var parameterType = parameters[0].ParameterType;
            if (typeof(TQuery).IsAssignableTo(parameterType))
            {
               subscriberQuery.SetTopic(topic, (TQuery query) => methodInfo.Invoke(this, [query]));
            }
         }
      }
   }

   protected void setUpResponse(Type type)
   {
      var methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).Where(m => m.Name.StartsWith(RESPONSE));
      foreach (var methodInfo in methods)
      {
         var parameters = methodInfo.GetParameters();
         if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableTo(typeof(TResponse)))
         {
            var topic = methodInfo.Name[RESPONSE.Length..];
            subscriberResponse.SetTopic(topic, (TResponse response) => methodInfo.Invoke(this, [response]));
         }
      }
   }

   public void Send(string topic, TQuery query) => publisherQuery.Publish(topic, query);

   public void Start()
   {
      if (autoStarted)
      {
         return;
      }

      var type = GetType();
      setUpQuery(type);
      setUpResponse(type);

      autoStarted = true;
   }

   public void QuerySuspend() => subscriberQuery.Suspend();

   public void QueryResume() => subscriberQuery.Resume();

   public void ResponseSuspend() => subscriberResponse.Suspend();

   public void ResponseResume() => subscriberResponse.Resume();

   public void Suspend()
   {
      QuerySuspend();
      ResponseSuspend();
   }

   public void Resume()
   {
      QueryResume();
      ResponseResume();
   }

   public void Dispose()
   {
      subscriberQuery.Unsubscribe();
      subscriberResponse.Unsubscribe();
      GC.SuppressFinalize(this);
   }

   ~Channel() => Dispose();
}

public class Channel<T>(string name, bool autoStart = true, bool autoSubscribe = true)
   : Channel<T, T>(name, autoStart, autoSubscribe) where T : notnull
{
}