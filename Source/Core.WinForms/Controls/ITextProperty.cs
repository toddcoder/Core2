namespace Core.WinForms.Controls;

public interface ITextProperty : ITextProvider, ITextReceiver
{
   new string Text { get; set; }
}