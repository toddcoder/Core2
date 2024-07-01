namespace Core.WinForms.Controls
{
   partial class Chooser
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
         listViewItems = new ListView();
         columnHeader1 = new ColumnHeader();
         SuspendLayout();
         // 
         // listViewItems
         // 
         listViewItems.Activation = ItemActivation.OneClick;
         listViewItems.Columns.AddRange(new ColumnHeader[] { columnHeader1 });
         listViewItems.Dock = DockStyle.Fill;
         listViewItems.Font = new Font("Consolas", 12F, FontStyle.Regular, GraphicsUnit.Point);
         listViewItems.FullRowSelect = true;
         listViewItems.GridLines = true;
         listViewItems.Location = new Point(0, 0);
         listViewItems.Margin = new Padding(4, 3, 4, 3);
         listViewItems.MultiSelect = false;
         listViewItems.Name = "listViewItems";
         listViewItems.OwnerDraw = true;
         listViewItems.Scrollable = false;
         listViewItems.Size = new Size(467, 519);
         listViewItems.TabIndex = 0;
         listViewItems.UseCompatibleStateImageBehavior = false;
         listViewItems.View = View.Details;
         listViewItems.DrawItem += listViewItems_DrawItem;
         listViewItems.ItemChecked += listViewItems_ItemChecked;
         listViewItems.SelectedIndexChanged += listViewItems_SelectedIndexChanged;
         // 
         // columnHeader1
         // 
         columnHeader1.Text = "";
         columnHeader1.Width = 25;
         // 
         // Chooser
         // 
         AutoScaleDimensions = new SizeF(7F, 15F);
         AutoScaleMode = AutoScaleMode.Font;
         ClientSize = new Size(467, 519);
         Controls.Add(listViewItems);
         FormBorderStyle = FormBorderStyle.None;
         KeyPreview = true;
         Margin = new Padding(4, 3, 4, 3);
         MaximizeBox = false;
         MinimizeBox = false;
         Name = "Chooser";
         StartPosition = FormStartPosition.Manual;
         FormClosed += Chooser_FormClosed;
         Load += Chooser_Load;
         KeyUp += Chooser_KeyUp;
         ResumeLayout(false);
      }

      #endregion

      private System.Windows.Forms.ListView listViewItems;
      private System.Windows.Forms.ColumnHeader columnHeader1;
   }
}