using Core.Monads;
using Core.Objects;
using Core.Strings;

namespace Core.Configurations;

public static class ConfigurationExtensions
{
   public static Maybe<int> GetInt32(this IConfigurationItem item, string key) => item.GetValue(key).Map(i => i.Maybe().Int32());

   public static Maybe<double> GetDouble(this IConfigurationItem item, string key) => item.GetValue(key).Map(d => d.Maybe().Double());

   public static Maybe<bool> GetBoolean(this IConfigurationItem item, string key) => item.GetValue(key).Map(b => b.Maybe().Boolean());

   public static Maybe<byte[]> GetBytes(this IConfigurationItem item, string key) => item.GetValue(key).Map(s => s.FromBase64());

   public static Result<int> RequireInt32(this IConfigurationItem item, string key) => item.RequireValue(key).Map(i => i.Result().Int32());

   public static Result<double> RequireDouble(this IConfigurationItem item, string key) => item.RequireValue(key).Map(d => d.Result().Double());

   public static Result<bool> RequireBoolean(this IConfigurationItem item, string key) => item.RequireValue(key).Map(b => b.Result().Boolean());

   public static Result<byte[]> RequireBytes(this IConfigurationItem item, string key) => item.RequireValue(key).Map(s => s.FromBase64());

   public static int Int32At(this IConfigurationItem item, string key, int defaultValue = default) => item.ValueAt(key).Value().Int32(defaultValue);

   public static double DoubleAt(this IConfigurationItem item, string key, double defaultValue = default)
   {
      return item.ValueAt(key).Value().Double(defaultValue);
   }

   public static bool BooleanAt(this IConfigurationItem item, string key, bool defaultValue = default)
   {
      return item.ValueAt(key).Value().Boolean(defaultValue);
   }

   public static byte[] BytesAt(this IConfigurationItem item, string key) => item.ValueAt(key).FromBase64();
}