using System;
using System.Collections.Generic;
using System.Text;
using Granity.communications;
using Estar.Business.DataManager;
using System.Data;
using Estar.Common.Tools;
using ComLib;
using System.Diagnostics;
using System.IO.Ports;

namespace Granity.CardOneCommi
{
    /// <summary>
    /// 协议命令,约束(a:一个通讯位置只有一套协议在执行;b:一个位置可管理多设备;c:当前时刻只处理一个指令)
    /// </summary>
    public class CmdProtocol : CommandBase
    {
        /// <summary>
        /// 协议数据源
        /// </summary>
        private const string dbSrc = "基础类";
        /// <summary>
        /// 协议参数数据项
        /// </summary>
        private const string dbItem = "协议指令";

        #region 内置类

        /// <summary>
        /// 协议参数
        /// </summary>
        private class PtlParam
        {
            /// <summary>
            /// 协议参数
            /// </summary>
            public string pmdevice;
            /// <summary>
            /// 输入参数
            /// </summary>
            public string pminput;
            /// <summary>
            /// 输出参数
            /// </summary>
            public string pmout;
        }

        #endregion

        private QueryDataRes query = null;
        private static ComClass comparse = new ComClass();

        /// <summary>
        /// 协议参数列表,缓存协议
        /// </summary>
        private static Dictionary<string, PtlParam> PTLParam = new Dictionary<string, PtlParam>();
        /// <summary>
        /// 设备通讯通用信息
        /// </summary>
        private devObj dvParam = new devObj();
        /// <summary>
        /// 请求格式
        /// </summary>
        private string inputParam = "";
        /// <summary>
        /// 响应格式
        /// </summary>
        private string outterParam = "";

        private string responseFormat = "";
        /// <summary>
        /// 读取当前响应格式化后字符串(tag格式化)
        /// </summary>
        public string ResponseFormat
        {
            get { return responseFormat; }
        }

        #region 构造函数

        /// <summary>
        /// 默认方式构造实例,不初始化同步事件句柄,无指令ID
        /// </summary>
        public CmdProtocol()
            : base()
        {
            try { this.query = new QueryDataRes(dbSrc); }
            catch (Exception ex) { }
            this.TimeOut = new TimeSpan(200 * 10000);
            this.TimeFailLimit = new TimeSpan(600 * 10000);
            this.TimeLimit = new TimeSpan(600 * 10000);
            this.IsResposeHandle = this.isResponse;
        }

        /// <summary>
        /// 构造函数指定构造指令ID(指令ID是可更改的)
        /// </summary>
        /// <param name="id">指令ID</param>
        public CmdProtocol(string id)
            : base(id)
        {
            try { this.query = new QueryDataRes(dbSrc); }
            catch (Exception ex) { }
            this.TimeOut = new TimeSpan(200 * 10000);
            this.TimeFailLimit = new TimeSpan(600 * 10000);
            this.TimeLimit = new TimeSpan(600 * 10000);
            this.IsResposeHandle = this.isResponse;
        }

        /// <summary>
        /// 构造函数,初始化同步事件句柄状态
        /// </summary>
        /// <param name="ewhState">同步事件初始状态</param>
        public CmdProtocol(bool ewhState)
            : base(ewhState)
        {
            try { this.query = new QueryDataRes(dbSrc); }
            catch (Exception ex) { }
            this.TimeOut = new TimeSpan(200 * 10000);
            this.TimeFailLimit = new TimeSpan(600 * 10000);
            this.TimeLimit = new TimeSpan(600 * 10000);
            this.IsResposeHandle = this.isResponse;
        }

        /// <summary>
        /// 构造函数,初始化指令ID,初始化同步事件句柄状态
        /// </summary>
        /// <param name="id">指令ID,使用中可更改</param>
        /// <param name="ewhState">同步事件初始状态</param>
        public CmdProtocol(string id, bool ewhState)
            : base(id, ewhState)
        {
            try { this.query = new QueryDataRes(dbSrc); }
            catch (Exception ex) { }
            this.TimeOut = new TimeSpan(200 * 10000);
            this.TimeFailLimit = new TimeSpan(600 * 10000);
            this.TimeLimit = new TimeSpan(600 * 10000);
            this.IsResposeHandle = this.isResponse;
        }

        #endregion

        /// <summary>
        /// 设置通讯协议定义
        /// </summary>
        /// <param name="tpl">协议名称</param>
        /// <param name="cmd">指令名称</param>
        /// <param name="tagdevice">协议设备定义</param>
        /// <param name="taginput">协议输入参数定义</param>
        /// <param name="tagoutput">协议输出参数定义</param>
        /// <returns>是否设置成功</returns>
        public bool setTpl(string tpl, string cmd, string tagdevice, string taginput, string tagoutput)
        {
            if (string.IsNullOrEmpty(tpl) || string.IsNullOrEmpty(cmd) || string.IsNullOrEmpty(tagdevice)
                || string.IsNullOrEmpty(taginput) || string.IsNullOrEmpty(tagoutput))
                return false;
            PtlParam ptlParam = new PtlParam();
            ptlParam.pmdevice = tagdevice;
            ptlParam.pminput = taginput;
            ptlParam.pmout = tagoutput;
            if (ptlParam.pmdevice.StartsWith(";"))
                ptlParam.pmdevice = ptlParam.pmdevice.Substring(1);
            if (ptlParam.pminput.StartsWith(";"))
                ptlParam.pminput = ptlParam.pminput.Substring(1);
            if (ptlParam.pmout.StartsWith(";"))
                ptlParam.pmout = ptlParam.pmout.Substring(1);
            if (ptlParam.pmdevice.EndsWith(";"))
                ptlParam.pmdevice = ptlParam.pmdevice.Substring(0, ptlParam.pmdevice.Length - 1);
            if (ptlParam.pminput.EndsWith(";"))
                ptlParam.pminput = ptlParam.pminput.Substring(0, ptlParam.pminput.Length - 1);
            if (ptlParam.pmout.EndsWith(";"))
                ptlParam.pmout = ptlParam.pmout.Substring(0, ptlParam.pmout.Length - 1);
            ptlParam.pmdevice = ptlParam.pmdevice.Replace(";;", ";");
            ptlParam.pminput = ptlParam.pminput.Replace(";;", ";");
            ptlParam.pmout = ptlParam.pmout.Replace(";;", ";");
            if (PTLParam.ContainsKey(tpl + ":" + cmd))
                PTLParam[tpl + ":" + cmd] = ptlParam;
            else
                PTLParam.Add(tpl + ":" + cmd, ptlParam);
            return true;
        }

        /// <summary>
        /// 设置设备指令,使用dvID做指令ID
        /// </summary>
        /// <param name="tpl">协议类型</param>
        /// <param name="cmd">指令</param>
        /// <param name="tagdata">数据参数,使用tag标记格式</param>
        public bool setCommand(string tpl, string cmd, string tagdata)
        {
            if (string.IsNullOrEmpty(tpl) || string.IsNullOrEmpty(cmd) || CmdState.Response == this.CheckState())
                return false;

            PtlParam ptlParam = new PtlParam();
            if (PTLParam.ContainsKey(tpl + ":" + cmd))
                ptlParam = PTLParam[tpl + ":" + cmd];
            else
            {
                Estar.Common.Tools.NameObjectList ps = new Estar.Common.Tools.NameObjectList();
                ps["tpl"] = tpl;
                ps["cmd"] = cmd;
                if (null == this.query)
                    return false;
                DataTable tab = this.query.getTable(dbItem, ps);
                if (null == tab || tab.Rows.Count < 1)
                    return false;
                //从数据库中取出的参数转换成tag格式参数
                DataColumnCollection dbcols = tab.Columns;
                if (!dbcols.Contains("type") || !dbcols.Contains("pms"))
                    return false;
                foreach (DataRow dr in tab.Rows)
                {
                    if (DBNull.Value == dr["type"])
                        continue;
                    string pmtype = Convert.ToString(dr["type"]);
                    switch (pmtype)
                    {
                        case "协议":
                            ptlParam.pmdevice = Convert.ToString(dr["pms"]);
                            break;
                        case "Input":
                            ptlParam.pminput += Convert.ToString(dr["pms"]) + ";";
                            break;
                        case "Output":
                            ptlParam.pmout += Convert.ToString(dr["pms"]) + ";";
                            break;
                    }
                }
                if (ptlParam.pmdevice.EndsWith(";"))
                    ptlParam.pmdevice = ptlParam.pmdevice.Substring(0, ptlParam.pmdevice.Length - 1);
                if (string.IsNullOrEmpty(ptlParam.pmdevice)) return false;
                if (ptlParam.pminput.EndsWith(";"))
                    ptlParam.pminput = ptlParam.pminput.Substring(0, ptlParam.pminput.Length - 1);
                if (ptlParam.pmout.EndsWith(";"))
                    ptlParam.pmout = ptlParam.pmout.Substring(0, ptlParam.pmout.Length - 1);
                ptlParam.pmdevice = ptlParam.pmdevice.Replace(";;", ";");
                ptlParam.pminput = ptlParam.pminput.Replace(";;", ";");
                ptlParam.pmout = ptlParam.pmout.Replace(";;", ";");
                if (!PTLParam.ContainsKey(tpl + ":" + cmd))
                    PTLParam.Add(tpl + ":" + cmd, ptlParam);
            }
            this.inputParam = ptlParam.pminput;
            this.outterParam = ptlParam.pmout;
            this.responseFormat = "";
            //根据协议组成通讯指令
            string pmdevice = basefun.setvaltag(ptlParam.pmdevice, "devid", basefun.valtag(tagdata, "设备地址"));
            dvParam.Command = "";
            string cmdstr = "";
            if (string.IsNullOrEmpty(tagdata))
                dvParam = this.setDevObj(pmdevice);
            else
                cmdstr = comparse.CommandString(pmdevice, ptlParam.pminput, tagdata, ref dvParam);
            IsAscii = dvParam.IsAsc;
            Exchanges = dvParam.Exchanges;
            if (string.IsNullOrEmpty(dvParam.Buss))
                dvParam.Buss = tpl;
            if (string.IsNullOrEmpty(dvParam.Command))
                dvParam.Command = cmd;
            //没有数据参数,则只获取格式参数,便于解析结果
            if (string.IsNullOrEmpty(tagdata))
                return true;
            //转义成字节,使用""""代替原来帧头帧尾的处理
            if (dvParam.IsAsc)
            {
                if (cmdstr.StartsWith("02") && cmdstr.EndsWith("03"))
                    cmdstr = "" + cmdstr.Substring(2, cmdstr.Length - 4) + "";
                return this.setCommand(cmdstr);
            }
            return this.setCommand(cmdstr, true);
        }
        /// <summary>
        /// 获取协议通用信息
        /// </summary>
        /// <param name="pmdevice">协议配置tag值</param>
        /// <returns>返回设备通讯通用信息实例</returns>
        private devObj setDevObj(string pmdevice)
        {
            devObj devobj = new devObj();
            devobj.ID = basefun.valtag(pmdevice, "devid");
            devobj.ControlNo = basefun.toIntval(basefun.valtag(pmdevice, "controlno"));
            devobj.Buss = basefun.valtag(pmdevice, "buss");
            devobj.IP = basefun.valtag(pmdevice, "ip");
            devobj.Mode = basefun.toIntval(basefun.valtag(pmdevice, "mode")) - 1;
            devobj.Port = basefun.valtag(pmdevice, "port");
            if (devobj.Mode == 0 && !devobj.Port.ToUpper().StartsWith("COM"))
                devobj.Port = "COM" + devobj.Port;
            devobj.Paritymode = basefun.valtag(pmdevice, "parity");
            devobj.LH = basefun.valtag(pmdevice, "LH") == "1" ? true : false;
            devobj.Exchanges = basefun.valtag(pmdevice, "exchange");
            devobj.Command = basefun.valtag(pmdevice, "cmd");
            devobj.WeekRule = basefun.valtag(pmdevice, "weekrule");
            devobj.IsAsc = basefun.valtag(pmdevice, "isasc") == "1" ? true : false;
            devobj.PreCommand = basefun.valtag(pmdevice, "precmd");
            devobj.Return = basefun.valtag(pmdevice, "return");
            devobj.parity = Parity.None;
            switch (basefun.valtag(pmdevice, "paritycom").ToLower())
            {
                case "none": devobj.parity = Parity.None; break;
                case "even": devobj.parity = Parity.Even; break;
                case "mark": devobj.parity = Parity.Mark; break;
                case "odd": devobj.parity = Parity.Odd; break;
                case "space": devobj.parity = Parity.Space; break;
            }
            devobj.baudRate = basefun.toIntval((basefun.valtag(pmdevice, "baudrate")));
            int dbit = basefun.toIntval(basefun.valtag(pmdevice, "databits"));
            devobj.DataBits = (dbit == 0) ? 8 : dbit;
            devobj.stopBits = StopBits.One;
            switch (basefun.valtag(pmdevice, "stopbits").ToLower())
            {
                case "none": devobj.stopBits = StopBits.None; break;
                case "one": devobj.stopBits = StopBits.One; break;
                case "onepoint": devobj.stopBits = StopBits.OnePointFive; break;
                case "two": devobj.stopBits = StopBits.Two; break;
            }
            //串口参数
            return devobj;
        }


        #region 监听响应触发指令响应事件

        /// <summary>
        /// 临时,是否ASCII
        /// </summary>
        private static bool IsAscii = false;
        /// <summary>
        /// 临时,转义字符集
        /// </summary>
        private static string Exchanges;

        /// <summary>
        /// 按照指定格式格式化响应结果
        /// </summary>
        /// <param name="response">响应数据字符串</param>
        /// <returns>返回格式化后的字符串</returns>
        public string FormatResponse(byte[] response)
        {
            if (null == response || response.Length < 1)
                return "";
            string info = CommandBase.Parse(response, !IsAscii);
            //恢复转义字符
            if (!string.IsNullOrEmpty(Exchanges))
            {
                string[] strExs = Exchanges.Split('#');
                for (int i = 0; i < strExs.Length; i++)
                {
                    string[] _a = strExs[i].Split('/');
                    info = comparse.SpecialRestore(info, _a[1], _a[0]);
                }
            }
            string state = "";
            if (info.StartsWith("") && info.EndsWith(""))
                info = info.Replace("", "02").Replace("", "03");
            try
            {

                info = comparse.AnalysisEateryResults(info, this.outterParam, dvParam, false, ref state);
            }
            catch { info = "@{状态}=响应解析错误,@Success=false"; }
            return info;
        }

        /// <summary>
        /// 是否当前指令的结果,验证设备ID和指令
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool isResponse(CommandBase cmd, byte[] response)
        {
            if (null == response || response.Length < 2)
                return false;
            byte[] bcmd = cmd.getCommand();
            if (null == bcmd || bcmd.Length < 2)
                return false;
            for (int i = 0; i < 3; i++)
                if (bcmd[i] != response[i])
                    return false;
            return true;
        }

        /// <summary>
        /// 重载触发事件,解析响应格式
        /// </summary>
        /// <param name="arg"></param>
        public override void RaiseResponse(ResponseEventArgs arg)
        {
            if (null == arg) return;
            if (arg.Success)
                this.responseFormat = this.FormatResponse(arg.Response);
            else
                this.responseFormat = "";
            base.RaiseResponse(arg);
        }

        #endregion


    }

}
