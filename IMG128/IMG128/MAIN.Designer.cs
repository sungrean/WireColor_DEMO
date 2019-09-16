namespace IMG128
{
    partial class IMG128
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
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.cBoxCOMPORT = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOpen = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.打开ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.参数设置ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导入配置文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.导出配置文件ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.修改密码ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.报警ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.报警复位ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选项ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.选择串口ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.版本信息ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.rtBox1 = new System.Windows.Forms.RichTextBox();
            this.btnLearn = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // cBoxCOMPORT
            // 
            this.cBoxCOMPORT.FormattingEnabled = true;
            this.cBoxCOMPORT.Location = new System.Drawing.Point(78, 20);
            this.cBoxCOMPORT.Name = "cBoxCOMPORT";
            this.cBoxCOMPORT.Size = new System.Drawing.Size(57, 20);
            this.cBoxCOMPORT.TabIndex = 0;
            this.cBoxCOMPORT.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "COM PORT";
            this.label1.Click += new System.EventHandler(this.label1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.progressBar1);
            this.groupBox1.Controls.Add(this.btnClose);
            this.groupBox1.Controls.Add(this.btnOpen);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.cBoxCOMPORT);
            this.groupBox1.Location = new System.Drawing.Point(847, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(161, 178);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "COM PORT";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(21, 83);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(113, 23);
            this.progressBar1.TabIndex = 4;
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(83, 54);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(52, 20);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnOpen
            // 
            this.btnOpen.Location = new System.Drawing.Point(20, 54);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(52, 20);
            this.btnOpen.TabIndex = 2;
            this.btnOpen.Text = "Open";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.pictureBox1.Location = new System.Drawing.Point(12, 28);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(512, 512);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.设置ToolStripMenuItem,
            this.报警ToolStripMenuItem,
            this.选项ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1008, 25);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.打开ToolStripMenuItem,
            this.保存ToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(44, 21);
            this.toolStripMenuItem1.Text = "文件";
            // 
            // 打开ToolStripMenuItem
            // 
            this.打开ToolStripMenuItem.Name = "打开ToolStripMenuItem";
            this.打开ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.打开ToolStripMenuItem.Text = "打开";
            this.打开ToolStripMenuItem.Click += new System.EventHandler(this.打开ToolStripMenuItem_Click);
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.保存ToolStripMenuItem.Text = "保存";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem_Click);
            // 
            // 设置ToolStripMenuItem
            // 
            this.设置ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.参数设置ToolStripMenuItem,
            this.导入配置文件ToolStripMenuItem,
            this.导出配置文件ToolStripMenuItem,
            this.修改密码ToolStripMenuItem});
            this.设置ToolStripMenuItem.Name = "设置ToolStripMenuItem";
            this.设置ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.设置ToolStripMenuItem.Text = "设置";
            // 
            // 参数设置ToolStripMenuItem
            // 
            this.参数设置ToolStripMenuItem.Name = "参数设置ToolStripMenuItem";
            this.参数设置ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.参数设置ToolStripMenuItem.Text = "参数设置";
            this.参数设置ToolStripMenuItem.Click += new System.EventHandler(this.参数设置ToolStripMenuItem_Click);
            // 
            // 导入配置文件ToolStripMenuItem
            // 
            this.导入配置文件ToolStripMenuItem.Name = "导入配置文件ToolStripMenuItem";
            this.导入配置文件ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.导入配置文件ToolStripMenuItem.Text = "导入配置文件";
            this.导入配置文件ToolStripMenuItem.Click += new System.EventHandler(this.导入配置文件ToolStripMenuItem_Click);
            // 
            // 导出配置文件ToolStripMenuItem
            // 
            this.导出配置文件ToolStripMenuItem.Name = "导出配置文件ToolStripMenuItem";
            this.导出配置文件ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.导出配置文件ToolStripMenuItem.Text = "导出配置文件";
            this.导出配置文件ToolStripMenuItem.Click += new System.EventHandler(this.导出配置文件ToolStripMenuItem_Click);
            // 
            // 修改密码ToolStripMenuItem
            // 
            this.修改密码ToolStripMenuItem.Name = "修改密码ToolStripMenuItem";
            this.修改密码ToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.修改密码ToolStripMenuItem.Text = "修改密码";
            this.修改密码ToolStripMenuItem.Click += new System.EventHandler(this.修改密码ToolStripMenuItem_Click);
            // 
            // 报警ToolStripMenuItem
            // 
            this.报警ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.报警复位ToolStripMenuItem});
            this.报警ToolStripMenuItem.Name = "报警ToolStripMenuItem";
            this.报警ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.报警ToolStripMenuItem.Text = "报警";
            // 
            // 报警复位ToolStripMenuItem
            // 
            this.报警复位ToolStripMenuItem.Name = "报警复位ToolStripMenuItem";
            this.报警复位ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.报警复位ToolStripMenuItem.Text = "报警复位";
            this.报警复位ToolStripMenuItem.Click += new System.EventHandler(this.报警复位ToolStripMenuItem_Click);
            // 
            // 选项ToolStripMenuItem
            // 
            this.选项ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.选择串口ToolStripMenuItem,
            this.版本信息ToolStripMenuItem});
            this.选项ToolStripMenuItem.Name = "选项ToolStripMenuItem";
            this.选项ToolStripMenuItem.Size = new System.Drawing.Size(44, 21);
            this.选项ToolStripMenuItem.Text = "选项";
            // 
            // 选择串口ToolStripMenuItem
            // 
            this.选择串口ToolStripMenuItem.Name = "选择串口ToolStripMenuItem";
            this.选择串口ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.选择串口ToolStripMenuItem.Text = "选择串口";
            this.选择串口ToolStripMenuItem.Click += new System.EventHandler(this.选择串口ToolStripMenuItem_Click);
            // 
            // 版本信息ToolStripMenuItem
            // 
            this.版本信息ToolStripMenuItem.Name = "版本信息ToolStripMenuItem";
            this.版本信息ToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.版本信息ToolStripMenuItem.Text = "版本信息";
            this.版本信息ToolStripMenuItem.Click += new System.EventHandler(this.版本信息ToolStripMenuItem_Click);
            // 
            // serialPort1
            // 
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.Receive);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // rtBox1
            // 
            this.rtBox1.Location = new System.Drawing.Point(545, 236);
            this.rtBox1.Name = "rtBox1";
            this.rtBox1.Size = new System.Drawing.Size(436, 304);
            this.rtBox1.TabIndex = 3;
            this.rtBox1.Text = "";
            // 
            // btnLearn
            // 
            this.btnLearn.Location = new System.Drawing.Point(545, 28);
            this.btnLearn.Name = "btnLearn";
            this.btnLearn.Size = new System.Drawing.Size(113, 20);
            this.btnLearn.TabIndex = 6;
            this.btnLearn.Text = "学习";
            this.btnLearn.UseVisualStyleBackColor = true;
            this.btnLearn.Click += new System.EventHandler(this.btnLearn_Click);
            // 
            // IMG128
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 699);
            this.Controls.Add(this.btnLearn);
            this.Controls.Add(this.rtBox1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "IMG128";
            this.Text = "IMG128x";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cBoxCOMPORT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 打开ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 参数设置ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导入配置文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 导出配置文件ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 修改密码ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 报警ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 报警复位ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选项ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 选择串口ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 版本信息ToolStripMenuItem;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.RichTextBox rtBox1;
        public System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.Button btnLearn;

    }
}

