namespace Core.Markup.Rtf;

public class FieldControlWord(int position, FieldType type) : Renderable
{
   protected int position = position;
   protected FieldType type = type;

   public int Position => position;

   public override string Render() => type switch
   {
      FieldType.Page => @"{\field{\*\fldinst PAGE }}",
      FieldType.NumPages => @"{\field{\*\fldinst NUMPAGES }}",
      FieldType.Date => @"{\field{\*\fldinst DATE }}",
      FieldType.Time => @"{\field{\*\fldinst TIME }}",
      _ => ""
   };
}