using Core.Collections;

namespace Core.WinForms.Controls;

public class Enabler(Control control) : IHash<string, Control>
{
   protected StringHash<Control> controls = [];

   public event EventHandler<EnableArgs>? Enable;

   public HookStatus HookTextChanged()
   {
      switch (control)
      {
         case TextBoxBase textBoxBase:
            textBoxBase.TextChanged += (_, _) => Evaluate(textBoxBase, new EventTriggered.TextChanged(textBoxBase.Text));
            break;
         default:
            return HookStatus.UnsupportedType;
      }

      return HookStatus.Hooked;
   }

   public HookStatus HookMessageShown()
   {
      switch (control)
      {
         case UiAction uiAction:
            uiAction.MessageShown += (_, e) => Evaluate(uiAction, new EventTriggered.MessageShown(e));
            break;
         default:
            return HookStatus.UnsupportedType;
      }

      return HookStatus.Hooked;
   }

   public HookStatus HookSelectedIndexChanged()
   {
      switch (control)
      {
         case ListView listView:
            listView.SelectedIndexChanged += (_, _) =>
            {
               Evaluate(listView, new EventTriggered.ListViewSelectedIndexChanged(listView.SelectedItemWithIndex()));
            };
            break;
         case ListBox listBox:
            listBox.SelectedIndexChanged += (_, _) => Evaluate(listBox, new EventTriggered.ListBoxSelectedIndexChanged(listBox.SelectedIndex()));
            break;
         default:
            return HookStatus.UnsupportedType;
      }

      return HookStatus.Hooked;
   }

   protected void Evaluate(Control hookedControl, EventTriggered eventTriggered)
   {
      if (Enable is not null)
      {
         foreach (var (key, involvedControl) in controls)
         {
            var args = new EnableArgs(involvedControl, key, eventTriggered);
            Enable.Invoke(hookedControl, args);
            if (args.Cancel)
            {
               break;
            }

            involvedControl.Enabled = args.Enabled;
         }
      }
   }

   public Control this[string key]
   {
      get => controls[key];
      set => controls[key] = value;
   }

   public bool ContainsKey(string key) => controls.ContainsKey(key);

   public Hash<string, Control> GetHash() => controls;

   public HashInterfaceMaybe<string, Control> Items => new(controls);
}