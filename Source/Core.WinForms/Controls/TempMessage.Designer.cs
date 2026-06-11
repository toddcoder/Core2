namespace Core.WinForms.Controls
{
   partial class TempMessage
   {
      /// <summary> 
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary> 
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         components = new System.ComponentModel.Container();
         timer = new System.Windows.Forms.Timer(components);
         SuspendLayout();
         // 
         // timer
         // 
         timer.Enabled = true;
         timer.Interval = 50;
         timer.Tick += timer_Tick;
         // 
         // TempMessage
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         Name = "TempMessage";
         ResumeLayout(false);
      }

      #endregion

      private System.Windows.Forms.Timer timer;
   }
}
