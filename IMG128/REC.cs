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
