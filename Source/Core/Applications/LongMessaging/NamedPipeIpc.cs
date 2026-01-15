using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Assertions;
using Core.Dates.DateIncrements;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.LongMessaging;

public static class NamedPipeIpc
{
   public static Task StartServerAsync(string pipeName, Action<string> onMessage, CancellationToken cancellationToken = default)
   {
      pipeName.Must().Not.BeEmpty().OrThrow("Pipe name must not be empty");
      return Task.Run(async () =>
      {
         while (!cancellationToken.IsCancellationRequested)
         {
#pragma warning disable CA1416
            await using var server = new NamedPipeServerStream(pipeName, PipeDirection.InOut, NamedPipeServerStream.MaxAllowedServerInstances,
               PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            try
            {
               await server.WaitForConnectionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
               break;
            }

            try
            {
               using var reader = new StreamReader(server, Encoding.UTF8);
               while (!cancellationToken.IsCancellationRequested && server.IsConnected)
               {
                  var builder = new StringBuilder();
                  var buffer = new char[1024];
                  do
                  {
                     var read = await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false);
                     if (read == 0)
                     {
                        break;
                     }

                     builder.Append(buffer, 0, read);
                  } while (!server.IsMessageComplete);

                  if (builder.Length > 0)
                  {
                     try
                     {
                        onMessage(builder.ToString());
                     }
                     catch
                     {
                     }
                  }

                  if (!server.IsConnected)
                  {
                     break;
                  }
               }
            }
            catch (IOException)
            {
            }
            finally
            {
               try
               {
                  server.Disconnect();
               }
               catch
               {
               }
            }
#pragma warning restore CA1416
         }
      }, cancellationToken);
   }

   public static async Task SendAsync(string pipeName, string message, Maybe<TimeSpan> timeout)
   {
      pipeName.Must().Not.BeEmpty().OrThrow("Pipe name must not be empty");
      message.Must().Not.BeEmpty().OrThrow("Message must not be empty");

      await using var client = new NamedPipeClientStream(".", pipeName, PipeDirection.Out, PipeOptions.Asynchronous);
      var connectTimeout = timeout | 2.Seconds();
      await client.ConnectAsync((int)connectTimeout.TotalMilliseconds).ConfigureAwait(false);

      await using var writer = new StreamWriter(client, Encoding.UTF8);
      writer.AutoFlush = true;
      await writer.WriteAsync(message).ConfigureAwait(false);
#pragma warning disable CA1416 // Validate platform compatibility
      client.WaitForPipeDrain();
#pragma warning restore CA1416 // Validate platform compatibility
   }

   public static async Task SendAsync(string pipeName, string message) => await SendAsync(pipeName, message, nil);
}