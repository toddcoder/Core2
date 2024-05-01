using System.Text;
using Core.Monads;

namespace Core.Io;

public static class StreamExtensions
{
   public static Result<string> FromStream(this Stream stream, Encoding encoding)
   {
      try
      {
         stream.Position = 0;

         using var reader = new StreamReader(stream, encoding);
         return reader.ReadToEnd();
      }
      catch (Exception exception)
      {
         return exception;
      }
   }
}