using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using Granity.communications;
using Granity.CardOneCommi;
using System.Diagnostics;
using System.IO.Ports;
using DevExpress.XtraEditors;

namespace Granity.granityMgr.DoorMgr
{
    /// <summary>
    /// 门禁监控
    /// </summary>
    public partial class FrmDoorMonitor : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "门禁监控";//单元名称
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        /// <summary>
        /// 门禁巡检状态记录
        /// </summary>
        private DataTable tabStateDoor = null;

        public FrmDoorMonitor()
        {
            InitializeComponent();
        }

        private void FrmDoorMonitor_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
            //对树的操作
            DataTable tab = this.ds.Tables["门禁监控"];
            this.bindMgr.BindTrv(this.trvDoorStall, tab, "名称", "ID", "PID", "@ID={ID},@PID={PID}");
            this.trvDoorStall.ExpandAll();
            this.tabStateDoor = this.ds.Tables["门禁巡检状态"];
            if (gridViewInfo.RowCount > 0)
                gridViewInfo.DeleteRow(0);
        }

        /// <summary>
        /// 导航树节点改变联动当前数据表当前行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        string FZID = "";
        private void trvParkStall_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (null == e.Node)
                return;
            string tag = Convert.ToString(e.Node.Tag);
            this.paramwin["分组ID"] = basefun.valtag(tag, "ID");
            FZID = basefun.valtag(tag, "ID");
            //得到数据源
            DataTable tabdev = ds.Tables["门禁设备"];
            this.lvDoor.Clear();
            tabdev.Clear();
            this.Query.FillDataSet(tabdev.TableName, this.paramwin, this.ds);
            this.Query.FillDataSet(tabStateDoor.TableName, this.paramwin, this.ds);
            string[] cols ={ "通讯类别", "串口", "波特率", "数据位", "停止位", "IP地址", "端口", "站址", "控制器", "名称", "控制器类型", "读卡器号", "站址" };
            for (int i = 0; i < tabdev.Rows.Count; i++)
            {
                DataRow drdev = tabdev.Rows[i];
                string txt = Convert.ToString(drdev["名称"]) + "(" + Convert.ToString(drdev["站址"]) + ")";
                ListViewItem item = new ListViewItem(txt);
                item.ImageIndex = 0;
                item.Text = txt;
                item.ToolTipText = "读头号：" + Convert.ToString(drdev["读卡器号"]);
                tag = "";
                for (int c = 0; c < cols.Length; c++)
                    tag += "," + Convert.ToString(drdev[cols[c]]);
                if (tag.StartsWith(","))
                    tag = tag.Substring(1);
                item.Tag = tag;
                lvDoor.Items.Add(item);
            }
        }
        public void DoorStatus(ListViewItem lst, string Status)
        {
            if (Status == "true")

                lst.ImageIndex = 1;
            else
                lst.ImageIndex = 0;
        }
        /// <summary>
        /// 是否正在巡检中
        /// </summary>
        private bool isRunning = false;
        /// <summary>
        /// 定时巡检,支持代理巡检,在检测到门禁记录在50—1000记录时则只取最新记录,其他累积记录到1000时再完全下载
        ///          另有选项强制下载全部记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>\
        /// 
        string DevID = "";
        string dataCount = "";
        private void tmRefresh_Tick(object sender, EventArgs e)
        {
            if (isRunning) return;
            isRunning = true;
            //巡检过的设备地址加入devs中,避免重复巡检
            string devs = ",", tpl = "门禁";
            // isrealtime/是否实时状态,在最后刷卡时间5分钟不变化时可对累积数据大批量采集
            bool isrealtime = false;
            DateTime dtpre = DateTime.Now.AddMinutes(-5);
            //有信息则记录日志
            DataView dvinfo = gridViewInfo.DataSource as DataView;
            //站址/记录数列表,非实时采集状态时优先批量采集记录数大的
            Dictionary<string, int> devrecord = new Dictionary<string, int>();
            Dictionary<string, string[]> devinfo = new Dictionary<string, string[]>();
            List<CommiTarget> targetlist = new List<CommiTarget>();
            CmdProtocol cmdP = new CmdProtocol(false);
            cmdP.TimeOut = new TimeSpan(400 * 10000);
            cmdP.TimeSendInv = new TimeSpan(1, 0, 0);
            cmdP.TimeFailLimit = new TimeSpan(900 * 10000);
            cmdP.TimeLimit = new TimeSpan(900 * 10000);
            NameObjectList psdata = new NameObjectList();
            for (int i = 0; i < lvDoor.Items.Count; i++)
            {
                string tag = Convert.ToString(lvDoor.Items[i].Tag);
                if (string.IsNullOrEmpty(tag))
                    continue;
                string[] devps = tag.Split(",".ToCharArray());
                if (devs.Contains("," + devps[7] + ","))
                    continue;
                CommiTarget target = this.getTarget(devps);
                if (null == target) continue;
                targetlist.Add(target);
                tag = "@设备地址=" + devps[7];
                devs += devps[7] + ",";
                cmdP.setCommand(tpl, "检测状态", tag);
                cmdP.ResetState();
                CommiManager.GlobalManager.SendCommand(target, cmdP);
                if (!cmdP.EventWh.WaitOne(300, false))
                    continue;
                string msg = cmdP.ResponseFormat;
                DevID = basefun.valtag(msg, "{设备地址}");
                //获得要开的门
                getDKQ(devps[10], basefun.valtag(msg, "读卡器"));
                //通讯图标显示
                ListViewItem list1 = null;
                for (int k = 0; k < lvDoor.Items.Count; k++)
                {
                    list1 = this.lvDoor.Items[k];
                    DoorStatus(list1, basefun.valtag(msg, "Success"));
                }
                if ("true" != basefun.valtag(msg, "Success"))
                    continue;
                //对比最后刷卡时间
                string dtstr = basefun.valtag(msg, "{刷卡时间}");
                if (!string.IsNullOrEmpty(dtstr) && !isrealtime)
                {
                    if (dtpre < Convert.ToDateTime(dtstr))
                        isrealtime = true;
                }
                //记录数列表
                int sum = Convert.ToInt32(basefun.valtag(msg, "{刷卡记录数}"));
                devrecord.Add(devps[7], sum);
                devinfo.Add(devps[7], devps);

                //记录数少于50则直接采集
                int rdcount = 0;
                for (int j = 0; j < sum && sum <= 50; j++)
                {
                    cmdP.ResetState();
                    cmdP.setCommand(tpl, "读取记录", tag);
                    CommiManager.GlobalManager.SendCommand(target, cmdP);
                    if (!cmdP.EventWh.WaitOne(1000, false))
                        continue;
                    msg = cmdP.ResponseFormat;
                    if ("true" != basefun.valtag(msg, "Success"))
                        break;
                    string state = basefun.valtag(msg, "状态编号");
                    state = this.getStateDoorCard(state);
                    psdata["控制器"] = devps[8];
                    psdata["读卡器"] = DKQNUN;
                    psdata["时间"] = basefun.valtag(msg, "{刷卡时间}");
                    psdata["卡号"] = basefun.valtag(msg, "{卡号}");
                    if (devps[10] == "考勤机")
                        psdata["状态"] = "考勤机";
                    else
                        psdata["状态"] = state;
                    this.Query.ExecuteNonQuery("采集门禁数据", psdata, psdata, psdata);
                    rdcount++;
                    //devrecord.Remove(devps[7]);
                    //DataRow drinfo = dvinfo.Table.NewRow();
                    //drinfo["状态"] = state;
                    //drinfo["卡号"] = basefun.valtag(msg, "{卡号}");
                    //int dkq = Convert.ToInt16(basefun.valtag(msg, "读卡器"));
                    //dkq = dkq + 1;
                    //drinfo["门"] = doorName + "(" + Convert.ToString(dkq) + ")";
                    //drinfo["设备地址"] = basefun.valtag(msg, "{设备地址}");
                    //drinfo["时间"] = basefun.valtag(msg, "{刷卡时间}");
                    //dvinfo.Table.Rows.InsertAt(drinfo, 0);
                    //gridViewInfo.SelectRow(0);
                }

                if (rdcount > 0)
                    dataCount = rdcount.ToString();
                else
                    dataCount = "0";
                //检查状态改变则记录
                this.validateSate(lvDoor.Items[i], devps[8], msg, devps[10]);
            }
            //当前是实时状态,则忽略采集数据
            if (isrealtime || devrecord.Count < 1)
            {
                foreach (CommiTarget tar in targetlist)
                    CommiManager.GlobalManager.RemoveCommand(tar, cmdP);
                isRunning = false;
                return;
            }
            string[] info = new string[0];
            int summax = 0;
            foreach (string key in devrecord.Keys)
            {
                if (summax >= devrecord[key])
                    continue;
                summax = devrecord[key];
                info = devinfo[key];
            }
            if (summax < 1)
            {
                foreach (CommiTarget tar in targetlist)
                    CommiManager.GlobalManager.RemoveCommand(tar, cmdP);
                isRunning = false;
                return;
            }
            CommiTarget dev = this.getTarget(info);
            //记录数少于50则直接采集
            int rdcountmax = 0;
            for (int j = 0; j < summax && null != dev; j++)
            {
                string tag = "@设备地址=" + info[7];
                cmdP.ResetState();
                cmdP.setCommand(tpl, "读取记录", tag);
                CommiManager.GlobalManager.SendCommand(dev, cmdP);
                if (!cmdP.EventWh.WaitOne(1000, false))
                    continue;
                string msg = cmdP.ResponseFormat;
                if ("true" != basefun.valtag(msg, "Success"))
                    break;
                string state = basefun.valtag(msg, "状态编号");
                state = this.getStateDoorCard(state);
                psdata["控制器"] = info[8];
                psdata["读卡器"] = DKQNUN;
                psdata["卡号"] = basefun.valtag(msg, "{卡号}");
                psdata["时间"] = basefun.valtag(msg, "{刷卡时间}");
                psdata["状态"] = state;
                this.Query.ExecuteNonQuery("采集门禁数据", psdata, psdata, psdata);
                rdcountmax++;
            }
            foreach (CommiTarget tar in targetlist)
                CommiManager.GlobalManager.RemoveCommand(tar, cmdP);
            isRunning = false;
        }

        string doorName = "";
        string DKQNUN = "0";
        /// <summary>
        /// 获取对应门信息
        /// </summary>
        /// <param name="KZQType"></param>
        /// <returns></returns>
        public string getDKQ(string KZQType, string ResponseDKQ)
        {
            switch (KZQType)
            {
                case "单门双向":
                    break;
                case "四门单向":
                    DKQNUN = ResponseDKQ;
                    break;
                case "双门双向":
                    if (ResponseDKQ == "0" || ResponseDKQ == "1")
                        DKQNUN = "0";
                    else if (ResponseDKQ == "2" || ResponseDKQ == "3")
                        DKQNUN = "1";
                    break;
                default:
                    break;
            }
            this.paramwin["分组ID"] = FZID;
            this.paramwin["读卡器号"] = DKQNUN;
            //得到数据源
            DataTable tabdev = ds.Tables["读卡器"];
            tabdev.Clear();
            this.Query.FillDataSet(tabdev.TableName, this.paramwin, this.ds);
            this.Query.FillDataSet(tabStateDoor.TableName, this.paramwin, this.ds);
            string[] cols ={ "通讯类别", "串口", "波特率", "数据位", "停止位", "IP地址", "端口", "站址", "控制器", "名称", "控制器类型", "读卡器号" };
            if (tabdev.Rows.Count < 1 || tabdev == null) return "";
            doorName = Convert.ToString(tabdev.Rows[0]["名称"]);
            return Convert.ToString(tabdev.Rows[0]["读卡器号"]);
        }
        /// <summary>
        /// 获取数据行的设备目标位置参数
        /// 记录包含字段【访问方式】(TCP/UDP/SerialPort)、【端口】(60000/COM1)、【地址】(192.168.1.146)
        /// </summary>
        /// <param name="dr">数据记录</param>
        /// <returns></returns>
        private CommiTarget getTarget(string[] devps)
        {
            string[] cols ={ "通讯类别", "串口", "波特率", "数据位", "停止位", "IP地址", "端口", "站址", "控制器", "名称" };

            if (null == devps || devps.Length < 7)
                return null;
            CommiTarget target = new CommiTarget();
            target.setProtocol(Protocol.PTLDoor);
            CommiType commiType = CommiType.UDP;
            string stype = Convert.ToString(devps[0]);
            switch (stype)
            {
                case "TCP/IP(局域网)":
                    commiType = CommiType.TCP; break;
                case "UDP/IP(局域网)":
                    commiType = CommiType.UDP; break;
                default:
                    commiType = CommiType.SerialPort; break;
            }
            try
            {
                if (CommiType.SerialPort == commiType)
                {
                    string portname = Convert.ToString(devps[1]);
                    int baudRate = Convert.ToInt16(devps[2]);
                    int dataBits = Convert.ToInt16(devps[3]);
                    decimal s = Convert.ToDecimal(devps[4]);
                    StopBits sb = StopBits.None;
                    if (1 == s) sb = StopBits.One;
                    else if (2 == s) sb = StopBits.Two;
                    else if (1 < s && s < 2) sb = StopBits.OnePointFive;

                    target.SetProtocolParam(portname, baudRate, Parity.None, dataBits, sb);
                }
                else
                {
                    string addr = Convert.ToString(devps[5]);
                    int port = Convert.ToInt32(devps[6]);
                    target.SetProtocolParam(addr, port, commiType);
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return target;
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
                default: statecode = "正常"; break;
            }
            return statecode;
        }

        /// <summary>
        /// 验证状态信息,对状态对比变化时生成事件记录
        /// </summary>
        /// <param name="item">巡检项</param>
        /// <param name="devid">控制器ID</param>
        /// <param name="taginfo">状态信息tag标记值</param>
        private void validateSate(ListViewItem item, string devid, string taginfo, string Type)
        {
            if (null == item || string.IsNullOrEmpty(devid) || string.IsNullOrEmpty(taginfo))
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
            string msg = "", alarmmsg = "";
            bool isalarm = false;
            NameObjectList psevent = new NameObjectList();
            psevent["控制器"] = new Guid(devid);
            psevent["时间"] = basefun.valtag(taginfo, "{当前时间}");
            alarm = new string[] { "警报", "报警", "故障", "按钮", "门磁", "继电器" };
            foreach (string state in alarm)
            {
                string filter = "控制器='{0}' and 类别='{1}'";
                DataRow[] drs = this.tabStateDoor.Select(string.Format(filter, devid, state));
                DataRow dr = null;
                if (null == drs || drs.Length < 1)
                {
                    dr = this.tabStateDoor.NewRow();
                    dr["控制器"] = devid;
                    dr["类别"] = state;
                    dr["更新日期"] = basefun.valtag(taginfo, "{刷卡时间}");
                    this.tabStateDoor.Rows.Add(dr);
                }
                else
                    dr = drs[0];
                //对比状态生成事件
                psevent["事件"] = state;
                string tagorgi = Convert.ToString(dr["内容"]);
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
                            msg += "；" + st + "(" + alarmmsg + ")";
                        }
                        else
                        {
                            psevent.Remove("编号");
                            psevent["内容"] = st;
                            msg += "；" + st;
                        }
                        if ("1" == valnew)
                            this.Query.ExecuteNonQuery("发生门禁事件", psevent, psevent, psevent);
                        else
                            this.Query.ExecuteNonQuery("结束门禁事件", psevent, psevent, psevent);
                    }
                    else
                    {
                        psevent["编号"] = inum.ToString();
                        psevent["内容"] = st + "变位 " + valnew;
                        // msg += "；" + st + "变位 " + valnew;
                        this.Query.ExecuteNonQuery("门禁变位事件", psevent, psevent, psevent);
                    }
                }//foreach (string st in dictstate[state])
                if (!ischanged) continue;
                psevent["类别"] = state;
                psevent["内容"] = dr["内容"] = tagnews;
                this.Query.ExecuteNonQuery("门禁巡检状态", psevent, psevent, psevent);
                //报警内容变化时,先结束原警报事件
                if ("警报" == state)
                {
                    isalarm = !string.IsNullOrEmpty(alarmmsg);
                    psevent.Remove("编号");
                    this.Query.ExecuteNonQuery("结束门禁事件", psevent, psevent, psevent);
                }
            }//foreach (string state in alarm)
            //刷卡记录
            string dtcard = basefun.valtag(taginfo, "{刷卡时间}");
            string devname = item.Text.Substring(0, item.Text.IndexOf("("));
            if (!string.IsNullOrEmpty(dtcard) && DateTime.MinValue < Convert.ToDateTime(dtcard))
            {
                string rsl = basefun.valtag(taginfo, "状态编号");
                rsl = this.getStateDoorCard(rsl);
                msg = rsl;
            }
            if (string.IsNullOrEmpty(msg))
                return;
            if (msg.StartsWith("；"))
                msg = msg.Substring(1);
            if (string.IsNullOrEmpty(doorName)) return;
            DataView dvinfo = gridViewInfo.DataSource as DataView;
            DataRow drinfo = dvinfo.Table.NewRow();
            if (msg == "正常")
            {
                drinfo["时间"] = basefun.valtag(taginfo, "{刷卡时间}");
                drinfo["采集记录数"] = dataCount;
            }
            else
            {
                drinfo["时间"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                drinfo["采集记录数"] = dataCount;
            }
            drinfo["卡号"] = basefun.valtag(taginfo, "{卡号}");
            int dkq = 1;
            if (Type == "考勤机")
            {
                if (string.IsNullOrEmpty(basefun.valtag(taginfo, "{卡号}")))
                    drinfo["状态"] = "此卡无效";
                else
                    drinfo["状态"] = "考勤机";
            }
            else
                drinfo["状态"] = msg;
            dkq = Convert.ToInt32(basefun.valtag(taginfo, "读卡器")) + 1;
            drinfo["门"] = doorName + "(" + Convert.ToString(dkq) + ")";
            drinfo["设备地址"] = basefun.valtag(taginfo, "{设备地址}");
            dvinfo.Table.Rows.InsertAt(drinfo, 0);
            gridViewInfo.SelectRow(0);
            if (this.gridViewInfo.RowCount > 15)
                this.gridViewInfo.DeleteRow(this.gridViewInfo.RowCount - 1);
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sbtClose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
    }
}