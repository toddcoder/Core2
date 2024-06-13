using Core.Lists;
using Core.Matching;
using Core.Numbers;
using Core.Strings;
using static Core.Monads.AttemptFunctions;

namespace Core.WinForms.Controls;

public class UiActionTextCanvas : UserControl
{
   protected string fontName;
   protected float fontSize;
   protected List<TextItem> textItems = [];
   protected int padding = 2;

   public UiActionTextCanvas(string fontName = "Consolas", float fontSize = 12f)
   {
      this.fontName = fontName;
      this.fontSize = fontSize;

      Resize += (_, _) => Refresh();
   }

   public void Clear() => textItems.Clear();

   public new int Padding
   {
      get => padding;
      set => padding = value;
   }

   public void Write(string text)
   {
      foreach (var rawSegment in text.Unjoin(@"-(< '\')'|'; f"))
      {
         var segment = rawSegment.Replace(@"\|", "|");

         Bits32<FontStyle> fontStyle = FontStyle.Regular;
         var foreColor = SystemColors.ControlText;
         var backColor = SystemColors.Control;

         if (segment.Matches("'[' /(-[']']+)']' $; f").Map(r => r.FirstGroup) is (true, var specifierList))
         {
            foreach (var specifier in specifierList.Unjoin("/s* ',' /s*; f"))
            {
               switch (specifier)
               {
                  case "b":
                     fontStyle[FontStyle.Bold] = true;
                     break;
                  case "i":
                     fontStyle[FontStyle.Italic] = true;
                     break;
                  default:
                  {
                     if (specifier.Matches("^ /('f' | 'b') ':' /(['A-Za-z']+) $; f").Map(r => (r.FirstGroup, r.SecondGroup)) is
                         (true, var (axis, colorName)))
                     {
                        var _color = tryTo(() => Color.FromName(colorName));
                        if (_color is (true, var color))
                        {
                           switch (axis)
                           {
                              case "f":
                                 foreColor = color;
                                 break;
                              case "b":
                                 backColor = color;
                                 break;
                           }
                        }
                     }

                     break;
                  }
               }
            }

            segment = segment.Drop(-(specifierList.Length + 2));
         }

         var textItem = new TextItem(segment, fontName, fontSize, fontStyle, foreColor, backColor, DrawOutline);
         textItems.Add(textItem);
      }
   }

   public void WriteLine(string text)
   {
      Write(text);
      if (textItems.LastItem() is (true, var textItem))
      {
         textItem.Line = true;
      }
   }

   public bool DrawOutline { get; set; }

   protected override void OnPaint(PaintEventArgs e)
   {
      base.OnPaint(e);

      var location = new Point(padding, padding);
      foreach (var textItem in textItems)
      {
         if (textItem.Render(e.Graphics, location, ClientSize, padding) is (true, var newLocation))
         {
            location = newLocation;
         }
         else
         {
            break;
         }
      }
   }
}