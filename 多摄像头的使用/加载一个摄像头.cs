using Base.DirectShow;
using Base.DirectShow.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinFormTestDemoV2;

namespace 多摄像头的使用
{
    public partial class 加载一个摄像头 : Form
    {
        string _camera_1_name = "USB2.0 PC CAMERA";
        string _audio_1_name = "麦克风 (USB2.0 MIC)";
        TestMulitCamera _camera1;

        string _picture_save_dir = "D:\\Test";

        public 加载一个摄像头()
        {
            InitializeComponent();
        }

        private void 加载一个摄像头_Load(object sender, EventArgs e)
        {

            //测试
            //CameraEntity cameraEntity = new CameraEntity();
            //cameraEntity.Name = _camera_1_name;
            //cameraEntity.Status = 2;
            //cameraEntity.Save();

            ////测试
            //CameraEntity cameraEntity2 = new CameraEntity();
            //cameraEntity2.Name = "cessss";
            //cameraEntity2.Status = 0;
            //cameraEntity2.Save();





            //DirectShowSimple.Instance.Preview(pnlCamera);
            //预览一个摄像头
            //_camera1 = new TestMulitCamera();
            //_camera1.BindCamera(_camera_1_name);
            //_camera1.BindAudio(_audio_1_name);
            _camera1 = TestManager.CreateCamera("key0");
            _camera1.Preview(pnlCamera.Handle, pnlCamera.Width, pnlCamera.Height);
        }

        private void 加载一个摄像头_FormClosing(object sender, FormClosingEventArgs e)
        {
            //DirectShowSimple.Instance.Dispose();
            _camera1.Dispose();
        }

        /// <summary>
        /// 拍照
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnShot_Click(object sender, EventArgs e)
        {
            string savePath = _picture_save_dir + "\\" + Guid.NewGuid().ToString() + ".jpg";
            _camera1.CameraShot(savePath);
            picShow.Image = Image.FromFile(savePath);
        }

        private void btnStartVideo_Click(object sender, EventArgs e)
        {
            btnStartVideo.Enabled = false;
            btnStopVideo.Enabled = true;
            string savePath = _picture_save_dir + "\\" + Guid.NewGuid().ToString() + ".wmv";
            btnStopVideo.Tag = savePath;
            _camera1.StartMonitorRecord(savePath);
        }

        private void btnStopVideo_Click(object sender, EventArgs e)
        {
            _camera1.StopMonitorRecord();
            //播放视频
            FrmVideoShower shower = new FrmVideoShower(btnStopVideo.Tag as string);
            shower.Show();

            btnStartVideo.Enabled = true;
            btnStopVideo.Enabled = false;
        }
    }
}
