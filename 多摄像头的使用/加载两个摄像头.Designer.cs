namespace 多摄像头的使用
{
    partial class 加载两个摄像头
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
            this.pnlVideo1 = new System.Windows.Forms.Panel();
            this.pnlVideo2 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(600, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 450);
            this.panel1.TabIndex = 0;
            // 
            // pnlVideo1
            // 
            this.pnlVideo1.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlVideo1.Location = new System.Drawing.Point(0, 0);
            this.pnlVideo1.Name = "pnlVideo1";
            this.pnlVideo1.Size = new System.Drawing.Size(600, 221);
            this.pnlVideo1.TabIndex = 1;
            // 
            // pnlVideo2
            // 
            this.pnlVideo2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlVideo2.Location = new System.Drawing.Point(0, 227);
            this.pnlVideo2.Name = "pnlVideo2";
            this.pnlVideo2.Size = new System.Drawing.Size(600, 223);
            this.pnlVideo2.TabIndex = 2;
            // 
            // 加载两个摄像头
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.pnlVideo2);
            this.Controls.Add(this.pnlVideo1);
            this.Controls.Add(this.panel1);
            this.Name = "加载两个摄像头";
            this.Text = "加载两个摄像头";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.加载两个摄像头_FormClosing);
            this.Load += new System.EventHandler(this.加载两个摄像头_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel pnlVideo1;
        private System.Windows.Forms.Panel pnlVideo2;
    }
}