namespace IMG128
{
    partial class DevCfgDlg
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
            this.btUploadCfg = new System.Windows.Forms.Button();
            this.btUpdateCfg = new System.Windows.Forms.Button();
            this.btOpenProfile = new System.Windows.Forms.Button();
            this.btSaveProfile = new System.Windows.Forms.Button();
            this.ModelWireSeperate = new System.Windows.Forms.CheckBox();
            this.FailWhenReverse = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ClrErrThresh = new System.Windows.Forms.TextBox();
            this.clrAvrPercent = new System.Windows.Forms.TextBox();
            this.shadeThreah = new System.Windows.Forms.TextBox();
            this.widthRatio = new System.Windows.Forms.TextBox();
            this.clrAvrMin = new System.Windows.Forms.TextBox();
            this.seperationRatio = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btUploadCfg
            // 
            this.btUploadCfg.Location = new System.Drawing.Point(296, 425);
            this.btUploadCfg.Name = "btUploadCfg";
            this.btUploadCfg.Size = new System.Drawing.Size(105, 23);
            this.btUploadCfg.TabIndex = 18;
            this.btUploadCfg.Text = "读取设备配置";
            this.btUploadCfg.UseVisualStyleBackColor = true;
            this.btUploadCfg.Click += new System.EventHandler(this.btUploadCfg_Click);
            // 
            // btUpdateCfg
            // 
            this.btUpdateCfg.Location = new System.Drawing.Point(415, 425);
            this.btUpdateCfg.Name = "btUpdateCfg";
            this.btUpdateCfg.Size = new System.Drawing.Size(105, 23);
            this.btUpdateCfg.TabIndex = 19;
            this.btUpdateCfg.Text = "更新设备配置";
            this.btUpdateCfg.UseVisualStyleBackColor = true;
            this.btUpdateCfg.Click += new System.EventHandler(this.btUpdateCfg_Click);
            // 
            // btOpenProfile
            // 
            this.btOpenProfile.Location = new System.Drawing.Point(16, 425);
            this.btOpenProfile.Name = "btOpenProfile";
            this.btOpenProfile.Size = new System.Drawing.Size(105, 23);
            this.btOpenProfile.TabIndex = 20;
            this.btOpenProfile.Text = "打开配置文件";
            this.btOpenProfile.UseVisualStyleBackColor = true;
            // 
            // btSaveProfile
            // 
            this.btSaveProfile.Location = new System.Drawing.Point(127, 425);
            this.btSaveProfile.Name = "btSaveProfile";
            this.btSaveProfile.Size = new System.Drawing.Size(105, 23);
            this.btSaveProfile.TabIndex = 21;
            this.btSaveProfile.Text = "保存配置文件";
            this.btSaveProfile.UseVisualStyleBackColor = true;
            // 
            // ModelWireSeperate
            // 
            this.ModelWireSeperate.AutoSize = true;
            this.ModelWireSeperate.Location = new System.Drawing.Point(100, 51);
            this.ModelWireSeperate.Name = "ModelWireSeperate";
            this.ModelWireSeperate.Size = new System.Drawing.Size(132, 16);
            this.ModelWireSeperate.TabIndex = 22;
            this.ModelWireSeperate.Text = "要求每条基准线分开";
            this.ModelWireSeperate.UseVisualStyleBackColor = true;
            // 
            // FailWhenReverse
            // 
            this.FailWhenReverse.AutoSize = true;
            this.FailWhenReverse.Location = new System.Drawing.Point(345, 51);
            this.FailWhenReverse.Name = "FailWhenReverse";
            this.FailWhenReverse.Size = new System.Drawing.Size(84, 16);
            this.FailWhenReverse.TabIndex = 23;
            this.FailWhenReverse.Text = "反线序报警";
            this.FailWhenReverse.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 123);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 24;
            this.label1.Text = "判定遮挡上限：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(294, 123);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 12);
            this.label2.TabIndex = 25;
            this.label2.Text = "颜色判别灵敏度：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(294, 210);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(113, 12);
            this.label3.TabIndex = 27;
            this.label3.Text = "取色区占比最小值：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(41, 210);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 12);
            this.label4.TabIndex = 26;
            this.label4.Text = "有效取色区占比：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(294, 298);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 29;
            this.label5.Text = "重叠占比阈值：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(41, 298);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(89, 12);
            this.label6.TabIndex = 28;
            this.label6.Text = "间隔占比阈值：";
            // 
            // ClrErrThresh
            // 
            this.ClrErrThresh.Location = new System.Drawing.Point(416, 120);
            this.ClrErrThresh.Name = "ClrErrThresh";
            this.ClrErrThresh.Size = new System.Drawing.Size(100, 21);
            this.ClrErrThresh.TabIndex = 30;
            // 
            // clrAvrPercent
            // 
            this.clrAvrPercent.Location = new System.Drawing.Point(148, 207);
            this.clrAvrPercent.Name = "clrAvrPercent";
            this.clrAvrPercent.Size = new System.Drawing.Size(100, 21);
            this.clrAvrPercent.TabIndex = 31;
            // 
            // shadeThreah
            // 
            this.shadeThreah.Location = new System.Drawing.Point(148, 120);
            this.shadeThreah.Name = "shadeThreah";
            this.shadeThreah.Size = new System.Drawing.Size(100, 21);
            this.shadeThreah.TabIndex = 32;
            // 
            // widthRatio
            // 
            this.widthRatio.Location = new System.Drawing.Point(148, 295);
            this.widthRatio.Name = "widthRatio";
            this.widthRatio.Size = new System.Drawing.Size(100, 21);
            this.widthRatio.TabIndex = 33;
            // 
            // clrAvrMin
            // 
            this.clrAvrMin.Location = new System.Drawing.Point(416, 207);
            this.clrAvrMin.Name = "clrAvrMin";
            this.clrAvrMin.Size = new System.Drawing.Size(100, 21);
            this.clrAvrMin.TabIndex = 34;
            // 
            // seperationRatio
            // 
            this.seperationRatio.Location = new System.Drawing.Point(416, 295);
            this.seperationRatio.Name = "seperationRatio";
            this.seperationRatio.Size = new System.Drawing.Size(100, 21);
            this.seperationRatio.TabIndex = 35;
            // 
            // DevCfgDlg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(538, 460);
            this.Controls.Add(this.seperationRatio);
            this.Controls.Add(this.clrAvrMin);
            this.Controls.Add(this.widthRatio);
            this.Controls.Add(this.shadeThreah);
            this.Controls.Add(this.clrAvrPercent);
            this.Controls.Add(this.ClrErrThresh);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.FailWhenReverse);
            this.Controls.Add(this.ModelWireSeperate);
            this.Controls.Add(this.btSaveProfile);
            this.Controls.Add(this.btOpenProfile);
            this.Controls.Add(this.btUpdateCfg);
            this.Controls.Add(this.btUploadCfg);
            this.Name = "DevCfgDlg";
            this.Text = "参数配置";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DevCfgDlg_FormClosing);
            this.Load += new System.EventHandler(this.DevCfgDlg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btUploadCfg;
        private System.Windows.Forms.Button btUpdateCfg;
        private System.Windows.Forms.Button btOpenProfile;
        private System.Windows.Forms.Button btSaveProfile;
        private System.Windows.Forms.CheckBox ModelWireSeperate;
        private System.Windows.Forms.CheckBox FailWhenReverse;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ClrErrThresh;
        private System.Windows.Forms.TextBox clrAvrPercent;
        private System.Windows.Forms.TextBox shadeThreah;
        private System.Windows.Forms.TextBox widthRatio;
        private System.Windows.Forms.TextBox clrAvrMin;
        private System.Windows.Forms.TextBox seperationRatio;
    }
}