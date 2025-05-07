using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Collections;
using Core.Configurations;
using Core.Dates.DateIncrements;

namespace Core.Applications.Messaging;

public class XSubscriber
{
   protected string name;
   protected StringHash<MessageEvent<Setting>> events = [];
   protected bool isListening;
   protected readonly TimeSpan timeout = 1.Second();

   public MessageEvent<Exception> ExceptionRaised = new();
   public MessageEvent Waiting = new();

   public XSubscriber(string name)
   {
      this.name = name;
   }

   public Action<Setting> this[string topic]
   {
      set => events[topic] = new MessageEvent<Setting> { Handler = value };
   }

   public void StartListening()
   {
      isListening = true;
      try
      {
         while (isListening)
         {
            foreach (var (topic, messageEvent) in events)
            {
               using var pipeClient = new NamedPipeClientStream(".", $"{name}${topic}", PipeDirection.In);
               pipeClient.Connect(timeout);

               using var reader = new StreamReader(pipeClient, Encoding.UTF8);
               var payload = reader.ReadLine();
               if (payload is not null)
               {
                  var _setting = Setting.FromString(payload);
                  if (_setting is (true, var setting))
                  {
                     messageEvent.Invoke(setting);
                  }
                  else
                  {
                     ExceptionRaised.Invoke(_setting.Exception);
                  }
               }

               Thread.Sleep(100);
               Waiting.Invoke();
               if (!isListening)
               {
                  break;
               }
            }
         }
      }
      catch (Exception exception)
      {
         ExceptionRaised.Invoke(exception);
      }
   }

   public async Task StartListeningAsync()
   {
      isListening = true;
      try
      {
         while (isListening)
         {
            foreach (var (topic, messageEvent) in events)
            {
               await using var pipeClient = new NamedPipeClientStream(".", $"{name}${topic}", PipeDirection.In);
               await pipeClient.ConnectAsync();
               using var reader = new StreamReader(pipeClient, Encoding.UTF8);
               var payload = await reader.ReadLineAsync();
               if (payload is not null)
               {
                  var _setting = Setting.FromString(payload);
                  if (_setting is (true, var setting))
                  {
                     await messageEvent.InvokeAsync(setting);
                  }
                  else
                  {
                     await ExceptionRaised.InvokeAsync(_setting.Exception);
                  }
               }

               Thread.Sleep(100);
               await Waiting.InvokeAsync();
               if (!isListening)
               {
                  break;
               }
            }
         }
      }
      catch (Exception exception)
      {
         await ExceptionRaised.InvokeAsync(exception);
      }
   }

   public void StopListening() => isListening = false;

   public bool IsListening => isListening;
}