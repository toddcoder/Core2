namespace Core.WinForms.TableLayoutPanels;

using SpanInfo = (Span span, int amount);
using AxisInfo = (Axis axis, int amount);
using BuilderSetup = (Setup setup, Axis axis, float amount);

public static class BuilderExtensions
{
   public static AxisInfo Col(this int amount) => (Axis.Column, amount);

   public static AxisInfo Row(this int amount) => (Axis.Row, amount);

   public static SpanInfo ColSpan(this int amount) => (Span.Column, amount);

   public static SpanInfo RowSpan(this int amount) => (Span.Row, amount);

   public static BuilderSetup ColPercent(this float amount) => (Setup.Percent, Axis.Column, amount);

   public static BuilderSetup ColPercent(this int amount) => ((float)amount).ColPercent();

   public static BuilderSetup ColPixels(this float amount) => (Setup.Pixels, Axis.Column, amount);

   public static BuilderSetup ColPixels(this int amount) => ((float)amount).ColPixels();

   public static BuilderSetup RowPercent(this float amount) => (Setup.Percent, Axis.Row, amount);

   public static BuilderSetup RowPercent(this int amount) => ((float)amount).RowPercent();

   public static BuilderSetup RowPixels(this float amount) => (Setup.Pixels, Axis.Row, amount);

   public static BuilderSetup RowPixels(this int amount) => ((float)amount).RowPixels();
}