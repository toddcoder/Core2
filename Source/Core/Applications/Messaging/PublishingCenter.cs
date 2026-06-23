namespace Core.Applications.Messaging;

public abstract class PublishingCenter<TPayload>(string name) where TPayload : notnull
{
   protected Publisher<TPayload> publisher = new(name);

   protected void publish(string topic, TPayload payload) => publisher.Publish(topic, payload);
}

public abstract class PublishingCenter(string name)
{
   protected Publisher publisher = new(name);

   protected void publish(string topic) => publisher.Publish(topic);
}