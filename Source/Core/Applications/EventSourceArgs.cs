using System;

namespace Core.Applications;

public class EventSourceArgs<T>(T value) : EventArgs where T : notnull
{
   public T Value => value;
}