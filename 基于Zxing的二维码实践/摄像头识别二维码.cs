using Base.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace 基于Zxing的二维码实践
{
    public partial class 摄像头识别二维码 : Form
    {

        /// <summary>
        /// 摄像头预览成功
        /// </summary>
        private bool _previewOk;

        public 摄像头识别二维码()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Load事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 摄像头识别二维码_Load(object sender, EventArgs e)
        {
            _previewOk = DirectShowSimple.Instance.Preview(this.pnlContent);
        }

        private void btnDecode_Click(object sender, EventArgs e)
        {
            if (cbMulti.Checked)
                readMultiQrCode();
            else
                readSingleQrCode();

        }

        private void readMultiQrCode()
        {
            lbTip.Text = "正在识别...,请将二维码置于摄像头内";
            if (!_previewOk)
                return;

            bool? find_qrCode = false;
            //尝试的次数
            int max_try = 10;
            //当前已经尝试的次数
            int try_count = 0;
            List<string> docode_result = null;

            //方法一，截取图片，从图片中解析
            //while (find_qrCode.HasValue && !find_qrCode.Value)
            //{
            //    string filePath = "D://" + Guid.NewGuid() + ".png";
            //    bool takeOk = DirectShowSimple.Instance.TakeAPicture(filePath);
            //    docode_result = EzQrCode.QrCodeReader.MultiRead(filePath);
            //    if (docode_result != null && docode_result.Count > 0)
            //        find_qrCode = true;
            //    else
            //    {
            //        if (try_count < max_try)
            //            //休息一秒
            //            Thread.Sleep(1000);
            //        else
            //            find_qrCode = null;
            //        try_count++;
            //    }
            //}


            //方法二,扩展Direcshow，直接返回截屏后的bitmap对象，减少文件操作，提高效率
            while (find_qrCode.HasValue && !find_qrCode.Value)
            {
                Bitmap bitmap = DirectShowSimple.Instance.CaptureCamera();
                //摄像头截图成功
                if (bitmap != null)
                {
                    docode_result = EzQrCode.CodeReader.MultiRead(bitmap);

                    //释放bitmap资源
                    bitmap.Dispose();
                    bitmap = null;
                }

                if (docode_result != null && docode_result.Count > 0)
                    find_qrCode = true;
                else
                {
                    if (try_count < max_try)
                        //休息一秒
                        Thread.Sleep(1000);
                    else
                        find_qrCode = null;
                    try_count++;
                }
            }

            //判断是否解析到二维码
            if (find_qrCode.HasValue && find_qrCode.Value && docode_result?.Count > 0)
            {
                for (int i = 0; i < docode_result.Count; i++)
                {
                    if (i == 0)
                        lbTip.Text = docode_result[0];
                    else
                        lbTip.Text = "\t" + lbTip.Text + docode_result[i];
                }
            }
            else
                lbTip.Text = "识别失败....";

        }


        /// <summary>
        /// 读取解析单个二维码
        /// </summary>
        private void readSingleQrCode()
        {
            lbTip.Text = "正在识别...,请将二维码置于摄像头内";
            if (!_previewOk)
                return;

            bool? find_qrCode = false;
            //尝试的次数
            int max_try = 10;
            //当前已经尝试的次数
            int try_count = 0;
            string docode_result = string.Empty;

            //方法一，截取图片，从图片中解析
            //while (find_qrCode.HasValue && !find_qrCode.Value)
            //{
            //    string filePath = "D://" + Guid.NewGuid() + ".png";
            //    bool takeOk = DirectShowSimple.Instance.TakeAPicture(filePath);
            //    docode_result = EzQrCode.QrCodeReader.Read(filePath);
            //if (!string.IsNullOrEmpty(docode_result))
            //    find_qrCode = true;
            //else
            //{
            //    if (try_count < max_try)
            //        //休息一秒
            //        Thread.Sleep(1000);
            //    else
            //        find_qrCode = null;
            //    try_count++;
            //}
            //}



            //方法二,扩展Direcshow，直接返回截屏后的bitmap对象，减少文件操作，提高效率
            while (find_qrCode.HasValue && !find_qrCode.Value)
            {
                Bitmap bitmap = DirectShowSimple.Instance.CaptureCamera();
                //摄像头截图成功
                if (bitmap != null)
                {
                    docode_result = EzQrCode.CodeReader.Read(bitmap);

                    //释放bitmap资源
                    bitmap.Dispose();
                    bitmap = null;
                }

                if (!string.IsNullOrEmpty(docode_result))
                    find_qrCode = true;
                else
                {
                    if (try_count < max_try)
                        //休息一秒
                        Thread.Sleep(1000);
                    else
                        find_qrCode = null;
                    try_count++;
                }
            }


            //判断是否解析到二维码
            if (find_qrCode.HasValue && find_qrCode.Value && !string.IsNullOrEmpty(docode_result))
                lbTip.Text = string.Format("解析结果:{0}", docode_result);
            else
                lbTip.Text = "识别失败....";
        }

        private void 摄像头识别二维码_FormClosing(object sender, FormClosingEventArgs e)
        {
            //释放摄像头资源
            DirectShowSimple.Instance.Dispose();
        }
    }
}
