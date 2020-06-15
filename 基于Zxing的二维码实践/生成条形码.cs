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
    public partial class 生成条形码 : Form
    {
        private const string CREATE_BAR_CODE = "生成条形码";
        private const string RE_CREATE_BAR_CODE = "再建一个";

        public 生成条形码()
        {
            InitializeComponent();
        }

        private void btnCreateQrCode_Click(object sender, EventArgs e)
        {
            if (btnCreateQrCode.Text == CREATE_BAR_CODE)
            {
                if (string.IsNullOrEmpty(tbContent.Text))
                    return;

                Bitmap barCode = BarCodeWriter.CreateBarCode(tbContent.Text);
                pbQrCode.Image = barCode;
                pbQrCode.Visible = true;
                tbContent.Visible = false;
                btnCreateQrCode.Text = RE_CREATE_BAR_CODE;
            }
            else
            {
                tbContent.Visible = true;
                pbQrCode.Visible = false;
                btnCreateQrCode.Text = CREATE_BAR_CODE;
                tbContent.Text = string.Empty;
                tbContent.Focus();
            }
        }

        private void 生成条形码_FormClosing(object sender, FormClosingEventArgs e)
        {
           
        }
    }
}
