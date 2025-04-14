using System;

namespace Core.Strings;

public class StringAppender(string source)
{
   public static implicit operator string(StringAppender appender) => appender.Source;

   public static string operator |(StringAppender appender, string defaultValue) => appender.isNotEmpty ? appender : defaultValue;

   protected bool isNotEmpty = source.IsNotEmpty();

   public string Source => source;

   public StringAppender Prefix(string prefix)
   {
      return isNotEmpty ? new StringAppender(prefix + source) : this;
   }

   public StringAppender Suffix(string suffix)
   {
      return isNotEmpty ? new StringAppender(source + suffix) : this;
   }

   public StringAppender Map(Func<string, string> mappingFunc)
   {
      return isNotEmpty ? new StringAppender(mappingFunc(source)) : this;
   }

   public StringAppender Replace(string replacement)
   {
      return isNotEmpty ? new StringAppender(this) : new StringAppender(replacement);
   }
}