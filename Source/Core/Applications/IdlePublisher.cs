using Core.Applications.Messaging;

namespace Core.Applications;

public class IdlePublisher
{
   protected Publisher<int> publisher;
   protected Idle idle;

   public IdlePublisher(string name, string topic, int idleThreshold = 60)
   {
      publisher = new Publisher<int>(name);
      idle = new Idle(idleThreshold)
      {
         UserIdle =
         {
            Handler = p => publisher.Publish(topic, p)
         }
      };
   }

   public void CheckIdleTime() => idle.CheckIdleTime();
}