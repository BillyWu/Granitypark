using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Granity.granityMgr.ParkMgr.VideoPlugin
{
    /// <summary>
    /// 迷你视频卡接口
    /// </summary>
    public class VideoMiniMV
    {
        //Mini V110视频卡常用方法
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern uint MV_GetDeviceNumber();   //板卡数

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OpenDevice(uint Index, bool bRelese);  //创建设备

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern bool MV_SetDeviceParameter(int hDevice, int Oper, uint Val); //设置设备属性

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 捕获一帧
        public static extern IntPtr MV_CaptureSingle(int hDevice, bool IsProcess, int Buffer, int Bufflen, ref MV_IMAGEINFO pinfo);

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OperateDevice(int hDevice, int Oper);   //操纵设备

        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 保存文件
        public static extern bool MV_SaveFile([MarshalAs(UnmanagedType.LPStr)] String FileName, int FileType, IntPtr pImageData, ref MV_IMAGEINFO pinfo, int ImageTotal, bool IsUpDown, bool ColororNot, int Quality, bool m_bRGB15);

        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern void MV_CloseDevice(int hDevice);  //删除设备
    }
    [StructLayout(LayoutKind.Explicit)]
    public struct MV_IMAGEINFO
    {
        [FieldOffset(0)]
        public uint Length;      // 图像的大小，以字节计
        [FieldOffset(8)]
        public uint nColor;      // 图像的颜色
        [FieldOffset(16)]
        public uint Heigth;      // 图像的高
        [FieldOffset(24)]
        public uint Width;       // 图像的宽
        [FieldOffset(32)]
        public uint SkipPixel;   // 每行跳过的像素
    }
    enum RUNOPER
    {
        MVSTOP = 0, // MVSTOP: 停止工作；
        MVRUN = 1, // MVRUN: 卡开始工作；
        MVPAUSE = 2, //MVPAUSE: 暂停卡的工作；
        MVQUERYSTATU = 3, //MVQUERYSTATU: 查询卡的当前状态，
        MVERROR = 4 //MVERROR:为错误的状态。
    };
    enum MV_PARAMTER
    {
        GET_BOARD_TYPE = 0, GET_GRAPHICAL_INTERFACE = 1,
        SET_GARBIMAGEINFO = 2, SET_DISPIMAGEINFO = 3,
        BUFFERTYPE = 4, DEFAULT_PARAM = 5,

        // 控制显示的
        DISP_PRESENCE = 6, DISP_WHND = 7,
        DISP_TOP = 8, DISP_LEFT = 9,
        DISP_HEIGHT = 10, DISP_WIDTH = 11,

        // 控制A/D的调节参数
        ADJUST_STANDARD = 12, ADJUST_SOURCE = 13,
        ADJUST_CHANNEL = 14, ADJUST_LUMINANCE = 15,
        ADJUST_CHROMINANE = 16, ADJUST_SATURATION = 17,
        ADJUST_HUE = 18, ADJUST_CONTRAST = 19,

        //支持RGB卡
        ADJUST_R_LUM = 20, ADJUST_G_LUM = 21,
        ADJUST_B_LUM = 22, ADJUST_R_COARSE = 23,
        ADJUST_G_COARSE = 24, ADJUST_B_COARSE = 25,

        // 控制板卡的捕获参数
        GRAB_XOFF = 60, GRAB_YOFF = 61,
        GRAB_HEIGHT = 62, GRAB_WIDTH = 63,
        GRAB_IN_HEIGHT = 64, GRAB_IN_WIDTH = 65,
        GRAB_BITDESCRIBE = 66, GRAB_WHOLEWIDTH = 67,

        // 控制板卡的工作参数
        WORK_UPDOWN = 34, WORK_FLIP = 35,
        WORK_SKIP = 36, WORK_SYNC = 37,
        WORK_INTERLACE = 38, WORK_ISBLACK = 39,
        WORK_FIELD = 40, OSD_MODE = 41,

        //支持V500系列卡
        TENBIT_MODE = 42, OUTPUT_VIDEO = 43,
        FILERSELECT1 = 44, FILERSELECT2 = 45,

        // 控制板卡的捕获参数(保留,兼容老版本)
        GARB_XOFF = 26, GARB_YOFF = 27,
        GARB_HEIGHT = 28, GARB_WIDTH = 29,
        GARB_IN_HEIGHT = 30, GARB_IN_WIDTH = 31,
        GARB_BITDESCRIBE = 32, GARB_WHOLEWIDTH = 33,


        //shen add 
        //支持卡类型MVBOARD2.h中所有卡
        DISP_FLIP = 201, IMAGE_PROCESS = 202,
        VIDEO_SINGLE = 203, GET_BOARD_PASS = 204,
        //20050407新增
        RESTARTCAPTURE = 300, RESTOPCAPTURE = 301,
    };
}
