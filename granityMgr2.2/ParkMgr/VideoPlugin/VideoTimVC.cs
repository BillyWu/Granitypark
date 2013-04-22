using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Granity.granityMgr.ParkMgr.VideoPlugin
{
    /// <summary>
    /// 天敏视频卡VC4000接口
    /// </summary>
    public class VideoTimVC
    {
        //VC4000视频卡常用方法
        //BOOL  WINAPI VCAInitSdk( HWND hWndMain, DISPLAYTRANSTYPE eDispTransType = PCI_VIEDOMEMORY, BOOL bInitAudDev = FALSE  );
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAInitSdk")]
        public extern static bool VCAInitSdk(IntPtr hWndMain, DISPLAYTRANSTYPE bInitVidDev, bool aa);
        //初始化系统资源
        // [DllImport("Sa7134Capture.dll", EntryPoint = "VCAInitSdk")]
        // public extern static bool VCAInitSdk(IntPtr hWndMain, bool bInitVidDev, bool bLnitAuDev);

        //打开指定卡号的设备，分配相应系统资源
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAOpenDevice")]
        public extern static bool VCAOpenDevice(Int32 dwCard, IntPtr hPreviewWnd);

        //开始视频预览
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAStartVideoPreview")]
        public extern static bool VCAStartVideoPreview(Int32 dwCard);

        //停止视频预览
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAStopVideoPreview")]
        public extern static bool VCAStopVideoPreview(Int32 dwCard);

        //保存快照为JPEG文件
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCASaveAsJpegFile")]
        public extern static bool VCASaveAsJpegFile(Int32 dwCard, string lpFileName, Int32 dwQuality);

        //返回系统当中卡号数量，即为SAA7134硬件数目，为0时表示没有设备存在
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCAGetDevNum")]
        public extern static int VCAGetDevNum();

        //关闭指定卡号的设备，释放相应系统资源
        [DllImport("Sa7134Capture.dll", EntryPoint = "VCACloseDevice")]
        public extern static bool VCACloseDevice(Int32 dwCard);
    }
    /// <summary>
    /// 显示类型
    /// </summary>
    public enum DISPLAYTRANSTYPE
    {
        NOT_DISPLAY = 0,
        PCI_VIEDOMEMORY = 1,
        PCI_MEMORY_VIDEOMEMORY = 2
    }
}
