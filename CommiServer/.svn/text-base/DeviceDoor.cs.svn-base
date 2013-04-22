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
    /// 门禁设备
    /// </summary>
    public class DeviceDoor : DeviceBase
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
        /// 记录位置
        /// </summary>
        private int posRecord = 0;
        /// <summary>
        /// 设备类型：考勤机，单门双向，双门双向，四门单向
        /// </summary>
        private string devtype = "考勤机";
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
        /// 判断是否繁忙状态的时间间隔,默认2分钟内记录总数超过60条,认为处于繁忙状态
        /// </summary>
        private TimeSpan tsbusy = new TimeSpan(0, 2, 0);
        /// <summary>
        /// 当前巡检状态
        /// </summary>
        private NameValueCollection tagInfos = new NameValueCollection();

        public DeviceDoor():base()
        {
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "门禁监控");
            this.query = new QueryDataRes(this.unitItem.DataSrcFile);
        }
        /// <summary>
        /// 设置通讯设备
        /// </summary>
        /// <param name="devid">设备ID</param>
        /// <param name="target">通讯目标</param>
        /// <param name="station">站址</param>
        public override void SetDevice(CommiManager mgr, CommiTarget target, string devid, int station)
        {
            if (null == mgr || string.IsNullOrEmpty(devid) || null == target || station < 1)
                return;
            this.commimgr = mgr;
            this.target = target;
            this.devid = devid;
            this.station = station;
            NameObjectList ps = new NameObjectList();
            ps["分组ID"] = devid;
            DataTable tab = this.query.getTable("门禁设备", ps);
            if (null == tab || tab.Rows.Count < 1)
                return;
            object val = tab.Rows[0]["记录位置"];
            if (null == val || DBNull.Value == val)
                val = 0;
            this.posRecord = Convert.ToInt32(val);
            val = tab.Rows[0]["控制器类型"];
            this.devtype = Convert.ToString(val);
            if (string.IsNullOrEmpty(this.devtype))
                this.devtype = "考勤机";
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

            this.tagInfos.Clear();
            NameObjectList ps = new NameObjectList();
            ps["控制器"] = this.devid;
            DataTable tab = this.query.getTable("门禁状态", ps);
            foreach (DataRow dr in tab.Rows)
            {
                if (null == dr || DBNull.Value == dr["类别"])
                    continue;
                this.tagInfos.Add(Convert.ToString(dr["类别"]), Convert.ToString(dr["内容"]));
            }
            string tag = "@设备地址=" + Convert.ToString(this.station);
            cmdGather.setCommand("门禁", "检测状态", tag);
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
                this.cmdGather.setCommand("门禁", "检测状态", tag);
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
            if (string.IsNullOrEmpty(msg) || "true" != basefun.valtag(msg, "Success"))
                return;
            //离线后恢复在线时，恢复巡检周期
            if (cmdP.TimeSendInv > this.tsbusy)
                cmdP.TimeSendInv = this.tsinv;
            this.validateSate(msg);

            //2分钟内记录数大于20条,则设备处于繁忙状态,空闲时可采集
            string sumstr = basefun.valtag(msg, "{刷卡记录数}");
            string dtstr = basefun.valtag(msg, "{刷卡时间}");
            if (string.IsNullOrEmpty(sumstr) || string.IsNullOrEmpty(dtstr))
                return;
            int sum = Convert.ToInt32(sumstr);
            DateTime dtcard = DateTime.MinValue;
            try { dtcard = Convert.ToDateTime(dtstr); }
            catch { }
            if ("考勤机" == this.devtype && this.posRecord > sum)
            {
                this.posRecord = 0;
                NameObjectList posps = new NameObjectList();
                posps["控制器"] = this.devid;
                this.query.ExecuteNonQuery("重置记录位置", posps, posps, posps);
            }
            //判定是否繁忙状态
            if (sum < 1 || (sum > 60 && DateTime.Now - dtcard < this.tsbusy))
                return;

            //有新记录且不繁忙时可采集新记录50条
            string[] cols ={ "{卡号}", "状态编号", "读卡器", "{刷卡时间}" };
            string tag = "@设备地址=" + Convert.ToString(this.station);
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
                if ("考勤机" == this.devtype)
                    tag = basefun.setvaltag(tag, "{记录索引}", Convert.ToString(this.posRecord));
                cmdP.setCommand("门禁", "读取记录", tag);
                msg = getResponse(this.commimgr, e.Target, cmdP, tswait);
                if ("true" != basefun.valtag(msg, "Success"))
                    break;
                string cardnum = basefun.valtag(msg, "{卡号}");
                if (string.IsNullOrEmpty(cardnum) || "16777215" == cardnum || "0" == cardnum)
                {
                    this.posRecord++;
                    continue;
                }
                string info = "";
                for (int c = 0; c < cols.Length; c++)
                    info = basefun.setvaltag(info, cols[c], basefun.valtag(msg, cols[c]));
                info = basefun.setvaltag(info, "状态", this.getStateDoorCard(basefun.valtag(info, "状态编号")));
                NameObjectList ps = ParamManager.createParam(info);
                ps["控制器"] = this.devid;
                ps["时间"] = ps["刷卡时间"];
                bool success = this.query.ExecuteNonQuery("采集门禁数据", ps, ps, ps);
                if (!success)
                    ServiceTool.LogMessage(info, null, EventLogEntryType.Warning);
                DateTime dt = Convert.ToDateTime(ps["刷卡时间"]);
                this.AddRecord(info);
                DvRecordEventArgs arg = new DvRecordEventArgs(this.devid, this.station, dt, info);
                this.RaiseRecord(arg);
                if (!success)
                    break;
                this.posRecord++;
                //最后一条提取记录
                if (i == sum - 1 && isreset && this.posRecord > 10000)
                {
                    cmdP.setCommand("门禁", "清空记录", tag);
                    cmdP.ResetState();
                    this.commimgr.SendCommand(this.target, cmdP);
                    if (cmdP.EventWh.WaitOne(this.waitTime, false))
                    {
                        string suc = basefun.valtag(cmdP.ResponseFormat, "Success");
                        if ("true" == suc)
                            this.query.ExecuteNonQuery("重置记录位置", ps, ps, ps);
                    }
                }
            }
            this.reChecking(st);
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
            string[] alarm ={ "4号门报警", "3号门报警", "2号门报警", "1号门报警" };
            dictstate.Add("报警", alarm);
            alarm = new string[] { "火警", "无效刷卡", "联动报警", "非法闯入", "超时", "胁迫" };
            dictstate.Add("警报", alarm);
            alarm = new string[] { "芯片故障", "系统故障4", "时钟故障", "系统故障2", "系统故障1" };
            dictstate.Add("故障", alarm);
            alarm = new string[] { "按钮4状态", "按钮3状态", "按钮2状态", "按钮1状态" };
            dictstate.Add("按钮", alarm);
            alarm = new string[] { "门磁4状态", "门磁3状态", "门磁2状态", "门磁1状态" };
            dictstate.Add("门磁", alarm);
            alarm = new string[] { "继4状态", "继3状态", "继2状态", "继1状态" };
            dictstate.Add("继电器", alarm);

            //检查前后状态改变；alarmmsg报警信息,isalarm当前是否报警改变
            string msg = "", alarmmsg = "", msgsigal = "";
            bool isalarm = false;
            NameObjectList psevent = new NameObjectList();
            psevent["控制器"] = this.devid;
            psevent["时间"] = basefun.valtag(taginfo, "{当前时间}");
            alarm = new string[] { "警报", "报警", "故障", "按钮", "门磁", "继电器" };
            foreach (string state in alarm)
            {
                //对比状态生成事件
                psevent["事件"] = state;
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
                    if ("警报" == state && "1" == valnew)
                    {
                        if (string.IsNullOrEmpty(alarmmsg))
                            alarmmsg = st;
                        else
                            alarmmsg += "," + st;
                    }
                    if (valorg == valnew && ("报警" != state || !isalarm))
                        continue;
                    ischanged = true;
                    if ("警报" == state) continue;
                    //处理事件
                    if ("报警" == state || "故障" == state)
                    {
                        if ("报警" == state)
                        {
                            psevent["编号"] = inum.ToString();
                            psevent["内容"] = st + "(" + alarmmsg + ")";
                        }
                        else
                        {
                            psevent.Remove("编号");
                            psevent["内容"] = st;
                        }
                        msg = basefun.setvaltag(msg, st, valnew);
                        if ("1" == valnew)
                            this.query.ExecuteNonQuery("发生门禁事件", psevent, psevent, psevent);
                        else
                            this.query.ExecuteNonQuery("结束门禁事件", psevent, psevent, psevent);
                    }
                    else
                    {
                        psevent["编号"] = inum.ToString();
                        psevent["内容"] = st + "变位 " + valnew;
                        msgsigal = basefun.setvaltag(msgsigal, st, valnew);
                        this.query.ExecuteNonQuery("门禁变位事件", psevent, psevent, psevent);
                    }
                }//foreach (string st in dictstate[state])
                this.tagInfos[state] = tagnews;
                if (!ischanged) continue;
                psevent["类别"] = state;
                psevent["内容"] = tagnews;
                this.query.ExecuteNonQuery("门禁巡检状态", psevent, psevent, psevent);
                //报警内容变化时,先结束原警报事件
                if ("警报" == state)
                {
                    isalarm = !string.IsNullOrEmpty(alarmmsg);
                    if (!isalarm)
                    {
                        foreach (string a in dictstate[state])
                            msg = basefun.setvaltag(msg, a, "0");
                    }
                    psevent.Remove("编号");
                    this.query.ExecuteNonQuery("结束门禁事件", psevent, psevent, psevent);
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
            DateTime dt = Convert.ToDateTime(basefun.valtag(taginfo, "{当前时间}"));
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
        /// 根据刷卡状态编码解释刷卡状态
        /// </summary>
        /// <param name="statecode">刷卡状态编码</param>
        /// <returns>返回刷卡状态</returns>
        private string getStateDoorCard(string statecode)
        {
            switch (statecode)
            {
                case "8": statecode = "禁止通过,原因不详"; break;
                case "9": statecode = "禁止通过,没有权限"; break;
                case "10": statecode = "禁止通过,密码不对"; break;
                case "11": statecode = "禁止通过,系统故障"; break;
                case "12": statecode = "禁止通过,反潜回,多卡开门或互锁"; break;
                case "13": statecode = "禁止通过,门常闭"; break;
                case "14": statecode = "禁止通过,不在有效时段或卡过期"; break;
                default: statecode = "正常开门"; break;
            }
            return statecode;
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
            string[] cols ={ "{卡号}", "状态编号", "读卡器", "{刷卡时间}" };
            string tag = "@设备地址=" + Convert.ToString(this.station);
            if ("考勤机" == this.devtype)
                tag = basefun.setvaltag(tag, "{记录索引}", Convert.ToString(this.posRecord));
            this.reChecking(0);
            cmdP.setCommand("门禁", "读取记录", tag);
            string msg = getResponse(this.commimgr, this.target, cmdP, this.waitTime);
            string cardnum = basefun.valtag(msg, "{卡号}");
            string suc = basefun.valtag(msg, "Success");
            if ("true" == suc)
                this.posRecord++;
            if ("true" != suc ||string.IsNullOrEmpty(cardnum)|| "16777215" == cardnum || "0" == cardnum)
            {
                if (this.posRecord > 10000 && ("false" == suc || "16777215" == cardnum || "0" == cardnum))
                {
                    //清空记录
                    cmdP.setCommand("门禁", "清空记录", tag);
                    cmdP.ResetState();
                    this.commimgr.SendCommand(this.target, cmdP);
                    if (cmdP.EventWh.WaitOne(this.waitTime, false))
                    {
                        suc = basefun.valtag(cmdP.ResponseFormat, "Success");
                        if ("true" == suc)
                        {
                            this.posRecord = 0;
                            NameObjectList pspos = new NameObjectList();
                            pspos["控制器"] = this.devid;
                            this.query.ExecuteNonQuery("重置记录位置", pspos, pspos, pspos);
                        }
                    }
                }
                this.reChecking(1);
                return msg;
            }
            string info = "";
            for (int c = 0; c < cols.Length; c++)
                info = basefun.setvaltag(info, cols[c], basefun.valtag(msg, cols[c]));
            info = basefun.setvaltag(info, "状态", this.getStateDoorCard(basefun.valtag(info, "状态编号")));
            NameObjectList ps = ParamManager.createParam(info);
            ps["控制器"] = this.devid;
            ps["时间"] = ps["刷卡时间"];
            DateTime dt = DateTime.MinValue;
            try
            {
                dt = Convert.ToDateTime(basefun.valtag(msg, "{刷卡时间}"));
            }
            catch { dt = DateTime.MinValue; }
            if (dt < DateTime.Today.AddYears(-5) || dt > DateTime.Today.AddYears(5))
                ps["时间"] = ps["刷卡时间"] = null;
            bool success = this.query.ExecuteNonQuery("采集门禁数据", ps, ps, ps);
            if (!success || dt < DateTime.Today.AddYears(-5) || dt > DateTime.Today.AddYears(5))
                ServiceTool.LogMessage(info, null, EventLogEntryType.Warning);
            this.AddRecord(info);
            DvRecordEventArgs arg = new DvRecordEventArgs(this.devid, this.station, dt, info);
            this.RaiseRecord(arg);
            return msg;
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
