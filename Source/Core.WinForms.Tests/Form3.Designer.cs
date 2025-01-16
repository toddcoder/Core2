namespace Core.WinForms.Tests
{
   partial class Form3
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
         tableLayoutPanel = new TableLayoutPanel();
         listBox1 = new ListBox();
         tableLayoutPanel.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel
         // 
         tableLayoutPanel.ColumnCount = 2;
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Controls.Add(listBox1, 0, 0);
         tableLayoutPanel.Dock = DockStyle.Fill;
         tableLayoutPanel.Location = new Point(0, 0);
         tableLayoutPanel.Name = "tableLayoutPanel";
         tableLayoutPanel.RowCount = 2;
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Size = new Size(800, 450);
         tableLayoutPanel.TabIndex = 0;
         // 
         // listBox1
         // 
         listBox1.Dock = DockStyle.Fill;
         listBox1.FormattingEnabled = true;
         listBox1.ItemHeight = 15;
         listBox1.Location = new Point(3, 3);
         listBox1.Name = "listBox1";
         listBox1.Size = new Size(394, 219);
         listBox1.TabIndex = 0;
         // 
         // Form3
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 450);
         Controls.Add(tableLayoutPanel);
         Name = "Form3";
         Text = "Form3";
         WindowState = FormWindowState.Maximized;
         tableLayoutPanel.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel;
      private ListBox listBox1;
   }
}