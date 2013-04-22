using System;
using System.Collections.Generic;
using System.Text;
using Granity.CardOneCommi;
using Granity.communications;
using System.Diagnostics;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using System.Collections.Specialized;
using System.Data;
using Granity.winTools;
using System.Threading;

namespace Granity.commiServer
{
    /// <summary>
    /// 消费设备
    /// </summary>
    public class DeviceEatery : DeviceBase
    {
        /// <summary>
        /// 消费监控单元
        /// </summary>
        private UnitItem unitItem = null;
        /// <summary>
        /// 数据操作
        /// </summary>
        private QueryDataRes query = null;
        /// <summary>
        /// 采集指令
        /// </summary>
        private CmdProtocol cmdGather = null;
        /// <summary>
        /// 同步等待时刻,默认500毫秒认为超时失败
        /// </summary>
        private TimeSpan waitTime = new TimeSpan(0, 0, 0, 0, 500);
        /// <summary>
        /// 采集周期,默认巡检周期2秒
        /// </summary>
        private TimeSpan tsinv = new TimeSpan(0, 0, 2);
        /// <summary>
        /// 判断是否繁忙状态的时间间隔,默认5分钟内巡检到状态是“工作中”认为是繁忙状态
        /// </summary>
        private TimeSpan tsbusy = new TimeSpan(0, 2, 0);
        /// <summary>
        /// 巡检到"工作中"或无采集记录时更新时间
        /// </summary>
        private DateTime dtwork = DateTime.Now;
        /// <summary>
        /// 当前巡检状态
        /// </summary>
        private NameValueCollection tagInfos = new NameValueCollection();
        /// <summary>
        /// 当前设备记录位置
        /// </summary>
        private static Dictionary<string, int> recordpos = new Dictionary<string, int>();

        /// <summary>
        /// 消费设备构造函数
        /// </summary>
        public DeviceEatery() : base()
        {
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "消费监控");
            this.query = new QueryDataRes(this.unitItem.DataSrcFile);
        }

        /// <summary>
        /// 启动采集
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

            this.dtwork = DateTime.Now;
            this.tagInfos.Clear();
            NameObjectList ps = new NameObjectList();
            ps["消费机"] = this.devid;
            DataTable tab = this.query.getTable("消费机状态", ps);
            foreach (DataRow dr in tab.Rows)
            {
                if (null == dr || DBNull.Value == dr["类别"])
                    continue;
                this.tagInfos.Add(Convert.ToString(dr["类别"]), Convert.ToString(dr["内容"]));
            }
            string tag = "@设备地址=" + Convert.ToString(this.station);
            cmdGather.setCommand("消费", "检测状态", tag);
            cmdGather.ResponseHandle += new EventHandler<ResponseEventArgs>(cmdGather_ResponseHandle);
            this.commimgr.SendCommand(this.target, cmdGather);
        }

        /// <summary>
        /// 巡检状态:0/采集记录(暂停巡检),1/巡检
        /// </summary>
        private int stateChecking = 1;
        /// <summary>
        /// 切换巡检状态：0/采集记录(暂停巡检),1/巡检
        /// </summary>
        /// <param name="stateChk">巡检类型</param>
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
            string tag = "@设备地址=" + Convert.ToString(this.station);
            if (1 == stateChk)
                this.cmdGather.setCommand("消费", "检测状态", tag);
            else
                this.cmdGather.setCommand(new byte[0]);
            if (0 < stateChk)
            {
                this.cmdGather.TimeSendInv = this.tsinv;
                this.commimgr.SendCommand(this.target, this.cmdGather);
            }
        }
        
        /// <summary>
        /// 巡检响应,连续失败5分钟(tsbusy)则认为停机,间隔tsbusy巡检
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
                //连续长时间失败时，增大巡检周期
                if (cmdP.TimeSendInv < this.tsbusy && DateTime.Now - cmdP.ResponseDatetime > this.tsbusy)
                {
                    if (cmdP.TimeSendInv < this.tsbusy)
                        cmdP.TimeSendInv = this.tsbusy.Add(this.tsinv);
                }
                return;
            }
            string msg = cmdP.ResponseFormat;
            if ("true" != basefun.valtag(msg, "Success"))
                return;
            //离线后恢复在线时，恢复巡检周期
            if (cmdP.TimeSendInv > this.tsbusy)
                cmdP.TimeSendInv = this.tsinv;
            //工作中或没有新记录,则
            string valwk = basefun.valtag(msg, "工作中");
            int sum = Convert.ToInt32(basefun.valtag(msg, "{采集标志}"));
            if ("1" == valwk || sum < 1)
                this.dtwork = DateTime.Now;
            this.validateSate(msg);
            //有新记录且不繁忙时可采集新记录50条
            if (sum < 1 || DateTime.Now - this.dtwork < tsbusy)
                return;
            string[] cols ={ "{卡号}", "{卡类}", "{消费时间}", "{消费金额}", "{卡余额}", "{累计补助金额}", "{消费机号}", "{操作员号}" };
            string tag = "@设备地址=" + Convert.ToString(this.station);
            this.reChecking(0);
            TimeSpan tswait = this.waitTime;
            for (int i = 0; i < 51; i++)
            {
                if(i<1)
                    cmdP.setCommand("消费", "取当前消费记录", tag);
                else if(i<2)
                    cmdP.setCommand("消费", "取下一条消费记录", tag);
                msg = getResponse(this.commimgr, this.target, cmdP, tswait);
                if ("true" != basefun.valtag(msg, "Success"))
                    break;
                string cardnum = basefun.valtag(msg, "{卡号}");
                if (string.IsNullOrEmpty(cardnum) || "16777215" == cardnum || "0" == cardnum)
                    continue;
                string info = "";
                for (int c = 0; c < cols.Length; c++)
                    info = basefun.setvaltag(info, cols[c], basefun.valtag(msg, cols[c]));
                NameObjectList ps = ParamManager.createParam(info);
                ps["消费机"] = this.devid;
                bool success = this.query.ExecuteNonQuery("采集数据", ps, ps, ps);
                if (!success)
                    ServiceTool.LogMessage(info, null, EventLogEntryType.Warning);
                DateTime dt = Convert.ToDateTime(ps["消费时间"]);
                this.AddRecord(info);
                DvRecordEventArgs arg = new DvRecordEventArgs(this.devid, this.station, dt, info);
                this.RaiseRecord(arg);
                if (!success)
                    break;
            }
            this.reChecking(1);
        }

        /// <summary>
        /// 停止采集
        /// </summary>
        public override void StopGather()
        {
            if (null == this.commimgr)
                return;
            this.commimgr.RemoveCommand(this.target, this.cmdGather);
            this.cmdGather = null;
        }

        /// <summary>
        /// 验证状态信息,对状态对比变化时生成事件记录
        /// </summary>
        /// <param name="item">巡检项</param>
        /// <param name="devid">控制器ID</param>
        /// <param name="taginfo">状态信息tag标记值</param>
        private void validateSate(string taginfo)
        {
            if (string.IsNullOrEmpty(taginfo))
                return;
            Dictionary<string, string[]> dictstate = new Dictionary<string, string[]>();
            string[] alarm ={ "时间格式错", "读头故障", "权限校验错", "时段参数错", "收费参数错", "黑名单错" };
            dictstate.Add("内部状态", alarm);

            //检查前后状态改变；alarmmsg报警信息,isalarm当前是否报警改变
            NameObjectList psevent = new NameObjectList();
            psevent["消费机"] = this.devid;
            psevent["时间"] = basefun.valtag(taginfo, "{当前时间}");
            alarm = new string[] { "内部状态" };
            string msg = "";
            foreach (string state in alarm)
            {
                //对比状态生成事件
                psevent["事件"] = state;
                string tagorgi = this.tagInfos[state];
                string tagnews = "";
                bool ischanged = false;
                foreach (string st in dictstate[state])
                {
                    string valorg = basefun.valtag(tagorgi, st);
                    if (string.IsNullOrEmpty(valorg))
                        valorg = "0";
                    string valnew = basefun.valtag(taginfo, st);
                    tagnews = basefun.setvaltag(tagnews, st, valnew);
                    if (valorg == valnew)
                        continue;
                    ischanged = true;
                    psevent["内容"] = st;
                    msg = basefun.setvaltag(msg, st, valnew);
                    if ("1" != valnew)
                        this.query.ExecuteNonQuery("结束消费机事件", psevent, psevent, psevent);
                    else
                        this.query.ExecuteNonQuery("发生消费机事件", psevent, psevent, psevent);
                }
                this.tagInfos[state] = tagnews;
                if (!ischanged) continue;
                psevent["类别"] = state;
                psevent["内容"] = tagnews;
                this.query.ExecuteNonQuery("消费巡检状态", psevent, psevent, psevent);
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
            if (string.IsNullOrEmpty(msg))
                return;
            this.Alarm.dtReceive = DateTime.Now;
            this.Alarm.tag = taginfo;
            DateTime dt = Convert.ToDateTime(basefun.valtag(taginfo, "{当前时间}"));
            DvAlarmEventArgs arg = new DvAlarmEventArgs(this.devid, this.station, dt, taginfo);
            arg.TagAlarm = msg;
            this.RaiseAlarm(arg);
        }

        /// <summary>
        /// 直接采集一条数据,并自动存入数据库,返回获取数据的记录
        /// 没有记录时恢复巡检
        /// </summary>
        /// <param name="isfirst">是否首次提取,首次会补充采集当前记录以防漏采</param>
        public override string GatherData(bool isfirst)
        {
            if (null == this.commimgr || null == this.target || string.IsNullOrEmpty(this.devid) || this.station < 1)
                return "";
            CmdProtocol cmdP = this.cmdGather;
            if (null == cmdP)
                cmdP = new CmdProtocol(false);
            string[] cols ={ "{卡号}", "{卡类}", "{消费时间}", "{消费金额}", "{卡余额}", "{累计补助金额}", "{消费机号}", "{操作员号}" };
            string tag = "@设备地址=" + Convert.ToString(this.station);
            this.reChecking(0);
            if (recordpos.ContainsKey(this.devid))
            {
                tag = basefun.setvaltag(tag, "{记录值指针}", Convert.ToString(recordpos[this.devid]));
                cmdP.setCommand("消费", "取指定记录", tag);
            }
            else
            {
                if (isfirst)
                {
                    this.getSubsidy();
                    cmdP.setCommand("消费", "取当前消费记录", tag);
                }
                else
                    cmdP.setCommand("消费", "取下一条消费记录", tag);
            }
            string msg = getResponse(this.commimgr, this.target, cmdP, this.waitTime);
            string cardnum = basefun.valtag(msg, "{卡号}");
            string suc = basefun.valtag(msg, "Success");
            if ("true" != suc || string.IsNullOrEmpty(cardnum) || "16777215" == cardnum || "0" == cardnum)
            {
                this.reChecking(1);
                if ("false" == suc || "16777215" == cardnum || "0" == cardnum)
                    recordpos.Remove(this.devid);
                return msg;
            }
            string info = "";
            for (int c = 0; c < cols.Length; c++)
                info = basefun.setvaltag(info, cols[c], basefun.valtag(msg, cols[c]));
            NameObjectList ps = ParamManager.createParam(info);
            ps["消费机"] = this.devid;
            bool success = this.query.ExecuteNonQuery("采集数据", ps, ps, ps);
            if (!success)
                ServiceTool.LogMessage(info, null, EventLogEntryType.Warning);
            if (recordpos.ContainsKey(this.devid))
            {
                recordpos[this.devid]++;
                ps.Clear();
                ps["消费机"] = this.devid;
                ps["记录位置"] = recordpos[this.devid];
                query.ExecuteNonQuery("记录指针", ps, ps, ps);
            }
            DateTime dt = DateTime.MinValue;
            try
            {
                dt = Convert.ToDateTime(ps["消费时间"]);
            }
            catch { }
            this.AddRecord(info);
            DvRecordEventArgs arg = new DvRecordEventArgs(this.devid, this.station, dt, info);
            this.RaiseRecord(arg);
            return msg;
        }
        /// <summary>
        /// 采集补助
        /// </summary>
        private void getSubsidy()
        {
            if (null == this.commimgr || null == this.target || string.IsNullOrEmpty(this.devid) || this.station < 1)
                return;
            CmdProtocol cmdP = this.cmdGather;
            if (null == cmdP)
                cmdP = new CmdProtocol(false);
            string[] cols ={ "{卡号}", "{卡类}", "{充值时间}", "{本次补助金额}", "{补助后总额}" };
            string tag = "@设备地址=" + Convert.ToString(this.station);
            this.reChecking(0);
            cmdP.setCommand("消费", "取当前补助记录", tag);
            string msg = getResponse(this.commimgr, this.target, cmdP, this.waitTime);
            cmdP.setCommand("消费", "取下一条补助记录", tag);
            string cardnum = basefun.valtag(msg, "{卡号}");
            NameObjectList ps = new NameObjectList();
            while (!string.IsNullOrEmpty(cardnum) && "16777215" != cardnum && "0" != cardnum)
            {
                for (int c = 0; c < cols.Length; c++)
                    ps[cols[c]] = basefun.valtag(msg, cols[c]);
                bool success = this.query.ExecuteNonQuery("消费接收补助", ps, ps, ps);
                if (!success) return;
                msg = getResponse(this.commimgr, this.target, cmdP, this.waitTime);
                cardnum = basefun.valtag(msg, "{卡号}");
            }
        }

        /// <summary>
        /// 重置设备记录位置，自
        /// </summary>
        /// <param name="devid">设备ID，为空则清空所有设备记录位置，有则重置为1，没有则添加</param>
        public static void ResetPosition(string devid)
        {
            bool isreset = false;
            if ("true" == devid)
                isreset = true;
            if ("true" == devid || "false" == devid)
                devid = "";
            if (recordpos.ContainsKey(devid))
                return;
            UnitItem unitItem = null;
            QueryDataRes query = null;
            if (isreset || !string.IsNullOrEmpty(devid))
            {
                unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "消费监控");
                query = new QueryDataRes(unitItem.DataSrcFile);
            }
            NameObjectList ps = new NameObjectList();
            if (isreset)
                query.ExecuteNonQuery("记录指针", ps, ps, ps);
            if (string.IsNullOrEmpty(devid))
            {
                recordpos.Clear();
                return;
            }
            ps["消费机"] = devid;
            DataTable tab = query.getTable("记录指针", ps);
            if (null == tab || tab.Rows.Count < 1)
                return;
            recordpos.Add(devid, Convert.ToInt32(tab.Rows[0]["记录位置"]));
        }
        /// <summary>
        /// 发送指令，获取指令结果，在通讯失败时自动尝试执行5次
        /// </summary>
        /// <param name="mgr">通讯管理器</param>
        /// <param name="target">通讯目标</param>
        /// <param name="cmd">执行指令</param>
        /// <param name="timeout">超时间隔</param>
        /// <returns>返回指令响应结果</returns>
        private static string getResponse(CommiManager mgr, CommiTarget target, CmdProtocol cmd, TimeSpan timeout)
        {
            if (null == mgr || null == target || null == cmd)
                return "";
            if(null==cmd.EventWh)
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
