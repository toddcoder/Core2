using Core.Collections;
using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public class ChooserSet(Chooser chooser)
{
   public ChooserSet Title(string title)
   {
      chooser.Title = title;
      return this;
   }

   public ChooserSet SizeToText(bool sizeToText)
   {
      chooser.SizeToText = sizeToText;
      return this;
   }

   protected void workingOn()
   {
      if (chooser.Working)
      {
         chooser.UiAction.Working = "working";
      }
   }

   protected void workingOff()
   {
      if (chooser.Working)
      {
         chooser.UiAction.Working = nil;
      }
   }

   public ChooserSet Sorting(ChooserSorting sorting)
   {
      chooser.Sorting = sorting;
      return this;
   }

   public ChooserSet Choices(StringHash choices)
   {
      try
      {
         workingOn();
         chooser.Choices = choices;
         return this;
      }
      finally
      {
         workingOff();
      }
   }

   public ChooserSet Choices(params string[] choices)
   {
      try
      {
         workingOn();
         chooser.Choices = choices.ToStringHash(c => c, c => c);
         return this;
      }
      finally
      {
         workingOff();
      }
   }

   public ChooserSet Choices(params (string key, string value)[] choices)
   {
      try
      {
         workingOn();
         chooser.Choices = choices.ToStringHash();
         return this;
      }
      finally
      {
         workingOff();
      }
   }

   public ChooserSet Choices(IEnumerable<string> choices) => Choices([.. choices]);

   public ChooserSet ForeColor(Color foreColor)
   {
      chooser.ChoiceForeColor = foreColor;
      return this;
   }

   public ChooserSet BackColor(Color backColor)
   {
      chooser.ChoiceBackColor = backColor;
      return this;
   }

   public ChooserSet NilItem(Maybe<string> _firstItem)
   {
      chooser.NilItem = _firstItem;
      return this;
   }

   public ChooserSet ModifyTitle(bool modifyTitle)
   {
      chooser.ModifyTitle = modifyTitle;
      return this;
   }

   public ChooserSet EmptyTitle(string emptyTitle)
   {
      chooser.EmptyTitle = emptyTitle;
      return this;
   }

   public ChooserSet Working(bool working = true)
   {
      chooser.Working = working;
      return this;
   }

   public ChooserSet AutoSizeText(bool autoSizeText)
   {
      chooser.AutoSizeText = autoSizeText;
      return this;
   }

   public ChooserSet MultiChoice(bool multiChoice)
   {
      chooser.MultiChoice = multiChoice;
      chooser.NilItem = nil;
      return this;
   }

   public ChooserSet MultiChoice(IEnumerable<Chosen> chosenEnumerable)
   {
      chooser.MultiChoice = true;
      chooser.NilItem = nil;
      chooser.ChosenSet.AddRange(chosenEnumerable);

      return this;
   }

   public ChooserSet AutoClose(bool autoClose = true)
   {
      chooser.AutoClose = autoClose;
      chooser.NilItem = nil;
      return this;
   }

   public ChooserSet CheckBoxes(bool checkBoxes = true)
   {
      chooser.CheckBoxes = checkBoxes;
      return this;
   }

   public ChooserSet FlyUp(bool flyUp = true)
   {
      chooser.FlyUp = flyUp;
      return this;
   }

   public Maybe<Chosen> Choose()
   {
      var _chosen = chooser.Open();
      if (chooser.ModifyTitle)
      {
         if (_chosen is (true, var chosen))
         {
            chooser.UiAction.Success(chosen.Key);
            if (chooser.SizeToText)
            {
               chooser.UiAction.SizeToText();
            }
         }
         else
         {
            chooser.UiAction.Message(chooser.EmptyTitle);
         }
      }

      return _chosen;
   }

   public Chooser Chooser() => chooser;
}