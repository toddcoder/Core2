namespace Core.WinForms.Controls;

public class EnableArgs(Control control, string key, EventTriggered eventTriggered) : EventArgs
{
   public Control Control => control;

   public string Key => key;

   public EventTriggered EventTriggered => eventTriggered;

   public bool Enabled { get; set; }

   public bool Cancel { get; set; }
}