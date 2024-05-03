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
         imageList1 = new ImageList(components);
         tableLayoutPanel = new TableLayoutPanel();
         SuspendLayout();
         // 
         // imageList1
         // 
         imageList1.ColorDepth = ColorDepth.Depth8Bit;
         imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
         imageList1.TransparentColor = Color.Transparent;
         imageList1.Images.SetKeyName(0, "AbstractAssociation.png");
         // 
         // tableLayoutPanel
         // 
         tableLayoutPanel.ColumnCount = 2;
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Dock = DockStyle.Fill;
         tableLayoutPanel.Location = new Point(0, 0);
         tableLayoutPanel.Name = "tableLayoutPanel";
         tableLayoutPanel.RowCount = 2;
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Size = new Size(938, 809);
         tableLayoutPanel.TabIndex = 0;
         // 
         // Form1
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(938, 809);
         Controls.Add(tableLayoutPanel);
         Margin = new Padding(4, 3, 4, 3);
         Name = "Form1";
         Text = "Form1";
         ResumeLayout(false);
      }

      #endregion
      private System.Windows.Forms.ImageList imageList1;
      private TableLayoutPanel tableLayoutPanel;
   }
}

