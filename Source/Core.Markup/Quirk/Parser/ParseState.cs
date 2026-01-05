using System.Text;
using Core.DataStructures;
using Core.Monads;

namespace Core.Markup.Quirk.Parser;

public class ParseState(string input)
{
   protected static string[] getLines(string input) => input.Split(["\r\n", "\r", "\n"], StringSplitOptions.TrimEntries);

   protected string[] lines = getLines(input);
   protected MaybeStack<string> tags = [];
   protected StringBuilder builder = new();

   public string Line => lines[Index];

   public int Index { get; set; }

   public void Advance(int amount = 1) => Index += amount;

   public void PushTag(string tag) => tags.Push(tag);

   public Maybe<string> PopTag() => tags.Pop();

   public bool More => Index < lines.Length;

   public void Append(string text) => builder.Append(text);
}