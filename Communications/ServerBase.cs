using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.IO;
using System.Reflection;

namespace Granity.communications
{
    /// <summary>
    /// 基本服务,为统一服务端模式的基础类
    /// 提供包含协议和服务指令,服务指令的名称使用属性声明
    /// </summary>
    public class ServerBase
    {
        #region 通讯控制属性

        private int port = -1;
        /// <summary>
        /// 读取或设置服务端口号
        /// </summary>
        public int Port
        {
            get { return port; }
            set { port = value; }
        }

        private TimeSpan timeDisconn = new TimeSpan(0, 3, 0);
        /// <summary>
        /// 读取或设置连接断开时间间隔,默认3分钟
        /// </summary>
        public TimeSpan TimeDisconn
        {
            get { return timeDisconn; }
            set { timeDisconn = value; }
        }
        #endregion

        private List<ClientInfo> clientlist = new List<ClientInfo>();
        /// <summary>
        /// 读取连接到服务端的客户端列表,断开时自动删除
        /// </summary>
        public List<ClientInfo> ClientList
        {
            get { return clientlist; }
        }
        

        /// <summary>
        /// 请求事件
        /// </summary>
        public event EventHandler<RequestEventArgs> RequestHandle;

        /// <summary>
        /// 解析数据返回响应数据
        /// </summary>
        /// <param name="clientinfo">通讯连接</param>
        /// <param name="data">数据字节</param>
        /// <param name="len">字节长度</param>
        /// <param name="buffer">已接收数据缓存</param>
        /// <returns>返回执行结果</returns>
        public virtual byte[] Execute(ClientInfo clientinfo, byte[] data, int len, ref MemoryStream stream)
        {
            return new byte[0];
        }

        /// <summary>
        /// 触发请求事件
        /// </summary>
        /// <param name="arg">请求事件参数</param>
        /// <returns>返回-1则失败,不响应请求</returns>
        public virtual int RaiseRequest(RequestEventArgs arg)
        {
            EventHandler<RequestEventArgs> handle = this.RequestHandle;
            if (null != handle)
            {
                try
                {
                    handle(this, arg);
                    return arg.Return;
                }
                catch (Exception ex)
                {
                    ExceptionManager.Publish(ex);
                    return -1;
                }
            }
            return 0;
        }
    }
    /// <summary>
    /// 客户端连接信息
    /// </summary>
    public class ClientInfo
    {
        private List<byte[]> bufferResponse = new List<byte[]>();
        /// <summary>
        /// 缓存响应数据
        /// </summary>
        public List<byte[]> BufferResponse
        {
            get { return bufferResponse; }
        }
        private Socket client = null;
        /// <summary>
        /// 读取连接Socket
        /// </summary>
        public Socket Client
        {
            get { return client; }
        }
        private string ipaddr;
        /// <summary>
        /// 读取连接的客户端IP端口地址
        /// </summary>
        public string IPEndPoint
        {
            get { return ipaddr; }
        }
        private DateTime dtop = DateTime.Now;
        /// <summary>
        /// 读取或设置通讯操作时间
        /// </summary>
        public DateTime OPdt
        {
            get { return dtop; }
            set { dtop = value; }
        }
        /// <summary>
        /// 构造客户端连接信息,一个Socket是连接基础
        /// </summary>
        /// <param name="client">已经建立连接的Socket</param>
        public ClientInfo(Socket client)
        {
            try { ipaddr = client.RemoteEndPoint.ToString(); }
            catch { }
            this.client = client;
        }
    }

    /// <summary>
    /// 定义服务指令
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CommandServerAttribute : Attribute
    {
        public string cmdName;
        /// <summary>
        /// 定义服务指令
        /// </summary>
        /// <param name="cmdname">指令名称</param>
        public CommandServerAttribute(string cmdname)
        {
            this.cmdName = cmdname;
        }
    }

    /// <summary>
    /// 服务指令执行委托
    /// </summary>
    /// <param name="client">通讯连接</param>
    /// <param name="data">请求数据</param>
    /// <returns>返回响应结果</returns>
    public delegate byte[] HdlExecute(ClientInfo clientinfo, byte[] data);

}
