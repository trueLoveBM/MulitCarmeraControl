namespace 基于Zxing的二维码实践
{
    partial class 生成条形码
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
            this.components = new System.ComponentModel.Container();
            this.panelBase1 = new Base.ControlBase.PanelBase(this.components);
            this.pbQrCode = new System.Windows.Forms.PictureBox();
            this.btnCreateQrCode = new System.Windows.Forms.Button();
            this.tbContent = new System.Windows.Forms.TextBox();
            this.panelBase1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbQrCode)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBase1
            // 
            this.panelBase1.AllowZoom = true;
            this.panelBase1.Controls.Add(this.pbQrCode);
            this.panelBase1.Controls.Add(this.btnCreateQrCode);
            this.panelBase1.Controls.Add(this.tbContent);
            this.panelBase1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBase1.Font = new System.Drawing.Font("微软雅黑", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.panelBase1.Location = new System.Drawing.Point(0, 0);
            this.panelBase1.Name = "panelBase1";
            this.panelBase1.Size = new System.Drawing.Size(763, 419);
            this.panelBase1.TabIndex = 0;
            // 
            // pbQrCode
            // 
            this.pbQrCode.Location = new System.Drawing.Point(251, 156);
            this.pbQrCode.Name = "pbQrCode";
            this.pbQrCode.Padding = new System.Windows.Forms.Padding(10);
            this.pbQrCode.Size = new System.Drawing.Size(253, 92);
            this.pbQrCode.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbQrCode.TabIndex = 2;
            this.pbQrCode.TabStop = false;
            this.pbQrCode.Visible = false;
            // 
            // btnCreateQrCode
            // 
            this.btnCreateQrCode.Location = new System.Drawing.Point(334, 376);
            this.btnCreateQrCode.Name = "btnCreateQrCode";
            this.btnCreateQrCode.Size = new System.Drawing.Size(96, 31);
            this.btnCreateQrCode.TabIndex = 1;
            this.btnCreateQrCode.Text = "生成条形码";
            this.btnCreateQrCode.UseVisualStyleBackColor = true;
            this.btnCreateQrCode.Click += new System.EventHandler(this.btnCreateQrCode_Click);
            // 
            // tbContent
            // 
            this.tbContent.Location = new System.Drawing.Point(12, 12);
            this.tbContent.Multiline = true;
            this.tbContent.Name = "tbContent";
            this.tbContent.Size = new System.Drawing.Size(739, 358);
            this.tbContent.TabIndex = 0;
            // 
            // 生成条形码
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(763, 419);
            this.Controls.Add(this.panelBase1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "生成条形码";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "生成条形码";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.生成条形码_FormClosing);
            this.panelBase1.ResumeLayout(false);
            this.panelBase1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbQrCode)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Base.ControlBase.PanelBase panelBase1;
        private System.Windows.Forms.TextBox tbContent;
        private System.Windows.Forms.Button btnCreateQrCode;
        private System.Windows.Forms.PictureBox pbQrCode;
    }
}