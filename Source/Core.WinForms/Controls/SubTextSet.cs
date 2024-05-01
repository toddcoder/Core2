﻿namespace Core.WinForms.Controls;

public class SubTextSet
{
   protected SubText subText;
   protected ISubTextHost subTextHost;

   internal SubTextSet(SubText subText, ISubTextHost subTextHost)
   {
      this.subText = subText;
      this.subTextHost = subTextHost;
   }

   public SubTextSet Text(string text)
   {
      subText.Text = text;
      return this;
   }

   public SubTextSet X(int x)
   {
      subText.X = x;
      return this;
   }

   public SubTextSet Y(int y)
   {
      subText.Y = y;
      return this;
   }

   public SubTextSet Font(string fontName, float fontSize, FontStyle fontStyle = System.Drawing.FontStyle.Regular)
   {
      subText.FontName = fontName;
      subText.FontSize = fontSize;
      subText.FontStyle = fontStyle;

      return this;
   }

   public SubTextSet FontName(string fontName)
   {
      subText.FontName = fontName;
      return this;
   }

   public SubTextSet FontSize(float fontSize)
   {
      subText.FontSize = fontSize;
      return this;
   }

   public SubTextSet FontStyle(FontStyle fontStyle)
   {
      subText.FontStyle = fontStyle;
      return this;
   }

   public SubTextSet ForeColor(Color foreColor)
   {
      subText.ForeColor = foreColor;
      return this;
   }

   public SubTextSet BackColor(Color backColor)
   {
      subText.BackColor = backColor;
      return this;
   }

   public SubTextSet Outline(bool outline = true)
   {
      subText.Outline = outline;
      return this;
   }

   public SubTextSet Invert(bool invert = true)
   {
      subText.Invert = invert;
      return this;
   }

   public SubTextSet TransparentBackground(bool transparentBackground = false)
   {
      subText.TransparentBackground = transparentBackground;
      return this;
   }

   public SubTextSet IncludeFloor(bool includeFloor = true)
   {
      subText.IncludeFloor = includeFloor;
      return this;
   }

   public SubTextSet IncludeCeiling(bool includeCeiling = true)
   {
      subText.IncludeCeiling = includeCeiling;
      return this;
   }

   public SubTextSet Exclude(bool excludeFloor = true, bool excludeCeiling = true) => IncludeFloor(!excludeFloor).IncludeCeiling(!excludeCeiling);

   public SubTextSet MiniInverted(bool excludeFloor = true, bool excludeCeiling = true) => FontSize(8).Invert().Exclude(excludeFloor, excludeCeiling);

   public SubTextSet MiniInverted(CardinalAlignment alignment, bool excludeFloor = true, bool excludeCeiling = true)
   {
      return MiniInverted(excludeFloor, excludeCeiling).Alignment(alignment);
   }

   public SubTextSet SquareFirstCharacter(bool squareFirstCharacter = true)
   {
      subText.SquareFirstCharacter = squareFirstCharacter;
      return this;
   }

   public SubTextSet Transparency(SubTextTransparency transparency)
   {
      subText.Transparency = transparency;
      return this;
   }

   protected Size getTextSize()
   {
      var text = subText.Text;
      using var font = new Font(subText.FontName, subText.FontSize, subText.FontStyle);
      return TextRenderer.MeasureText(text, font);
   }

   public SubTextSet GoToUpperLeft(int margin)
   {
      subText.SetAlignment(CardinalAlignment.NorthWest);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToUpperRight(int margin)
   {
      subText.SetAlignment(CardinalAlignment.NorthEast);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToLowerLeft(int margin)
   {
      subText.SetAlignment(CardinalAlignment.SouthWest);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToLowerRight(int margin)
   {
      subText.SetAlignment(CardinalAlignment.SouthEast);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToMiddleLeft(int margin)
   {
      subText.SetAlignment(CardinalAlignment.West);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToMiddleRight(int margin)
   {
      subText.SetAlignment(CardinalAlignment.East);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToTop(int margin)
   {
      subText.SetAlignment(CardinalAlignment.North);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet GoToBottom(int margin)
   {
      subText.SetAlignment(CardinalAlignment.South);
      subText.SetMargin(margin);

      return this;
   }

   public SubTextSet Alignment(CardinalAlignment alignment)
   {
      subText.SetAlignment(alignment);
      return this;
   }

   public SubTextSet Margin(int margin)
   {
      subText.SetMargin(margin);
      return this;
   }

   public SubTextSet Small() => FontSize(8);

   public SubTextSet Italic() => FontStyle(System.Drawing.FontStyle.Italic);

   public SubTextSet Bold() => FontStyle(System.Drawing.FontStyle.Bold);

   public SubTextSet Option(SubTextOption option)
   {
      subText.Option = option;
      return this;
   }

   public SubTextSet LeftOf(SubText rightSubText, int margin = 2)
   {
      subText.LeftOf(rightSubText, margin);
      return this;
   }

   public SubTextSet RightOf(SubText leftSubText, int margin = 2)
   {
      subText.RightOf(leftSubText, margin);
      return this;
   }

   public SubTextSet RightOfLegend(CardinalAlignment alignment = CardinalAlignment.NorthEast, int margin = 2)
   {
      if (subTextHost.CurrentLegend is (true, var legend))
      {
         return IncludeFloor(false).IncludeCeiling(false).RightOf(legend, margin);
      }
      else
      {
         return IncludeFloor(false).IncludeCeiling(false).Alignment(alignment);
      }
   }

   [Obsolete("Use SubText or ClickableSubText")]
   public SubText End => subText;

   public SubText SubText
   {
      get
      {
         subTextHost.Refresh();
         return subText;
      }
   }

   public ClickableSubText ClickableSubText => (ClickableSubText)subText;
}