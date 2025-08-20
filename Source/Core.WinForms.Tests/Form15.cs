using Core.WinForms.Controls;

namespace Core.WinForms.Tests;

public partial class Form15 : Form
{
   protected UiAction uiDisplay = new();
   protected UiAction uiStandard = new();
   protected UiAction uiChannel = new();
   protected UiAction uiElapsedTime = new();

   public Form15()
   {
      InitializeComponent();

      uiDisplay.Message("ready");

      uiStandard.Button("Standard");
      uiStandard.Click += (_, _) => { };
      uiStandard.ClickText = "Using InvokeRequired/Invoke";

      uiChannel.Button("Channel");
      uiChannel.Click += (_, _) => { };
      uiChannel.ClickText = "Using UiActionChannel";

      uiElapsedTime.Message("ready");
   }
}