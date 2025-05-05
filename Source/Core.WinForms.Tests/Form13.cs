using Core.Computers;
using Core.WinForms.Consoles;
using Core.WinForms.Controls;

namespace Core.WinForms.Tests;

public partial class Form13 : Form
{
   protected FileChanges fileChanges = new(@"C:\Temp", true);
   protected ExRichTextBox textBox = new() { Dock = DockStyle.Fill };
   protected TextBoxConsole console;

   public Form13()
   {
      InitializeComponent();

      Controls.Add(textBox);

      console = new TextBoxConsole(this, textBox);
      var writer = console.Writer();

      fileChanges.Changed.Handler = fileChange =>
      {
         switch (fileChange)
         {
            case FileChange.Changed changed:
               textBox.Do(() => writer.WriteLine($"File changed: {changed.File.NameExtension}"));
               break;
            case FileChange.Created created:
               textBox.Do(() => writer.WriteLine($"File created: {created.File.NameExtension}"));
               break;
            case FileChange.Deleted deleted:
               textBox.Do(() => writer.WriteLine($"File deleted: {deleted.File.NameExtension}"));
               break;
            case FileChange.Error error:
               textBox.Do(() => writer.WriteLine($"Error: {error.Exception.Message}"));
               break;
            case FileChange.Renamed renamed:
               textBox.Do(() => writer.WriteLine($"File renamed: {renamed.OldFile.NameExtension} to {renamed.NewFile.NameExtension}"));
               break;
         }
      };
      fileChanges.Enabled = true;
   }
}