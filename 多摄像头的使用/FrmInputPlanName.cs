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
    public partial class FrmInputPlanName : Form
    {
        TestMulitCamera _camera1;
        public FrmInputPlanName(TestMulitCamera camera1)
        {
            InitializeComponent();
            _camera1 = camera1;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            string txt = txtName.Text;
            if (_camera1.SaveCameraConfigToSetting(txt, true))
                this.DialogResult = DialogResult.OK;
        }
    }
}
