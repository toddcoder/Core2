using Core.Collections;
using Core.DataStructures;
using System.Text;
using Core.Markup.Xml;
using Core.Strings;

namespace Core.Markup.Html.Parser;

public class HtmlGatherer
{
   protected AutoStringHash<Set<StyleKeyValue>> styles = new(_ => [], true);
   protected StringBuilder body = new();
   protected StringBuilder gathering = new();
   protected ParsingStage stage = ParsingStage.Name;
   protected MaybeStack<string> tagStack = [];
   protected MaybeStack<bool> closedStack = [];
   protected string styleName = "";
   protected string styleKey = "";
   protected string attribute = "";
   protected string gathered = "";
   protected bool isEmpty;

   public void Gather()
   {
      gathered = gathering.ToString();
      isEmpty = gathered.IsEmpty() || gathered.IsWhitespace();
   }

   public ParsingStage Stage
   {
      get => stage;
      set => stage = value;
   }

   public bool IsEmpty => isEmpty;

   public string Gathered => gathered;

   public bool InStyle { get; set; }

   public bool Escaped { get; set; }

   public AutoStringHash<Set<StyleKeyValue>> Styles => styles;

   public StringBuilder Body => body;

   public void GatherCharacter(char character, bool inDefault = false)
   {
      if (inDefault && Escaped)
      {
         gathering.Append('/');
         gathering.Append(character);
         Escaped = false;

         return;
      }

      if (Escaped)
      {
         gathering.Append(character);
         Escaped = false;
      }

      switch (stage)
      {
         case ParsingStage.Name or ParsingStage.Tag or ParsingStage.StyleName:
            if (char.IsLetter(character) || char.IsDigit(character) || character == '-')
            {
               gathering.Append(character);
            }

            break;
         case ParsingStage.Text:
            if (character != '\t' && character != '\r' && character != '\n')
            {
               gathering.Append(character);
            }

            break;
         default:
            gathering.Append(character);
            break;
      }
   }

   public void AppendToGathering(char character) => gathering.Append(character);

   public void BeginTag()
   {
      if (closedStack.Pop() is (true, false))
      {
         body.Append('>');
         closedStack.Push(true);
      }

      body.Append($"<{gathered}");
      tagStack.Push($"</{gathered}>");
      stage = ParsingStage.Tag;
      InStyle = false;
      gathering.Clear();
      closedStack.Push(false);
   }

   public void BeginStyle()
   {
      stage = ParsingStage.Style;
      InStyle = true;
      gathering.Clear();
   }

   public void BeginStyleName()
   {
      styleName = gathered;
      stage = ParsingStage.StyleName;
      gathering.Clear();
   }

   public void BeginStyleKey()
   {
      styleKey = gathered;
      stage = ParsingStage.StyleKey;
      gathering.Clear();
   }

   public void BeginStyleValue()
   {
      styles[styleName].Add(new StyleKeyValue(styleKey, gathered));
      stage = ParsingStage.StyleName;
      gathering.Clear();
   }

   public void BeginAttribute()
   {
      attribute = gathered;
      stage = ParsingStage.Attribute;
      gathering.Clear();
   }

   public void BeginText()
   {
      if (closedStack.Pop() is (true, false))
      {
         body.Append('>');
         closedStack.Push(true);
      }

      stage = ParsingStage.Text;
      gathering.Clear();
   }

   public void EndTag()
   {
      if (closedStack.Pop() is (true, false))
      {
         body.Append('>');
      }

      if (tagStack.Pop() is (true, var endTag))
      {
         body.Append(endTag);
         stage = ParsingStage.Tag;
      }
      else
      {
         stage = ParsingStage.Name;
      }

      gathering.Clear();
   }

   public void ClosedTag()
   {
      if (closedStack.Pop() is (true, false))
      {
         body.Append('>');
         closedStack.Push(true);
      }

      body.Append($"<{gathered} />");
      stage = ParsingStage.Tag;
      InStyle = false;
      gathering.Clear();
   }

   public void EndAttribute()
   {
      body.Append($" {attribute}=\"{MarkupTextHolder.Markupify(gathered, QuoteType.Double)}\"");
      stage = ParsingStage.Tag;
      gathering.Clear();
   }

   public void EndStyleName()
   {
      stage = ParsingStage.Style;
      gathering.Clear();
   }

   public void EndStyle()
   {
      stage = ParsingStage.Name;
      gathering.Clear();
   }

   protected string getText() => MarkupTextHolder.Markupify(gathered, QuoteType.Double);

   public void EndText()
   {
      body.Append(getText());
      gathering.Clear();
      stage = ParsingStage.Tag;
   }

   public void EndAll()
   {
      if (stage is ParsingStage.Text && gathering.Length > 0)
      {
         body.Append(getText());
      }

      while (tagStack.Pop() is (true, var tag))
      {
         body.Append(tag);
      }
   }

   public void Clear() => gathering.Clear();
}