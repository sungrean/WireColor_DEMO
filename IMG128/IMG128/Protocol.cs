using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

//通用串口Tx数据帧结构体
[StructLayout(LayoutKind.Sequential, Pack = 1)] //Pack =4保持与STM32内存结构一致
public struct COMM_TX_GEN_T{
    public char frameHeader;
    public char rsvU8;  //保留，占位，保证与下位机数据结构一致
    public UInt16 len;
    public UInt16 type;
    public byte sum;
    public char frameTail;
};
//通用串口Tx数据帧结构体
[StructLayout(LayoutKind.Sequential, Pack = 1)] //Pack =4保持与STM32内存结构一致
public struct COMM_TX_GEN_HEADER_T
{
    public char frameHeader;
    public char rsvU8;  //保留，占位，保证与下位机数据结构一致
    public UInt16 len;
    public UInt16 type;
};

//串口Rx Tx数据帧结构体
[StructLayout(LayoutKind.Sequential, Pack=1)]
public struct COMM_FRAME_T{
	public char frameHeader;
	public char rsvu8_1;		//占位，用于调整字节对齐
    public UInt16 len;
    public UInt16 type;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8192)]
	public byte[] data;	//
	public byte sum;
	public char frameTail;
};

namespace IMG128
{
    public class Protocol
    {
        public const UInt16 FRAME_TYPE_LI = 0x494C;	//"LI" Login
        public const UInt16 FRAME_TYPE_LO = 0x4F4C;	//"LO" Logout
        public const UInt16 FRAME_TYPE_SM = 0x4D53;	//"SM" SetMode
        public const UInt16 FRAME_TYPE_GC = 0x4347;	//"GC" GetConfig
        public const UInt16 FRAME_TYPE_SC = 0x4353;	//"SC" SetConfig 
        public const UInt16 FRAME_TYPE_GR = 0x5247;	//"GR" Get record
        public const UInt16 FRAME_TYPE_CR = 0x5243;	//"CR" clr record
        public const UInt16 FRAME_TYPE_GS = 0x5347;	//"GS" Get state
        public const UInt16 FRAME_TYPE_RA = 0x4152;	//"RA" Reset Alarm
        public const UInt16 FRAME_TYPE_RP = 0x5052;	//"RP" Report
        public const UInt16 FRAME_TYPE_AL = 0x4C41;	//"AL" Alarm
        public const UInt16 FRAME_TYPE_CE = 0x4543;	//"CE" Check ENC
        public const UInt16 FRAME_TYPE_SN = 0x4E53;	//"SN" Get SN
        public const UInt16 FRAME_TYPE_RG = 0x4752;	//"RG" Registration
        public const UInt16 FRAME_TYPE_RC = 0x4352;	//"RC" Reset counter
        public const UInt16 FRAME_TYPE_ST = 0x5453;	//"ST" Set RTC Time
        public const UInt16 FRAME_TYPE_MS = 0x534D;	//"MS" Mode select
        public const UInt16 FRAME_TYPE_FS = 0x5346;	//"FS" Factory Setting
        public const UInt16 FRAME_TYPE_DT = 0x5444;	//"DT" Start Detection


        static  UInt16 DEF_FRM_LEN = 8;                                       //default frm len
        //static UInt16 FRM_LEN_SM	=	DEF_FRM_LEN + sizeof(CH_MODE);          //Set mode
        //static UInt16 FRM_LEN_AL = DEF_FRM_LEN + sizeof(ANAL_TYPE);	    //报警帧长度
        static UInt16 FRM_LEN_RP = (UInt16)(DEF_FRM_LEN + Marshal.SizeOf(typeof(REC_ITEM)));         //report
        static UInt16 FRM_LEN_GR = (UInt16)(DEF_FRM_LEN + Marshal.SizeOf(typeof(REC_ITEM)));         //Get report
        //static UInt16 FRM_LEN_GS = 	DEF_FRM_LEN + sizeof(PM_STATE) + 4);	//4:程序版本号 Get satate 
        static UInt16 FRM_LEN_GC = (UInt16)(DEF_FRM_LEN + Marshal.SizeOf(typeof(CFG_T)));	        //Get Config 
        static UInt16 FRM_LEN_SC = (UInt16)(DEF_FRM_LEN + Marshal.SizeOf(typeof(CFG_T)));		    //Set Config 
        //static UInt16 FRM_LEN_ST = DEF_FRM_LEN;					                        //Set time
        static UInt16 FRM_LEN_MAX = FRM_LEN_RP;

        const char FRM_HEADER = '[';
        const char FRM_TAIL = ']';
        const byte DEF_CHECKSUM = 0x55; //default frm checksum

        Queue<COMM_FRAME_T> queueRX = new Queue<COMM_FRAME_T>();

        //byte[] frmTX = new byte[2000];    //Tx frame

        ////Cmd DT
        //public byte[] GetCmdFrmDT()
        //{
        //    COMM_TX_GEN_T frm = new COMM_TX_GEN_T();
        //    frm.frameHeader = FRM_HEADER;
        //    frm.len = DEF_FRM_LEN;
        //    frm.type = FRAME_TYPE_DT;
        //    frm.sum = DEF_CHECKSUM;
        //    frm.frameTail = FRM_TAIL ;
        //    byte[] txBuf = StructToBytes(frm);
        //    return txBuf;
        //}


        //Cmd frame without data
        public byte[] GetCmdFrm(ushort type)
        {
            COMM_TX_GEN_T frm = new COMM_TX_GEN_T();
            frm.frameHeader = FRM_HEADER;
            frm.len = DEF_FRM_LEN;
            frm.type = type;
            frm.sum = DEF_CHECKSUM;
            frm.frameTail = FRM_TAIL;
            byte[] txBuf = StructToBytes(frm);
            return txBuf;
        }
        //Cmd frame with data
        public byte[] GetCmdFrm(ushort type, byte[] data, UInt16 len)
        {
            COMM_TX_GEN_HEADER_T frm = new COMM_TX_GEN_HEADER_T();
            frm.frameHeader = FRM_HEADER;
            frm.len = (UInt16)(DEF_FRM_LEN + len);
            frm.type = type;
            byte[] header = StructToBytes(frm);
            byte[] tail = { DEF_CHECKSUM, (byte)FRM_TAIL };
            byte[] txBuf = header.Concat(data).ToArray();
            txBuf = txBuf.Concat(tail).ToArray();
            //txBuf.Concat(tail);
            return txBuf;
        }
        /*********************************************************************************************************
	        Function: Identify the first frame from buffer, copy the frame to _buf[] and delete the frame from _sBuf[]
	        Return: If find frame, return the length of the frame, else return 0.
        */

        private bool sBufGetFrame(ref COMM_FRAME_T frame)
        {
            bool ret = false;
	        int i;
	        int start, end;
	        int len = 0;
	        int lng = 0;	//LNG in frame
	        int frmLen;
	
            //在buff中查找帧头
            for (i = 0; i < rxCnt; i++)
                if (FRM_HEADER == buffRX[i])
                    break;

            //删除帧头前的数据
	        start = i;
	
	        //trim char before header
	        if(start != 0)
	             for (i = 0; i < rxCnt - start; i++)
	                buffRX[i] = buffRX[i + start];
	        rxCnt -= start;
	
	        //Get LNG
            if (rxCnt >= DEF_FRM_LEN)
	        {
                //lng = frame.len;// lng = *(u16*)(_sBuf[port] + 1);//lng = _sBuf[port][1] * 256 + _sBuf[port][2];
                lng = (int)buffRX[2] + (int)buffRX[3] * 256;
                if ((FRM_LEN_MAX < lng) || (lng == 0))	//如果接收到的长度信息不正确，删除缓冲区第一字节
		         {
                    rxCnt--;
                    for (i = 0; i < rxCnt; i++)
                        buffRX[i] = buffRX[i + 1];
                    lng = 0;
		         }
	        }
            ////if lng err, delete frame head, search frame again
            //if (lng > FRM_LEN_MAX)
            //{
            //    for (i = 0; i < rxCnt - HEADER_LEN; i++)
            //        _sBuf[port][i] = _sBuf[port][i+HEADER_LEN];
            //    _sCnt[port] -= HEADER_LEN;
            //}
	        if(lng != 0)
	        {
		        frmLen = lng; //frmLen = lng + HEADER_LEN + TAIL_LEN;
                if (rxCnt >= frmLen)	//接收数据结束
		        {
                    if ((FRM_TAIL == buffRX[frmLen - 1]) && (DEF_CHECKSUM == buffRX[frmLen - 2]))	//frame tail OK Checksum OK
			        {
				        end = frmLen;
                        byte[] buf = new byte[FRM_LEN_MAX];
				        for(i = 0; i < end; i++)
                            buf[i] = buffRX[i];
				        len = frmLen;
                        for (i = end; i < rxCnt; i++)
				        {
					        buffRX[i - end] = buffRX[i];
				        }
                        rxCnt = rxCnt - end;
                        frame.data = new byte[frmLen - DEF_FRM_LEN];


                        frame = (COMM_FRAME_T)BytesToStruct(buf, frmLen, typeof(COMM_FRAME_T));
                        ret = true;
			        }
			        else	//if frame tail err, delete frame head, search frame again
			        {
                        rxCnt--;
                        for (i = 0; i < rxCnt; i++)
                            buffRX[i] = buffRX[i + 1];
			        }
		        }
	        }
	        
	        return ret;
        }

        //private void COMM_frameDeal(COMM_FRAME_T frame)
        //{
        //    ushort type = frame.type;
        //    type -= 0x2020; //两个字符转为大写
        //    switch (type)
        //    {
        //        case FRAME_TYPE_RP: queueRX.Enqueue(frame); break;
        //        //case FRAME_TYPE_LI: COMM_cmdLI(port); break;
        //        //case FRAME_TYPE_LO: COMM_cmdLO(port); break;
        //        //case FRAME_TYPE_SM: COMM_cmdSM(port, p); break;
        //        //case FRAME_TYPE_GC: COMM_cmdGC(port); break;
        //        //case FRAME_TYPE_SC: COMM_cmdSC(port, p); break;
        //        //case FRAME_TYPE_GR: COMM_cmdGR(port); break;
        //        //case FRAME_TYPE_CR: COMM_cmdCR(port); break;
        //        //case FRAME_TYPE_GS: COMM_cmdGS(); break;
        //        //case FRAME_TYPE_RA: COMM_cmdRA(port, p); break;
        //        //case FRAME_TYPE_RC: COMM_cmdRC(port); break;		//20180902 清除计数器
        //        //case FRAME_TYPE_ST: COMM_cmdST(port, p); break;	//20180902 同步系统时间
        //        //case FRAME_TYPE_MS: COMM_cmdMS(port, p); break;	//20190618 选择基准波形
        //        //case FRAME_TYPE_FS: COMM_cmdFS(port, p); break;	//20190722 厂家设置
        //        //case FRAME_TYPE_DT: COMM_cmdDT(); break;	//20190722 厂家设置

        //        default: break;
        //    }
        //}
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //Add rx data to rx buff
        const int RX_BUFF_SIZE = 20000;
        byte[] buffRX = new byte[RX_BUFF_SIZE];    //RX buffer
        int rxCnt = 0;
        public bool RxBuffAdd(byte[] rx, int len)
        {
            int i;
            bool ret = true;
            COMM_FRAME_T frame = new COMM_FRAME_T();

            if (len + rxCnt >= RX_BUFF_SIZE)
                return false;

            //添加新收到的数据到末端
            for (i = 0; i < len; i++)
                buffRX[i + rxCnt] = rx[i];
            rxCnt += len;
            

            while(sBufGetFrame(ref frame))
			{
                queueRX.Enqueue(frame);
			}
            return ret;
        }

        public bool GetRxFrame(ref COMM_FRAME_T frame)
        {
            bool ret = false;

            if(queueRX.Count >= 1)
            {
                frame = queueRX.Dequeue();
                ret = true;
            }
            return ret;
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
        //BytesToStruct
        IntPtr buffer = Marshal.AllocHGlobal(20000);
        public object BytesToStruct(byte[] buf, int len, Type type)
        {
            object rtn;
            
            Marshal.Copy(buf, 0, buffer, len);
            rtn = Marshal.PtrToStructure(buffer, type);
            //Marshal.FreeHGlobal(buffer);
            return rtn;
        }
        //BytesToStruct
        public void BytesToStruct(byte[] buf, int len, object rtn)
        {
            IntPtr buffer = Marshal.AllocHGlobal(len);
            Marshal.Copy(buf, 0, buffer, len);
            Marshal.PtrToStructure(buffer, rtn);
            Marshal.FreeHGlobal(buffer);
        }
        //BytesToStruct
        public void BytesToStruct(byte[] buf, object rtn)
        {
            BytesToStruct(buf, buf.Length, rtn);
        }
        //BytesToStruct
        public object BytesToStruct(byte[] buf, Type type)
        {
            return BytesToStruct(buf, buf.Length, type);
        }

        //BytesTohexString
        public static string BytesTohexString(byte[] bytes)
        {
            if (bytes == null || bytes.Count() < 1)
            {
                return string.Empty;
            }

            var count = bytes.Count();

            var cache = new StringBuilder();
            //cache.Append("0x");
            for (int ii = 0; ii < count; ++ii)
            {
                var tempHex = Convert.ToString(bytes[ii], 16).ToUpper();
                cache.Append(tempHex.Length == 1 ? "0" + tempHex : tempHex);
            }

            return cache.ToString();
        }
    }
}
