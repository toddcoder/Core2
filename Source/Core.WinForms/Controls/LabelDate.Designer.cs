namespace Core.WinForms.Controls
{
   partial class LabelDate
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
         tableLayoutPanel = new TableLayoutPanel();
         dateTimePicker = new CoreDateTimePicker();
         tableLayoutPanel.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel
         // 
         tableLayoutPanel.ColumnCount = 2;
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Controls.Add(dateTimePicker, 0, 1);
         tableLayoutPanel.Dock = DockStyle.Fill;
         tableLayoutPanel.Location = new Point(0, 0);
         tableLayoutPanel.Name = "tableLayoutPanel";
         tableLayoutPanel.RowCount = 2;
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Size = new Size(150, 150);
         tableLayoutPanel.TabIndex = 0;
         // 
         // dateTimePicker
         // 
         dateTimePicker.Location = new Point(3, 78);
         dateTimePicker.Name = "dateTimePicker";
         dateTimePicker.Size = new Size(38, 23);
         dateTimePicker.TabIndex = 0;
         // 
         // LabelDate
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         Controls.Add(tableLayoutPanel);
         Name = "LabelDate";
         tableLayoutPanel.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel;
      private CoreDateTimePicker dateTimePicker;
   }
}
