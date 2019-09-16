using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 4)]	
public struct PIC_SIZE_T{
	public UInt32 x, y;
};

[StructLayout(LayoutKind.Sequential, Pack = 4)]
public struct REC_ITEM{	
	public UInt32 valid;						//记录有效性标识
	public PIC_SIZE_T size;
	
    //u32  img[IMG_HEIGHT][PIX_NUM/32];	//图像
    [MarshalAs(UnmanagedType.ByValArray , SizeConst = 100 * 4)]
    public UInt32[] img;

	public UInt32 modelIdx;						//参考基准序号

	public ANAL_TYPE result, analEN;			//报警状态及报警使能设置
	
	public Single corePosMax, corePosMin, corePosStd, corePos;	//线端位置及有效范围
	public Single  coreWidthMax, coreWidthMin, coreWidthStd, coreWidth;	//线径有效范围及线径
	public Single  sealPosMax, sealPosMin, sealPosStd, sealPos;	//防水栓位置及有效范围
	public Single  sealWidthMax, sealWidthMin, sealWidthStd, sealWidth;	//防水栓直径
	public Single  stripPosMax, stripPosMin, stripPosStd, stripPos;	//剥切位置有效范围及值
	public Single  stripLenMax, stripLenMin, stripLenStd, stripLen;	//剥切长度有效范围及值
	public Single  splayMax, splayMin, splayStd, splay;	//分叉宽度

    public Int32 totalCnt;						//总计个数
	public Int32 alarmCnt;						//次品个数
	//float cpk;							//CPK		ANAL_CalcCPK()
	//float stability;					//安定性		ANAL_CalcStb()

	//以下参数是分析波形时会用到
	public UInt32 isSeal;						//有无防水栓
	public Single   refWidth;					//导线外径，作为图像高度缩放比例参考
	public Single   yScale;						//原始图像纵向缩放比例

    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
	UInt32[] reserved;					//备以后扩展
};

namespace IMG128
{
    public class REC
    {
        public const int PIX_NUM = 128;                        //行像素数量
        public const int IMG_HEIGHT = 100;                     //图片高度
        public const int BYTES_PER_IMG_LINE = (PIX_NUM / 8);   //每行字节数
        public const int DWORDS_PER_IMG_LINE = (PIX_NUM / 32); //每行UInt32数
        public const double PIX_SIZE = 0.127;         		//200dpi  25.4/200

        REC_ITEM curRec = new REC_ITEM();

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
