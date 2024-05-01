namespace Core.Strings;

public class CharacterSearcher(CharacterSearchParameter parameters)
{
   public bool Success => parameters == CharacterSearchParameter.Any;
}