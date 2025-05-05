using System;

namespace Core.Computers;

public abstract record FileChange
{
   public record Created(FileName File) : FileChange;

   public record Deleted(FileName File) : FileChange;

   public record Changed(FileName File) : FileChange;

   public record Renamed(FileName OldFile, FileName NewFile) : FileChange;

   public record Error(Exception Exception) : FileChange;
}