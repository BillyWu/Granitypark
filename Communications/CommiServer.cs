using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Microsoft.ApplicationBlocks.ExceptionManagement;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace Granity.communications
{
    /// <summary>
    /// 统一模式通讯服务
    /// </summary>
    public class CommiServer
    {
        /// <summary>
        /// 监听端口列表
        /// </summary>
        private Dictionary<int, TcpListener> listenermap = new Dictionary<int, TcpListener>();

        private static CommiServer globalServer = null;
        /// <summary>
        /// 全局实例
        /// </summary>
        public static CommiServer GlobalServer
        {
            get 
            {
                if (null == globalServer)
                    globalServer = new CommiServer();
                return globalServer; 
            }
        }
        /// <summary>
        /// 请求事件
        /// </summary>
        public event EventHandler<RequestEventArgs> RequestHandle;
        /// <summary>
        /// 响应事件
        /// </summary>
        public event EventHandler<ResponseSrvEventArgs> ResponseHandle;
        /// <summary>
        /// 异常时事件
        /// </summary>
        public event EventHandler<ErrorRequestEventArgs> ErrorHandle;
        /// <summary>
        /// 断开连接时事件
        /// </summary>
        public event EventHandler<DisconnEventArgs> DisconnHandle;

        /// <summary>
        /// 启动服务,已经启动则失败
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="server">服务器</param>
        public void Start(int port, ServerBase server)
        {
            if (port < 1024 || null == server)
                return;
            if (this.listenermap.ContainsKey(port))
                return;
            ThreadManager.QueueUserWorkItem(delegate(object obj) { this.start(port, server); }, null);
        }

        /// <summary>
        /// 停止服务
        /// </summary>
        /// <param name="port">端口号</param>
        public void Stop(int port)
        {
            if (!this.listenermap.ContainsKey(port))
                return;
            TcpListener lsn = this.listenermap[port];
            this.listenermap.Remove(port);
            lsn.Stop();
        }
        /// <summary>
        /// 停止所有端口服务
        /// </summary>
        public void Stop()
        {
            foreach(TcpListener lsn in this.listenermap.Values)
                lsn.Stop();
            this.listenermap.Clear();
        }

        /// <summary>
        /// 启动服务,已经启动则忽略,被占用则忽略加入和启动
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="server">服务器</param>
        private void start(int port, ServerBase server)
        {
            if (port < 1024 || null == server)
                return;
            if (this.listenermap.ContainsKey(port))
                return;
            server.Port = port;
            IPEndPoint localport = new IPEndPoint(IPAddress.Any, port);
            TcpListener listener = new TcpListener(localport);
            this.listenermap.Add(port, listener);
            try
            {
                listener.Start();
            }
            catch
            {
                this.listenermap.Remove(port);
                return;
            }
            while (true)
            {
                Socket client = null;
                try
                {
                    client = listener.AcceptSocket();
                }
                catch (Exception ex)
                {
                    ErrorRequestEventArgs args = new ErrorRequestEventArgs(client, server, new byte[0], ex);
                    this.raiseEvent(args);
                    if (!this.listenermap.ContainsKey(port))
                        return;
                    listener.Start();
                    continue;
                }
                ClientInfo info = new ClientInfo(client);
                Monitor.Enter(server);
                server.ClientList.Add(info);
                Monitor.PulseAll(server);
                Monitor.Exit(server);
                ThreadManager.QueueUserWorkItem(delegate(object obj) { this.readData(info, server); }, null);
                ThreadManager.QueueUserWorkItem(delegate(object obj) { this.writeData(info, server); }, null);
            }
        }
        /// <summary>
        /// 服务器读取通讯数据
        /// </summary>
        /// <param name="client">客户端连接</param>
        /// <param name="server">服务器</param>
        private void readData(ClientInfo clientinfo, ServerBase server)
        {
            if (null == clientinfo || null == server)
                return;
            Socket client = clientinfo.Client;
            bool isopen = false;
            int iM = 1048576, available = 0;
            try
            {
                isopen = client.Connected;
                available = isopen ? client.Available : 0;
            }
            catch { return; }

            List<byte[]> responseList = clientinfo.BufferResponse;
            byte[] buffer = new byte[client.ReceiveBufferSize];
            MemoryStream stream = new MemoryStream();
            byte[] b = null;
            while (true)
            {
                while (available > 0)
                {
                    try
                    {
                        //执行请求
                        byte[] request = stream.ToArray();
                        long pos = stream.Position;
                        int len = client.Receive(buffer);
                        available = client.Available;
                        clientinfo.OPdt = DateTime.Now;
                        if (request.Length > 0)
                        {
                            stream = new MemoryStream();
                            long lenr = request.LongLength;
                            for (int i = 0; i * iM < lenr; i++)
                                stream.Write(request, iM * i, lenr > (i + 1) * iM ? iM : (int)(lenr - i * iM));
                        }
                        stream.Seek(pos, SeekOrigin.Begin);
                        byte[] reqarg = new byte[len];
                        Array.Copy(buffer, reqarg, len);
                        RequestEventArgs argreq = new RequestEventArgs(client, server, reqarg);
                        this.raiseEvent(argreq);
                        b = server.Execute(clientinfo, buffer, len, ref stream);
                    }
                    catch (Exception ex)
                    {
                        byte[] request = stream.ToArray();
                        stream = new MemoryStream();
                        ErrorRequestEventArgs argerr = new ErrorRequestEventArgs(client, server, request, ex);
                        this.raiseEvent(argerr);
                        if (ex is SocketException)
                        {
                            isopen = false;
                            break;
                        }
                        continue;
                    }
                    //响应结果写入缓存,触发同步事件写入响应
                    if (null != b && b.Length > 0)
                    {
                        Monitor.Enter(clientinfo);
                        responseList.Add(b);
                        Monitor.PulseAll(clientinfo);
                        Monitor.Exit(clientinfo);
                    }
                }//while (available > 0)
                while(available < 1)
                {
                    Thread.Sleep(10);
                    try
                    {
                        isopen = client.Connected;
                        available = isopen ? client.Available : 0;
                        if (!isopen) break;
                    }
                    catch
                    {
                        available = 0; 
                        isopen = false; 
                        break;
                    }
                }
                if (!isopen)
                {
                    this.closeClientInfo(clientinfo, server);
                    return;
                }
            }//while (true)
        }
        /// <summary>
        /// 服务器写入通讯数据
        /// </summary>
        /// <param name="client">客户端连接</param>
        /// <param name="server">服务器</param>
        private void writeData(ClientInfo clientinfo, ServerBase server)
        {
            if (null == clientinfo || null == server)
                return;
            Socket client = clientinfo.Client;
            List<byte[]> responseList = clientinfo.BufferResponse;
            while (true)
            {
                //心跳超时关闭退出
                if (DateTime.Now - clientinfo.OPdt > server.TimeDisconn)
                {
                    this.closeClientInfo(clientinfo, server);
                    DisconnEventArgs args = new DisconnEventArgs(client, server);
                    this.raiseEvent(args);
                    return;
                }
                for (int i = 0; i < 80; i++)
                {
                    if (responseList.Count > 0)
                        break;
                    Thread.Sleep(5);
                }
                if (responseList.Count < 1)
                    continue;
                //同步事件触发发送响应数据
                clientinfo.OPdt = DateTime.Now;
                Monitor.Enter(clientinfo);
                byte[][] responseArray = responseList.ToArray();
                responseList.Clear();
                Monitor.PulseAll(clientinfo);
                Monitor.Exit(clientinfo);
                try
                {
                    for (int i = 0; i < responseArray.Length; i++)
                    {
                        client.Send(responseArray[i]);
                        ResponseSrvEventArgs argresp = new ResponseSrvEventArgs(client, server, responseArray[i]);
                        this.raiseEvent(argresp);
                    }
                }
                catch (Exception ex)
                {
                    this.closeClientInfo(clientinfo, server);
                    DisconnEventArgs args = new DisconnEventArgs(client, server, ex);
                    this.raiseEvent(args);
                    return;
                }
                clientinfo.OPdt = DateTime.Now;
            }
        }
        /// <summary>
        /// 关闭客户端连接并释放缓存信息
        /// </summary>
        /// <param name="clientinfo"></param>
        /// <param name="server"></param>
        private void closeClientInfo(ClientInfo clientinfo, ServerBase server)
        {
            try
            {
                clientinfo.Client.Close();
                Monitor.Enter(clientinfo);
                clientinfo.BufferResponse.Clear();
                Monitor.PulseAll(clientinfo);
                Monitor.Exit(clientinfo);

                Monitor.Enter(server);
                server.ClientList.Remove(clientinfo);
                Monitor.PulseAll(server);
                Monitor.Exit(server);
            }
            catch { }
        }

        /// <summary>
        /// 触发服务端事件:请求/响应/异常
        /// </summary>
        /// <param name="args">要触发的事件</param>
        private void raiseEvent(EventArgs args)
        {
            if (null == args)
                return;
            if (args is RequestEventArgs)
            {
                if (null != this.RequestHandle)
                    ThreadManager.QueueUserWorkItem(delegate(object obj)
                    {
                        this.RequestHandle(this, obj as RequestEventArgs);
                    }, args);
            }
            else if (args is ResponseSrvEventArgs)
            {
                if (null != this.ResponseHandle)
                    ThreadManager.QueueUserWorkItem(delegate(object obj)
                    {
                        this.ResponseHandle(this, obj as ResponseSrvEventArgs);
                    }, args);
            }
            else if (args is ErrorRequestEventArgs)
            {
                if (null != this.ErrorHandle)
                    ThreadManager.QueueUserWorkItem(delegate(object obj)
                    {
                        this.ErrorHandle(this, obj as ErrorRequestEventArgs);
                    }, args);
            }
            else if (args is DisconnEventArgs)
            {
                if (null != this.DisconnHandle)
                    ThreadManager.QueueUserWorkItem(delegate(object obj)
                    {
                        this.DisconnHandle(this, obj as DisconnEventArgs);
                    }, args);
            }
        }
    }
}
