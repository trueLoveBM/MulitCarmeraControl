using EzQrCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 基于Zxing的二维码实践
{
    public partial class 生成二维码 : Form
    {
        private const string CREATE_QR_CODE = "生成二维码";
        private const string RE_CREATE_QR_CODE = "再建一个";

        public 生成二维码()
        {
            InitializeComponent();
        }

        private void btnCreateQrCode_Click(object sender, EventArgs e)
        {
            if (btnCreateQrCode.Text == CREATE_QR_CODE)
            {
                if (string.IsNullOrEmpty(tbContent.Text))
                    return;

                //普通二维码的生成
                //Bitmap qrCode = QrCodeWriter.CreateQrCode(tbContent.Text, ImageFormat.Png);

                //带logo的二维码的生成
                string CurrentDirectory = System.IO.Directory.GetCurrentDirectory();
                string logoPath = System.IO.Path.Combine(CurrentDirectory, "Resources\\Logo.jpg");
                Bitmap Logo = new Bitmap(logoPath);
                Bitmap qrCode = QrCodeWriter.CreateQrCodeWithLogo(tbContent.Text, Logo, "utf-8", 10, 10);
                pbQrCode.Image = qrCode;
                pbQrCode.Visible = true;
                tbContent.Visible = false;
                btnSave.Visible = true;
                btnCreateQrCode.Text = RE_CREATE_QR_CODE;
            }
            else
            {
                tbContent.Visible = true;
                pbQrCode.Visible = false;
                btnCreateQrCode.Text = CREATE_QR_CODE;
                tbContent.Text = string.Empty;
                btnSave.Visible = false;
                tbContent.Focus();
            }
        }

        private void 生成二维码_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = new Bitmap(pbQrCode.Image);
            bitmap.Save($"D:\\Test\\" + tbContent.Text + ".png", ImageFormat.Png);
            bitmap.Dispose();
        }
    }
}
