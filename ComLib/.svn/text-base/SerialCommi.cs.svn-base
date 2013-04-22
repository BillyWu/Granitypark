#region 版本说明

/*
 * 功能内容：   简单的串口通讯<在新版本程序中去去掉>
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-05-23
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace ComLib
{
    /// <summary>
    /// 串口读取数据通讯类
    /// </summary>
    public class SerialCommi
    {
        private SerialPort _serialPort = new SerialPort();

        private int delay = 50;

        /// <summary>
        /// 读取或设置通讯时接收数据的延时时刻(单位ms,默认50)
        /// </summary>
        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        #region 协议参数定义

        private string portName = "";
        /// <summary>
        /// 读取串口端口名称
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

        #endregion


        #region 设置通讯参数

        /// <summary>
        /// 设置串口通讯参数
        /// </summary>
        /// <param name="portName">串口名称</param>
        /// <param name="baudRate">波特率(默认9600)</param>
        /// <param name="parity">奇偶校验位(默认None)</param>
        /// <param name="dataBits">数据位(默认8)</param>
        /// <param name="stopBits">停止位(默认1)</param>
        /// <param name="delay">延迟时间(ms),默认40ms</param>
        public void SetProtocolParam(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, int delay)
        {
            if (string.IsNullOrEmpty(portName))
                throw new Exception("串口端口不能为空");
            if (dataBits < 5 || dataBits > 8)
                throw new Exception("数据位范围是从 5 到 8");

            this.portName = this._serialPort.PortName = portName;
            this.baudRate = this._serialPort.BaudRate = baudRate;
            this.parity = this._serialPort.Parity = parity;
            this.dataBits = this._serialPort.DataBits = dataBits;
            this.stopBits = this._serialPort.StopBits = stopBits;
            this.delay = delay;
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
            SetProtocolParam(portName, baudRate, parity, dataBits, stopBits, this.delay);
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
        /// <param name="delay">延迟时间(ms),默认40ms</param>
        public void SetProtocolParam(string portName, int baudRate, int delay)
        {
            SetProtocolParam(portName, baudRate, Parity.None, 8, StopBits.One, delay);
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
        /// 串口通讯发送命令接收数据
        /// </summary>
        /// <param name="cmd">发出的请求字符串</param>
        /// <returns>返回接收到的字节数据</returns>
        public byte[] SendCommand(string cmd)
        {
            if (string.IsNullOrEmpty(this._serialPort.PortName) || string.IsNullOrEmpty(cmd))
                return new byte[0];

            bool isopen = this._serialPort.IsOpen;
            if (!isopen) this._serialPort.Open();
            this._serialPort.Write(cmd);
            Thread.Sleep(this.delay);
            byte[] buffer = new byte[0];
            List<byte[]> list = new List<byte[]>();
            Int64 count = 0;
            while (true)
            {
                buffer = new byte[this._serialPort.BytesToRead];
                this._serialPort.Read(buffer, 0, buffer.Length);
                if (buffer.Length <= 0)
                    break;
                list.Add(buffer);
                count += buffer.Length;
                Thread.Sleep(this.delay);
            }
            if (!isopen)
                this._serialPort.Close();
            if (1 == list.Count)
                return list[0];
            buffer = new byte[count];
            count = 0;
            foreach (byte[] b in list)
            {
                b.CopyTo(buffer, count);
                count += b.Length;
            }
            return buffer;
        }

        /// <summary>
        /// 串口通讯发送命令接收数据
        /// </summary>
        /// <param name="cmd">发出的请求字符串</param>
        /// <param name="isHEX">是否是16进制格式的字符串</param>
        /// <returns>返回接收到的字节数据</returns>
        public byte[] SendCommand(string cmd, bool isHEX)
        {
            if (string.IsNullOrEmpty(cmd))
                return new byte[0];
            if (!isHEX)
                return SendCommand(cmd);

            if (0 != cmd.Length % 2)
                cmd = "0" + cmd;
            cmd = cmd.ToUpper().Replace(" ", "");
            byte[] cmdb = new byte[cmd.Length / 2];
            for (int i = 0; i < cmd.Length / 2; i++)
                cmdb[i] = (byte)Convert.ToInt16(cmd.Substring(i * 2, 2), 16);
            return SendCommand(cmdb);
        }
        /// <summary>
        /// 串口通讯发送命令接收数据
        /// </summary>
        /// <param name="cmd">发出的请求字节数据</param>
        /// <returns>返回接收到的字节数据</returns>
        public byte[] SendCommand(byte[] cmd)
        {
            if (null == cmd || cmd.Length < 1)
                return new byte[0];

            bool isopen = this._serialPort.IsOpen;
            if (!isopen) this._serialPort.Open();

            this._serialPort.Write(cmd, 0, cmd.Length);
            Thread.Sleep(this.delay);
            byte[] buffer = new byte[0];
            List<byte[]> list = new List<byte[]>();
            Int64 count = 0;
            while (true)
            {
                buffer = new byte[this._serialPort.BytesToRead];
                this._serialPort.Read(buffer, 0, buffer.Length);
                if (buffer.Length <= 0)
                    break;
                list.Add(buffer);
                count += buffer.Length;
                Thread.Sleep(this.delay);
            }
            if (!isopen) this._serialPort.Close();

            if (1 == list.Count)
                return list[0];
            buffer = new byte[count];
            count = 0;
            foreach (byte[] b in list)
            {
                b.CopyTo(buffer, count);
                count += b.Length;
            }
            return buffer;
        }

    }
}
