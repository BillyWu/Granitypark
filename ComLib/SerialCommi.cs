#region �汾˵��

/*
 * �������ݣ�   �򵥵Ĵ���ͨѶ<���°汾������ȥȥ��>
 *
 * ��    �ߣ�   ���ٲ�
 *
 * �� �� �ߣ�   ���ٲ�
 *
 * ��    �ڣ�   2010-05-23
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
    /// ���ڶ�ȡ����ͨѶ��
    /// </summary>
    public class SerialCommi
    {
        private SerialPort _serialPort = new SerialPort();

        private int delay = 50;

        /// <summary>
        /// ��ȡ������ͨѶʱ�������ݵ���ʱʱ��(��λms,Ĭ��50)
        /// </summary>
        public int Delay
        {
            get { return delay; }
            set { delay = value; }
        }

        #region Э���������

        private string portName = "";
        /// <summary>
        /// ��ȡ���ڶ˿�����
        /// </summary>
        public string PortName
        {
            get { return portName; }
        }

        private int baudRate = 9600;
        /// <summary>
        /// ��ȡ����Э�鲨����
        /// </summary>
        public int BaudRate
        {
            get { return baudRate; }
        }
        private Parity parity = Parity.None;
        /// <summary>
        /// ��ȡ����Э����żУ��
        /// </summary>
        public Parity Parity
        {
            get { return parity; }
        }
        private int dataBits = 8;
        /// <summary>
        /// ��ȡ����Э������λ
        /// </summary>
        public int DataBits
        {
            get { return dataBits; }
        }
        private StopBits stopBits = StopBits.One;
        /// <summary>
        /// ��ȡ����Э��ֹͣλ
        /// </summary>
        public StopBits StopBits
        {
            get { return stopBits; }
        }

        #endregion


        #region ����ͨѶ����

        /// <summary>
        /// ���ô���ͨѶ����
        /// </summary>
        /// <param name="portName">��������</param>
        /// <param name="baudRate">������(Ĭ��9600)</param>
        /// <param name="parity">��żУ��λ(Ĭ��None)</param>
        /// <param name="dataBits">����λ(Ĭ��8)</param>
        /// <param name="stopBits">ֹͣλ(Ĭ��1)</param>
        /// <param name="delay">�ӳ�ʱ��(ms),Ĭ��40ms</param>
        public void SetProtocolParam(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits, int delay)
        {
            if (string.IsNullOrEmpty(portName))
                throw new Exception("���ڶ˿ڲ���Ϊ��");
            if (dataBits < 5 || dataBits > 8)
                throw new Exception("����λ��Χ�Ǵ� 5 �� 8");

            this.portName = this._serialPort.PortName = portName;
            this.baudRate = this._serialPort.BaudRate = baudRate;
            this.parity = this._serialPort.Parity = parity;
            this.dataBits = this._serialPort.DataBits = dataBits;
            this.stopBits = this._serialPort.StopBits = stopBits;
            this.delay = delay;
        }

        /// <summary>
        /// ���ô���ͨѶ����
        /// </summary>
        /// <param name="portName">��������</param>
        /// <param name="baudRate">������(Ĭ��9600)</param>
        /// <param name="parity">��żУ��λ(Ĭ��None)</param>
        /// <param name="dataBits">����λ(Ĭ��8)</param>
        /// <param name="stopBits">ֹͣλ(Ĭ��1)</param>
        public void SetProtocolParam(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            SetProtocolParam(portName, baudRate, parity, dataBits, stopBits, this.delay);
        }
        /// <summary>
        /// ���ô���ͨѶ����
        /// </summary>
        /// <param name="portName">��������</param>
        /// <param name="baudRate">������(Ĭ��9600)</param>
        /// <param name="parity">��żУ��λ(Ĭ��None)</param>
        /// <param name="dataBits">����λ(Ĭ��8)</param>
        public void SetProtocolParam(string portName, int baudRate, Parity parity, int dataBits)
        {
            SetProtocolParam(portName, baudRate, parity, dataBits, StopBits.One);
        }
        /// <summary>
        /// ���ô���ͨѶ����
        /// </summary>
        /// <param name="portName">��������</param>
        /// <param name="baudRate">������(Ĭ��9600)</param>
        /// <param name="parity">��żУ��λ(Ĭ��None)</param>
        public void SetProtocolParam(string portName, int baudRate, Parity parity)
        {
            SetProtocolParam(portName, baudRate, parity, 8, StopBits.One);
        }

        /// <summary>
        /// ���ô���ͨѶ����
        /// </summary>
        /// <param name="portName">��������</param>
        /// <param name="baudRate">������(Ĭ��9600)</param>
        /// <param name="delay">�ӳ�ʱ��(ms),Ĭ��40ms</param>
        public void SetProtocolParam(string portName, int baudRate, int delay)
        {
            SetProtocolParam(portName, baudRate, Parity.None, 8, StopBits.One, delay);
        }
        /// <summary>
        /// ���ô���ͨѶ����
        /// </summary>
        /// <param name="portName">��������</param>
        /// <param name="baudRate">������(Ĭ��9600)</param>
        public void SetProtocolParam(string portName, int baudRate)
        {
            SetProtocolParam(portName, baudRate, Parity.None, 8, StopBits.One);
        }
        /// <summary>
        /// ���ô���ͨѶ����
        /// </summary>
        /// <param name="portName">��������</param>
        public void SetProtocolParam(string portName)
        {
            SetProtocolParam(portName, 9600, Parity.None, 8, StopBits.One);
        }

        #endregion

        /// <summary>
        /// ����ͨѶ���������������
        /// </summary>
        /// <param name="cmd">�����������ַ���</param>
        /// <returns>���ؽ��յ����ֽ�����</returns>
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
        /// ����ͨѶ���������������
        /// </summary>
        /// <param name="cmd">�����������ַ���</param>
        /// <param name="isHEX">�Ƿ���16���Ƹ�ʽ���ַ���</param>
        /// <returns>���ؽ��յ����ֽ�����</returns>
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
        /// ����ͨѶ���������������
        /// </summary>
        /// <param name="cmd">�����������ֽ�����</param>
        /// <returns>���ؽ��յ����ֽ�����</returns>
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
