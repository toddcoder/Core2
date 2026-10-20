namespace Core.WinForms.Forms
{
   partial class Response
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
         Controls.DividerValidation.None none7 = new Controls.DividerValidation.None();
         Controls.KeyDownCapture.None none8 = new Controls.KeyDownCapture.None();
         Controls.DividerValidation.None none9 = new Controls.DividerValidation.None();
         Controls.KeyDownCapture.None none10 = new Controls.KeyDownCapture.None();
         Controls.DividerValidation.None none11 = new Controls.DividerValidation.None();
         Controls.KeyDownCapture.None none12 = new Controls.KeyDownCapture.None();
         tableLayoutPanel = new TableLayoutPanel();
         uiNegative = new Core.WinForms.Controls.UiAction();
         uiTitle = new Core.WinForms.Controls.UiAction();
         uiPositive = new Core.WinForms.Controls.UiAction();
         tableLayoutPanel.SuspendLayout();
         SuspendLayout();
         // 
         // tableLayoutPanel
         // 
         tableLayoutPanel.ColumnCount = 1;
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
         tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20F));
         tableLayoutPanel.Controls.Add(uiNegative, 0, 3);
         tableLayoutPanel.Controls.Add(uiTitle, 0, 0);
         tableLayoutPanel.Controls.Add(uiPositive, 0, 2);
         tableLayoutPanel.Dock = DockStyle.Fill;
         tableLayoutPanel.Location = new Point(0, 0);
         tableLayoutPanel.Name = "tableLayoutPanel";
         tableLayoutPanel.RowCount = 4;
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         tableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
         tableLayoutPanel.Size = new Size(800, 300);
         tableLayoutPanel.TabIndex = 0;
         // 
         // uiNegative
         // 
         uiNegative.Arrow = false;
         uiNegative.AutoSizeText = false;
         uiNegative.BoxChecked = false;
         uiNegative.ButtonType = WinForms.Controls.UiActionButtonType.Normal;
         uiNegative.Cancelled = true;
         uiNegative.CardinalAlignment = WinForms.Controls.CardinalAlignment.Center;
         uiNegative.Checked = false;
         uiNegative.CheckStyle = WinForms.Controls.CheckStyle.None;
         uiNegative.ChooserGlyph = false;
         uiNegative.ClickGlyph = true;
         uiNegative.ClickText = "negative";
         uiNegative.ClickToCancel = false;
         uiNegative.CountBackColor = Color.Blue;
         uiNegative.CountForeColor = Color.White;
         uiNegative.DialogResult = DialogResult.None;
         uiNegative.DisabledIndex = -1;
         uiNegative.DividerValidation = none7;
         uiNegative.Dock = DockStyle.Fill;
         uiNegative.DrawNot = true;
         uiNegative.FlipFlop = false;
         uiNegative.HostCanSizeToText = true;
         uiNegative.HostCanSizeToTextPadding = 40;
         uiNegative.Is3D = false;
         uiNegative.IsDirty = false;
         uiNegative.IsPath = false;
         uiNegative.KeyDownCaption = none8;
         uiNegative.Location = new Point(3, 263);
         uiNegative.Maximum = 1;
         uiNegative.MessageAlignment = WinForms.Controls.CardinalAlignment.Center;
         uiNegative.Minimum = 1;
         uiNegative.Name = "uiNegative";
         uiNegative.NonNullFont = new Font("Consolas", 12F);
         uiNegative.NonNullText = "negative";
         uiNegative.ObjectId = 0L;
         uiNegative.ProgressStripe = false;
         uiNegative.RectangleCount = 0;
         uiNegative.Required = false;
         uiNegative.SelectedIndex = -1;
         uiNegative.ShowFocus = false;
         uiNegative.ShowToGo = false;
         uiNegative.Size = new Size(794, 34);
         uiNegative.Status = WinForms.Controls.StatusType.None;
         uiNegative.Stopwatch = false;
         uiNegative.StopwatchInverted = true;
         uiNegative.StretchImage = false;
         uiNegative.TabIndex = 4;
         uiNegative.TaskBarProgress = false;
         uiNegative.ToolTipBox = false;
         uiNegative.ToolTipTitle = "";
         uiNegative.Type = WinForms.Controls.UiActionType.Button;
         uiNegative.UseEmojis = false;
         uiNegative.WorkingAlignment = WinForms.Controls.CardinalAlignment.SouthWest;
         uiNegative.WorkReportsProgress = false;
         // 
         // uiTitle
         // 
         uiTitle.Arrow = false;
         uiTitle.AutoSizeText = false;
         uiTitle.BoxChecked = false;
         uiTitle.ButtonType = WinForms.Controls.UiActionButtonType.Normal;
         uiTitle.Cancelled = true;
         uiTitle.CardinalAlignment = WinForms.Controls.CardinalAlignment.Center;
         uiTitle.Checked = false;
         uiTitle.CheckStyle = WinForms.Controls.CheckStyle.None;
         uiTitle.ChooserGlyph = false;
         uiTitle.ClickGlyph = false;
         uiTitle.ClickText = "title";
         uiTitle.ClickToCancel = false;
         tableLayoutPanel.SetColumnSpan(uiTitle, 2);
         uiTitle.CountBackColor = Color.Blue;
         uiTitle.CountForeColor = Color.White;
         uiTitle.DialogResult = DialogResult.None;
         uiTitle.DisabledIndex = -1;
         uiTitle.DividerValidation = none9;
         uiTitle.Dock = DockStyle.Fill;
         uiTitle.DrawNot = true;
         uiTitle.FlipFlop = false;
         uiTitle.HostCanSizeToText = true;
         uiTitle.HostCanSizeToTextPadding = 40;
         uiTitle.Is3D = false;
         uiTitle.IsDirty = false;
         uiTitle.IsPath = false;
         uiTitle.KeyDownCaption = none10;
         uiTitle.Location = new Point(3, 3);
         uiTitle.Maximum = 1;
         uiTitle.MessageAlignment = WinForms.Controls.CardinalAlignment.Center;
         uiTitle.Minimum = 1;
         uiTitle.Name = "uiTitle";
         uiTitle.NonNullFont = new Font("Consolas", 12F);
         uiTitle.NonNullText = "title";
         uiTitle.ObjectId = 0L;
         uiTitle.ProgressStripe = false;
         uiTitle.RectangleCount = 0;
         uiTitle.Required = false;
         uiTitle.SelectedIndex = -1;
         uiTitle.ShowFocus = false;
         uiTitle.ShowToGo = false;
         uiTitle.Size = new Size(794, 34);
         uiTitle.Status = WinForms.Controls.StatusType.None;
         uiTitle.Stopwatch = false;
         uiTitle.StopwatchInverted = true;
         uiTitle.StretchImage = false;
         uiTitle.TabIndex = 0;
         uiTitle.TaskBarProgress = false;
         uiTitle.ToolTipBox = false;
         uiTitle.ToolTipTitle = "";
         uiTitle.Type = WinForms.Controls.UiActionType.ControlLabel;
         uiTitle.UseEmojis = true;
         uiTitle.WorkingAlignment = WinForms.Controls.CardinalAlignment.SouthWest;
         uiTitle.WorkReportsProgress = false;
         // 
         // uiPositive
         // 
         uiPositive.Arrow = false;
         uiPositive.AutoSizeText = false;
         uiPositive.BoxChecked = false;
         uiPositive.ButtonType = WinForms.Controls.UiActionButtonType.Normal;
         uiPositive.Cancelled = true;
         uiPositive.CardinalAlignment = WinForms.Controls.CardinalAlignment.Center;
         uiPositive.Checked = false;
         uiPositive.CheckStyle = WinForms.Controls.CheckStyle.None;
         uiPositive.ChooserGlyph = false;
         uiPositive.ClickGlyph = true;
         uiPositive.ClickText = "positive";
         uiPositive.ClickToCancel = false;
         uiPositive.CountBackColor = Color.Blue;
         uiPositive.CountForeColor = Color.White;
         uiPositive.DialogResult = DialogResult.None;
         uiPositive.DisabledIndex = -1;
         uiPositive.DividerValidation = none11;
         uiPositive.Dock = DockStyle.Fill;
         uiPositive.DrawNot = true;
         uiPositive.FlipFlop = false;
         uiPositive.HostCanSizeToText = true;
         uiPositive.HostCanSizeToTextPadding = 40;
         uiPositive.Is3D = false;
         uiPositive.IsDirty = false;
         uiPositive.IsPath = false;
         uiPositive.KeyDownCaption = none12;
         uiPositive.Location = new Point(3, 223);
         uiPositive.Maximum = 1;
         uiPositive.MessageAlignment = WinForms.Controls.CardinalAlignment.Center;
         uiPositive.Minimum = 1;
         uiPositive.Name = "uiPositive";
         uiPositive.NonNullFont = new Font("Consolas", 12F);
         uiPositive.NonNullText = "positive";
         uiPositive.ObjectId = 0L;
         uiPositive.ProgressStripe = false;
         uiPositive.RectangleCount = 0;
         uiPositive.Required = false;
         uiPositive.SelectedIndex = -1;
         uiPositive.ShowFocus = false;
         uiPositive.ShowToGo = false;
         uiPositive.Size = new Size(794, 34);
         uiPositive.Status = WinForms.Controls.StatusType.None;
         uiPositive.Stopwatch = false;
         uiPositive.StopwatchInverted = true;
         uiPositive.StretchImage = false;
         uiPositive.TabIndex = 2;
         uiPositive.TaskBarProgress = false;
         uiPositive.ToolTipBox = false;
         uiPositive.ToolTipTitle = "";
         uiPositive.Type = WinForms.Controls.UiActionType.Button;
         uiPositive.UseEmojis = false;
         uiPositive.WorkingAlignment = WinForms.Controls.CardinalAlignment.SouthWest;
         uiPositive.WorkReportsProgress = false;
         // 
         // Response
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(800, 300);
         Controls.Add(tableLayoutPanel);
         FormBorderStyle = FormBorderStyle.None;
         Name = "Response";
         StartPosition = FormStartPosition.CenterParent;
         Text = "Response";
         MouseDown += Response_MouseDown;
         tableLayoutPanel.ResumeLayout(false);
         ResumeLayout(false);
      }

      #endregion

      private TableLayoutPanel tableLayoutPanel;
      private Controls.UiAction uiTitle;
      private Controls.UiAction uiNegative;
      private Controls.UiAction uiPositive;
   }
}