using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form9 : Form
{
   protected LabelUrl luUrl1 = new("url 1");
   protected LabelUrl luUrl2 = new("url 2");

   public Form9()
   {
      InitializeComponent();

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col + 100f;
      _ = builder.Row + 60 + 60 + 100f;
      builder.SetUp();

      var uiSuccess = new UiAction();
      uiSuccess.ZeroOut();
      uiSuccess.Success("success");

      var uiFailure = new UiAction();
      uiFailure.ZeroOut();
      uiFailure.Failure("failure");

      luUrl1.Url = "http://google.com";
      luUrl2.Url = "http://google.com";
      luUrl2.AddUiActions(uiSuccess, uiFailure);

      (builder + luUrl1).Row();
      (builder + luUrl2).Row();
   }
}