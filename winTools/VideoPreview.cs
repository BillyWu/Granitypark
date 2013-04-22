using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace Granity.winTools
{
    /// <summary>
    /// ��ƵԤ��
    /// </summary>
    public class VideoPreview
    {
        /// <summary>
        /// ��Ƶ�����
        /// </summary>
        private VideoType VideoType;

        public VideoType videoType
        {
            get { return VideoType; }
            set { VideoType = value; }
        }
        /// <summary>
        /// ͨ�����
        /// </summary>
        private Dictionary<int, int> channelHandle = new Dictionary<int, int>();

        /// <summary>
        /// ��ʼ������
        /// </summary>
        /// <param name="hdlwin">��ʼ����ǰ����</param>
        /// <param name="videotype">��Ƶ�����</param>
        /// <returns>������Ƶ��ͨ������</returns>
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
        /// ��ʼ������
        /// </summary>
        /// <param name="hdlwin">��ʼ����ǰ����</param>
        /// <param name="videotype">��Ƶ�����</param>
        /// <returns>������Ƶ��ͨ������</returns>
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
        /// ����Ƶͨ��,�Ѿ�����رպ����´�
        /// </summary>
        /// <param name="channel">��Ƶͨ����</param>
        /// <param name="hdlpreview">Ԥ���ؼ����</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
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
        /// ����Ƶͨ��,�Ѿ�����رպ����´�
        /// </summary>
        /// <param name="channel">��Ƶͨ����</param>
        /// <param name="hdlpreview">Ԥ���ؼ����</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
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
        /// ֹͣ��Ƶ��,�Ѿ�����ر�
        /// </summary>
        /// <param name="channel">��Ƶͨ����</param>
        /// <param name="hdlpreview">Ԥ���ؼ����</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
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
        /// ������Ƶ��,�Ѿ�����ر����¿���
        /// </summary>
        /// <param name="channel">��Ƶͨ����</param>
        /// <param name="hdlpreview">Ԥ���ؼ����</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
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
        /// ������Ƶͼ��
        /// </summary>
        /// <param name="channel">ͨ����</param>
        /// <param name="hdlpreview">Ԥ���ؼ����</param>
        /// <returns>true �ɹ���false ʧ��</returns>
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
        /// �ر���Ƶͨ��
        /// </summary>
        /// <param name="channel">��Ƶͨ����</param>
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
        /// �ر�������Ƶͨ��
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
        /// ץ��ͼƬ,�����ļ�
        /// </summary>
        /// <param name="channel">ͨ����</param>
        /// <param name="file">�ļ�ȫ·��</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
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
        /// ץ��ͼƬ,�����ļ�
        /// </summary>
        /// <param name="channel">ͨ����</param>
        /// <param name="file">�ļ�ȫ·��</param>
        /// <returns>�ɹ�����true,ʧ�ܷ���false</returns>
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
    /// ��Ƶ������
    /// </summary>
    public enum VideoType
    {
        /// <summary>
        /// ΢����Ƶ��
        /// </summary>
        MvAPI,
         /// <summary>
        /// ������Ƶ��
        /// </summary>
        Sa7134Capture
    }
}
