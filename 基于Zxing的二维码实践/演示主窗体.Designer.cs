namespace 基于Zxing的二维码实践
{
    partial class 演示主窗体
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnCreateQrCodeDemo = new System.Windows.Forms.Button();
            this.btnCreateBarCode = new System.Windows.Forms.Button();
            this.btnDecodeCodeFromCamera = new System.Windows.Forms.Button();
            this.btnDecodeCodeFromImage = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.btnCreateQrCodeDemo);
            this.flowLayoutPanel1.Controls.Add(this.btnCreateBarCode);
            this.flowLayoutPanel1.Controls.Add(this.btnDecodeCodeFromCamera);
            this.flowLayoutPanel1.Controls.Add(this.btnDecodeCodeFromImage);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // btnCreateQrCodeDemo
            // 
            this.btnCreateQrCodeDemo.Location = new System.Drawing.Point(3, 3);
            this.btnCreateQrCodeDemo.Name = "btnCreateQrCodeDemo";
            this.btnCreateQrCodeDemo.Size = new System.Drawing.Size(785, 35);
            this.btnCreateQrCodeDemo.TabIndex = 0;
            this.btnCreateQrCodeDemo.Text = "生成二维码";
            this.btnCreateQrCodeDemo.UseVisualStyleBackColor = true;
            this.btnCreateQrCodeDemo.Click += new System.EventHandler(this.btnCreateQrCodeDemo_Click);
            // 
            // btnCreateBarCode
            // 
            this.btnCreateBarCode.Location = new System.Drawing.Point(3, 44);
            this.btnCreateBarCode.Name = "btnCreateBarCode";
            this.btnCreateBarCode.Size = new System.Drawing.Size(785, 35);
            this.btnCreateBarCode.TabIndex = 1;
            this.btnCreateBarCode.Text = "生成条形码";
            this.btnCreateBarCode.UseVisualStyleBackColor = true;
            this.btnCreateBarCode.Click += new System.EventHandler(this.btnCreateBarCode_Click);
            // 
            // btnDecodeCodeFromCamera
            // 
            this.btnDecodeCodeFromCamera.Location = new System.Drawing.Point(3, 85);
            this.btnDecodeCodeFromCamera.Name = "btnDecodeCodeFromCamera";
            this.btnDecodeCodeFromCamera.Size = new System.Drawing.Size(785, 35);
            this.btnDecodeCodeFromCamera.TabIndex = 2;
            this.btnDecodeCodeFromCamera.Text = "摄像头识别二维码或者条形码";
            this.btnDecodeCodeFromCamera.UseVisualStyleBackColor = true;
            this.btnDecodeCodeFromCamera.Click += new System.EventHandler(this.btnDecodeCodeFromCamera_Click);
            // 
            // btnDecodeCodeFromImage
            // 
            this.btnDecodeCodeFromImage.Location = new System.Drawing.Point(3, 126);
            this.btnDecodeCodeFromImage.Name = "btnDecodeCodeFromImage";
            this.btnDecodeCodeFromImage.Size = new System.Drawing.Size(785, 35);
            this.btnDecodeCodeFromImage.TabIndex = 3;
            this.btnDecodeCodeFromImage.Text = "识别图片中的二维码或者条形码";
            this.btnDecodeCodeFromImage.UseVisualStyleBackColor = true;
            this.btnDecodeCodeFromImage.Click += new System.EventHandler(this.btnDecodeCodeFromImage_Click);
            // 
            // 演示主窗体
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "演示主窗体";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "演示主窗体";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCreateQrCodeDemo;
        private System.Windows.Forms.Button btnCreateBarCode;
        private System.Windows.Forms.Button btnDecodeCodeFromCamera;
        private System.Windows.Forms.Button btnDecodeCodeFromImage;
    }
}

