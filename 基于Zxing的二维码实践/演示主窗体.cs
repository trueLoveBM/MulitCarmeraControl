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
    public partial class 演示主窗体 : Form
    {
        public 演示主窗体()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateQrCodeDemo_Click(object sender, EventArgs e)
        {
            生成二维码 frm = new 生成二维码();
            frm.ShowDialog();
        }

        /// <summary>
        /// 生成条形码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreateBarCode_Click(object sender, EventArgs e)
        {
            生成条形码 frm = new 生成条形码();
            frm.ShowDialog();
        }

        /// <summary>
        /// 摄像头识别二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecodeCodeFromCamera_Click(object sender, EventArgs e)
        {
            摄像头识别二维码 frm = new 摄像头识别二维码();
            frm.ShowDialog();
        }

        /// <summary>
        /// 图片中识别二维码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDecodeCodeFromImage_Click(object sender, EventArgs e)
        {
            多二维码的扫描 frm = new 多二维码的扫描();
            frm.ShowDialog();
        }
    }
}
