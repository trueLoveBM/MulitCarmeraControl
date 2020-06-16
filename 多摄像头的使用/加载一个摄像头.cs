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
    public partial class 加载一个摄像头 : Form
    {
        public 加载一个摄像头()
        {
            InitializeComponent();
        }

        private void 加载一个摄像头_Load(object sender, EventArgs e)
        {
            DirectShowSimple.Instance.Preview(pnlCamera);
        }

        private void 加载一个摄像头_FormClosing(object sender, FormClosingEventArgs e)
        {
            DirectShowSimple.Instance.Dispose();
        }
    }
}
