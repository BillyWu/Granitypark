using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System.Reflection;
using System.Collections.Specialized;
using System.Web;
using System.IO;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.Threading;

namespace Granity.communications
{
    /// <summary>
    /// 文件上传到服务器端保存,包含有增删改查功能
    ///     文件头协议格式：1,使用一个简单XML格式,属性值可数字字符汉字及.\-符号, 2,包含属性：id,pathfile,len,cmd,msg
    /// </summary>
    public class SvrFileTrans:ServerBase
    {
        #region 文件头XML格式辅助

        /// <summary>
        /// 合法字符验证
        /// </summary>
        private static Regex regFilePath = new Regex(@"^[\w\.]+$", RegexOptions.Compiled);
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

        #region 解析文件协议
        
        /// <summary>
        /// 从响应字节中检查匹配的头信息
        /// </summary>
        /// <param name="data">数据字节</param>
        /// <param name="index">寻找匹配的启点</param>
        /// <returns>有头信息返回头信息字节数据,无则返回NULL</returns>
        public static byte[] GetFileheader(byte[] data, int index)
        {
            byte[] blank = new byte[0];
            if (null == data || data.LongLength < 5 || index >= data.Length)
                return blank;
            //检查文件头起点
            for (int i = 0; i < fhStart.Length; i++)
                if (fhStart[i] != data[i + index])
                    return blank;
            //检查文件头结尾
            byte bend = fhEndseal[fhEndseal.Length - 1];
            for (int i = index; i < data.Length; i++)
            {
                if (bend != data[i])
                    continue;
                bool ismatch = true;
                //检查匹配自封闭的XML段
                for (long j = i - 1, k = fhEndseal.Length - 2; ismatch && k > -1; k--, j--)
                    if (fhEndseal[k] != data[j])
                        ismatch = false;
                //检查匹配节点关闭的XML段
                if (!ismatch)
                {
                    if (i <= fhEndclose.Length)
                        continue;
                    ismatch = true;
                    for (long j = i - 1, k = fhEndclose.Length - 2; ismatch && k > -1; k--, j--)
                        if (fhEndclose[k] != data[j])
                            ismatch = false;
                }
                if (!ismatch) continue;
                //匹配XML段
                byte[] header = new byte[i + 1 - index];
                Array.Copy(data, index, header, 0, i + 1 - index);
                return header;
            }
            return blank;
        }

        /// <summary>
        /// 从响应字节中检查匹配的头信息
        /// </summary>
        /// <param name="data">数据字节</param>
        /// <returns>有头信息返回头信息字节数据,无则返回NULL</returns>
        public static byte[] GetFileheader(byte[] data)
        {
            return GetFileheader(data, 0);
        }

        /// <summary>
        /// 头信息解析为字典对
        /// </summary>
        /// <param name="header">头字节</param>
        /// <returns>字典对</returns>
        public static NameValueCollection ParseInfo(byte[] header)
        {
            NameValueCollection info = new NameValueCollection();
            if (null == header || header.Length < 1)
                return info;
            string str = Encoding.GetEncoding("GB2312").GetString(header);
            MatchCollection mts = regAttribute.Matches(str);
            foreach (Match m in mts)
            {
                if (m.Groups.Count > 2 && !string.IsNullOrEmpty(m.Groups[1].Value))
                    info[m.Groups[1].Value] = m.Groups[2].Value;
            }
            return info;
        }

        /// <summary>
        /// 头信息解析为字典对
        /// </summary>
        /// <param name="header">头字节</param>
        /// <returns>字典对</returns>
        public static byte[] ParseInfo(NameValueCollection info)
        {
            if (null == info || info.Count < 1)
                return new byte[0];
            string header = "<F";
            foreach (string k in info.AllKeys)
                header += string.Format(" {0}='{1}'", k, info[k]);
            header += "/>";
            return Encoding.GetEncoding("GB2312").GetBytes(header);
        }

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
        private static byte[] MergeResponseHdl(byte[] buffer, IList<byte[]> destlist)
        {
            if (null == buffer || buffer.Length < 1 )
                return buffer;

            int index = 0;
            byte bstart = fhStart[0];
            byte[] header = GetFileheader(buffer,index);
            while (null != header && header.Length > 0)
            {
                int flen = -1;
                int hlen = header.Length;
                NameValueCollection info = ParseInfo(header);
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
                        header = GetFileheader(buffer, index);
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
                header = GetFileheader(buffer, index);
            }
            //在有很长字节时仍然没有完整头信息则认为字节无效
            if (index < 1 && buffer.Length > 2000 && (null == header || header.Length < 1))
                return new byte[0];

            if (index < 1) return buffer;
            byte[] temp = new byte[buffer.Length - index];
            if (temp.Length > 0)
                Array.Copy(buffer, index, temp, 0, buffer.Length - index);
            return temp;
        }

        #endregion

        #region 协议功能指令基础

        private static Dictionary<string, HdlExecute> cmdMap = new Dictionary<string, HdlExecute>();
        /// <summary>
        /// 提供服务的指令映射列表
        /// </summary>
        private  Dictionary<string, HdlExecute> CmdMap
        {
            get
            {
                if (cmdMap.Count < 1)
                {
                    MethodInfo[] mds = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                    foreach (MethodInfo m in mds)
                    {
                        object[] attr = m.GetCustomAttributes(typeof(CommandServerAttribute), false);
                        if (attr.Length < 1) continue;
                        HdlExecute hdl = Delegate.CreateDelegate(typeof(HdlExecute), this, m) as HdlExecute;
                        foreach (CommandServerAttribute cmd in attr)
                        {
                            if (null == cmd) continue;
                            cmdMap.Add(cmd.cmdName, hdl);
                        }
                    }
                }
                return cmdMap;
            }
        }

        /// <summary>
        /// 重载执行函数,不完整帧时接收数据缓存
        /// </summary>
        /// <param name="client">通讯连接</param>
        /// <param name="data">接收请求数据</param>
        /// <param name="bufferStream">缓存字节流</param>
        /// <returns>返回响应数据,帧不完整返回0字节,待接收完整帧执行后返回响应结果</returns>
        public override byte[] Execute(ClientInfo clientinfo, byte[] data, int len, ref MemoryStream stream)
        {
            IList<byte[]> list = new List<byte[]>();
            byte[] buffer = new byte[0];
            //没有头信息的，读入字节合并后检查头信息
            if (stream.Position < 1)
            {
                buffer = new byte[stream.Length + len];
                stream.Read(buffer, 0, (int)stream.Length);
                Array.Copy(data, 0, buffer, stream.Length, len);
            }
            else
            {
                //读取数据流头信息,直接读入流
                int pos = (int)stream.Position;
                buffer = new byte[pos];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(buffer, 0, pos);
                NameValueCollection info = ParseInfo(buffer);
                long flen = Convert.ToInt64(info["len"]);
                stream.Seek(0, SeekOrigin.End);
                //大字节流继续读入
                if (flen > stream.Length + len - pos)
                {
                    stream.Write(data, 0, len);
                    stream.Seek(pos, SeekOrigin.Begin);
                    return new byte[0];
                }
                int lennext = (int)(stream.Length + len - pos - flen);
                if (lennext > len)
                {
                    //上次流中已经有完整信息
                    buffer = new byte[pos + flen];
                    stream.Seek(0, SeekOrigin.Begin);
                    stream.Read(buffer, 0, buffer.Length);
                    list.Add(buffer);
                    buffer = new byte[lennext];
                    stream.Read(buffer, 0, (int)stream.Length - pos - (int)flen);
                    Array.Copy(data, buffer, len);
                }
                else
                {
                    //大字节流完整字节加入列表,后续字节继续分析头信息
                    if (len > lennext)
                        stream.Write(data, 0, len - lennext);
                    list.Add(stream.ToArray());
                    buffer = new byte[lennext];
                    Array.Copy(data, len - lennext, buffer, 0, lennext);
                }
            }
            buffer = MergeResponseHdl(buffer, list);
            if (stream.Length > 0 || buffer.Length > 0)
            {
                stream = new MemoryStream(buffer);
                data = GetFileheader(buffer);
                if (null != data && data.Length > 0)
                    stream.Seek(data.Length, SeekOrigin.Begin);
            }
            for (int i = list.Count - 1; i > -1; i--)
            {
                byte[] context = list[i];
                byte[] header = GetFileheader(context);
                NameValueCollection info = ParseInfo(header);
                RequestEventArgs args = new RequestEventArgs(null, this, context);
                int rtn = this.RaiseRequest(args);
                string keycmd = info["cmd"];
                if (string.IsNullOrEmpty(keycmd) || !this.CmdMap.ContainsKey(keycmd))
                {
                    list.RemoveAt(i);
                    continue;
                }
                if (rtn < 0)
                    continue;
                HdlExecute hdl = this.CmdMap[info["cmd"]];
                //执行指令获取响应结果
                try
                {
                    list[i] = hdl(clientinfo, context);
                }
                catch (Exception ex)
                {
                    ExceptionManager.Publish(ex);
                    list.RemoveAt(i);
                }
            }
            //输出响应
            if (list.Count < 1)
                return new byte[0];
            if (list.Count < 2)
                return list[0];
            //多个请求时合并输出字节
            long lend = 0;
            foreach (byte[] b in list)
                lend += b.LongLength;
            data = new byte[lend];
            lend = 0;
            foreach (byte[] b in list)
            {
                Array.Copy(b, 0, data, lend, b.LongLength);
                lend += b.LongLength;
            }
            return data;
        }

        #endregion

        /// <summary>
        /// 扩展服务事件
        /// </summary>
        public event EventHandler<ExtendEventArgs> ExtendHandle;

        #region 协议指令定义

        /// <summary>
        /// 加载文件,执行结果返回
        /// </summary>
        /// <param name="client">通讯连接</param>
        /// <param name="context">文件头和文件内容</param>
        /// <returns>返回执行结果文件头</returns>
        [CommandServer("TransFile.send")]
        public byte[] LoadFile(ClientInfo clientinfo, byte[] context)
        {
            byte[] header = GetFileheader(context);
            NameValueCollection info = ParseInfo(header);
            if (string.IsNullOrEmpty(info["id"]) || string.IsNullOrEmpty(info["pathfile"]))
                return new byte[0];
            FileStream fs = null;
            try
            {
                string pathfile = getLocalPath(info["pathfile"]);
                string dir = Path.GetDirectoryName(pathfile);
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                fs = File.Open(pathfile, FileMode.OpenOrCreate);
                fs.Write(context, header.Length, context.Length - header.Length);
                fs.Close();
                info["msg"] = "文件保存成功";
                info["success"] = "true";
            }
            catch (Exception ex)
            {
                if (null != fs)
                    fs.Close();
                ExceptionManager.Publish(ex);
                info["msg"] = ex.Message.Replace("\"", "“").Replace("'", "‘");
                info["success"] = "false";
            }
            info["len"] = "0";
            return ParseInfo(info);
        }

        /// <summary>
        /// 获取文件,执行结果返回
        /// </summary>
        /// <param name="client">通讯连接</param>
        /// <param name="context">文件头和文件内容</param>
        /// <returns>返回执行结果头信息和文件内容</returns>
        [CommandServer("TransFile.get")]
        public byte[] GetFile(ClientInfo clientinfo, byte[] context)
        {
            byte[] header = GetFileheader(context);
            NameValueCollection info = ParseInfo(header);
            if (string.IsNullOrEmpty(info["id"]) || string.IsNullOrEmpty(info["pathfile"]))
                return new byte[0];
            context = new byte[0];
            try
            {
                string pathfile = getLocalPath(info["pathfile"]);
                if (!File.Exists(pathfile))
                {
                    info["msg"] = "文件不存在";
                    info["success"] = "false";
                }
                else
                {
                    context = File.ReadAllBytes(pathfile);
                    info["msg"] = "文件读取成功";
                    info["success"] = "true";
                }
            }
            catch (Exception ex)
            {
                context = new byte[0];
                ExceptionManager.Publish(ex);
                info["msg"] = ex.Message.Replace("\"", "“").Replace("'", "‘");
                info["success"] = "false";
            }
            info["len"] = Convert.ToString(context.LongLength);
            byte[] response = ParseInfo(info);
            if (context.Length > 1)
            {
                long len=response.Length;
                Array.Resize<byte>(ref response, (int)(len + context.LongLength));
                Array.Copy(context, 0, response, len, context.LongLength);
            }
            return response;
        }

        /// <summary>
        /// 删除文件,执行结果返回
        /// </summary>
        /// <param name="context">文件头和文件内容</param>
        /// <returns>返回执行结果头信息</returns>
        [CommandServer("TransFile.del")]
        public byte[] DelFile(ClientInfo clientinfo, byte[] context)
        {
            byte[] header = GetFileheader(context);
            NameValueCollection info = ParseInfo(header);
            if (string.IsNullOrEmpty(info["id"]) || string.IsNullOrEmpty(info["pathfile"]))
                return new byte[0];
            try
            {
                string pathfile = getLocalPath(info["pathfile"]);
                if (File.Exists(pathfile))
                {
                    File.Delete(pathfile);
                    info["msg"] = "文件删除成功";
                    info["success"] = "true";
                }
            }
            catch (Exception ex)
            {
                ExceptionManager.Publish(ex);
                info["msg"] = ex.Message.Replace("\"", "“").Replace("'", "‘");
                info["success"] = "false";
            }
            info["len"] = "0";
            return ParseInfo(info);
        }

        /// <summary>
        /// 心跳,无执行直接返回
        /// </summary>
        /// <param name="client">通讯连接</param>
        /// <param name="context">文件头和文件内容</param>
        /// <returns>返回执行结果头信息</returns>
        [CommandServer("TransFile.beat")]
        public byte[] HeadBeat(ClientInfo clientinfo, byte[] context)
        {
            byte[] header = GetFileheader(context);
            NameValueCollection info = ParseInfo(header);
            if (string.IsNullOrEmpty(info["id"]))
                return new byte[0];
            info["success"] = "true";
            info["len"] = "0";
            return ParseInfo(info);
        }
        /// <summary>
        /// 传递通讯,作为通讯的中转桥,既传送请求指令又传送响应结果
        ///     包含参数: len,dir=request/response,cmd=TransFile.trans,source,target,
        ///               CommiType,addr=ipaddr/COM1,port,baudRate,parity,dataBits,stopBits
        /// </summary>
        /// <param name="client">通讯连接</param>
        /// <param name="context">头信息和要传送的指令或结果</param>
        /// <returns>返回0字节,这里返回数据需要的是异步过程,由响应事件后再回传数据</returns>
        [CommandServer("TransFile.trans")]
        public byte[] CommiTrans(ClientInfo clientinfo, byte[] context)
        {
            byte[] header = GetFileheader(context);
            NameValueCollection info = ParseInfo(header);
            if (string.IsNullOrEmpty(info["id"]) || string.IsNullOrEmpty(info["dir"])
                || string.IsNullOrEmpty(info["target"]))
                return new byte[0];
            string ipaddr = info["target"];
            if ("response" == info["dir"])
                ipaddr = info["source"];
            else
            {
                info["source"] = clientinfo.IPEndPoint.Substring(0, clientinfo.IPEndPoint.IndexOf(":"));
                byte[] hf = ParseInfo(info);
                byte[] data = new byte[hf.Length + context.Length - header.Length];
                Array.Copy(hf, data, hf.Length);
                Array.Copy(context, header.Length, data, hf.Length, context.Length - header.Length);
                context = data;
            }
            //加入客户端连接信息的响应列表中
            Monitor.Enter(this);
            ClientInfo[] clients = this.ClientList.ToArray();
            Monitor.PulseAll(this);
            Monitor.Exit(this);
            foreach (ClientInfo cf in clients)
                if (cf.IPEndPoint.StartsWith(ipaddr + ":"))
                {
                    Monitor.Enter(cf);
                    cf.BufferResponse.Add(context);
                    Monitor.PulseAll(cf);
                    Monitor.Exit(cf);
                }
            return new byte[0];
        }

        /// <summary>
        /// 扩展服务通讯,作为系统扩展应用,能够触发扩展事件
        ///     包含参数: len,cmd=TransFile.extend,及其他扩展参数
        /// </summary>
        /// <param name="client">通讯连接</param>
        /// <param name="context">头信息和要传送的指令或结果</param>
        /// <returns>返回传送</returns>
        [CommandServer("TransFile.extend")]
        public byte[] ExtendSrv(ClientInfo clientinfo, byte[] context)
        {
            EventHandler<ExtendEventArgs> handle = this.ExtendHandle;
            if (null == handle)
                return context;
            ExtendEventArgs args = new ExtendEventArgs(this, clientinfo, context);
            ThreadManager.QueueUserWorkItem(delegate(object obj) { handle(this, obj as ExtendEventArgs); }, args);
            byte[] header = GetFileheader(context);
            NameValueCollection info = ParseInfo(header);
            if ("false" == info["response"])
                return new byte[0];
            info["len"] = "0";
            info["success"] = "true";
            info["msg"] = "触发扩展事件";
            return ParseInfo(info);
        }

        #endregion
    }
}
