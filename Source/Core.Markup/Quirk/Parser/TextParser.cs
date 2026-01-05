using System.Text.RegularExpressions;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Markup.Quirk.Parser;

public partial class TextParser : BaseParser
{
   public static void ParseText(ParseState state, string line)
   {
      var escaped = false;
      var bold = false;
      var italic = false;
      var raw = false;

      foreach (var ch in line)
      {
         switch (ch)
         {
            case '*' when escaped:
               state.Append("*");
               escaped = false;
               break;
            case '*' when bold:
            {
               if (state.PopTag() is (true, var tag))
               {
                  state.Append($"</{tag}>");
               }

               bold = false;
               break;
            }
            case '*':
               state.PushTag("strong");
               state.Append("<strong>");
               bold = true;
               break;
            case '_' when escaped:
               state.Append("_");
               escaped = false;
               break;
            case '_' when italic:
            {
               if (state.PopTag() is (true, var tag))
               {
                  state.Append($"</{tag}>");
               }

               italic = false;
               break;
            }
            case '_':
               state.PushTag("em");
               state.Append("<em>");
               italic = true;
               break;
            case '`' when escaped:
               state.Append("`");
               escaped = false;
               break;
            case '`' when raw:
            {
               if (state.PopTag() is (true, var tag))
               {
                  state.Append($"</{tag}>");
               }

               raw = false;
               break;
            }
            case '`':
               state.PushTag("code");
               state.Append("<code>");
               raw = true;
               break;
            default:
               if (ch == '\\' && !escaped)
               {
                  escaped = true;
               }
               else
               {
                  state.Append(ch.ToString());
                  escaped = false;
               }

               break;
         }
      }
   }

   [GeneratedRegex("^(.+)$")]
   public override partial Regex Regex();

   public override Optional<Unit> Parse(ParseState state, Token[] tokens)
   {
      var line = tokens[1].Text;
      ParseText(state, line);

      return unit;
   }
}