namespace Core.Json;

[Flags]
public enum JsonRetrieverOptions
{
   None = 0,
   UsesPath = 1,
   StopAfterFirstRetrieval = 2,
   StopAfterParametersConsumed = 4
}