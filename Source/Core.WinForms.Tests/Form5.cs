namespace Core.WinForms.Tests;

public partial class Form5 : Form
{
   public Form5()
   {
      InitializeComponent();

      uiAction1.AttachTo("Test action", exTextBox1);
   }
}