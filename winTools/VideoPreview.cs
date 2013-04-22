using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Granity.winTools
{
    /// <summary>
    /// 视频预览
    /// </summary>
    public class VideoPreview
    {
        /// <summary>
        /// 视频卡类别
        /// </summary>
        private VideoType VideoType;

        public VideoType videoType
        {
            get { return VideoType; }
            set { VideoType = value; }
        }
        /// <summary>
        /// 通道句柄
        /// </summary>
        private Dictionary<int, int> channelHandle = new Dictionary<int, int>();

        /// <summary>
        /// 初始化环境
        /// </summary>
        /// <param name="hdlwin">初始化当前窗口</param>
        /// <param name="videotype">视频卡类别</param>
        /// <returns>返回视频卡通道数量</returns>
        public int InitVideo(IntPtr hdlwin, VideoType videotype)
        {
            int channel = 0;
            switch (videotype)
            {
                case VideoType.MvAPI:
                    try
                    {
                        channel = (int)MvAPI.MV_GetDeviceNumber();
                    }
                    catch (Exception ex)
                    {
                        channel = 0;
                    }
                    break;
                case VideoType.Sa7134Capture:
                    try
                    {
                        if (Sa7134Capture.VCAInitSdk(hdlwin, DISPLAYTRANSTYPE.PCI_MEMORY_VIDEOMEMORY, false))
                            channel = Sa7134Capture.VCAGetDevNum();
                    }
                    catch (Exception ex)
                    {
                        channel = 0;
                    }
                    break;
            }
            if (channel > 0)
            {
                this.channelHandle.Clear();
                this.VideoType = videotype;
            }
            return channel;
        }

        /// <summary>
        /// 初始化环境
        /// </summary>
        /// <param name="hdlwin">初始化当前窗口</param>
        /// <param name="videotype">视频卡类别</param>
        /// <returns>返回视频卡通道数量</returns>
        public int InitVideo(IntPtr hdlwin)
        {
            int channel = 0;
            switch (VideoType)
            {
                case VideoType.MvAPI:
                    try
                    {
                        channel = (int)MvAPI.MV_GetDeviceNumber();
                    }
                    catch (Exception ex)
                    {
                        channel = 0;
                    }
                    break;
                case VideoType.Sa7134Capture:
                    try
                    {
                        if (Sa7134Capture.VCAInitSdk(hdlwin, DISPLAYTRANSTYPE.PCI_MEMORY_VIDEOMEMORY, false))
                            channel = Sa7134Capture.VCAGetDevNum();
                    }
                    catch (Exception ex)
                    {
                        channel = 0;
                    }
                    break;
            }
            if (channel > 0)
            {
                this.channelHandle.Clear();
            }
            return channel;
        }
        
        /// <summary>
        /// 打开视频通道,已经打开则关闭后重新打开
        /// </summary>
        /// <param name="channel">视频通道号</param>
        /// <param name="hdlpreview">预览控件句柄</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool OpenChannel(int channel, IntPtr hdlpreview)
        {
            bool success = true;
            int hdl = channel;
            if (this.channelHandle.ContainsKey(channel))
                this.CloseChannel(channel);
            switch (this.VideoType)
            {
                case VideoType.MvAPI:
                    try
                    {
                        hdl = MvAPI.MV_OpenDevice((uint)channel, false);
                    }
                    catch (Exception ex)
                    {
                        hdl = 0;
                    }
                    if (hdl < 1) return false;
                    try
                    {
                        success = MvAPI.MV_SetDeviceParameter(hdl, (int)MV_PARAMTER.DISP_WHND, (uint)hdlpreview);
                        success = success && MvAPI.MV_SetDeviceParameter(hdl, (int)MV_PARAMTER.DISP_WIDTH, 336);
                        success = success && MvAPI.MV_SetDeviceParameter(hdl, (int)MV_PARAMTER.DISP_HEIGHT, 256);
                        success = success && 1 == MvAPI.MV_OperateDevice(hdl, 1);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                    break;
                case VideoType.Sa7134Capture:
                    try
                    {
                        success = Sa7134Capture.VCAOpenDevice(channel, hdlpreview);
                        success = success && Sa7134Capture.VCAStartVideoPreview(channel);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                    break;
            }
            if (success)
                this.channelHandle.Add(channel, hdl);
            return success;
        }

        /// <summary>
        /// 打开视频通道,已经打开则关闭后重新打开
        /// </summary>
        /// <param name="channel">视频通道号</param>
        /// <param name="hdlpreview">预览控件句柄</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool OpenChannel(int channel, Panel hdlpreview)
        {
            bool success = true;
            int hdl = channel;
            if (this.channelHandle.ContainsKey(channel))
                this.CloseChannel(channel);
            switch (this.VideoType)
            {
                case VideoType.MvAPI:
                    try
                    {
                        hdl = MvAPI.MV_OpenDevice((uint)channel, false);
                    }
                    catch (Exception ex)
                    {
                        hdl = 0;
                    }
                    if (hdl < 1) return false;
                    try
                    {
                        success = MvAPI.MV_SetDeviceParameter(hdl, (int)MV_PARAMTER.DISP_WHND, (uint)hdlpreview.Handle);
                        success = success && MvAPI.MV_SetDeviceParameter(hdl, hdlpreview.Width, 336);
                        success = success && MvAPI.MV_SetDeviceParameter(hdl, hdlpreview.Height, 256);
                        success = success && 1 == MvAPI.MV_OperateDevice(hdl, (int)RUNOPER.MVRUN);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                    break;
                case VideoType.Sa7134Capture:
                    try
                    {
                        success = Sa7134Capture.VCAOpenDevice(channel, hdlpreview.Handle);
                        success = success && Sa7134Capture.VCAStartVideoPreview(channel);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                    break;
            }
            if (success)
                this.channelHandle.Add(channel, hdl);
            return success;
        }

        /// <summary>
        /// 停止视频，,已经打开则关闭
        /// </summary>
        /// <param name="channel">视频通道号</param>
        /// <param name="hdlpreview">预览控件句柄</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool StopChannel(int channel, IntPtr hdlpreview)
        {
            bool success = true;
            int hdl = channel;
            if (!this.channelHandle.ContainsKey(channel))
                return false;
            switch (this.VideoType)
            {
                case VideoType.MvAPI:
                    this.CloseChannel(channel);
                    return success == true;
                case VideoType.Sa7134Capture:
                    try
                    {
                        success = Sa7134Capture.VCAStopVideoPreview(this.channelHandle[channel]);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                    break;
            }
            return success;
        }

        /// <summary>
        /// 开启视频，,已经打开则关闭重新开启
        /// </summary>
        /// <param name="channel">视频通道号</param>
        /// <param name="hdlpreview">预览控件句柄</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool StartChannel(int channel, Panel hdlpreview)
        {
            bool success = true;
            int hdl = channel;
            if (!this.channelHandle.ContainsKey(channel))
                return false;
            switch (this.VideoType)
            {
                case VideoType.MvAPI:
                    return this.OpenChannel(this.channelHandle[channel], hdlpreview.Handle);
                case VideoType.Sa7134Capture:
                    try
                    {
                        success = Sa7134Capture.VCAStartVideoPreview(this.channelHandle[channel]);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                    break;
            }
            return success;
        }

        /// <summary>
        /// 更新视频图像
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="hdlpreview">预览控件句柄</param>
        /// <returns>true 成功，false 失败</returns>
        public bool UpdateChannel(int channel, Panel hdlpreview)
        {
            bool success = true;
            switch (this.VideoType)
            {
                case VideoType.MvAPI:
                    InitVideo(hdlpreview.Handle, this.videoType);
                    return this.OpenChannel(channel, hdlpreview);
                case VideoType.Sa7134Capture:
                    if (!this.channelHandle.ContainsKey(channel))
                        return false;
                    try
                    {
                        success = Sa7134Capture.VCAUpdateVideoPreview(this.channelHandle[channel], hdlpreview.Handle);
                    }
                    catch (Exception ex)
                    {
                        success = false;
                    }
                    break;
            }
            return success;
        }

        /// <summary>
        /// 关闭视频通道
        /// </summary>
        /// <param name="channel">视频通道号</param>
        public void CloseChannel(int channel)
        {
            if (!this.channelHandle.ContainsKey(channel))
                return;
            switch (this.VideoType)
            {
                case VideoType.MvAPI:
                    try
                    {
                        MvAPI.MV_CloseDevice(this.channelHandle[channel]);
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                    break;
                case VideoType.Sa7134Capture:
                    try
                    {
                        Sa7134Capture.VCAStopVideoPreview(this.channelHandle[channel]);
                        Sa7134Capture.VCACloseDevice(this.channelHandle[channel]);
                    }
                    catch (Exception ex)
                    {
                        return;
                    }
                    break;
            }
            this.channelHandle.Remove(channel);
        }
        
        /// <summary>
        /// 关闭所有视频通道
        /// </summary>
        public void CloseVideo()
        {
            int[] channels = new int[this.channelHandle.Count];
            int i=0;
            foreach (int ch in this.channelHandle.Keys)
                channels[i++] = ch;
            for (i = 0; i < channels.Length; i++)
                this.CloseChannel(channels[i]);
        }

        /// <summary>
        /// 抓拍图片,保存文件
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="file">文件全路径</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool SaveCaptureFile(int channel,string file)
        {
            if (!this.channelHandle.ContainsKey(channel))
                return false;
            string path = Path.GetDirectoryName(file);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            bool success = true;
            switch (this.VideoType)
            {
                case VideoType.MvAPI:
                    MV_IMAGEINFO bufinfo = new MV_IMAGEINFO();
                    try
                    {
                         MvAPI.MV_GetDeviceNumber();
                         MvAPI.MV_OpenDevice((uint)channel, true);
                        IntPtr buf = MvAPI.MV_CaptureSingle(channel, false, 0, 0, ref bufinfo);
                        success = MvAPI.MV_SaveFile(file, 2, buf, ref bufinfo, 1, false, true, 100, false);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    break;
                case VideoType.Sa7134Capture:
                    try
                    {
                       success = Sa7134Capture.VCASaveAsJpegFile(this.channelHandle[channel], file, 100);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    break;
            }
            return success;
        }

        /// <summary>
        /// 抓拍图片,保存文件
        /// </summary>
        /// <param name="channel">通道号</param>
        /// <param name="file">文件全路径</param>
        /// <returns>成功返回true,失败返回false</returns>
        public bool TempSaveCaptureFile(int channel, string file)
        {
            string path = Path.GetDirectoryName(file);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            bool success = true;
            switch (this.VideoType)
            {
                case VideoType.MvAPI:
                    MV_IMAGEINFO bufinfo = new MV_IMAGEINFO();
                    try
                    {
                        MvAPI.MV_OpenDevice((uint)channel, true);
                        IntPtr buf = MvAPI.MV_CaptureSingle(channel, false, 0, 0, ref bufinfo);
                        success = MvAPI.MV_SaveFile(file, 2, buf, ref bufinfo, 1, false, true, 100, false);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    break;
                case VideoType.Sa7134Capture:
                    try
                    {
                        success = Sa7134Capture.VCASaveAsJpegFile(this.channelHandle[channel], file, 100);
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }
                    break;
            }
            return success;
        }
    }

    /// <summary>
    /// 视频卡类型
    /// </summary>
    public enum VideoType
    {
        /// <summary>
        /// 微视视频卡
        /// </summary>
        MvAPI,
         /// <summary>
        /// 天敏视频卡
        /// </summary>
        Sa7134Capture
    }
}
