using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using Granity.communications;
using System.Collections.Specialized;
using System.Configuration;
using Granity.commiServer;
using System.Timers;
using Estar.Common.Tools;
using Estar.Business.DataManager;

namespace winFileSrv
{
    partial class ServiceFile : ServiceBase
    {
        /// <summary>
        /// 内置定时器，控制内部定时执行。默认两分钟。
        /// </summary>
        private Timer timer = new Timer(2 * 60 * 1000);
        /// <summary>
        /// 定时器事件是否正在执行
        /// </summary>
        private bool isTmRunning = false;

        private int port = 2010;
        /// <summary>
        /// 服务端口号,默认2010
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        private SvrFileTrans srvTrans;
        /// <summary>
        /// 传输服务
        /// </summary>
        public SvrFileTrans SrvTrans
        {
            get { return srvTrans; }
        }
        private SynDeviceParam synDevice = new SynDeviceParam();
        /// <summary>
        /// 更新设备控制参数服务,定时器控制执行后台服务
        /// </summary>
        public SynDeviceParam SynDevice
        {
            get { return synDevice; }
        }
        private DeviceMonitorMgr moniDevice = new DeviceMonitorMgr();
        /// <summary>
        /// 设备巡检监控管理器
        /// </summary>
        public DeviceMonitorMgr MoniDevice
        {
            get { return moniDevice; }
        }

        public ServiceFile()
        {
            InitializeComponent();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
        }

        /// <summary>
        /// 定时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            DateTime dt = DateTime.Now;
            DateTime dtR = DateTime.Today.AddHours(4);
            if (isTmRunning) return;
            isTmRunning = true;
            if (dt < dtR || dt > dtR.AddMinutes(2.1))
            {
                isTmRunning = false;
                return;
            }
            try
            {
                ThreadManager.QueueUserWorkItem(new System.Threading.WaitCallback(executeTimer), null);
            }
            catch (Exception ex)
            {
                isTmRunning = false;
                NameValueCollection data = new NameValueCollection();
                data["服务"] = "定时服务";
                LogMessage(ex, data, EventLogEntryType.Error);
            }
        }
        /// <summary>
        /// 服务启动
        /// </summary>
        /// <param name="args"></param>
        protected override void OnStart(string[] args)
        {
            string port = ConfigurationManager.AppSettings[this.ServiceName];
            if(!string.IsNullOrEmpty(port))
                try { this.port = Convert.ToInt32(port); }
                catch { }
            SvrFileTrans svrfile = new SvrFileTrans();
            svrfile.ExtendHandle += new EventHandler<ExtendEventArgs>(svrfile_ExtendHandle);
            CommiServer commisrv = CommiServer.GlobalServer;
            commisrv.ErrorHandle += new EventHandler<ErrorRequestEventArgs>(commisrv_ErrorHandle);

            commisrv.Start(this.port, svrfile);
            this.srvTrans = svrfile;
            NameValueCollection data = new NameValueCollection();
            data["服务"] = this.ServiceName;
            data["端口"] = this.port.ToString();
            LogMessage("启动Granity文件服务", data, EventLogEntryType.Information);
        }

        /// <summary>
        /// 传输服务接收扩展服务事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void svrfile_ExtendHandle(object sender, ExtendEventArgs e)
        {
            if (null == sender || null == e || null == e.Request || e.Request.Length < 1)
                return;
            byte[] header = SvrFileTrans.GetFileheader(e.Request);
            NameValueCollection info = SvrFileTrans.ParseInfo(header);
            string cmdext = info["service"];
            if (string.IsNullOrEmpty(cmdext))
                return;
            string op = "";
            try
            {
                switch (cmdext)
                {
                    case "updateparam":
                        op = "更新设备控制参数";
                        this.synDevice.CommiDevice(); 
                        break;
                    case "monitor":
                        // 指令内容：service='monitor',device,id,patrol='true'
                        if (null == e.Client) break;
                        op = "巡检设备";
                        MoniDevice.Monitordev(info, e.Client);
                        break;
                    case "halt":
                        // 指令内容：service='halt',device,id,all='true'
                        if (null == e.Client) break;
                        op = "停止巡检";
                        MoniDevice.Haltdev(info, e.Client);
                        break;
                    case "readinfo":
                        // 指令内容：service='readinfo',device,id
                        if(null==e.Client) break;
                        op = "读取设备信息";
                        moniDevice.ReadInfodev(info, e.Client);
                        break;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                NameValueCollection data = new NameValueCollection();
                data["操作"] = op;
                LogMessage(ex, data, EventLogEntryType.Error);
            }
        }
        /// <summary>
        /// 服务停止
        /// </summary>
        protected override void OnStop()
        {
            CommiServer.GlobalServer.Stop(this.port);
            NameValueCollection data = new NameValueCollection();
            data["服务"] = this.ServiceName;
            data["端口"] = this.port.ToString();
            LogMessage("停止Granity文件服务", data, EventLogEntryType.Information);
        }

        /// <summary>
        /// 定时执行业务
        /// </summary>
        /// <param name="obj"></param>
        void executeTimer(object obj)
        {
            DateTime dt = DateTime.Today.AddDays(-1);
            NameObjectList ps = new NameObjectList();
            ps.Add("开始日期", dt);
            ps.Add("结束日期", dt);
            QueryDataRes query = new QueryDataRes("基础类");
            DataTable tab = query.getTable("考勤人员列表", null);
            if (null == tab || tab.Rows.Count < 1)
            {
                isTmRunning = false;
                return;
            }
            NameValueCollection data = new NameValueCollection();
            data["服务"] = "考勤作业";
            DataColumnCollection cols = tab.Columns;
            foreach (DataRow dr in tab.Rows)
            {
                foreach (DataColumn c in cols)
                    ps[c.ColumnName] = Convert.ToString(dr[c]);
                try
                {
                    bool b = query.ExecuteNonQuery("考勤人员列表", ps, ps, ps);
                    if (b) continue;
                }
                catch (Exception ex)
                {
                    foreach (string key in ps.AllKeys)
                        data.Add(key, Convert.ToString(ps[key]));
                    LogMessage(ex, data, EventLogEntryType.Error);
                    continue;
                }
                foreach (string key in ps.AllKeys)
                    data.Add(key, Convert.ToString(ps[key]));
                LogMessage("考勤作业失败！", data, EventLogEntryType.Warning);
            }
            isTmRunning = false;
        }

        #region 响应事件

        /// <summary>
        /// 服务端解析异常事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void commisrv_ErrorHandle(object sender, ErrorRequestEventArgs e)
        {
            if (null == e)
                return;
            NameValueCollection data = new NameValueCollection();
            try
            {
                if (null != e.Client && null != e.Client.RemoteEndPoint)
                    data["客户端"] = e.Client.RemoteEndPoint.ToString();
            }
            catch { }
            data["类型"] = "服务端异常";
            if (null != e.Exception)
                LogMessage(e.Exception, data, EventLogEntryType.Error);
        }

        #endregion

        /// <summary>
        /// 日志记录消息
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="data">附加数据</param>
        /// <param name="logtype">日志类型</param>
        public static void LogMessage(string msg, NameValueCollection data, EventLogEntryType logtype)
        {
            if (string.IsNullOrEmpty(msg))
                return;

            string logname = "GranityLog";

            string head = "";
            EventLog log = new EventLog();
            if (null != data)
                foreach (string key in data)
                    head += key + "：" + data[key] + "\r\n";
            msg = head + "\n" + msg;

            if (msg.Length > 30000)
                msg = msg.Substring(0, 30000);
            log.Log = logname;
            log.Source = logname;
            log.MachineName = Environment.MachineName;
            log.WriteEntry(msg, logtype);
        }
        /// <summary>
        /// 日志记录消息
        /// </summary>
        /// <param name="msg">日志信息</param>
        /// <param name="data">附加数据</param>
        /// <param name="logtype">日志类型</param>
        public static void LogMessage(Exception ex, NameValueCollection data, EventLogEntryType logtype)
        {
            string msg = ex.Message;
            if (string.IsNullOrEmpty(msg))
                return;

            string logname = "GranityLog";

            string head = "";
            EventLog log = new EventLog();
            if (null != data)
                foreach (string key in data)
                    head += key + "：" + data[key] + "\r\n";
            msg = head + "\r\n" + msg;
            msg = msg + "\r\n" + ex.Source + "\r\n" + ex.StackTrace;

            if (msg.Length > 30000)
                msg = msg.Substring(0, 30000);
            log.Log = logname;
            log.Source = logname;
            log.MachineName = Environment.MachineName;
            log.WriteEntry(msg, logtype);
        }

    }
}
