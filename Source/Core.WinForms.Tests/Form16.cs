using Core.Computers;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;
using static Core.Monads.MonadFunctions;

namespace Core.WinForms.Tests;

public partial class Form16 : Form
{
   protected string[] fileNames = [];
   protected UiMenuAction menu1 = new();
   protected UiMenuAction menu2 = new();
   protected TempMessage tmDisplay1 = new();
   protected UiAction uiSwitch = new();
   protected TempMessage tmBusy = new();
   protected TempProgress tmProgress = new();
   protected int value;

   public Form16()
   {
      fileNames = [.. ((FolderName)@"c:\Temp").Files.Take(20).Select(f => f.Name).Order()];
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

      uiSwitch.CheckBox("Busy", false);
      uiSwitch.Click += (_, _) =>
      {
         tmBusy.IsBusy = uiSwitch.BoxChecked;
         timer1.Enabled = uiSwitch.BoxChecked;
         tmProgress.Maximum = uiSwitch.BoxChecked ? 20 : nil;
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel1);
      _ = builder.Col + 200 + 400 + 200 + 100f;
      _ = builder.Row + 60 + 60 + 60 + 100f;
      builder.SetUp();

      (builder + menu1).Next();
      (builder + menu2).Next();
      (builder + tmDisplay1).Row();

      (builder + uiSwitch).Next();
      (builder + tmBusy).Row();

      (builder + tmProgress).SpanCol(4).Next();
   }

   protected void timer1_Tick(object sender, EventArgs e)
   {
      if (tmProgress.Maximum is (true, var maximum))
      {
         if (value < maximum)
         {
            tmProgress.Progress(fileNames[value++]);
         }
         else
         {
            value = 0;
            tmProgress.Maximum = nil;
            timer1.Enabled = false;
         }
      }
      else
      {
         tmProgress.Maximum = fileNames.Length;
      }
   }
}