using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using Granity.CardOneCommi;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.communications;
using System.Data;
using Granity.winTools;
using System.Diagnostics;
using System.Threading;

namespace Granity.commiServer
{
    /// <summary>
    /// �Ž��豸
    /// </summary>
    public class DeviceDoor : DeviceBase
    {
        /// <summary>
        /// ���Ѽ�ص�Ԫ
        /// </summary>
        private UnitItem unitItem = null;
        /// <summary>
        /// ���ݲ���
        /// </summary>
        private QueryDataRes query = null;
        /// <summary>
        /// ��¼λ��
        /// </summary>
        private int posRecord = 0;
        /// <summary>
        /// �豸���ͣ����ڻ�������˫��˫��˫�����ŵ���
        /// </summary>
        private string devtype = "���ڻ�";
        /// <summary>
        /// �ɼ�ָ��
        /// </summary>
        private CmdProtocol cmdGather = null;
        /// <summary>
        /// ͬ���ȴ�ʱ��,Ĭ��500������Ϊ��ʱʧ��
        /// </summary>
        private TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 500);
        /// <summary>
        /// �ɼ�����,Ĭ��Ѳ������2��
        /// </summary>
        private TimeSpan tsinv = new TimeSpan(0, 0, 2);
        /// <summary>
        /// �ж��Ƿ�æ״̬��ʱ����,Ĭ��2�����ڼ�¼��������60��,��Ϊ���ڷ�æ״̬
        /// </summary>
        private TimeSpan tsbusy = new TimeSpan(0, 2, 0);
        /// <summary>
        /// ��ǰѲ��״̬
        /// </summary>
        private NameValueCollection tagInfos = new NameValueCollection();

        public DeviceDoor():base()
        {
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "�Ž����");
            this.query = new QueryDataRes(this.unitItem.DataSrcFile);
        }
        /// <summary>
        /// ����ͨѶ�豸
        /// </summary>
        /// <param name="devid">�豸ID</param>
        /// <param name="target">ͨѶĿ��</param>
        /// <param name="station">վַ</param>
        public override void SetDevice(CommiManager mgr, CommiTarget target, string devid, int station)
        {
            if (null == mgr || string.IsNullOrEmpty(devid) || null == target || station < 1)
                return;
            this.commimgr = mgr;
            this.target = target;
            this.devid = devid;
            this.station = station;
            NameObjectList ps = new NameObjectList();
            ps["����ID"] = devid;
            DataTable tab = this.query.getTable("�Ž��豸", ps);
            if (null == tab || tab.Rows.Count < 1)
                return;
            object val = tab.Rows[0]["��¼λ��"];
            if (null == val || DBNull.Value == val)
                val = 0;
            this.posRecord = Convert.ToInt32(val);
            val = tab.Rows[0]["����������"];
            this.devtype = Convert.ToString(val);
            if (string.IsNullOrEmpty(this.devtype))
                this.devtype = "���ڻ�";
        }
        /// <summary>
        /// �����ɼ�
        /// </summary>
        public override void StartGather()
        {
            if (null == this.commimgr || null == this.target || string.IsNullOrEmpty(this.devid) || this.station < 1)
                return;
            if (null != cmdGather)
                return;
            cmdGather = new CmdProtocol(false);
            cmdGather.TimeLimit = TimeSpan.MaxValue;
            cmdGather.TimeFailLimit = TimeSpan.MaxValue;
            cmdGather.FailProAf = FailAftPro.Ignor;
            cmdGather.TimeSendInv = this.tsinv;

            this.tagInfos.Clear();
            NameObjectList ps = new NameObjectList();
            ps["������"] = this.devid;
            DataTable tab = this.query.getTable("�Ž�״̬", ps);
            foreach (DataRow dr in tab.Rows)
            {
                if (null == dr || DBNull.Value == dr["���"])
                    continue;
                this.tagInfos.Add(Convert.ToString(dr["���"]), Convert.ToString(dr["����"]));
            }
            string tag = "@�豸��ַ=" + Convert.ToString(this.station);
            cmdGather.setCommand("�Ž�", "���״̬", tag);
            cmdGather.ResponseHandle += new EventHandler<ResponseEventArgs>(cmdGather_ResponseHandle);
            this.commimgr.SendCommand(this.target, cmdGather);
        }

        /// <summary>
        /// Ѳ��״̬:0/�ɼ���¼(��ͣѲ��),1/Ѳ��
        /// </summary>
        private int stateChecking = 1;
        /// <summary>
        /// �л�Ѳ��״̬��0/�ɼ���¼(��ͣѲ��),1/Ѳ��
        /// </summary>
        /// <param name="stateChk">Ѳ������</param>
        private void reChecking(int stateChk)
        {
            if (null == this.cmdGather)
                return;
            CmdState st = this.cmdGather.CheckState();
            if (CmdState.Response == st || CmdState.Request == st)
            {
                this.cmdGather.EventWh.Reset();
                this.cmdGather.EventWh.WaitOne(cmdGather.TimeOut, false);
            }
            this.cmdGather.TimeSendInv = new TimeSpan(24, 0, 0);
            this.commimgr.ClearBuffer(this.target);
            this.stateChecking = stateChk;
            string tag = "@�豸��ַ=" + Convert.ToString(this.station);
            if (1 == stateChk)
                this.cmdGather.setCommand("�Ž�", "���״̬", tag);
            else
                this.cmdGather.setCommand(new byte[0]);
            if (0 < stateChk)
            {
                this.cmdGather.TimeSendInv = this.tsinv;
                this.commimgr.SendCommand(this.target, this.cmdGather);
            }
        }

        /// <summary>
        /// Ѳ����Ӧ,����ʧ��5����(tsbusy)����Ϊͣ��,���tsbusyѲ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdGather_ResponseHandle(object sender, ResponseEventArgs e)
        {
            if (null == sender || null == e)
                return;
            CmdProtocol cmdP = sender as CmdProtocol;
            if (null == cmdP || 0 == this.stateChecking)
                return;
            if (!e.Success)
            {
                //������ʱ��ʧ��ʱ������Ѳ������
                if (cmdP.TimeSendInv < this.tsbusy && DateTime.Now - cmdP.ResponseDatetime > this.tsbusy)
                {
                    if (cmdP.TimeSendInv < this.tsbusy)
                        cmdP.TimeSendInv = this.tsbusy.Add(this.tsinv);
                }
                return;
            }
            string msg = cmdP.ResponseFormat;
            if (string.IsNullOrEmpty(msg) || "true" != basefun.valtag(msg, "Success"))
                return;
            //���ߺ�ָ�����ʱ���ָ�Ѳ������
            if (cmdP.TimeSendInv > this.tsbusy)
                cmdP.TimeSendInv = this.tsinv;
            this.validateSate(msg);

            //2�����ڼ�¼������20��,���豸���ڷ�æ״̬,����ʱ�ɲɼ�
            string sumstr = basefun.valtag(msg, "{ˢ����¼��}");
            string dtstr = basefun.valtag(msg, "{ˢ��ʱ��}");
            if (string.IsNullOrEmpty(sumstr) || string.IsNullOrEmpty(dtstr))
                return;
            int sum = Convert.ToInt32(sumstr);
            DateTime dtcard = DateTime.MinValue;
            try { dtcard = Convert.ToDateTime(dtstr); }
            catch { }
            if ("���ڻ�" == this.devtype && this.posRecord > sum)
            {
                this.posRecord = 0;
                NameObjectList posps = new NameObjectList();
                posps["������"] = this.devid;
                this.query.ExecuteNonQuery("���ü�¼λ��", posps, posps, posps);
            }
            //�ж��Ƿ�æ״̬
            if (sum < 1 || (sum > 60 && DateTime.Now - dtcard < this.tsbusy))
                return;

            //���¼�¼�Ҳ���æʱ�ɲɼ��¼�¼50��
            string[] cols ={ "{����}", "״̬���", "������", "{ˢ��ʱ��}" };
            string tag = "@�豸��ַ=" + Convert.ToString(this.station);
            int st = this.stateChecking;
            this.reChecking(0);
            bool isreset = false;
            if (sum > 10 && DateTime.Now - dtcard > (this.tsbusy + this.tsbusy + this.tsbusy))
            {
                isreset = sum <= 50;
                sum = sum > 50 ? 51 : sum + 1;
            }
            else
            {
                isreset = sum <= 10;
                sum = sum > 10 ? 11 : sum + 1;
            }
            TimeSpan tswait = this.waitTime;
            for (int i = 0; i < sum; i++)
            {
                if ("���ڻ�" == this.devtype)
                    tag = basefun.setvaltag(tag, "{��¼����}", Convert.ToString(this.posRecord));
                cmdP.setCommand("�Ž�", "��ȡ��¼", tag);
                msg = getResponse(this.commimgr, e.Target, cmdP, tswait);
                if ("true" != basefun.valtag(msg, "Success"))
                    break;
                string cardnum = basefun.valtag(msg, "{����}");
                if (string.IsNullOrEmpty(cardnum) || "16777215" == cardnum || "0" == cardnum)
                {
                    this.posRecord++;
                    continue;
                }
                string info = "";
                for (int c = 0; c < cols.Length; c++)
                    info = basefun.setvaltag(info, cols[c], basefun.valtag(msg, cols[c]));
                info = basefun.setvaltag(info, "״̬", this.getStateDoorCard(basefun.valtag(info, "״̬���")));
                NameObjectList ps = ParamManager.createParam(info);
                ps["������"] = this.devid;
                ps["ʱ��"] = ps["ˢ��ʱ��"];
                bool success = this.query.ExecuteNonQuery("�ɼ��Ž�����", ps, ps, ps);
                if (!success)
                    ServiceTool.LogMessage(info, null, EventLogEntryType.Warning);
                DateTime dt = Convert.ToDateTime(ps["ˢ��ʱ��"]);
                this.AddRecord(info);
                DvRecordEventArgs arg = new DvRecordEventArgs(this.devid, this.station, dt, info);
                this.RaiseRecord(arg);
                if (!success)
                    break;
                this.posRecord++;
                //���һ����ȡ��¼
                if (i == sum - 1 && isreset && this.posRecord > 10000)
                {
                    cmdP.setCommand("�Ž�", "��ռ�¼", tag);
                    cmdP.ResetState();
                    this.commimgr.SendCommand(this.target, cmdP);
                    if (cmdP.EventWh.WaitOne(this.waitTime, false))
                    {
                        string suc = basefun.valtag(cmdP.ResponseFormat, "Success");
                        if ("true" == suc)
                            this.query.ExecuteNonQuery("���ü�¼λ��", ps, ps, ps);
                    }
                }
            }
            this.reChecking(st);
        }

        /// <summary>
        /// ֹͣ�ɼ�
        /// </summary>
        public override void StopGather()
        {
            if (null == this.commimgr)
                return;
            this.commimgr.RemoveCommand(this.target, this.cmdGather);
            this.cmdGather = null;
        }

        /// <summary>
        /// ��֤״̬��Ϣ,��״̬�Աȱ仯ʱ�����¼���¼
        /// </summary>
        /// <param name="item">Ѳ����</param>
        /// <param name="devid">������ID</param>
        /// <param name="taginfo">״̬��Ϣtag���ֵ</param>
        private void validateSate(string taginfo)
        {
            if (string.IsNullOrEmpty(taginfo))
                return;
            Dictionary<string, string[]> dictstate = new Dictionary<string, string[]>();
            string[] alarm ={ "4���ű���", "3���ű���", "2���ű���", "1���ű���" };
            dictstate.Add("����", alarm);
            alarm = new string[] { "��", "��Чˢ��", "��������", "�Ƿ�����", "��ʱ", "в��" };
            dictstate.Add("����", alarm);
            alarm = new string[] { "оƬ����", "ϵͳ����4", "ʱ�ӹ���", "ϵͳ����2", "ϵͳ����1" };
            dictstate.Add("����", alarm);
            alarm = new string[] { "��ť4״̬", "��ť3״̬", "��ť2״̬", "��ť1״̬" };
            dictstate.Add("��ť", alarm);
            alarm = new string[] { "�Ŵ�4״̬", "�Ŵ�3״̬", "�Ŵ�2״̬", "�Ŵ�1״̬" };
            dictstate.Add("�Ŵ�", alarm);
            alarm = new string[] { "��4״̬", "��3״̬", "��2״̬", "��1״̬" };
            dictstate.Add("�̵���", alarm);

            //���ǰ��״̬�ı䣻alarmmsg������Ϣ,isalarm��ǰ�Ƿ񱨾��ı�
            string msg = "", alarmmsg = "", msgsigal = "";
            bool isalarm = false;
            NameObjectList psevent = new NameObjectList();
            psevent["������"] = this.devid;
            psevent["ʱ��"] = basefun.valtag(taginfo, "{��ǰʱ��}");
            alarm = new string[] { "����", "����", "����", "��ť", "�Ŵ�", "�̵���" };
            foreach (string state in alarm)
            {
                //�Ա�״̬�����¼�
                psevent["�¼�"] = state;
                string tagorgi = this.tagInfos[state];
                string tagnews = "";
                bool ischanged = false;
                int inum = 5;
                foreach (string st in dictstate[state])
                {
                    inum--;
                    string valorg = basefun.valtag(tagorgi, st);
                    if (string.IsNullOrEmpty(valorg))
                        valorg = "0";
                    string valnew = basefun.valtag(taginfo, st);
                    tagnews = basefun.setvaltag(tagnews, st, valnew);
                    if ("����" == state && "1" == valnew)
                    {
                        if (string.IsNullOrEmpty(alarmmsg))
                            alarmmsg = st;
                        else
                            alarmmsg += "," + st;
                    }
                    if (valorg == valnew && ("����" != state || !isalarm))
                        continue;
                    ischanged = true;
                    if ("����" == state) continue;
                    //�����¼�
                    if ("����" == state || "����" == state)
                    {
                        if ("����" == state)
                        {
                            psevent["���"] = inum.ToString();
                            psevent["����"] = st + "(" + alarmmsg + ")";
                        }
                        else
                        {
                            psevent.Remove("���");
                            psevent["����"] = st;
                        }
                        msg = basefun.setvaltag(msg, st, valnew);
                        if ("1" == valnew)
                            this.query.ExecuteNonQuery("�����Ž��¼�", psevent, psevent, psevent);
                        else
                            this.query.ExecuteNonQuery("�����Ž��¼�", psevent, psevent, psevent);
                    }
                    else
                    {
                        psevent["���"] = inum.ToString();
                        psevent["����"] = st + "��λ " + valnew;
                        msgsigal = basefun.setvaltag(msgsigal, st, valnew);
                        this.query.ExecuteNonQuery("�Ž���λ�¼�", psevent, psevent, psevent);
                    }
                }//foreach (string st in dictstate[state])
                this.tagInfos[state] = tagnews;
                if (!ischanged) continue;
                psevent["���"] = state;
                psevent["����"] = tagnews;
                this.query.ExecuteNonQuery("�Ž�Ѳ��״̬", psevent, psevent, psevent);
                //�������ݱ仯ʱ,�Ƚ���ԭ�����¼�
                if ("����" == state)
                {
                    isalarm = !string.IsNullOrEmpty(alarmmsg);
                    if (!isalarm)
                    {
                        foreach (string a in dictstate[state])
                            msg = basefun.setvaltag(msg, a, "0");
                    }
                    psevent.Remove("���");
                    this.query.ExecuteNonQuery("�����Ž��¼�", psevent, psevent, psevent);
                }
            }//foreach (string state in alarm)
            if (string.IsNullOrEmpty(this.Alarm.tag))
            {
                this.Alarm.tag = taginfo;
                this.Alarm.dtReceive = DateTime.Now;
            }
            if (string.IsNullOrEmpty(this.Signal.tag))
            {
                this.Signal.tag = taginfo;
                this.Signal.dtReceive = DateTime.Now;
            }
            if (string.IsNullOrEmpty(msg) && string.IsNullOrEmpty(msgsigal))
                return;
            DateTime dt = Convert.ToDateTime(basefun.valtag(taginfo, "{��ǰʱ��}"));
            if (!string.IsNullOrEmpty(msg))
            {
                alarm = alarmmsg.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string a in alarm)
                    msg = basefun.setvaltag(msg, a, "1");
                this.Alarm.dtReceive = DateTime.Now;
                this.Alarm.tag = taginfo;
                DvAlarmEventArgs arg = new DvAlarmEventArgs(this.devid, this.station, dt, taginfo);
                arg.TagAlarm = msg;
                this.RaiseAlarm(arg);
            }
            if (!string.IsNullOrEmpty(msgsigal))
            {
                this.Signal.dtReceive = DateTime.Now;
                this.Signal.tag = taginfo;
                DvSignalEventArgs arg = new DvSignalEventArgs(this.devid, this.station, dt, taginfo);
                arg.TagSignal = msgsigal;
                this.RaiseSignal(arg);
            }
        }

        /// <summary>
        /// ����ˢ��״̬�������ˢ��״̬
        /// </summary>
        /// <param name="statecode">ˢ��״̬����</param>
        /// <returns>����ˢ��״̬</returns>
        private string getStateDoorCard(string statecode)
        {
            switch (statecode)
            {
                case "8": statecode = "��ֹͨ��,ԭ����"; break;
                case "9": statecode = "��ֹͨ��,û��Ȩ��"; break;
                case "10": statecode = "��ֹͨ��,���벻��"; break;
                case "11": statecode = "��ֹͨ��,ϵͳ����"; break;
                case "12": statecode = "��ֹͨ��,��Ǳ��,�࿨���Ż���"; break;
                case "13": statecode = "��ֹͨ��,�ų���"; break;
                case "14": statecode = "��ֹͨ��,������Чʱ�λ򿨹���"; break;
                default: statecode = "��������"; break;
            }
            return statecode;
        }
        /// <summary>
        /// ֱ�Ӳɼ�һ������,���Զ��������ݿ�,���ػ�ȡ���ݵļ�¼
        /// û�м�¼ʱ�ָ�Ѳ��
        /// </summary>
        /// <param name="isfirst">�Ƿ��״���ȡ,�״λᲹ��ɼ���ǰ��¼�Է�©��</param>
        public override string GatherData(bool isfirst)
        {
            if (null == this.commimgr || null == this.target || string.IsNullOrEmpty(this.devid) || this.station < 1)
                return "";
            CmdProtocol cmdP = this.cmdGather;
            if (null == cmdP)
                cmdP = new CmdProtocol(false);
            string[] cols ={ "{����}", "״̬���", "������", "{ˢ��ʱ��}" };
            string tag = "@�豸��ַ=" + Convert.ToString(this.station);
            if ("���ڻ�" == this.devtype)
                tag = basefun.setvaltag(tag, "{��¼����}", Convert.ToString(this.posRecord));
            this.reChecking(0);
            cmdP.setCommand("�Ž�", "��ȡ��¼", tag);
            string msg = getResponse(this.commimgr, this.target, cmdP, this.waitTime);
            string cardnum = basefun.valtag(msg, "{����}");
            string suc = basefun.valtag(msg, "Success");
            if ("true" == suc)
                this.posRecord++;
            if ("true" != suc ||string.IsNullOrEmpty(cardnum)|| "16777215" == cardnum || "0" == cardnum)
            {
                if (this.posRecord > 10000 && ("false" == suc || "16777215" == cardnum || "0" == cardnum))
                {
                    //��ռ�¼
                    cmdP.setCommand("�Ž�", "��ռ�¼", tag);
                    cmdP.ResetState();
                    this.commimgr.SendCommand(this.target, cmdP);
                    if (cmdP.EventWh.WaitOne(this.waitTime, false))
                    {
                        suc = basefun.valtag(cmdP.ResponseFormat, "Success");
                        if ("true" == suc)
                        {
                            this.posRecord = 0;
                            NameObjectList pspos = new NameObjectList();
                            pspos["������"] = this.devid;
                            this.query.ExecuteNonQuery("���ü�¼λ��", pspos, pspos, pspos);
                        }
                    }
                }
                this.reChecking(1);
                return msg;
            }
            string info = "";
            for (int c = 0; c < cols.Length; c++)
                info = basefun.setvaltag(info, cols[c], basefun.valtag(msg, cols[c]));
            info = basefun.setvaltag(info, "״̬", this.getStateDoorCard(basefun.valtag(info, "״̬���")));
            NameObjectList ps = ParamManager.createParam(info);
            ps["������"] = this.devid;
            ps["ʱ��"] = ps["ˢ��ʱ��"];
            DateTime dt = DateTime.MinValue;
            try
            {
                dt = Convert.ToDateTime(basefun.valtag(msg, "{ˢ��ʱ��}"));
            }
            catch { dt = DateTime.MinValue; }
            if (dt < DateTime.Today.AddYears(-5) || dt > DateTime.Today.AddYears(5))
                ps["ʱ��"] = ps["ˢ��ʱ��"] = null;
            bool success = this.query.ExecuteNonQuery("�ɼ��Ž�����", ps, ps, ps);
            if (!success || dt < DateTime.Today.AddYears(-5) || dt > DateTime.Today.AddYears(5))
                ServiceTool.LogMessage(info, null, EventLogEntryType.Warning);
            this.AddRecord(info);
            DvRecordEventArgs arg = new DvRecordEventArgs(this.devid, this.station, dt, info);
            this.RaiseRecord(arg);
            return msg;
        }

        /// <summary>
        /// ����ָ���ȡָ��������ͨѶʧ��ʱ�Զ�����ִ��5��
        /// </summary>
        /// <param name="mgr">ͨѶ������</param>
        /// <param name="target">ͨѶĿ��</param>
        /// <param name="cmd">ִ��ָ��</param>
        /// <param name="timeout">��ʱ���</param>
        /// <returns>����ָ����Ӧ���</returns>
        private static string getResponse(CommiManager mgr, CommiTarget target, CmdProtocol cmd, TimeSpan timeout)
        {
            if (null == mgr || null == target || null == cmd)
                return "";
            if (null == cmd.EventWh)
                cmd.EventWh = new ManualResetEvent(false);
            string msg = "";
            for (int i = 0; i < 5; i++)
            {
                msg = "";
                cmd.ResetState();
                mgr.SendCommand(target, cmd);
                if (cmd.EventWh.WaitOne(timeout, false))
                    msg = cmd.ResponseFormat;
                string suc = basefun.valtag(msg, "Success");
                if ("true" == suc)
                    break;
                Thread.Sleep(200);
            }
            return msg;
        }

    }
}
