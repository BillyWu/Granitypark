using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Common.Tools;
using Granity.winTools;
using Estar.Business.DataManager;
using Granity.CardOneCommi;
using Granity.communications;
using System.IO.Ports;

namespace Granity.granityMgr.Eatery
{
    public partial class FrmEateryMonitor : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "消费监控";//单元名称
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;

        /// <summary>
        /// 门禁巡检状态记录
        /// </summary>
        private DataTable tabStateEatery = null;

        public FrmEateryMonitor()
        {
            InitializeComponent();
        }

        private void FrmEateryMonitor_Load(object sender, EventArgs e)
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
            DataTable tab = this.ds.Tables["餐厅树"];
            this.bindMgr.BindTrv(this.trvEateryStall, tab, "名称", "ID", "PID", "@ID={ID},@PID={PID}");
            this.trvEateryStall.ExpandAll();
            this.tabStateEatery = this.ds.Tables["消费巡检状态"];
        }

        private void trvEateryStall_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (null == e.Node)
                return;
            string tag = Convert.ToString(e.Node.Tag);
            this.paramwin["餐厅ID"] = basefun.valtag(tag, "ID");
            //得到数据源
            DataTable tabdev = ds.Tables["餐厅设备列表"];
            tabdev.Clear();
            this.lvEatery.Clear();
            this.dtprelist.Clear();
            this.Query.FillDataSet(tabdev.TableName, this.paramwin, this.ds);
            this.Query.FillDataSet(tabStateEatery.TableName, this.paramwin, this.ds);
            string[] cols ={ "通讯类别", "串口", "波特率", "数据位", "停止位", "IP地址", "端口", "站址", "ID", "名称" };
            for (int i = 0; i < tabdev.Rows.Count; i++)
            {
                DataRow drdev = tabdev.Rows[i];
                string txt = Convert.ToString(drdev["名称"]);
                ListViewItem item = new ListViewItem(txt);
                item.ImageIndex = 0;
                item.Text = txt;
                tag = "";
                for (int c = 0; c < cols.Length; c++)
                    tag += "," + Convert.ToString(drdev[cols[c]]);
                if (tag.StartsWith(","))
                    tag = tag.Substring(1);
                item.Tag = tag;
                this.lvEatery.Items.Add(item);
            }
        }

        /// <summary>
        /// 是否正在巡检中
        /// </summary>
        private bool isRunning = false;
        private Dictionary<string, DateTime> dtprelist = new Dictionary<string, DateTime>();
        /// <summary>
        /// 定时巡检,支持代理巡检,在检测到门禁记录在50—1000记录时则只取最新记录,其他累积记录到1000时再完全下载
        ///          另有选项强制下载全部记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        public void EateryStatus(ListViewItem lst, string Status)
        {
            if (Status == "操作成功！")

                lst.ImageIndex = 1;
            else
                lst.ImageIndex = 0;
        }
        private void tmRefresh_Tick(object sender, EventArgs e)
        {
            if (isRunning) return;
            isRunning = true;
            //巡检过的设备地址加入devs中,避免重复巡检
            string devs = ",", tpl = "消费";
            // isrealtime/是否实时状态,在最后刷卡时间5分钟不变化时可对累积数据大批量采集
            bool isrealtime = false;
            TimeSpan tspre = new TimeSpan(0, 5, 0);
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
            for (int i = 0; i < lvEatery.Items.Count; i++)
            {
                string tag = Convert.ToString(lvEatery.Items[i].Tag);
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
                ListViewItem list1 = this.lvEatery.Items[i];
                if (!cmdP.EventWh.WaitOne(300, false))
                    continue;
                string msg = cmdP.ResponseFormat;

                EateryStatus(list1, basefun.valtag(msg, "{状态}"));
                if ("true" != basefun.valtag(msg, "Success"))
                    continue;
                string val = basefun.valtag(msg, "工作中");
                if (!this.dtprelist.ContainsKey(devps[7]))
                    this.dtprelist.Add(devps[7], DateTime.Now.AddMinutes(-4));
                if ("1" == val)
                    this.dtprelist[devps[7]] = DateTime.Now;
                //记录数列表
                int sum = Convert.ToInt32(basefun.valtag(msg, "{采集标志}"));
                devrecord.Add(devps[7], sum);
                devinfo.Add(devps[7], devps);
                //检查状态改变则记录
                this.validateSate(lvEatery.Items[i], devps[8], msg);
            }
            foreach (string key in devrecord.Keys)
            {
                if (devrecord[key] < 1 || !this.dtprelist.ContainsKey(key) || !devinfo.ContainsKey(key))
                    continue;
                if (DateTime.Now - this.dtprelist[key] < tspre)
                    continue;
                //在间隔tspre(5分钟)没有工作状态，则可以采集10条记录
                string[] info = devinfo[key];
                for (int i = 0; i < 11; i++)
                {
                    string tag = "@设备地址=" + info[7];
                    CommiTarget target = this.getTarget(info);
                    if (null == target) continue;
                    if (i < 1)
                        cmdP.setCommand(tpl, "取当前消费记录", tag);
                    else
                        cmdP.setCommand(tpl, "取下一条消费记录", tag);
                    cmdP.ResetState();
                    CommiManager.GlobalManager.SendCommand(target, cmdP);
                    if (!cmdP.EventWh.WaitOne(300, false))
                        continue;
                    string msg = cmdP.ResponseFormat;
                    if ("true" != basefun.valtag(msg, "Success"))
                        continue;
                    if (string.IsNullOrEmpty(basefun.valtag(msg, "{卡号}")))
                        break;
                    NameObjectList ps = ParamManager.createParam(msg);
                    ps["消费机"] = info[8];
                    bool success = this.Query.ExecuteNonQuery("采集数据", ps, ps, ps);
                    AddEateryStatus(msg);
                    if (!success) break;
                }
                break;
            }
            foreach (CommiTarget tar in targetlist)
                CommiManager.GlobalManager.RemoveCommand(tar, cmdP);
            isRunning = false;
        }
        int i = 0;
        public void AddEateryStatus(string msg)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = this.dbgrid.DataSource as DataTable;
            if (null == tab) return;
            string CardMoney = "";
            string CardAddMoney = "";
            if (tab.Rows.Count > 0)
            {
                CardMoney = tab.Rows[tab.Rows.Count - 1]["卡余额"].ToString();
                CardAddMoney = tab.Rows[tab.Rows.Count - 1]["补助余额"].ToString();
            }
            if (CardMoney == basefun.valtag(msg, "{卡余额}") && CardAddMoney == basefun.valtag(msg, "{累计补助金额}")) return;
            if (i == 0) gridViewInfo.DeleteRow(0);
            DataRow dr = tab.NewRow();
            dr["卡号"] = basefun.valtag(msg, "{卡号}");
            dr["消费时间"] = basefun.valtag(msg, "{消费时间}");
            dr["卡余额"] = basefun.valtag(msg, "{卡余额}");
            dr["金额"] = basefun.valtag(msg, "{消费金额}");
            dr["状态"] = basefun.valtag(msg, "{状态}");
            dr["消费机号"] = basefun.valtag(msg, "{消费机号}");
            dr["补助余额"] = basefun.valtag(msg, "{累计补助金额}");
            dr["序号"] = i + 1;
            tab.Rows.Add(dr);
            i++;
            if (this.gridViewInfo.RowCount > 15)
                this.gridViewInfo.DeleteRow(this.gridViewInfo.RowCount - 1);
        }
        /// <summary>
        /// 获取数据行的设备目标位置参数
        /// 记录包含字段【访问方式】(TCP/UDP/SerialPort)、【端口】(60000/COM1)、【地址】(192.168.1.146)
        /// </summary>
        /// <param name="dr">数据记录</param>
        /// <returns></returns>
        private CommiTarget getTarget(string[] devps)
        {
            string[] cols ={ "通讯类别", "串口", "波特率", "数据位", "停止位", "IP地址", "端口", "站址", "名称" };

            if (null == devps || devps.Length < 7)
                return null;
            CommiTarget target = new CommiTarget();
            target.setProtocol(Protocol.PTLEatery);
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
        /// 验证状态信息,对状态对比变化时生成事件记录
        /// </summary>
        /// <param name="item">巡检项</param>
        /// <param name="devid">控制器ID</param>
        /// <param name="taginfo">状态信息tag标记值</param>
        private void validateSate(ListViewItem item, string devid, string taginfo)
        {
            if (null == item || string.IsNullOrEmpty(devid) || string.IsNullOrEmpty(taginfo))
                return;
            Dictionary<string, string[]> dictstate = new Dictionary<string, string[]>();
            string[] alarm ={ "时间格式错", "读头故障", "权限校验错", "时段参数错", "收费参数错", "黑名单错" };
            dictstate.Add("内部状态", alarm);
            //检查前后状态改变；alarmmsg报警信息,isalarm当前是否报警改变
            string msg = "";
            NameObjectList psevent = new NameObjectList();
            psevent["消费机"] = new Guid(devid);
            psevent["时间"] = basefun.valtag(taginfo, "{当前时间}");
            alarm = new string[] { "内部状态" };
            foreach (string state in alarm)
            {
                string filter = "消费机='{0}' and 类别='{1}'";
                DataRow[] drs = this.tabStateEatery.Select(string.Format(filter, devid, state));
                DataRow dr = null;
                if (null == drs || drs.Length < 1)
                {
                    dr = this.tabStateEatery.NewRow();
                    dr["消费机"] = devid;
                    dr["类别"] = state;
                    dr["更新日期"] = basefun.valtag(taginfo, "{当前时间}");
                    this.tabStateEatery.Rows.Add(dr);
                }
                else
                {

                }
                if (drs.Length < 1) return;
                dr = drs[0];
                //对比状态生成事件
                psevent["事件"] = state;
                string tagorgi = Convert.ToString(dr["内容"]);
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
                    if ("1" != valnew)
                        this.Query.ExecuteNonQuery("结束消费机事件", psevent, psevent, psevent);
                    else
                    {
                        msg += "；" + st;
                        this.Query.ExecuteNonQuery("发生消费机事件", psevent, psevent, psevent);
                    }
                }
                if (!ischanged) continue;
                psevent["类别"] = state;
                psevent["内容"] = dr["内容"] = tagnews;
                this.Query.ExecuteNonQuery("消费巡检状态", psevent, psevent, psevent);
            }//foreach (string state in alarm)
            if (string.IsNullOrEmpty(msg))
                return;
            if (msg.StartsWith("；"))
                msg = item.Text + "：" + msg.Substring(1);
            //有信息则记录日志
            DataView dvinfo = gridViewInfo.DataSource as DataView;
            DataRow drinfo = dvinfo.Table.NewRow();
            drinfo["内容"] = msg;
            dvinfo.Table.Rows.InsertAt(drinfo, 0);
            for (int i = dvinfo.Count - 1; i > 100; i--)
                dvinfo.Delete(i);
            gridViewInfo.SelectRow(0);
        }
        /// <summary>
        /// 关闭窗体
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