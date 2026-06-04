using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form16 : Form
{
   protected UiMenuAction menu1 = new();
   protected UiMenuAction menu2 = new();
   protected TempMessage tmDisplay = new();

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
                  tmDisplay.Display("Alpha");
                  break;
               case "B":
                  tmDisplay.Display("Beta");
                  break;
               case "C":
                  tmDisplay.Display("Kappa");
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
                  tmDisplay.Display("Alfa");
                  break;
               case "B":
                  tmDisplay.Display("Bravo");
                  break;
               case "C":
                  tmDisplay.Display("Charlie");
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
                  tmDisplay.Display("Ay");
                  break;
               case "B":
                  tmDisplay.Display("Bee");
                  break;
               case "C":
                  tmDisplay.Display("Cee");
                  break;
            }
         });
      };
        /*menu2.RequestAlternateMenuItems.Handler = indexLocation =>
        {
           var (index, location) = indexLocation;
           menu2.Choose("A", "B", "C").Then(letter =>
           {
              uiDisplay.ClearSubTexts();
              uiDisplay.SubText(location.ToString());
              switch (index)
              {
                 case 0:
                    switch (letter)
                    {
                       case "A":
                          uiDisplay.Success("Alpha");
                          break;
                       case "B":
                          uiDisplay.Success("Beta");
                          break;
                       case "C":
                          uiDisplay.Success("Kappa");
                          break;
                    }

                    break;
                 case 1:
                    switch (letter)
                    {
                       case "A":
                          uiDisplay.Success("Alfa");
                          break;
                       case "B":
                          uiDisplay.Success("Bravo");
                          break;
                       case "C":
                          uiDisplay.Success("Charlie");
                          break;
                    }

                    break;
                 case 2:
                    switch (letter)
                    {
                       case "A":
                          uiDisplay.Success("Ay");
                          break;
                       case "B":
                          uiDisplay.Success("Bee");
                          break;
                       case "C":
                          uiDisplay.Success("Cee");
                          break;
                    }

                    break;
              }
           });
        };*/

        tmDisplay.Display("display");

      var builder = new TableLayoutBuilder(tableLayoutPanel1);
      _ = builder.Col + 200 + 400 + 200 + 100f;
      _ = builder.Row + 60 + 100f;
      builder.SetUp();

      (builder + menu1).Next();
      (builder + menu2).Next();
      (builder + tmDisplay).Row();
   }
}