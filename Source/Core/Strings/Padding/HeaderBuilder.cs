namespace Core.Strings.Padding;

public class HeaderBuilder(string headerText)
{
   public string HeaderText => headerText;

   public Justification Justification { get; set; } = Justification.Left;
}