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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form5));
         tableLayoutPanel1 = new TableLayoutPanel();
         panel1 = new Panel();
         exTextBox1 = new Controls.ExTextBox();
         uiAction1 = new Controls.UiAction();
         tableLayoutPanel1.SuspendLayout();
         panel1.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         tableLayoutPanel1.ColumnCount = 2;
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
         tableLayoutPanel1.Controls.Add(panel1, 0, 0);
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
         panel1.Controls.Add(uiAction1);
         panel1.Controls.Add(exTextBox1);
         panel1.Dock = DockStyle.Fill;
         panel1.Location = new Point(3, 3);
         panel1.Name = "panel1";
         panel1.Size = new Size(394, 219);
         panel1.TabIndex = 0;
         // 
         // exTextBox1
         // 
         exTextBox1.AllowMessage = "allowed";
         exTextBox1.AutoSelectAll = false;
         exTextBox1.CueBanner = "";
         exTextBox1.DenyMessage = "denied";
         exTextBox1.Location = new Point(3, 193);
         exTextBox1.Name = "exTextBox1";
         exTextBox1.RefreshOnTextChange = false;
         exTextBox1.Selection = ((int, int))resources.GetObject("exTextBox1.Selection");
         exTextBox1.Size = new Size(100, 23);
         exTextBox1.TabIndex = 0;
         exTextBox1.TrendMessage = "trending";
         exTextBox1.ValidateMessages = false;
         // 
         // uiAction1
         // 
         uiAction1.Arrow = false;
         uiAction1.AutoSizeText = false;
         uiAction1.BoxChecked = false;
         uiAction1.ButtonType = WinForms.Controls.UiActionButtonType.Normal;
         uiAction1.Cancelled = true;
         uiAction1.CardinalAlignment = WinForms.Controls.CardinalAlignment.Center;
         uiAction1.Checked = false;
         uiAction1.CheckStyle = WinForms.Controls.CheckStyle.None;
         uiAction1.ChooserGlyph = false;
         uiAction1.ClickGlyph = true;
         uiAction1.ClickText = "";
         uiAction1.ClickToCancel = false;
         uiAction1.DialogResult = DialogResult.None;
         uiAction1.DisabledIndex = -1;
         uiAction1.FlipFlop = false;
         uiAction1.Is3D = false;
         uiAction1.IsDirty = false;
         uiAction1.IsPath = false;
         uiAction1.Location = new Point(3, 153);
         uiAction1.Maximum = 1;
         uiAction1.MessageAlignment = WinForms.Controls.CardinalAlignment.Center;
         uiAction1.Minimum = 1;
         uiAction1.Name = "uiAction1";
         uiAction1.ProgressStripe = false;
         uiAction1.RectangleCount = 0;
         uiAction1.Required = false;
         uiAction1.SelectedIndex = -1;
         uiAction1.ShowFocus = false;
         uiAction1.ShowToGo = false;
         uiAction1.Size = new Size(150, 34);
         uiAction1.Status = WinForms.Controls.StatusType.None;
         uiAction1.Stopwatch = false;
         uiAction1.StopwatchInverted = true;
         uiAction1.StretchImage = false;
         uiAction1.TabIndex = 1;
         uiAction1.TaskBarProgress = false;
         uiAction1.ToolTipBox = false;
         uiAction1.ToolTipTitle = "";
         uiAction1.Type = WinForms.Controls.UiActionType.Success;
         uiAction1.WorkingAlignment = WinForms.Controls.CardinalAlignment.SouthWest;
         uiAction1.WorkReportsProgress = false;
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
         panel1.ResumeLayout(false);
         panel1.PerformLayout();
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel1;
      private Panel panel1;
      private Controls.UiAction uiAction1;
      private Controls.ExTextBox exTextBox1;
   }
}