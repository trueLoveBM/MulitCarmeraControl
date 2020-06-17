using Base.DirectShow;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace 多摄像头的使用
{
    public partial class 加载两个摄像头 : Form
    {
        string _camera_1_name = "USB2.0 PC CAMERA";
        string _audio_1_name = "麦克风 (USB2.0 MIC)";
        TestMulitCamera _camera1;


        string _camera_2_name = "";
        string _audio_2_name = "";
        TestMulitCamera _camera2;

        public 加载两个摄像头()
        {
            InitializeComponent();
        }

        private void 加载两个摄像头_Load(object sender, EventArgs e)
        {
            //预览一个摄像头
            _camera1 = TestManager.CreateCamera("key0");
            _camera1.Preview(pnlVideo1.Handle, pnlVideo1.Width, pnlVideo1.Height);

            //预览第二个摄像头
            _camera2 = TestManager.CreateCamera("key1");
            _camera2.Preview(pnlVideo2.Handle, pnlVideo2.Width, pnlVideo2.Height);
        }

        private void 加载两个摄像头_FormClosing(object sender, FormClosingEventArgs e)
        {
            _camera1.Dispose();
            _camera2.Dispose();
        }
    }
}
