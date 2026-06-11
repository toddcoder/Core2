using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form16 : Form
{
   protected UiMenuAction menu1 = new();
   protected UiMenuAction menu2 = new();
   protected TempMessage tmDisplay1 = new();
   protected UiAction uiSwitch = new();
   protected TempMessage tmBusy = new();

   public Form16()
   {
      InitializeComponent();

      menu1.TextItem("Alfa (A or alpha)", text => menu1.Success(text));
      menu1.TextItem("Bravo (B or beta)", text => menu1.Success(text));
      menu1.Success("Menu");

      menu2.AlternateReadOnly("Greek", "Nato", "English");
      menu2.RequestAlternateMenuItems[0].Handler = _ =>
      {
         menu2.Choose("A", "B", "C").Then(letter =>
         {
            switch (letter)
            {
               case "A":
                  tmDisplay1.Display("Alpha");
                  break;
               case "B":
                  tmDisplay1.Display("Beta");
                  break;
               case "C":
                  tmDisplay1.Display("Kappa");
                  break;
            }
         });
      };
      menu2.RequestAlternateMenuItems[1].Handler = _ =>
      {
         menu2.Choose("A", "B", "C").Then(letter =>
         {
            switch (letter)
            {
               case "A":
                  tmDisplay1.Display("Alfa");
                  break;
               case "B":
                  tmDisplay1.Display("Bravo");
                  break;
               case "C":
                  tmDisplay1.Display("Charlie");
                  break;
            }
         });
      };
      menu2.RequestAlternateMenuItems[2].Handler = _ =>
      {
         menu2.Choose("A", "B", "C").Then(letter =>
         {
            switch (letter)
            {
               case "A":
                  tmDisplay1.Display("Ay");
                  break;
               case "B":
                  tmDisplay1.Display("Bee");
                  break;
               case "C":
                  tmDisplay1.Display("Cee");
                  break;
            }
         });
      };

      tmDisplay1.Display("display");

      uiSwitch.CheckBox("Busy", false);
      uiSwitch.Click += (_, _) => tmBusy.IsBusy = uiSwitch.BoxChecked;
      tmBusy.Display("not busy");

      var builder = new TableLayoutBuilder(tableLayoutPanel1);
      _ = builder.Col + 200 + 400 + 200 + 100f;
      _ = builder.Row + 60 + 60 + 100f;
      builder.SetUp();

      (builder + menu1).Next();
      (builder + menu2).Next();
      (builder + tmDisplay1).Row();

      (builder + uiSwitch).Next();
      (builder + tmBusy).Row();
   }
}