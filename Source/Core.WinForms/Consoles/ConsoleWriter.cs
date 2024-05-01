﻿using System.Text;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Consoles;

public class TextBoxWriter : TextWriter
{
   protected TextBoxConsole console;
   protected Maybe<StringBuilder> _buffer;

   public TextBoxWriter(TextBoxConsole console)
   {
      this.console = console;
      _buffer = maybe<StringBuilder>() & this.console.Buffer & (() => new StringBuilder());
   }

   public bool AutoStop { get; set; }

   public override void Write(char value)
   {
      if (value != '\r')
      {
         if (_buffer is (true, var buffer))
         {
            buffer.Append(value);
         }
         else
         {
            if (AutoStop)
            {
               console.StopUpdating();
            }

            console.Write(value);
            if (AutoStop)
            {
               console.ResumeUpdating();
               console.ScrollToCaret();
            }

            console.ScrollToCaret();
         }
      }
   }

   protected void flush()
   {
      if (_buffer is (true, var buffer))
      {
         if (AutoStop)
         {
            console.StopUpdating();
         }

         foreach (var ch in buffer.ToString())
         {
            console.Write(ch);
         }

         buffer.Clear();

         if (AutoStop)
         {
            console.ResumeUpdating();
         }
      }
      else
      {
         console.Clear();
      }
   }

   public override void Flush() => flush();

   public override Task FlushAsync() => Task.Run(flush);

   public override Encoding Encoding => Encoding.UTF8;
}