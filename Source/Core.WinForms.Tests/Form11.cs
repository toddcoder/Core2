﻿using Core.Applications.Messaging;
using Core.WinForms.Controls;
using Core.WinForms.TableLayoutPanels;

namespace Core.WinForms.Tests;

public partial class Form11 : Form
{
   protected UiAction uiPublish = new();
   protected UiAction uiSubscribe = new();
   protected Subscriber<string> subscriber = new();

   public Form11()
   {
      InitializeComponent();

      var publisher = new Publisher<string>();

      uiPublish.Button("Publish");
      uiPublish.Click += (_, _) => publisher.Publish("user", Environment.UserName);
      uiPublish.ClickText = "Publish";

      uiSubscribe.Message("waiting");

      subscriber.Received.Handler = p =>
      {
         if (p.Topic == "user")
         {
            uiSubscribe.Success(p.Payload);
         }
      };

      var builder = new TableLayoutBuilder(tableLayoutPanel);
      _ = builder.Col * 2 * 50f;
      _ = builder.Row + 100f;
      builder.SetUp();

      (builder + uiPublish).Next();
      (builder + uiSubscribe).Row();
   }

   protected void Form11_FormClosing(object sender, FormClosingEventArgs e) => subscriber.Dispose();
}