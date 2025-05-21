using System.Text.RegularExpressions;

namespace Core.Matching;

public readonly record struct PatternKey(string Regex, RegexOptions Options);