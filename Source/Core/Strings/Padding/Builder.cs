using System.Collections.Generic;

namespace Core.Strings.Padding;

public class Builder
{
   public static HeaderBuilder operator +(Builder builder, string columnText)
   {
      var columnBuilder = new HeaderBuilder(columnText);
      builder.AddColumnBuilder(columnBuilder);

      return columnBuilder;
   }

   protected List<HeaderBuilder> headerBuilders = [];

   public Builder AddColumnBuilder(HeaderBuilder headerBuilder)
   {
      headerBuilders.Add(headerBuilder);
      return this;
   }
}