using Core.Matching;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Emojis;

public static class EmojiExtensions
{
   public static string Image(this Emoji emoji) => emoji switch
   {
      Emoji.Arrow => "⇒",
      Emoji.Check => "✔",
      Emoji.X => "✘",
      Emoji.Dot => "•",
      Emoji.Degree => "°",
      Emoji.Copyright => "©",
      Emoji.Pilcrow => "¶",
      Emoji.Diamond => "♦",
      Emoji.DoubleLeft => "«",
      Emoji.DoubleRight => "»",
      Emoji.Times => "✖",
      Emoji.Divide => "➗",
      Emoji.PawsLeft => "„",
      Emoji.PawsRight => "“",
      Emoji.NotEqual => "≠",
      Emoji.Error => "ℯ",
      Emoji.Ellipsis => "…",
      Emoji.Hourglass => "⧖",
      Emoji.Empty => "Ø",
      Emoji.LeftAngle => "〈",
      Emoji.RightAngle => "〉",
      Emoji.Locked => "🔒",
      Emoji.Unlocked => "🔓",
      Emoji.Text => "🗛",
      Emoji.Format => "ƒ",
      Emoji.Copy => "❏",
      Emoji.Paste => "📋",
      Emoji.Cut => "✄",
      Emoji.Bar => "║",
      Emoji.No => "🚫",
      Emoji.Refresh => "🗘",
      Emoji.CheckBox => "☒",
      Emoji.UncheckBox => "☐",
      Emoji.Plus => "➕",
      Emoji.Minus => "➖",
      Emoji.Not => "¬",
      Emoji.Vertical3 => "⋮",
      Emoji.Vertical4 => "⁞",
      Emoji.Vertical6 => "⸽",
      Emoji.Stop => "🛑",
      Emoji.BigX => "╳",
      _ => ""
   };

   public static string EmojiSubstitutions(this string text)
   {
      if (text.Matches("-(< '//') /('//' /([/w '-']+) '.'?); f") is (true, var result))
      {
         foreach (var match in result)
         {
            Maybe<string> _replacement = match.SecondGroup switch
            {
               "arrow" => "⇒",
               "check" => "✔",
               "x" => "✘",
               "dot" => "•",
               "degree" => "°",
               "copyright" => "©",
               "pilcrow" => "¶",
               "diamond" => "♦",
               "double-left" or "2left" => "«",
               "double-right" or "2right" => "»",
               "times" => "✖",
               "divide" => "➗",
               "paws-left" => "„",
               "paws-right" => "“",
               "not-equal" => "≠",
               "error" => "ℯ",
               "ellipsis" => "…",
               "hourglass" => "⧖",
               "empty" => "∅",
               "left-angle" => "〈",
               "right-angle" => "〉",
               "locked" => "🔐",
               "unlocked" => "🔓",
               "text" => "🗛",
               "format" => "ƒ",
               "copy" => "❏",
               "paste" => "📋",
               "cut" => "✄",
               "bar" => "║",
               "no" => "🚫",
               "refresh" => "🗘",
               "checkbox" => "☒",
               "uncheckbox" => "☐",
               "plus" => "➕",
               "minus" => "➖",
               "not" => "¬",
               "vertical3" or "v3" => "⋮",
               "vertical4" or "v4" => "⁞",
               "vertical6" or "v6" => "⸽",
               "stop" => "🛑",
               "big-x" => "╳",
               _ => nil
            };
            if (_replacement is (true, var replacement))
            {
               match.FirstGroup = replacement;
            }
         }

         return result.ToString().Replace("//", "/");
      }
      else
      {
         return text;
      }
   }
}