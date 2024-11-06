using Core.Objects;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form4 : Form
{
   protected UiActionContainer container = [];
   protected UiAction uiAdd = new();
   protected UiAction uiRemove = new();
   protected UiAction uiInsert = new();
   protected ExTextBox textCaption = new();
   protected UiAction uiIndexes = new() { ClickGlyph = true };

   public Form4()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col * 5 * 20f;
      _ = builder.Row + 60 + 60 + 100f;
      builder.SetUp();

      uiAdd.Button("Add");
      uiAdd.Click += (_, _) =>
      {
         if (textCaption.TextLength > 0)
         {
            container.Add(textCaption.Text);
         }
      };
      uiAdd.ClickText = "Add UiAction";

      uiRemove.Button("Remove");
      uiRemove.Click += (_, _) =>
      {
         if (textCaption.TextLength > 0)
         {
            container.Remove(textCaption.Text);
         }
      };
      uiRemove.ClickText = "Remove UiAction";

      uiInsert.Button("Insert");
      uiInsert.Click += (_, _) =>
      {
         if (textCaption.TextLength > 0 && uiIndexes.NonNullText.Maybe().Int32() is (true, var index))
         {
            container.Insert(index, textCaption.Text);
         }
      };
      uiInsert.ClickText = "Insert UiAction";

      textCaption.Shortcut(Keys.Control | Keys.R, _ => textCaption.Text = "foobar");

      uiIndexes.NoStatus("indexes");
      uiIndexes.Click += (_, _) =>
      {
         var _chosen = uiIndexes.Choose("Indexes").Choices(getIndexes()).Choose();
         if (_chosen is (true, var chosen))
         {
            uiIndexes.Success(chosen.Value);
         }

         return;

         IEnumerable<string> getIndexes() => Enumerable.Range(0, container.Count).Select(i => i.ToString());
      };
      uiInsert.ClickText = "Select index";

      (builder + container).SpanCol(5).Row();

      (builder + uiAdd).Next();
      (builder + uiRemove).Next();
      (builder + uiInsert).Next();
      (builder + textCaption).Next();

      (builder + uiIndexes).Row();

      container.AddRange("alfa", "bravo", "charlie", "delta", "echo", "foxtrot");
   }
}