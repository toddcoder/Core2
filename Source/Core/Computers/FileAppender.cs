using System;
using System.IO;

namespace Core.Computers;

public class FileAppender(FileName file) : IDisposable
{
   protected StreamWriter writer = new(file.FullPath, append: true);

   public void Write(object value) => writer.Write(value);

   public void WriteLine(object value) => writer.WriteLine(value);

   public void Close() => writer.Close();

   public void Flush() => writer.Flush();

   public void Dispose()
   {
      writer.Dispose();
      GC.SuppressFinalize(this);
   }

   ~FileAppender() => writer.Dispose();
}