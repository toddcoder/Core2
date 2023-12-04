namespace Core.WinForms.Tests
{
   partial class Form1
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
         button1 = new Button();
         button2 = new Button();
         button3 = new Button();
         panel1 = new Panel();
         imageList1 = new ImageList(components);
         panel2 = new Panel();
         textBox1 = new TextBox();
         panel3 = new Panel();
         panel4 = new Panel();
         button4 = new Button();
         panel5 = new Panel();
         tableLayoutPanel1 = new TableLayoutPanel();
         SuspendLayout();
         // 
         // button1
         // 
         button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
         button1.Location = new Point(836, 14);
         button1.Margin = new Padding(4, 3, 4, 3);
         button1.Name = "button1";
         button1.Size = new Size(88, 27);
         button1.TabIndex = 0;
         button1.Text = "button1";
         button1.UseVisualStyleBackColor = true;
         button1.Click += button1_Click;
         // 
         // button2
         // 
         button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
         button2.Location = new Point(836, 47);
         button2.Margin = new Padding(4, 3, 4, 3);
         button2.Name = "button2";
         button2.Size = new Size(88, 27);
         button2.TabIndex = 1;
         button2.Text = "button2";
         button2.UseVisualStyleBackColor = true;
         button2.Click += button2_Click;
         // 
         // button3
         // 
         button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
         button3.Location = new Point(836, 81);
         button3.Margin = new Padding(4, 3, 4, 3);
         button3.Name = "button3";
         button3.Size = new Size(88, 27);
         button3.TabIndex = 2;
         button3.Text = "button3";
         button3.UseVisualStyleBackColor = true;
         button3.Click += button3_Click;
         // 
         // panel1
         // 
         panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
         panel1.Location = new Point(14, 27);
         panel1.Margin = new Padding(4, 3, 4, 3);
         panel1.Name = "panel1";
         panel1.Size = new Size(816, 69);
         panel1.TabIndex = 3;
         // 
         // imageList1
         // 
         imageList1.ColorDepth = ColorDepth.Depth8Bit;
         imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
         imageList1.TransparentColor = Color.Transparent;
         imageList1.Images.SetKeyName(0, "AbstractAssociation.png");
         // 
         // panel2
         // 
         panel2.Location = new Point(14, 175);
         panel2.Margin = new Padding(4, 3, 4, 3);
         panel2.Name = "panel2";
         panel2.Size = new Size(23, 23);
         panel2.TabIndex = 4;
         // 
         // textBox1
         // 
         textBox1.Location = new Point(44, 175);
         textBox1.Margin = new Padding(4, 3, 4, 3);
         textBox1.Name = "textBox1";
         textBox1.Size = new Size(874, 23);
         textBox1.TabIndex = 5;
         // 
         // panel3
         // 
         panel3.Location = new Point(14, 205);
         panel3.Margin = new Padding(4, 3, 4, 3);
         panel3.Name = "panel3";
         panel3.Size = new Size(905, 23);
         panel3.TabIndex = 6;
         // 
         // panel4
         // 
         panel4.Location = new Point(14, 235);
         panel4.Margin = new Padding(4, 3, 4, 3);
         panel4.Name = "panel4";
         panel4.Size = new Size(905, 36);
         panel4.TabIndex = 7;
         // 
         // button4
         // 
         button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
         button4.Location = new Point(836, 114);
         button4.Margin = new Padding(4, 3, 4, 3);
         button4.Name = "button4";
         button4.Size = new Size(88, 27);
         button4.TabIndex = 8;
         button4.Text = "button4";
         button4.UseVisualStyleBackColor = true;
         button4.Click += button4_Click;
         // 
         // panel5
         // 
         panel5.Location = new Point(569, 118);
         panel5.Margin = new Padding(4, 3, 4, 3);
         panel5.Name = "panel5";
         panel5.Size = new Size(47, 46);
         panel5.TabIndex = 9;
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Location = new Point(14, 296);
         tableLayoutPanel1.Name = "tableLayoutPanel1";
         tableLayoutPanel1.RowCount = 2;
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Size = new Size(904, 201);
         tableLayoutPanel1.TabIndex = 10;
         // 
         // Form1
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(938, 524);
         Controls.Add(tableLayoutPanel1);
         Controls.Add(panel5);
         Controls.Add(button4);
         Controls.Add(panel4);
         Controls.Add(panel3);
         Controls.Add(textBox1);
         Controls.Add(panel2);
         Controls.Add(panel1);
         Controls.Add(button3);
         Controls.Add(button2);
         Controls.Add(button1);
         Margin = new Padding(4, 3, 4, 3);
         Name = "Form1";
         Text = "Form1";
         ResumeLayout(false);
         PerformLayout();
      }

      #endregion

      private System.Windows.Forms.Button button1;
      private System.Windows.Forms.Button button2;
      private System.Windows.Forms.Button button3;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.ImageList imageList1;
      private System.Windows.Forms.Panel panel2;
      private System.Windows.Forms.TextBox textBox1;
      private System.Windows.Forms.Panel panel3;
      private System.Windows.Forms.Panel panel4;
      private System.Windows.Forms.Button button4;
      private System.Windows.Forms.Panel panel5;
      private TableLayoutPanel tableLayoutPanel1;
   }
}

