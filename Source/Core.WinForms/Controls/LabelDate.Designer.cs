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
         tableLayoutPanel.Margin = new Padding(4, 4, 4, 4);
         tableLayoutPanel.Name = "tableLayoutPanel";
         tableLayoutPanel.RowCount = 2;
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
         tableLayoutPanel.Size = new Size(193, 190);
         tableLayoutPanel.TabIndex = 0;
         // 
         // dateTimePicker
         // 
         dateTimePicker.Location = new Point(4, 99);
         dateTimePicker.Margin = new Padding(4, 4, 4, 4);
         dateTimePicker.Name = "dateTimePicker";
         dateTimePicker.Size = new Size(48, 26);
         dateTimePicker.TabIndex = 0;
         // 
         // LabelDate
         // 
         AutoScaleDimensions = new SizeF(9F, 19F);
         AutoScaleMode = AutoScaleMode.Font;
         Controls.Add(tableLayoutPanel);
         Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
         Margin = new Padding(4, 4, 4, 4);
         Name = "LabelDate";
         Size = new Size(193, 190);
         tableLayoutPanel.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel;
      private CoreDateTimePicker dateTimePicker;
   }
}
