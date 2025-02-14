namespace Core.Applications.Messaging;

public record Publication<TPayload>(string Topic, TPayload Payload) where TPayload : notnull;