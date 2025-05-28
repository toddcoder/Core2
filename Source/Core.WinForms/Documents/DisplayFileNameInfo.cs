using Core.Computers;
using Core.Monads;

namespace Core.WinForms.Documents;

public record DisplayFileNameInfo(Maybe<FileName> File, string FormName, bool IsDirty, string DirtyCharacter);