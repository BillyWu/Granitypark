using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Estar.Business.DataManager;
using System.Data;
using Estar.Common.Tools;
using Granity.communications;
using System.IO.Ports;
using System.Collections.Specialized;
using System.Diagnostics;
using Granity.CardOneCommi;
using System.Net;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Threading;

namespace Granity.commiServer
{
    /// <summary>
    /// ͬ���豸����,���ºڰ��������豸���Ʋ���
    /// </summary>
    public class SynDeviceParam
    {
        /// <summary>
        /// ����ʱ��
        /// </summary>
        private System.Timers.Timer tmService = new System.Timers.Timer();
        /// <summary>
        /// ��ǰ�Ƿ�����ִ��
        /// </summary>
        private bool isRuning = false;
        /// <summary>
        /// Э������Դ
        /// </summary>
        private const string dbSrc = "������";
        /// <summary>
        /// ���ݲ�ѯ
        /// </summary>
        private QueryDataRes query = new QueryDataRes(dbSrc);

        /// <summary>
        /// ���ö�����ָ��ʱ,���ݲ�����
        /// </summary>
        private class TransArg
        {
            /// <summary>
            /// ��תָ��
            /// </summary>
            public CmdFileTrans trans;
            /// <summary>
            /// ͨѶ�����ַ
            /// </summary>
            public CommiTarget target;
            /// <summary>
            /// ͨѶĿ�ĵش���IP��ַ
            /// </summary>
            public IPAddress proxy;
            /// <summary>
            /// ͨѶĿ��
            /// </summary>
            public CommiTarget dest;
            /// <summary>
            /// ��ǰ�豸ID
            /// </summary>
            public string devID;
            /// <summary>
            /// ͨѶվַ
            /// </summary>
            public string addrst;
            /// <summary>
            /// ����ָ����,tag��ʽ��Ϣ,��ʵ��ָ�����ִ��
            /// </summary>
            public string attrCmdtag;
        }

        public SynDeviceParam()
        {
            //20����ִ��һ��
            tmService.Interval = 1200000;
            tmService.Elapsed += new ElapsedEventHandler(tmService_Elapsed);
            tmService.Enabled = true;
            tmService.Start();
        }

        void tmService_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isRuning) return;
            try
            {
                isRuning = true;
                CommiDevice();
            }
            catch (Exception ex)
            {
                ServiceTool.LogMessage(ex, null, EventLogEntryType.Error);
            }
            isRuning = false;
        }

        /// <summary>
        /// ������ʱִ��,ÿ20����ִ��һ�μ��
        /// </summary>
        public void Start()
        {
            //20����ִ��һ��
            tmService.Interval = 1200000;
            tmService.Enabled = true;
            tmService.Start();
        }

        /// <summary>
        /// ֹͣ��ʱִ��
        /// </summary>
        public void Stop()
        {
            //20����ִ��һ��
            tmService.Enabled = false;
            tmService.Stop();
        }

        /// <summary>
        /// �����豸���Ʋ���
        /// </summary>
        public void CommiDevice()
        {
        }

        /// <summary>
        /// ���������豸�ڰ�����
        /// </summary>
        /// <returns>���ش�����ʾ���޴�����ʾ�ɹ�</returns>
        public string downCardALL()
        {
            NameObjectList ps = new NameObjectList();
            DataTable tab = this.query.getTable("�豸�б�", ps);
            if (null == tab || tab.Rows.Count < 1)
                return "";
            string devid = "";
            string attrcmd = "|����ID������|���غ�����|ɾ��������|ɾ��ID������|���ذ�����|";

            //������ת������
            CmdFileTrans trans = new CmdFileTrans(false);
            int port = 2010;
            string sport = DataAccRes.AppSettings("Granity�ļ�����");
            if (!string.IsNullOrEmpty(sport))
                try { port = Convert.ToInt32(sport); }
                catch { return ""; }
            string conn = DataAccRes.DefaultDataConnInfo.Value;
            Regex regIP = new Regex(@"server=([\w.\(\)]*)(;|\\)");
            string ipsrv = "127.0.0.1";
            if (regIP.IsMatch(conn))
            {
                Match mt = regIP.Match(conn);
                if (mt.Groups.Count > 1)
                    ipsrv = mt.Groups[1].Value.ToLower();
                if ("(local)" == ipsrv || "127.0.0.1" == ipsrv)
                    ipsrv = Dns.GetHostName();
                ipsrv = Dns.GetHostAddresses(ipsrv)[0].ToString();
            }
            CommiTarget tarsrv = new CommiTarget(ipsrv, port, CommiType.TCP);
            tarsrv.setProtocol(CmdFileTrans.PTL);

            string msg = "";
            foreach (DataRow dr in tab.Rows)
            {
                //����IP,�������Ƿ񱾵�local,ͨѶĿ��
                string addr = Convert.ToString(dr["IP��ַ"]);
                if (string.IsNullOrEmpty(addr))
                    continue;
                IPAddress proxy = null;
                try
                {
                    proxy = IPAddress.Parse(addr);
                }
                catch (Exception ex)
                {
                    NameValueCollection data = new NameValueCollection();
                    data["IP��ַ"] = addr;
                    ServiceTool.LogMessage(ex, data, EventLogEntryType.Error);
                    msg += string.Format("��{0}({1})", dr["����"], dr["IP��ַ"]);
                    continue;
                }
                CommiTarget dest = this.getTarget(dr);
                if (null == dest)
                {
                    msg += string.Format("��{0}({1})", dr["����"], dr["IP��ַ"]);
                    continue;
                }
                string tpl = Convert.ToString(dr["ͨѶЭ��"]);
                bool rtn = true;
                switch (tpl)
                {
                    case "ͣ����":
                        dest.setProtocol(Protocol.PTLPark);
                        rtn = this.commiDevicePark(tarsrv, trans, proxy, dest, dr, attrcmd);
                        break;
                    case "�Ž�":
                        dest.setProtocol(Protocol.PTLDoor);
                        rtn = this.commiDeviceDoor(tarsrv, trans, proxy, dest, dr, attrcmd);
                        break;
                    case "����":
                        dest.setProtocol(Protocol.PTLEatery);
                        rtn = this.commiDeviceEatery(tarsrv, trans, proxy, dest, dr, attrcmd);
                        break;

                }
                if (!rtn)
                    msg += string.Format("��{0}({1})", dr["����"], dr["IP��ַ"]);
            }
            if (!string.IsNullOrEmpty(msg))
                msg = msg.Substring(1);
            return msg;
        }

        /// <summary>
        /// ǿ�ƶ��豸��ȫ����ʼ�������ز���,���ºڰ�����
        /// </summary>
        /// <param name="tagdevcmds">�豸ID������ָ��,tag��ʽ,����ָ����"|"�ָ�</param>
        public bool CommiDevice(string tagdevcmds)
        {
            string devID = basefun.valtag(tagdevcmds, "�豸ID");
            string attrcmd = "|" + basefun.valtag(tagdevcmds, "ָ��") + "|";
            if (string.IsNullOrEmpty(tagdevcmds))
                return false;

            //������ת������
            CmdFileTrans trans = new CmdFileTrans(false);
            int port = 2010;
            string sport = DataAccRes.AppSettings("Granity�ļ�����");
            if (!string.IsNullOrEmpty(sport))
                try { port = Convert.ToInt32(sport); }
                catch { return false; }
            string conn = DataAccRes.DefaultDataConnInfo.Value;
            Regex regIP = new Regex(@"server=([\w.\(\)]*)(;|\\)");
            string ipsrv = "127.0.0.1";
            if (regIP.IsMatch(conn))
            {
                Match mt = regIP.Match(conn);
                if (mt.Groups.Count > 1)
                    ipsrv = mt.Groups[1].Value.ToLower();
                if ("(local)" == ipsrv || "127.0.0.1" == ipsrv)
                    ipsrv = Dns.GetHostName();
                ipsrv = Dns.GetHostAddresses(ipsrv)[0].ToString();
            }
            CommiTarget tarsrv = new CommiTarget(ipsrv, port, CommiType.TCP);
            tarsrv.setProtocol(CmdFileTrans.PTL);

            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            DataTable tab = this.query.getTable("�豸ͨѶ����", ps);
            if (null == tab || tab.Rows.Count < 1)
                return true;
            DataRow dr = tab.Rows[0];
            //����IP,�������Ƿ񱾵�local,ͨѶĿ��
            string addr = Convert.ToString(dr["IP��ַ"]);
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();

            if (string.IsNullOrEmpty(addr))
                addr = myip;
            if (string.IsNullOrEmpty(addr))
                return true;
            IPAddress proxy = null;
            try
            {
                proxy = IPAddress.Parse(addr);
            }
            catch (Exception ex)
            {
                NameValueCollection data = new NameValueCollection();
                data["IP��ַ"] = addr;
                ServiceTool.LogMessage(ex, null, EventLogEntryType.Error);
                return false;
            }
            CommiTarget dest = this.getTarget(dr);
            if (null == dest)
                return false;

            string tpl = Convert.ToString(dr["ͨѶЭ��"]);
            switch (tpl)
            {
                case "ͣ����":
                    dest.setProtocol(Protocol.PTLPark);
                    return this.commiDevicePark(tarsrv, trans, proxy, dest, dr, attrcmd);
                case "�Ž�":
                    dest.setProtocol(Protocol.PTLDoor);
                    return this.commiDeviceDoor(tarsrv, trans, proxy, dest, dr, attrcmd);
                case "����":
                    dest.setProtocol(Protocol.PTLEatery);
                    return this.commiDeviceEatery(tarsrv, trans, proxy, dest, dr, attrcmd);
            }
            return true;
        }


        /// <summary>
        /// ͣ����
        /// </summary>
        /// <param name="tarsrv"></param>
        /// <param name="trans"></param>
        /// <param name="proxy"></param>
        /// <param name="dest"></param>
        /// <param name="cmdP"></param>
        /// <param name="tpl"></param>
        /// <param name="cmd"></param>
        /// <param name="tagdata"></param>
        /// <returns></returns>
        public string getParkResponse(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, CmdProtocol cmdP, string tpl, string cmd, string tagdata)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == cmdP)
                return "";
            if (string.IsNullOrEmpty(tpl) || string.IsNullOrEmpty(cmd) || string.IsNullOrEmpty(tagdata))
                return "";
            CommiManager mgr = CommiManager.GlobalManager;
            string msg = "";

            bool islocal = false;
            string addr = proxy.ToString();
            IPAddress[] locals = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in locals)
                if (addr == ip.ToString())
                {
                    islocal = true;
                    break;
                }
            if ("127.0.0.1" == addr)
                islocal = true;
            //����ͨѶ��Ŀ���Ǵ�����������ֱ��ͨѶ
            if (islocal || CommiType.SerialPort != dest.ProtocolType)
            {
                if (null == cmdP.EventWh)
                    cmdP.EventWh = new ManualResetEvent(false);
                for (int i = 0; i < 3; i++)
                {
                    cmdP.setCommand(tpl, cmd, tagdata);
                    cmdP.ResetState();
                    mgr.SendCommand(dest, cmdP);
                    msg = "";
                    if (cmdP.EventWh.WaitOne(800, false))
                        msg = cmdP.ResponseFormat;
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{״̬}")))
                        break;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    cmdP.setCommand(tpl, cmd, tagdata);
                    byte[] data = cmdP.getCommand();
                    trans.CommiTrans(proxy, dest, tpl, cmd, ref data);
                    trans.ResetState();
                    mgr.SendCommand(tarsrv, trans);
                    //ͬ��ͨѶ�ȴ�ʱ��1.5��
                    if (trans.EventWh.WaitOne(1500, false))
                        data = trans.FileContext;
                    msg = cmdP.FormatResponse(data);
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{״̬}")))
                        break;
                }
            }
            if ("true" != basefun.valtag(msg, "Success"))
            {
                NameValueCollection attr = new NameValueCollection();
                attr["Э��"] = tpl;
                attr["����"] = tagdata + "\r\n" + cmd;
                ServiceTool.LogMessage(msg, attr, EventLogEntryType.Error);
                return msg;
            }
            return msg;
        }


        /// <summary>
        /// ��תִ��ָ��
        /// </summary>
        /// <param name="tarsrv">������</param>
        /// <param name="trans">����ʵ��</param>
        /// <param name="proxy">�豸��������IP��ַ</param>
        /// <param name="dest">�豸Ŀ��</param>
        /// <param name="cmdP">ָ��ʵ��</param>
        /// <param name="tpl">Э������</param>
        /// <param name="cmd">ָ������</param>
        /// <param name="tagdata">ָ������ tag��ʽ</param>
        /// <returns>�Ƿ�ִ�гɹ�</returns>
        private bool sendCommand(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, CmdProtocol cmdP, string tpl, string cmd, string tagdata)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == cmdP)
                return false;
            if (string.IsNullOrEmpty(tpl) || string.IsNullOrEmpty(cmd) || string.IsNullOrEmpty(tagdata))
                return false;
            CommiManager mgr = CommiManager.GlobalManager;
            string msg = "";

            bool islocal = false;
            string addr = proxy.ToString();
            IPAddress[] locals = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in locals)
                if (addr == ip.ToString())
                {
                    islocal = true;
                    break;
                }
            if ("127.0.0.1" == addr)
                islocal = true;
            //����ͨѶ��Ŀ���Ǵ�����������ֱ��ͨѶ
            if (islocal || CommiType.SerialPort != dest.ProtocolType)
            {
                if (null == cmdP.EventWh)
                    cmdP.EventWh = new ManualResetEvent(false);
                for (int i = 0; i < 3; i++)
                {
                    cmdP.setCommand(tpl, cmd, tagdata);
                    cmdP.ResetState();
                    mgr.SendCommand(dest, cmdP);
                    msg = "";
                    if (cmdP.EventWh.WaitOne(800, false))
                        msg = cmdP.ResponseFormat;
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{״̬}")))
                        break;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    cmdP.setCommand(tpl, cmd, tagdata);
                    byte[] data = cmdP.getCommand();
                    trans.CommiTrans(proxy, dest, tpl, cmd, ref data);
                    trans.ResetState();
                    mgr.SendCommand(tarsrv, trans);
                    //ͬ��ͨѶ�ȴ�ʱ��1.5��
                    if (trans.EventWh.WaitOne(1500, false))
                        data = trans.FileContext;
                    msg = cmdP.FormatResponse(data);
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{״̬}")))
                        break;
                }
            }
            if ("true" != basefun.valtag(msg, "Success"))
            {
                NameValueCollection attr = new NameValueCollection();
                attr["Э��"] = tpl;
                attr["����"] = tagdata + "\r\n" + cmd;
                ServiceTool.LogMessage(msg, attr, EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// ��תִ��ָ�������Ӧ���
        /// </summary>
        /// <param name="tarsrv">������</param>
        /// <param name="trans">����ʵ��</param>
        /// <param name="proxy">�豸��������IP��ַ</param>
        /// <param name="dest">�豸Ŀ��</param>
        /// <param name="cmdP">ָ��ʵ��</param>
        /// <param name="tpl">Э������</param>
        /// <param name="cmd">ָ������</param>
        /// <param name="tagdata">ָ������ tag��ʽ</param>
        /// <returns>����ִ�н��</returns>
        private string getResponse(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, CmdProtocol cmdP, string tpl, string cmd, string tagdata)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == cmdP)
                return "";
            if (string.IsNullOrEmpty(tpl) || string.IsNullOrEmpty(cmd) || string.IsNullOrEmpty(tagdata))
                return "";
            CommiManager mgr = CommiManager.GlobalManager;
            string msg = "";

            bool islocal = false;
            string addr = proxy.ToString();
            IPAddress[] locals = Dns.GetHostAddresses(Dns.GetHostName());
            foreach (IPAddress ip in locals)
                if (addr == ip.ToString())
                {
                    islocal = true;
                    break;
                }
            if ("127.0.0.1" == addr)
                islocal = true;
            //����ͨѶ��Ŀ���Ǵ�����������ֱ��ͨѶ
            if (islocal || CommiType.SerialPort != dest.ProtocolType)
            {
                if (null == cmdP.EventWh)
                    cmdP.EventWh = new ManualResetEvent(false);
                for (int i = 0; i < 3; i++)
                {
                    cmdP.setCommand(tpl, cmd, tagdata);
                    cmdP.ResetState();
                    mgr.SendCommand(dest, cmdP);
                    msg = "";
                    if (cmdP.EventWh.WaitOne(800, false))
                        msg = cmdP.ResponseFormat;
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{״̬}")))
                        break;
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    cmdP.setCommand(tpl, cmd, tagdata);
                    byte[] data = cmdP.getCommand();
                    trans.CommiTrans(proxy, dest, tpl, cmd, ref data);
                    trans.ResetState();
                    mgr.SendCommand(tarsrv, trans);
                    //ͬ��ͨѶ�ȴ�ʱ��1.5��
                    if (trans.EventWh.WaitOne(1500, false))
                        data = trans.FileContext;
                    msg = cmdP.FormatResponse(data);
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{״̬}")))
                        break;
                }
            }
            return msg;
        }
        /// <summary>
        /// 10����ת16����
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string sValue10to16(string str)
        {
            if (string.IsNullOrEmpty(str)) str = "00";
            string s = Convert.ToInt32(str).ToString("X");
            if (s.Length % 2 != 0)
            {
                s = "0" + s;//����λ�򲹣������λ�ĸ�ʽ
            }
            return s;
        }
        public string Internal_code(string values)
        {
            string newValue = "";
            string st = string.Empty;
            byte[] array = System.Text.Encoding.Default.GetBytes(values);
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] >= 128 && array[i] <= 247)
                {
                    st += string.Format("{0},{1},", array[i], array[i + 1]);
                    i++;
                }
                else
                {
                    st = st + string.Format("{0},", array[i]);
                }
            }
            string[] str2 = st.Split(',');
            foreach (string i in str2)
            {
                newValue = sValue10to16(i.ToString());
            }
            return newValue;
        }
        #region ͣ����ͨѶ

        /// <summary>
        /// ͣ�����豸��ʼ�������ز���,���ºڰ�����
        /// </summary>
        /// <param name="tarsrv">��ת������</param>
        /// <param name="trans">����ָ��</param>
        /// <param name="proxy">�豸ͨѶ��������IP��ַ</param>
        /// <param name="dest">Ŀ���豸</param>
        /// <param name="drdevice">�豸��Ϣ��¼��</param>
        /// <param name="attrcmd">ִ��ָ��</param>
        /// <returns>����ͨѶ���ز����Ƿ�ɹ�</returns>
        private bool commiDevicePark(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, DataRow drdevice, string attrcmd)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == drdevice || string.IsNullOrEmpty(attrcmd))
                return true;
            CmdProtocol cmdP = new CmdProtocol(false);
            string devID = Convert.ToString(drdevice["ID"]);
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            bool success = false;

            //ϵͳʱ��
            string tpl = "ͣ����";
            string valst = Convert.ToString(drdevice["վַ"]);
            DateTime dtsystem = Convert.ToDateTime(this.query.ExecuteScalar("ϵͳʱ��", ps));
            string cmdstr = ",��ʽ��,��ʼ��ID������,��ʼ��������,����ϵͳʱ��,���ؿ��Ʋ���,��ȡ���Ʋ���,����������Ϣ,";
            string[] cmds = attrcmd.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmdstr.IndexOf("," + cmds[i] + ",") < 0)
                    continue;
                string tagdata = "@�豸��ַ=" + valst;
                if ("���ؿ��Ʋ���" == cmds[i])
                {
                    DataTable tabctrlpm = this.query.getTable("�豸���Ʋ���", ps);
                    if (null == tabctrlpm || tabctrlpm.Rows.Count < 1)
                        continue;
                    tagdata = this.getctrlpm(tabctrlpm.Rows[0], valst);
                }
                else if ("����ϵͳʱ��" == cmds[i])
                {
                    tagdata = basefun.setvaltag(tagdata, "{ϵͳʱ��}", dtsystem.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if ("����������Ϣ" == cmds[i])
                {
                    DataTable tabScress = this.query.getTable("����������Ϣ", ps);
                    if (null == tabScress || tabScress.Rows.Count < 1)
                        continue;
                    string cmd = "";
                    string downtype = "";
                    string datatype = "";
                    string message = "";
                    if (string.IsNullOrEmpty(Convert.ToString(tabScress.Rows[0]["������Ϣ"])))
                    {
                        cmd = "0000" + Internal_code(Convert.ToString(tabScress.Rows[0]["������Ϣ"]));
                        downtype = "00";
                        datatype = "00";
                        message = Convert.ToString(tabScress.Rows[0]["������Ϣ"]);
                    }
                    else
                    {
                        cmd = "0100" + Internal_code(Convert.ToString(tabScress.Rows[0]["��ӡ��Ϣ"]));
                        downtype = "01";
                        datatype = "00";
                        message = Convert.ToString(tabScress.Rows[0]["��ӡ��Ϣ"]);
                    }
                    tagdata = basefun.setvaltag(tagdata, "{���ط�ʽ}", downtype);
                    tagdata = basefun.setvaltag(tagdata, "��������", datatype);
                    tagdata = basefun.setvaltag(tagdata, "�Զ�����Ϣ", Internal_code(message));
                    string sLen = (cmd.Length / 2).ToString("X");
                    tagdata = basefun.setvaltag(tagdata, "{�����}", sLen);
                }
                cmdP.setCommand(tpl, cmds[i], tagdata);
                success = this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, cmds[i], tagdata);
                if (!success) return false;
                //��ʽ��ʱ�豸��2.5s��Ӧ��
                if ("��ʽ��" == cmds[i])
                    System.Threading.Thread.Sleep(3500);
            }

            //��������ָ�����ʵ��
            TransArg arg = new TransArg();
            arg.trans = trans;
            arg.target = tarsrv;
            arg.proxy = proxy;
            arg.dest = dest;
            arg.devID = devID;
            arg.addrst = Convert.ToString(drdevice["վַ"]);
            arg.attrCmdtag = attrcmd;

            //���ؼƷѱ�׼
            success = this.downPayRule(arg);
            //if (!success) return false;
            ////������������
            //this.query.ExecuteNonQuery("�豸���ر�־����", ps, ps, ps);

            //���úڰ�����
            if (attrcmd.IndexOf("|��ʽ��|") > -1 || attrcmd.IndexOf("|��ʼ��ID������|") > -1 || attrcmd.IndexOf("|��ʼ��������|") > -1)
                this.query.ExecuteNonQuery("�����豸����", ps, ps, ps);
            success = this.downparkCardList(arg);
            //�豸������ʾ
            string tag = "@�豸��ַ=" + valst;
            this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, "����", tag);
            return success;
        }

        /// <summary>
        /// �����豸�ĺڰ�������,ִ�гɹ�����ºڰ�������������
        /// </summary>
        /// <param name="arg">����ָ�����</param>
        private bool downparkCardList(TransArg arg)
        {
            CmdFileTrans trans = arg.trans;
            CommiTarget target = arg.target;
            IPAddress proxy = arg.proxy;
            CommiTarget dest = arg.dest;
            string devID = arg.devID;
            string addrst = arg.addrst;
            string attrcmd = arg.attrCmdtag;

            if (null == trans || null == proxy || null == dest || string.IsNullOrEmpty(devID) || string.IsNullOrEmpty(addrst))
                return false;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameValueCollection data = new NameValueCollection();
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            //ͬ��ͨѶ�ȴ�ʱ��15��
            bool success = true;

            DataTable tab = this.query.getTable("�豸������", ps);
            string tpl = "ͣ����", cmd = "ɾ��ID������";
            string tagdata = "@�豸��ַ=" + addrst;
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|ɾ��ID������|") > -1)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    tagdata = basefun.setvaltag(tagdata, "{����}", Convert.ToString(dr["����"]));
                    bool rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                    success = success && rtn;
                    if (!rtn)
                    {
                        if (!testConnect(target, dest))
                            return false;
                        continue;
                    }
                    ps["״̬"] = "��";
                    ps["����"] = Convert.ToString(dr["����"]);
                    this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                }
            tab = this.query.getTable("�豸������", ps);
            cmd = "����ID������";
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|����ID������|") > -1)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    string[,] colmap ={ { "{����}", "����" }, { "{����}", "����" }, { "{����}", "����" }, { "{ʱ��}", "ʱ��" }, { "{��Ч����}", "��Ч����" } };
                    for (int j = 0; j < colmap.GetLength(0); j++)
                        tagdata = basefun.setvaltag(tagdata, colmap[j, 0], Convert.ToString(dr[colmap[j, 1]]));
                    bool rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                    success = success && rtn;
                    if (!rtn)
                    {
                        if (!testConnect(target, dest))
                            return false;
                        continue;
                    }
                    ps["״̬"] = "��";
                    ps["����"] = Convert.ToString(dr["����"]);
                    ps["�Ƿ����"] = 1;
                    ps["������"] = 1;
                    this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                }
            return success;
        }

        /// <summary>
        /// �����豸�ĺڰ�������,ִ�гɹ�����ºڰ�������������
        /// </summary>
        /// <param name="arg">����ָ�����</param>
        private bool downPayRule(TransArg arg)
        {
            CmdFileTrans trans = arg.trans;
            CommiTarget target = arg.target;
            IPAddress proxy = arg.proxy;
            CommiTarget dest = arg.dest;
            string devID = arg.devID;
            string addrst = arg.addrst;
            string attrcmd = arg.attrCmdtag;

            if (null == trans || null == proxy || null == dest || string.IsNullOrEmpty(devID) || string.IsNullOrEmpty(addrst))
                return false;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            bool success = false;

            string tpl = "ͣ����";
            DataTable tabpay = this.query.getTable("�շѱ�׼�б�", ps);
            string[,] dbpay ={ { "���ѷ�ʽ1", "�շѱ�׼1", "1", "�����շѱ�׼1" }, { "���ѷ�ʽ2", "�շѱ�׼2", "2", "�����շѱ�׼2" }, { "���ѷ�ʽ3", "�շѱ�׼3", "3", "�����շѱ�׼3" },
                               { "���ѷ�ʽ4", "�շѱ�׼4", "4", "�����շѱ�׼4" }, { "���ѷ�ʽ5", "�շѱ�׼5", "5", "�����շѱ�׼5" }, { "���ѷ�ʽ6", "�շѱ�׼6", "6", "�����շѱ�׼6" } };
            int ct = 0;
            if (null != tabpay && tabpay.Rows.Count > 0)
                ct = tabpay.Rows.Count;
            for (int j = 0; j < ct; j++)
            {
                DataRow drpay = tabpay.Rows[j];
                string ncar = tabpay.Rows[j]["����"].ToString();
                ps["����"] = ncar;
                string ntype = Convert.ToString(drpay["�Ʒѷ�ʽ"]);
                int k = -1;
                for (int m = 0; m < dbpay.GetLength(0); m++)
                    if (ntype == dbpay[m, 0])
                    {
                        k = m; break;
                    }
                if (k < 0) continue;
                //ָ����������ִ��
                if (!string.IsNullOrEmpty(attrcmd) && attrcmd.IndexOf("|" + dbpay[k, 3] + "|") < 0)
                    continue;
                DataTable tabitem = this.query.getTable(dbpay[k, 1], ps);
                if (null == tabitem || tabitem.Rows.Count < 1)
                    continue;
                string tag = "";
                switch (k)
                {
                    case 0: tag = getpaypm1(drpay, tabitem); break;
                    case 1: tag = getpaypm2(drpay, tabitem); break;
                    case 2: tag = getpaypm3(drpay, tabitem); break;
                    case 3: tag = getpaypm4(drpay, tabitem); break;
                    case 4: tag = getpaypm5(drpay, tabitem); break;
                    case 5: tag = getpaypm6(drpay, tabitem); break;
                }
                if (string.IsNullOrEmpty(tag))
                    continue;
                tag = basefun.setvaltag(tag, "�豸��ַ", addrst);
                tag = basefun.setvaltag(tag, "{���ʹ���}", ncar);
                tag = basefun.setvaltag(tag, "{��ʽ����}", dbpay[k, 2]);
                //ͨѶ�����շѲ���
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, dbpay[k, 3], tag);
                if (!success) return false;
            }
            return true;
        }

        /// <summary>
        /// ���м�¼��ȡtag��ʽ���Ʋ���
        /// </summary>
        /// <param name="dr">��¼��</param>
        /// <param name="addrst">վַ</param>
        /// <returns></returns>
        private string getctrlpm(DataRow dr, string addrst)
        {
            //@{������Ƿ����������}=0,@{�ܳ�λ��}=1000,@{�ѻ�ʱ��}.{�ѻ�����}=1,@{�ѻ�ʱ��}.{�ѻ�ʱ��}=25,
            //@{��ʱ������ѡ��}.{����}=C,@{��ʱ������ѡ��}.{��ʽ}=1,@{���ҵ�λ}=01,@{�����λ����}.{�볡}=0,@{�����λ����}.{��ʱ��}=0,
            //@{��������}=00011001,@{���п���}=00101001,@{���ڳ��߼�����}=aa,@{��λռ�ÿ�����}=01010000,
            //@{��������}=11111111,@{ͨ��ѡ��}=00 00 01 10,@{��Ч�ڱ�������}=20,@{�������ž�}=30,
            //@{�����շ�}.{�����շ�}=1,@{�����շ�}.{��Чʱ��}=30,@{�����߼�����}=AA,@{��������}.{h1}=10,
            //@{��������}.{m1}=30,@{��������}.{h2}=08,@{��������}.{m2}=30,@{������λ}=10,@{���ڳ����}=10,@{��Ƭʹ����}=7,@{������}=65535,@{������}=20

            //@{֡ͷ}=02,@{�豸��ַ}=1,@{״̬}=�����ɹ���,@{�����}=0040,@{������ַ}=1,@{ϵͳ��ʶ��}=2102803,@{ͨ������}=0,@{ϵͳ����}=0,
            //@{�û�����}=1467233176,@{������Ƿ����������}=0,@{�ܳ�λ��}=0,@�ѻ�����=1,@�ѻ�ʱ��=80,@����=0,@��ʽ=6,@{���ҵ�λ}=0A,
            //@{�����λ����}=00000000,@{��������}=10100100,@{���п���}=00010100,@{���ڳ��߼�����}=3,@{��λռ�ÿ�����}=00010000,@{��������}=11111111,
            //@{ͨ��ѡ��}=00001100,@{��Ч�ڱ�������}=30,@{�������ž�}=80,@�����շ�=0,@��Чʱ��=0,@{�����߼�����}=AA,@��1=00:00,@��2=00:00,
            //@{������λ}=0,@{���ڳ����}=0,@{��Ƭʹ����}=7,@{������}=0,@{������}=0,@{������}=0,@{У����}=82,@{֡β}=03

            //Ĭ�ϰ�������
            string type_id = Convert.ToInt32(Convert.ToInt32(dr["����Ĭ�ϳ���"]).ToString("X").Substring(0, 1), 16).ToString();
            //Ĭ�ϰ�����ʽ
            string type = Convert.ToInt32(dr["��ʱ����ʽ"]).ToString("X").Substring(1, 1);
            //�����ظ�
            string inout = "0";
            //Billy
            //DataRow dr1 = dr.Table["���Ʋ���"].rows[0];
            if (Convert.ToInt32(addrst) < 129)
            {
                if (dr.Table.Columns.Contains("�볡�ظ�"))
                {
                    inout = Convert.ToString(dr["�볡�ظ�"]);
                }
            }
            else
            {
                inout = Convert.ToString(dr["���޵ظ�"]);

            }
            string periphery = "00" + Convert.ToString(dr["Զ���ͷ2"]) + Convert.ToString(dr["Զ���ͷ1"]) + Convert.ToString(dr["�շ���ʾ��"]) + Convert.ToString(dr["������"]) + Convert.ToString(dr["���޳�λ��"]) + inout;


            string[,] colmap ={ { "{��Ч�ڱ�������}", "��Ч�ڱ���" }, { "{�������ž�}", "������" },{"{�ܳ�λ��}","�ܳ�λ��"},{"{������λ}","������λ"},
                                { "{���ڳ��߼�����}", "���ڳ��߼�" },
                                {"{�����շ�}.{�����շ�}","�Ƿ������շ�"}, {"{�����շ�}.{��Чʱ��}","��Чʱ��"},
                                {"{�����λ����}.{�볡}", "���������볡" }, { "{�����λ����}.{��ʱ��}", "��������ʱ��" }
                             };
            string[,] valmap ={ { "{�ѻ�ʱ��}.{�ѻ�����}", "1" }, { "{�ѻ�ʱ��}.{�ѻ�ʱ��}", "20" },{"{��λռ�ÿ�����}","00010000"},{"{��ʱ������ѡ��}.{����}",type_id},{"{��ʱ������ѡ��}.{��ʽ}",type},
                                { "{���ҵ�λ}", "01" }, { "{��Ƭʹ����}","70" }, {"{��������}",periphery},
                                {"{��������}.{h1}","00"}, {"{��������}.{m1}","00"}, {"{��������}.{h2}","00"}, {"{��������}.{m2}","00"}};
            string tagdata = "@�豸��ַ=" + addrst;
            tagdata = basefun.setvaltag(tagdata, "{������ַ}", addrst);
            for (int i = 0; i < colmap.GetLength(0); i++)
            {
                string val = Convert.ToString(dr[colmap[i, 1]]);
                if (true.Equals(dr[colmap[i, 1]])) val = "1";
                if (false.Equals(dr[colmap[i, 1]])) val = "0";
                if ("1" == val && ("���������볡" == colmap[i, 1] || "��������ʱ��" == colmap[i, 1]))
                    val = "10";
                tagdata = basefun.setvaltag(tagdata, colmap[i, 0], val);
            }
            for (int i = 0; i < valmap.GetLength(0); i++)
                tagdata = basefun.setvaltag(tagdata, valmap[i, 0], valmap[i, 1]);

            string v = "";

            if (true.Equals(dr["�����߼�����"]))
                tagdata = basefun.setvaltag(tagdata, "{�����߼�����}", "AA");
            else
                tagdata = basefun.setvaltag(tagdata, "{�����߼�����}", "86");


            string valk = "";
            string fx = "";
            string td = "";

            colmap = new string[,] { { "{��������}", "��������" }, { "{���п���}", "���п���" }, { "{ͨ��ѡ��}", "ͨ��ѡ��" } };

            valk = Convert.ToString(dr["Cardһ��ͨ"]) + Convert.ToString(dr["Card��ֵ��"]) + Convert.ToString(dr["Card�����"]) + Convert.ToString(dr["Card���⿨"]);
            valk += Convert.ToString(dr["Card��ѿ�"]) + Convert.ToString(dr["Card��ʱ��"]) + Convert.ToString(dr["Cardʱ�ο�"]) + Convert.ToString(dr["Card�ڿ�"]);
            v = valk.Replace("True", "1").Replace("False", "0");


            valk = Convert.ToString(dr["FXһ��ͨ"]) + Convert.ToString(dr["FX��ֵ��"]) + Convert.ToString(dr["FX�����"]) + Convert.ToString(dr["FX���⿨"]);
            valk += Convert.ToString(dr["FX��ѿ�"]) + Convert.ToString(dr["FX��ʱ��"]) + Convert.ToString(dr["FXʱ�ο�"]) + Convert.ToString(dr["FX�ڿ�"]);
            fx = valk.Replace("True", "1").Replace("False", "0");


            if (Convert.ToString(dr["A�೵"]) != "False" || Convert.ToString(dr["B�೵"]) != "False" || Convert.ToString(dr["C�೵"]) != "False" || Convert.ToString(dr["D�೵"]) != "False" || Convert.ToString(dr["E�೵"]) != "False")
            {
                valk = "000" + Convert.ToString(dr["E�೵"]) + Convert.ToString(dr["D�೵"]) + Convert.ToString(dr["C�೵"]) + Convert.ToString(dr["B�೵"]) + Convert.ToString(dr["A�೵"]);
            }
            else
            {
                valk = "11" + Convert.ToString(dr["TDһ��ͨ"]) + Convert.ToString(dr["TD��ֵ��"]) + Convert.ToString(dr["TD�����"]);
                valk += Convert.ToString(dr["TD��ѿ�"]) + Convert.ToString(dr["TDʱ�ο�"]) + Convert.ToString(dr["TD�ڿ�"]);
            }
            td = valk.Replace("True", "1").Replace("False", "0");
            tagdata = basefun.setvaltag(tagdata, colmap[1, 0], fx);
            tagdata = basefun.setvaltag(tagdata, colmap[0, 0], v);
            tagdata = basefun.setvaltag(tagdata, colmap[2, 0], td);
            return tagdata;
        }

        /// <summary>
        /// �շѱ�׼1 tag��ʽ����
        /// </summary>
        /// <param name="drpay">�Ʒѱ�׼��¼</param>
        /// <param name="tabdetail">��׼������ϸ</param>
        /// <returns>����tag��ʽЭ�����</returns>
        private string getpaypm1(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "���ʱ��", "{����1}" }, { "������", "{����2}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));

            string val = Convert.ToString(tabdetail.Rows[0]["������"]);
            tag = basefun.setvaltag(tag, "{����3}", val);
            return tag;
        }
        /// <summary>
        /// �շѱ�׼2 tag��ʽ����
        /// </summary>
        /// <param name="drpay">�Ʒѱ�׼��¼</param>
        /// <param name="tabdetail">��׼������ϸ</param>
        /// <returns>����tag��ʽЭ�����</returns>
        private string getpaypm2(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "���ʱ��", "{����1}" }, { "������", "{����3}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));
            string limit = Convert.ToString(tabdetail.Rows[0]["�޶�"]);
            if (Convert.ToString(tabdetail.Rows[0]["ÿ���޶�"]) == "False")
            {
                tag = basefun.setvaltag(tag, "{����8}", "55");
                limit = Convert.ToString(tabdetail.Rows[0]["�޶�"]);
            }
            else
                tag = basefun.setvaltag(tag, "{����8}", "00");
            tag = tag = basefun.setvaltag(tag, "{����7}", limit);
            DataRow dr = tabdetail.Rows[0];
            colmap = new string[,] { { "������ʱ��T1", "{����2}" }, { "�Ʒ�ʱ��T2", "{����4}" }, { "�Ʒ�ʱ��T3", "{����5}" }, { "������F2", "{����6}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            return tag;
        }
        /// <summary>
        /// �շѱ�׼3 tag��ʽ����
        /// </summary>
        /// <param name="drpay">�Ʒѱ�׼��¼</param>
        /// <param name="tabdetail">��׼������ϸ</param>
        /// <returns>����tag��ʽЭ�����</returns>
        private string getpaypm3(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "���ʱ��", "{����1}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));

            int k = 3;
            colmap = new string[,] { { "ѭ����", "{����2}" }, { "T1", "{����3}" }, { "F1", "{����4}" }, { "T2", "{����5}" }, { "F2", "{����6}" }, { "T3", "{����7}" }, { "F3", "{����8}" }, { "T4", "{����9}" }, { "F4", "{����10}" }, { "T5", "{����11}" }, { "F5", "{����12}" } };
            for (int r = 0; r < tabdetail.Rows.Count; r++)
            {
                DataRow dr = tabdetail.Rows[r];
                for (int i = 0; i < colmap.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            }
            return tag;
        }
        /// <summary>
        /// �շѱ�׼4 tag��ʽ����
        /// </summary>
        /// <param name="drpay">�Ʒѱ�׼��¼</param>
        /// <param name="tabdetail">��׼������ϸ</param>
        /// <returns>����tag��ʽЭ�����</returns>
        private string getpaypm4(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "���ʱ��", "{����2}" }, { "������", "{����3}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));


            colmap = new string[,] { { "ʱ����", "{����1}" }, { "ʱ���T1", "{����4}" }, { "��η�F2", "{����5}" }, { "ʱ���T2", "{����6}" }, { "��η�F2", "{����7}" }, { "ʱ���T3", "{����8}" }, { "��η�F3", "{����9}" }, { "ʱ���T4", "{����10}" }, { "��η�F4", "{����11}" }, { "ʱ���T5", "{����12}" }, { "��η�F5", "{����13}" } };
            for (int r = 0; r < tabdetail.Rows.Count; r++)
            {
                DataRow dr = tabdetail.Rows[r];
                for (int i = 0; i < colmap.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            }
            return tag;
        }
        /// <summary>
        /// �շѱ�׼5 tag��ʽ����
        /// </summary>
        /// <param name="drpay">�Ʒѱ�׼��¼</param>
        /// <param name="tabdetail">��׼������ϸ</param>
        /// <returns>����tag��ʽЭ�����</returns>
        private string getpaypm5(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "���ʱ��", "{����1}" }, { "������", "{����3}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));


            colmap = new string[,] { { "������ʱ��", "{����2}" }, { "�����Ѻ����ʱ��", "{����4}" }, { "ʱ���T3", "{����5}" }, { "ʱ���T4", "{����6}" }, { "�Ʒ�ʱ��T5", "{����7}" }, { "�Ʒ�F2", "{����8}" }, { "�Ʒ�ʱ��T6", "{����9}" }, { "�Ʒ�F3", "{����10}" } };
            for (int r = 0; r < tabdetail.Rows.Count; r++)
            {
                DataRow dr = tabdetail.Rows[r];
                for (int i = 0; i < colmap.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            }
            return tag;
        }
        /// <summary>
        /// �շѱ�׼6 tag��ʽ����
        /// </summary>
        /// <param name="drpay">�Ʒѱ�׼��¼</param>
        /// <param name="tabdetail">��׼������ϸ</param>
        /// <returns>����tag��ʽЭ�����</returns>
        private string getpaypm6(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "���ʱ��", "{����1}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));

            colmap = new string[,] { { "24Сʱ�޶�", "{����2}" }, { "�շ�1", "{����3}" }, { "�շ�2", "{����4}" }, { "�շ�3", "{����5}" }, { "�շ�4", "{����6}" }, { "�շ�5", "{����7}" }, { "�շ�6", "{����8}" }, { "�շ�7", "{����9}" }, { "�շ�8", "{����10}" }, { "�շ�9", "{����11}" }, { "�շ�10", "{����12}" }, { "�շ�11", "{����13}" }, { "�շ�12", "{����14}" }, { "�շ�13", "{����15}" }, { "�շ�14", "{����16}" }, { "�շ�15", "{����17}" }, { "�շ�16", "{����18}" }, { "�շ�17", "{����19}" }, { "�շ�18", "{����20}" }, { "�շ�19", "{����21}" }, { "�շ�20", "{����22}" }, { "�շ�21", "{����23}" }, { "�շ�22", "{����24}" }, { "�շ�23", "{����25}" }, { "�շ�24", "{����26}" } };
            for (int r = 0; r < tabdetail.Rows.Count; r++)
            {
                DataRow dr = tabdetail.Rows[r];
                for (int i = 0; i < colmap.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            }
            return tag;
        }

        #endregion


        #region �Ž�ͨѶ

        /// <summary>
        /// ͣ�����豸��ʼ�������ز���,���ºڰ�����
        /// </summary>
        /// <param name="tarsrv">��ת������</param>
        /// <param name="trans">����ָ��</param>
        /// <param name="proxy">�豸ͨѶ��������IP��ַ</param>
        /// <param name="dest">Ŀ���豸</param>
        /// <param name="drdevice">�豸��Ϣ��¼��</param>
        /// <param name="attrcmd">ִ��ָ��</param>
        /// <returns>����ͨѶ���ز����Ƿ�ɹ�</returns>
        private bool commiDeviceDoor(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, DataRow drdevice, string attrcmd)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == drdevice || string.IsNullOrEmpty(attrcmd))
                return true;

            CmdProtocol cmdP = new CmdProtocol(false);
            string devID = Convert.ToString(drdevice["ID"]);
            //��������ָ�����ʵ��
            TransArg arg = new TransArg();
            arg.trans = trans;
            arg.target = tarsrv;
            arg.proxy = proxy;
            arg.dest = dest;
            arg.devID = devID;
            arg.addrst = Convert.ToString(drdevice["վַ"]);
            arg.attrCmdtag = attrcmd;

            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            bool success = false;
            string tpl = "�Ž�";
            string valst = Convert.ToString(drdevice["վַ"]);
            //ϵͳʱ��
            DateTime dtsystem = Convert.ToDateTime(this.query.ExecuteScalar("ϵͳʱ��", ps));
            string cmdstr = ",��ʽ��,��ռ�¼,��հ�����,���ʱ��,����ʱ��,����ʱ��,���ÿ��Ʋ���,";
            string[] cmds = attrcmd.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmdstr.IndexOf("," + cmds[i] + ",") < 0)
                    continue;
                string tagdata = "@�豸��ַ=" + valst;
                if ("����ʱ��" == cmds[i])
                    tagdata = basefun.setvaltag(tagdata, "{����ʱ��}", dtsystem.ToString("yyyy-MM-dd HH:mm:ss"));
                if ("����ʱ��" == cmds[i])
                {
                    DataTable tabtime = this.query.getTable("�Ž�ʱ��", ps);
                    if (null == tabtime || tabtime.Rows.Count < 1)
                        continue;
                    foreach (DataRow dr in tabtime.Rows)
                    {
                        tagdata = this.getdoortime(dr);
                        tagdata = basefun.setvaltag(tagdata, "�豸��ַ", valst);
                        success = this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, cmds[i], tagdata);
                        if (!success) return false;
                    }
                    continue;
                }
                if ("���ÿ��Ʋ���" == cmds[i])
                {
                    success = this.downdoorctrlpm(arg);
                    if (!success) return false;
                    continue;
                }
                success = this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, cmds[i], tagdata);
                //��ʽ��ʱ�豸��2.5s��Ӧ��
                if ("��ʽ��" == cmds[i])
                {
                    this.query.ExecuteNonQuery("�Ž���¼λ������", ps, ps, ps);
                    System.Threading.Thread.Sleep(3500);
                    continue;
                }
                if (!success) return false;
                if ("��ռ�¼" == cmds[i])
                    this.query.ExecuteNonQuery("�Ž���¼λ������", ps, ps, ps);
            }
            //������������
            this.query.ExecuteNonQuery("�豸���ر�־����", ps, ps, ps);

            //���úڰ�����,���Ž���������500�������������������
            if (attrcmd.IndexOf("|��ʽ��|") > -1 || attrcmd.IndexOf("|��հ�����|") > -1)
                this.query.ExecuteNonQuery("�����豸����", ps, ps, ps);
            else if (attrcmd.IndexOf("|���ذ�����|") > -1)
            {
                DataTable tab = this.query.getTable("�Ž�������", ps);
                if (null != tab && tab.Rows.Count > 500)
                {
                    attrcmd += "|��հ�����|";
                    this.query.ExecuteNonQuery("�����豸����", ps, ps, ps);
                }
            }
            success = this.downdoorCardList(arg);
            return success;
        }

        /// <summary>
        /// ���ͨѶĿ���Ƿ������
        /// </summary>
        /// <param name="tarsrv">��ת����Ŀ�꣬�ڴ���ʱ��ת��ͨѶ</param>
        /// <param name="dest">ͨѶĿ��</param>
        /// <returns>�����Ƿ������</returns>
        private bool testConnect(CommiTarget tarsrv, CommiTarget dest)
        {
            bool rtn = false;
            if (null == tarsrv || null == dest)
                return rtn;
            if (CommiType.SerialPort != dest.ProtocolType)
                rtn = CommiManager.GlobalManager.TestConnect(dest);
            else
                rtn = CommiManager.GlobalManager.TestConnect(tarsrv);
            return rtn;
        }

        /// <summary>
        /// �Ž�ʱ��  tag��ʽ����
        /// </summary>
        /// <param name="drtime">ʱ�μ�¼</param>
        /// <returns>����tag��ʽЭ�����</returns>
        private string getdoortime(DataRow drtime)
        {
            if (null == drtime)
                return "";
            string[,] colmap ={ { "{ʱ�κ�}", "ʱ�κ�" }, { "{���ڿ���}.{һ}", "����һ" }, { "{���ڿ���}.{��}", "���ڶ�" },
                        { "{���ڿ���}.{��}", "������" }, { "{���ڿ���}.{��}", "������" }, { "{���ڿ���}.{��}", "������" }, 
                        { "{���ڿ���}.{��}", "������" }, { "{���ڿ���}.{��}", "������" },
                        {"{��ʼʱ��1}","��ʼʱ��1"},{"{��ֹʱ��1}","����ʱ��1"},{"{��ʼʱ��2}","��ʼʱ��2"},{"{��ֹʱ��2}","����ʱ��2"},
                        {"{��ʼʱ��3}","��ʼʱ��3"},{"{��ֹʱ��3}","����ʱ��3"}};
            string tag = "";
            for (int i = 0; i < colmap.GetLength(0); i++)
            {
                if (DBNull.Value == drtime[colmap[i, 1]])
                    return "";
                if (true.Equals(drtime[colmap[i, 1]]))
                    tag = basefun.setvaltag(tag, colmap[i, 0], "1");
                else if (false.Equals(drtime[colmap[i, 1]]))
                    tag = basefun.setvaltag(tag, colmap[i, 0], "0");
                else
                    tag = basefun.setvaltag(tag, colmap[i, 0], Convert.ToString(drtime[colmap[i, 1]]));
            }
            return tag;
        }

        /// <summary>
        /// ���ؿ��Ʋ���
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool downdoorctrlpm(TransArg arg)
        {
            CmdFileTrans trans = arg.trans;
            CommiTarget target = arg.target;
            IPAddress proxy = arg.proxy;
            CommiTarget dest = arg.dest;
            string devID = arg.devID;
            string addrst = arg.addrst;
            string attrcmd = arg.attrCmdtag;

            if (null == trans || null == proxy || null == dest || string.IsNullOrEmpty(devID) || string.IsNullOrEmpty(addrst))
                return false;
            CommiManager mgr = CommiManager.GlobalManager;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            DataTable tabpm = this.query.getTable("�Ž����Ʋ���", ps);
            if (null == tabpm || tabpm.Rows.Count < 1)
                return true;
            bool success = false;
            string tpl = "�Ž�";
            string tag = "@�豸��ַ=" + addrst;
            //�����Ų���
            string[,] colmapdoor ={ { "{���Ʒ�ʽ}", "���Ʒ�ʽ" }, { "{��ʱ}", "������ʱ" }, { "{�ź�}", "�ź�" } };
            foreach (DataRow dr in tabpm.Rows)
            {
                tag = "@�豸��ַ=" + addrst;
                string mode = Convert.ToString(dr["���Ʒ�ʽ"]);
                switch (mode)
                {
                    case "����": mode = "1"; break;
                    case "����": mode = "1"; break;
                    default: mode = "3"; break;
                }
                tag = basefun.setvaltag(tag, "{���Ʒ�ʽ}", mode);
                tag = basefun.setvaltag(tag, "{��ʱ}", Convert.ToString(dr["������ʱ"]));
                tag = basefun.setvaltag(tag, "{�ź�}", Convert.ToString(Convert.ToInt16(dr["�ź�"]) + 1));
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "���ÿ��Ʋ���", tag);
                if (!success) return false;
            }
            //���ñ�������
            string[,] colmapalarm ={ { "{����״̬}.{��}", "�𾯸澯" }, { "{����״̬}.{��Чˢ��}", "�Ƿ�ˢ���澯" },
                        { "{����״̬}.{��������}", "�����澯" }, { "{����״̬}.{�Ƿ�����}", "�Ƿ����Ÿ澯" },
                        { "{����״̬}.{��ʱ}", "��ʱ���Ÿ澯" }, { "{����״̬}.{в��}", "в�ȱ���" },
                        {"{���ų�ʱʱ��}","���ų�ʱ"}, {"{�������ʱ��}","����������ʱ"} };
            DataRow drpm = tabpm.Rows[0];
            tag = "@�豸��ַ=" + addrst;
            for (int c = 0; c < colmapalarm.GetLength(0); c++)
            {
                string val = Convert.ToString(drpm[colmapalarm[c, 1]]);
                if (true.Equals(drpm[colmapalarm[c, 1]]))
                    val = "1";
                else if (false.Equals(drpm[colmapalarm[c, 1]]))
                    val = "0";
                tag = basefun.setvaltag(tag, colmapalarm[c, 0], val);
            }
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "���ñ���", tag);
            if (!success) return false;
            //�����¼���־��������¼ָ��
            string[,] colmaplog ={ { "��¼���Ű�ť", "2D" }, { "��¼�澯�¼�", "2E" }, { "��¼�Ŵ��¼�", "DC" } };
            for (int m = 0; m < colmaplog.GetLength(0); m++)
            {
                tag = "@�豸��ַ=" + addrst;
                tag = basefun.setvaltag(tag, "{������}", colmaplog[m, 1]);
                tag = basefun.setvaltag(tag, "{����}", "0");
                if (true.Equals(drpm[colmaplog[m, 0]]))
                    tag = basefun.setvaltag(tag, "{����}", "1");
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "�����¼���־", tag);
                if (!success) return false;
            }
            //���÷�Ǳ����Ǳָ����Ҫ��ȷ��ָ��
            tag = "@�豸��ַ=" + addrst;
            string unhide = Convert.ToString(drpm["��Ǳ���"]);
            string unhideOK = "00";
            switch (unhide)
            {
                case "���÷�Ǳ": unhide = "0"; unhideOK = "0A"; break;
                case "������Ǳ": unhide = "1"; unhideOK = "FA"; break;
                case "������Ǳ": unhide = "2"; unhideOK = "FA"; break;
                case "һ������": unhide = "3"; unhideOK = "7E"; break;
                case "һ�����": unhide = "4"; unhideOK = "FE"; break;
                default: unhide = "0"; unhideOK = "00"; break;
            }
            tag = basefun.setvaltag(tag, "{��Ǳ��}", unhide);
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "���÷�Ǳ", tag);
            if (!success) return false;
            tag = basefun.setvaltag(tag, "{��Ǳ��}", unhideOK);
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "���÷�Ǳȷ��", tag);
            if (!success) return false;
            //���û����������跢���ָ�����
            tag = "@�豸��ַ=" + addrst;
            string[] cmds = new string[] { "001E", "001F", "0020", "0021" };
            string locklink = Convert.ToString(drpm["�������"]);
            switch (locklink)
            {
                case "���û���": locklink = "00"; break;
                case "��������": locklink = "01"; break;
                case "���Ż���":
                    locklink = "71";
                    cmds = new string[] { "001E", "001F", "0020" };
                    break;
                case "ȫ������": locklink = "F1"; break;
                default: locklink = "00"; break;
            }
            tag = basefun.setvaltag(tag, "{����}", locklink);
            for (int i = 0; i < cmds.Length; i++)
            {
                tag = basefun.setvaltag(tag, "{������}", cmds[i]);
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "���û���", tag);
                if (!success) return false;
            }
            //������չ�����
            DataTable tabext = this.query.getTable("�Ž���չ�����", ps);
            if (null == tabext || tabext.Rows.Count < 1)
                return true;
            //������չ��ָ�ÿ����չ��ֱ��ĸ�ָ��
            string[,] codefun ={
                                 { "98", "9A", "9B", "9C", "9D" }, { "9E", "A0", "A1", "A2", "A3" },
                                 { "A4", "A6", "A7", "A8", "A9" }, { "AA", "AC", "AD", "AE", "AF" }  };
            colmapdoor = new string[,]{ { "{��λ}.{��1}", "һ����" }, { "{��λ}.{��2}", "������" }, 
                                 { "{��λ}.{��3}", "������" }, { "{��λ}.{��4}", "�ĺ���" } };
            colmapalarm = new string[,]{ { "{����״̬}.{��}", "�𾯸澯" }, { "{����״̬}.{��Чˢ��}", "�Ƿ�ˢ���澯" },
                        { "{����״̬}.{��������}", "�����澯" }, { "{����״̬}.{�Ƿ�����}", "�Ƿ����ű���" },
                        { "{����״̬}.{��ʱ}", "��ʱ���Ÿ澯" }, { "{����״̬}.{в��}", "в�ȱ���" } };
            string strtime = Convert.ToString(drpm["����������ʱ"]);
            if (string.IsNullOrEmpty(strtime))
                strtime = "0";
            long latetime = Convert.ToInt64(strtime);
            string latestr = Convert.ToString(latetime, 16).PadLeft(4, '0');
            latestr = latestr.Substring(latestr.Length - 4);
            foreach (DataRow drext in tabext.Rows)
            {
                string strbh = Convert.ToString(drext["���"]);
                if (string.IsNullOrEmpty(strbh))
                    continue;
                int index = Convert.ToInt32(strbh);
                if (index < 0 || index > 3)
                    continue;
                //�����¼�Դ
                tag = "@�豸��ַ=" + addrst;
                tag = basefun.setvaltag(tag, "{������}", codefun[index, 0]);
                for (int i = 0; i < colmapdoor.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmapdoor[i, 0], true.Equals(drext[colmapdoor[i, 1]]) ? "1" : "0");
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "��չ���¼�Դ", tag);
                if (!success) return false;
                //���ñ���
                tag = "@�豸��ַ=" + addrst;
                tag = basefun.setvaltag(tag, "{������}", codefun[index, 1]);
                for (int i = 0; i < colmapalarm.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmapalarm[i, 0], true.Equals(drext[colmapalarm[i, 1]]) ? "1" : "0");
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "��չ�屨������", tag);
                if (!success) return false;
                //������ʱ��λ
                tag = "@�豸��ַ=" + addrst;
                tag = basefun.setvaltag(tag, "{������}", codefun[index, 3]);
                tag = basefun.setvaltag(tag, "{��ʱ}", latestr.Substring(2));
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "��չ����ʱ��λ", tag);
                if (!success) return false;
                //������ʱ��λ
                tag = "@�豸��ַ=" + addrst;
                tag = basefun.setvaltag(tag, "{������}", codefun[index, 4]);
                tag = basefun.setvaltag(tag, "{��ʱ}", latestr.Substring(0, 2));
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "��չ����ʱ��λ", tag);
                if (!success) return false;
            }
            return true;
        }

        /// <summary>
        /// �����豸�ĺڰ�������,ִ�гɹ�����ºڰ�������������
        /// </summary>
        /// <param name="arg">����ָ�����</param>
        private bool downdoorCardList(TransArg arg)
        {
            CmdFileTrans trans = arg.trans;
            CommiTarget target = arg.target;
            IPAddress proxy = arg.proxy;
            CommiTarget dest = arg.dest;
            string devID = arg.devID;
            string addrst = arg.addrst;
            string attrcmd = arg.attrCmdtag;

            if (null == trans || null == proxy || null == dest || string.IsNullOrEmpty(devID) || string.IsNullOrEmpty(addrst))
                return false;
            if (string.IsNullOrEmpty(attrcmd) || (attrcmd.IndexOf("|ɾ��������|") < 0 && attrcmd.IndexOf("|���ذ�����|") < 0))
                return true;
            CommiManager mgr = CommiManager.GlobalManager;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameValueCollection data = new NameValueCollection();
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            //ͬ��ͨѶ�ȴ�ʱ��15��
            bool success = true, isconn = true;
            string cardsdel = "", cardswh = "";
            DataTable tab = this.query.getTable("�Ž�������", ps);
            string tpl = "�Ž�", cmd = "ɾ��������";
            string tagdata = "@�豸��ַ=" + addrst;
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|ɾ��������|") > -1)
                for (int i = 0; isconn && i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    string cardnum = Convert.ToString(dr["����"]);
                    tagdata = basefun.setvaltag(tagdata, "{����}", cardnum);
                    //�����ֶα����˶�Ӧ�ź�(��������)
                    string dstr = Convert.ToString(dr["��Ȩ"]);
                    if (string.IsNullOrEmpty(dstr))
                        continue;
                    string[] bh = dstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    bool rtn = true;
                    for (int b = 0; b < bh.Length; b++)
                    {
                        //�ź���1��ʼ
                        bh[b] = Convert.ToString(Convert.ToInt32(bh[b]) + 1);
                        tagdata = basefun.setvaltag(tagdata, "{�ź�}", bh[b]);
                        rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                        if (!rtn) break;
                    }
                    success = success && rtn;
                    if (!rtn)
                    {
                        isconn = testConnect(target, dest);
                        continue;
                    }
                    cardsdel += "," + cardnum;
                    //ps["״̬"] = "��";
                    //ps["����"] = Convert.ToString(dr["����"]);
                    //this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                }
            tab = this.query.getTable("�Ž�������", ps);
            cmd = "���ذ�����";
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|���ذ�����|") > -1)
                for (int i = 0; isconn && i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    string cardnum = Convert.ToString(dr["����"]);
                    string[,] colmap ={ { "{����}", "����" }, { "{��ʼ����}", "��������" }, { "{��ֹ����}", "��Ч����" }, { "{ʱ��}", "ʱ��" },
                                        { "{����}", "����" }, { "{���}", "�û����" } };
                    for (int j = 0; j < colmap.GetLength(0); j++)
                        tagdata = basefun.setvaltag(tagdata, colmap[j, 0], Convert.ToString(dr[colmap[j, 1]]));
                    tagdata = basefun.setvaltag(tagdata, "{����}", "FF");
                    ps["״̬"] = "��";
                    ps["����"] = cardnum;

                    cmd = "���ذ�����";
                    bool rtn = true;
                    if ("���ڻ�" == Convert.ToString(dr["����������"]))
                    {
                        cmd = "���ؿ�������";
                        rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                        success = success && rtn;
                        if (!rtn)
                        {
                            isconn = testConnect(target, dest);
                            continue;
                        }
                        cardswh += "," + cardnum;
                        continue;
                        //this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                        //continue;
                    }
                    //{ "{�ź�}", "��������" },
                    string dstr = Convert.ToString(dr["��Ȩ"]);
                    if (string.IsNullOrEmpty(dstr))
                        continue;
                    string[] bh = dstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (int b = 0; b < bh.Length; b++)
                    {
                        //�ź���1��ʼ
                        bh[b] = Convert.ToString(Convert.ToInt32(bh[b]) + 1);
                        tagdata = basefun.setvaltag(tagdata, "{�ź�}", bh[b]);
                        rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                        if (!rtn) break;
                    }
                    success = success && rtn;
                    if (!rtn)
                    {
                        isconn = testConnect(target, dest);
                        continue;
                    }
                    cardswh += "," + cardnum;
                    //this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                }
            if (success)
            {
                ps["�豸ID"] = devID;
                this.query.ExecuteNonQuery("�豸�ڰ�������־����", ps, ps, ps);
            }
            else
            {
                //���ز��ɹ�ʱ���ֱ���ºڰ�������־
                string[] cards = cardsdel.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string c in cards)
                {
                    ps["״̬"] = "��";
                    ps["����"] = c;
                    this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                }
                cards = cardswh.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string c in cards)
                {
                    ps["״̬"] = "��";
                    ps["����"] = c;
                    this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                }
            }
            return success;
        }

        #endregion

        #region ����ͨѶ

        /// <summary>
        /// �����豸��ʼ�������ز���,���ºڰ�����
        /// </summary>
        /// <param name="tarsrv">��ת������</param>
        /// <param name="trans">����ָ��</param>
        /// <param name="proxy">�豸ͨѶ��������IP��ַ</param>
        /// <param name="dest">Ŀ���豸</param>
        /// <param name="drdevice">�豸��Ϣ��¼��</param>
        /// <param name="attrcmd">ִ��ָ��</param>
        /// <returns>����ͨѶ���ز����Ƿ�ɹ�</returns>
        private bool commiDeviceEatery(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, DataRow drdevice, string attrcmd)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == drdevice || string.IsNullOrEmpty(attrcmd))
                return true;
            CmdProtocol cmdP = new CmdProtocol(false);
            string devID = Convert.ToString(drdevice["ID"]);
            //��������ָ�����ʵ��
            TransArg arg = new TransArg();
            arg.trans = trans;
            arg.target = tarsrv;
            arg.proxy = proxy;
            arg.dest = dest;
            arg.devID = devID;
            arg.addrst = Convert.ToString(drdevice["վַ"]);
            arg.attrCmdtag = attrcmd;

            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            bool success = false;
            string tpl = "����";
            string valst = Convert.ToString(drdevice["վַ"]);
            //ϵͳʱ��
            DateTime dtsystem = Convert.ToDateTime(this.query.ExecuteScalar("ϵͳʱ��", ps));
            string cmdstr = ",��ʽ��,��ռ�¼,��ղ�������,��պ�����,���ʱ��,����ʱ��,����ʱ��,���ÿ��Ʋ���,";
            string[] cmds = attrcmd.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmdstr.IndexOf("," + cmds[i] + ",") < 0)
                    continue;
                string tagdata = "@�豸��ַ=" + valst;
                if ("����ʱ��" == cmds[i])
                    tagdata = basefun.setvaltag(tagdata, "{����ʱ��}", dtsystem.ToString("yyyy-MM-dd HH:mm:ss"));
                if ("����ʱ��" == cmds[i])
                {
                    success = this.downeaterytime(arg);
                    if (!success) return false;
                    continue;
                }
                if ("���ÿ��Ʋ���" == cmds[i])
                {
                    success = this.downeateryctrlpm(arg);
                    if (!success) return false;
                    continue;
                }
                success = this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, cmds[i], tagdata);
                //��ʽ��ʱ�豸��2.5s��Ӧ��
                if ("��ʽ��" == cmds[i])
                {
                    System.Threading.Thread.Sleep(3500);
                    continue;
                }
                if (!success) return false;
            }
            //������������
            this.query.ExecuteNonQuery("�豸���ر�־����", ps, ps, ps);

            //���úڰ�����
            if (attrcmd.IndexOf("|��ʽ��|") > -1 || attrcmd.IndexOf("|��հ�����|") > -1)
                this.query.ExecuteNonQuery("�����豸����", ps, ps, ps);
            success = this.downeateryCardList(arg);
            if (!success) return false;
            //�������ѿ�û�н��յ��Ĳ�����������δ���صĲ���
            success = this.downeaterySubsidy(arg);
            return success;
        }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool downeaterytime(TransArg arg)
        {
            CmdFileTrans trans = arg.trans;
            CommiTarget target = arg.target;
            IPAddress proxy = arg.proxy;
            CommiTarget dest = arg.dest;
            string devID = arg.devID;
            string addrst = arg.addrst;
            string attrcmd = arg.attrCmdtag;

            if (null == trans || null == proxy || null == dest || string.IsNullOrEmpty(devID) || string.IsNullOrEmpty(addrst))
                return false;
            CommiManager mgr = CommiManager.GlobalManager;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            //ͬ��ͨѶ�ȴ�ʱ��15��
            bool success = false;

            DataTable tab = this.query.getTable("����ʱ������", ps);
            string tpl = "����", cmd = "����ʱ������";
            string[,] colmap ={ { "{ʱ�������}", "���" }, { "{��ʼʱ��}", "��ʼ" }, { "{����ʱ��}", "����" },
                                { "{�����޶�}", "ʱ���޶�" }, { "{�����޴�}", "ʱ���޴�" } };
            string tagdata = "@�豸��ַ=" + addrst;
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                DataRow drtime = tab.Rows[i];
                for (int c = 0; c < colmap.GetLength(0); c++)
                {
                    string val = "0";
                    if (DBNull.Value != drtime[colmap[c, 1]])
                        val = Convert.ToString(drtime[colmap[c, 1]]);
                    tagdata = basefun.setvaltag(tagdata, colmap[c, 0], val);
                }
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                if (!success) return false;
            }
            cmd = "����ʱ��";
            colmap = new string[,]{ { "{����}", "�������" }, 
                                { "{ʱ��1}", "����" }, { "{ʱ��2}", "���" }, { "{ʱ��3}", "���" }, { "{ʱ��4}", "�����" }, 
                                { "{ʱ��5}", "����" }, { "{ʱ��6}", "ҹ��" },{ "{ʱ��7}", "�Ӳ�1" }, { "{ʱ��8}", "�Ӳ�2" },
                                { "{�����޶�}", "ÿ���޶�" }, { "{��������}", "ÿ���޴�" } };
            tagdata = "@�豸��ַ=" + addrst;
            tab = this.query.getTable("������ʱ��", ps);
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                DataRow drtime = tab.Rows[i];
                for (int c = 0; c < colmap.GetLength(0); c++)
                {
                    string val = "0";
                    if (DBNull.Value != drtime[colmap[c, 1]])
                        val = Convert.ToString(drtime[colmap[c, 1]]);
                    tagdata = basefun.setvaltag(tagdata, colmap[c, 0], val);
                }
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                if (!success) return false;
            }
            return true;
        }

        /// <summary>
        /// ���ؿ��Ʋ���
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private bool downeateryctrlpm(TransArg arg)
        {
            CmdFileTrans trans = arg.trans;
            CommiTarget target = arg.target;
            IPAddress proxy = arg.proxy;
            CommiTarget dest = arg.dest;
            string devID = arg.devID;
            string addrst = arg.addrst;
            string attrcmd = arg.attrCmdtag;

            if (null == trans || null == proxy || null == dest || string.IsNullOrEmpty(devID) || string.IsNullOrEmpty(addrst))
                return false;
            CommiManager mgr = CommiManager.GlobalManager;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            bool success = false;
            //��ղ˵�
            string tpl = "����", cmd = "������Ѳ���";
            string tagdata = "@�豸��ַ=" + addrst;
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
            if (!success) return false;
            //���ؿ��Ʋ���
            DataTable tab = this.query.getTable("���ѿ��Ʋ���", ps);
            if (null == tab || tab.Rows.Count < 1)
                return false;
            DataRow drctrl = tab.Rows[0];
            string[,] colmap ={ { "{ͨ������}", "ͨѶ����" }, { "{ϵͳ����}", "ϵͳ����" }, { "{�û�����}", "�û�����" }, { "{��Ƭ����}", "��Ƭ����" },
                    { "{��¼����}", "�ռ�澯" }, { "{����������}", "����������" }, { "{ȡ����������}", "ȡ����������" },
                    { "{���������޶�}", "�����޶�" }, { "{��������}", "���޶�" }, { "{������ʱ}", "������ʱ" }, { "{��ʾ��ʱ}", "��ʾ��ʱ" }, 
                    { "{��������}.{��¼�����������}", "�������" }, { "{��������}.{ʱ������}", "ʱ������" }, { "{��������}.{����Աƾ��ȡ���ϱʽ���}", "ƾ��ȡ������" },
                    { "{��������}.{����Աֱ��ȡ���ϱʽ���}", "ֱ��ȡ������" }, { "{��������}.{�����������ò���}", "�����ò���" }, 
                    { "{��������}.{��Ȩ���ɳ�ʼ���豸}", "�ɳ�ʼ��" }, 
                    { "{���ѿ���}.{����ֱ�ӿ۷�}", "����ֱ�ӿ۷�" }, { "{���ѿ���}.{��������}", "��������" }, { "{���ѿ���}.{��ӡ��¼}", "��ӡ��¼" }, 
                    { "{���ѿ���}.{���ֽ�����}", "���ֽ�����" }, { "{���ѿ���}.{����������}", "����������" }, { "{���ѿ���}.{�ɳ���}", "�ɳ���" }, 
                    { "{���ѿ���}.{ƾ������}", "ƾ������" }, 
                    { "{�����޶�}", "�����޶�" }, { "{������ʽ}", "������ʽ" }, { "{ʳ�ñ��}", "���" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tagdata = basefun.setvaltag(tagdata, colmap[i, 0], Convert.ToString(drctrl[colmap[i, 1]]));
            cmd = "���ÿ��Ʋ���";
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
            if (!success) return false;
            //���ز˵�
            tagdata = "@�豸��ַ=" + addrst;
            tab = this.query.getTable("���Ѽ۸����", ps);
            colmap = new string[,] { { "{����}", "���" }, { "{�۸�}", "�۸�" } };
            cmd = "�������Ѳ���";
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                DataRow drmenu = tab.Rows[i];
                for (int c = 0; c < colmap.GetLength(0); c++)
                {
                    string val = Convert.ToString(drmenu[colmap[c, 1]]);
                    tagdata = basefun.setvaltag(tagdata, colmap[c, 0], val);
                }
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                if (!success) return false;
            }
            return true;
        }

        /// <summary>
        /// �����豸�ĺڰ�������,ִ�гɹ�����ºڰ�������������
        /// </summary>
        /// <param name="arg">����ָ�����</param>
        private bool downeateryCardList(TransArg arg)
        {
            CmdFileTrans trans = arg.trans;
            CommiTarget target = arg.target;
            IPAddress proxy = arg.proxy;
            CommiTarget dest = arg.dest;
            string devID = arg.devID;
            string addrst = arg.addrst;
            string attrcmd = arg.attrCmdtag;

            if (null == trans || null == proxy || null == dest || string.IsNullOrEmpty(devID) || string.IsNullOrEmpty(addrst))
                return false;
            CommiManager mgr = CommiManager.GlobalManager;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameValueCollection data = new NameValueCollection();
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            //ͬ��ͨѶ�ȴ�ʱ��15��
            bool success = true;
            DataTable tab = this.query.getTable("����������", ps);
            string tpl = "����", cmd = "ɾ��������";
            string tagdata = "@�豸��ַ=" + addrst;
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|ɾ��������|") > -1)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    tagdata = basefun.setvaltag(tagdata, "{����}", Convert.ToString(dr["����"]));
                    bool rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                    success = success && rtn;
                    if (!rtn)
                    {
                        if (testConnect(target, dest))
                            return false;
                        continue;
                    }
                    ps["״̬"] = "��";
                    ps["����"] = Convert.ToString(dr["����"]);
                    this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                }
            tab = this.query.getTable("���Ѻ�����", ps);
            cmd = "���غ�����";
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|���غ�����|") > -1)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    tagdata = basefun.setvaltag(tagdata, "{����}", Convert.ToString(dr["����"]));
                    bool rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                    success = success && rtn;
                    if (!rtn)
                    {
                        if (testConnect(target, dest))
                            return false;
                        continue;
                    }
                    ps["״̬"] = "��";
                    ps["����"] = Convert.ToString(dr["����"]);
                    this.query.ExecuteNonQuery("�������ر�־����", ps, ps, ps);
                }
            return success;
        }
        /// <summary>
        /// ���ղ�����û�����صĲ���,��ղ�����,�ϲ���δ���ز���һ������
        /// </summary>
        /// <param name="arg">����ָ�����</param>
        /// <returns></returns>
        private bool downeaterySubsidy(TransArg arg)
        {
            CmdFileTrans trans = arg.trans;
            CommiTarget target = arg.target;
            IPAddress proxy = arg.proxy;
            CommiTarget dest = arg.dest;
            string devID = arg.devID;
            string addrst = arg.addrst;
            string attrcmd = arg.attrCmdtag;

            if (null == trans || null == proxy || null == dest || string.IsNullOrEmpty(devID) || string.IsNullOrEmpty(addrst))
                return false;
            //�ж��Ƿ����ز���
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|���ز�������|") < 0)
                return true;
            CommiManager mgr = CommiManager.GlobalManager;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameObjectList ps = new NameObjectList();
            ps["�豸ID"] = devID;
            string tpl = "����", cmd = "ȡ��ǰ������¼";
            string tag = "@�豸��ַ=" + addrst;
            //������ȡ������¼
            string msg = this.getResponse(target, trans, proxy, dest, cmdP, tpl, cmd, tag);
            if (string.IsNullOrEmpty(msg))
                return false;
            string cardnum = basefun.valtag(msg, "{����}");
            cmd = "ȡ��һ��������¼";
            while (!string.IsNullOrEmpty(cardnum) && "16777215" != cardnum && "0" != cardnum)
            {
                NameObjectList pm = createParam(msg);
                bool rtn = this.query.ExecuteNonQuery("���ѽ��ղ���", pm, pm, pm);
                if (!rtn) return false;
                msg = this.getResponse(target, trans, proxy, dest, cmdP, tpl, cmd, tag);
                cardnum = basefun.valtag(msg, "{����}");
            }
            //��ȡû�н��յĲ�����¼�����պϲ���������¼
            DataTable tab = this.query.getTable("���ѻ��ղ���", ps);
            if (null == tab) return false;
            cmd = "��ѯ�û��������";
            string[,] cols = { { "����", "{�û����}" }, { "��Ƭ���к�", "{��Ƭ�������}" } };
            string[,] pn = { { "����", "{�û����}" }, { "��Ƭ���к�", "{��Ƭ�������}" }, { "�������", "{�������}" }, { "����״̬", "{����״̬}" } };
            bool success = true;
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                DataRow dr = tab.Rows[i];
                for (int c = 0; c < cols.GetLength(0); c++)
                    tag = basefun.setvaltag(tag, cols[c, 1], Convert.ToString(dr[cols[c, 0]]));
                msg = this.getResponse(target, trans, proxy, dest, cmdP, tpl, cmd, tag);
                if (string.IsNullOrEmpty(msg))
                {
                    success = false;
                    break;
                }
                if ("true" != basefun.valtag(msg, "Success"))
                    continue;
                NameObjectList pm = new NameObjectList();
                for (int p = 0; p < pn.GetLength(0); p++)
                    pm[pn[p, 0]] = basefun.valtag(msg, pn[p, 1]);
                this.query.ExecuteNonQuery("���ѻ��ղ���", pm, pm, pm);
            }
            if (!success) return false;
            tab = this.query.getTable("�������ز���", ps);
            cmd = "���ز�������";
            tag = "@�豸��ַ=" + addrst;
            cols = new string[,] { { "����", "{�û����}" }, { "��Ƭ���к�", "{��Ƭ�������}" }, { "������ֵ", "{�������}" } };
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                DataRow dr = tab.Rows[i];
                for (int c = 0; c < cols.GetLength(0); c++)
                    tag = basefun.setvaltag(tag, cols[c, 1], Convert.ToString(dr[cols[c, 0]]));
                bool suc = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tag);
                if (!suc)
                {
                    success = suc;
                    continue;
                }
                ps["����"] = Convert.ToString(dr["����"]);
                this.query.ExecuteNonQuery("�������ز���", ps, ps, ps);
            }
            return success;
        }
        #endregion

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

        private static Regex regex = new Regex(@"@([\u4E00-\u9FA5\s\w\{\}]+)=", RegexOptions.Compiled);
        /// <summary>
        /// ����tag��Ǵ�������
        /// </summary>
        /// <param name="tag">����������tag���</param>
        /// <returns>�����½����Ĳ���</returns>
        public static NameObjectList createParam(string tag)
        {
            MatchCollection matchs = regex.Matches(tag);
            NameObjectList ps = new NameObjectList();
            foreach (Match m in matchs)
            {
                string key = m.Groups[1].Value;
                string k = key;
                if (key.StartsWith("{") && key.EndsWith("}"))
                    k = key.Substring(1, key.Length - 2);
                ps[k] = basefun.valtag(tag, key);
            }
            return ps;
        }

    }
}