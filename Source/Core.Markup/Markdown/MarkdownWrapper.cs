using Core.Collections;

namespace Core.Markup.Markdown;

public class MarkdownWrapper(string source)
{
   protected Memo<string, StringHash> styles = new Memo<string, StringHash>.Function(_ => new StringHash());
   protected Memo<string, string> variables = new Memo<string, string>.Function(v => v);

   public string this[string variable]
   {
      get => variables[variable];
      set => variables[variable] = value;
   }

   public string this[string style, string key]
   {
      get => styles[style].Maybe[key] | "";
      set => styles[style][key] = value;
   }
}