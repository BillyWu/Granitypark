#region 版本说明

/*
 * 功能内容：   通讯响应参数
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-05-22
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Granity.communications
{
    /// <summary>
    /// 通讯响应参数。
    /// </summary>
    public class ResponseEventArgs : EventArgs
    {
        /// <summary>
        /// 响应数据的读取流
        /// </summary>
        public byte[] Response;

        /// <summary>
        /// 是否成功响应
        /// </summary>
        public bool Success=true;

        /// <summary>
        /// 响应数据长度
        /// </summary>
        public int ContentLen = 0;

        /// <summary>
        /// 通讯Client：Socket/UdpClient/SerialPort
        /// </summary>
        public object Client;

        /// <summary>
        /// 通讯地址参数
        /// </summary>
        public CommiTarget Target;

        /// <summary>
        /// 来源指令
        /// </summary>
        public CommandBase[] Commands;

        /// <summary>
        /// 当前指令,事件无法确定当前指令时为空
        /// </summary>
        public CommandBase CurrentCommand;

        /// <summary>
        /// 构造函数,默认成功,响应和指令默认长度0
        /// </summary>
        public ResponseEventArgs()
        {
            this.Response = new byte[0];
            this.Commands = new CommandBase[0];
        }

        /// <summary>
        /// 构造函数,默认成功
        /// </summary>
        /// <param name="client">通讯的Socket/UdpClient/SerailPort</param>
        /// <param name="param">通讯参数,IP地址或串口参数</param>
        /// <param name="cmds">当前进行通讯的指令列表</param>
        /// <param name="response">响应结果</param>
        public ResponseEventArgs(object client, CommiTarget param, CommandBase[] cmds, byte[] response)
        {
            this.Client = client;
            this.Commands = cmds;
            this.Response = response;
            this.ContentLen = response.Length;
            this.Target = param;
        }
        /// <summary>
        /// 构造函数,默认成功
        /// </summary>
        /// <param name="client">通讯的Socket/UdpClient/SerailPort</param>
        /// <param name="cmds">当前进行通讯的指令列表</param>
        /// <param name="response">响应结果</param>
        /// <param name="cmd">当前指令</param>
        /// <param name="success">响应是否成功</param>
        public ResponseEventArgs(object client, CommiTarget param, CommandBase[] cmds, byte[] response, CommandBase cmd, bool success)
        {
            this.Client = client;
            this.Commands = cmds;
            this.Response = response;
            this.ContentLen = null == response ? 0 : response.Length;
            this.CurrentCommand = cmd;
            this.Success = success;
            this.Target = param;
        }
    }

    /// <summary>
    /// 通讯错误参数。
    /// </summary>
    public class ErrorCommiEventArgs : EventArgs
    {
        /// <summary>
        /// 通讯Client：Socket/UdpClient/SerialPort
        /// </summary>
        public object Client;

        /// <summary>
        /// 通讯地址参数
        /// </summary>
        public CommiTarget Target;

        /// <summary>
        /// 当前指令,事件无法确定当前指令时为空
        /// </summary>
        public CommandBase CurrentCommand;
        private Exception ex;
        /// <summary>
        /// 读取当前触发的异常
        /// </summary>
        public Exception Exception
        {
            get { return this.ex; }
        }

        /// <summary>
        /// 构造函数,默认成功,响应和指令默认长度0
        /// </summary>
        public ErrorCommiEventArgs(Exception ex)
        {
            this.ex = ex;
        }

        /// <summary>
        /// 构造函数,默认成功
        /// </summary>
        /// <param name="client">通讯的Socket/UdpClient/SerailPort</param>
        /// <param name="param">通讯参数,IP地址或串口参数</param>
        /// <param name="cmds">当前进行通讯的指令列表</param>
        /// <param name="response">响应结果</param>
        public ErrorCommiEventArgs(object client, CommiTarget param, CommandBase cmd, Exception ex)
        {
            this.Client = client;
            this.CurrentCommand = cmd;
            this.Target = param;
            this.ex = ex;
        }

    }

    /// <summary>
    /// 请求事件参数
    /// </summary>
    public class RequestEventArgs : EventArgs
    {
        /// <summary>
        /// 请求数据
        /// </summary>
        public byte[] Request;
        /// <summary>
        /// 请求的客户端
        /// </summary>
        public Socket Client;
        /// <summary>
        /// 请求内容长度
        /// </summary>
        public long ContentLen;
        /// <summary>
        /// 服务实例
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// 返回值,默认0
        /// </summary>
        public int Return = 0;
        /// <summary>
        /// 请求事件参数构造函数
        /// </summary>
        /// <param name="client">发出请求的客户端</param>
        /// <param name="server">服务器服务实例</param>
        /// <param name="request">请求数据内容</param>
        public RequestEventArgs(Socket client, ServerBase server, byte[] request)
        {
            Client = client;
            Server = server;
            Request = request;
            if (null != request)
                ContentLen = request.Length;
        }
 
    }

    /// <summary>
    /// 服务端响应事件参数
    /// </summary>
    public class ResponseSrvEventArgs : EventArgs
    {
        /// <summary>
        /// 请求数据
        /// </summary>
        public byte[] Response;
        /// <summary>
        /// 请求的客户端
        /// </summary>
        public Socket Client;
        /// <summary>
        /// 请求内容长度
        /// </summary>
        public long ContentLen;
        /// <summary>
        /// 服务实例
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// 服务端响应事件参数
        /// </summary>
        /// <param name="client">发出请求的客户端</param>
        /// <param name="server">服务器实例</param>
        /// <param name="response">响应数据</param>
        public ResponseSrvEventArgs(Socket client, ServerBase server, byte[] response)
        {
            Client = client;
            Server = server;
            Response = response;
            if (null != response)
                ContentLen = response.Length;
        }
    }

    /// <summary>
    /// 服务器端异常事件参数
    /// </summary>
    public class ErrorRequestEventArgs : EventArgs
    {
        /// <summary>
        /// 请求数据
        /// </summary>
        public byte[] Request;
        /// <summary>
        /// 请求的客户端
        /// </summary>
        public Socket Client;
        /// <summary>
        /// 请求内容长度
        /// </summary>
        public long ContentLen;
        /// <summary>
        /// 服务实例
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// 服务端异常
        /// </summary>
        public Exception Exception;
        /// <summary>
        /// 服务器端异常事件参数
        /// </summary>
        /// <param name="client">发出请求的客户端</param>
        /// <param name="server">服务器服务实例</param>
        /// <param name="request">请求数据内容</param>
        /// <param name="ex">服务端异常</param>
        public ErrorRequestEventArgs(Socket client, ServerBase server, byte[] request,Exception ex)
        {
            Client = client;
            Server = server;
            Request = request;
            if (null != request)
                ContentLen = request.Length;
            Exception = ex;
        }
    }

    /// <summary>
    /// 断开连接事件参数
    /// </summary>
    public class DisconnEventArgs : EventArgs
    {
        /// <summary>
        /// 请求的客户端
        /// </summary>
        public Socket Client;
        /// <summary>
        /// 服务实例
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// 服务端异常
        /// </summary>
        public Exception Exception;
        /// <summary>
        /// 请求事件参数构造函数
        /// </summary>
        /// <param name="client">发出请求的客户端</param>
        /// <param name="server">服务器服务实例</param>
        public DisconnEventArgs(Socket client, ServerBase server)
        {
            Client = client;
            Server = server;
        }
        /// <summary>
        /// 请求事件参数构造函数
        /// </summary>
        /// <param name="client">发出请求的客户端</param>
        /// <param name="server">服务器服务实例</param>
        /// <param name="ex">异常事件</param>
        public DisconnEventArgs(Socket client, ServerBase server,Exception ex)
        {
            Client = client;
            Server = server;
            Exception = ex;
        }

    }
    /// <summary>
    /// 扩展事件参数
    /// </summary>
    public class ExtendEventArgs : EventArgs
    {
        /// <summary>
        /// 请求数据
        /// </summary>
        public byte[] Request;
        /// <summary>
        /// 请求内容长度
        /// </summary>
        public long ContentLen;
        /// <summary>
        /// 服务实例
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// 请求客户端实例，可对实例追加响应数据
        /// </summary>
        public ClientInfo Client;
        /// <summary>
        /// 请求事件参数构造函数
        /// </summary>
        /// <param name="server">服务器服务实例</param>
        /// <param name="client">客户端实例</param>
        /// <param name="request">请求数据内容</param>
        public ExtendEventArgs(ServerBase server, ClientInfo client, byte[] request)
        {
            Server = server;
            Client = client;
            Request = request;
            if (null != request)
                ContentLen = request.Length;
        }

    }


}
