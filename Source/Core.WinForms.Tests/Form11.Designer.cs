namespace Core.WinForms.Tests
{
   partial class Form11
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
         tableLayoutPanel.Controls.Add(listBox1, 1, 1);
         tableLayoutPanel.Dock = DockStyle.Fill;
         tableLayoutPanel.Location = new Point(0, 0);
         tableLayoutPanel.Name = "tableLayoutPanel";
         tableLayoutPanel.RowCount = 2;
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Size = new Size(1199, 337);
         tableLayoutPanel.TabIndex = 0;
         // 
         // listBox1
         // 
         listBox1.Dock = DockStyle.Fill;
         listBox1.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
         listBox1.FormattingEnabled = true;
         listBox1.ItemHeight = 14;
         listBox1.Location = new Point(602, 171);
         listBox1.Name = "listBox1";
         listBox1.Size = new Size(594, 163);
         listBox1.TabIndex = 0;
         // 
         // Form11
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(1199, 337);
         Controls.Add(tableLayoutPanel);
         Name = "Form11";
         Text = "Form11";
         tableLayoutPanel.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel;
      private ListBox listBox1;
   }
}