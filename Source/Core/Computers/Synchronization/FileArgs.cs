﻿using System;

namespace Core.Computers.Synchronization;

public class FileArgs : EventArgs
{
   public FileArgs(FileName sourceFile, FileName targetFile, string message = "")
   {
      SourceFile = sourceFile;
      TargetFile = targetFile;
      Message = message;
   }

   public FileName SourceFile { get; }

   public FileName TargetFile { get; }

   public virtual string Message { get; }
}