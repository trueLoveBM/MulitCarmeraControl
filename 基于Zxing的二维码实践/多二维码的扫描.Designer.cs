namespace 基于Zxing的二维码实践
{
    partial class 多二维码的扫描
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
            this.txtBarcodeImageFile = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectBarcodeImageFileForDecoding = new System.Windows.Forms.Button();
            this.btnDecode = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbResult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtBarcodeImageFile
            // 
            this.txtBarcodeImageFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBarcodeImageFile.Location = new System.Drawing.Point(63, 6);
            this.txtBarcodeImageFile.Name = "txtBarcodeImageFile";
            this.txtBarcodeImageFile.Size = new System.Drawing.Size(418, 21);
            this.txtBarcodeImageFile.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "选择图片";
            // 
            // btnSelectBarcodeImageFileForDecoding
            // 
            this.btnSelectBarcodeImageFileForDecoding.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSelectBarcodeImageFileForDecoding.Location = new System.Drawing.Point(493, 4);
            this.btnSelectBarcodeImageFileForDecoding.Name = "btnSelectBarcodeImageFileForDecoding";
            this.btnSelectBarcodeImageFileForDecoding.Size = new System.Drawing.Size(26, 23);
            this.btnSelectBarcodeImageFileForDecoding.TabIndex = 4;
            this.btnSelectBarcodeImageFileForDecoding.Text = "...";
            this.btnSelectBarcodeImageFileForDecoding.UseVisualStyleBackColor = true;
            this.btnSelectBarcodeImageFileForDecoding.Click += new System.EventHandler(this.btnSelectBarcodeImageFileForDecoding_Click);
            // 
            // btnDecode
            // 
            this.btnDecode.Location = new System.Drawing.Point(461, 38);
            this.btnDecode.Name = "btnDecode";
            this.btnDecode.Size = new System.Drawing.Size(58, 23);
            this.btnDecode.TabIndex = 5;
            this.btnDecode.Text = "解析";
            this.btnDecode.UseVisualStyleBackColor = true;
            this.btnDecode.Click += new System.EventHandler(this.btnDecode_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 70);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "解析结果：";
            // 
            // tbResult
            // 
            this.tbResult.Location = new System.Drawing.Point(63, 67);
            this.tbResult.Multiline = true;
            this.tbResult.Name = "tbResult";
            this.tbResult.ReadOnly = true;
            this.tbResult.Size = new System.Drawing.Size(456, 362);
            this.tbResult.TabIndex = 7;
            // 
            // 多二维码的扫描
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(531, 441);
            this.Controls.Add(this.tbResult);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnDecode);
            this.Controls.Add(this.btnSelectBarcodeImageFileForDecoding);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBarcodeImageFile);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "多二维码的扫描";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "多二维码的扫描";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.多二维码的扫描_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtBarcodeImageFile;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectBarcodeImageFileForDecoding;
        private System.Windows.Forms.Button btnDecode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbResult;
    }
}