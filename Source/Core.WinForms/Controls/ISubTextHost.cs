﻿using Core.Monads;

namespace Core.WinForms.Controls;

public interface ISubTextHost
{
   Maybe<SubText> CurrentLegend { get; }

   bool UseEmojis { get; }

   void Refresh();
}