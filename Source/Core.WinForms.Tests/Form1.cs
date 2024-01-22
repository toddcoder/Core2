using Core.WinForms.Controls;

namespace Core.WinForms.Tests;

public partial class Form1 : Form
{
   public Form1()
   {
      InitializeComponent();

      var uiButton1 = new UiAction(this);
      uiButton1.DefaultButton("button 1");
      uiButton1.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 0);
      AcceptButton = uiButton1;
      uiButton1.Click += (_, _) => uiButton1.Success("OK");
      uiButton1.ClickText = "OK";

      var uiButton2 = new UiAction(this) { ButtonType = UiActionButtonType.Cancel };
      uiButton2.CancelButton("button 2");
      uiButton2.SetUpInTableLayoutPanel(tableLayoutPanel, 2, 1);
      CancelButton = uiButton2;
      uiButton2.Click += (_, _) => uiButton2.Success("Cancel");
      uiButton2.ClickText = "Cancel";
   }
}