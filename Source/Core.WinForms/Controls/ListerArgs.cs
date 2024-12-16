namespace Core.WinForms.Controls;

public class ListerArgs(int index) : EventArgs
{
   public int Index => index;
}