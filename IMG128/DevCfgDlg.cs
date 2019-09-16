using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
//线型定义11
//#define MAX_MODEL_NUM	2	//最多线型数量

//输入信号定义
//#define OUTPUT_NUM	2	//两数输出信号

//输入信号定义
//#define INPUT_NUM	4	//两路输入信号

//分析/报警类型
public enum ANAL_TYPE
{
    ANAL_NONE = 0x0000,	//无报警
    ANAL_CORE_POS = 0x0001,	//线端位置异常
    ANAL_CORE_WIDTH = 0x0002,	//线径异常
    ANAL_SEAL_POS = 0x0004,	//防水栓位置异常
    ANAL_SEAL_WIDTH = 0x0008,	//防水栓直径异常
    ANAL_STRIP_POS = 0x0010,	//剥切位置异常
    ANAL_STRIP_LEN = 0x0020,	//剥切长度异常
    ANAL_SPLAY = 0x0040,	//分叉异常
    ANAL_IS_SEAL = 0x0080,	//有无防水栓
    ANAL_SEAL_ORI = 0x0100, //防水栓方向
    ANAL_INVALID_IMG = 0x8000	//无效图像
} ;

//output mode define
public enum TYPE_OUTPUT
{
    OUTPUT_NO = 0x0000,	//bit 0 Normally open
    OUTPUT_NC = 0x0001,	//bit 0 Normally close
    OUTPUT_PASS = 0x0002,	//bit 1-2 active when pass
    OUTPUT_FAIL = 0x0004,	//bit 1-2 active when fail
    OUTPUT_ANY = 0x0006,	//bit 1-2 active pass or fail
    OUTPUT_IGN_FIRST = 0x0008, //bit 3 first learn no output
    OUTPUT_IGN_LEARN = 0x0018,	//bit 4 all learn no output
    OUTPUT_SEN_EN = 0x0020	//bit 5 输出其间传感器是否使能
};


//input mode define
public enum TYPE_INPUT
{
    INPUT_DISABLE = 0x0000,
    INPUT_TRIG = 0x0001,	//bit 0, trigger mode	1:LEVEL  0:EDGE
    INPUT_LEARN = 0x0002, //bit 1-2 function
    INPUT_FIRING = 0x0004, //bit 1-2 function
    INPUT_SETUP = 0x0006, //bit 1-2 function
    INPUT_POS = 0x0008,	//bit 3 PosTrigger
    INPUT_NEG = 0x0010, //bit 4 NegTrigger
};

//公差设置
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct TOL_ITEM_T
{
    public ANAL_TYPE analEN;				//分析哪些项
    public Single sealPosP, sealPosN;
    public Single sealWidthP, sealWidthN;
    public Single corePosP, corePosN;
    public Single coreWidthP, coreWidthN;
    public Single stripPosP, stripPosN;
    public Single stripLenP, stripLenN;
    public Single wireSplay;
    public Single variationFilter;
    public Single sealRatio;
    public Single sealLimit;
    public Single stripLimit;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    Int32[] reserve;
};	//一组公差


public struct WIRE_SETTING_T
{
    public TOL_ITEM_T tol;				//
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public Int32[] reserve;		//保留 备扩展
};


//配置信息结构体，描述了一份完整的设备配置信息
//[StructLayout(LayoutKind.Sequential, Pack = 4)]
//public struct CFG_T
//{
//    public Int32 modelNum;

//    //检测线型
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //MAX_MODEL_NUM = 2
//    public WIRE_SETTING_T[] wire;

//    //输出
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //OUTPUT_NUM = 2
//    public TYPE_OUTPUT[] outputMode;	//输出模式
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //OUTPUT_NUM = 2
//    public Int32[] outputWireIdx;		//输出对应的线型序号
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //OUTPUT_NUM = 2
//    public Int32[] outputDelay;		//输出延时
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //OUTPUT_NUM = 2
//    public Int32[] outputWidth;		//输出脉宽

//    //输入
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]    //INPUT_NUM = 4
//    public TYPE_INPUT[] inputMode;
//    public Int32 firingTimeOut;		//触发信号超时设置
//    public Int32 minSetupDuration;	//进入setup模式前，输入信号有效的最小持续时间，用于在没有上位机时手动操作进入学习模式

//    public Int32 senThresh;			//CCD sensor threshold
//    public Int32 learnNum;			//学习模式个数 2～10次 初值：4  设置在学习时以几根良品的波形平均值作为基准波形。

//    public UInt32 sn;					//产品序列号
//    public UInt32 downCnt;			//20180816 设备寿命计数器。默认为0，无限寿命。大于零时，每压接一条递减。递减到0时显示“设备计数异常！”
//    public UInt32 isSpeedAdj;		//是否进行速度矫正。如果通过速度不稳定的应用环境，启用此功能
//    public UInt32 isIgnInWhenOut;	//输出有效期间忽略输入（忽略返程阶段）
//    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
//    Int32[] reserve;		//保留 备扩展
//    public UInt32 valid;				//If (valid != CFG_VALID), set the config data to default
//};	//需写入FLASH, sizeof(CFG_T)必需为偶数

namespace IMG128
{
    public partial class DevCfgDlg : Form
    {

        const UInt32 CFG_VALID = 0x55AA55AA;
        const Int32 MAX_MODEL_NUM = 2;         //#define MAX_MODEL_NUM	2	//最多线型数量
        const Int32 OUTPUT_NUM = 2;
        const Int32 INPUT_NUM = 4; 
        const Int32 RESERVED_NUM = 4;

        //bool isLoadDevCfg = false;              //是否从设备读取过配置信息
        //public static CFG_T _cfg = new CFG_T();
        IMG128 hParent;
        

        public DevCfgDlg(IMG128 parent)
        {
            InitializeComponent();
            hParent = parent;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void DevCfgDlg_Load(object sender, EventArgs e)
        {
            
        }



        //获取输出引脚属性
        public bool IS_OUTPUT_NO(TYPE_OUTPUT outputMode) { return TYPE_OUTPUT.OUTPUT_NO == (TYPE_OUTPUT)((UInt32)outputMode & 0x0001); }
        public bool IS_OUTPUT_NC(TYPE_OUTPUT outputMode) { return TYPE_OUTPUT.OUTPUT_NC == (TYPE_OUTPUT)((UInt32)outputMode & 0x0001); }
        public bool IS_OUTPUT_PASS(TYPE_OUTPUT outputMode) { return TYPE_OUTPUT.OUTPUT_PASS == (TYPE_OUTPUT)((UInt32)outputMode & (UInt32)TYPE_OUTPUT.OUTPUT_PASS); }
        public bool IS_OUTPUT_FAIL(TYPE_OUTPUT outputMode) { return TYPE_OUTPUT.OUTPUT_FAIL == (TYPE_OUTPUT)((UInt32)outputMode & (UInt32)TYPE_OUTPUT.OUTPUT_FAIL); }
        public bool IS_OUTPUT_ANY(TYPE_OUTPUT outputMode) { return TYPE_OUTPUT.OUTPUT_ANY == (TYPE_OUTPUT)((UInt32)outputMode & (UInt32)TYPE_OUTPUT.OUTPUT_ANY); }
        public bool IS_OUTPUT_ING_FIRST(TYPE_OUTPUT outputMode) { return TYPE_OUTPUT.OUTPUT_IGN_FIRST == (TYPE_OUTPUT)((UInt32)outputMode & (UInt32)TYPE_OUTPUT.OUTPUT_IGN_FIRST); }
        public bool IS_OUTPUT_ING_LEARN(TYPE_OUTPUT outputMode) { return TYPE_OUTPUT.OUTPUT_IGN_LEARN == (TYPE_OUTPUT)((UInt32)outputMode & (UInt32)TYPE_OUTPUT.OUTPUT_IGN_LEARN); }
        public bool IS_OUTPUT_SEN_EN(TYPE_OUTPUT outputMode) { return TYPE_OUTPUT.OUTPUT_SEN_EN == (TYPE_OUTPUT)((UInt32)outputMode & (UInt32)TYPE_OUTPUT.OUTPUT_SEN_EN); }

        //获取输入引脚属性
        public bool IS_INPUT_EDGE(TYPE_INPUT input) { return 0 == ((UInt32)input & (UInt32)TYPE_INPUT.INPUT_TRIG); }
        public bool IS_INPUT_LEVEL(TYPE_INPUT input) { return TYPE_INPUT.INPUT_TRIG == (TYPE_INPUT)((UInt32)input & (UInt32)TYPE_INPUT.INPUT_TRIG); }
        public bool IS_INPUT_LEARN(TYPE_INPUT input) { return TYPE_INPUT.INPUT_LEARN == (TYPE_INPUT)((UInt32)input & 0x0006); }
        public bool IS_INPUT_FIRING(TYPE_INPUT input) { return TYPE_INPUT.INPUT_FIRING == (TYPE_INPUT)((UInt32)input & 0x0006); }
        public bool IS_INPUT_SETUP(TYPE_INPUT input) { return TYPE_INPUT.INPUT_SETUP == (TYPE_INPUT)((UInt32)input & 0x0006); }
        public bool IS_INPPUT_POS(TYPE_INPUT input) { return TYPE_INPUT.INPUT_POS == (TYPE_INPUT)((UInt32)input & (UInt32)TYPE_INPUT.INPUT_POS); }
        public bool IS_INPPUT_NEG(TYPE_INPUT input) { return TYPE_INPUT.INPUT_NEG == (TYPE_INPUT)((UInt32)input & (UInt32)TYPE_INPUT.INPUT_NEG); }

        //获取检测模式
        public bool IS_ANAL_CORE_POS(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_CORE_POS == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_CORE_POS);}
        public bool IS_ANAL_CORE_WIDTH(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_CORE_WIDTH == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_CORE_WIDTH); }
        public bool IS_ANAL_SEAL_POS(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_SEAL_POS == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_SEAL_POS); }
        public bool IS_ANAL_SEAL_WIDTH(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_SEAL_WIDTH == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_SEAL_WIDTH); }
        public bool IS_ANAL_STRIP_POS(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_STRIP_POS == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_STRIP_POS); }
        public bool IS_ANAL_STRIP_LEN(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_STRIP_LEN == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_STRIP_LEN); }
        public bool IS_ANAL_SPLAY(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_SPLAY == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_SPLAY); }
        public bool IS_ANAL_IS_SEAL(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_IS_SEAL == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_IS_SEAL); }       //是否有防水栓
        public bool IS_ANAL_SEAL_ORI(ANAL_TYPE type) { return (UInt32)ANAL_TYPE.ANAL_SEAL_ORI == ((UInt32)type & (UInt32)ANAL_TYPE.ANAL_SEAL_ORI); }    //防水栓方向

        /*********************************************************************************************************
            检验参数有效性。如果参数无效，设为默认参数
        */
        int CFG_CheckVal(int val, int defVal, int min, int max)
        {
            int ret = val;
            if (val < min)
                ret = defVal;
            if (val > max)
                ret = defVal;
            return ret;
        }
        float CFG_CheckVal(float val, float defVal, float min, float max)
        {
            if(Single.IsNaN(val)) //if(Single.IsInfinity(val)) //if (!IsFinite(val))
                return defVal;

            float ret = val;
            if (val < min)
                ret = defVal;
            if (val > max)
                ret = defVal;
            return ret;
        }

        //检查配置有效性
        //bool CfgCheck(ref CFG_T cfg)
        //{
        //    int i;

        //    if (CFG_VALID != cfg.valid)
        //        return false;

        //    cfg.modelNum = CFG_CheckVal(cfg.modelNum, 1, 1, MAX_MODEL_NUM);

        //    //检测线型
        //    //WIRE_SETTING_T* pW = _cfg.wire;

        //    for (i = 0; i < MAX_MODEL_NUM; i++)
        //    {
        //        cfg.wire[i].tol.sealPosP = CFG_CheckVal(cfg.wire[i].tol.sealPosP, (float)2.0, (float)0.0, (float)10.0);				//mm
        //        cfg.wire[i].tol.sealPosN = CFG_CheckVal(cfg.wire[i].tol.sealPosN, (float)2.0, (float)0.0, (float)10.0);				//mm
        //        cfg.wire[i].tol.corePosP = CFG_CheckVal(cfg.wire[i].tol.corePosP, (float)2.0, (float)0.0, (float)10.0);				//mm
        //        cfg.wire[i].tol.corePosN = CFG_CheckVal(cfg.wire[i].tol.corePosN, (float)2.0, (float)0.0, (float)10.0);				//mm
        //        cfg.wire[i].tol.stripPosP = CFG_CheckVal(cfg.wire[i].tol.stripPosP, (float)2.0, (float)0.0, (float)10.0);			//mm
        //        cfg.wire[i].tol.stripPosN = CFG_CheckVal(cfg.wire[i].tol.stripPosN, (float)2.0, (float)0.0, (float)10.0);			//mm
        //        cfg.wire[i].tol.stripLenP = CFG_CheckVal(cfg.wire[i].tol.stripLenP, (float)2.0, (float)0.0, (float)10.0);			//mm
        //        cfg.wire[i].tol.stripLenN = CFG_CheckVal(cfg.wire[i].tol.stripLenN, (float)2.0, (float)0.0, (float)10.0);			//mm

        //        cfg.wire[i].tol.sealWidthP = CFG_CheckVal(cfg.wire[i].tol.sealWidthP, (float)10.0, (float)5.0, (float)100.0);		//%
        //        cfg.wire[i].tol.sealWidthN = CFG_CheckVal(cfg.wire[i].tol.sealWidthN, (float)10.0, (float)5.0, (float)100.0);		//%
        //        cfg.wire[i].tol.coreWidthP = CFG_CheckVal(cfg.wire[i].tol.coreWidthP, (float)10.0, (float)5.0, (float)100.0);		//%
        //        cfg.wire[i].tol.coreWidthN = CFG_CheckVal(cfg.wire[i].tol.coreWidthN, (float)10.0, (float)5.0, (float)100.0);		//%

        //        cfg.wire[i].tol.wireSplay = CFG_CheckVal(cfg.wire[i].tol.wireSplay, (float)50.0, (float)5.0, (float)1000.0);		//%

        //        cfg.wire[i].tol.variationFilter = CFG_CheckVal(cfg.wire[i].tol.variationFilter, (float)1.25, (float)0.0, (float)10.0);//mm
        //        cfg.wire[i].tol.sealRatio = CFG_CheckVal(cfg.wire[i].tol.sealRatio, (float)85.0, (float)2.0, (float)500.0);			//%
        //        cfg.wire[i].tol.sealLimit = CFG_CheckVal(cfg.wire[i].tol.sealLimit, (float)5.0, (float)2.0, (float)800.0);			//%
        //        cfg.wire[i].tol.stripLimit = CFG_CheckVal(cfg.wire[i].tol.stripLimit, (float)15.0, (float)5.0, (float)800.0);		//%
        //    }

        //    //输出
        //    //TYPE_OUTPUT outputMode[OUTPUT_NUM];	//输出模式
        //    for (i = 0; i < OUTPUT_NUM; i++)
        //    {
        //        cfg.outputWireIdx[i] = CFG_CheckVal(cfg.outputWireIdx[i], 1, 1, MAX_MODEL_NUM);		//输出对应的线型序号
        //        cfg.outputDelay[i] = CFG_CheckVal(cfg.outputDelay[i], 0, 0, 10000);					//输出延时ms
        //        cfg.outputWidth[i] = CFG_CheckVal(cfg.outputWidth[i], 50, 0, 60000);					//输出脉宽
        //    }

        //    //输入
        //    //TYPE_INPUT inputMode[INPUT_NUM];
        //    cfg.firingTimeOut = CFG_CheckVal(cfg.firingTimeOut, 500, 10, 60000);						//触发信号超时设置
        //    cfg.minSetupDuration = CFG_CheckVal(cfg.minSetupDuration, 4000, 4000, 60000);				//进入setup模式前，输入信号有效的最小持续时间，用于在没有上位机时手动操作进入学习模式

        //    cfg.senThresh = CFG_CheckVal(cfg.senThresh, 500, 5, 4000);	//CCD sensor threshold
        //    cfg.learnNum = CFG_CheckVal(cfg.learnNum, 1, 1, 16);		//学习模式个数 1～16次 初值：1  设置在学习时以几根良品的波形平均值作为基准波形。
        //    return true;
        //}

        //public void UpdateShow(CFG_T cfg)
        //{ 
        //}

        ////解析串口数据帧，更新显示
        //public void CfgFreameDeal(COMM_FRAME_T frame)
        //{
        //    CFG_T cfg = new CFG_T();

        //    cfg = (CFG_T)BytesToStruct(frame.data, Marshal.SizeOf(typeof(CFG_T)), typeof(CFG_T));

        //    if (CfgCheck(ref cfg))
        //    {
        //       // hParent.devCfg = cfg;
        //        UpdateShow(cfg);
        //    }
        //}
        public void SensorFrameDeal(COMM_FRAME_T frame)
        {
            UInt16[] line = new UInt16[REC.PIX_NUM];

            int i;

            //这里怎样通过内存拷贝实现以下功能？？
            for (i = 0; i < REC.PIX_NUM; i++)
            {
                line[i] = (UInt16)frame.data[2 * i];
                line[i] += (UInt16)((UInt16)frame.data[2 * i + 1] * (UInt16)256);
                //}

                //Graphics grfx = pBoxSen.CreateGraphics();
                //int picWidth = pBoxSen.Size.Width;
                //int picHeight = pBoxSen.Size.Height;

                //const int SENSOR_RANGE = 1024;  //传感器输出最大值

                //int X_SCALE = picWidth / REC.PIX_NUM;// 3;
                //int Y_SCALE = SENSOR_RANGE / picHeight;// 4;

                //Pen pen = new Pen(Color.Blue, 1);

                //Point[] pts = new Point[REC.PIX_NUM+1];
                //pts[0] = new Point(0, 0);
                //for (i = 0; i < REC.PIX_NUM; i++)
                //{
                //    pts[i+1].X = (i+1) * X_SCALE;
                //    pts[i+1].Y = picHeight - line[i] / Y_SCALE;
                //}
                //grfx.Clear(Color.White);
                //grfx.DrawLines(pen, pts);

                //grfx.Dispose();

                //byte[] line = new byte[sizeof(UInt16 )* REC.PIX_NUM];
                ////line = (UInt16[])BytesToStruct(frame.data, sizeof(UInt16) * REC.PIX_NUM, typeof(UInt16 []));
                //Copy(frame.data, 0, line, sizeof(UInt16) * REC.PIX_NUM;
            }
        }

            //BytesToStruct
            public object BytesToStruct(byte[] buf, int len, Type type)
        {
            object rtn;
            IntPtr buffer = Marshal.AllocHGlobal(len);
            Marshal.Copy(buf, 0, buffer, len);
            rtn = Marshal.PtrToStructure(buffer, type);
            Marshal.FreeHGlobal(buffer);
            return rtn;
        }

        private void cBoxModelNum_SelectedIndexChanged(object sender, EventArgs e)
        { 
        }

        //private void setDefault(ref CFG_T cfg)
        //{
        //    int i;

        //    cfg.modelNum = 1;

        //    //检测线型
        //    WIRE_SETTING_T []wire = cfg.wire;

        //    for (i = 0; i < MAX_MODEL_NUM; i++)
        //    {
        //        wire[i].tol.analEN = (ANAL_TYPE)0xFFFF;
        //        wire[i].tol.sealPosP = 2.0f;		//mm
        //        wire[i].tol.sealPosN = 2.0f;		//mm
        //        wire[i].tol.corePosP = 2.0f;		//mm
        //        wire[i].tol.corePosN = 2.0f;		//mm
        //        wire[i].tol.stripPosP = 2.0f;		//mm
        //        wire[i].tol.stripPosN = 2.0f;		//mm
        //        wire[i].tol.stripLenP = 2.0f;		//mm
        //        wire[i].tol.stripLenN = 2.0f;		//mm

        //        wire[i].tol.sealWidthP = 10.0f;		//%
        //        wire[i].tol.sealWidthN = 10.0f;		//%
        //        wire[i].tol.coreWidthP = 10.0f;		//%
        //        wire[i].tol.coreWidthN = 10.0f;		//%

        //        wire[i].tol.wireSplay = 50.0f;		//%

        //        wire[i].tol.variationFilter = 1.25f;//mm
        //        wire[i].tol.sealRatio = 85.0f;		//%
        //        wire[i].tol.sealLimit = 100.0f;		//%
        //        wire[i].tol.stripLimit = 50.0f;		//%
        //    }

        //    //输出
        //    for (i = 0; i < OUTPUT_NUM; i++)
        //    {
        //        cfg.outputMode[i] = (TYPE_OUTPUT)((UInt32)TYPE_OUTPUT.OUTPUT_NO | (UInt32)TYPE_OUTPUT.OUTPUT_PASS);
        //        cfg.outputWireIdx[i] = 1;		//输出对应的线型序号
        //        cfg.outputDelay[i] = 0;			//输出延时ms
        //        cfg.outputWidth[i] = 50;		//输出脉宽
        //    }

        //    //输入
        //    cfg.inputMode[0] = (TYPE_INPUT)((UInt32)TYPE_INPUT.INPUT_SETUP | (UInt32)TYPE_INPUT.INPUT_TRIG | (UInt32)TYPE_INPUT.INPUT_POS);			//trigger:level function:Setup, polarity:Positive
        //    cfg.inputMode[1] = (TYPE_INPUT)((UInt32)TYPE_INPUT.INPUT_LEARN | (UInt32)TYPE_INPUT.INPUT_POS | (UInt32)TYPE_INPUT.INPUT_NEG);			//trigger:Pos Neg edge function:Learn
        //    cfg.inputMode[2] = (TYPE_INPUT)((UInt32)TYPE_INPUT.INPUT_FIRING | (UInt32)TYPE_INPUT.INPUT_SETUP | (UInt32)TYPE_INPUT.INPUT_POS);		//trigger:level function:Setup, polarity:Positive
        //    cfg.inputMode[3] = TYPE_INPUT.INPUT_DISABLE;												//trigger:Pos Neg edge function:Learn

        //    cfg.firingTimeOut = 500;					//触发信号超时设置
        //    cfg.minSetupDuration = 4000;				//进入setup模式前，输入信号有效的最小持续时间，用于在没有上位机时手动操作进入学习模式

        //    cfg.senThresh = 10;						//CCD sensor threshold %
        //    cfg.learnNum = 1;							//学习模式个数 1～16次 初值：1  设置在学习时以几根良品的波形平均值作为基准波形。

        //    cfg.sn = 0;			//产品序列号
        //    cfg.downCnt = 0;			//20180816 设备寿命计数器。默认为0，无限寿命。大于零时，每压接一条递减。递减到0时显示“设备计数异常！”
        //    cfg.valid = CFG_VALID;	//设置配置有效标识
        //}

        //提取字符串中前部数字部分
        private string GetNumber(string str)
        {
            int len = str.Length;
            int i;
            for (i = 0; i < len; i++)
                if (((str[i] < '0') || (str[i] > '9')) && (str[i] != '.'))
                    break;
            if (i > 0)
                str = str.Substring(0, i);
            else
                str = "0";
            return str;
        }
        /// <summary>
        /// 申请结构体空间并设置默认值
        /// </summary>
        /// <returns>初始化后的结构体</returns>
        public WC_DEV_CFG alloc_CFG_T()
        {
            WC_DEV_CFG cfg = new WC_DEV_CFG();

            cfg.valid = 0;
            cfg.ModelWireSeperate = false;
            cfg.FailWhenReverse = false;
            cfg.shadeThreah = 5;
            cfg.ClrErrThresh = 100;
            cfg.clrAvrPercent = 5;
            cfg.clrAvrMin =4;
            cfg.widthRatio = 0.6F;
            cfg.seperationRatio = 0.3F; 
            cfg.reserved= new uint[RESERVED_NUM];
            return cfg;
        } 

        private void btUpdateCfg_Click(object sender, EventArgs e)
        {

            WC_DEV_CFG cfg = alloc_CFG_T();
            cfg.ModelWireSeperate = ModelWireSeperate.Checked;
            cfg.FailWhenReverse = FailWhenReverse.Checked;
            cfg.shadeThreah =Int32.Parse(shadeThreah.Text);
            cfg.ClrErrThresh = Int32.Parse(ClrErrThresh.Text);
            cfg.clrAvrMin = Int32.Parse(clrAvrMin.Text);
            cfg.clrAvrPercent = Int32.Parse(clrAvrPercent.Text);
            cfg.widthRatio = float.Parse(widthRatio.Text);
            cfg.seperationRatio = float.Parse(seperationRatio.Text);

            if (CfgCheck(ref cfg))
            { 
                if (hParent.serialPort1.IsOpen) 
                {
                    byte[] cfgBuf = StructToBytes(cfg);    //将CFG_T转换为byte[]
                    byte[] frm = hParent.devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_SC, cfgBuf, (ushort)cfgBuf.Length);  //生成串口数据帧字符数组
                    hParent.serialPort1.Write(frm, 0, frm.Length);      //发送串口数据
                }
            }

        }
        /// <summary>
        /// 检查配置是否有效
        /// </summary>
        /// <param name="cfg">配置信息结构体</param>
        /// <returns>是否通过</returns>
        public bool CfgCheck(ref WC_DEV_CFG cfg)
        {
            bool valid = false;

            return true;
        }

        //StructToBytes
        public byte[] StructToBytes(object obj)
        {
            int rawsize = Marshal.SizeOf(obj);
            IntPtr buffer = Marshal.AllocHGlobal(rawsize);
            Marshal.StructureToPtr(obj, buffer, false);
            byte[] rawdatas = new byte[rawsize];
            Marshal.Copy(buffer, rawdatas, 0, rawsize);
            Marshal.FreeHGlobal(buffer);
            return rawdatas;
        }

        private void DevCfgDlg_FormClosing(object sender, FormClosingEventArgs e)
        {
            
        }

        private void btnSetupCCD_Click(object sender, EventArgs e)
        {
        }

        private void btUploadCfg_Click(object sender, EventArgs e)
        {

        } 
    }
}
