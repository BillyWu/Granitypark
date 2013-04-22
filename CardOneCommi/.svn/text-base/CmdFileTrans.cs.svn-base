#region 版本说明

/*
 * 功能内容：   传输文件协议应用
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-09-15
 * 摘要说明：   文件传输指令,帧头是文件说明的帧头<文件名称,内容长度,标识号,文件路径>，用XML格式化表达；随后是文件内容。
 *              多个请求以标识号相区别
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System.Collections.Specialized;
using System.Web;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Diagnostics;
using Granity.communications;

namespace Granity.CardOneCommi
{
    /// <summary>
    /// 协议命令,约束(a:一个通讯位置只有一套协议在执行;b:一个位置可管理多客户端文件传输请求;c:一个客户端可同时发多个文件请求指令)
    ///               d:当前时刻传输的流中只有一个文件
    ///         文件头协议格式：1,使用一个简单XML格式,属性值可数字字符汉字及.\-符号, 2,包含属性：id,pathfile,len,cmd,msg
    /// 通讯时间默认3秒超时,10秒失败
    /// </summary>
    public class CmdFileTrans : CommandBase
    {
        #region 文件头XML格式辅助

        /// <summary>
        /// 合法字符验证
        /// </summary>
        private static Regex regFilePath = new Regex(@"^[\w\.\\\-]+$", RegexOptions.Compiled);
        /// <summary>
        /// 验证XML格式
        /// </summary>
        private static Regex regXMLFormat = new Regex(@"^<F(\s+.+?)(/>|></F>)$", RegexOptions.Compiled);
        /// <summary>
        /// 文件头信息属性内容
        /// </summary>
        private static Regex regAttribute = new Regex("([\\w\\.\\u4E00-\\u9FA5]+)\\s*=\\s*[\"']([\\w\\s\\:\\.\\\\\\u4E00-\\u9FA5-]*)[\"']", RegexOptions.Compiled);
        /// <summary>
        /// 文件头开始字节
        /// </summary>
        private static byte[] fhStart = Encoding.GetEncoding("GB2312").GetBytes("<F");
        /// <summary>
        /// 文件头自封闭式结尾
        /// </summary>
        private static byte[] fhEndseal = Encoding.GetEncoding("GB2312").GetBytes("/>");
        /// <summary>
        /// 文件头关闭式结尾
        /// </summary>
        private static byte[] fhEndclose = Encoding.GetEncoding("GB2312").GetBytes("></F>");
        
        #endregion

        #region 协议相关
        
        private static Protocol ptl = new Protocol();
        /// <summary>
        /// 读取通讯协议帧头帧尾及键值位置
        /// </summary>
        public static Protocol PTL
        {
            get
            {
                if (null == ptl.MergeListHandle)
                    ptl.MergeListHandle = CmdFileTrans.MergeResponseHdl;
                return ptl;
            }
        }

        private static CommandBase cmdHeaderBeat;
        /// <summary>
        /// 心跳指令,默认1分钟,网络断开则重新连接
        /// </summary>
        private static CommandBase CmdHeaderBeat
        {
            get
            {
                if (null == cmdHeaderBeat)
                {
                    cmdHeaderBeat = new CmdFileTrans("-1", false);
                    cmdHeaderBeat.setCommand("<F id='-1' len='0' cmd='TransFile.beat' />");
                    cmdHeaderBeat.TimeOut = new TimeSpan(0, 0, 10);
                    cmdHeaderBeat.TimeFailLimit = new TimeSpan(0, 0, 30);
                    cmdHeaderBeat.TimeLimit = TimeSpan.MaxValue;
                    cmdHeaderBeat.FailProAf = FailAftPro.Reconn;
                    cmdHeaderBeat.TimeSendInv = new TimeSpan(0, 1, 0);
                }
                return cmdHeaderBeat;
            }
        }

        #endregion

        #region 公共属性

        private byte[] filecontext=new byte[0];
        /// <summary>
        /// 读取响应文件内容
        /// </summary>
        public byte[] FileContext
        {
            get { return filecontext; }
            set { filecontext = value; }
        }
        private NameValueCollection fileheader = new NameValueCollection();
        /// <summary>
        /// 读取响应文件头信息
        /// </summary>
        public NameValueCollection FileHeader
        {
            get { return fileheader; }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认方式构造实例,不初始化同步事件句柄,无指令ID
        /// </summary>
        public CmdFileTrans(): base()
        {
            this.TimeOut = new TimeSpan(0, 0, 3);
            this.TimeFailLimit = new TimeSpan(0, 0, 10);
            this.TimeLimit = new TimeSpan(0, 0, 10);
            this.IsResposeHandle = isResponse;
            this.ResponseHandle += new EventHandler<ResponseEventArgs>(onCommiTransHandle);
        }

        /// <summary>
        /// 构造函数指定构造指令ID(指令ID是可更改的)
        /// </summary>
        /// <param name="id">指令ID</param>
        public CmdFileTrans(string id) : base(id)
        {
            this.TimeOut = new TimeSpan(0, 0, 3);
            this.TimeFailLimit = new TimeSpan(0, 0, 10);
            this.TimeLimit = new TimeSpan(0, 0, 10);
            this.IsResposeHandle = isResponse;
            this.ResponseHandle += new EventHandler<ResponseEventArgs>(onCommiTransHandle);
        }

        /// <summary>
        /// 构造函数,初始化同步事件句柄状态
        /// </summary>
        /// <param name="ewhState">同步事件初始状态</param>
        public CmdFileTrans(bool ewhState) : base(ewhState)
        {
            this.TimeOut = new TimeSpan(0, 0, 3);
            this.TimeFailLimit = new TimeSpan(0, 0, 10);
            this.TimeLimit = new TimeSpan(0, 0, 10);
            this.IsResposeHandle = isResponse;
            this.ResponseHandle += new EventHandler<ResponseEventArgs>(onCommiTransHandle);
        }

        /// <summary>
        /// 构造函数,初始化指令ID,初始化同步事件句柄状态
        /// </summary>
        /// <param name="id">指令ID,使用中可更改</param>
        /// <param name="ewhState">同步事件初始状态</param>
        public CmdFileTrans(string id, bool ewhState) : base(id, ewhState)
        {
            this.TimeOut = new TimeSpan(0, 0, 3);
            this.TimeFailLimit = new TimeSpan(0, 0, 10);
            this.TimeLimit = new TimeSpan(0, 0, 10);
            this.IsResposeHandle = isResponse;
            this.ResponseHandle += new EventHandler<ResponseEventArgs>(onCommiTransHandle);
        }

        #endregion

        #region 协议指令定义

        /// <summary>
        /// 发送文件,传入服务器
        /// </summary>
        /// <param name="filename">文件名称</param>
        /// <param name="path">路径,服务器端相对路径</param>
        /// <param name="fileContext">文件字节内容</param>
        public void SendFile(string pathfile, byte[] context)
        {
            //检查参数,不能有非法字符
            if (!regFilePath.IsMatch(pathfile))
                throw new Exception("文件名称或路径非法！");
            if (string.IsNullOrEmpty(this.CmdId))
                this.CmdId = Guid.NewGuid().ToString();
            NameValueCollection info = new NameValueCollection();
            info.Add("id", this.CmdId);
            info.Add("cmd", "TransFile.send");
            info.Add("len", context.Length.ToString());
            info.Add("pathfile", pathfile);
            byte[] fh = SvrFileTrans.ParseInfo(info);
            byte[] data = new byte[fh.Length + context.Length];
            Array.Copy(fh, data, fh.Length);
            Array.Copy(context, 0, data, fh.Length, context.Length);
            this.setCommand(data);
        }
        /// <summary>
        /// 获取文件
        /// </summary>
        /// <param name="pathfile">路径文件</param>
        public void GetFile(string pathfile)
        {
            //检查参数,不能有非法字符
            if (!regFilePath.IsMatch(pathfile))
                throw new Exception("文件名称或路径非法！");
            if (string.IsNullOrEmpty(this.CmdId))
                this.CmdId = Guid.NewGuid().ToString();
            NameValueCollection info = new NameValueCollection();
            info.Add("id", this.CmdId);
            info.Add("cmd", "TransFile.get");
            info.Add("len", "0");
            info.Add("pathfile", pathfile);
            byte[] fh = SvrFileTrans.ParseInfo(info);
            this.setCommand(fh);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="pathfile">路径文件</param>
        public void DelFile(string pathfile)
        {
            //检查参数,不能有非法字符
            if (!regFilePath.IsMatch(pathfile))
                throw new Exception("文件名称或路径非法！");
            if (string.IsNullOrEmpty(this.CmdId))
                this.CmdId = Guid.NewGuid().ToString();
            NameValueCollection info = new NameValueCollection();
            info.Add("id", this.CmdId);
            info.Add("cmd", "TransFile.del");
            info.Add("len", "0");
            info.Add("pathfile", pathfile);
            byte[] fh = SvrFileTrans.ParseInfo(info);
            this.setCommand(fh);
        }
        /// <summary>
        /// 扩展服务功能,触发同步设备控制参数
        /// </summary>
        /// <param name="service">扩展服务名</param>
        /// <param name="msg">附加信息,根据服务定义(不能包含：id,cmd,len,service)</param>
        public void ExtService(ServiceType servicetype, NameValueCollection info)
        {
            if (string.IsNullOrEmpty(this.CmdId))
                this.CmdId = Guid.NewGuid().ToString();
            NameValueCollection data = new NameValueCollection();
            if (null != info)
            {
                foreach (string k in info.AllKeys)
                    data.Set(k, info[k]);
            }
            //定义扩展
            string service = "";
            switch (servicetype)
            {
                case ServiceType.UpdatePmDevice:
                    service = "updateparam";
                    break;
                case ServiceType.MonitorDevice:
                    service = "monitor";
                    break;
                case ServiceType.HaltDevice:
                    service = "halt";
                    break;
                case ServiceType.ReadInfodev:
                    service = "readinfo";
                    break;
            }
            data["service"] = service;
            string[,] vals ={ { "id", this.CmdId }, { "cmd", "TransFile.extend" }, { "len", "0" } };
            for (int i = 0; i < vals.GetLength(0); i++)
                data[vals[i, 0]] = vals[i, 1];
            byte[] fh = SvrFileTrans.ParseInfo(data);
            this.setCommand(fh);
        }

        /// <summary>
        /// 对目标通讯启动心跳,已经有则不会重复
        /// </summary>
        /// <param name="mgr">通讯管理器</param>
        /// <param name="target">通讯目标</param>
        public static void OpenHeaderBeat(CommiManager mgr, CommiTarget target)
        {
            if (null == mgr || null == target)
                return;
            target.setProtocol(CmdFileTrans.PTL);
            mgr.SendCommand(target, CmdHeaderBeat);
        }
        /// <summary>
        /// 对目标通讯关闭心跳
        /// </summary>
        /// <param name="mgr">通讯管理器</param>
        /// <param name="target">通讯目标</param>
        public static void CloseHeaderBeat(CommiManager mgr, CommiTarget target)
        {
            if (null == mgr || null == target)
                return;
            mgr.RemoveCommand(target, CmdHeaderBeat);
        }

        /// <summary>
        /// 传递通讯指令
        ///     包含参数: len,dir=request/response,cmd=TransFile.trans,source,target,
        ///               CommiType,addr=ipaddr/COM1,port,baudRate,parity,dataBits,stopBits
        /// </summary>
        /// <param name="src">当前指令自身IP地址</param>
        /// <param name="proxy">执行目标的代理IP地址</param>
        /// <param name="dest">最终目标位置</param>
        /// <param name="dtpl">目标协议类型名称</param>
        /// <param name="dcmd">目标指令名称</param>
        /// <param name="data">传送的指令字节,本地执行时返回响应字节,非本地执行则返回0长度字节</param>
        /// <returns></returns>
        public void CommiTrans(IPAddress proxy, CommiTarget dest,string dtpl, string dcmd, ref byte[] data)
        {
            if (null == proxy || null == dest || null == data || data.Length < 1)
                return;
            string ipaddr = proxy.ToString();
            if (string.IsNullOrEmpty(this.CmdId))
                this.CmdId = Guid.NewGuid().ToString();
            NameValueCollection info = new NameValueCollection();
            info["dtpl"] = dtpl;
            info["dcmd"] = dcmd;
            string[,] head ={ {"id",this.CmdId},{"cmd","TransFile.trans"},{"len", data.Length.ToString()},
                        {"dir","request"},{"target",proxy.ToString()},{"CommiType",dest.ProtocolType.ToString()}};
            for (int i = 0; i < head.GetLength(0); i++)
                info.Add(head[i, 0], head[i, 1]);
            if (CommiType.SerialPort != dest.ProtocolType)
            {
                info.Add("addr", dest.SrvEndPoint.Address.ToString());
                info.Add("port", dest.SrvEndPoint.Port.ToString());
            }
            else
            {
                info.Add("addr", dest.PortName);
                info.Add("baudRate", dest.BaudRate.ToString());
                info.Add("parity", dest.Parity.ToString());
                info.Add("dataBits", dest.DataBits.ToString());
                info.Add("stopBits", dest.StopBits.ToString());
            }
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
            if (islocal || proxy.ToString() == info["addr"])
            {
                info["dir"] = "response";
                info["source"] = info["target"];
                CmdProtocol cmd = new CmdProtocol(false);
                cmd.setCommand(dtpl, dcmd, "");
                cmd.setCommand(data);
                CommiManager.GlobalManager.SendCommand(dest, cmd);
                if (!cmd.EventWh.WaitOne(800, false))
                    info["msg"] = "目标通讯超时失败";
                data = cmd.ResponseData;
                info["len"] = data.Length.ToString();
                CommiManager.GlobalManager.RemoveCommand(dest, cmd);
            }
            byte[] fh = SvrFileTrans.ParseInfo(info);
            byte[] context = new byte[fh.Length + data.Length];
            Array.Copy(fh, context, fh.Length);
            Array.Copy(data, 0, context, fh.Length, data.Length);
            this.setCommand(context);
            if (!islocal) data = new byte[0];
        }

        /// <summary>
        /// 接收传递通讯
        ///     包含参数: len,dir=request/response,cmd=TransFile.trans,source,target,
        ///               CommiType,addr=ipaddr/COM1,port,baudRate,parity,dataBits,stopBits
        /// </summary>
        /// <param name="sender">触发事件的实例</param>
        /// <param name="e">响应参数</param>
        private static void onCommiTransHandle(object sender, ResponseEventArgs e)
        {
            CmdFileTrans trans = sender as CmdFileTrans;
            if (null == trans || null == e || null == e.Target)
                return;
            NameValueCollection ps = trans.FileHeader;
            byte[] data = trans.FileContext;
            if ("TransFile.trans" != ps["cmd"] || "request" != ps["dir"])
                return;
            if (string.IsNullOrEmpty(ps["CommiType"]) || string.IsNullOrEmpty(ps["addr"]))
                return;
            CommiType commitype = CommiType.SerialPort;
            string addr = ps["addr"];
            int port = -1;
            //检测地址
            try
            {
                addr = addr.ToUpper();
                if (!addr.StartsWith("COM"))
                    commitype = (CommiType)Enum.Parse(typeof(CommiType), ps["CommiType"], true);
                if (!string.IsNullOrEmpty(ps["port"]))
                    port = Convert.ToInt32(ps["port"]);
            }
            catch { return; }
            //检测通讯参数
            if (CommiType.SerialPort != commitype && port < 0)
                return;
            if (CommiType.SerialPort == commitype &&
                (string.IsNullOrEmpty(ps["baudRate"]) || string.IsNullOrEmpty(ps["parity"])
                    || string.IsNullOrEmpty(ps["dataBits"]) || string.IsNullOrEmpty(ps["stopBits"])))
                return;
            //创建通讯目标
            CommiTarget target = null;
            if (CommiType.SerialPort != commitype)
                target = new CommiTarget(addr, port, commitype);
            else
                try
                {
                    int baudRate = Convert.ToInt32(ps["baudRate"]);
                    Parity parity = (Parity)Enum.Parse(typeof(Parity), ps["parity"], true);
                    int databits = Convert.ToInt16(ps["dataBits"]);
                    StopBits stopBits = (StopBits)Enum.Parse(typeof(StopBits), ps["stopBits"], true);
                    target = new CommiTarget(addr, baudRate, parity, databits, stopBits);
                }
                catch { return; }
            target.setProtocol(Protocol.PTLCard);
            switch (ps["dtpl"])
            {
                case "停车场":
                    target.setProtocol(Protocol.PTLPark); break;
                case "门禁":
                    target.setProtocol(Protocol.PTLDoor); break;
                case "消费":
                    target.setProtocol(Protocol.PTLEatery); break;
            }
            //执行传递的指令
            CmdProtocol cmd = new CmdProtocol(false);
            cmd.setCommand(data);
            ps["dir"] = "response";
            CommiManager.GlobalManager.SendCommand(target, cmd);
            if (!cmd.EventWh.WaitOne(800, false))
                ps["msg"] = "目标通讯超时失败";
            data = cmd.ResponseData;
            ps["len"] = data.Length.ToString();
            byte[] fh = SvrFileTrans.ParseInfo(ps);
            byte[] response = new byte[fh.Length + data.Length];
            Array.Copy(fh, response, fh.Length);
            if (data.Length > 0)
                Array.Copy(data, 0, response, fh.Length, data.Length);
            cmd.setCommand(response);
            cmd.ResetState();
            CommiManager.GlobalManager.SendCommand(e.Target, cmd);
        }

        #endregion

        #region 监听响应触发指令响应事件

        /// <summary>
        /// 由相对路径转换本地路径
        /// </summary>
        /// <param name="pathfile">请求指令中的相对路径</param>
        /// <returns>本地路径</returns>
        private static string getLocalPath(string pathfile)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (null != HttpContext.Current && null != HttpContext.Current.Server)
                pathfile = HttpContext.Current.Server.MapPath(pathfile);
            else if (!string.IsNullOrEmpty(baseDir))
            {
                if (pathfile.StartsWith("~/"))
                    pathfile = pathfile.Replace("~/", baseDir);
                else if (!pathfile.Contains(":"))
                    pathfile = Path.Combine(baseDir, pathfile);
                pathfile = pathfile.Replace("/", "\\");
            }
            return pathfile;
        }

        /// <summary>
        /// 对响应结果按照协议分成完整包,返回剩余不完整包
        /// </summary>
        /// <param name="buffer">数据字节</param>
        /// <param name="destlist">完整包列表</param>
        /// <returns>返回剩余不完整包</returns>
        private static IList<byte[]> MergeResponseHdl(byte[] buffer, int len, ref byte[] temp, IList<byte[]> destlist)
        {
            if (null == buffer || buffer.Length < 1 || buffer.Length < len)
                return destlist;
            if (null == temp || temp.Length < 1)
                temp = new byte[len];
            else
                Array.Resize<byte>(ref temp, temp.Length + len);
            Array.Copy(buffer, 0, temp, temp.Length - len, len);
            buffer = temp;

            int index = 0;
            byte bstart = fhStart[0];
            byte[] header = SvrFileTrans.GetFileheader(buffer, index);
            while (null != header && header.Length > 0)
            {
                int flen = -1;
                int hlen = header.Length;
                NameValueCollection info = SvrFileTrans.ParseInfo(header);
                try { flen = Convert.ToInt32(info["len"]); }
                catch { }
                if (flen < 0)
                {
                    //截取数据字节
                    header = null;
                    for (index = index + hlen; index < buffer.Length; index++)
                    {
                        if (bstart != buffer[index])
                            continue;
                        header = SvrFileTrans.GetFileheader(buffer, index);
                        break;
                    }
                    continue;
                }
                //不完整字节返回
                if (index + hlen + flen > buffer.Length)
                    break;
                //完整字节并加入列表
                if (flen > 0)
                {
                    Array.Resize<byte>(ref header, header.Length + flen);
                    Array.Copy(buffer, index + hlen, header, hlen, flen);
                }
                destlist.Add(header);
                index = index + hlen + flen;
                header = SvrFileTrans.GetFileheader(buffer, index);
            }
            //在有很长字节时仍然没有完整头信息则认为字节无效
            if (index < 1 && buffer.Length > 2000 && (null == header || header.Length < 1))
                temp = new byte[0];
            if (index > 0)
            {
                temp = new byte[buffer.Length - index];
                if (temp.Length > 0)
                    Array.Copy(buffer, index, temp, 0, buffer.Length - index);
            }
            return destlist;
        }

        /// <summary>
        /// 是否当前指令的结果,验证设备ID和指令
        /// </summary>
        /// <param name="response">响应字节</param>
        /// <returns>是否与指令相对应</returns>
        private static bool isResponse(CommandBase cmd, byte[] response)
        {
            if (!(cmd is CmdFileTrans) || string.IsNullOrEmpty(cmd.CmdId))
                return false;
            byte[] header = SvrFileTrans.GetFileheader(response);
            if (null == header || header.Length < 1)
                return false;
            NameValueCollection info = SvrFileTrans.ParseInfo(header);
            if (cmd.CmdId == info["id"])
                return true;
            if ("TransFile.beat" == info["cmd"] && "-1" == cmd.CmdId)
                return true;
            if ("request" == info["dir"] && "TransFile.trans" == info["cmd"] && "-1" == cmd.CmdId)
                return true;
            return false;
        }

        /// <summary>
        /// 重载触发事件,解析响应格式
        /// </summary>
        /// <param name="arg"></param>
        public override void RaiseResponse(ResponseEventArgs arg)
        {
            if (null == arg) return;
            this.fileheader.Clear();
            this.filecontext = new byte[0];
            if (arg.Response.Length < 1 || !arg.Success)
            {
                base.RaiseResponse(arg);
                return;
            }
            //检查文件头信息
            byte[] header = SvrFileTrans.GetFileheader(arg.Response);
            this.fileheader = SvrFileTrans.ParseInfo(header);
            if (arg.Response.Length > header.Length)
            {
                this.filecontext = new byte[arg.Response.Length - header.Length];
                Array.Copy(arg.Response, header.Length, this.filecontext, 0, this.filecontext.Length);
            }
            base.RaiseResponse(arg);
        }

        #endregion

    }
}
