namespace Core.Markdown;

public class MultipleReplacementArg(string key)
{
   public string Key => key;

   public IEnumerable<string> Values { get; set; } = [];
}