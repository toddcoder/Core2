﻿namespace Core.WinForms.Controls
{
   partial class Lister
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
         SuspendLayout();
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
         tableLayoutPanel.Size = new Size(500, 50);
         tableLayoutPanel.TabIndex = 0;
         // 
         // Lister
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         Controls.Add(tableLayoutPanel);
         Name = "Lister";
         Size = new Size(500, 50);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel;
   }
}
