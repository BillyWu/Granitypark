using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Granity.winTools
{
    /// <summary>
    /// 微视视频卡驱动接口
    /// </summary>
    internal class MvAPI
    {
        /// <summary>
        /// 获取通道总数
        /// </summary>
        /// <returns>返回通道数,没有则返回0</returns>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern uint MV_GetDeviceNumber();

        /// <summary>
        /// 打开通道,返回通道句柄
        /// </summary>
        /// <param name="Index">通道号</param>
        /// <param name="bRelese">是否发布版本SDK</param>
        /// <returns>返回通道句柄,失败返回0</returns>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OpenDevice(uint Index, bool bRelese);

        /// <summary>
        /// 设置设备和SDK的工作参数
        /// </summary>
        /// <param name="hDevice">通道句柄</param>
        /// <param name="Oper">工作参数</param>
        /// <param name="Val">参数值</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern bool MV_SetDeviceParameter(int hDevice, int Oper, uint Val);

        /// <summary>
        /// 捕获一幅图像的数据，当pInBuff为NULL时使用内部的缓冲区。
        /// </summary>
        /// <param name="hDevice">通道句柄</param>
        /// <param name="IsProcess">捕获的图像是否经过处理</param>
        /// <param name="Buffer">用户指定的缓冲区地址</param>
        /// <param name="Bufflen">缓冲区长度</param>
        /// <param name="pinfo">调用返回后为图像信息描述的结构</param>
        /// <returns>返回存有图像数据的缓冲区指针</returns>
        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr MV_CaptureSingle(int hDevice, bool IsProcess, int Buffer, int Bufflen, ref MV_IMAGEINFO pinfo);

        /// <summary>
        /// 操纵设备, 即使采集卡处于运行/停止/暂停状态。
        /// </summary>
        /// <param name="hDevice">通道句柄</param>
        /// <param name="Oper">设备的状态(运行，停止，暂停，查询)</param>
        /// <returns>返回卡的当前工作状态</returns>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern int MV_OperateDevice(int hDevice, int Oper);

        /// <summary>
        /// 保存pImageData中图像到MV_FILETYPE指定类型的文件。
        /// </summary>
        /// <param name="FileName">文件路径</param>
        /// <param name="FileType">文件类型RAW:1/BMP文件，2/JPEG文件。</param>
        /// <param name="pImageData">图像数据缓冲区指针</param>
        /// <param name="pinfo">图像描述信息</param>
        /// <param name="ImageTotal">图像数量</param>
        /// <param name="IsUpDown">保存图像时是否上下颠倒</param>
        /// <param name="ColororNot">存JPEG文件时是否要颜色</param>
        /// <param name="Quality">存JPEG文件时的质量0—99</param>
        /// <param name="m_bRGB15">是否将16bit色彩转换为24bit色彩</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("MVAPI.dll", CharSet = CharSet.Auto)]  // 保存文件
        public static extern bool MV_SaveFile([MarshalAs(UnmanagedType.LPStr)] String FileName, int FileType, IntPtr pImageData, ref MV_IMAGEINFO pinfo, int ImageTotal, bool IsUpDown, bool ColororNot, int Quality, bool m_bRGB15);

        /// <summary>
        /// 关闭通道并释放句柄资源
        /// </summary>
        /// <param name="hDevice">通道句柄</param>
        [DllImport("mvapi.dll", CharSet = CharSet.Auto)]
        public static extern void MV_CloseDevice(int hDevice);  //删除设备
    }

    /// <summary>
    /// 微视视频卡图像描述信息
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MV_IMAGEINFO
    {
        /// <summary>
        /// 图像的大小，以字节计
        /// </summary>
        [FieldOffset(0)]
        public uint Length;
        /// <summary>
        /// 图像的颜色
        /// </summary>
        [FieldOffset(8)]
        public uint nColor;
        /// <summary>
        /// 图像的高
        /// </summary>
        [FieldOffset(16)]
        public uint Heigth;
        /// <summary>
        /// 图像的宽
        /// </summary>
        [FieldOffset(24)]
        public uint Width;
        /// <summary>
        /// 每行跳过的像素
        /// </summary>
        [FieldOffset(32)]
        public uint SkipPixel;
    }

    /// <summary>
    /// 微视视频卡运行状态
    /// </summary>
    internal enum RUNOPER
    {
        /// <summary>
        /// 停止工作
        /// </summary>
        MVSTOP = 0,
        /// <summary>
        /// 卡开始工作
        /// </summary>
        MVRUN = 1,
        /// <summary>
        /// 暂停卡的工作
        /// </summary>
        MVPAUSE = 2,
        /// <summary>
        /// 查询卡的当前状态
        /// </summary>
        MVQUERYSTATU = 3,
        /// <summary>
        /// 为错误的状态
        /// </summary>
        MVERROR = 4
    }

    /// <summary>
    /// 微视视频卡工作参数
    /// </summary>
    internal enum MV_PARAMTER
    {
        GET_BOARD_TYPE = 0, GET_GRAPHICAL_INTERFACE = 1,
        SET_GARBIMAGEINFO = 2, SET_DISPIMAGEINFO = 3,
        BUFFERTYPE = 4, DEFAULT_PARAM = 5,

        // 控制显示的
        DISP_PRESENCE = 6, 
        /// <summary>
        /// 显示控件句柄
        /// </summary>
        DISP_WHND = 7,
        /// <summary>
        /// 显示区顶位置
        /// </summary>
        DISP_TOP = 8,
        /// <summary>
        /// 显示区左位置
        /// </summary>
        DISP_LEFT = 9,
        /// <summary>
        /// 显示区高度
        /// </summary>
        DISP_HEIGHT = 10,
        /// <summary>
        /// 显示区宽度
        /// </summary>
        DISP_WIDTH = 11,

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
