using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Granity.winTools
{
    /// <summary>
    /// 天敏视频卡驱动接口
    /// </summary>
    internal class Sa7134Capture
    {
        /// <summary>
        /// 初始化视频卡环境
        /// </summary>
        /// <param name="hWndMain">当前渲染的窗口句柄</param>
        /// <param name="bInitVidDev">视频模式</param>
        /// <param name="bInitAudDev">是否音频</param>
        /// <returns>初始化成功返回true,无卡或无驱动则失败</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAInitSdk")]
        public extern static bool VCAInitSdk(IntPtr hWndMain, DISPLAYTRANSTYPE bInitVidDev, bool bInitAudDev);
        //初始化系统资源
        // [DllImport("Sa7134Capture.dll", EntryPoint = "VCAInitSdk")]
        // public extern static bool VCAInitSdk(IntPtr hWndMain, bool bInitVidDev, bool bLnitAuDev);

        /// <summary>
        /// 获取当前视频通道总数
        /// </summary>
        /// <returns>返回通道数,没有则返回0</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAGetDevNum")]
        public extern static int VCAGetDevNum();

        /// <summary>
        /// 打开指定通道,分配响应资源
        /// 系统将自动给定传入子窗口的大小来显示视频. 预览窗口的分辨率为该窗口的尺寸大小
        /// </summary>
        /// <param name="dwCard">通道号</param>
        /// <param name="hPreviewWnd">视频预览窗口句柄</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAOpenDevice")]
        public extern static bool VCAOpenDevice(Int32 dwCard, IntPtr hPreviewWnd);

        /// <summary>
        /// 启动视频预览
        /// </summary>
        /// <param name="dwCard">通道号</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAStartVideoPreview")]
        public extern static bool VCAStartVideoPreview(Int32 dwCard);

        /// <summary>
        /// 停止视频预览
        /// </summary>
        /// <param name="dwCard">通道号</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAStopVideoPreview")]
        public extern static bool VCAStopVideoPreview(Int32 dwCard);

        /// <summary>
        /// 更新视频预览，比如改变了显示窗口时，调用
        /// </summary>
        /// <param name="dwCard">通道号</param>
        /// <param name="hPreviewWnd">预览窗口句柄</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAUpdateVideoPreview")]
        public static extern bool VCAUpdateVideoPreview(Int32 dwCard, IntPtr hPreviewWnd);

        /// <summary>
        /// 保存快照数据到相应的缓冲区
        /// </summary>
        /// <param name="dwCard">通道号</param>
        /// <param name="pDestBuf">缓冲区指针</param>
        /// <param name="dwWidth">图像宽度</param>
        /// <param name="dwHeight">图像高度</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCASaveBitsToBuf")]
        public static extern bool VCASaveBitsToBuf(Int32 dwCard, IntPtr pDestBuf, ref Int32 dwWidth, ref Int32 dwHeight);

        /// <summary>
        /// 保存快照为JPEG文件
        /// </summary>
        /// <param name="dwCard">通道号</param>
        /// <param name="lpFileName">文件绝对路径</param>
        /// <param name="dwQuality">视频质量：1—100</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCASaveAsJpegFile")]
        public extern static bool VCASaveAsJpegFile(Int32 dwCard, string lpFileName, Int32 dwQuality);

        /// <summary>
        /// 关闭通道，释放相应系统资源
        /// </summary>
        /// <param name="dwCard">通道号</param>
        /// <returns>成功返回true,失败返回false</returns>
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCACloseDevice")]
        public extern static bool VCACloseDevice(Int32 dwCard);
    }
    /// <summary>
    /// 天敏卡视频显示模式
    /// </summary>
    internal enum DISPLAYTRANSTYPE
    {
        /// <summary>
        /// 不显示
        /// </summary>
        NOT_DISPLAY = 0,
        /// <summary>
        /// 
        /// </summary>
        PCI_VIEDOMEMORY = 1,
        /// <summary>
        /// 常用模式
        /// </summary>
        PCI_MEMORY_VIDEOMEMORY = 2
    }

}
