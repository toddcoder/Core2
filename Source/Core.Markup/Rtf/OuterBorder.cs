﻿using Core.Monads;

namespace Core.Markup.Rtf;

public class OuterBorder(BorderStyle borderStyle, float width, Maybe<ColorDescriptor> color)
{
   public BorderStyle BorderStyle => borderStyle;

   public float Width => width;

   public Maybe<ColorDescriptor> Color => color;
}