using System.Text.RegularExpressions;

namespace Core.Matching;

public record PatternKey(string Regex, RegexOptions Options);