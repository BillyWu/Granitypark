#region 版本说明

/*
 * 功能内容：   通讯协议规约,确定了帧头帧尾和通讯指令交互键值索引号(通常是站址)
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-06-19
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace Granity.communications
{
    /// <summary>
    /// 通讯协议规约定义
    /// </summary>
    public class Protocol
    {
        /// <summary>
        /// 帧头字节
        /// </summary>
        public byte[] FrameHeader = new byte[0];
        /// <summary>
        /// 帧尾字节
        /// </summary>
        public byte[] FrameFoot = new byte[0];
        /// <summary>
        /// 键值索引号(通常是站址,默认-1,以帧头字节后取值)
        /// </summary>
        public int KeyIndexStart = -1;
        /// <summary>
        /// 键值字节长度,默认一个字节,可以为0
        /// </summary>
        public int KeyLength = 1;
        /// <summary>
        /// 固定长度的协议,字节总长度,默认是变长,该值为-1
        /// </summary>
        public int TotalBytes = -1;
        /// <summary>
        /// 帧内数据长度索引号(默认-1,表示协议无该项内容)
        /// </summary>
        public int LenIndexStart = -1;
        /// <summary>
        /// 帧内数据长度字节所占位数,默认一个字节,可以是0
        /// </summary>
        public int LenLength = 1;
        /// <summary>
        /// 编码字节数,数据内容每字占用字节,计算数据长度时倍乘系数
        /// </summary>
        public int EncodingByte = 1;
        /// <summary>
        /// 帧内数据长度字节,读取或设置是否16进制数据
        /// </summary>
        public bool IsLenHEX = true;
        /// <summary>
        /// 读取或设置是否高低位交换
        /// </summary>
        public bool IsLenChangeHL = false;
        
        private bool isLenByte = false;
        /// <summary>
        /// 帧内数据长度字节,读取或设置是否字节,否时以编码方式处理
        /// </summary>
        public bool IsLenByte
        {
            get { return isLenByte; }
            set { isLenByte = value; if (value) IsLenHEX = true; }
        }
        private Encoding lenEncoding = Encoding.ASCII;
        /// <summary>
        /// 帧内数据长度字节,读取或设置字节所属编码类别,默认ASCII编码
        /// </summary>
        public Encoding LenEncoding
        {
            get { return lenEncoding; }
            set { lenEncoding = value; isLenByte = false; }
        }
        
        /// <summary>
        /// 协议指令在执行时的执行序列
        /// </summary>
        public SequenceType ExecuteSequence = SequenceType.Parallel;
        /// <summary>
        /// 一帧分多次接收数据时，连接完整帧加入目标字节数组列表中,不完整帧放入临时上下文字节数组
        /// 合并帧数据，加入列表数据，以供业务应用。
        /// </summary>
        public HdlMergeList MergeListHandle;

        private static Protocol ptlcard = new Protocol();
        /// <summary>
        /// 读取发行器通讯协议帧头,帧尾,数据长度及键值位置
        /// </summary>
        public static Protocol PTLCard
        {
            get
            {
                if (ptlcard.FrameHeader.Length < 1)
                {
                    ptlcard.FrameHeader = new byte[] { 2 };
                    ptlcard.FrameFoot = new byte[] { 3 };
                    ptlcard.KeyIndexStart = 1;
                    ptlcard.KeyLength = 2;

                    ////帧长度是ASCII编码,16进制数据,高低位不换位
                    //ptlcard.LenEncoding = Encoding.ASCII;
                    //ptlcard.EncodingByte = 2;
                    //ptlcard.IsLenHEX = true;
                    //ptlcard.IsLenChangeHL = false;
                    ////帧头,站址,功能码,长度
                    //ptlcard.LenIndexStart = 5;
                    //ptlcard.LenLength = 4;
                }
                return ptlcard;
            }
        }

        private static Protocol ptlpark = new Protocol();
        /// <summary>
        /// 读取停车场通讯协议帧头帧尾及键值位置
        /// </summary>
        public static Protocol PTLPark
        {
            get
            {
                if (ptlpark.FrameHeader.Length < 1)
                {
                    ptlpark.FrameHeader = new byte[] { 2 };
                    ptlpark.FrameFoot = new byte[] { 3 };
                    ptlpark.KeyIndexStart = 1;
                    ptlpark.KeyLength = 2;

                    ////帧长度是ASCII编码,16进制数据,高低位不换位
                    //ptlpark.LenEncoding = Encoding.ASCII;
                    //ptlpark.EncodingByte = 2;
                    //ptlpark.IsLenHEX = true;
                    //ptlpark.IsLenChangeHL = false;
                    ////帧头,站址,功能码,长度
                    //ptlpark.LenIndexStart = 5;
                    //ptlpark.LenLength = 4;
                }
                return ptlpark;
            }
        }
        
        private static Protocol ptldoor = new Protocol();
        /// <summary>
        /// 读取门禁通讯协议帧头帧尾及键值位置
        /// </summary>
        public static Protocol PTLDoor
        {
            get
            {
                if (ptldoor.FrameHeader.Length < 1)
                {
                    ptldoor.FrameHeader = new byte[] { 0x7E };
                    ptldoor.FrameFoot = new byte[] { 0x0D };
                    ptldoor.KeyIndexStart = 1;
                    ptldoor.KeyLength = 2;
                    //协议定长
                    ptldoor.TotalBytes = 34;
                }
                return ptldoor;
            }
        }

        private static Protocol ptleatery = new Protocol();
        /// <summary>
        /// 读取消费通讯协议帧头帧尾及键值位置
        /// </summary>
        public static Protocol PTLEatery
        {
            get
            {
                if (ptleatery.FrameHeader.Length < 1)
                {
                    ptleatery.FrameHeader = new byte[] { 0xC0 };
                    ptleatery.FrameFoot = new byte[] { 0xC0 };
                    ptleatery.KeyIndexStart = 1;
                    ptleatery.KeyLength = 1;

                    ////帧长度是ASCII编码,16进制数据,高低位不换位
                    //ptleatery.IsLenByte = true;
                    //ptleatery.IsLenChangeHL = false;
                    ////帧头,站址,功能码,长度
                    //ptleatery.LenIndexStart = 3;
                    //ptleatery.LenLength = 1;
                }
                return ptleatery;
            }
        }

    }

    /// <summary>
    /// 通讯指令执行序列类别：并行和串行
    /// </summary>
    public enum SequenceType
    {
        /// <summary>
        /// 指令以并行方式执行
        /// </summary>
        Parallel,
        /// <summary>
        /// 指令以串行方式执行
        /// </summary>
        Serial
    }
}
