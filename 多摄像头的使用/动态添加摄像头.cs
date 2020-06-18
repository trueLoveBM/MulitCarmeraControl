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
    public partial class 动态添加摄像头 : Form
    {
        public 动态添加摄像头()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 添加摄像头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            UcCamera ucCamera = null;
            try
            {
                ucCamera = new UcCamera();
                ucCamera.Name = "camara" + fpContent.Controls.Count;
                fpContent.Controls.Add(ucCamera);
                ucCamera.showCamera();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

                if (ucCamera != null)
                {
                    fpContent.Controls.Remove(ucCamera);
                    ucCamera.closeCamera();
                }
                ucCamera = null;
            }

        }

        private void 动态添加摄像头_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (UcCamera item in fpContent.Controls)
            {
                item.closeCamera();
            }
        }

        private void btnRemoveCamera_Click(object sender, EventArgs e)
        {
            if (fpContent.Controls.Count > 0)
            {
                UcCamera uc = fpContent.Controls[fpContent.Controls.Count - 1] as UcCamera;
                uc.closeCamera();
                fpContent.Controls.Remove(uc);
            }
        }
    }
}
