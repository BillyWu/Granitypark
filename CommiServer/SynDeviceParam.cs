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
    /// 同步设备参数,更新黑白名单和设备控制参数
    /// </summary>
    public class SynDeviceParam
    {
        /// <summary>
        /// 服务定时器
        /// </summary>
        private System.Timers.Timer tmService = new System.Timers.Timer();
        /// <summary>
        /// 当前是否正在执行
        /// </summary>
        private bool isRuning = false;
        /// <summary>
        /// 协议数据源
        /// </summary>
        private const string dbSrc = "基础类";
        /// <summary>
        /// 数据查询
        /// </summary>
        private QueryDataRes query = new QueryDataRes(dbSrc);

        /// <summary>
        /// 调用多个组合指令时,传递参数类
        /// </summary>
        private class TransArg
        {
            /// <summary>
            /// 中转指令
            /// </summary>
            public CmdFileTrans trans;
            /// <summary>
            /// 通讯服务地址
            /// </summary>
            public CommiTarget target;
            /// <summary>
            /// 通讯目的地代理IP地址
            /// </summary>
            public IPAddress proxy;
            /// <summary>
            /// 通讯目的
            /// </summary>
            public CommiTarget dest;
            /// <summary>
            /// 当前设备ID
            /// </summary>
            public string devID;
            /// <summary>
            /// 通讯站址
            /// </summary>
            public string addrst;
            /// <summary>
            /// 附加指令标记,tag格式信息,可实现指令过滤执行
            /// </summary>
            public string attrCmdtag;
        }

        public SynDeviceParam()
        {
            //20分钟执行一次
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
        /// 启动定时执行,每20分钟执行一次检查
        /// </summary>
        public void Start()
        {
            //20分钟执行一次
            tmService.Interval = 1200000;
            tmService.Enabled = true;
            tmService.Start();
        }

        /// <summary>
        /// 停止定时执行
        /// </summary>
        public void Stop()
        {
            //20分钟执行一次
            tmService.Enabled = false;
            tmService.Stop();
        }

        /// <summary>
        /// 更新设备控制参数
        /// </summary>
        public void CommiDevice()
        {
        }

        /// <summary>
        /// 更新所有设备黑白名单
        /// </summary>
        /// <returns>返回错误提示，无错误提示成功</returns>
        public string downCardALL()
        {
            NameObjectList ps = new NameObjectList();
            DataTable tab = this.query.getTable("设备列表", ps);
            if (null == tab || tab.Rows.Count < 1)
                return "";
            string devid = "";
            string attrcmd = "|下载ID白名单|下载黑名单|删除白名单|删除ID白名单|下载白名单|";

            //设置中转服务器
            CmdFileTrans trans = new CmdFileTrans(false);
            int port = 2010;
            string sport = DataAccRes.AppSettings("Granity文件服务");
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
                //代理IP,检查代理是否本地local,通讯目的
                string addr = Convert.ToString(dr["IP地址"]);
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
                    data["IP地址"] = addr;
                    ServiceTool.LogMessage(ex, data, EventLogEntryType.Error);
                    msg += string.Format("、{0}({1})", dr["名称"], dr["IP地址"]);
                    continue;
                }
                CommiTarget dest = this.getTarget(dr);
                if (null == dest)
                {
                    msg += string.Format("、{0}({1})", dr["名称"], dr["IP地址"]);
                    continue;
                }
                string tpl = Convert.ToString(dr["通讯协议"]);
                bool rtn = true;
                switch (tpl)
                {
                    case "停车场":
                        dest.setProtocol(Protocol.PTLPark);
                        rtn = this.commiDevicePark(tarsrv, trans, proxy, dest, dr, attrcmd);
                        break;
                    case "门禁":
                        dest.setProtocol(Protocol.PTLDoor);
                        rtn = this.commiDeviceDoor(tarsrv, trans, proxy, dest, dr, attrcmd);
                        break;
                    case "消费":
                        dest.setProtocol(Protocol.PTLEatery);
                        rtn = this.commiDeviceEatery(tarsrv, trans, proxy, dest, dr, attrcmd);
                        break;

                }
                if (!rtn)
                    msg += string.Format("、{0}({1})", dr["名称"], dr["IP地址"]);
            }
            if (!string.IsNullOrEmpty(msg))
                msg = msg.Substring(1);
            return msg;
        }

        /// <summary>
        /// 强制对设备做全部初始化和下载参数,更新黑白名单
        /// </summary>
        /// <param name="tagdevcmds">设备ID和下载指令,tag格式,其中指令以"|"分割</param>
        public bool CommiDevice(string tagdevcmds)
        {
            string devID = basefun.valtag(tagdevcmds, "设备ID");
            string attrcmd = "|" + basefun.valtag(tagdevcmds, "指令") + "|";
            if (string.IsNullOrEmpty(tagdevcmds))
                return false;

            //设置中转服务器
            CmdFileTrans trans = new CmdFileTrans(false);
            int port = 2010;
            string sport = DataAccRes.AppSettings("Granity文件服务");
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
            ps["设备ID"] = devID;
            DataTable tab = this.query.getTable("设备通讯参数", ps);
            if (null == tab || tab.Rows.Count < 1)
                return true;
            DataRow dr = tab.Rows[0];
            //代理IP,检查代理是否本地local,通讯目的
            string addr = Convert.ToString(dr["IP地址"]);
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
                data["IP地址"] = addr;
                ServiceTool.LogMessage(ex, null, EventLogEntryType.Error);
                return false;
            }
            CommiTarget dest = this.getTarget(dr);
            if (null == dest)
                return false;

            string tpl = Convert.ToString(dr["通讯协议"]);
            switch (tpl)
            {
                case "停车场":
                    dest.setProtocol(Protocol.PTLPark);
                    return this.commiDevicePark(tarsrv, trans, proxy, dest, dr, attrcmd);
                case "门禁":
                    dest.setProtocol(Protocol.PTLDoor);
                    return this.commiDeviceDoor(tarsrv, trans, proxy, dest, dr, attrcmd);
                case "消费":
                    dest.setProtocol(Protocol.PTLEatery);
                    return this.commiDeviceEatery(tarsrv, trans, proxy, dest, dr, attrcmd);
            }
            return true;
        }


        /// <summary>
        /// 停车场
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
            //本机通讯或目标是代理机本机则直接通讯
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
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{状态}")))
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
                    //同步通讯等待时间1.5秒
                    if (trans.EventWh.WaitOne(1500, false))
                        data = trans.FileContext;
                    msg = cmdP.FormatResponse(data);
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{状态}")))
                        break;
                }
            }
            if ("true" != basefun.valtag(msg, "Success"))
            {
                NameValueCollection attr = new NameValueCollection();
                attr["协议"] = tpl;
                attr["操作"] = tagdata + "\r\n" + cmd;
                ServiceTool.LogMessage(msg, attr, EventLogEntryType.Error);
                return msg;
            }
            return msg;
        }


        /// <summary>
        /// 中转执行指令
        /// </summary>
        /// <param name="tarsrv">服务器</param>
        /// <param name="trans">传输实例</param>
        /// <param name="proxy">设备代理电脑IP地址</param>
        /// <param name="dest">设备目标</param>
        /// <param name="cmdP">指令实例</param>
        /// <param name="tpl">协议名称</param>
        /// <param name="cmd">指令名称</param>
        /// <param name="tagdata">指令数据 tag格式</param>
        /// <returns>是否执行成功</returns>
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
            //本机通讯或目标是代理机本机则直接通讯
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
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{状态}")))
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
                    //同步通讯等待时间1.5秒
                    if (trans.EventWh.WaitOne(1500, false))
                        data = trans.FileContext;
                    msg = cmdP.FormatResponse(data);
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{状态}")))
                        break;
                }
            }
            if ("true" != basefun.valtag(msg, "Success"))
            {
                NameValueCollection attr = new NameValueCollection();
                attr["协议"] = tpl;
                attr["操作"] = tagdata + "\r\n" + cmd;
                ServiceTool.LogMessage(msg, attr, EventLogEntryType.Error);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 中转执行指令，返回响应结果
        /// </summary>
        /// <param name="tarsrv">服务器</param>
        /// <param name="trans">传输实例</param>
        /// <param name="proxy">设备代理电脑IP地址</param>
        /// <param name="dest">设备目标</param>
        /// <param name="cmdP">指令实例</param>
        /// <param name="tpl">协议名称</param>
        /// <param name="cmd">指令名称</param>
        /// <param name="tagdata">指令数据 tag格式</param>
        /// <returns>返回执行结果</returns>
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
            //本机通讯或目标是代理机本机则直接通讯
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
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{状态}")))
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
                    //同步通讯等待时间1.5秒
                    if (trans.EventWh.WaitOne(1500, false))
                        data = trans.FileContext;
                    msg = cmdP.FormatResponse(data);
                    if ("true" == basefun.valtag(msg, "Success"))
                        break;
                    if (!string.IsNullOrEmpty(basefun.valtag(msg, "{状态}")))
                        break;
                }
            }
            return msg;
        }
        /// <summary>
        /// 10进制转16进制
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public string sValue10to16(string str)
        {
            if (string.IsNullOrEmpty(str)) str = "00";
            string s = Convert.ToInt32(str).ToString("X");
            if (s.Length % 2 != 0)
            {
                s = "0" + s;//不到位则补，如则变位的格式
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
        #region 停车场通讯

        /// <summary>
        /// 停车场设备初始化和下载参数,更新黑白名单
        /// </summary>
        /// <param name="tarsrv">中转服务器</param>
        /// <param name="trans">传输指令</param>
        /// <param name="proxy">设备通讯代理电脑IP地址</param>
        /// <param name="dest">目标设备</param>
        /// <param name="drdevice">设备信息记录行</param>
        /// <param name="attrcmd">执行指令</param>
        /// <returns>返回通讯下载参数是否成功</returns>
        private bool commiDevicePark(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, DataRow drdevice, string attrcmd)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == drdevice || string.IsNullOrEmpty(attrcmd))
                return true;
            CmdProtocol cmdP = new CmdProtocol(false);
            string devID = Convert.ToString(drdevice["ID"]);
            NameObjectList ps = new NameObjectList();
            ps["设备ID"] = devID;
            bool success = false;

            //系统时间
            string tpl = "停车场";
            string valst = Convert.ToString(drdevice["站址"]);
            DateTime dtsystem = Convert.ToDateTime(this.query.ExecuteScalar("系统时间", ps));
            string cmdstr = ",格式化,初始化ID白名单,初始化黑名单,加载系统时间,下载控制参数,读取控制参数,下载屏显信息,";
            string[] cmds = attrcmd.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmdstr.IndexOf("," + cmds[i] + ",") < 0)
                    continue;
                string tagdata = "@设备地址=" + valst;
                if ("下载控制参数" == cmds[i])
                {
                    DataTable tabctrlpm = this.query.getTable("设备控制参数", ps);
                    if (null == tabctrlpm || tabctrlpm.Rows.Count < 1)
                        continue;
                    tagdata = this.getctrlpm(tabctrlpm.Rows[0], valst);
                }
                else if ("加载系统时间" == cmds[i])
                {
                    tagdata = basefun.setvaltag(tagdata, "{系统时间}", dtsystem.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else if ("下载屏显信息" == cmds[i])
                {
                    DataTable tabScress = this.query.getTable("下载屏显信息", ps);
                    if (null == tabScress || tabScress.Rows.Count < 1)
                        continue;
                    string cmd = "";
                    string downtype = "";
                    string datatype = "";
                    string message = "";
                    if (string.IsNullOrEmpty(Convert.ToString(tabScress.Rows[0]["屏显信息"])))
                    {
                        cmd = "0000" + Internal_code(Convert.ToString(tabScress.Rows[0]["屏显信息"]));
                        downtype = "00";
                        datatype = "00";
                        message = Convert.ToString(tabScress.Rows[0]["屏显信息"]);
                    }
                    else
                    {
                        cmd = "0100" + Internal_code(Convert.ToString(tabScress.Rows[0]["打印信息"]));
                        downtype = "01";
                        datatype = "00";
                        message = Convert.ToString(tabScress.Rows[0]["打印信息"]);
                    }
                    tagdata = basefun.setvaltag(tagdata, "{下载方式}", downtype);
                    tagdata = basefun.setvaltag(tagdata, "数据类型", datatype);
                    tagdata = basefun.setvaltag(tagdata, "自定义信息", Internal_code(message));
                    string sLen = (cmd.Length / 2).ToString("X");
                    tagdata = basefun.setvaltag(tagdata, "{命令长度}", sLen);
                }
                cmdP.setCommand(tpl, cmds[i], tagdata);
                success = this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, cmds[i], tagdata);
                if (!success) return false;
                //格式化时设备有2.5s不应期
                if ("格式化" == cmds[i])
                    System.Threading.Thread.Sleep(3500);
            }

            //建立传输指令参数实例
            TransArg arg = new TransArg();
            arg.trans = trans;
            arg.target = tarsrv;
            arg.proxy = proxy;
            arg.dest = dest;
            arg.devID = devID;
            arg.addrst = Convert.ToString(drdevice["站址"]);
            arg.attrCmdtag = attrcmd;

            //下载计费标准
            success = this.downPayRule(arg);
            //if (!success) return false;
            ////更新下载日期
            //this.query.ExecuteNonQuery("设备下载标志更新", ps, ps, ps);

            //重置黑白名单
            if (attrcmd.IndexOf("|格式化|") > -1 || attrcmd.IndexOf("|初始化ID白名单|") > -1 || attrcmd.IndexOf("|初始化黑名单|") > -1)
                this.query.ExecuteNonQuery("重置设备名单", ps, ps, ps);
            success = this.downparkCardList(arg);
            //设备蜂鸣提示
            string tag = "@设备地址=" + valst;
            this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, "蜂鸣", tag);
            return success;
        }

        /// <summary>
        /// 更新设备的黑白清名单,执行成功后更新黑白名单下载日期
        /// </summary>
        /// <param name="arg">传输指令参数</param>
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
            ps["设备ID"] = devID;
            //同步通讯等待时间15秒
            bool success = true;

            DataTable tab = this.query.getTable("设备清名单", ps);
            string tpl = "停车场", cmd = "删除ID白名单";
            string tagdata = "@设备地址=" + addrst;
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|删除ID白名单|") > -1)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    tagdata = basefun.setvaltag(tagdata, "{卡号}", Convert.ToString(dr["卡号"]));
                    bool rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                    success = success && rtn;
                    if (!rtn)
                    {
                        if (!testConnect(target, dest))
                            return false;
                        continue;
                    }
                    ps["状态"] = "清";
                    ps["卡号"] = Convert.ToString(dr["卡号"]);
                    this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                }
            tab = this.query.getTable("设备白名单", ps);
            cmd = "下载ID白名单";
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|下载ID白名单|") > -1)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    string[,] colmap ={ { "{卡号}", "卡号" }, { "{卡类}", "卡型" }, { "{车型}", "车型" }, { "{时段}", "时段" }, { "{有效日期}", "有效日期" } };
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
                    ps["状态"] = "白";
                    ps["卡号"] = Convert.ToString(dr["卡号"]);
                    ps["是否完成"] = 1;
                    ps["已下载"] = 1;
                    this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                }
            return success;
        }

        /// <summary>
        /// 更新设备的黑白清名单,执行成功后更新黑白名单下载日期
        /// </summary>
        /// <param name="arg">传输指令参数</param>
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
            ps["设备ID"] = devID;
            bool success = false;

            string tpl = "停车场";
            DataTable tabpay = this.query.getTable("收费标准列表", ps);
            string[,] dbpay ={ { "消费方式1", "收费标准1", "1", "下载收费标准1" }, { "消费方式2", "收费标准2", "2", "下载收费标准2" }, { "消费方式3", "收费标准3", "3", "下载收费标准3" },
                               { "消费方式4", "收费标准4", "4", "下载收费标准4" }, { "消费方式5", "收费标准5", "5", "下载收费标准5" }, { "消费方式6", "收费标准6", "6", "下载收费标准6" } };
            int ct = 0;
            if (null != tabpay && tabpay.Rows.Count > 0)
                ct = tabpay.Rows.Count;
            for (int j = 0; j < ct; j++)
            {
                DataRow drpay = tabpay.Rows[j];
                string ncar = tabpay.Rows[j]["车型"].ToString();
                ps["车型"] = ncar;
                string ntype = Convert.ToString(drpay["计费方式"]);
                int k = -1;
                for (int m = 0; m < dbpay.GetLength(0); m++)
                    if (ntype == dbpay[m, 0])
                    {
                        k = m; break;
                    }
                if (k < 0) continue;
                //指令过滤则忽略执行
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
                tag = basefun.setvaltag(tag, "设备地址", addrst);
                tag = basefun.setvaltag(tag, "{车型代码}", ncar);
                tag = basefun.setvaltag(tag, "{方式代码}", dbpay[k, 2]);
                //通讯下载收费参数
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, dbpay[k, 3], tag);
                if (!success) return false;
            }
            return true;
        }

        /// <summary>
        /// 从行记录获取tag格式控制参数
        /// </summary>
        /// <param name="dr">记录行</param>
        /// <param name="addrst">站址</param>
        /// <returns></returns>
        private string getctrlpm(DataRow dr, string addrst)
        {
            //@{出卡机欠量报警门限}=0,@{总车位数}=1000,@{脱机时间}.{脱机处理}=1,@{脱机时间}.{脱机时间}=25,
            //@{临时卡出卡选择}.{车型}=C,@{临时卡出卡选择}.{方式}=1,@{货币单位}=01,@{入口满位处理}.{入场}=0,@{入口满位处理}.{临时卡}=0,
            //@{外设配置}=00011001,@{放行控制}=00101001,@{场内场逻辑控制}=aa,@{车位占用卡处理}=01010000,
            //@{卡类允许}=11111111,@{通道选择}=00 00 01 10,@{有效期报警门限}=20,@{卡余额报警门警}=30,
            //@{中央收费}.{中央收费}=1,@{中央收费}.{有效时间}=30,@{进出逻辑控制}=AA,@{灯饰设置}.{h1}=10,
            //@{灯饰设置}.{m1}=30,@{灯饰设置}.{h2}=08,@{灯饰设置}.{m2}=30,@{保留车位}=10,@{场内场编号}=10,@{卡片使用区}=7,@{最高余额}=65535,@{最低余额}=20

            //@{帧头}=02,@{设备地址}=1,@{状态}=操作成功！,@{命令长度}=0040,@{本机地址}=1,@{系统标识符}=2102803,@{通信密码}=0,@{系统密码}=0,
            //@{用户密码}=1467233176,@{出卡机欠量报警门限}=0,@{总车位数}=0,@脱机处理=1,@脱机时间=80,@车型=0,@方式=6,@{货币单位}=0A,
            //@{入口满位处理}=00000000,@{外设配置}=10100100,@{放行控制}=00010100,@{场内场逻辑控制}=3,@{车位占用卡处理}=00010000,@{卡类允许}=11111111,
            //@{通道选择}=00001100,@{有效期报警门限}=30,@{卡余额报警门警}=80,@中央收费=0,@有效时间=0,@{进出逻辑控制}=AA,@灯1=00:00,@灯2=00:00,
            //@{保留车位}=0,@{场内场编号}=0,@{卡片使用区}=7,@{最高余额}=0,@{最低余额}=0,@{保留字}=0,@{校验字}=82,@{帧尾}=03

            //默认按键车型
            string type_id = Convert.ToInt32(Convert.ToInt32(dr["按键默认车型"]).ToString("X").Substring(0, 1), 16).ToString();
            //默认按键方式
            string type = Convert.ToInt32(dr["临时卡方式"]).ToString("X").Substring(1, 1);
            //进出地感
            string inout = "0";
            //Billy
            //DataRow dr1 = dr.Table["控制参数"].rows[0];
            if (Convert.ToInt32(addrst) < 129)
            {
                if (dr.Table.Columns.Contains("入场地感"))
                {
                    inout = Convert.ToString(dr["入场地感"]);
                }
            }
            else
            {
                inout = Convert.ToString(dr["有无地感"]);

            }
            string periphery = "00" + Convert.ToString(dr["远距读头2"]) + Convert.ToString(dr["远距读头1"]) + Convert.ToString(dr["收费显示屏"]) + Convert.ToString(dr["中文屏"]) + Convert.ToString(dr["有无车位屏"]) + inout;


            string[,] colmap ={ { "{有效期报警门限}", "有效期报警" }, { "{卡余额报警门警}", "卡余额报警" },{"{总车位数}","总车位数"},{"{保留车位}","保留车位"},
                                { "{场内场逻辑控制}", "场内场逻辑" },
                                {"{中央收费}.{中央收费}","是否中央收费"}, {"{中央收费}.{有效时间}","有效时分"},
                                {"{入口满位处理}.{入场}", "允许车辆入场" }, { "{入口满位处理}.{临时卡}", "允许出临时卡" }
                             };
            string[,] valmap ={ { "{脱机时间}.{脱机处理}", "1" }, { "{脱机时间}.{脱机时间}", "20" },{"{车位占用卡处理}","00010000"},{"{临时卡出卡选择}.{车型}",type_id},{"{临时卡出卡选择}.{方式}",type},
                                { "{货币单位}", "01" }, { "{卡片使用区}","70" }, {"{外设配置}",periphery},
                                {"{灯饰设置}.{h1}","00"}, {"{灯饰设置}.{m1}","00"}, {"{灯饰设置}.{h2}","00"}, {"{灯饰设置}.{m2}","00"}};
            string tagdata = "@设备地址=" + addrst;
            tagdata = basefun.setvaltag(tagdata, "{本机地址}", addrst);
            for (int i = 0; i < colmap.GetLength(0); i++)
            {
                string val = Convert.ToString(dr[colmap[i, 1]]);
                if (true.Equals(dr[colmap[i, 1]])) val = "1";
                if (false.Equals(dr[colmap[i, 1]])) val = "0";
                if ("1" == val && ("允许车辆入场" == colmap[i, 1] || "允许出临时卡" == colmap[i, 1]))
                    val = "10";
                tagdata = basefun.setvaltag(tagdata, colmap[i, 0], val);
            }
            for (int i = 0; i < valmap.GetLength(0); i++)
                tagdata = basefun.setvaltag(tagdata, valmap[i, 0], valmap[i, 1]);

            string v = "";

            if (true.Equals(dr["进出逻辑控制"]))
                tagdata = basefun.setvaltag(tagdata, "{进出逻辑控制}", "AA");
            else
                tagdata = basefun.setvaltag(tagdata, "{进出逻辑控制}", "86");


            string valk = "";
            string fx = "";
            string td = "";

            colmap = new string[,] { { "{卡类允许}", "卡类允许" }, { "{放行控制}", "放行控制" }, { "{通道选择}", "通道选择" } };

            valk = Convert.ToString(dr["Card一卡通"]) + Convert.ToString(dr["Card储值卡"]) + Convert.ToString(dr["Card贵宾卡"]) + Convert.ToString(dr["Card临免卡"]);
            valk += Convert.ToString(dr["Card免费卡"]) + Convert.ToString(dr["Card临时卡"]) + Convert.ToString(dr["Card时段卡"]) + Convert.ToString(dr["Card期卡"]);
            v = valk.Replace("True", "1").Replace("False", "0");


            valk = Convert.ToString(dr["FX一卡通"]) + Convert.ToString(dr["FX储值卡"]) + Convert.ToString(dr["FX贵宾卡"]) + Convert.ToString(dr["FX临免卡"]);
            valk += Convert.ToString(dr["FX免费卡"]) + Convert.ToString(dr["FX临时卡"]) + Convert.ToString(dr["FX时段卡"]) + Convert.ToString(dr["FX期卡"]);
            fx = valk.Replace("True", "1").Replace("False", "0");


            if (Convert.ToString(dr["A类车"]) != "False" || Convert.ToString(dr["B类车"]) != "False" || Convert.ToString(dr["C类车"]) != "False" || Convert.ToString(dr["D类车"]) != "False" || Convert.ToString(dr["E类车"]) != "False")
            {
                valk = "000" + Convert.ToString(dr["E类车"]) + Convert.ToString(dr["D类车"]) + Convert.ToString(dr["C类车"]) + Convert.ToString(dr["B类车"]) + Convert.ToString(dr["A类车"]);
            }
            else
            {
                valk = "11" + Convert.ToString(dr["TD一卡通"]) + Convert.ToString(dr["TD储值卡"]) + Convert.ToString(dr["TD贵宾卡"]);
                valk += Convert.ToString(dr["TD免费卡"]) + Convert.ToString(dr["TD时段卡"]) + Convert.ToString(dr["TD期卡"]);
            }
            td = valk.Replace("True", "1").Replace("False", "0");
            tagdata = basefun.setvaltag(tagdata, colmap[1, 0], fx);
            tagdata = basefun.setvaltag(tagdata, colmap[0, 0], v);
            tagdata = basefun.setvaltag(tagdata, colmap[2, 0], td);
            return tagdata;
        }

        /// <summary>
        /// 收费标准1 tag格式参数
        /// </summary>
        /// <param name="drpay">计费标准记录</param>
        /// <param name="tabdetail">标准规则明细</param>
        /// <returns>返回tag格式协议参数</returns>
        private string getpaypm1(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "免费时长", "{参数1}" }, { "基本费", "{参数2}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));

            string val = Convert.ToString(tabdetail.Rows[0]["跨天金额"]);
            tag = basefun.setvaltag(tag, "{参数3}", val);
            return tag;
        }
        /// <summary>
        /// 收费标准2 tag格式参数
        /// </summary>
        /// <param name="drpay">计费标准记录</param>
        /// <param name="tabdetail">标准规则明细</param>
        /// <returns>返回tag格式协议参数</returns>
        private string getpaypm2(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "免费时长", "{参数1}" }, { "基本费", "{参数3}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));
            string limit = Convert.ToString(tabdetail.Rows[0]["限额"]);
            if (Convert.ToString(tabdetail.Rows[0]["每日限额"]) == "False")
            {
                tag = basefun.setvaltag(tag, "{参数8}", "55");
                limit = Convert.ToString(tabdetail.Rows[0]["限额"]);
            }
            else
                tag = basefun.setvaltag(tag, "{参数8}", "00");
            tag = tag = basefun.setvaltag(tag, "{参数7}", limit);
            DataRow dr = tabdetail.Rows[0];
            colmap = new string[,] { { "基本费时长T1", "{参数2}" }, { "计费时长T2", "{参数4}" }, { "计费时长T3", "{参数5}" }, { "基本费F2", "{参数6}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            return tag;
        }
        /// <summary>
        /// 收费标准3 tag格式参数
        /// </summary>
        /// <param name="drpay">计费标准记录</param>
        /// <param name="tabdetail">标准规则明细</param>
        /// <returns>返回tag格式协议参数</returns>
        private string getpaypm3(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "免费时长", "{参数1}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));

            int k = 3;
            colmap = new string[,] { { "循环数", "{参数2}" }, { "T1", "{参数3}" }, { "F1", "{参数4}" }, { "T2", "{参数5}" }, { "F2", "{参数6}" }, { "T3", "{参数7}" }, { "F3", "{参数8}" }, { "T4", "{参数9}" }, { "F4", "{参数10}" }, { "T5", "{参数11}" }, { "F5", "{参数12}" } };
            for (int r = 0; r < tabdetail.Rows.Count; r++)
            {
                DataRow dr = tabdetail.Rows[r];
                for (int i = 0; i < colmap.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            }
            return tag;
        }
        /// <summary>
        /// 收费标准4 tag格式参数
        /// </summary>
        /// <param name="drpay">计费标准记录</param>
        /// <param name="tabdetail">标准规则明细</param>
        /// <returns>返回tag格式协议参数</returns>
        private string getpaypm4(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "免费时长", "{参数2}" }, { "基本费", "{参数3}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));


            colmap = new string[,] { { "时段数", "{参数1}" }, { "时间点T1", "{参数4}" }, { "跨段费F2", "{参数5}" }, { "时间点T2", "{参数6}" }, { "跨段费F2", "{参数7}" }, { "时间点T3", "{参数8}" }, { "跨段费F3", "{参数9}" }, { "时间点T4", "{参数10}" }, { "跨段费F4", "{参数11}" }, { "时间点T5", "{参数12}" }, { "跨段费F5", "{参数13}" } };
            for (int r = 0; r < tabdetail.Rows.Count; r++)
            {
                DataRow dr = tabdetail.Rows[r];
                for (int i = 0; i < colmap.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            }
            return tag;
        }
        /// <summary>
        /// 收费标准5 tag格式参数
        /// </summary>
        /// <param name="drpay">计费标准记录</param>
        /// <param name="tabdetail">标准规则明细</param>
        /// <returns>返回tag格式协议参数</returns>
        private string getpaypm5(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "免费时长", "{参数1}" }, { "基本费", "{参数3}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));


            colmap = new string[,] { { "基本费时长", "{参数2}" }, { "基本费后免费时长", "{参数4}" }, { "时间点T3", "{参数5}" }, { "时间点T4", "{参数6}" }, { "计费时长T5", "{参数7}" }, { "计费F2", "{参数8}" }, { "计费时长T6", "{参数9}" }, { "计费F3", "{参数10}" } };
            for (int r = 0; r < tabdetail.Rows.Count; r++)
            {
                DataRow dr = tabdetail.Rows[r];
                for (int i = 0; i < colmap.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            }
            return tag;
        }
        /// <summary>
        /// 收费标准6 tag格式参数
        /// </summary>
        /// <param name="drpay">计费标准记录</param>
        /// <param name="tabdetail">标准规则明细</param>
        /// <returns>返回tag格式协议参数</returns>
        private string getpaypm6(DataRow drpay, DataTable tabdetail)
        {
            if (null == drpay || null == tabdetail || tabdetail.Rows.Count < 1)
                return "";
            string tag = "";
            string[,] colmap ={ { "免费时长", "{参数1}" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(drpay[colmap[i, 0]]));

            colmap = new string[,] { { "24小时限额", "{参数2}" }, { "收费1", "{参数3}" }, { "收费2", "{参数4}" }, { "收费3", "{参数5}" }, { "收费4", "{参数6}" }, { "收费5", "{参数7}" }, { "收费6", "{参数8}" }, { "收费7", "{参数9}" }, { "收费8", "{参数10}" }, { "收费9", "{参数11}" }, { "收费10", "{参数12}" }, { "收费11", "{参数13}" }, { "收费12", "{参数14}" }, { "收费13", "{参数15}" }, { "收费14", "{参数16}" }, { "收费15", "{参数17}" }, { "收费16", "{参数18}" }, { "收费17", "{参数19}" }, { "收费18", "{参数20}" }, { "收费19", "{参数21}" }, { "收费20", "{参数22}" }, { "收费21", "{参数23}" }, { "收费22", "{参数24}" }, { "收费23", "{参数25}" }, { "收费24", "{参数26}" } };
            for (int r = 0; r < tabdetail.Rows.Count; r++)
            {
                DataRow dr = tabdetail.Rows[r];
                for (int i = 0; i < colmap.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmap[i, 1], Convert.ToString(dr[colmap[i, 0]]));
            }
            return tag;
        }

        #endregion


        #region 门禁通讯

        /// <summary>
        /// 停车场设备初始化和下载参数,更新黑白名单
        /// </summary>
        /// <param name="tarsrv">中转服务器</param>
        /// <param name="trans">传输指令</param>
        /// <param name="proxy">设备通讯代理电脑IP地址</param>
        /// <param name="dest">目标设备</param>
        /// <param name="drdevice">设备信息记录行</param>
        /// <param name="attrcmd">执行指令</param>
        /// <returns>返回通讯下载参数是否成功</returns>
        private bool commiDeviceDoor(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, DataRow drdevice, string attrcmd)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == drdevice || string.IsNullOrEmpty(attrcmd))
                return true;

            CmdProtocol cmdP = new CmdProtocol(false);
            string devID = Convert.ToString(drdevice["ID"]);
            //建立传输指令参数实例
            TransArg arg = new TransArg();
            arg.trans = trans;
            arg.target = tarsrv;
            arg.proxy = proxy;
            arg.dest = dest;
            arg.devID = devID;
            arg.addrst = Convert.ToString(drdevice["站址"]);
            arg.attrCmdtag = attrcmd;

            NameObjectList ps = new NameObjectList();
            ps["设备ID"] = devID;
            bool success = false;
            string tpl = "门禁";
            string valst = Convert.ToString(drdevice["站址"]);
            //系统时间
            DateTime dtsystem = Convert.ToDateTime(this.query.ExecuteScalar("系统时间", ps));
            string cmdstr = ",格式化,清空记录,清空白名单,清空时段,设置时间,设置时段,设置控制参数,";
            string[] cmds = attrcmd.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmdstr.IndexOf("," + cmds[i] + ",") < 0)
                    continue;
                string tagdata = "@设备地址=" + valst;
                if ("设置时间" == cmds[i])
                    tagdata = basefun.setvaltag(tagdata, "{日期时间}", dtsystem.ToString("yyyy-MM-dd HH:mm:ss"));
                if ("设置时段" == cmds[i])
                {
                    DataTable tabtime = this.query.getTable("门禁时段", ps);
                    if (null == tabtime || tabtime.Rows.Count < 1)
                        continue;
                    foreach (DataRow dr in tabtime.Rows)
                    {
                        tagdata = this.getdoortime(dr);
                        tagdata = basefun.setvaltag(tagdata, "设备地址", valst);
                        success = this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, cmds[i], tagdata);
                        if (!success) return false;
                    }
                    continue;
                }
                if ("设置控制参数" == cmds[i])
                {
                    success = this.downdoorctrlpm(arg);
                    if (!success) return false;
                    continue;
                }
                success = this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, cmds[i], tagdata);
                //格式化时设备有2.5s不应期
                if ("格式化" == cmds[i])
                {
                    this.query.ExecuteNonQuery("门禁记录位置重置", ps, ps, ps);
                    System.Threading.Thread.Sleep(3500);
                    continue;
                }
                if (!success) return false;
                if ("清空记录" == cmds[i])
                    this.query.ExecuteNonQuery("门禁记录位置重置", ps, ps, ps);
            }
            //更新下载日期
            this.query.ExecuteNonQuery("设备下载标志更新", ps, ps, ps);

            //重置黑白名单,或门禁名单超过500条，则重新清空再下载
            if (attrcmd.IndexOf("|格式化|") > -1 || attrcmd.IndexOf("|清空白名单|") > -1)
                this.query.ExecuteNonQuery("重置设备名单", ps, ps, ps);
            else if (attrcmd.IndexOf("|下载白名单|") > -1)
            {
                DataTable tab = this.query.getTable("门禁白名单", ps);
                if (null != tab && tab.Rows.Count > 500)
                {
                    attrcmd += "|清空白名单|";
                    this.query.ExecuteNonQuery("重置设备名单", ps, ps, ps);
                }
            }
            success = this.downdoorCardList(arg);
            return success;
        }

        /// <summary>
        /// 检查通讯目标是否可连接
        /// </summary>
        /// <param name="tarsrv">中转服务目标，在串口时可转发通讯</param>
        /// <param name="dest">通讯目标</param>
        /// <returns>返回是否可连接</returns>
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
        /// 门禁时段  tag格式参数
        /// </summary>
        /// <param name="drtime">时段记录</param>
        /// <returns>返回tag格式协议参数</returns>
        private string getdoortime(DataRow drtime)
        {
            if (null == drtime)
                return "";
            string[,] colmap ={ { "{时段号}", "时段号" }, { "{星期控制}.{一}", "星期一" }, { "{星期控制}.{二}", "星期二" },
                        { "{星期控制}.{三}", "星期三" }, { "{星期控制}.{四}", "星期四" }, { "{星期控制}.{五}", "星期五" }, 
                        { "{星期控制}.{六}", "星期六" }, { "{星期控制}.{日}", "星期日" },
                        {"{起始时间1}","开始时间1"},{"{终止时间1}","结束时间1"},{"{起始时间2}","开始时间2"},{"{终止时间2}","结束时间2"},
                        {"{起始时间3}","开始时间3"},{"{终止时间3}","结束时间3"}};
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
        /// 下载控制参数
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
            ps["设备ID"] = devID;
            DataTable tabpm = this.query.getTable("门禁控制参数", ps);
            if (null == tabpm || tabpm.Rows.Count < 1)
                return true;
            bool success = false;
            string tpl = "门禁";
            string tag = "@设备地址=" + addrst;
            //设置门参数
            string[,] colmapdoor ={ { "{控制方式}", "控制方式" }, { "{延时}", "开门延时" }, { "{门号}", "门号" } };
            foreach (DataRow dr in tabpm.Rows)
            {
                tag = "@设备地址=" + addrst;
                string mode = Convert.ToString(dr["控制方式"]);
                switch (mode)
                {
                    case "常开": mode = "1"; break;
                    case "常闭": mode = "1"; break;
                    default: mode = "3"; break;
                }
                tag = basefun.setvaltag(tag, "{控制方式}", mode);
                tag = basefun.setvaltag(tag, "{延时}", Convert.ToString(dr["开门延时"]));
                tag = basefun.setvaltag(tag, "{门号}", Convert.ToString(Convert.ToInt16(dr["门号"]) + 1));
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "设置控制参数", tag);
                if (!success) return false;
            }
            //设置报警参数
            string[,] colmapalarm ={ { "{报警状态}.{火警}", "火警告警" }, { "{报警状态}.{无效刷卡}", "非法刷卡告警" },
                        { "{报警状态}.{联动报警}", "联动告警" }, { "{报警状态}.{非法闯入}", "非法开门告警" },
                        { "{报警状态}.{超时}", "超时开门告警" }, { "{报警状态}.{胁迫}", "胁迫报警" },
                        {"{开门超时时间}","开门超时"}, {"{联动输出时间}","报警联动延时"} };
            DataRow drpm = tabpm.Rows[0];
            tag = "@设备地址=" + addrst;
            for (int c = 0; c < colmapalarm.GetLength(0); c++)
            {
                string val = Convert.ToString(drpm[colmapalarm[c, 1]]);
                if (true.Equals(drpm[colmapalarm[c, 1]]))
                    val = "1";
                else if (false.Equals(drpm[colmapalarm[c, 1]]))
                    val = "0";
                tag = basefun.setvaltag(tag, colmapalarm[c, 0], val);
            }
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "设置报警", tag);
            if (!success) return false;
            //设置事件日志；三个记录指令
            string[,] colmaplog ={ { "记录开门按钮", "2D" }, { "记录告警事件", "2E" }, { "记录门磁事件", "DC" } };
            for (int m = 0; m < colmaplog.GetLength(0); m++)
            {
                tag = "@设备地址=" + addrst;
                tag = basefun.setvaltag(tag, "{功能码}", colmaplog[m, 1]);
                tag = basefun.setvaltag(tag, "{开启}", "0");
                if (true.Equals(drpm[colmaplog[m, 0]]))
                    tag = basefun.setvaltag(tag, "{开启}", "1");
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "设置事件日志", tag);
                if (!success) return false;
            }
            //设置反潜；反潜指令需要有确认指令
            tag = "@设备地址=" + addrst;
            string unhide = Convert.ToString(drpm["反潜类别"]);
            string unhideOK = "00";
            switch (unhide)
            {
                case "禁用反潜": unhide = "0"; unhideOK = "0A"; break;
                case "单独反潜": unhide = "1"; unhideOK = "FA"; break;
                case "关联反潜": unhide = "2"; unhideOK = "FA"; break;
                case "一进两出": unhide = "3"; unhideOK = "7E"; break;
                case "一进多出": unhide = "4"; unhideOK = "FE"; break;
                default: unhide = "0"; unhideOK = "00"; break;
            }
            tag = basefun.setvaltag(tag, "{反潜号}", unhide);
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "设置反潜", tag);
            if (!success) return false;
            tag = basefun.setvaltag(tag, "{反潜号}", unhideOK);
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "设置反潜确定", tag);
            if (!success) return false;
            //设置互锁；互锁需发多个指令配合
            tag = "@设备地址=" + addrst;
            string[] cmds = new string[] { "001E", "001F", "0020", "0021" };
            string locklink = Convert.ToString(drpm["互锁类别"]);
            switch (locklink)
            {
                case "禁用互锁": locklink = "00"; break;
                case "关联互锁": locklink = "01"; break;
                case "多门互锁":
                    locklink = "71";
                    cmds = new string[] { "001E", "001F", "0020" };
                    break;
                case "全部互锁": locklink = "F1"; break;
                default: locklink = "00"; break;
            }
            tag = basefun.setvaltag(tag, "{开启}", locklink);
            for (int i = 0; i < cmds.Length; i++)
            {
                tag = basefun.setvaltag(tag, "{功能码}", cmds[i]);
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "设置互锁", tag);
                if (!success) return false;
            }
            //设置扩展板参数
            DataTable tabext = this.query.getTable("门禁扩展板参数", ps);
            if (null == tabext || tabext.Rows.Count < 1)
                return true;
            //四组扩展板指令；每组扩展板分别发四个指令
            string[,] codefun ={
                                 { "98", "9A", "9B", "9C", "9D" }, { "9E", "A0", "A1", "A2", "A3" },
                                 { "A4", "A6", "A7", "A8", "A9" }, { "AA", "AC", "AD", "AE", "AF" }  };
            colmapdoor = new string[,]{ { "{门位}.{门1}", "一号门" }, { "{门位}.{门2}", "二号门" }, 
                                 { "{门位}.{门3}", "三号门" }, { "{门位}.{门4}", "四号门" } };
            colmapalarm = new string[,]{ { "{报警状态}.{火警}", "火警告警" }, { "{报警状态}.{无效刷卡}", "非法刷卡告警" },
                        { "{报警状态}.{联动报警}", "联动告警" }, { "{报警状态}.{非法闯入}", "非法开门报警" },
                        { "{报警状态}.{超时}", "超时开门告警" }, { "{报警状态}.{胁迫}", "胁迫报警" } };
            string strtime = Convert.ToString(drpm["报警联动延时"]);
            if (string.IsNullOrEmpty(strtime))
                strtime = "0";
            long latetime = Convert.ToInt64(strtime);
            string latestr = Convert.ToString(latetime, 16).PadLeft(4, '0');
            latestr = latestr.Substring(latestr.Length - 4);
            foreach (DataRow drext in tabext.Rows)
            {
                string strbh = Convert.ToString(drext["编号"]);
                if (string.IsNullOrEmpty(strbh))
                    continue;
                int index = Convert.ToInt32(strbh);
                if (index < 0 || index > 3)
                    continue;
                //设置事件源
                tag = "@设备地址=" + addrst;
                tag = basefun.setvaltag(tag, "{功能码}", codefun[index, 0]);
                for (int i = 0; i < colmapdoor.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmapdoor[i, 0], true.Equals(drext[colmapdoor[i, 1]]) ? "1" : "0");
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "扩展板事件源", tag);
                if (!success) return false;
                //设置报警
                tag = "@设备地址=" + addrst;
                tag = basefun.setvaltag(tag, "{功能码}", codefun[index, 1]);
                for (int i = 0; i < colmapalarm.GetLength(0); i++)
                    tag = basefun.setvaltag(tag, colmapalarm[i, 0], true.Equals(drext[colmapalarm[i, 1]]) ? "1" : "0");
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "扩展板报警参数", tag);
                if (!success) return false;
                //设置延时低位
                tag = "@设备地址=" + addrst;
                tag = basefun.setvaltag(tag, "{功能码}", codefun[index, 3]);
                tag = basefun.setvaltag(tag, "{延时}", latestr.Substring(2));
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "扩展板延时低位", tag);
                if (!success) return false;
                //设置延时高位
                tag = "@设备地址=" + addrst;
                tag = basefun.setvaltag(tag, "{功能码}", codefun[index, 4]);
                tag = basefun.setvaltag(tag, "{延时}", latestr.Substring(0, 2));
                success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, "扩展板延时高位", tag);
                if (!success) return false;
            }
            return true;
        }

        /// <summary>
        /// 更新设备的黑白清名单,执行成功后更新黑白名单下载日期
        /// </summary>
        /// <param name="arg">传输指令参数</param>
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
            if (string.IsNullOrEmpty(attrcmd) || (attrcmd.IndexOf("|删除白名单|") < 0 && attrcmd.IndexOf("|下载白名单|") < 0))
                return true;
            CommiManager mgr = CommiManager.GlobalManager;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameValueCollection data = new NameValueCollection();
            NameObjectList ps = new NameObjectList();
            ps["设备ID"] = devID;
            //同步通讯等待时间15秒
            bool success = true, isconn = true;
            string cardsdel = "", cardswh = "";
            DataTable tab = this.query.getTable("门禁清名单", ps);
            string tpl = "门禁", cmd = "删除白名单";
            string tagdata = "@设备地址=" + addrst;
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|删除白名单|") > -1)
                for (int i = 0; isconn && i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    string cardnum = Convert.ToString(dr["卡号"]);
                    tagdata = basefun.setvaltag(tagdata, "{卡号}", cardnum);
                    //附加字段保存了对应门号(读卡器号)
                    string dstr = Convert.ToString(dr["撤权"]);
                    if (string.IsNullOrEmpty(dstr))
                        continue;
                    string[] bh = dstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    bool rtn = true;
                    for (int b = 0; b < bh.Length; b++)
                    {
                        //门号以1起始
                        bh[b] = Convert.ToString(Convert.ToInt32(bh[b]) + 1);
                        tagdata = basefun.setvaltag(tagdata, "{门号}", bh[b]);
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
                    //ps["状态"] = "清";
                    //ps["卡号"] = Convert.ToString(dr["卡号"]);
                    //this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                }
            tab = this.query.getTable("门禁白名单", ps);
            cmd = "下载白名单";
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|下载白名单|") > -1)
                for (int i = 0; isconn && i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    string cardnum = Convert.ToString(dr["卡号"]);
                    string[,] colmap ={ { "{卡号}", "卡号" }, { "{起始日期}", "发布日期" }, { "{终止日期}", "有效日期" }, { "{时段}", "时段" },
                                        { "{姓名}", "姓名" }, { "{编号}", "用户编号" } };
                    for (int j = 0; j < colmap.GetLength(0); j++)
                        tagdata = basefun.setvaltag(tagdata, colmap[j, 0], Convert.ToString(dr[colmap[j, 1]]));
                    tagdata = basefun.setvaltag(tagdata, "{密码}", "FF");
                    ps["状态"] = "白";
                    ps["卡号"] = cardnum;

                    cmd = "下载白名单";
                    bool rtn = true;
                    if ("考勤机" == Convert.ToString(dr["控制器类型"]))
                    {
                        cmd = "下载考勤名单";
                        rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                        success = success && rtn;
                        if (!rtn)
                        {
                            isconn = testConnect(target, dest);
                            continue;
                        }
                        cardswh += "," + cardnum;
                        continue;
                        //this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                        //continue;
                    }
                    //{ "{门号}", "读卡器号" },
                    string dstr = Convert.ToString(dr["授权"]);
                    if (string.IsNullOrEmpty(dstr))
                        continue;
                    string[] bh = dstr.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    for (int b = 0; b < bh.Length; b++)
                    {
                        //门号以1起始
                        bh[b] = Convert.ToString(Convert.ToInt32(bh[b]) + 1);
                        tagdata = basefun.setvaltag(tagdata, "{门号}", bh[b]);
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
                    //this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                }
            if (success)
            {
                ps["设备ID"] = devID;
                this.query.ExecuteNonQuery("设备黑白名单标志更新", ps, ps, ps);
            }
            else
            {
                //下载不成功时，分别更新黑白名单标志
                string[] cards = cardsdel.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string c in cards)
                {
                    ps["状态"] = "清";
                    ps["卡号"] = c;
                    this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                }
                cards = cardswh.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (string c in cards)
                {
                    ps["状态"] = "白";
                    ps["卡号"] = c;
                    this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                }
            }
            return success;
        }

        #endregion

        #region 消费通讯

        /// <summary>
        /// 消费设备初始化和下载参数,更新黑白名单
        /// </summary>
        /// <param name="tarsrv">中转服务器</param>
        /// <param name="trans">传输指令</param>
        /// <param name="proxy">设备通讯代理电脑IP地址</param>
        /// <param name="dest">目标设备</param>
        /// <param name="drdevice">设备信息记录行</param>
        /// <param name="attrcmd">执行指令</param>
        /// <returns>返回通讯下载参数是否成功</returns>
        private bool commiDeviceEatery(CommiTarget tarsrv, CmdFileTrans trans, IPAddress proxy, CommiTarget dest, DataRow drdevice, string attrcmd)
        {
            if (null == tarsrv || null == proxy || null == dest || null == trans || null == drdevice || string.IsNullOrEmpty(attrcmd))
                return true;
            CmdProtocol cmdP = new CmdProtocol(false);
            string devID = Convert.ToString(drdevice["ID"]);
            //建立传输指令参数实例
            TransArg arg = new TransArg();
            arg.trans = trans;
            arg.target = tarsrv;
            arg.proxy = proxy;
            arg.dest = dest;
            arg.devID = devID;
            arg.addrst = Convert.ToString(drdevice["站址"]);
            arg.attrCmdtag = attrcmd;

            NameObjectList ps = new NameObjectList();
            ps["设备ID"] = devID;
            bool success = false;
            string tpl = "消费";
            string valst = Convert.ToString(drdevice["站址"]);
            //系统时间
            DateTime dtsystem = Convert.ToDateTime(this.query.ExecuteScalar("系统时间", ps));
            string cmdstr = ",格式化,清空记录,清空补助名单,清空黑名单,清空时段,设置时间,设置时段,设置控制参数,";
            string[] cmds = attrcmd.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < cmds.Length; i++)
            {
                if (cmdstr.IndexOf("," + cmds[i] + ",") < 0)
                    continue;
                string tagdata = "@设备地址=" + valst;
                if ("设置时间" == cmds[i])
                    tagdata = basefun.setvaltag(tagdata, "{日期时间}", dtsystem.ToString("yyyy-MM-dd HH:mm:ss"));
                if ("设置时段" == cmds[i])
                {
                    success = this.downeaterytime(arg);
                    if (!success) return false;
                    continue;
                }
                if ("设置控制参数" == cmds[i])
                {
                    success = this.downeateryctrlpm(arg);
                    if (!success) return false;
                    continue;
                }
                success = this.sendCommand(tarsrv, trans, proxy, dest, cmdP, tpl, cmds[i], tagdata);
                //格式化时设备有2.5s不应期
                if ("格式化" == cmds[i])
                {
                    System.Threading.Thread.Sleep(3500);
                    continue;
                }
                if (!success) return false;
            }
            //更新下载日期
            this.query.ExecuteNonQuery("设备下载标志更新", ps, ps, ps);

            //重置黑白名单
            if (attrcmd.IndexOf("|格式化|") > -1 || attrcmd.IndexOf("|清空白名单|") > -1)
                this.query.ExecuteNonQuery("重置设备名单", ps, ps, ps);
            success = this.downeateryCardList(arg);
            if (!success) return false;
            //回收消费卡没有接收到的补助，并下载未下载的补助
            success = this.downeaterySubsidy(arg);
            return success;
        }

        /// <summary>
        /// 下载时段
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
            ps["设备ID"] = devID;
            //同步通讯等待时间15秒
            bool success = false;

            DataTable tab = this.query.getTable("消费时间区间", ps);
            string tpl = "消费", cmd = "下载时间区间";
            string[,] colmap ={ { "{时间区间号}", "序号" }, { "{起始时间}", "开始" }, { "{结束时间}", "结束" },
                                { "{消费限额}", "时段限额" }, { "{消费限次}", "时段限次" } };
            string tagdata = "@设备地址=" + addrst;
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
            cmd = "设置时段";
            colmap = new string[,]{ { "{星期}", "星期序号" }, 
                                { "{时段1}", "晨餐" }, { "{时段2}", "早餐" }, { "{时段3}", "午餐" }, { "{时段4}", "下午茶" }, 
                                { "{时段5}", "晚餐" }, { "{时段6}", "夜宵" },{ "{时段7}", "加餐1" }, { "{时段8}", "加餐2" },
                                { "{消费限额}", "每天限额" }, { "{次数限制}", "每日限次" } };
            tagdata = "@设备地址=" + addrst;
            tab = this.query.getTable("消费周时段", ps);
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
        /// 下载控制参数
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
            ps["设备ID"] = devID;
            bool success = false;
            //清空菜单
            string tpl = "消费", cmd = "清空消费参数";
            string tagdata = "@设备地址=" + addrst;
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
            if (!success) return false;
            //下载控制参数
            DataTable tab = this.query.getTable("消费控制参数", ps);
            if (null == tab || tab.Rows.Count < 1)
                return false;
            DataRow drctrl = tab.Rows[0];
            string[,] colmap ={ { "{通信密码}", "通讯密码" }, { "{系统密码}", "系统密码" }, { "{用户密码}", "用户密码" }, { "{卡片区域}", "卡片区号" },
                    { "{记录门限}", "空间告警" }, { "{黑名单门限}", "黑名单门限" }, { "{取消交易门限}", "取消交易门限" },
                    { "{单卡单次限额}", "单次限额" }, { "{卡最大余额}", "卡限额" }, { "{操作延时}", "操作延时" }, { "{显示延时}", "显示延时" }, 
                    { "{操作控制}.{记录溢出允许消费}", "允许溢出" }, { "{操作控制}.{时段限制}", "时段限制" }, { "{操作控制}.{操作员凭密取消上笔交易}", "凭密取消交易" },
                    { "{操作控制}.{操作员直接取消上笔交易}", "直接取消交易" }, { "{操作控制}.{管理卡可设置参数}", "可设置参数" }, 
                    { "{操作控制}.{授权卡可初始化设备}", "可初始化" }, 
                    { "{消费控制}.{定额直接扣费}", "定额直接扣费" }, { "{消费控制}.{副卡独立}", "副卡独立" }, { "{消费控制}.{打印记录}", "打印记录" }, 
                    { "{消费控制}.{可现金消费}", "可现金消费" }, { "{消费控制}.{可赊账消费}", "可赊账消费" }, { "{消费控制}.{可超额}", "可超额" }, 
                    { "{消费控制}.{凭密消费}", "凭密消费" }, 
                    { "{赊账限额}", "赊账限额" }, { "{开机方式}", "开机方式" }, { "{食堂编号}", "编号" } };
            for (int i = 0; i < colmap.GetLength(0); i++)
                tagdata = basefun.setvaltag(tagdata, colmap[i, 0], Convert.ToString(drctrl[colmap[i, 1]]));
            cmd = "设置控制参数";
            success = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
            if (!success) return false;
            //下载菜单
            tagdata = "@设备地址=" + addrst;
            tab = this.query.getTable("消费价格参数", ps);
            colmap = new string[,] { { "{编码}", "编号" }, { "{价格}", "价格" } };
            cmd = "下载消费参数";
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
        /// 更新设备的黑白清名单,执行成功后更新黑白名单下载日期
        /// </summary>
        /// <param name="arg">传输指令参数</param>
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
            ps["设备ID"] = devID;
            //同步通讯等待时间15秒
            bool success = true;
            DataTable tab = this.query.getTable("消费清名单", ps);
            string tpl = "消费", cmd = "删除黑名单";
            string tagdata = "@设备地址=" + addrst;
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|删除黑名单|") > -1)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    tagdata = basefun.setvaltag(tagdata, "{卡号}", Convert.ToString(dr["卡号"]));
                    bool rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                    success = success && rtn;
                    if (!rtn)
                    {
                        if (testConnect(target, dest))
                            return false;
                        continue;
                    }
                    ps["状态"] = "清";
                    ps["卡号"] = Convert.ToString(dr["卡号"]);
                    this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                }
            tab = this.query.getTable("消费黑名单", ps);
            cmd = "下载黑名单";
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|下载黑名单|") > -1)
                for (int i = 0; i < tab.Rows.Count; i++)
                {
                    DataRow dr = tab.Rows[i];
                    tagdata = basefun.setvaltag(tagdata, "{卡号}", Convert.ToString(dr["卡号"]));
                    bool rtn = this.sendCommand(target, trans, proxy, dest, cmdP, tpl, cmd, tagdata);
                    success = success && rtn;
                    if (!rtn)
                    {
                        if (testConnect(target, dest))
                            return false;
                        continue;
                    }
                    ps["状态"] = "黑";
                    ps["卡号"] = Convert.ToString(dr["卡号"]);
                    this.query.ExecuteNonQuery("名单下载标志更新", ps, ps, ps);
                }
            return success;
        }
        /// <summary>
        /// 回收补助机没有下载的补助,清空补助后,合并到未下载补助一起下载
        /// </summary>
        /// <param name="arg">传输指令参数</param>
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
            //判断是否下载补助
            if (string.IsNullOrEmpty(attrcmd) || attrcmd.IndexOf("|下载补助名单|") < 0)
                return true;
            CommiManager mgr = CommiManager.GlobalManager;
            CmdProtocol cmdP = new CmdProtocol(false);
            NameObjectList ps = new NameObjectList();
            ps["设备ID"] = devID;
            string tpl = "消费", cmd = "取当前补助记录";
            string tag = "@设备地址=" + addrst;
            //首先提取补助记录
            string msg = this.getResponse(target, trans, proxy, dest, cmdP, tpl, cmd, tag);
            if (string.IsNullOrEmpty(msg))
                return false;
            string cardnum = basefun.valtag(msg, "{卡号}");
            cmd = "取下一条补助记录";
            while (!string.IsNullOrEmpty(cardnum) && "16777215" != cardnum && "0" != cardnum)
            {
                NameObjectList pm = createParam(msg);
                bool rtn = this.query.ExecuteNonQuery("消费接收补助", pm, pm, pm);
                if (!rtn) return false;
                msg = this.getResponse(target, trans, proxy, dest, cmdP, tpl, cmd, tag);
                cardnum = basefun.valtag(msg, "{卡号}");
            }
            //读取没有接收的补助记录，回收合并到补助记录
            DataTable tab = this.query.getTable("消费回收补助", ps);
            if (null == tab) return false;
            cmd = "查询用户补助金额";
            string[,] cols = { { "卡号", "{用户编号}" }, { "卡片序列号", "{卡片物理序号}" } };
            string[,] pn = { { "卡号", "{用户编号}" }, { "卡片序列号", "{卡片物理序号}" }, { "补助金额", "{补助金额}" }, { "补助状态", "{补助状态}" } };
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
                this.query.ExecuteNonQuery("消费回收补助", pm, pm, pm);
            }
            if (!success) return false;
            tab = this.query.getTable("消费下载补助", ps);
            cmd = "下载补助名单";
            tag = "@设备地址=" + addrst;
            cols = new string[,] { { "卡号", "{用户编号}" }, { "卡片序列号", "{卡片物理序号}" }, { "补助充值", "{补助金额}" } };
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
                ps["卡号"] = Convert.ToString(dr["卡号"]);
                this.query.ExecuteNonQuery("消费下载补助", ps, ps, ps);
            }
            return success;
        }
        #endregion

        /// <summary>
        /// 获取数据行的设备目标位置参数
        /// 记录包含字段【访问方式】(TCP/UDP/SerialPort)、【端口】(60000/COM1)、【地址】(192.168.1.146)
        /// </summary>
        /// <param name="dr">数据记录</param>
        /// <returns></returns>
        private CommiTarget getTarget(DataRow dr)
        {
            if (null == dr || (DBNull.Value == dr["串口"] && DBNull.Value == dr["IP地址"]))
                return null;
            CommiTarget target = new CommiTarget();
            CommiType commiType = CommiType.UDP;
            string stype = Convert.ToString(dr["通讯类别"]);
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
                    string portname = Convert.ToString(dr["串口"]);
                    int baudRate = Convert.ToInt16(dr["波特率"]);
                    int dataBits = Convert.ToInt16(dr["数据位"]);
                    decimal s = Convert.ToDecimal(dr["停止位"]);
                    StopBits sb = StopBits.None;
                    if (1 == s) sb = StopBits.One;
                    else if (2 == s) sb = StopBits.Two;
                    else if (1 < s && s < 2) sb = StopBits.OnePointFive;

                    target.SetProtocolParam(portname, baudRate, Parity.None, dataBits, sb);
                }
                else
                {
                    string addr = Convert.ToString(dr["IP地址"]);
                    if (string.IsNullOrEmpty(addr) || DBNull.Value == dr["端口"])
                        return null;
                    int port = Convert.ToInt32(dr["端口"]);
                    target.SetProtocolParam(addr, port, commiType);
                }
            }
            catch (Exception ex)
            {
                NameValueCollection data = new NameValueCollection();
                data["操作"] = "创建通讯目标";
                data["设备ID"] = Convert.ToString(dr["ID"]);
                ServiceTool.LogMessage(ex, data, EventLogEntryType.Error);
                return null;
            }
            return target;
        }

        private static Regex regex = new Regex(@"@([\u4E00-\u9FA5\s\w\{\}]+)=", RegexOptions.Compiled);
        /// <summary>
        /// 根据tag标记创建参数
        /// </summary>
        /// <param name="tag">包含参数的tag标记</param>
        /// <returns>返回新建立的参数</returns>
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
