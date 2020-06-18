namespace 多摄像头的使用
{
    partial class 加载一个摄像头
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.picShow = new System.Windows.Forms.PictureBox();
            this.btnShot = new System.Windows.Forms.Button();
            this.btnStopVideo = new System.Windows.Forms.Button();
            this.btnStartVideo = new System.Windows.Forms.Button();
            this.pnlCamera = new System.Windows.Forms.Panel();
            this.lblCameraParams = new System.Windows.Forms.Label();
            this.cmbResolution = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOleSetting = new System.Windows.Forms.Button();
            this.btnSavePlan = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbSettings = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picShow)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.cmbSettings);
            this.panel1.Controls.Add(this.btnSavePlan);
            this.panel1.Controls.Add(this.btnOleSetting);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.cmbResolution);
            this.panel1.Controls.Add(this.lblCameraParams);
            this.panel1.Controls.Add(this.picShow);
            this.panel1.Controls.Add(this.btnShot);
            this.panel1.Controls.Add(this.btnStopVideo);
            this.panel1.Controls.Add(this.btnStartVideo);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(600, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 450);
            this.panel1.TabIndex = 0;
            // 
            // picShow
            // 
            this.picShow.Location = new System.Drawing.Point(3, 286);
            this.picShow.Name = "picShow";
            this.picShow.Size = new System.Drawing.Size(194, 152);
            this.picShow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picShow.TabIndex = 3;
            this.picShow.TabStop = false;
            // 
            // btnShot
            // 
            this.btnShot.Location = new System.Drawing.Point(6, 70);
            this.btnShot.Name = "btnShot";
            this.btnShot.Size = new System.Drawing.Size(182, 23);
            this.btnShot.TabIndex = 2;
            this.btnShot.Text = "拍照";
            this.btnShot.UseVisualStyleBackColor = true;
            this.btnShot.Click += new System.EventHandler(this.btnShot_Click);
            // 
            // btnStopVideo
            // 
            this.btnStopVideo.Enabled = false;
            this.btnStopVideo.Location = new System.Drawing.Point(6, 41);
            this.btnStopVideo.Name = "btnStopVideo";
            this.btnStopVideo.Size = new System.Drawing.Size(182, 23);
            this.btnStopVideo.TabIndex = 1;
            this.btnStopVideo.Text = "结束录像";
            this.btnStopVideo.UseVisualStyleBackColor = true;
            this.btnStopVideo.Click += new System.EventHandler(this.btnStopVideo_Click);
            // 
            // btnStartVideo
            // 
            this.btnStartVideo.Location = new System.Drawing.Point(6, 12);
            this.btnStartVideo.Name = "btnStartVideo";
            this.btnStartVideo.Size = new System.Drawing.Size(182, 23);
            this.btnStartVideo.TabIndex = 0;
            this.btnStartVideo.Text = "开始录像";
            this.btnStartVideo.UseVisualStyleBackColor = true;
            this.btnStartVideo.Click += new System.EventHandler(this.btnStartVideo_Click);
            // 
            // pnlCamera
            // 
            this.pnlCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCamera.Location = new System.Drawing.Point(0, 0);
            this.pnlCamera.Name = "pnlCamera";
            this.pnlCamera.Size = new System.Drawing.Size(600, 450);
            this.pnlCamera.TabIndex = 1;
            // 
            // lblCameraParams
            // 
            this.lblCameraParams.AutoSize = true;
            this.lblCameraParams.Location = new System.Drawing.Point(6, 271);
            this.lblCameraParams.Name = "lblCameraParams";
            this.lblCameraParams.Size = new System.Drawing.Size(65, 12);
            this.lblCameraParams.TabIndex = 4;
            this.lblCameraParams.Text = "摄像头参数";
            // 
            // cmbResolution
            // 
            this.cmbResolution.FormattingEnabled = true;
            this.cmbResolution.Location = new System.Drawing.Point(67, 157);
            this.cmbResolution.Name = "cmbResolution";
            this.cmbResolution.Size = new System.Drawing.Size(121, 20);
            this.cmbResolution.TabIndex = 5;
            this.cmbResolution.SelectedIndexChanged += new System.EventHandler(this.cmbResolution_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 160);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "分辨率";
            // 
            // btnOleSetting
            // 
            this.btnOleSetting.Location = new System.Drawing.Point(6, 99);
            this.btnOleSetting.Name = "btnOleSetting";
            this.btnOleSetting.Size = new System.Drawing.Size(182, 23);
            this.btnOleSetting.TabIndex = 7;
            this.btnOleSetting.Text = "摄像头配置(原始)";
            this.btnOleSetting.UseVisualStyleBackColor = true;
            this.btnOleSetting.Click += new System.EventHandler(this.btnOleSetting_Click);
            // 
            // btnSavePlan
            // 
            this.btnSavePlan.Location = new System.Drawing.Point(6, 128);
            this.btnSavePlan.Name = "btnSavePlan";
            this.btnSavePlan.Size = new System.Drawing.Size(182, 23);
            this.btnSavePlan.TabIndex = 8;
            this.btnSavePlan.Text = "保存摄像头配置";
            this.btnSavePlan.UseVisualStyleBackColor = true;
            this.btnSavePlan.Click += new System.EventHandler(this.btnSavePlan_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 187);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 10;
            this.label2.Text = "配置方案";
            // 
            // cmbSettings
            // 
            this.cmbSettings.FormattingEnabled = true;
            this.cmbSettings.Location = new System.Drawing.Point(67, 184);
            this.cmbSettings.Name = "cmbSettings";
            this.cmbSettings.Size = new System.Drawing.Size(121, 20);
            this.cmbSettings.TabIndex = 9;
            this.cmbSettings.SelectedIndexChanged += new System.EventHandler(this.cmbSettings_SelectedIndexChanged);
            // 
            // 加载一个摄像头
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnlCamera);
            this.Controls.Add(this.panel1);
            this.Name = "加载一个摄像头";
            this.Text = "加载一个摄像头";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.加载一个摄像头_FormClosing);
            this.Load += new System.EventHandler(this.加载一个摄像头_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picShow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlCamera;
        private System.Windows.Forms.Button btnStartVideo;
        private System.Windows.Forms.Button btnStopVideo;
        private System.Windows.Forms.Button btnShot;
        private System.Windows.Forms.PictureBox picShow;
        private System.Windows.Forms.Label lblCameraParams;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbResolution;
        private System.Windows.Forms.Button btnOleSetting;
        private System.Windows.Forms.Button btnSavePlan;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbSettings;
    }
}