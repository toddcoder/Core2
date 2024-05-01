using System.Net;
using System.Net.Mail;
using Core.Computers;
using Core.Monads;
using Core.Strings;
using Core.Matching;
using Core.Objects;
using static Core.Monads.AttemptFunctions;
using static Core.Monads.MonadFunctions;

namespace Core.Internet.Smtp;

public class Emailer
{
   public static Result<Emailer> StandardEmailer(Address address, string body) => tryTo(() => new Emailer
   {
      Address = address, Priority = PriorityType.High, UseCredentials = true, Body = body
   });

   public Emailer()
   {
      Address = new Address();
      Body = string.Empty;
      Attachments = nil;
      Port = nil;
      Priority = PriorityType.Normal;
      UseCredentials = true;
      Credentials = CredentialCache.DefaultNetworkCredentials;
   }

   public Address Address { get; init; }

   public string Body { get; init; }

   public Maybe<FileName[]> Attachments { get; init; }

   public Maybe<int> Port { get; init; }

   public PriorityType Priority { get; set; }

   public bool UseCredentials { get; set; }

   public ICredentialsByHost Credentials { get; set; }

   protected static void addTo(string destination, MailMessage message, Action<MailMessage, string> action)
   {
      if (destination.IsNotEmpty())
      {
         foreach (var item in destination.Unjoin("/s* [',;'] /s*"))
         {
            action(message, item);
         }
      }
   }

   protected void sendMail(bool isHtml)
   {
      var from = new MailAddress(Address.From);

      var message = new MailMessage
      {
         From = from,
         IsBodyHtml = isHtml,
         Body = Body,
         Subject = Address.Subject,
         Priority = Enum.GetName(typeof(PriorityType), Priority)!.Value().Enumeration<MailPriority>()
      };

      addTo(Address.To, message, (m, s) => m.To.Add(s));
      addTo(Address.CC, message, (m, s) => m.CC.Add(s));
      addTo(Address.BCC, message, (m, s) => m.Bcc.Add(s));

      message.IsBodyHtml = isHtml;

      if (Attachments is (true, var attachments))
      {
         foreach (var attachment in attachments)
         {
            message.Attachments.Add(new Attachment(attachment.ToString()));
         }
      }

      var client = Port.Map(port => new SmtpClient(Address.Server, port)) | (() => new SmtpClient(Address.Server));
      if (UseCredentials)
      {
         client.Credentials = Credentials;
      }

      try
      {
         client.Send(message);
      }
      finally
      {
         if (Attachments)
         {
            foreach (var attachment in message.Attachments)
            {
               attachment.Dispose();
            }
         }
      }
   }

   public void SendText() => sendMail(false);

   public void SendHtml() => sendMail(true);

   public void SendAsHtmlIf(bool isHTML) => sendMail(isHTML);

   public void SendAsTextIf(bool isText) => sendMail(!isText);

   public static void SendTo(string to, string subject, string server, string message, string from)
   {
      var address = new Address { From = from, To = to, Subject = subject, Server = server };
      SendTo(address, message);
   }

   public static void SendTo(string to, string subject, string server, string message) => SendTo(to, subject, server, message, to);

   public static void SendTo(Address address, string message) => new Emailer { Address = address, Body = message }.SendText();

   public EmailerTrying TryTo => new(this);
}