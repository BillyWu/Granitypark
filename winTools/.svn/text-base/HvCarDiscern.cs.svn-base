#region 版本说明

/*
 * 功能内容：   车牌硬识别
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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Granity.communications;

namespace Granity.winTools
{
    /// <summary>
    /// 车牌硬识别
    /// </summary>
    public class HvCarDiscern
    {
        /// <summary>
        /// 识别结果
        /// </summary>
        private class HvResult
        {
            /// <summary>
            /// 当前结果所属的HV句柄
            /// </summary>
            public IntPtr m_Hv = IntPtr.Zero;

            /// <summary>
            /// 识别器车序号
            /// </summary>
            public uint CarID;
            /// <summary>
            /// 当前时间
            /// </summary>
            public string strTime;
            /// <summary>
            /// 车牌附加信息
            /// </summary>
            public string info;
            /// <summary>
            /// 车牌颜色
            /// </summary>
            public string strcolor;
            /// <summary>
            /// 车牌信息
            /// </summary>
            public string strplateno;

            /// <summary>
            /// 图像字节
            /// </summary>
            public byte[] BigImage = new byte[0];
            /// <summary>
            /// 小图字节
            /// </summary>
            public byte[] SmallImage = new byte[0];
            /// <summary>
            /// 视频图像
            /// </summary>
            public byte[] VideoImage = new byte[0];
            /// <summary>
            /// 视频图像尺寸
            /// </summary>
            public int VideoSize = 0;
            /// <summary>
            /// 视频回送的时间
            /// </summary>
            public DateTime dtVideo = DateTime.Now;
            /// <summary>
            /// 视频字节包装成为视频图像流
            /// </summary>
            public MemoryStream streamVideo = new MemoryStream();
        }

        #region 定义回调函数

        //定义车牌号/小图/大图/视频回调函数
        HVDLLFun.PLATE_NO_CALLBACK plateCallback;
        HVDLLFun.SMALL_IMAGE_CALLBACK sImgCallback;
        HVDLLFun.BIG_IMAGE_CALLBACK bImgCallbak;
        HVDLLFun.VIDEO_CALLBACK videoCallback;
        
        #endregion

        #region 回调函数

        /// <summary>
        /// 车牌号回调
        /// </summary>
        /// <param name="pFirstParameter">第一个传递参数</param>
        /// <param name="dwCarID">车号</param>
        /// <param name="pcPlateNo">车牌号</param>
        /// <param name="dwTimeMs">时间</param>
        /// <returns></returns>
        private long onPlateNoCallBack(IntPtr pFirstParameter, uint dwCarID, string pcPlateNo, ulong dwTimeMs)
        {
            HvResult hvresult = Marshal.GetObjectForIUnknown(pFirstParameter) as HvResult;
            if (null == hvresult)
                return -1;

            string strplateno, strcolor;
            int length = pcPlateNo.Length;
            strcolor = pcPlateNo.Substring(0, 1);
            strplateno = pcPlateNo.Substring(1, length - 1);
            //时间的转化
            long time_C_Long = (long)dwTimeMs;          		 	   //C++长整型日期，毫秒为单位
            DateTime dt_1970 = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            long tricks_1970 = dt_1970.Ticks;                   		 	   //1970年1月1日刻度
            long time_tricks = tricks_1970 + time_C_Long * 10000;   				  //日志日期刻度
            DateTime dt = new DateTime(time_tricks);    				//转化为DateTime(世界时间)

            DateTime dt2 = dt.ToLocalTime();      		//将世界时间转化为当前时间（本时区的时间）
            hvresult.strTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", dt2);
            byte[] plateinfo = new byte[256];
            unsafe
            {
                byte* pszInfo = null;
                HVDLLFun.HV_GetPlateInfo(hvresult.m_Hv, &pszInfo); 					//提取附加的信息
                Marshal.Copy((IntPtr)pszInfo, plateinfo, 0, 256);
            }
            hvresult.CarID = dwCarID;
            hvresult.info = Encoding.Default.GetString(plateinfo);
            hvresult.strcolor = strcolor;
            hvresult.strplateno = strplateno;
            return 0;
        }

        /// <summary>
        /// 小图回调
        /// </summary>
        /// <param name="pFirstParameter"></param>
        /// <param name="dwCarID"></param>
        /// <param name="wImageWidth"></param>
        /// <param name="wImageHigh"></param>
        /// <param name="bType"></param>
        /// <param name="wSize"></param>
        /// <param name="pbImage"></param>
        /// <param name="dwTimeMs"></param>
        /// <returns></returns>
        private unsafe long onSmallImgCallBack(IntPtr pFirstParameter, uint dwCarID, ushort wImageWidth, ushort wImageHigh, byte bType,
                            ushort wSize, byte* pbImage, ulong dwTimeMs)
        {
            HvResult hvresult = Marshal.GetObjectForIUnknown(pFirstParameter) as HvResult;
            if (null == hvresult) return -1;

            ////设置缓存长度
            int iRealLen = 0;
            int iBuffLen = (int)wImageWidth * (int)wImageHigh * 3 + 1024;
            hvresult.SmallImage = new byte[iBuffLen];
            ////m_Redult为存储结果流的类
            long state = 0;
            fixed (byte* pDest = hvresult.SmallImage)
            {
                state = HVDLLFun.Yuv2BMP(pDest, iBuffLen, ref iRealLen, pbImage, wImageWidth, wImageHigh);
                Array.Resize<byte>(ref hvresult.SmallImage, iRealLen);
            }
            return 0;
        }

        /// <summary>
        /// 大图回传
        /// </summary>
        /// <param name="pFirstParameter">第一个传递参数</param>
        /// <param name="dwCarID">车号</param>
        /// <param name="wImageWidth">图像尺寸宽度</param>
        /// <param name="wImageHigh">图像尺寸高度</param>
        /// <param name="bType">图像类别</param>
        /// <param name="wSize">数据大小</param>
        /// <param name="pbImage">图像数据的指针</param>
        /// <param name="wImageID">图像的ID号，表示得到的是哪一张图片</param>
        /// <param name="wHighImgFlag">高清图片标志</param>
        /// <param name="wPlateWidth">表示该图象中检测到的车牌矩形左上坐标</param>
        /// <param name="wPlateHigh">表示该图象中检测到的车牌矩形右下坐标</param>
        /// <param name="dwTimeMs">时间</param>
        /// <returns></returns>
        private unsafe long onBigImgCallBack(IntPtr pFirstParameter, uint dwCarID, ushort wImageWidth, ushort wImageHigh, byte bType, ushort wSize, byte* pbImage,
                    ushort wImageID, ushort wHighImgFlag, ushort wPlateWidth, ushort wPlateHigh, ulong dwTimeMs)
        {
            HvResult hvresult = Marshal.GetObjectForIUnknown(pFirstParameter) as HvResult;
            if (null == hvresult) return -1;

            int iSize = wSize;
            if (iSize < 1) return -1;
            //wHighImgFlag的值跟FF00按位与操作，如果高8位是FF，&结果不为0，则应是高清图片
            ////取wHighImgFlag低8位，并左移16位
            if ((wHighImgFlag & 0xFF00) != 0)
                iSize = wSize + (int)((wHighImgFlag & 0x00FF) << 16);

            hvresult.CarID = dwCarID;
            //拷贝图片数据
            hvresult.BigImage = new byte[iSize];
            Marshal.Copy((IntPtr)pbImage, hvresult.BigImage, 0, iSize);
            return 0;
        }

        /// <summary>
        /// 实时视频回调
        /// </summary>
        /// <param name="pFirstParameter"></param>
        /// <param name="wVideoID"></param>
        /// <param name="wSize"></param>
        /// <param name="pbImage"></param>
        /// <returns></returns>
        private static unsafe long onVideoCallBack(IntPtr pFirstParameter, UInt16 wVideoID, UInt16 wSize, byte* pbImage)
        {
            HvResult hvresult = Marshal.GetObjectForIUnknown(pFirstParameter) as HvResult;
            if (null == hvresult) return -1;

            int iSize = wSize;
            if (iSize < 1) return -1;
            if (iSize > hvresult.VideoImage.Length)
                Array.Resize<byte>(ref hvresult.VideoImage, iSize);
            //分配图像字节,该句注释再测试节省内存空间
            hvresult.VideoImage = new byte[iSize];
            Marshal.Copy((IntPtr)pbImage, hvresult.VideoImage, 0, iSize);
            hvresult.VideoSize = iSize;
            hvresult.dtVideo = DateTime.Now;
            return 0;
        }
        /// <summary>
        /// 检查是否连接,断开时自动重联
        /// </summary>
        private void checkedOpened()
        {
            while (IntPtr.Zero != this.m_Hv)
            {
                Int32 piStatus = -1;
                HVDLLFun.HvIsConnected(this.m_Hv, ref piStatus);
                if (0 != piStatus && IntPtr.Zero != this.m_Hv)
                {
                    IntPtr hdl = this.m_Hv;
                    this.m_Hv = IntPtr.Zero;
                    this.Open(this.ipaddr);
                    if (IntPtr.Zero == this.m_Hv)
                        this.m_Hv = hdl;
                }
                this.whCheckOpened.WaitOne(10000,false);
            }
        }
        #endregion

        
        private HvResult resultFirst = new HvResult();
        private HvResult resultSecond = new HvResult();
        private IntPtr m_Hv = IntPtr.Zero;
        /// <summary>
        /// 可同步处理检查连接打开状态
        /// </summary>
        private AutoResetEvent whCheckOpened = new AutoResetEvent(false);
        
        private string ipaddr;
        /// <summary>
        /// 读取当前识别器IP地址
        /// </summary>
        public string IpAddr
        {
            get { return ipaddr; }
        }

        /// <summary>
        /// 打开指定IP地址的识别器
        /// </summary>
        /// <param name="ip">识别器IP地址</param>
        /// <returns></returns>
        public string Open(string ip)
        {
            //已经打开直接返回,关闭时同步退出打开状态检查
            if (IntPtr.Zero != this.m_Hv && this.ipaddr == ip)
                return "";
            this.Close();
            this.m_Hv = HVDLLFun.OpenHv(ip);
            if (m_Hv == IntPtr.Zero)
            {
                this.m_Hv = IntPtr.Zero;
                this.ipaddr = "";
                resultFirst.m_Hv = resultSecond.m_Hv = IntPtr.Zero;
                return "连接设备失败！";
            }
            this.ipaddr = ip;
            resultFirst.m_Hv = resultSecond.m_Hv = this.m_Hv;
            //创建参数
            //第一通道参数
            IntPtr argfirst = Marshal.GetIUnknownForObject(this.resultFirst);
            //第二通道参数
            IntPtr argsecond = Marshal.GetIUnknownForObject(this.resultSecond);

            //车牌号回调
            plateCallback = new HVDLLFun.PLATE_NO_CALLBACK(onPlateNoCallBack);
            IntPtr ptfunPlt = Marshal.GetFunctionPointerForDelegate(plateCallback);
            UInt16 streamtype = Convert.ToUInt16(HvStreamType.PlateStr);
            int f = HVDLLFun.SetHvCallBack(this.m_Hv, ptfunPlt, argfirst, 0, streamtype);
            if (f > -1)
                f = HVDLLFun.SetHvCallBack(this.m_Hv, ptfunPlt, argsecond, 1, streamtype);
            if (f > -1)
            {
                //车牌小图回调
                unsafe
                {
                    sImgCallback = new HVDLLFun.SMALL_IMAGE_CALLBACK(onSmallImgCallBack);
                    IntPtr ptfunImgsmall = Marshal.GetFunctionPointerForDelegate(sImgCallback);
                    streamtype = Convert.ToUInt16(HvStreamType.SmallImage);
                    f = HVDLLFun.SetHvCallBack(this.m_Hv, ptfunImgsmall, argfirst, 0, streamtype);
                    if(f>-1)
                        f = HVDLLFun.SetHvCallBack(this.m_Hv, ptfunImgsmall, argsecond, 1, streamtype);
                }
            }
            if (f > -1)
            {
                //大图回调
                unsafe
                {
                    bImgCallbak = new HVDLLFun.BIG_IMAGE_CALLBACK(onBigImgCallBack);
                    IntPtr ptfunImgbig = Marshal.GetFunctionPointerForDelegate(bImgCallbak);
                    streamtype = Convert.ToUInt16(HvStreamType.BigImage);
                    f = HVDLLFun.SetHvCallBack(this.m_Hv, ptfunImgbig, argfirst, 0, streamtype);
                    if(f>-1)
                        f = HVDLLFun.SetHvCallBack(this.m_Hv, ptfunImgbig, argsecond, 1, streamtype);
                }
            }
            if (f > -1)
            {
                //视频回调
                unsafe
                {
                    videoCallback = new HVDLLFun.VIDEO_CALLBACK(onVideoCallBack);
                    IntPtr ptfunVideo = Marshal.GetFunctionPointerForDelegate(videoCallback);
                    streamtype = Convert.ToUInt16(HvStreamType.FullCmpImage);
                    f = HVDLLFun.SetHvCallBack(this.m_Hv, ptfunVideo, argfirst, 0, streamtype);
                    if(f>-1)
                        f = HVDLLFun.SetHvCallBack(this.m_Hv, ptfunVideo, argsecond, 1, streamtype);
                }
            }
            if (f < 0)
            {
                this.Close();
                return "回调设置失败！";
            }
            ThreadManager.QueueUserWorkItem(delegate(object obj) { this.checkedOpened(); }, null);
            return "";
        }

        /// <summary>
        /// 关闭当前识别器
        /// </summary>
        /// <returns></returns>
        public string Close()
        {
            if (IntPtr.Zero != this.m_Hv)
                HVDLLFun.CloseHv(this.m_Hv);
            this.m_Hv = IntPtr.Zero;
            this.ipaddr = "";
            this.resultFirst.m_Hv = this.resultSecond.m_Hv = IntPtr.Zero;
            this.whCheckOpened.Set();
            return "";
        }

        /// <summary>
        /// 读取视频图像流
        /// </summary>
        /// <param name="iVideo">视频通道号(0,1)</param>
        /// <returns></returns>
        public MemoryStream getStreamVideo(int iVideo)
        {
            if (IntPtr.Zero == this.m_Hv)
                return null;
            HvResult hvresult = this.resultFirst;
            if (0 != iVideo)
                hvresult = this.resultSecond;
            if (hvresult.VideoSize < 1 || hvresult.VideoImage.Length < 1)
                return null;
            try
            {
                hvresult.streamVideo.SetLength(hvresult.VideoSize);
                hvresult.streamVideo.Seek(0, SeekOrigin.Begin);
                hvresult.streamVideo.Write(hvresult.VideoImage, 0, hvresult.VideoSize);
            }catch { }
            return hvresult.streamVideo;
        }

        /// <summary>
        /// 获取视频通道当前识别信息
        /// </summary>
        /// <param name="iVideo">视频通道号(0,1)</param>
        /// <returns></returns>
        public HvCarPlateInfo getCarPlateInfo(int iVideo)
        {
            HvCarPlateInfo info = new HvCarPlateInfo();
            if (IntPtr.Zero == this.m_Hv)
                return info;
            HvResult hvresult = this.resultFirst;
            if (0 != iVideo)
                hvresult = this.resultSecond;

            info.CarNum = hvresult.strplateno;
            info.BigImage = hvresult.BigImage;
            info.SmallImage = hvresult.SmallImage;

            return info;
        }
        /// <summary>
        /// 获取视频信息
        /// </summary>
        /// <param name="iVideo">视频通道号</param>
        /// <returns></returns>
        public HvVideoInfo getVideoInfo(int iVideo)
        {
            HvVideoInfo info = new HvVideoInfo();
            if (IntPtr.Zero == this.m_Hv)
                return info;
            HvResult hvresult = this.resultFirst;
            if (0 != iVideo)
                hvresult = this.resultSecond;
            info.DtVideo = hvresult.dtVideo;
            info.VideoSize = hvresult.VideoSize;
            info.VideoImage = hvresult.VideoImage;
            return info;
        }

        /// <summary>
        /// 重置识别器结果信息
        /// <param name="iVideo">视频通道号</param>
        /// </summary>
        public void ResetInfo(int iVideo)
        {
            if (IntPtr.Zero == this.m_Hv)
                return;
            HvResult hvresult = this.resultFirst;
            if (0 != iVideo)
                hvresult = this.resultSecond;

            hvresult.info = "";
            hvresult.strcolor = "";
            hvresult.strplateno = "";
            hvresult.strTime = "";
            hvresult.BigImage = new byte[0];
            hvresult.SmallImage = new byte[0];
        }

    }

    /// <summary>
    /// 车牌信息
    /// </summary>
    public class HvCarPlateInfo
    {
        /// <summary>
        /// 车辆牌号
        /// </summary>
        public string CarNum;
        /// <summary>
        /// 车牌抓拍大图
        /// </summary>
        public byte[] BigImage = new byte[0];
        /// <summary>
        /// 车牌抓拍小图
        /// </summary>
        public byte[] SmallImage = new byte[0];
    }

    /// <summary>
    /// 识别器视频信息
    /// </summary>
    public class HvVideoInfo
    {
        /// <summary>
        /// 视频回传最后时间
        /// </summary>
        public DateTime DtVideo;
        /// <summary>
        /// 视频图像大小
        /// </summary>
        public int VideoSize;
        /// <summary>
        /// 视频流字节
        /// </summary>
        public byte[] VideoImage = new byte[0];
    }

}
