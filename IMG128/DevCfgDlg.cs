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
[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct CFG_T
{
    public Int32 modelNum;

    //检测线型
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //MAX_MODEL_NUM = 2
    public WIRE_SETTING_T[] wire;

    //输出
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //OUTPUT_NUM = 2
    public TYPE_OUTPUT[] outputMode;	//输出模式
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //OUTPUT_NUM = 2
    public Int32[] outputWireIdx;		//输出对应的线型序号
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //OUTPUT_NUM = 2
    public Int32[] outputDelay;		//输出延时
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]    //OUTPUT_NUM = 2
    public Int32[] outputWidth;		//输出脉宽

    //输入
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]    //INPUT_NUM = 4
    public TYPE_INPUT[] inputMode;
    public Int32 firingTimeOut;		//触发信号超时设置
    public Int32 minSetupDuration;	//进入setup模式前，输入信号有效的最小持续时间，用于在没有上位机时手动操作进入学习模式

    public Int32 senThresh;			//CCD sensor threshold
    public Int32 learnNum;			//学习模式个数 2～10次 初值：4  设置在学习时以几根良品的波形平均值作为基准波形。

    public UInt32 sn;					//产品序列号
    public UInt32 downCnt;			//20180816 设备寿命计数器。默认为0，无限寿命。大于零时，每压接一条递减。递减到0时显示“设备计数异常！”
    public UInt32 isSpeedAdj;		//是否进行速度矫正。如果通过速度不稳定的应用环境，启用此功能
    public UInt32 isIgnInWhenOut;	//输出有效期间忽略输入（忽略返程阶段）
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
    Int32[] reserve;		//保留 备扩展
    public UInt32 valid;				//If (valid != CFG_VALID), set the config data to default
};	//需写入FLASH, sizeof(CFG_T)必需为偶数

namespace IMG128
{
    public partial class DevCfgDlg : Form
    {

        const UInt32 CFG_VALID = 0x55AA55AA;
        const Int32 MAX_MODEL_NUM = 2;         //#define MAX_MODEL_NUM	2	//最多线型数量
        const Int32 OUTPUT_NUM = 2;
        const Int32 INPUT_NUM = 4;

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
        bool CfgCheck(ref CFG_T cfg)
        {
            int i;

            if (CFG_VALID != cfg.valid)
                return false;

            cfg.modelNum = CFG_CheckVal(cfg.modelNum, 1, 1, MAX_MODEL_NUM);

            //检测线型
            //WIRE_SETTING_T* pW = _cfg.wire;

            for (i = 0; i < MAX_MODEL_NUM; i++)
            {
                cfg.wire[i].tol.sealPosP = CFG_CheckVal(cfg.wire[i].tol.sealPosP, (float)2.0, (float)0.0, (float)10.0);				//mm
                cfg.wire[i].tol.sealPosN = CFG_CheckVal(cfg.wire[i].tol.sealPosN, (float)2.0, (float)0.0, (float)10.0);				//mm
                cfg.wire[i].tol.corePosP = CFG_CheckVal(cfg.wire[i].tol.corePosP, (float)2.0, (float)0.0, (float)10.0);				//mm
                cfg.wire[i].tol.corePosN = CFG_CheckVal(cfg.wire[i].tol.corePosN, (float)2.0, (float)0.0, (float)10.0);				//mm
                cfg.wire[i].tol.stripPosP = CFG_CheckVal(cfg.wire[i].tol.stripPosP, (float)2.0, (float)0.0, (float)10.0);			//mm
                cfg.wire[i].tol.stripPosN = CFG_CheckVal(cfg.wire[i].tol.stripPosN, (float)2.0, (float)0.0, (float)10.0);			//mm
                cfg.wire[i].tol.stripLenP = CFG_CheckVal(cfg.wire[i].tol.stripLenP, (float)2.0, (float)0.0, (float)10.0);			//mm
                cfg.wire[i].tol.stripLenN = CFG_CheckVal(cfg.wire[i].tol.stripLenN, (float)2.0, (float)0.0, (float)10.0);			//mm

                cfg.wire[i].tol.sealWidthP = CFG_CheckVal(cfg.wire[i].tol.sealWidthP, (float)10.0, (float)5.0, (float)100.0);		//%
                cfg.wire[i].tol.sealWidthN = CFG_CheckVal(cfg.wire[i].tol.sealWidthN, (float)10.0, (float)5.0, (float)100.0);		//%
                cfg.wire[i].tol.coreWidthP = CFG_CheckVal(cfg.wire[i].tol.coreWidthP, (float)10.0, (float)5.0, (float)100.0);		//%
                cfg.wire[i].tol.coreWidthN = CFG_CheckVal(cfg.wire[i].tol.coreWidthN, (float)10.0, (float)5.0, (float)100.0);		//%

                cfg.wire[i].tol.wireSplay = CFG_CheckVal(cfg.wire[i].tol.wireSplay, (float)50.0, (float)5.0, (float)1000.0);		//%

                cfg.wire[i].tol.variationFilter = CFG_CheckVal(cfg.wire[i].tol.variationFilter, (float)1.25, (float)0.0, (float)10.0);//mm
                cfg.wire[i].tol.sealRatio = CFG_CheckVal(cfg.wire[i].tol.sealRatio, (float)85.0, (float)2.0, (float)500.0);			//%
                cfg.wire[i].tol.sealLimit = CFG_CheckVal(cfg.wire[i].tol.sealLimit, (float)5.0, (float)2.0, (float)800.0);			//%
                cfg.wire[i].tol.stripLimit = CFG_CheckVal(cfg.wire[i].tol.stripLimit, (float)15.0, (float)5.0, (float)800.0);		//%
            }

            //输出
            //TYPE_OUTPUT outputMode[OUTPUT_NUM];	//输出模式
            for (i = 0; i < OUTPUT_NUM; i++)
            {
                cfg.outputWireIdx[i] = CFG_CheckVal(cfg.outputWireIdx[i], 1, 1, MAX_MODEL_NUM);		//输出对应的线型序号
                cfg.outputDelay[i] = CFG_CheckVal(cfg.outputDelay[i], 0, 0, 10000);					//输出延时ms
                cfg.outputWidth[i] = CFG_CheckVal(cfg.outputWidth[i], 50, 0, 60000);					//输出脉宽
            }

            //输入
            //TYPE_INPUT inputMode[INPUT_NUM];
            cfg.firingTimeOut = CFG_CheckVal(cfg.firingTimeOut, 500, 10, 60000);						//触发信号超时设置
            cfg.minSetupDuration = CFG_CheckVal(cfg.minSetupDuration, 4000, 4000, 60000);				//进入setup模式前，输入信号有效的最小持续时间，用于在没有上位机时手动操作进入学习模式

            cfg.senThresh = CFG_CheckVal(cfg.senThresh, 500, 5, 4000);	//CCD sensor threshold
            cfg.learnNum = CFG_CheckVal(cfg.learnNum, 1, 1, 16);		//学习模式个数 1～16次 初值：1  设置在学习时以几根良品的波形平均值作为基准波形。
            return true;
        }

        public void UpdateShow(CFG_T cfg)
        {
            //modelNum
            cBoxModelNum.Text = cfg.modelNum.ToString();

            //如果只有一个基准线型，则基准线型2相关设置灰化
            if (cBoxModelNum.Text == "1")
            {
                gBoxModel2.Enabled = false;
                gBoxAdvanceModel2.Enabled = false;
            }
            else
            {
                gBoxModel2.Enabled = true;
                gBoxAdvanceModel2.Enabled = false;
            }
            
            ckBoxAutoAdjSpeed.Checked = (cfg.isSpeedAdj != 0);      //速度补偿
            cBoxLearnNum.Text = cfg.learnNum.ToString();            //学习数量
            tBoxSensorSensitivity.Text = cfg.senThresh.ToString();  //传感器灵敏度%


            //检测模式
            ANAL_TYPE type = cfg.wire[0].tol.analEN;
            ckBoxCorePos1.Checked = IS_ANAL_CORE_POS(type);
            ckBoxCoreWidth1.Checked = IS_ANAL_CORE_WIDTH(type);
            ckBoxSealPos1.Checked = IS_ANAL_SEAL_POS(type);
            ckBoxSealWidth1.Checked = IS_ANAL_SEAL_WIDTH(type);
            ckBoxStripPos1.Checked = IS_ANAL_STRIP_POS(type);
            ckBoxStripLen1.Checked = IS_ANAL_STRIP_LEN(type);
            ckBoxSplay1.Checked = IS_ANAL_SPLAY(type);
            ckBoxSeal1.Checked = IS_ANAL_IS_SEAL(type);
            ckBoxSealOri1.Checked = IS_ANAL_SEAL_ORI(type);

            type = cfg.wire[1].tol.analEN;
            ckBoxCorePos2.Checked = IS_ANAL_CORE_POS(type);
            ckBoxCoreWidth2.Checked = IS_ANAL_CORE_WIDTH(type);
            ckBoxSealPos2.Checked = IS_ANAL_SEAL_POS(type);
            ckBoxSealWidth2.Checked = IS_ANAL_SEAL_WIDTH(type);
            ckBoxStripPos2.Checked = IS_ANAL_STRIP_POS(type);
            ckBoxStripLen2.Checked = IS_ANAL_STRIP_LEN(type);
            ckBoxSplay2.Checked = IS_ANAL_SPLAY(type);
            ckBoxSeal2.Checked = IS_ANAL_IS_SEAL(type);
            ckBoxSealOri2.Checked = IS_ANAL_SEAL_ORI(type);

            //检测参数设置
            tBoxCorePosN1.Text = cfg.wire[0].tol.corePosN.ToString() + "mm";
            tBoxCorePosN2.Text = cfg.wire[1].tol.corePosN.ToString() + "mm";
            tBoxCorePosP1.Text = cfg.wire[0].tol.corePosP.ToString() + "mm";
            tBoxCorePosP2.Text = cfg.wire[1].tol.corePosP.ToString() + "mm";
            
            tBoxSealPosN1.Text = cfg.wire[0].tol.sealPosN.ToString() + "mm";
            tBoxSealPosN2.Text = cfg.wire[1].tol.sealPosN.ToString() + "mm";
            tBoxSealPosP1.Text = cfg.wire[0].tol.sealPosP.ToString() + "mm";
            tBoxSealPosP2.Text = cfg.wire[1].tol.sealPosP.ToString() + "mm";
            
            tBoxStripPosN1.Text = cfg.wire[0].tol.stripPosN.ToString() + "mm";
            tBoxStripPosN2.Text = cfg.wire[1].tol.stripPosN.ToString() + "mm";
            tBoxStripPosP1.Text = cfg.wire[0].tol.stripPosP.ToString() + "mm";
            tBoxStripPosP2.Text = cfg.wire[1].tol.stripPosP.ToString() + "mm";
            
            tBoxStripLenN1.Text = cfg.wire[0].tol.stripLenN.ToString() + "mm";
            tBoxStripLenN2.Text = cfg.wire[1].tol.stripLenN.ToString() + "mm";
            tBoxStripLenP1.Text = cfg.wire[0].tol.stripLenP.ToString() + "mm";
            tBoxStripLenP2.Text = cfg.wire[1].tol.stripLenP.ToString() + "mm";

            tBoxCoreWidthN1.Text = cfg.wire[0].tol.coreWidthN.ToString() + "%";
            tBoxCoreWidthN2.Text = cfg.wire[1].tol.coreWidthN.ToString() + "%";
            tBoxCoreWidthP1.Text = cfg.wire[0].tol.coreWidthP.ToString() + "%";
            tBoxCoreWidthP2.Text = cfg.wire[1].tol.coreWidthP.ToString() + "%";

            tBoxSealWidthN1.Text = cfg.wire[0].tol.sealWidthN.ToString() + "%";
            tBoxSealWidthN2.Text = cfg.wire[1].tol.sealWidthN.ToString() + "%";
            tBoxSealWidthP1.Text = cfg.wire[0].tol.sealWidthP.ToString() + "%";
            tBoxSealWidthP2.Text = cfg.wire[1].tol.sealWidthP.ToString() + "%";

            tBoxVariationFilter1.Text = cfg.wire[0].tol.variationFilter.ToString() + "mm";
            tBoxVariationFilter2.Text = cfg.wire[1].tol.variationFilter.ToString() + "mm";
            tBoxSealLimit1.Text = cfg.wire[0].tol.sealLimit.ToString();
            tBoxSealLimit2.Text = cfg.wire[1].tol.sealLimit.ToString();
            tBoxStripLimit1.Text = cfg.wire[0].tol.stripLimit.ToString();
            tBoxStripLimit2.Text = cfg.wire[1].tol.stripLimit.ToString();
            tBoxSealRatio1.Text = cfg.wire[0].tol.sealRatio.ToString();
            tBoxSealRatio2.Text = cfg.wire[1].tol.sealRatio.ToString();

            //输入 怎样使用控件数组防止代码拷贝？
            TYPE_INPUT input = cfg.inputMode[0];
            if (IS_INPUT_LEARN(input))
                cBoxInputModeLvl1.Text = "进入学习模式";
            else if (IS_INPUT_FIRING(input))
                cBoxInputModeLvl1.Text = "触发采样";
            else if (IS_INPUT_SETUP(input))
                cBoxInputModeLvl1.Text = "进入设置模式";
            if (IS_INPPUT_POS(input))
                cBoxInputPolLvl1.Text = "上升沿触发";
            else
                cBoxInputPolLvl1.Text = "下降沿触发";
            

            input = cfg.inputMode[1];
            if (IS_INPUT_LEARN(input))
                cBoxInputModeLvl2.Text = "进入学习模式";
            else if (IS_INPUT_FIRING(input))
                cBoxInputModeLvl2.Text = "触发采样";
            else if (IS_INPUT_SETUP(input))
                cBoxInputModeLvl2.Text = "进入设置模式";
            if (IS_INPPUT_POS(input))
                cBoxInputPolLvl2.Text = "上升沿触发";
            else
                cBoxInputPolLvl2.Text = "下降沿触发";
            
            input = cfg.inputMode[2];
            if (IS_INPUT_LEARN(input))
                cBoxInputModeLvl3.Text = "进入学习模式";
            else if (IS_INPUT_FIRING(input))
                cBoxInputModeLvl3.Text = "触发采样";
            else if (IS_INPUT_SETUP(input))
                cBoxInputModeLvl3.Text = "进入设置模式";
            if (IS_INPPUT_POS(input))
                cBoxInputPolLvl3.Text = "上升沿触发";
            else
                cBoxInputPolLvl3.Text = "下降沿触发";
            
            input = cfg.inputMode[3];
            if (IS_INPUT_LEARN(input))
                cBoxInputModeLvl4.Text = "进入学习模式";
            else if (IS_INPUT_FIRING(input))
                cBoxInputModeLvl4.Text = "触发采样";
            else if (IS_INPUT_SETUP(input))
                cBoxInputModeLvl4.Text = "进入设置模式";
            if (IS_INPPUT_POS(input))
                cBoxInputPolLvl4.Text = "上升沿触发";
            else
                cBoxInputPolLvl4.Text = "下降沿触发";

            //输出
            TYPE_OUTPUT output = cfg.outputMode[0];
            if (IS_OUTPUT_NO(output))
                rBtnNO1.Checked = true;
            else
                rBtnNC1.Checked = true;
            
            if (IS_OUTPUT_ANY(output))
                cBoxOutMode1.Text = "合格/不良品都输出脉冲";
            else if (IS_OUTPUT_PASS(output))
                cBoxOutMode1.Text = "合格品输出脉冲";
            else if (IS_OUTPUT_FAIL(output))
                cBoxOutMode1.Text = "不良品输出脉冲";
            
            if (IS_OUTPUT_ING_FIRST(output))
                rBtnIgnoreFirst1.Checked = true;
            else if (IS_OUTPUT_ING_LEARN(output))
                rBtnIgnoreLearn1.Checked = true;
            else
                rBtnNormal1.Checked = true;
            tBoxOutDelay1.Text = cfg.outputDelay[0].ToString();
            tBoxDuration1.Text = cfg.outputWidth[0].ToString();

            output = cfg.outputMode[1];
            if (IS_OUTPUT_NO(output))
                rBtnNO2.Checked = true;
            else
                rBtnNC2.Checked = true;

            if (IS_OUTPUT_PASS(output))
                cBoxOutMode2.Text = "合格品输出脉冲";
            else if (IS_OUTPUT_FAIL(output))
                cBoxOutMode2.Text = "不良品输出脉冲";
            else if (IS_OUTPUT_ANY(output))
                cBoxOutMode2.Text = "合格/不良品都输出脉冲";

            if (IS_OUTPUT_ING_FIRST(output))
                rBtnIgnoreFirst2.Checked = true;
            else if (IS_OUTPUT_ING_LEARN(output))
                rBtnIgnoreLearn2.Checked = true;
            else
                rBtnNormal2.Checked = true;
            tBoxOutDelay2.Text = cfg.outputDelay[1].ToString();
            tBoxDuration2.Text = cfg.outputWidth[1].ToString();

            //输出有效期间忽略输入（忽略返程阶段）
            checkBoxOutIgnoreWhileOutActive.Checked = (cfg.isIgnInWhenOut != 0);
        }

        //解析串口数据帧，更新显示
        public void CfgFreameDeal(COMM_FRAME_T frame)
        {
            CFG_T cfg = new CFG_T();

            cfg = (CFG_T)BytesToStruct(frame.data, Marshal.SizeOf(typeof(CFG_T)), typeof(CFG_T));

            if (CfgCheck(ref cfg))
            {
                hParent.devCfg = cfg;
                UpdateShow(cfg);
            }
        }
        public void SensorFrameDeal(COMM_FRAME_T frame)
        {
            UInt16[] line = new UInt16[REC.PIX_NUM];
            
            int i;

            //这里怎样通过内存拷贝实现以下功能？？
            for(i = 0; i < REC.PIX_NUM; i++)
            {
                line[i] = (UInt16)frame.data[2*i];
                line[i] += (UInt16)((UInt16)frame.data[2*i+1] * (UInt16)256);
            }

            Graphics grfx = pBoxSen.CreateGraphics();
            int picWidth = pBoxSen.Size.Width;
            int picHeight = pBoxSen.Size.Height;

            const int SENSOR_RANGE = 1024;  //传感器输出最大值

            int X_SCALE = picWidth / REC.PIX_NUM;// 3;
            int Y_SCALE = SENSOR_RANGE / picHeight;// 4;
            
            Pen pen = new Pen(Color.Blue, 1);

            Point[] pts = new Point[REC.PIX_NUM+1];
            pts[0] = new Point(0, 0);
            for (i = 0; i < REC.PIX_NUM; i++)
            {
                pts[i+1].X = (i+1) * X_SCALE;
                pts[i+1].Y = picHeight - line[i] / Y_SCALE;
            }
            grfx.Clear(Color.White);
            grfx.DrawLines(pen, pts);
            
            grfx.Dispose();


            //byte[] line = new byte[sizeof(UInt16 )* REC.PIX_NUM];
            ////line = (UInt16[])BytesToStruct(frame.data, sizeof(UInt16) * REC.PIX_NUM, typeof(UInt16 []));
            //Copy(frame.data, 0, line, sizeof(UInt16) * REC.PIX_NUM;
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
            //如果只有一个基准线型，则基准线型2相关设置灰化
            if (cBoxModelNum.Text == "1")
            {
                gBoxModel2.Enabled = false;
                gBoxAdvanceModel2.Enabled = false;
            }
            else
            {
                gBoxModel2.Enabled = true;
                gBoxAdvanceModel2.Enabled = false;
            }
        }

        private void setDefault(ref CFG_T cfg)
        {
            int i;

            cfg.modelNum = 1;

            //检测线型
            WIRE_SETTING_T []wire = cfg.wire;

            for (i = 0; i < MAX_MODEL_NUM; i++)
            {
                wire[i].tol.analEN = (ANAL_TYPE)0xFFFF;
                wire[i].tol.sealPosP = 2.0f;		//mm
                wire[i].tol.sealPosN = 2.0f;		//mm
                wire[i].tol.corePosP = 2.0f;		//mm
                wire[i].tol.corePosN = 2.0f;		//mm
                wire[i].tol.stripPosP = 2.0f;		//mm
                wire[i].tol.stripPosN = 2.0f;		//mm
                wire[i].tol.stripLenP = 2.0f;		//mm
                wire[i].tol.stripLenN = 2.0f;		//mm

                wire[i].tol.sealWidthP = 10.0f;		//%
                wire[i].tol.sealWidthN = 10.0f;		//%
                wire[i].tol.coreWidthP = 10.0f;		//%
                wire[i].tol.coreWidthN = 10.0f;		//%

                wire[i].tol.wireSplay = 50.0f;		//%

                wire[i].tol.variationFilter = 1.25f;//mm
                wire[i].tol.sealRatio = 85.0f;		//%
                wire[i].tol.sealLimit = 100.0f;		//%
                wire[i].tol.stripLimit = 50.0f;		//%
            }

            //输出
            for (i = 0; i < OUTPUT_NUM; i++)
            {
                cfg.outputMode[i] = (TYPE_OUTPUT)((UInt32)TYPE_OUTPUT.OUTPUT_NO | (UInt32)TYPE_OUTPUT.OUTPUT_PASS);
                cfg.outputWireIdx[i] = 1;		//输出对应的线型序号
                cfg.outputDelay[i] = 0;			//输出延时ms
                cfg.outputWidth[i] = 50;		//输出脉宽
            }

            //输入
            cfg.inputMode[0] = (TYPE_INPUT)((UInt32)TYPE_INPUT.INPUT_SETUP | (UInt32)TYPE_INPUT.INPUT_TRIG | (UInt32)TYPE_INPUT.INPUT_POS);			//trigger:level function:Setup, polarity:Positive
            cfg.inputMode[1] = (TYPE_INPUT)((UInt32)TYPE_INPUT.INPUT_LEARN | (UInt32)TYPE_INPUT.INPUT_POS | (UInt32)TYPE_INPUT.INPUT_NEG);			//trigger:Pos Neg edge function:Learn
            cfg.inputMode[2] = (TYPE_INPUT)((UInt32)TYPE_INPUT.INPUT_FIRING | (UInt32)TYPE_INPUT.INPUT_SETUP | (UInt32)TYPE_INPUT.INPUT_POS);		//trigger:level function:Setup, polarity:Positive
            cfg.inputMode[3] = TYPE_INPUT.INPUT_DISABLE;												//trigger:Pos Neg edge function:Learn

            cfg.firingTimeOut = 500;					//触发信号超时设置
            cfg.minSetupDuration = 4000;				//进入setup模式前，输入信号有效的最小持续时间，用于在没有上位机时手动操作进入学习模式

            cfg.senThresh = 10;						//CCD sensor threshold %
            cfg.learnNum = 1;							//学习模式个数 1～16次 初值：1  设置在学习时以几根良品的波形平均值作为基准波形。

            cfg.sn = 0;			//产品序列号
            cfg.downCnt = 0;			//20180816 设备寿命计数器。默认为0，无限寿命。大于零时，每压接一条递减。递减到0时显示“设备计数异常！”
            cfg.valid = CFG_VALID;	//设置配置有效标识
        }

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
        public CFG_T alloc_CFG_T()
        {
            CFG_T cfg = new CFG_T();
            cfg.wire = new WIRE_SETTING_T[MAX_MODEL_NUM];
            cfg.outputMode = new TYPE_OUTPUT[OUTPUT_NUM];
            cfg.outputDelay = new int[OUTPUT_NUM];
            cfg.outputWidth = new int[OUTPUT_NUM];
            cfg.outputWireIdx = new int[OUTPUT_NUM];
            cfg.inputMode = new TYPE_INPUT[INPUT_NUM];
            return cfg;
        }

        private void btUpdateCfg_Click(object sender, EventArgs e)
        {
            CFG_T cfg = alloc_CFG_T();
            
            setDefault(ref cfg);       //先设置参数为默认状态，保证各参数有效

            //通过控件参数更新配置信息
            cfg.modelNum = Convert.ToInt32(GetNumber(cBoxModelNum.Text));
            //WIRE_SETTING_T[] wire = cfg.wire;
            cfg.learnNum = Convert.ToInt32(cBoxLearnNum.Text);
           
            //线型设置
            cfg.wire = new WIRE_SETTING_T[MAX_MODEL_NUM];
            //wire[0]
            //Analyse type
            UInt32 type = 0;
            if (true == ckBoxCorePos1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_CORE_POS;
            if (true == ckBoxSealPos1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_SEAL_POS;
            if (true == ckBoxStripPos1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_STRIP_POS;
            if (true == ckBoxCoreWidth1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_CORE_WIDTH;
            if (true == ckBoxSealWidth1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_SEAL_WIDTH;
            if (true == ckBoxStripLen1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_STRIP_LEN;
            if (true == ckBoxSplay1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_SPLAY;
            if (true == ckBoxSeal1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_IS_SEAL;
            if (true == ckBoxSealOri1.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_SEAL_ORI;

            cfg.wire[0].tol.analEN = (ANAL_TYPE)type;

            cfg.wire[0].tol.sealPosP = Convert.ToSingle(GetNumber(tBoxSealPosP1.Text));		//mm
            cfg.wire[0].tol.sealPosN = Convert.ToSingle(GetNumber(tBoxSealPosN1.Text));		//mm
            cfg.wire[0].tol.corePosP = Convert.ToSingle(GetNumber(tBoxCorePosP1.Text));		//mm
            cfg.wire[0].tol.corePosN = Convert.ToSingle(GetNumber(tBoxCorePosN1.Text));		//mm
            cfg.wire[0].tol.stripPosP = Convert.ToSingle(GetNumber(tBoxStripPosP1.Text));		//mm
            cfg.wire[0].tol.stripPosN = Convert.ToSingle(GetNumber(tBoxStripPosN1.Text));		//mm
            cfg.wire[0].tol.stripLenP = Convert.ToSingle(GetNumber(tBoxStripLenP1.Text));		//mm
            cfg.wire[0].tol.stripLenN = Convert.ToSingle(GetNumber(tBoxStripLenN1.Text));		//mm

            cfg.wire[0].tol.sealWidthP = Convert.ToSingle(GetNumber(tBoxSealWidthP1.Text));	//%
            cfg.wire[0].tol.sealWidthN = Convert.ToSingle(GetNumber(tBoxSealWidthN1.Text));	//%
            cfg.wire[0].tol.coreWidthP = Convert.ToSingle(GetNumber(tBoxCoreWidthP1.Text));	//%
            cfg.wire[0].tol.coreWidthN = Convert.ToSingle(GetNumber(tBoxCoreWidthN1.Text));	//%

            cfg.wire[0].tol.wireSplay = Convert.ToSingle(GetNumber(tBoxSprayP1.Text));		    //%

            cfg.wire[0].tol.variationFilter = Convert.ToSingle(GetNumber(tBoxVariationFilter1.Text));//mm
            cfg.wire[0].tol.sealRatio = Convert.ToSingle(GetNumber(tBoxSealRatio1.Text));		//%
            cfg.wire[0].tol.sealLimit = Convert.ToSingle(GetNumber(tBoxSealLimit1.Text));		//%
            cfg.wire[0].tol.stripLimit = Convert.ToSingle(GetNumber(tBoxStripLimit1.Text));	//%

            //wire[1]   能不能将控件做成数组？这样就可以用for循环，就不用以下拷贝代码
            type = 0;
            if (true == ckBoxCorePos2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_CORE_POS;
            if (true == ckBoxSeal2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_SEAL_POS;
            if (true == ckBoxStripPos2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_STRIP_POS;
            if (true == ckBoxCoreWidth2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_CORE_WIDTH;
            if (true == ckBoxSealWidth2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_SEAL_WIDTH;
            if (true == ckBoxStripLen2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_STRIP_LEN;
            if (true == ckBoxSplay2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_SPLAY;
            if (true == ckBoxSeal2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_IS_SEAL;
            if (true == ckBoxSealOri2.Checked)
                type |= (UInt32)ANAL_TYPE.ANAL_SEAL_ORI;
            cfg.wire[1].tol.analEN = (ANAL_TYPE)type;

            cfg.wire[1].tol.sealPosP = Convert.ToSingle(GetNumber(tBoxSealPosP2.Text));		//mm
            cfg.wire[1].tol.sealPosN = Convert.ToSingle(GetNumber(tBoxSealPosN2.Text));		//mm
            cfg.wire[1].tol.corePosP = Convert.ToSingle(GetNumber(tBoxCorePosP2.Text));		//mm
            cfg.wire[1].tol.corePosN = Convert.ToSingle(GetNumber(tBoxCorePosN2.Text));		//mm
            cfg.wire[1].tol.stripPosP = Convert.ToSingle(GetNumber(tBoxStripPosP2.Text));		//mm
            cfg.wire[1].tol.stripPosN = Convert.ToSingle(GetNumber(tBoxStripPosN2.Text));		//mm
            cfg.wire[1].tol.stripLenP = Convert.ToSingle(GetNumber(tBoxStripLenP2.Text));		//mm
            cfg.wire[1].tol.stripLenN = Convert.ToSingle(GetNumber(tBoxStripLenN2.Text));		//mm

            cfg.wire[1].tol.sealWidthP = Convert.ToSingle(GetNumber(tBoxSealWidthP2.Text));	//%
            cfg.wire[1].tol.sealWidthN = Convert.ToSingle(GetNumber(tBoxSealWidthN2.Text));	//%
            cfg.wire[1].tol.coreWidthP = Convert.ToSingle(GetNumber(tBoxCoreWidthP2.Text));	//%
            cfg.wire[1].tol.coreWidthN = Convert.ToSingle(GetNumber(tBoxCoreWidthN2.Text));	//%

            cfg.wire[1].tol.wireSplay = Convert.ToSingle(GetNumber(tBoxSprayP2.Text));		    //%

            cfg.wire[1].tol.variationFilter = Convert.ToSingle(GetNumber(tBoxVariationFilter2.Text));//mm
            cfg.wire[1].tol.sealRatio = Convert.ToSingle(GetNumber(tBoxSealRatio2.Text));		//%
            cfg.wire[1].tol.sealLimit = Convert.ToSingle(GetNumber(tBoxSealLimit2.Text));		//%
            cfg.wire[1].tol.stripLimit = Convert.ToSingle(GetNumber(tBoxStripLimit2.Text));	//%

            //输出
            cfg.outputMode = new TYPE_OUTPUT[OUTPUT_NUM];
            cfg.outputWireIdx = new int[OUTPUT_NUM];
            cfg.outputDelay = new int[OUTPUT_NUM];
            cfg.outputWidth = new int[OUTPUT_NUM];

            //输出0
            UInt32 output = 0;
            if (rBtnNC1.Checked)
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_NC;


            if (cBoxOutMode1.Text == "合格品输出脉冲")
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_PASS;
            else if (cBoxOutMode1.Text == "不良品输出脉冲")
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_FAIL;
            else if (cBoxOutMode1.Text == "合格/不良品都输出脉冲")
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_ANY;
                
            if (rBtnIgnoreFirst1.Checked)
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_IGN_FIRST;
            else if (rBtnIgnoreLearn1.Checked)
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_IGN_LEARN;
            cfg.outputMode[0] = (TYPE_OUTPUT)output;

            cfg.outputWireIdx[0] = Convert.ToInt32(GetNumber(cBoxOutWire1.Text));		//输出对应的线型序号
            cfg.outputDelay[0] = Convert.ToInt32(GetNumber(tBoxOutDelay1.Text));			//输出延时ms
            cfg.outputWidth[0] = Convert.ToInt32(GetNumber(tBoxDuration1.Text));		//输出脉宽

            //输出1   能不能将控件做成数组？这样就可以用for循环，就不用以下拷贝代码
            output = 0;
            if (rBtnNC2.Checked)
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_NC;


            if (cBoxOutMode2.Text == "合格品输出脉冲")
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_PASS;
            else if (cBoxOutMode2.Text == "不良品输出脉冲")
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_FAIL;
            else if (cBoxOutMode2.Text == "合格/不良品都输出脉冲")
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_ANY;

            if (rBtnIgnoreFirst2.Checked)
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_IGN_FIRST;
            else if (rBtnIgnoreLearn2.Checked)
                output |= (UInt32)TYPE_OUTPUT.OUTPUT_IGN_LEARN;
            cfg.outputMode[1] = (TYPE_OUTPUT)output;

            cfg.outputWireIdx[1] = Convert.ToInt32(GetNumber(cBoxOutWire2.Text));		//输出对应的线型序号
            cfg.outputDelay[1] = Convert.ToInt32(GetNumber(tBoxOutDelay2.Text));			//输出延时ms
            cfg.outputWidth[1] = Convert.ToInt32(GetNumber(tBoxDuration2.Text));		//输出脉宽


            //输入
            cfg.inputMode = new TYPE_INPUT[INPUT_NUM];

            //输入1  能不能将控件做成数组？这样就可以用for循环，就不用以下拷贝代码
            UInt32 input = 0;
            if(cBoxInputModeLvl1.Text == "进入学习模式")
                input |= (UInt32 )TYPE_INPUT.INPUT_LEARN;
            else if(cBoxInputModeLvl1.Text =="触发采样")
                input |= (UInt32)TYPE_INPUT.INPUT_FIRING;
            else if(cBoxInputModeLvl1.Text =="进入设置模式")
                input |= (UInt32)TYPE_INPUT.INPUT_SETUP;
            
            if(cBoxInputPolLvl1.Text == " 上升沿触发")
                input |= (UInt32)TYPE_INPUT.INPUT_POS;
            else if(cBoxInputPolLvl1.Text == "下降沿触发")
                input |= (UInt32)TYPE_INPUT.INPUT_NEG;
            cfg.inputMode[0] = (TYPE_INPUT)input;

            //输入2
            input = 0;
            if (cBoxInputModeLvl2.Text == "进入学习模式")
                input |= (UInt32)TYPE_INPUT.INPUT_LEARN;
            else if (cBoxInputModeLvl2.Text == "触发采样")
                input |= (UInt32)TYPE_INPUT.INPUT_FIRING;
            else if (cBoxInputModeLvl2.Text == "进入设置模式")
                input |= (UInt32)TYPE_INPUT.INPUT_SETUP;

            if (cBoxInputPolLvl2.Text == " 上升沿触发")
                input |= (UInt32)TYPE_INPUT.INPUT_POS;
            else if (cBoxInputPolLvl2.Text == "下降沿触发")
                input |= (UInt32)TYPE_INPUT.INPUT_NEG;
            cfg.inputMode[1] = (TYPE_INPUT)input;

            //输入3
            input = 0;
            if (cBoxInputModeLvl3.Text == "进入学习模式")
                input |= (UInt32)TYPE_INPUT.INPUT_LEARN;
            else if (cBoxInputModeLvl3.Text == "触发采样")
                input |= (UInt32)TYPE_INPUT.INPUT_FIRING;
            else if (cBoxInputModeLvl3.Text == "进入设置模式")
                input |= (UInt32)TYPE_INPUT.INPUT_SETUP;

            if (cBoxInputPolLvl3.Text == " 上升沿触发")
                input |= (UInt32)TYPE_INPUT.INPUT_POS;
            else if (cBoxInputPolLvl3.Text == "下降沿触发")
                input |= (UInt32)TYPE_INPUT.INPUT_NEG;
            cfg.inputMode[2] = (TYPE_INPUT)input;

            //输入4
            input = 0;
            if (cBoxInputModeLvl4.Text == "进入学习模式")
                input |= (UInt32)TYPE_INPUT.INPUT_LEARN;
            else if (cBoxInputModeLvl4.Text == "触发采样")
                input |= (UInt32)TYPE_INPUT.INPUT_FIRING;
            else if (cBoxInputModeLvl4.Text == "进入设置模式")
                input |= (UInt32)TYPE_INPUT.INPUT_SETUP;

            if (cBoxInputPolLvl4.Text == " 上升沿触发")
                input |= (UInt32)TYPE_INPUT.INPUT_POS;
            else if (cBoxInputPolLvl4.Text == "下降沿触发")
                input |= (UInt32)TYPE_INPUT.INPUT_NEG;
            cfg.inputMode[3] = (TYPE_INPUT)input;

            cfg.firingTimeOut = Convert.ToInt32(GetNumber(tBoxFiringTimeOut.Text));					//触发信号超时设置
            cfg.minSetupDuration = Convert.ToInt32(GetNumber(tBoxMinSetupDuration.Text));				//进入setup模式前，输入信号有效的最小持续时间，用于在没有上位机时手动操作进入学习模式

            cfg.senThresh = Convert.ToInt32(GetNumber(tBoxSensorSensitivity.Text));						//CCD sensor threshold %
            cfg.learnNum = Convert.ToInt32(GetNumber(cBoxLearnNum.Text));							//学习模式个数 1～16次 初值：1  设置在学习时以几根良品的波形平均值作为基准波形。

            //速度补偿使能
            if (ckBoxAutoAdjSpeed.Checked)
                cfg.isSpeedAdj = 1;
            else
                cfg.isSpeedAdj = 0;

            //输出有效期间忽略输入（忽略返程阶段）
            if (checkBoxOutIgnoreWhileOutActive.Checked)
                cfg.isIgnInWhenOut = 1;
            else
                cfg.isIgnInWhenOut = 0;

            cfg.valid = CFG_VALID;	//设置配置有效标识

            if(CfgCheck(ref cfg))
            {
                //IMG128 hParent = (IMG128)this.Parent;
                hParent.devCfg = cfg;
                if(hParent.serialPort1.IsOpen)
                //if (Main.serialPort1.IsOpen)
                {
                    byte[] cfgBuf = StructToBytes(cfg);    //将CFG_T转换为byte[]
                    byte[] frm = hParent.devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_SC, cfgBuf, (ushort)cfgBuf.Length);  //生成串口数据帧字符数组
                    hParent.serialPort1.Write(frm, 0, frm.Length);      //发送串口数据
                }
            }

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
            if (btnSetupCCD.Text == "进入传感器设置模式")
            {
                btnSetupCCD.Text = "退出传感器设置模式";
                byte[] mode = new byte[Marshal.SizeOf(typeof(UInt32))];
                mode[0] = (byte)CH_MODE.ADJUSTMENT;
                byte[] frm = hParent.devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_SM, mode, (ushort)Marshal.SizeOf(typeof(UInt32)));
                hParent.serialPort1.Write(frm, 0, frm.Length);

            }
            else
            {
                btnSetupCCD.Text = "进入传感器设置模式";
                byte[] mode = new byte[Marshal.SizeOf(typeof(UInt32))];
                mode[0] = (byte)CH_MODE.WORK;
                byte[] frm = hParent.devProtocol.GetCmdFrm(Protocol.FRAME_TYPE_SM, mode, (ushort)Marshal.SizeOf(typeof(UInt32)));
                hParent.serialPort1.Write(frm, 0, frm.Length);
            }
        }

    }
}
