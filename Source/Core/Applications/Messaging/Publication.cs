using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Applications.Messaging;

public record Publication<TPayload>(string Topic, TPayload Payload) where TPayload : notnull;

public record Publication(string Topic) : Publication<Unit>(Topic, unit);