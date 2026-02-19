namespace Core.Markdown;

public class ScalarReplacementArg(string key)
{
   public string Key => key;

   public string Value { get; set; } = "";
}