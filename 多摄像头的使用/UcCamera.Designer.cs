namespace 多摄像头的使用
{
    partial class UcCamera
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ctxRight = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.menuResolution = new System.Windows.Forms.ToolStripMenuItem();
            this.tooltest = new System.Windows.Forms.ToolStripMenuItem();
            this.menuPlan = new System.Windows.Forms.ToolStripMenuItem();
            this.menuChangeSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.menu_savePlan = new System.Windows.Forms.ToolStripMenuItem();
            this.menuCamera = new System.Windows.Forms.ToolStripMenuItem();
            this.ctxRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // ctxRight
            // 
            this.ctxRight.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuResolution,
            this.menuPlan,
            this.menuChangeSetting,
            this.menu_savePlan,
            this.menuCamera});
            this.ctxRight.Name = "ctxRight";
            this.ctxRight.ShowCheckMargin = true;
            this.ctxRight.Size = new System.Drawing.Size(203, 136);
            this.ctxRight.Opening += new System.ComponentModel.CancelEventHandler(this.ctxRight_Opening);
            // 
            // menuResolution
            // 
            this.menuResolution.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tooltest});
            this.menuResolution.Name = "menuResolution";
            this.menuResolution.Size = new System.Drawing.Size(202, 22);
            this.menuResolution.Text = "分辨率";
            // 
            // tooltest
            // 
            this.tooltest.Name = "tooltest";
            this.tooltest.Size = new System.Drawing.Size(180, 22);
            this.tooltest.Text = "100*100";
            // 
            // menuPlan
            // 
            this.menuPlan.Name = "menuPlan";
            this.menuPlan.Size = new System.Drawing.Size(202, 22);
            this.menuPlan.Text = "方案";
            // 
            // menuChangeSetting
            // 
            this.menuChangeSetting.Name = "menuChangeSetting";
            this.menuChangeSetting.Size = new System.Drawing.Size(202, 22);
            this.menuChangeSetting.Text = "调整参数";
            this.menuChangeSetting.Click += new System.EventHandler(this.menuChangeSetting_Click);
            // 
            // menu_savePlan
            // 
            this.menu_savePlan.Name = "menu_savePlan";
            this.menu_savePlan.Size = new System.Drawing.Size(202, 22);
            this.menu_savePlan.Text = "保存方案";
            this.menu_savePlan.Click += new System.EventHandler(this.menu_savePlan_Click);
            // 
            // menuCamera
            // 
            this.menuCamera.Name = "menuCamera";
            this.menuCamera.Size = new System.Drawing.Size(202, 22);
            this.menuCamera.Text = "摄像头";
            // 
            // UcCamera
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ContextMenuStrip = this.ctxRight;
            this.Name = "UcCamera";
            this.Size = new System.Drawing.Size(300, 300);
            this.Load += new System.EventHandler(this.UcCamera_Load);
            this.VisibleChanged += new System.EventHandler(this.UcCamera_VisibleChanged);
            this.ctxRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip ctxRight;
        private System.Windows.Forms.ToolStripMenuItem menuResolution;
        private System.Windows.Forms.ToolStripMenuItem tooltest;
        private System.Windows.Forms.ToolStripMenuItem menuPlan;
        private System.Windows.Forms.ToolStripMenuItem menuChangeSetting;
        private System.Windows.Forms.ToolStripMenuItem menu_savePlan;
        private System.Windows.Forms.ToolStripMenuItem menuCamera;
    }
}
