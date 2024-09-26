using System.Collections;

namespace Core.Markup.Rtf.Markup.Parsers;

public class Specifiers : IEnumerable<Specifier>
{
   protected List<Specifier> specifiers = [];

   public int SourceLength { get; set; }

   public void Add(Specifier specifier) => specifiers.Add(specifier);

   public void Add(Specifiers specifiers) => this.specifiers.AddRange(specifiers);

   public IEnumerator<Specifier> GetEnumerator() => specifiers.GetEnumerator();

   IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}