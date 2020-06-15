namespace 基于Zxing的二维码实践
{
    partial class 摄像头识别二维码
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
            this.pnlBottom = new System.Windows.Forms.Panel();
            this.cbMulti = new System.Windows.Forms.CheckBox();
            this.lbTip = new System.Windows.Forms.Label();
            this.btnDecode = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.pnlBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBottom
            // 
            this.pnlBottom.Controls.Add(this.cbMulti);
            this.pnlBottom.Controls.Add(this.lbTip);
            this.pnlBottom.Controls.Add(this.btnDecode);
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Location = new System.Drawing.Point(0, 530);
            this.pnlBottom.Name = "pnlBottom";
            this.pnlBottom.Size = new System.Drawing.Size(890, 50);
            this.pnlBottom.TabIndex = 0;
            // 
            // cbMulti
            // 
            this.cbMulti.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cbMulti.AutoSize = true;
            this.cbMulti.Location = new System.Drawing.Point(725, 19);
            this.cbMulti.Name = "cbMulti";
            this.cbMulti.Size = new System.Drawing.Size(72, 16);
            this.cbMulti.TabIndex = 2;
            this.cbMulti.Text = "多二维码";
            this.cbMulti.UseVisualStyleBackColor = true;
            // 
            // lbTip
            // 
            this.lbTip.AutoSize = true;
            this.lbTip.Location = new System.Drawing.Point(12, 20);
            this.lbTip.Name = "lbTip";
            this.lbTip.Size = new System.Drawing.Size(53, 12);
            this.lbTip.TabIndex = 1;
            this.lbTip.Text = "显示结果";
            // 
            // btnDecode
            // 
            this.btnDecode.Location = new System.Drawing.Point(803, 15);
            this.btnDecode.Name = "btnDecode";
            this.btnDecode.Size = new System.Drawing.Size(75, 23);
            this.btnDecode.TabIndex = 0;
            this.btnDecode.Text = "识别";
            this.btnDecode.UseVisualStyleBackColor = true;
            this.btnDecode.Click += new System.EventHandler(this.btnDecode_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 0);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(890, 530);
            this.pnlContent.TabIndex = 1;
            // 
            // 摄像头识别二维码
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 580);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlBottom);
            this.Name = "摄像头识别二维码";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "摄像头识别二维码";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.摄像头识别二维码_FormClosing);
            this.Load += new System.EventHandler(this.摄像头识别二维码_Load);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Button btnDecode;
        private System.Windows.Forms.Label lbTip;
        private System.Windows.Forms.CheckBox cbMulti;
    }
}