using Core.Strings.Emojis;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Controls;

public partial class Lister : UserControl
{
   protected readonly string plus = Emoji.X.Image();
   protected ControlContainer<UiAction> list = ControlContainer<UiAction>.HorizontalContainer();
   protected UiAction uiAdd = new();

   public event EventHandler<ListerArgs>? ListItemClick;

   public Lister()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f + 50;
      _ = builder.Row + 100f;
      builder.SetUp();

      (builder + list).Next();
      (builder + uiAdd).Row();
   }

   public void Add(string text)
   {
      var uiAction = new UiAction { UseEmojis = false };
      uiAction.SubText(plus).Set.MiniInverted(CardinalAlignment.NorthEast);
      var uiActionIndex = list.Count;
      uiAction.Display(text, Color.White, Color.CadetBlue);
      uiAction.Click += (_, _) => ListItemClick?.Invoke(this, new ListerArgs(uiActionIndex));
      list.Add(uiAction);
      uiAction.ZeroOut();
   }
}