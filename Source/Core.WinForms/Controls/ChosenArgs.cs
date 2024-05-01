using Core.Collections;

namespace Core.WinForms.Controls;

public class ChosenArgs(Chosen chosen, Set<Chosen> chosenSet) : EventArgs
{
   public Chosen Chosen => chosen;

   public Set<Chosen> ChosenSet => chosenSet;
}