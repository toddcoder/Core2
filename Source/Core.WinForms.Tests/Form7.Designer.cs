namespace Core.WinForms.Tests
{
   partial class Form7
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

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         components = new System.ComponentModel.Container();
         tableLayoutPanel = new TableLayoutPanel();
         panel = new Panel();
         timer = new System.Windows.Forms.Timer(components);
         tableLayoutPanel.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel
         // 
         tableLayoutPanel.ColumnCount = 2;
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Controls.Add(panel, 0, 0);
         tableLayoutPanel.Dock = DockStyle.Fill;
         tableLayoutPanel.Location = new Point(0, 0);
         tableLayoutPanel.Name = "tableLayoutPanel";
         tableLayoutPanel.RowCount = 2;
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Size = new Size(1209, 837);
         tableLayoutPanel.TabIndex = 0;
         // 
         // panel
         // 
         panel.BackColor = Color.IndianRed;
         panel.Dock = DockStyle.Fill;
         panel.Location = new Point(3, 3);
         panel.Name = "panel";
         panel.Size = new Size(598, 412);
         panel.TabIndex = 0;
         // 
         // timer
         // 
         timer.Interval = 1000;
         timer.Tick += timer_Tick;
         // 
         // Form7
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(1209, 837);
         Controls.Add(tableLayoutPanel);
         Name = "Form7";
         StartPosition = FormStartPosition.CenterScreen;
         Text = "Form7";
         Load += Form7_Load;
         tableLayoutPanel.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel;
      private Panel panel;
      private System.Windows.Forms.Timer timer;
   }
}