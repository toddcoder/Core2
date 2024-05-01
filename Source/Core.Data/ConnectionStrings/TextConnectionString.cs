﻿using Core.Collections;
using Core.Dates.DateIncrements;
using Core.Strings;

namespace Core.Data.ConnectionStrings;

public class TextConnectionString : IConnectionString
{
   protected string fileName;
   protected string header;
   protected string delimited;

   public TextConnectionString(Connection connection)
   {
      fileName = connection.Value("file");
      header = (connection.Items["header"] | "true") == "true" ? "YES" : "NO";
      delimited = connection.Value("delimited");
      delimited = delimited switch
      {
         "comma" => "CSVDelimited",
         "," => "CSVDelimited",
         "tab" => "TabDelimited",
         "\t" => "TabDelimited",
         _ => $"Delimited({delimited.Keep(1)})"
      };
   }

   public string ConnectionString => $"Provider=Microsoft.Jet.OLEDB.4.0; Data Source={fileName};" +
      $" Extended Properties=\"text;HDR={header};FMT={delimited}";

   public TimeSpan ConnectionTimeout => 30.Seconds();
}