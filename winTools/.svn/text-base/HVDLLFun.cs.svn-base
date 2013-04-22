#region 版本说明

/*
 * 功能内容：   车牌硬识别DLL接口
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-07-27
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Granity.winTools
{
    /// <summary>
    /// 车牌识别函数库
    /// </summary>
    public unsafe class HVDLLFun
    {
        /// <summary>
        /// 车牌信息开始回调函数指针,系统在接收一次识别结果前调用本函数
        /// 函数指针在函数返回时即被释放
        /// </summary>
        /// <param name="pFirstParameter">回调时传递的第一个参数,该参数一般为一个对象的指针(Marshal.GetObjectForIUnknown)</param>
        /// <param name="dwCarID">车辆ID</param>
        /// <returns>预留将来用途,建议0/成功,-1/失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long CARINFO_BEGIN_CALLBACK(IntPtr pFirstParameter, UInt32 dwCarID);

        /// <summary>
        /// 车牌信息结束回调,系统在接收完一次识别结果后调用本函数
        /// 函数指针在函数返回时即被释放
        /// </summary>
        /// <param name="pFirstParameter">回调时传递的第一个参数,该参数一般为一个对象的指针(Marshal.GetObjectForIUnknown)</param>
        /// <param name="dwCarID">车辆ID</param>
        /// <returns>预留将来用途,建议0/成功,-1/失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long CARINFO_END_CALLBACK(IntPtr pFirstParameter, UInt32 dwCarID);

        /// <summary>
        /// 车牌号码回调函数指针,
        /// 函数指针在函数返回时即被释放
        /// </summary>
        /// <param name="pFirstParameter">回调时传递的第一个参数,该参数一般为一个对象的指针(Marshal.GetObjectForIUnknown)</param>
        /// <param name="dwCarID">车辆ID</param>
        /// <param name="pcPlateNo">车牌号码字符串</param>
        /// <param name="dwTimeMs">时间(1970-1-1 0:0:0以来的毫秒数);</param>
        /// <returns>预留将来用途,建议0/成功,-1/失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long PLATE_NO_CALLBACK(IntPtr pFirstParameter, uint dwCarID, string pcPlateNo, ulong dwTimeMs);

        /// <summary>
        /// 车辆全景大图回调函数指针,
        /// 函数指针在函数返回时即被释放
        /// </summary>
        /// <param name="pFirstParameter">回调时传递的第一个参数,该参数一般为一个对象的指针(Marshal.GetObjectForIUnknown)</param>
        /// <param name="dwCarID">车辆ID</param>
        /// <param name="wImageWidth">图像的宽度</param>
        /// <param name="wImageHigh">图像的高度</param>
        /// <param name="bType">图像类型</param>
        /// <param name="wSize">数据大小</param>
        /// <param name="pbImage">图像数据的指针</param>
        /// <param name="wImageID">图像的ID号，表示得到的是哪一张图片</param>
        /// <param name="wHighImgFlag">高清图片标志,如果该变量高8位值是0xFF00, 则该变量低8位值为图片大小的高位值</param>
        /// <param name="wPlateWidth">表示该图象中检测到的车牌矩形左上坐标（低8位是左，高8位是上）,坐标都是用原图的百分比表示</param>
        /// <param name="wPlateHigh">表示该图象中检测到的车牌矩形右下坐标（低8位是右，高8位是下）,坐标都是用原图的百分比表示</param>
        /// <param name="dwTimeMs">时间(1970-1-1 0:0:0以来的毫秒数)</param>
        /// <returns>预留将来用途,建议0/成功,-1/失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long BIG_IMAGE_CALLBACK(IntPtr pFirstParameter, uint dwCarID, ushort wImageWidth, ushort wImageHigh, byte bType, ushort wSize,
            byte* pbImage, ushort wImageID, ushort wHighImgFlag, ushort wPlateWidth, ushort wPlateHigh, ulong dwTimeMs);

        /// <summary>
        /// 车牌小图回调函数指针
        /// 函数指针在函数返回时即被释放
        /// </summary>
        /// <param name="pFirstParameter">回调时传递的第一个参数,该参数一般为一个对象的指针(Marshal.GetObjectForIUnknown)</param>
        /// <param name="dwCarID">车辆ID</param>
        /// <param name="wImageWidth">图像的宽度</param>
        /// <param name="wImageHigh">图像的高度</param>
        /// <param name="bType">图像类型</param>
        /// <param name="wSize">数据大小</param>
        /// <param name="pbImage">图像数据的指针</param>
        /// <param name="dwTimeMs">时间(1970-1-1 0:0:0以来的毫秒数)</param>
        /// <returns>预留将来用途,建议0/成功,-1/失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long SMALL_IMAGE_CALLBACK(IntPtr pFirstParameter, uint dwCarID, ushort wImageWidth, ushort wImageHigh,
            byte bType, ushort wSize, byte* pbImage, ulong dwTimeMs);

        /// <summary>
        /// 车牌小图像二值化回调函数指针
        /// 函数指针在函数返回时即被释放
        /// </summary>
        /// <param name="pFirstParameter">回调时传递的第一个参数,该参数一般为一个对象的指针(Marshal.GetObjectForIUnknown)</param>
        /// <param name="dwCarID">车辆ID</param>
        /// <param name="wImageWidth">图像的宽度</param>
        /// <param name="wImageHigh">图像的高度</param>
        /// <param name="bType">图像类型</param>
        /// <param name="wSize">数据大小</param>
        /// <param name="pbImage">图像数据的指针</param>
        /// <param name="dwTimeMs">时间(1970-1-1 0:0:0以来的毫秒数)</param>
        /// <returns>预留将来用途,建议0/成功,-1/失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long BINARY_IMAGE_CALLBACK(IntPtr pFirstParameter, UInt32 dwCarID, UInt16 wImageWidth, UInt16 wImageHigh,
                Byte bType, UInt16 wSize, ref Byte pbImage, UInt64 dwTimeMs);

        /// <summary>
        /// 实时视频回调函数指针
        /// </summary>
        /// <param name="pFirstParameter">回调时传递的第一个参数,该参数一般为一个对象的指针(Marshal.GetObjectForIUnknown)</param>
        /// <param name="wVideoID">视频流编号</param>
        /// <param name="dwSize">数据大小</param>
        /// <param name="pbImage">图像数据的指针</param>
        /// <returns>预留将来用途,建议0/成功,-1/失败</returns>
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate long VIDEO_CALLBACK(IntPtr pFirstParameter, UInt16 wVideoID, UInt16 dwSize, Byte* pbImage);

        /// <summary>
        /// 搜索局域网内所有的视频处理设备，返回设备数目
        /// </summary>
        /// <param name="pdwCount">返回车牌识别器设备数目</param>
        /// <returns>成功返回一个的视频处理系统的句柄,失败则返回E_HV_INVALID_HANDLE</returns>
        [DllImport("HVDLL.dll", EntryPoint = "SearchHVDeviceCount", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SearchHVDeviceCount(uint* pdwCount);

        /// <summary>
        /// 根据索引查询设备ip地址
        /// </summary>
        /// <param name="iIndex">指定查询的索引,该索引不大于视频处理系统的最大数目</param>
        /// <param name="pdw64MacAddr">返回设备以太网地址</param>
        /// <param name="pdwIP">返回设备IP地址</param>
        /// <param name="pdwMask">返回设备IP掩码</param>
        /// <param name="pdwGateway">返回设备IP网关地址</param>
        /// <returns>成功返回一个的视频处理系统的句柄,失败则返回E_HV_INVALID_HANDLE</returns>
        [DllImport("HVDLL.dll", EntryPoint = "GetHVDeviceAddr", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetHVDeviceAddr(int iIndex, UInt64* pdw64MacAddr, UInt32* pdwIP, UInt32* pdwMask, UInt32* pdwGateway);

        /// <summary>
        /// 打开一个视频处理系统句柄 在对视频处理系统作任何操作之前打开视频处理系统取得一个句柄
        /// </summary>
        /// <param name="pcIp">视频处理系统的IP地址或串口名称</param>
        /// <returns>成功返回一个的视频处理系统的句柄,失败则返回E_HV_INVALID_HANDLE</returns>
        [DllImport("HVDLL.dll", EntryPoint = "OpenHv", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr OpenHv(string pcIp);

        /// <summary>
        /// 关闭一个视频处理系统句柄
        /// </summary>
        /// <param name="hHandle">由OpenHv()函数打开的句柄</param>
        /// <returns></returns>
        [DllImport("HVDLL.dll", EntryPoint = "CloseHv", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int CloseHv(IntPtr hHandle);

        /// <summary>
        /// 设置一个视频流的一种数据类型的回调函数
        /// 若有数据传回而且是视频，则调用视频回调函数；若是其他数据传回，则回调其他数据类型函数；若数据传回失败，则返回上一层。
        /// </summary>
        /// <param name="hHandle">由OpenHv()函数打开的句柄</param>
        /// <param name="pFunction">回调函数的函数指针，为NULL则关闭该回调(Marshal.GetFunctionPointerForDelegate)</param>
        /// <param name="pFirstParameter">回调pFunction的第一个参数，该参数一般为一个对象的指针(Marshal.GetIUnknownForObject)</param>
        /// <param name="wVideoID">表示视频ID的2字节的无符号整数</param>
        /// <param name="wStream">表示数据类型的2字节的无符号整数(HvStreamType)</param>
        /// <returns></returns>
        [DllImport("HVDLL.dll", EntryPoint = "SetHvCallBack", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int SetHvCallBack(IntPtr hHandle, IntPtr pFunction, IntPtr pFirstParameter, UInt16 wVideoID, UInt16 wStream);

        /// <summary>
        /// 获取连接状态
        /// </summary>
        /// <param name="hHandle">由OpenHv()函数打开的句柄</param>
        /// <param name="piStatus">输出连接状态 0表示已连接 -1表示未连接</param>
        /// <returns>返回S_OK/成功；返回E_HANDLE/非法的句柄；返回E_FAIL/操作失败</returns>
        [DllImport("HVDLL.dll", EntryPoint = "HvIsConnected", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int HvIsConnected(IntPtr hHandle, ref Int32 piStatus);

        /// <summary>
        /// 转换YUV数据到RGB
        /// </summary>
        /// <param name="pbDest">输出RGB数据的缓冲区指针</param>
        /// <param name="iDestBufLen">输出缓冲区大小</param>
        /// <param name="piDestLen">实际输出数据大小</param>
        /// <param name="pbSrc">输入YUV数据的缓冲区指针</param>
        /// <param name="iSrcWidth">图像宽度</param>
        /// <param name="iSrcHeight">图像高度</param>
        /// <returns>返回S_OK/成功；返回E_HANDLE/非法的句柄；返回E_FAIL/操作失败</returns>
        [DllImport("HVDLL.dll", EntryPoint = "Yuv2BMP", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int Yuv2BMP(byte* pbDest, int iDestBufLen, ref int piDestLen, byte* pbSrc, int iSrcWidth, int iSrcHeight);

        /// <summary>
        /// 读取车牌附加信息(根据实际情况包括车速，车牌位置，亮度等,)
        /// 本函数在识别器设置为输出附加信息时才能取得数据
        /// </summary>
        /// <param name="hHandle">由OpenHv()函数打开的句柄</param>
        /// <param name="ppszPlateInfo">输入指向车牌附加信息的指针的地址</param>
        /// <returns>返回S_OK/成功；返回E_HANDLE/非法的句柄；返回E_FAIL/操作失败</returns>
        [DllImport("HVDLL.dll", EntryPoint = "HV_GetPlateInfo", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int HV_GetPlateInfo(IntPtr hHandle, byte** ppszPlateInfo);
    }

    /// <summary>
    /// 车牌识别字节流类型
    /// </summary>
    public enum HvStreamType
    {
        /// <summary>
        /// 车牌号码字符流
        /// </summary>
        PlateStr = 0x8002,
        /// <summary>
        /// 车牌大图象流
        /// </summary>
        BigImage = 0x8003,
        /// <summary>
        /// 车牌小图象流
        /// </summary>
        SmallImage = 0x8004,
        /// <summary>
        /// 完整的压缩视频流(暂不支持)
        /// </summary>
        FullCmpImage = 0x8005,
        /// <summary>
        /// 车牌二值化小图象流
        /// </summary>
        BinaryImage = 0x8006,
        /// <summary>
        /// 车辆信息开始接收信号
        /// </summary>
        CarinfoBegin = 0x8008,
        /// <summary>
        /// 车辆信息结束接收信号
        /// </summary>
        CarinfoEnd = 0x8009,
        /// <summary>
        /// 视频检测器的触发信息
        /// </summary>
        TriggerInfo = 0x8010,
        /// <summary>
        /// 与检测有关的调试二进制流
        /// </summary>
        CustomerDetectBin = 0x810D,
        /// <summary>
        /// 与性能有关的调试字符流
        /// </summary>
        PerfStr = 0x8105,
        /// <summary>
        /// AVI数据块
        /// </summary>
        AviBlock = 0x810E,
        /// <summary>
        /// 高清硬盘录像
        /// </summary>
        VideoHistory = 0x810F
    }

}
