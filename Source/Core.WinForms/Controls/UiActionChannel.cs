using Core.Applications.Messaging;

namespace Core.WinForms.Controls;

public class UiActionChannel
{
   protected readonly UiAction uiAction;
   protected readonly string id;
   protected readonly Subscriber<UiActionState> subscriber;

   public UiActionChannel(UiAction uiAction)
   {
      this.uiAction = uiAction;
      id = Guid.NewGuid().ToString();
      subscriber = new(id, false);
   }

   public void Subscribe()
   {
      subscriber.Subscribe();
      subscriber["send"] = p => uiAction.Do(() => uiAction.State = p.Payload);
   }

   public void Unsubscribe() => subscriber.Unsubscribe();

   public void Send(UiActionState state) => Publisher<UiActionState>.Publish(id, "send", state);
}