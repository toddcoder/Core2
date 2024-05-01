using Core.Configurations;
using Core.Enumerables;
using Core.Matching;
using Core.Strings;

namespace Core.Internet.Smtp;

public class Address
{
   protected static string joinByCommas(string value) => value.Unjoin("/s* [';,'] /s*; f").ToString(", ");

   protected string to;
   protected string cc;
   protected string bcc;

   public Address()
   {
      Server = string.Empty;
      From = string.Empty;
      to = string.Empty;
      cc = string.Empty;
      bcc = string.Empty;
      Subject = string.Empty;
   }

   public Address(Setting addressSetting)
   {
      Server = addressSetting.Value.String("server");
      From = addressSetting.Value.String("from");
      to = joinByCommas(addressSetting.Value.String("to"));
      cc = joinByCommas(addressSetting.Value.String("cc"));
      bcc = joinByCommas(addressSetting.Value.String("bcc"));
      Subject = addressSetting.Value.String("subject");
   }

   public string Server { get; init; }

   public string From { get; init; }

   public string To
   {
      get => to;
      init => to = joinByCommas(value);
   }

   public string CC
   {
      get => cc;
      init => cc = joinByCommas(value);
   }

   public string BCC
   {
      get => bcc;
      init => bcc = joinByCommas(value);
   }

   public string Subject { get; init; }

   public Address Clone(string subject) => new()
   {
      Server = new string(Server),
      From = new string(From),
      To = new string(to),
      CC = new string(cc),
      BCC = new string(bcc),
      Subject = subject
   };

   public Address Clone() => Clone(new string(Subject));

   public override string ToString()
   {
      List<string> result = [];

      if (Server.IsNotEmpty())
      {
         result.Add(Server);
      }

      if (From.IsNotEmpty())
      {
         result.Add(From);
      }

      if (To.IsNotEmpty())
      {
         result.Add(to);
      }

      if (CC.IsNotEmpty())
      {
         result.Add(cc);
      }

      if (BCC.IsNotEmpty())
      {
         result.Add(bcc);
      }

      if (Subject.IsNotEmpty())
      {
         result.Add(Subject);
      }

      return result.ToString(", ");
   }
}