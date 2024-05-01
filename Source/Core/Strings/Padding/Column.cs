using Core.Monads;
using static Core.Monads.MonadFunctions;

namespace Core.Strings.Padding;

public class Column(string columnText)
{
   public string ColumnText => columnText;

   public Maybe<Justification> Justification { get; set; } = nil;
}