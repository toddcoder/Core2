using Core.Strings;
using Core.WinForms.Controls;

namespace Core.WinForms.Forms;

public partial class Response : Form
{
   public static ResponseType Ask(string title, string message, string positive, string negative)
   {
      using var response = new Response(title, message, positive, negative);
      response.ShowDialog();

      return response.ResponseType;
   }

   protected UiActionTextCanvas uiMessage = new() { Dock = DockStyle.Fill };

   protected Response(string title, string message, string positive, string negative)
   {
      InitializeComponent();

      uiTitle.ControlLabel(title);

      Controls.Add(uiMessage);
      tableLayoutPanel.Controls.Add(uiMessage);
      tableLayoutPanel.SetColumn(uiMessage, 0);
      tableLayoutPanel.SetRow(uiMessage, 1);
      uiMessage.BackColor = Color.White;
      foreach (var line in message.Lines())
      {
         uiMessage.WriteLine(line);
      }

      uiMessage.Refresh();

      uiPositive.Success(positive);
      uiPositive.Click += (_, _) =>
      {
         ResponseType = new ResponseType.Positive();
         Close();
      };
      uiPositive.ClickText = positive;

      uiNegative.Failure(negative);
      uiNegative.Click += (_, _) =>
      {
         ResponseType = new ResponseType.Negative();
         Close();
      };
      uiNegative.ClickText = negative;
   }

   public ResponseType ResponseType { get; set; } = new ResponseType.Cancel();

   protected void Response_MouseDown(object sender, MouseEventArgs e)
   {
      var mouseLocation = PointToScreen(e.Location);
      if (!ClientRectangle.Contains(mouseLocation))
      {
         Close();
      }
   }
}