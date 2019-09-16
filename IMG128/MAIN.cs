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
    ADJUSTMENT = 3,
};
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct WC_DEV_CFG
{
    public UInt32 valid;				//记录有效性标识
    public bool ModelWireSeperate;      //是否要求基准线型必需每条线分开
    public int shadeThreah;             //line[0] line[4] 中值小于SHADE_THRESH认为是被遮挡区域
    public bool FailWhenReverse;        //反线序报警   
    public int ClrErrThresh;            //颜色判别灵敏度
    public int clrAvrPercent;           //取颜色值时，使用线宽中间部分的均值作为颜色值。此项设置均值部分宽度占线宽的比例
    public int clrAvrMin;               //取颜色值时，使用线宽中间部分的均值作为颜色值。当按clrAvrPercent计算宽度小于clrAvrMin时，以clrAvrMin计算均值
    public Single widthRatio;           //判别是否各导线分开的阈值
    public Single seperationRatio;      //多条导线并在一起时，拆分各导线宽度时使用的阈值 0.0-0.5之间，值越小约束条件越强。>=0.5时无此约束
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    public UInt32[] reserved;					//备以后扩展
};
namespace IMG128
{


    public partial class IMG128 : Form
    {

        DevCfgDlg dlgDevCfg;
        public REC devRec = new REC(); 
        public Protocol devProtocol = new Protocol();
        long openDevCfgDlgTimeOut;

        const int RCV_BUF_SIZE = 1000000;
        const String bmpFileName = "D:\\IMG128.png";
        //byte[] rcvBuf = new byte[RCV_BUF_SIZE];
        //int rcvCnt = 0;

        byte [] imgBuf = new byte[RCV_BUF_SIZE];
        
        const int imgScaleX = 1;
        const int imgScaleY = 250;
        const int imgWidth = imgScaleX * REC.PIX_NUM;
        const int imgHeight = imgScaleY * REC.IMG_HEIGHT;

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

            timer1.Interval = 10;
            timer1.Enabled = true;
            //this.Width = 1920;
            //this.Height = 1024;
            this.WindowState = FormWindowState.Maximized;
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
        int clrDistance(Color c1, Color c2)
        {
            int ret = 0;
            int rmean = (c1.R + c2.R) / 2;
            int R = c1.R - c2.R;
            int G = c1.G - c2.G;
            int B = c1.B - c2.B;
            int dist = (2 + rmean / 256) * R ^ 2 + 4 * G ^ 2 + (2 + (255 - rmean) / 256) * B ^ 2;

            ret = (int)Math.Sqrt((double)dist);
            return ret;
        }
        //将记录结构体中的图片绘制到窗口
        private void DrawRecImg(REC_ITEM rec)
        {
            int w = REC.PIX_NUM;    //图片宽度 = 216 * 3
            int x, y;
            //Bitmap bmpOrg = new Bitmap(REC.PIX_NUM, REC.IMG_HEIGHT);    //保存记录中的原始图片。显示在界面上的图片是经过放大后的，保存在bmp中
            int h1 = 50;
            int h2 = 150;
            int h3 = 200;
            int h4 = 250;
            int h5 = 300;
            const int shadeThresh = 50;


            //shade1
            y = 0;
            for (x = 0; x < w; x++)
                bmp.SetPixel(x, y, Color.FromArgb(rec.img[0].pix[x], rec.img[0].pix[x], rec.img[0].pix[x]));
            for (; y < h1; y++)
                for (x = 0; x < w; x++)
                    bmp.SetPixel(x, y, bmp.GetPixel(x, 0));
            //color
            for (x = 0; x < w; x++)
                bmp.SetPixel(x, y, Color.FromArgb(rec.img[1].pix[x], rec.img[2].pix[x], rec.img[3].pix[x]));
            for (; y < h2; y++)
                for (x = 0; x < w; x++)
                    bmp.SetPixel(x, y, bmp.GetPixel(x, h1));

            //shade2
            for (x = 0; x < w; x++)
                bmp.SetPixel(x, y, Color.FromArgb(rec.img[4].pix[x], rec.img[4].pix[x], rec.img[4].pix[x]));
            for (; y < h3; y++)
                for (x = 0; x < w; x++)
                    bmp.SetPixel(x, y, bmp.GetPixel(x, h2));
  
            pictureBox1.Image = bmp;        //将放大后的图片显示到界面

            //画RGB曲线
            UInt16[] line = new UInt16[REC.PIX_NUM];

            int i;
            Graphics grfx = pBoxWaveR.CreateGraphics();
            int picHeight = pBoxWaveR.Height;

            Pen pen = new Pen(Color.Blue, 1);

            Point[] pts = new Point[REC.PIX_NUM];

            //R
            for (i = 0; i < REC.PIX_NUM; i++)
            {
                pts[i].X = i;
                if(rec.img[0].pix[i] < shadeThresh)
                    pts[i].Y = picHeight - rec.img[1].pix[i] / 2;
                else
                    pts[i].Y = picHeight-1;
            }
            grfx.Clear(Color.White);
            grfx.DrawLines(pen, pts);

            //G
            grfx = pBoxWaveG.CreateGraphics();
            for (i = 0; i < REC.PIX_NUM; i++)
            {
                pts[i].X = i;
                if (rec.img[0].pix[i] < shadeThresh)
                    pts[i].Y = picHeight - rec.img[2].pix[i] / 2;
                else
                    pts[i].Y = picHeight-1;
            }
            grfx.Clear(Color.White);
            grfx.DrawLines(pen, pts);

            //B
            grfx = pBoxWaveB.CreateGraphics();
            for (i = 0; i < REC.PIX_NUM; i++)
            {
                pts[i].X = i;
                if (rec.img[0].pix[i] < shadeThresh)
                    pts[i].Y = picHeight - rec.img[3].pix[i] / 2;
                else
                    pts[i].Y = picHeight-1;
            }
            grfx.Clear(Color.White);
            grfx.DrawLines(pen, pts);
            
            //RGB
            grfx = pBoxWave.CreateGraphics();
            for (i = 0; i < REC.PIX_NUM; i++)
            {
                pts[i].X = i;
                if (rec.img[0].pix[i] < shadeThresh)
                    pts[i].Y = picHeight - 1 - clrDistance(Color.FromArgb(255,rec.img[1].pix[i], rec.img[2].pix[i], rec.img[3].pix[i]), Color.Black);
                else
                    pts[i].Y = picHeight - 1;
            }
            grfx.Clear(Color.White);
            grfx.DrawLines(pen, pts);

            grfx.Dispose();

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
        }


        //串口数据帧中type的定义，UInt16型，保存两个字节ASCII码
        //const ushort FRAME_TYPE_LI = 0x494C;	//"LI" Login
        //const ushort FRAME_TYPE_LO = 0x4F4C;	//"LO" Logout
        //const ushort FRAME_TYPE_SM = 0x4D53;	//"SM" SetMode
        //const ushort FRAME_TYPE_GC = 0x4347;	//"GC" GetConfig
        //const ushort FRAME_TYPE_SC = 0x4353;	//"SC" SetConfig 
        //const ushort FRAME_TYPE_GR = 0x5247;	//"GR" Get record
        //const ushort FRAME_TYPE_CR = 0x5243;	//"CR" clr record
        //const ushort FRAME_TYPE_GS = 0x5347;	//"GS" Get state
        //const ushort FRAME_TYPE_RA = 0x4152;	//"RA" Reset Alarm
        //const ushort FRAME_TYPE_RP = 0x5052;	//"RP" Report
        //const ushort FRAME_TYPE_AL = 0x4C41;	//"AL" Alarm
        //const ushort FRAME_TYPE_CE = 0x4543;	//"CE" Check ENC
        //const ushort FRAME_TYPE_SN = 0x4E53;	//"SN" Get SN
        //const ushort FRAME_TYPE_RG = 0x4752;	//"RG" Registration
        //const ushort FRAME_TYPE_RC = 0x4352;	//"RC" Reset counter
        //const ushort FRAME_TYPE_ST = 0x5453;	//"ST" Set RTC Time
        //const ushort FRAME_TYPE_MS = 0x534D;	//"MS" Mode select
        //const ushort FRAME_TYPE_FS = 0x5346;	//"FS" Factory Setting
        //const ushort FRAME_TYPE_DT = 0x5444;	//"DT" Start Detection

        //const ushort FRAME_TYPE_NK = 0x4E4B;	//"NK" Device busy

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
                    case Protocol.FRAME_TYPE_RP:
                        ShowRec(devRec.RecFrameDeal(frameRX));
                        break;
                    case Protocol.FRAME_TYPE_NK: MessageBox.Show("设备忙"); break;
                    case Protocol.FRAME_TYPE_GC:
                        if (null == dlgDevCfg)
                            dlgDevCfg = new DevCfgDlg(this);
                        if (dlgDevCfg.IsDisposed)
                            dlgDevCfg = new DevCfgDlg(this);

                        //dlgDevCfg.CfgFreameDeal(frameRX);
                        //dlgDevCfg.UpdateShow(devCfg);
                        if (openDevCfgDlgTimeOut > 0)
                        {
                            dlgDevCfg.Show();
                            openDevCfgDlgTimeOut = 0;
                        }
                        break;
                    case Protocol.FRAME_TYPE_SC: MessageBox.Show("更新设备信息成功！"); break;
                    case Protocol.FRAME_TYPE_LN:
                        //传感器实时采集数据帧
                        if (null == dlgDevCfg)
                            dlgDevCfg = new DevCfgDlg(this);
                        if (dlgDevCfg.IsDisposed)
                            dlgDevCfg = new DevCfgDlg(this);
                        dlgDevCfg.SensorFrameDeal(frameRX);
                        break;
                    //case FRAME_TYPE_LI: COMM_cmdLI(port); break;
                    //case FRAME_TYPE_LO: COMM_cmdLO(port); break;
                    //case FRAME_TYPE_SM: COMM_cmdSM(port, p); break;
                    //case FRAME_TYPE_GR: COMM_cmdGR(port); break;
                    //case FRAME_TYPE_CR: COMM_cmdCR(port); break;
                    case Protocol.FRAME_TYPE_GS: 
                        //To do... show mode
                        ; 
                        break;
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

        private void btCalSensor_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                byte[] frm = devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_CS);
                MessageBox.Show("请用纯白色纸板覆盖传感器表面后，点击“确定”。");
                serialPort1.Write(frm, 0, frm.Length);
            }
            else
                MessageBox.Show("请先打开串口。");
        }

        private void btClrModel_Click(object sender, EventArgs e)
        {
           if (serialPort1.IsOpen)
            {
                byte[] frm = devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_CR);
                serialPort1.Write(frm, 0, frm.Length);
            }
            else
                MessageBox.Show("请先打开串口。");
        }

        private void btSetModel_Click(object sender, EventArgs e)
        {
            if (serialPort1.IsOpen)
            {
                byte[] frm = devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_SM);
                serialPort1.Write(frm, 0, frm.Length);
            }
            else
                MessageBox.Show("请先打开串口。");

        }

        private void btAdjustment_Click(object sender, EventArgs e)
        {
            //Get cofig from device
            if (serialPort1.IsOpen)
            {
                byte[] mode = new byte[Marshal.SizeOf(typeof(UInt32))];
                mode[0] = (byte)CH_MODE.ADJUSTMENT;
                byte[] frm = devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_SM, mode, (ushort)Marshal.SizeOf(typeof(UInt32)));
                serialPort1.Write(frm, 0, frm.Length);
            }
            else
                MessageBox.Show("请先打开串口。");
        }

        private void btWork_Click(object sender, EventArgs e)
        {
            //Get cofig from device
            if (serialPort1.IsOpen)
            {
                byte[] mode = new byte[Marshal.SizeOf(typeof(UInt32))];
                mode[0] = (byte)CH_MODE.WORK;
                byte[] frm = devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_SM, mode, (ushort)Marshal.SizeOf(typeof(UInt32)));
                serialPort1.Write(frm, 0, frm.Length);
            }
            else
                MessageBox.Show("请先打开串口。");
        }
    }
}
