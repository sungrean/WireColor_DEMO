using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

enum CH_MODE
{
    STOP = 0,
    WORK = 1,
    LEARN = 2,
    CH_OFF = 3,
    PASS = 4,
    ERR = 8
};

namespace IMG128
{
    public partial class IMG128 : Form
    {
        DevCfgDlg dlgDevCfg;
        public Protocol devProtocol = new Protocol();
        public REC devRec = new REC();
        public CFG_T devCfg = new CFG_T();
        long openDevCfgDlgTimeOut;

        const int RCV_BUF_SIZE = 1000000;
        const String bmpFileName = "D:\\IMG128.png";
        //byte[] rcvBuf = new byte[RCV_BUF_SIZE];
        //int rcvCnt = 0;

        byte [] imgBuf = new byte[RCV_BUF_SIZE];
        
        const int imgScaleX = 4;
        const int imgScaleY = 1;
        const int imgWidth = imgScaleX * REC.PIX_NUM;
        const int imgHeight = imgWidth;

        Bitmap bmp = new Bitmap(imgWidth, imgHeight);


        public IMG128()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string [] ports = SerialPort.GetPortNames();
            cBoxCOMPORT.Items.AddRange(ports);
            cBoxCOMPORT.Text = ports[ports.Length-1];//"COM5";
            btnOpen_Click(this, e);

            timer1.Interval = 100;
            timer1.Enabled = true;
            this.Width = 1024;
            this.Height = 738;
            pictureBox1.Width = imgWidth;
            pictureBox1.Height = imgHeight;

            //timer1.AutoReset = true;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            try
            {
                serialPort1.PortName = cBoxCOMPORT.Text;
                serialPort1.BaudRate = 115200;
                serialPort1.DataBits = 8;
                serialPort1.StopBits = StopBits.One;
                serialPort1.Parity = Parity.None;

                serialPort1.Open();
                progressBar1.Value = 100;
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                serialPort1.Close();
                    
                progressBar1.Value = 0;
            }

        }

        private void Receive(object sender, SerialDataReceivedEventArgs e)
        {
            
            int cnt = serialPort1.BytesToRead;
            byte[] rcvBuf = new byte[cnt];
            //if(rcvCnt + cnt >= RCV_BUF_SIZE)
            //    rcvCnt = 0;
            serialPort1.Read(rcvBuf, 0, cnt);
            devProtocol.RxBuffAdd(rcvBuf, cnt);
        }
        
        private void btnLearn_Click(object sender, EventArgs e) //学习按钮
        {
            //Get cofig from device
            if (serialPort1.IsOpen)
            {
                byte[] mode = new byte[Marshal.SizeOf(typeof(UInt32))];
                mode[0] = (byte)CH_MODE.LEARN;
                byte[] frm = devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_SM,mode, (ushort)Marshal.SizeOf(typeof(UInt32)));
                serialPort1.Write(frm, 0, frm.Length);
            }
            else
                MessageBox.Show("请先打开串口。");
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

            
        private void 参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get cofig from device
            if (serialPort1.IsOpen)
            {
                byte[] frm = devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_GC);
                serialPort1.Write(frm, 0, frm.Length);
                openDevCfgDlgTimeOut = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;     //时间戳
                openDevCfgDlgTimeOut += 1000;   //timeout is 1000 ms later
            }
            else
                MessageBox.Show("请先打开串口。");
        }

        private void 导入配置文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 导出配置文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 修改密码ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 报警复位ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 选择串口ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void 版本信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        //将记录结构体中的图片绘制到窗口
        private void DrawRecImg(REC_ITEM rec)
        {
            int idx = 0;            //取图片中数据计数器。图片每行128个像素，保存在4个UInt32型数据中，每个bit表示一个像素。每取一行数据，idx加4，指向下一行
            uint w = rec.size.x;    //图片宽度 = 128
            uint h = rec.size.y;    //图片高度 = 80
            int x, y;
            Bitmap bmpOrg = new Bitmap(REC.PIX_NUM, REC.IMG_HEIGHT);    //保存记录中的原始图片。显示在界面上的图片是经过放大后的，保存在bmp中

            for (y = 0; y < h; y++)             //循环读取每一行
            {
                int[] line = new int[w];        //保存一行数据。将一行中每个像素展开，保存到line[]中
                int cnt = 0;                    //行内像素计数器，line[cnt]

                //从记录中取一行数据
                UInt32[] data = new UInt32[4];  //保存一行数据
                for (int i = 0; i < 4; i++)
                    data[i] = rec.img[i + idx];
                idx += 4;                       //指向下一行数据

                //将4个UInt32型数据展开到128个数据的数组line[]，表示一行数据
                for (int i = 0; i < 4; i++)
                {
                    for (int j = 0; j < 32; j++)
                    {
                        if ((data[i] & 0x00000001) != 0)
                        {
                            line[cnt] = 1;
                        }
                        else
                        {
                            line[cnt] = 0;
                        }
                        data[i] >>= 1;
                        cnt++;
                    }
                }
                
                //将这一行数据写入到bmpOrg中的第y行
                for (x = 0; x < w; x++)
                {
                    if (0 == line[x])
                        bmpOrg.SetPixel(x, y, Color.White);
                    else
                        bmpOrg.SetPixel(x, y, Color.Green);
                }
            }

            //scale img
            int xs, ys, xd, yd;     //xs ys 源图宽高   xd yd 目标图宽高
            int ydOffset = (imgHeight - y * imgScaleY) / 2;   //图片纵向居中
            for (yd = 0; yd < imgHeight; yd++)
            {
                for (xd = 0; xd < imgWidth; xd++)
                {
                    xs = xd / imgScaleX;         //源图中x
                    ys = yd / imgScaleY;         //源图中y

                    if (ys >= bmpOrg.Height)
                        break;

                    Color clr = bmpOrg.GetPixel(xs, ys);    //获取源图中像素
                    bmp.SetPixel(xd, yd + ydOffset , clr);  //填充到目标图中
                }
            }

            pictureBox1.Image = bmp;        //将放大后的图片显示到界面
        }

        //Show Device configuration dialog
        void OpenDevCfgDlg()
        {
            if (null == dlgDevCfg)
                dlgDevCfg = new DevCfgDlg(this);
            if (dlgDevCfg.IsDisposed)
                dlgDevCfg = new DevCfgDlg(this);
            dlgDevCfg.Show();

            openDevCfgDlgTimeOut = 0;
        }

        //Show Record
        void ShowRec(REC_ITEM rec)
        {
            DrawRecImg(rec);    //显示记录录的图片

            //显示记录中的数据
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            string tmp = string.Format("基准序号： {0}\r\n", rec.modelIdx);
            sb.Append(tmp);
            tmp = string.Format("判定结果   ：{0:X}", rec.result);
            sb.Append(tmp);
            tmp = string.Format("判定项目   ：{0:X}\r\n", rec.analEN);
            sb.Append(tmp);
            tmp = string.Format("线端位置   ：最大:{0}  最小:{1}  基准:{2} 检测:{3}\r\n", rec.corePosMax * REC.PIX_SIZE, rec.corePosMin * REC.PIX_SIZE, rec.corePosStd * REC.PIX_SIZE, rec.corePos * REC.PIX_SIZE);
            sb.Append(tmp);
            tmp = string.Format("线径       ：最大:{0}  最小:{1}  基准:{2} 检测:{3}\r\n", rec.coreWidthMax, rec.coreWidthMin, rec.coreWidthStd, rec.coreWidth);
            sb.Append(tmp);
            tmp = string.Format("防水栓位置 ：最大:{0}  最小:{1}  基准:{2} 检测:{3}\r\n", rec.sealPosMax * REC.PIX_SIZE, rec.sealPosMin * REC.PIX_SIZE, rec.sealPosStd * REC.PIX_SIZE, rec.sealPos * REC.PIX_SIZE);
            sb.Append(tmp);
            tmp = string.Format("防水栓直径 ：最大:{0}  最小:{1}  基准:{2} 检测:{3}\r\n", rec.sealWidthMax, rec.sealWidthMin, rec.sealWidthStd, rec.sealWidth);
            sb.Append(tmp);
            tmp = string.Format("剥切位置   ：最大:{0}  最小:{1}  基准:{2} 检测:{3}\r\n", rec.stripPosMax * REC.PIX_SIZE, rec.stripPosMin * REC.PIX_SIZE, rec.stripPosStd * REC.PIX_SIZE, rec.stripPos * REC.PIX_SIZE);
            sb.Append(tmp);
            tmp = string.Format("剥切长度   ：最大:{0}  最小:{1}  基准:{2} 检测:{3}\r\n", rec.stripLenMax * REC.PIX_SIZE, rec.stripLenMin * REC.PIX_SIZE, rec.stripLenStd * REC.PIX_SIZE, rec.stripLen * REC.PIX_SIZE);
            sb.Append(tmp);
            tmp = string.Format("分叉宽度   ：最大:{0}  最小:{1}  基准:{2} 检测:{3}\r\n", rec.splayMax, rec.splayMin, rec.splayStd, rec.splay);
            sb.Append(tmp);

            if(0 != rec.isSeal)
                tmp = string.Format("有无防水栓 ：有\r\n");
            else
                tmp = string.Format("有无防水栓 ：无\r\n");
            sb.Append(tmp);

            tmp = string.Format("导线外径   ：{0:f}\r\n", rec.refWidth);
            sb.Append(tmp);
            tmp = string.Format("缩放比例   ：{0:f}\r\n", rec.yScale);
            sb.Append(tmp);

            string str = sb.ToString();
            rtBox1.Text = str;
        }


        //串口数据帧中type的定义，UInt16型，保存两个字节ASCII码
        const ushort FRAME_TYPE_LI = 0x494C;	//"LI" Login
        const ushort FRAME_TYPE_LO = 0x4F4C;	//"LO" Logout
        const ushort FRAME_TYPE_SM = 0x4D53;	//"SM" SetMode
        const ushort FRAME_TYPE_GC = 0x4347;	//"GC" GetConfig
        const ushort FRAME_TYPE_SC = 0x4353;	//"SC" SetConfig 
        const ushort FRAME_TYPE_GR = 0x5247;	//"GR" Get record
        const ushort FRAME_TYPE_CR = 0x5243;	//"CR" clr record
        const ushort FRAME_TYPE_GS = 0x5347;	//"GS" Get state
        const ushort FRAME_TYPE_RA = 0x4152;	//"RA" Reset Alarm
        const ushort FRAME_TYPE_RP = 0x5052;	//"RP" Report
        const ushort FRAME_TYPE_AL = 0x4C41;	//"AL" Alarm
        const ushort FRAME_TYPE_CE = 0x4543;	//"CE" Check ENC
        const ushort FRAME_TYPE_SN = 0x4E53;	//"SN" Get SN
        const ushort FRAME_TYPE_RG = 0x4752;	//"RG" Registration
        const ushort FRAME_TYPE_RC = 0x4352;	//"RC" Reset counter
        const ushort FRAME_TYPE_ST = 0x5453;	//"ST" Set RTC Time
        const ushort FRAME_TYPE_MS = 0x534D;	//"MS" Mode select
        const ushort FRAME_TYPE_FS = 0x5346;	//"FS" Factory Setting
        const ushort FRAME_TYPE_DT = 0x5444;	//"DT" Start Detection
        const ushort FRAME_TYPE_NK = 0x4E4B;	//"NK" Device busy

        //100ms timer
        COMM_FRAME_T frameRX = new COMM_FRAME_T();  //保存串口收到的数据
        private void timer1_Tick(object sender, EventArgs e)
        {
            long ms = (DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000;     //时间戳;

            if (openDevCfgDlgTimeOut > 0)       // is waiting device response
            {
                if (ms > openDevCfgDlgTimeOut)  //timeout
                {
                    openDevCfgDlgTimeOut = 0;
                    MessageBox.Show("等待设备响应超时");
                    OpenDevCfgDlg();

                }
            }

            while (devProtocol.GetRxFrame(ref frameRX))
            {
                ushort type = frameRX.type;
                type -= 0x2020; //两个字符转为大写
                switch (type)
                {
                    case FRAME_TYPE_RP: ShowRec(devRec.RecFrameDeal(frameRX)); break;
                    case FRAME_TYPE_NK: MessageBox.Show("设备忙"); break;
                    case FRAME_TYPE_GC:
                        if (null == dlgDevCfg)
                            dlgDevCfg = new DevCfgDlg(this);
                        if (dlgDevCfg.IsDisposed)
                            dlgDevCfg = new DevCfgDlg(this);

                        dlgDevCfg.CfgFreameDeal(frameRX);
                        dlgDevCfg.UpdateShow(devCfg);
                        if (openDevCfgDlgTimeOut > 0)
                        {
                            dlgDevCfg.Show();
                            openDevCfgDlgTimeOut = 0;
                        }
                        break;
                    case FRAME_TYPE_SC: MessageBox.Show("更新设备信息成功！"); break;
                    
                    //case FRAME_TYPE_LI: COMM_cmdLI(port); break;
                    //case FRAME_TYPE_LO: COMM_cmdLO(port); break;
                    //case FRAME_TYPE_SM: COMM_cmdSM(port, p); break;
                    //case FRAME_TYPE_GR: COMM_cmdGR(port); break;
                    //case FRAME_TYPE_CR: COMM_cmdCR(port); break;
                    //case FRAME_TYPE_GS: COMM_cmdGS(); break;
                    //case FRAME_TYPE_RA: COMM_cmdRA(port, p); break;
                    //case FRAME_TYPE_RC: COMM_cmdRC(port); break;		//20180902 清除计数器
                    //case FRAME_TYPE_ST: COMM_cmdST(port, p); break;	//20180902 同步系统时间
                    //case FRAME_TYPE_MS: COMM_cmdMS(port, p); break;	//20190618 选择基准波形
                    //case FRAME_TYPE_FS: COMM_cmdFS(port, p); break;	//20190722 厂家设置
                    //case FRAME_TYPE_DT: COMM_cmdDT(); break;	//20190722 厂家设置

                    default: break;
                }

                //if (isNewImg)
                //{
                //    //DrawImg(imgCnt);
                //    isNewImg = false;
                //}
            }
        }


    }
}
