using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 4)]	
public struct PIC_SIZE_T{
	public UInt32 x, y;
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct IMG_LINE
{
    [MarshalAs(UnmanagedType.ByValArray , SizeConst = 216 * 3)]
    public byte[] pix;
};

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct REC_ITEM{	
	public UInt32 valid;						//记录有效性标识
	public PIC_SIZE_T size;

    //u8 img[LINE_NUM][CISPixcel];
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]  //REC_IMG_HEIGHT 2 CISPixcel:216*3
    public IMG_LINE[] img;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	UInt32[] reserved;					//备以后扩展
};
//一条记录结构体
////分析过程中用到的各种常量和阈值
////#define PIX_SIZE			0.127f	//200dpi 0.127mm / pixel
////#define SHADE_THRESH		50		//line[0] line[4] 中值小于SHADE_THRESH认为是被遮挡区域
////#define MIN_SPACE_BT_WIRE	80		//线组间最小间距。大于此间距被判定为两组线。单位 pixel 80约为10mm
////#define MAX_WIRE_WITH		40		//最大线宽。
////#define COLOR_ERR_THRESH	20		//颜色判别灵敏度
//#define PIX_NUM			CISPixel
//#define LINE_SHADE1		0
//#define LINE_RED		1
//#define LINE_GREEN		2
//#define LINE_BLUE		3
//#define LINE_SHADE2		4
//#define MAX_WIRE_NUM	100		//一帧中最大导线数量
//#define LEFT			0
//#define RIGHT			1

////分析数据时采5条线
////line[0]	采集彩色之前采集的阴影线
////line[1]	红灯亮时采集的数据线
////line[2]	绿灯亮时采集的数据线
////line[3]	蓝灯亮时采集的数据线
////line[4]	采集彩色之后采集的阴影线
////阴影线用于判断彩线的有效区域。阴影区域是被导线遮挡的区域。用于将导线和背景区分开。
////在采集彩线前后分别采一次阴影线，用于当导线移动时，校准阴影与彩线间的偏移量。

////分过过程中间数据结构体，在各分析函数间传递
//typedef struct tagAnal
//{
//    BOOL valid;			//对于基准_model，valid表示基准是否有效。对于检测_test，valid 表示是否有物体在检测区域
//    BOOL result, lastResult;
//    int wireCnt, lastWireCnt;
//    int shadeCnt[LINE_NUM];  //阴影线导线条数识别数量
//    int wirePos[LINE_NUM][MAX_WIRE_NUM] [2]; //记录每条导线左右边缘位置 第一维表示line[0], line[1-3], line[4],第二维为线序号，第三维为左右
//    int shift;
//    u8 red[MAX_WIRE_NUM], green[MAX_WIRE_NUM], blue[MAX_WIRE_NUM];
//    u8 line[LINE_NUM][PIX_NUM];     //保存处理后的各条线
//    int avrWidth, minWidth, maxWidth;       //线宽

//}
//ANAL_T;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct ANAL_T
{
    public bool isModel;
    public bool valid;//对于基准_model，valid表示基准是否有效。对于检测_test，valid 表示是否有物体在检测区域
    public bool result, lastResult;
    public Int32 wireCnt, lastWireCnt;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]    //REC.LINE_NUM = 5
    public Int32[] shadeCnt;  //阴影线导线条数识别数量
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5 * 100 * 2)]    //LINE_NUM * MAX_WIRE_NUM * 2
    public Int32[] wirePos;//[LINE_NUM][MAX_WIRE_NUM] [2]; //记录每条导线左右边缘位置 第一维表示line[0], line[1-3], line[4],第二维为线序号，第三维为左右
    public Int32 shift;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]    // MAX_WIRE_NUM
    public Byte[] red;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]    // MAX_WIRE_NUM
    public Byte[] green;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]    // MAX_WIRE_NUM
    public Byte[] blue;

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]  //REC_IMG_HEIGHT 2 CISPixcel:216*3
    public IMG_LINE[] img;
    public Int32 avrWidth, minWidth, maxWidth;       //线宽
}

namespace IMG128
{
    public class REC
    {
        public const int PIX_NUM = 216 * 3;                        //行像素数量
        public const int IMG_HEIGHT = 2;                     //图片高度
        public const int BYTES_PER_IMG_LINE = (PIX_NUM / 8);   //每行字节数
        public const int DWORDS_PER_IMG_LINE = (PIX_NUM / 32); //每行UInt32数
        public const double PIX_SIZE = 0.127;         		//200dpi  25.4/200
        //#define LINE_NUM		5	//0: first shade 1:R 2:G 3:B 4:last Shade
        public const int LINE_NUM = 5;	//0: first shade 1:R 2:G 3:B 4:last Shade
        public const int LINE_SHADE1 = 0;
        public const int LINE_RED = 1;
        public const int LINE_GREEN = 2;
        public const int LINE_BLUE = 3;
        public const int LINE_SHADE2 = 4;
        public const int LEFT = 0;
        public const int RIGHT = 1;
        public const int MAX_WIRE_NUM = PIX_NUM;

        REC_ITEM  curRec = new REC_ITEM();
        
        public REC_ITEM RecFrameDeal(COMM_FRAME_T frm)
        {
            curRec = (REC_ITEM)BytesToStruct(frm.data, Marshal.SizeOf(typeof(REC_ITEM)), typeof(REC_ITEM));

            //IntPtr buff = (IntPtr)(new byte[Marshal.SizeOf(typeof(REC_ITEM))]);

            //check record data
            //curRec.valid = true;
            //if dataerror set curRec.valid to false

            return curRec;
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

    }
}
