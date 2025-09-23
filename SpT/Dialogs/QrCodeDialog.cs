using System;
using System.Drawing;
using System.Windows.Forms;
using ZXing;
using ZXing.Common;
using ZXing.Rendering;

namespace SpT.Dialogs
{
    public class QrCodeDialog : Form
    {
        private readonly PictureBox pictureBox;
        private readonly Label titleLabel;
        private readonly string otpAuthUri;

        public QrCodeDialog(string otpAuthUri, string title = "Quét mã QR 2FA")
        {
            this.otpAuthUri = otpAuthUri ?? throw new ArgumentNullException(nameof(otpAuthUri));

            Text = title;
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            ClientSize = new Size(380, 420);

            titleLabel = new Label
            {
                Dock = DockStyle.Top,
                Height = 48,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Text = title
            };

            pictureBox = new PictureBox
            {
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.White
            };

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(16)
            };
            panel.Controls.Add(pictureBox);

            Controls.Add(panel);
            Controls.Add(titleLabel);

            Load += QrCodeDialog_Load;
        }

        private void QrCodeDialog_Load(object sender, EventArgs e)
        {
            try
            {
                var writer = new ZXing.BarcodeWriter
                {
                    Format = BarcodeFormat.QR_CODE,
                    Options = new EncodingOptions
                    {
                        Height = 320,
                        Width = 320,
                        Margin = 1
                    },
                    Renderer = new BitmapRenderer()
                };
                Bitmap bmp = writer.Write(otpAuthUri);
                pictureBox.Image = bmp;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this,
                    "Không thể tạo mã QR.\r\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                DialogResult = DialogResult.Abort;
                Close();
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // QrCodeDialog
            // 
            this.ClientSize = new System.Drawing.Size(310, 284);
            this.Name = "QrCodeDialog";
            this.ResumeLayout(false);

        }
    }
}
