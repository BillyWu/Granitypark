using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Granity.commiServer
{
    public class ServiceTool
    {
        /// <summary>
        /// 服务使用日志名称
        /// </summary>
        private const string logname = "GranityLog";
        /// <summary>
        /// 服务使用日志名称
        /// </summary>
        public static string LogName
        {
            get { return logname; }
        }
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

            string logname = ServiceTool.logname;

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

            string logname = ServiceTool.logname;

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
