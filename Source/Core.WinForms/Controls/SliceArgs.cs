using Core.Strings;

namespace Core.WinForms.Controls;

public class SliceArgs(string text, params IEnumerable<Slice> slices) : EventArgs
{
   public string Text => text;

   public IEnumerable<Slice> Slices => slices;
}