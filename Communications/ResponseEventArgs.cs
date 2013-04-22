#region �汾˵��

/*
 * �������ݣ�   ͨѶ��Ӧ����
 *
 * ��    �ߣ�   ���ٲ�
 *
 * �� �� �ߣ�   ���ٲ�
 *
 * ��    �ڣ�   2010-05-22
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;

namespace Granity.communications
{
    /// <summary>
    /// ͨѶ��Ӧ������
    /// </summary>
    public class ResponseEventArgs : EventArgs
    {
        /// <summary>
        /// ��Ӧ���ݵĶ�ȡ��
        /// </summary>
        public byte[] Response;

        /// <summary>
        /// �Ƿ�ɹ���Ӧ
        /// </summary>
        public bool Success=true;

        /// <summary>
        /// ��Ӧ���ݳ���
        /// </summary>
        public int ContentLen = 0;

        /// <summary>
        /// ͨѶClient��Socket/UdpClient/SerialPort
        /// </summary>
        public object Client;

        /// <summary>
        /// ͨѶ��ַ����
        /// </summary>
        public CommiTarget Target;

        /// <summary>
        /// ��Դָ��
        /// </summary>
        public CommandBase[] Commands;

        /// <summary>
        /// ��ǰָ��,�¼��޷�ȷ����ǰָ��ʱΪ��
        /// </summary>
        public CommandBase CurrentCommand;

        /// <summary>
        /// ���캯��,Ĭ�ϳɹ�,��Ӧ��ָ��Ĭ�ϳ���0
        /// </summary>
        public ResponseEventArgs()
        {
            this.Response = new byte[0];
            this.Commands = new CommandBase[0];
        }

        /// <summary>
        /// ���캯��,Ĭ�ϳɹ�
        /// </summary>
        /// <param name="client">ͨѶ��Socket/UdpClient/SerailPort</param>
        /// <param name="param">ͨѶ����,IP��ַ�򴮿ڲ���</param>
        /// <param name="cmds">��ǰ����ͨѶ��ָ���б�</param>
        /// <param name="response">��Ӧ���</param>
        public ResponseEventArgs(object client, CommiTarget param, CommandBase[] cmds, byte[] response)
        {
            this.Client = client;
            this.Commands = cmds;
            this.Response = response;
            this.ContentLen = response.Length;
            this.Target = param;
        }
        /// <summary>
        /// ���캯��,Ĭ�ϳɹ�
        /// </summary>
        /// <param name="client">ͨѶ��Socket/UdpClient/SerailPort</param>
        /// <param name="cmds">��ǰ����ͨѶ��ָ���б�</param>
        /// <param name="response">��Ӧ���</param>
        /// <param name="cmd">��ǰָ��</param>
        /// <param name="success">��Ӧ�Ƿ�ɹ�</param>
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
    /// ͨѶ���������
    /// </summary>
    public class ErrorCommiEventArgs : EventArgs
    {
        /// <summary>
        /// ͨѶClient��Socket/UdpClient/SerialPort
        /// </summary>
        public object Client;

        /// <summary>
        /// ͨѶ��ַ����
        /// </summary>
        public CommiTarget Target;

        /// <summary>
        /// ��ǰָ��,�¼��޷�ȷ����ǰָ��ʱΪ��
        /// </summary>
        public CommandBase CurrentCommand;
        private Exception ex;
        /// <summary>
        /// ��ȡ��ǰ�������쳣
        /// </summary>
        public Exception Exception
        {
            get { return this.ex; }
        }

        /// <summary>
        /// ���캯��,Ĭ�ϳɹ�,��Ӧ��ָ��Ĭ�ϳ���0
        /// </summary>
        public ErrorCommiEventArgs(Exception ex)
        {
            this.ex = ex;
        }

        /// <summary>
        /// ���캯��,Ĭ�ϳɹ�
        /// </summary>
        /// <param name="client">ͨѶ��Socket/UdpClient/SerailPort</param>
        /// <param name="param">ͨѶ����,IP��ַ�򴮿ڲ���</param>
        /// <param name="cmds">��ǰ����ͨѶ��ָ���б�</param>
        /// <param name="response">��Ӧ���</param>
        public ErrorCommiEventArgs(object client, CommiTarget param, CommandBase cmd, Exception ex)
        {
            this.Client = client;
            this.CurrentCommand = cmd;
            this.Target = param;
            this.ex = ex;
        }

    }

    /// <summary>
    /// �����¼�����
    /// </summary>
    public class RequestEventArgs : EventArgs
    {
        /// <summary>
        /// ��������
        /// </summary>
        public byte[] Request;
        /// <summary>
        /// ����Ŀͻ���
        /// </summary>
        public Socket Client;
        /// <summary>
        /// �������ݳ���
        /// </summary>
        public long ContentLen;
        /// <summary>
        /// ����ʵ��
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// ����ֵ,Ĭ��0
        /// </summary>
        public int Return = 0;
        /// <summary>
        /// �����¼��������캯��
        /// </summary>
        /// <param name="client">��������Ŀͻ���</param>
        /// <param name="server">����������ʵ��</param>
        /// <param name="request">������������</param>
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
    /// �������Ӧ�¼�����
    /// </summary>
    public class ResponseSrvEventArgs : EventArgs
    {
        /// <summary>
        /// ��������
        /// </summary>
        public byte[] Response;
        /// <summary>
        /// ����Ŀͻ���
        /// </summary>
        public Socket Client;
        /// <summary>
        /// �������ݳ���
        /// </summary>
        public long ContentLen;
        /// <summary>
        /// ����ʵ��
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// �������Ӧ�¼�����
        /// </summary>
        /// <param name="client">��������Ŀͻ���</param>
        /// <param name="server">������ʵ��</param>
        /// <param name="response">��Ӧ����</param>
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
    /// ���������쳣�¼�����
    /// </summary>
    public class ErrorRequestEventArgs : EventArgs
    {
        /// <summary>
        /// ��������
        /// </summary>
        public byte[] Request;
        /// <summary>
        /// ����Ŀͻ���
        /// </summary>
        public Socket Client;
        /// <summary>
        /// �������ݳ���
        /// </summary>
        public long ContentLen;
        /// <summary>
        /// ����ʵ��
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// ������쳣
        /// </summary>
        public Exception Exception;
        /// <summary>
        /// ���������쳣�¼�����
        /// </summary>
        /// <param name="client">��������Ŀͻ���</param>
        /// <param name="server">����������ʵ��</param>
        /// <param name="request">������������</param>
        /// <param name="ex">������쳣</param>
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
    /// �Ͽ������¼�����
    /// </summary>
    public class DisconnEventArgs : EventArgs
    {
        /// <summary>
        /// ����Ŀͻ���
        /// </summary>
        public Socket Client;
        /// <summary>
        /// ����ʵ��
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// ������쳣
        /// </summary>
        public Exception Exception;
        /// <summary>
        /// �����¼��������캯��
        /// </summary>
        /// <param name="client">��������Ŀͻ���</param>
        /// <param name="server">����������ʵ��</param>
        public DisconnEventArgs(Socket client, ServerBase server)
        {
            Client = client;
            Server = server;
        }
        /// <summary>
        /// �����¼��������캯��
        /// </summary>
        /// <param name="client">��������Ŀͻ���</param>
        /// <param name="server">����������ʵ��</param>
        /// <param name="ex">�쳣�¼�</param>
        public DisconnEventArgs(Socket client, ServerBase server,Exception ex)
        {
            Client = client;
            Server = server;
            Exception = ex;
        }

    }
    /// <summary>
    /// ��չ�¼�����
    /// </summary>
    public class ExtendEventArgs : EventArgs
    {
        /// <summary>
        /// ��������
        /// </summary>
        public byte[] Request;
        /// <summary>
        /// �������ݳ���
        /// </summary>
        public long ContentLen;
        /// <summary>
        /// ����ʵ��
        /// </summary>
        public ServerBase Server;
        /// <summary>
        /// ����ͻ���ʵ�����ɶ�ʵ��׷����Ӧ����
        /// </summary>
        public ClientInfo Client;
        /// <summary>
        /// �����¼��������캯��
        /// </summary>
        /// <param name="server">����������ʵ��</param>
        /// <param name="client">�ͻ���ʵ��</param>
        /// <param name="request">������������</param>
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
