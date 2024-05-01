using System;

namespace Core.Strings;

[Flags]
public enum CharacterSearchParameter
{
   Any = 0,
   Letter = 1,
   NotLetter = 2,
   UpperCaseLetter = 4,
   NotUpperCaseLetter = 8,
   LowerCaseLetter = 16,
   NotLowerCaseLetter = 32,
   Digit = 64,
   NotDigit = 128,
   Punctuation = 256,
   NotPunctuation = 512,
   Whitespace = 1024,
   NotWhitespace = 2048
}