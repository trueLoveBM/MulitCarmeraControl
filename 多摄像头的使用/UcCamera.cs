using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Base.DirectShow;
using Base.DirectShow.Entity;

namespace 多摄像头的使用
{
    public partial class UcCamera : UserControl
    {
        TestMulitCamera camera;

        public UcCamera()
        {
            InitializeComponent();
        }

        private void UcCamera_Load(object sender, EventArgs e)
        {

        }

        public void showCamera()
        {
            camera = TestManager.CreateCamera(this.Name);
            camera.Preview(this.Handle, this.Width, this.Height);

            //获取分辨率
            List<string> resolutions = camera.GetCameraSupportResolution();

            menuResolution.DropDownItems.Clear();
            for (int i = 0; i < resolutions.Count; i++)
            {
                var item = resolutions[i];
                ToolStripMenuItem s_item = new ToolStripMenuItem();
                s_item.Name = "resolution_" + i;
                s_item.Text = item;
                s_item.CheckOnClick = true;
                s_item.Checked = item == camera.Resolution;
                s_item.CheckedChanged += resolution_CheckChanged;
                menuResolution.DropDownItems.Add(s_item);
            }

            //获取方案
            List<CameraParamPlanEntity> plans = camera.GetCameraParamPlans();
            menuPlan.DropDownItems.Clear();
            for (int i = 0; i < plans.Count; i++)
            {
                var item = plans[i];
                ToolStripMenuItem s_item = new ToolStripMenuItem();
                s_item.Name = "plan_" + i;
                s_item.Text = item.ParamPlanName;
                s_item.CheckOnClick = true;
                s_item.Tag = item;
                s_item.Checked = item.DefaultSetting == true;
                s_item.CheckedChanged += plan_CheckChanged;
                menuPlan.DropDownItems.Add(s_item);

            }


        }

        /// <summary>
        /// 方案的选择改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void plan_CheckChanged(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in menuPlan.DropDownItems)
            {
                if (item == sender)
                {
                    camera.ChangeCameraConfigToSetting((sender as ToolStripMenuItem).Tag as CameraParamPlanEntity, true);
                    continue;
                }

                if (item.Checked == true)
                {
                    item.CheckedChanged -= plan_CheckChanged;
                    item.Checked = false;
                    item.CheckedChanged += plan_CheckChanged;
                }
            }
        }

        /// <summary>
        /// 分辨率的选择改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void resolution_CheckChanged(object sender, EventArgs e)
        {
            foreach (ToolStripMenuItem item in menuResolution.DropDownItems)
            {
                if (item == sender)
                {
                    camera.SetResolution((sender as ToolStripMenuItem).Text);
                    continue;
                }

                if (item.Checked == true)
                {
                    item.CheckedChanged -= resolution_CheckChanged;
                    item.Checked = false;
                    item.CheckedChanged += resolution_CheckChanged;
                }



            }
        }

        public void closeCamera()
        {
            camera.Dispose();
        }

        private void UcCamera_VisibleChanged(object sender, EventArgs e)
        {

        }

        private void menuChangeSetting_Click(object sender, EventArgs e)
        {
            camera.ChangeCameraSetting();
        }

        private void menu_savePlan_Click(object sender, EventArgs e)
        {
            FrmInputPlanName frm = new FrmInputPlanName(camera);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                //获取方案
                List<CameraParamPlanEntity> plans = camera.GetCameraParamPlans();
                menuPlan.DropDownItems.Clear();
                for (int i = 0; i < plans.Count; i++)
                {
                    var item = plans[i];
                    ToolStripMenuItem s_item = new ToolStripMenuItem();
                    s_item.Name = "plan_" + i;
                    s_item.Text = item.ParamPlanName;
                    s_item.CheckOnClick = true;
                    s_item.Tag = item;
                    s_item.Checked = item.DefaultSetting == true;
                    s_item.CheckedChanged += plan_CheckChanged;
                    menuPlan.DropDownItems.Add(s_item);

                }
            }
        }
    }
}
