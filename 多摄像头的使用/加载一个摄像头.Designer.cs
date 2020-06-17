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
            this.pnlCamera = new System.Windows.Forms.Panel();
            this.btnStartVideo = new System.Windows.Forms.Button();
            this.btnStopVideo = new System.Windows.Forms.Button();
            this.btnShot = new System.Windows.Forms.Button();
            this.picShow = new System.Windows.Forms.PictureBox();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picShow)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
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
            // pnlCamera
            // 
            this.pnlCamera.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCamera.Location = new System.Drawing.Point(0, 0);
            this.pnlCamera.Name = "pnlCamera";
            this.pnlCamera.Size = new System.Drawing.Size(600, 450);
            this.pnlCamera.TabIndex = 1;
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
            // picShow
            // 
            this.picShow.Location = new System.Drawing.Point(3, 286);
            this.picShow.Name = "picShow";
            this.picShow.Size = new System.Drawing.Size(194, 152);
            this.picShow.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.picShow.TabIndex = 3;
            this.picShow.TabStop = false;
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
    }
}