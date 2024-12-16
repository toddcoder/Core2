using Core.WinForms.Controls;

namespace Core.WinForms.Tests;

public partial class Form10 : Form
{
   protected Lister lister = new();

   public Form10()
   {
      InitializeComponent();

      Controls.Add(lister);
      lister.Dock = DockStyle.Top;

      lister.Add("alfa");
      lister.Add("bravo");
      lister.Add("charlie");
   }
}