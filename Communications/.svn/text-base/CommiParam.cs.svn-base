#region 版本说明

/*
 * 功能内容：   通讯参数,对TCP/UDP/SerialPort通讯的参数定义
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-05-24
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO.Ports;

namespace Granity.communications
{
    /// <summary>
    /// 通讯参数
    /// </summary>
    public class CommiTarget
    {
        private CommiType protocolType = CommiType.UDP;
        /// <summary>
        /// 读取当前通讯的类型
        /// </summary>
        public CommiType ProtocolType
        {
            get { return protocolType; }
        }

        #region 构造函数

        /// <summary>
        /// 构建默认参数,协议参数为空,默认UDP通讯
        /// </summary>
        public CommiTarget()
        {

        }

        /// <summary>
        /// 构建通讯参数,并以UDP/TCP参数初始值
        /// </summary>
        /// <param name="ep">通讯另一端IP地址和端口号</param>
        /// <param name="ptype">通讯类别UDP/TCP,设置SerialPort触发异常</param>
        public CommiTarget(IPEndPoint ep, CommiType ptype)
        {
            SetProtocolParam(ep, ptype);
        }

        /// <summary>
        /// 构建通讯参数,并以UDP/TCP参数初始值
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="ptype">协议类型UDP/TCP,设置SerialPort触发异常</param>
        public CommiTarget(string ipaddress, int port, CommiType ptype)
        {
            SetProtocolParam(ipaddress, port, ptype);
        }

        /// <summary>
        /// 构建通讯参数,并以串口参数初始值
        /// </summary>
        /// <param name="portName">通讯串口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">奇偶校验</param>
        /// <param name="dataBits">数据位</param>
        /// <param name="stopBits">停止位</param>
        public CommiTarget(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            SetProtocolParam(portName, baudRate, parity, dataBits, stopBits);
        }

        /// <summary>
        /// 构建通讯参数,并以串口参数初始值
        /// </summary>
        /// <param name="portName">通讯串口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">奇偶校验</param>
        /// <param name="dataBits">数据位</param>
        public CommiTarget(string portName, int baudRate, Parity parity, int dataBits)
        {
            SetProtocolParam(portName, baudRate, parity, dataBits);
        }

        /// <summary>
        /// 构建通讯参数,并以串口参数初始值
        /// </summary>
        /// <param name="portName">通讯串口名称</param>
        /// <param name="baudRate">波特率</param>
        /// <param name="parity">奇偶校验</param>
        public CommiTarget(string portName, int baudRate, Parity parity)
        {
            SetProtocolParam(portName, baudRate, parity);
        }

        /// <summary>
        /// 构建通讯参数,并以串口参数初始值
        /// </summary>
        /// <param name="portName">通讯串口名称</param>
        /// <param name="baudRate">波特率</param>
        public CommiTarget(string portName, int baudRate)
        {
            SetProtocolParam(portName, baudRate);
        }

        /// <summary>
        /// 构建通讯参数,并以串口参数初始值
        /// </summary>
        /// <param name="portName">通讯串口名称</param>
        public CommiTarget(string portName)
        {
            SetProtocolParam(portName);
        }

        #endregion

        #region 设置通讯协议参数

        /// <summary>
        /// 设置TCP/UDP通讯端点,类型使用串口触发异常
        /// </summary>
        /// <param name="ep">通讯端点,包含地址和端口号</param>
        /// <param name="ptype">协议类型</param>
        public void SetProtocolParam(IPEndPoint ep, CommiType ptype)
        {
            if (CommiType.SerialPort == ptype)
                throw new Exception("串口通讯请设置串口名称和波特率等参数");
            if (null == ep)
                throw new Exception("通讯目标地址不能为空");
            if (ep.Port < 1024 || ep.Port > 65535)
                throw new Exception("目标端口号不在[1024～65535]范围内");
            this.srvEndPoint = ep;
            this.protocolType = ptype;
        }

        /// <summary>
        /// 设置TCP/UDP通讯端点,类型使用串口触发异常
        /// </summary>
        /// <param name="ipaddress">IP地址</param>
        /// <param name="port">端口号</param>
        /// <param name="ptype">协议类型</param>
        public void SetProtocolParam(string ipaddress, int port, CommiType ptype)
        {
            IPAddress ad = null;
            if (!IPAddress.TryParse(ipaddress, out ad))
                throw new Exception("目标地址不是有效IP地址：" + ipaddress);
            SetProtocolParam(new IPEndPoint(ad, port), ptype);
        }

        /// <summary>
        /// 设置串口通讯参数
        /// </summary>
        /// <param name="portName">串口名称</param>
        /// <param name="baudRate">波特率(默认9600)</param>
        /// <param name="parity">奇偶校验位(默认None)</param>
        /// <param name="dataBits">数据位(默认8)</param>
        /// <param name="stopBits">停止位(默认1)</param>
        public void SetProtocolParam(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            if (string.IsNullOrEmpty(portName))
                throw new Exception("串口端口不能为空");
            if (dataBits < 5 || dataBits > 8)
                throw new Exception("数据位范围是从 5 到 8");

            this.portName = portName;
            this.baudRate = baudRate;
            this.parity = parity;
            this.dataBits = dataBits;
            this.stopBits = stopBits;

            this.srvEndPoint = null;
            this.protocolType = CommiType.SerialPort;
        }

        /// <summary>
        /// 设置串口通讯参数
        /// </summary>
        /// <param name="portName">串口名称</param>
        /// <param name="baudRate">波特率(默认9600)</param>
        /// <param name="parity">奇偶校验位(默认None)</param>
        /// <param name="dataBits">数据位(默认8)</param>
        public void SetProtocolParam(string portName, int baudRate, Parity parity, int dataBits)
        {
            SetProtocolParam(portName, baudRate, parity, dataBits, StopBits.One);
        }
        /// <summary>
        /// 设置串口通讯参数
        /// </summary>
        /// <param name="portName">串口名称</param>
        /// <param name="baudRate">波特率(默认9600)</param>
        /// <param name="parity">奇偶校验位(默认None)</param>
        public void SetProtocolParam(string portName, int baudRate, Parity parity)
        {
            SetProtocolParam(portName, baudRate, parity, 8, StopBits.One);
        }

        /// <summary>
        /// 设置串口通讯参数
        /// </summary>
        /// <param name="portName">串口名称</param>
        /// <param name="baudRate">波特率(默认9600)</param>
        public void SetProtocolParam(string portName, int baudRate)
        {
            SetProtocolParam(portName, baudRate, Parity.None, 8, StopBits.One);
        }
        /// <summary>
        /// 设置串口通讯参数
        /// </summary>
        /// <param name="portName">串口名称</param>
        public void SetProtocolParam(string portName)
        {
            SetProtocolParam(portName, 9600, Parity.None, 8, StopBits.One);
        }

        #endregion

        /// <summary>
        /// 设置通讯协议规约
        /// </summary>
        /// <param name="frameH">帧头字节</param>
        /// <param name="frameF">帧尾字节</param>
        /// <param name="keyIndex">键值索引号</param>
        /// <param name="keyLen">键值字节长度</param>
        /// <param name="seq">执行序列</param>
        /// <returns>返回设置的协议</returns>
        public Protocol setProtocol(byte[] frameH, byte[] frameF, int keyIndex, int keyLen, SequenceType seq)
        {
            Protocol p = new Protocol();
            p.FrameHeader = frameH;
            p.FrameFoot = frameF;
            p.KeyIndexStart = keyIndex;
            p.KeyLength = keyLen;
            p.ExecuteSequence = seq;

            return this.ptl = p;
        }

        /// <summary>
        /// 设置通讯协议规约
        /// </summary>
        /// <param name="pt">协议规约</param>
        /// <returns>返回设置的协议</returns>
        public Protocol setProtocol(Protocol pt)
        {
            if (null == pt)
                return this.ptl = pt;
            //帧头帧尾属性
            Protocol p = new Protocol();
            p.FrameHeader = pt.FrameHeader;
            p.FrameFoot = pt.FrameFoot;
            p.KeyIndexStart = pt.KeyIndexStart;
            p.KeyLength = pt.KeyLength;
            p.ExecuteSequence = pt.ExecuteSequence;
            p.MergeListHandle = pt.MergeListHandle;
            //帧长度属性
            p.LenEncoding = pt.LenEncoding;
            p.IsLenByte = pt.IsLenByte;
            p.EncodingByte = pt.EncodingByte;
            p.IsLenChangeHL = pt.IsLenChangeHL;
            p.IsLenHEX = pt.IsLenHEX;
            p.LenIndexStart = pt.LenIndexStart;
            p.LenLength = pt.LenLength;
            //帧定长属性
            p.TotalBytes = pt.TotalBytes;
            return this.ptl = p;
        }

        /// <summary>
        /// 克隆通讯协议参数
        /// </summary>
        /// <returns>创建新的协议参数,参数值相同</returns>
        public CommiTarget Clone()
        {
            CommiTarget target = new CommiTarget();
            target.protocolType = this.protocolType;

            if (null != this.srvEndPoint)
                target.srvEndPoint = new IPEndPoint(this.srvEndPoint.Address, this.srvEndPoint.Port);

            target.portName = this.portName;
            target.baudRate = this.baudRate;
            target.parity = this.parity;
            target.dataBits = this.dataBits;
            target.stopBits = this.stopBits;
            target.setProtocol(this.ptl);
            return target;
        }

        #region 协议参数定义

        private IPEndPoint srvEndPoint = null;
        /// <summary>
        /// 读取服务端地址端口,使用串口协议时为null
        /// </summary>
        public IPEndPoint SrvEndPoint
        {
            get { return srvEndPoint; }
        }
        private string portName = "";
        /// <summary>
        /// 读取串口协议端口号
        /// </summary>
        public string PortName
        {
            get { return portName; }
        }
        private int baudRate = 9600;
        /// <summary>
        /// 读取串口协议波特率
        /// </summary>
        public int BaudRate
        {
            get { return baudRate; }
        }
        private Parity parity = Parity.None;
        /// <summary>
        /// 读取串口协议奇偶校验
        /// </summary>
        public Parity Parity
        {
            get { return parity; }
        }
        private int dataBits = 8;
        /// <summary>
        /// 读取串口协议数据位
        /// </summary>
        public int DataBits
        {
            get { return dataBits; }
        }
        private StopBits stopBits = StopBits.One;
        /// <summary>
        /// 读取串口协议停止位
        /// </summary>
        public StopBits StopBits
        {
            get { return stopBits; }
        }

        private Protocol ptl = new Protocol();
        /// <summary>
        /// 读取协议规约
        /// </summary>
        public Protocol Ptl
        {
            get { return ptl; }
        }

        #endregion
    }

    /// <summary>
    /// 通讯类别
    /// </summary>
    public enum CommiType
    {
        /// <summary>
        /// 传输控制协议
        /// </summary>
        TCP,
        /// <summary>
        /// 用户数据报协议
        /// </summary>
        UDP,
        /// <summary>
        /// 串口通讯协议
        /// </summary>
        SerialPort
    }
}
