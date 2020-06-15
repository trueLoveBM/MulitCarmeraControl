using EzQrCode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 基于Zxing的二维码实践
{
    public partial class 多二维码的扫描 : Form
    {
        public 多二维码的扫描()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 选择图片
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelectBarcodeImageFileForDecoding_Click(object sender, EventArgs e)
        {
            using (var openDlg = new OpenFileDialog())
            {
                openDlg.FileName = txtBarcodeImageFile.Text;
                openDlg.Multiselect = false;
                openDlg.Filter = "PNG Files (*.png)|*.png|BMP Files (*.bmp)|*.bmp|TIFF Files (*.tif)|*.tif|JPG Files (*.jpg)|*.jpg|PDF Files (*.pdf)|*.pdf|All Files (*.*)|*.*";
                if (openDlg.ShowDialog(this) == DialogResult.OK)
                {
                    txtBarcodeImageFile.Text = openDlg.FileName;
                }
            }
        }

        /// <summary>
        /// 解析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtBarcodeImageFile.Text))
                return;


            List<string> result = CodeReader.MultiRead(txtBarcodeImageFile.Text);

            tbResult.Text = string.Format("共获取到{0}条结果", result?.Count ?? 0);

            foreach (var item in result)
            {
                if (string.IsNullOrEmpty(tbResult.Text))
                    tbResult.Text = item;
                else
                    tbResult.Text = tbResult.Text + "\r\n" + item;
            }

        }

        private void 多二维码的扫描_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
