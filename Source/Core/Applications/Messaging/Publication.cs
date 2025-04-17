using System;

namespace Core.Applications.Messaging;

public record Publication<TPayload>(string Topic, TPayload Payload) where TPayload : notnull;

public record Publication<TTopic, TPayload>(TTopic Topic, TPayload Payload) where TTopic : notnull where TPayload : notnull;

public record Publication(string Topic);