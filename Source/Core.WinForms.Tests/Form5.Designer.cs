namespace Core.WinForms.Tests
{
   partial class Form5
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
         tableLayoutPanel1 = new TableLayoutPanel();
         panel1 = new Panel();
         pictureBox1 = new PictureBox();
         tableLayoutPanel1.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(panel1, 0, 0);
         tableLayoutPanel1.Controls.Add(pictureBox1, 1, 1);
         tableLayoutPanel1.Dock = DockStyle.Fill;
         tableLayoutPanel1.Location = new Point(0, 0);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Size = new Size(800, 450);
         tableLayoutPanel1.TabIndex = 0;
         // 
         // panel1
         // 
         panel1.Dock = DockStyle.Fill;
         panel1.Location = new Point(3, 3);
         panel1.Name = "panel1";
         panel1.Size = new Size(394, 219);
         panel1.TabIndex = 0;
         // 
         // pictureBox1
         // 
         pictureBox1.Dock = DockStyle.Fill;
         pictureBox1.Location = new Point(403, 228);
         pictureBox1.Name = "pictureBox1";
         pictureBox1.Size = new Size(394, 219);
         pictureBox1.TabIndex = 1;
         pictureBox1.TabStop = false;
         pictureBox1.Paint += pictureBox1_Paint;
         // 
         // Form5
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 450);
         Controls.Add(tableLayoutPanel1);
         Name = "Form5";
         Text = "Form5";
         tableLayoutPanel1.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Panel panel1;
      private PictureBox pictureBox1;
   }
}