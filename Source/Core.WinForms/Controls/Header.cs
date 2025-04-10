using System.Drawing.Drawing2D;
using Core.Applications.Messaging;
using Core.Collections;
using Core.Monads;
using Core.WinForms.TableLayoutPanels;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Controls;

public partial class Header : UserControl
{
   public static HeaderColumnBuilder operator +(Header header, string name) => new(name, header.headerColumns);

   protected HeaderColumns headerColumns = [];
   protected StringHash<UiAction> headers = [];

   public readonly MessageEvent<HeaderClickArgs> HeaderClick = new();

   public Header()
   {
      InitializeComponent();
   }

   public void Arrange()
   {
      headers.Clear();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      var setup = builder.Col;
      Maybe<DashStyle> needsStripe = nil;
      foreach (var (name, headerColumn) in headerColumns)
      {
         _ = setup + headerColumn.ColumnSize;
         var uiAction = headerColumn.UiAction();
         uiAction.LeftStripe = needsStripe;
         headers[name] = uiAction;
         needsStripe = DashStyle.Dash;

         uiAction.Click += (_, _) => HeaderClick.Invoke(new HeaderClickArgs(name, uiAction));
      }

      _ = builder.Row + 100f;
      builder.SetUp();

      foreach (var (name, _) in headerColumns)
      {
         (builder + headers[name]).Next();
      }
   }
}