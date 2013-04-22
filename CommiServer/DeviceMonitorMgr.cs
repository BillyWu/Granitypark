using System;
using System.Collections.Generic;
using System.Text;
using Granity.communications;
using System.Data;
using Estar.Business.DataManager;
using ComLib;
using System.Threading;
using System.Collections.Specialized;
using Estar.Common.Tools;
using System.IO.Ports;
using System.Diagnostics;
using System.Timers;

namespace Granity.commiServer
{
    /// <summary>
    /// �豸Ѳ�����: 
    /// �Ծ����豸����Ѳ��ɼ�,��Ѳ�����Զ����������ݿ�,���ַ�Ѳ�������ͻ���
    /// </summary>
    public class DeviceMonitorMgr
    {
        /// <summary>
        /// ����ʱ��
        /// </summary>
        private System.Timers.Timer tmService = new System.Timers.Timer();
        /// <summary>
        /// ��ǰ��ʱ���Ƿ���ִ��
        /// </summary>
        private bool isRuning = false;
        /// <summary>
        /// �豸��ص�Ԫ
        /// </summary>
        private UnitItem unitItem = null;
        /// <summary>
        /// ���ݲ���
        /// </summary>
        private QueryDataRes query = null;
        /// <summary>
        /// Ѳ���豸�б�
        /// </summary>
        private List<DeviceBase> devlist = new List<DeviceBase>();

        /// <summary>
        /// �豸Ѳ������캯��
        /// </summary>
        public DeviceMonitorMgr()
        {
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "�豸��ط���");
            this.query = new QueryDataRes(this.unitItem.DataSrcFile);

            //2����ִ��һ��
            tmService.Interval = 360000;
            tmService.Elapsed += new ElapsedEventHandler(tmService_Elapsed);
            tmService.Enabled = true;
            tmService.Start();
        }
        /// <summary>
        /// ��ʱ��ִ��,��ʱ�����Ч�豸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void tmService_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isRuning) return;
            try
            {
                isRuning = true;
                Haltdev(null, null);
            }
            catch (Exception ex)
            {
                ServiceTool.LogMessage(ex, null, EventLogEntryType.Error);
            }
            isRuning = false;
        }

        /// <summary>
        /// ����豸,Ѳ��ָ������,����Ѳ���豸
        /// �������5�����Զ���Ϊ��ָ���˳�
        /// </summary>
        /// <param name="info">ָ�����ݣ�service='monitor',device(���ŷָ����豸ID),id,patrol='true'(patrol����ִ��)</param>
        /// <param name="client">�ͻ���������Ϣ</param>
        public void Monitordev(NameValueCollection info, ClientInfo client)
        {
            if (null == info || null == client || null == client.Client || !client.Client.Connected)
                return;
            string devid = info["device"];
            if (string.IsNullOrEmpty(devid))
            {
                this.returnInfo(info, client, true);
                return;
            }
            ServiceTool.LogMessage("����Ѳ��(�豸)��" + devid, null, EventLogEntryType.Warning);
            //�����豸�б�,������Ѳ���豸
            string[] devices = devid.ToLower().Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            DeviceBase[] devs = devlist.ToArray();
            for (int i = 0; i < devices.Length; i++)
            {
                string id=devices[i];
                bool exist = false;
                for (int j = 0; j < devs.Length; j++)
                {
                    if (id != devs[i].DevID.ToLower())
                        continue;
                    devs[j].dtBeat = DateTime.Now;
                    exist = true;
                    break;
                }
                if (exist) continue;
                this.addDevice(devid, true);
            }
            this.returnInfo(info, client, true);
        }

        /// <summary>
        /// ��Ӽ�ع�����豸
        /// </summary>
        /// <param name="devid">����Ŀͻ���</param>
        /// <param name="ispatrol">�Ƿ����Ѳ��</param>
        private DeviceBase addDevice(string devid, bool ispatrol)
        {
            //�����豸���
            Estar.Common.Tools.NameObjectList ps = new Estar.Common.Tools.NameObjectList();
            ps["�豸ID"] = devid;
            DataTable tab = this.query.getTable("�豸ͨѶ����", ps);
            if (null == tab || tab.Rows.Count < 1)
                return null;
            DataRow dr = tab.Rows[0];
            string dvtype = Convert.ToString(dr["ͨѶЭ��"]);
            if (string.IsNullOrEmpty(dvtype) || DBNull.Value == dr["վַ"])
                return null;
            int station = Convert.ToInt32(dr["վַ"]);
            CommiTarget target = this.getTarget(dr);
            CommiManager commimgr = CommiManager.GlobalManager;
            DeviceBase device = null;
            switch (dvtype)
            {
                case "�Ž�":
                    device = new DeviceDoor();
                    target.setProtocol(Protocol.PTLDoor);
                    break;
                case "����":
                    device = new DeviceEatery();
                    target.setProtocol(Protocol.PTLEatery);
                    break;
                default:
                    return null;
            }
            device.IsPatrol = ispatrol;
            device.dtBeat = DateTime.Now;
            device.SetDevice(commimgr, target, devid, station);
            this.devlist.Add(device);
            device.StartGather();
            return device;
        }

        /// <summary>
        /// �Ƴ���ص��豸��û��ָ���豸���Ƴ�����ָ��,�����Ч(������ʱ)ָ���Ƴ�
        /// </summary>
        /// <param name="info">ָ�����ݣ�service='halt',device(���ŷָ����豸ID),id,all='true'</param>
        /// <param name="client">�ͻ���������Ϣ</param>
        public void Haltdev(NameValueCollection info, ClientInfo client)
        {
            if (null == info)
                info = new NameValueCollection();
            string devid = info["device"];
            if (string.IsNullOrEmpty(devid))
                devid = "";
            ServiceTool.LogMessage("ֹͣѲ��(�豸)��" + devid, null, EventLogEntryType.Warning);
            //�Ƴ�ָ�����豸���,���Ƴ�ָ��,���Ƴ���Чָ��
            devid = devid.ToLower();
            DateTime dtindex = DateTime.Now.AddMonths(-5);
            DeviceBase[] devs = this.devlist.ToArray();
            for (int i = 0; i < devs.Length; i++)
            {
                string id = devs[i].DevID.ToLower();
                if (!devid.Contains(id) && devs[i].dtBeat > dtindex)
                    continue;
                this.devlist.Remove(devs[i]);
                devs[i].StopGather();
                ServiceTool.LogMessage("����豸��" + devs[i].DevID, null, EventLogEntryType.Warning);
            }
            this.returnInfo(info, client, true);
        }

        /// <summary>
        /// ����ָ���Ƿ�ɹ�
        /// </summary>
        /// <param name="isSeccess">�Ƿ�ɹ�</param>
        private void returnInfo(NameValueCollection info, ClientInfo client, bool isSeccess)
        {
            if (null == info || null == client || null == client.Client)
                return;
            string[] keys ={ "id", "cmd", "service" };
            NameValueCollection msg = new NameValueCollection();
            foreach (string k in keys)
                msg[k] = info[k];
            msg["len"] = "0";
            msg["success"] = isSeccess ? "true" : "false";
            byte[] context = SvrFileTrans.ParseInfo(msg);
            Monitor.Enter(client);
            client.BufferResponse.Add(context);
            Monitor.PulseAll(client);
            Monitor.Exit(client);
        }

        /// <summary>
        /// ��ȡ�豸��Ϣ,���������ݺ�״̬
        /// </summary>
        /// <param name="info">ָ�����ݣ�service='halt',device,id, dt='yyyy-MM-dd HH:mm:ss',datatype='record|alarm|signal'</param>
        /// <param name="client">�ͻ���������Ϣ</param>
        public void ReadInfodev(NameValueCollection info, ClientInfo client)
        {
            if (null == info)
                return;
            
            string cmdid = info["id"];
            string devid = info["device"];
            string datatype = info["datatype"];
            string dt = info["dt"];
            if (string.IsNullOrEmpty(devid))
                return;
            if ("record" != datatype && "signal" != datatype && "alarm" != datatype)
                return;
            devid = devid.ToLower();
            DateTime dtindex = DateTime.Now.AddSeconds(-30);
            if (!string.IsNullOrEmpty(dt))
                try { dtindex = Convert.ToDateTime(dt); }
                catch { }

            //���ǳ���Ѳ���豸,����Ѳ��
            DeviceBase[] devs = this.devlist.ToArray();
            DeviceBase device = null;
            for (int i = 0; i < devs.Length; i++)
            {
                if (devid != devs[i].DevID.ToLower())
                    continue;
                device = devs[i];
                break;
            }
            if (null == device)
            {
                device = this.addDevice(devid, true);
                if (null == device)
                    return;
                Thread.Sleep(800);
            }

            //��ȡ����
            string tag = "";
            if ("alarm" == datatype)
            {
                tag = device.Alarm.tag;
                dt = device.Alarm.dtReceive.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if ("signal" == datatype)
            {
                tag = device.Signal.tag;
                dt = device.Signal.dtReceive.ToString("yyyy-MM-dd HH:mm:ss");
            }
            else if ("record" == datatype)
            {
                RecordDev[] rds = device.Rows.ToArray();
                for (int i = 0; i < rds.Length; i++)
                {
                    if (rds[i].dtReceive < dtindex)
                        continue;
                    tag += ";" + rds[i].tag;
                    dt = rds[i].dtReceive.ToString("yyyy-MM-dd HH:mm:ss");
                }
                if (!string.IsNullOrEmpty(tag))
                    tag = tag.Substring(1);
            }
            NameValueCollection nvclient = new NameValueCollection();
            string[,] map ={ { "id", cmdid }, { "cmd", "TransFile.extend" }, { "service", "readinfo" }, { "len", "0" }, { "deviceid", device.DevID }, { "datatype", datatype }, { "dt", dt } };
            for (int i = 0; i < map.GetLength(0); i++)
                nvclient.Add(map[i, 0], map[i, 1]);
            byte[] data = Encoding.GetEncoding("GB2312").GetBytes(tag);
            nvclient["len"] = Convert.ToString(data.Length);
            byte[] context = SvrFileTrans.ParseInfo(nvclient);
            int len = context.Length;
            Array.Resize<byte>(ref context, context.Length + data.Length);
            Array.Copy(data, 0, context, len, data.Length);
            Monitor.Enter(client);
            client.BufferResponse.Add(context);
            Monitor.PulseAll(client);
            Monitor.Exit(client);
        }
        /// <summary>
        /// ��ȡ�����е��豸Ŀ��λ�ò���
        /// ��¼�����ֶΡ����ʷ�ʽ��(TCP/UDP/SerialPort)�����˿ڡ�(60000/COM1)������ַ��(192.168.1.146)
        /// </summary>
        /// <param name="dr">���ݼ�¼</param>
        /// <returns></returns>
        private CommiTarget getTarget(DataRow dr)
        {
            if (null == dr || (DBNull.Value == dr["����"] && DBNull.Value == dr["IP��ַ"]))
                return null;
            CommiTarget target = new CommiTarget();
            CommiType commiType = CommiType.UDP;
            string stype = Convert.ToString(dr["ͨѶ���"]);
            switch (stype)
            {
                case "TCP/IP(������)":
                    commiType = CommiType.TCP; break;
                case "UDP/IP(������)":
                    commiType = CommiType.UDP; break;
                default:
                    commiType = CommiType.SerialPort; break;
            }
            try
            {
                if (CommiType.SerialPort == commiType)
                {
                    string portname = Convert.ToString(dr["����"]);
                    int baudRate = Convert.ToInt16(dr["������"]);
                    int dataBits = Convert.ToInt16(dr["����λ"]);
                    decimal s = Convert.ToDecimal(dr["ֹͣλ"]);
                    StopBits sb = StopBits.None;
                    if (1 == s) sb = StopBits.One;
                    else if (2 == s) sb = StopBits.Two;
                    else if (1 < s && s < 2) sb = StopBits.OnePointFive;

                    target.SetProtocolParam(portname, baudRate, Parity.None, dataBits, sb);
                }
                else
                {
                    string addr = Convert.ToString(dr["IP��ַ"]);
                    if (string.IsNullOrEmpty(addr) || DBNull.Value == dr["�˿�"])
                        return null;
                    int port = Convert.ToInt32(dr["�˿�"]);
                    target.SetProtocolParam(addr, port, commiType);
                }
            }
            catch (Exception ex)
            {
                NameValueCollection data = new NameValueCollection();
                data["����"] = "����ͨѶĿ��";
                data["�豸ID"] = Convert.ToString(dr["ID"]);
                ServiceTool.LogMessage(ex, data, EventLogEntryType.Error);
                return null;
            }
            return target;
        }

    }
}
