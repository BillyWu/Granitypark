#region �汾˵��

/*
 * �������ݣ�   �����ļ�Э��Ӧ��
 *
 * ��    �ߣ�   ���ٲ�
 *
 * �� �� �ߣ�   ���ٲ�
 *
 * ��    �ڣ�   2010-09-15
 * ժҪ˵����   �ļ�����ָ��,֡ͷ���ļ�˵����֡ͷ<�ļ�����,���ݳ���,��ʶ��,�ļ�·��>����XML��ʽ����������ļ����ݡ�
 *              ��������Ա�ʶ��������
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
    /// Э������,Լ��(a:һ��ͨѶλ��ֻ��һ��Э����ִ��;b:һ��λ�ÿɹ����ͻ����ļ���������;c:һ���ͻ��˿�ͬʱ������ļ�����ָ��)
    ///               d:��ǰʱ�̴��������ֻ��һ���ļ�
    ///         �ļ�ͷЭ���ʽ��1,ʹ��һ����XML��ʽ,����ֵ�������ַ����ּ�.\-����, 2,�������ԣ�id,pathfile,len,cmd,msg
    /// ͨѶʱ��Ĭ��3�볬ʱ,10��ʧ��
    /// </summary>
    public class CmdFileTrans : CommandBase
    {
        #region �ļ�ͷXML��ʽ����

        /// <summary>
        /// �Ϸ��ַ���֤
        /// </summary>
        private static Regex regFilePath = new Regex(@"^[\w\.\\\-]+$", RegexOptions.Compiled);
        /// <summary>
        /// ��֤XML��ʽ
        /// </summary>
        private static Regex regXMLFormat = new Regex(@"^<F(\s+.+?)(/>|></F>)$", RegexOptions.Compiled);
        /// <summary>
        /// �ļ�ͷ��Ϣ��������
        /// </summary>
        private static Regex regAttribute = new Regex("([\\w\\.\\u4E00-\\u9FA5]+)\\s*=\\s*[\"']([\\w\\s\\:\\.\\\\\\u4E00-\\u9FA5-]*)[\"']", RegexOptions.Compiled);
        /// <summary>
        /// �ļ�ͷ��ʼ�ֽ�
        /// </summary>
        private static byte[] fhStart = Encoding.GetEncoding("GB2312").GetBytes("<F");
        /// <summary>
        /// �ļ�ͷ�Է��ʽ��β
        /// </summary>
        private static byte[] fhEndseal = Encoding.GetEncoding("GB2312").GetBytes("/>");
        /// <summary>
        /// �ļ�ͷ�ر�ʽ��β
        /// </summary>
        private static byte[] fhEndclose = Encoding.GetEncoding("GB2312").GetBytes("></F>");
        
        #endregion

        #region Э�����
        
        private static Protocol ptl = new Protocol();
        /// <summary>
        /// ��ȡͨѶЭ��֡ͷ֡β����ֵλ��
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
        /// ����ָ��,Ĭ��1����,����Ͽ�����������
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

        #region ��������

        private byte[] filecontext=new byte[0];
        /// <summary>
        /// ��ȡ��Ӧ�ļ�����
        /// </summary>
        public byte[] FileContext
        {
            get { return filecontext; }
            set { filecontext = value; }
        }
        private NameValueCollection fileheader = new NameValueCollection();
        /// <summary>
        /// ��ȡ��Ӧ�ļ�ͷ��Ϣ
        /// </summary>
        public NameValueCollection FileHeader
        {
            get { return fileheader; }
        }

        #endregion

        #region ���캯��

        /// <summary>
        /// Ĭ�Ϸ�ʽ����ʵ��,����ʼ��ͬ���¼����,��ָ��ID
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
        /// ���캯��ָ������ָ��ID(ָ��ID�ǿɸ��ĵ�)
        /// </summary>
        /// <param name="id">ָ��ID</param>
        public CmdFileTrans(string id) : base(id)
        {
            this.TimeOut = new TimeSpan(0, 0, 3);
            this.TimeFailLimit = new TimeSpan(0, 0, 10);
            this.TimeLimit = new TimeSpan(0, 0, 10);
            this.IsResposeHandle = isResponse;
            this.ResponseHandle += new EventHandler<ResponseEventArgs>(onCommiTransHandle);
        }

        /// <summary>
        /// ���캯��,��ʼ��ͬ���¼����״̬
        /// </summary>
        /// <param name="ewhState">ͬ���¼���ʼ״̬</param>
        public CmdFileTrans(bool ewhState) : base(ewhState)
        {
            this.TimeOut = new TimeSpan(0, 0, 3);
            this.TimeFailLimit = new TimeSpan(0, 0, 10);
            this.TimeLimit = new TimeSpan(0, 0, 10);
            this.IsResposeHandle = isResponse;
            this.ResponseHandle += new EventHandler<ResponseEventArgs>(onCommiTransHandle);
        }

        /// <summary>
        /// ���캯��,��ʼ��ָ��ID,��ʼ��ͬ���¼����״̬
        /// </summary>
        /// <param name="id">ָ��ID,ʹ���пɸ���</param>
        /// <param name="ewhState">ͬ���¼���ʼ״̬</param>
        public CmdFileTrans(string id, bool ewhState) : base(id, ewhState)
        {
            this.TimeOut = new TimeSpan(0, 0, 3);
            this.TimeFailLimit = new TimeSpan(0, 0, 10);
            this.TimeLimit = new TimeSpan(0, 0, 10);
            this.IsResposeHandle = isResponse;
            this.ResponseHandle += new EventHandler<ResponseEventArgs>(onCommiTransHandle);
        }

        #endregion

        #region Э��ָ���

        /// <summary>
        /// �����ļ�,���������
        /// </summary>
        /// <param name="filename">�ļ�����</param>
        /// <param name="path">·��,�����������·��</param>
        /// <param name="fileContext">�ļ��ֽ�����</param>
        public void SendFile(string pathfile, byte[] context)
        {
            //������,�����зǷ��ַ�
            if (!regFilePath.IsMatch(pathfile))
                throw new Exception("�ļ����ƻ�·���Ƿ���");
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
        /// ��ȡ�ļ�
        /// </summary>
        /// <param name="pathfile">·���ļ�</param>
        public void GetFile(string pathfile)
        {
            //������,�����зǷ��ַ�
            if (!regFilePath.IsMatch(pathfile))
                throw new Exception("�ļ����ƻ�·���Ƿ���");
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
        /// ɾ���ļ�
        /// </summary>
        /// <param name="pathfile">·���ļ�</param>
        public void DelFile(string pathfile)
        {
            //������,�����зǷ��ַ�
            if (!regFilePath.IsMatch(pathfile))
                throw new Exception("�ļ����ƻ�·���Ƿ���");
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
        /// ��չ������,����ͬ���豸���Ʋ���
        /// </summary>
        /// <param name="service">��չ������</param>
        /// <param name="msg">������Ϣ,���ݷ�����(���ܰ�����id,cmd,len,service)</param>
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
            //������չ
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
        /// ��Ŀ��ͨѶ��������,�Ѿ����򲻻��ظ�
        /// </summary>
        /// <param name="mgr">ͨѶ������</param>
        /// <param name="target">ͨѶĿ��</param>
        public static void OpenHeaderBeat(CommiManager mgr, CommiTarget target)
        {
            if (null == mgr || null == target)
                return;
            target.setProtocol(CmdFileTrans.PTL);
            mgr.SendCommand(target, CmdHeaderBeat);
        }
        /// <summary>
        /// ��Ŀ��ͨѶ�ر�����
        /// </summary>
        /// <param name="mgr">ͨѶ������</param>
        /// <param name="target">ͨѶĿ��</param>
        public static void CloseHeaderBeat(CommiManager mgr, CommiTarget target)
        {
            if (null == mgr || null == target)
                return;
            mgr.RemoveCommand(target, CmdHeaderBeat);
        }

        /// <summary>
        /// ����ͨѶָ��
        ///     ��������: len,dir=request/response,cmd=TransFile.trans,source,target,
        ///               CommiType,addr=ipaddr/COM1,port,baudRate,parity,dataBits,stopBits
        /// </summary>
        /// <param name="src">��ǰָ������IP��ַ</param>
        /// <param name="proxy">ִ��Ŀ��Ĵ���IP��ַ</param>
        /// <param name="dest">����Ŀ��λ��</param>
        /// <param name="dtpl">Ŀ��Э����������</param>
        /// <param name="dcmd">Ŀ��ָ������</param>
        /// <param name="data">���͵�ָ���ֽ�,����ִ��ʱ������Ӧ�ֽ�,�Ǳ���ִ���򷵻�0�����ֽ�</param>
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
            //����ͨѶ��Ŀ���Ǵ����������ֱ��ͨѶ
            if (islocal || proxy.ToString() == info["addr"])
            {
                info["dir"] = "response";
                info["source"] = info["target"];
                CmdProtocol cmd = new CmdProtocol(false);
                cmd.setCommand(dtpl, dcmd, "");
                cmd.setCommand(data);
                CommiManager.GlobalManager.SendCommand(dest, cmd);
                if (!cmd.EventWh.WaitOne(800, false))
                    info["msg"] = "Ŀ��ͨѶ��ʱʧ��";
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
        /// ���մ���ͨѶ
        ///     ��������: len,dir=request/response,cmd=TransFile.trans,source,target,
        ///               CommiType,addr=ipaddr/COM1,port,baudRate,parity,dataBits,stopBits
        /// </summary>
        /// <param name="sender">�����¼���ʵ��</param>
        /// <param name="e">��Ӧ����</param>
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
            //����ַ
            try
            {
                addr = addr.ToUpper();
                if (!addr.StartsWith("COM"))
                    commitype = (CommiType)Enum.Parse(typeof(CommiType), ps["CommiType"], true);
                if (!string.IsNullOrEmpty(ps["port"]))
                    port = Convert.ToInt32(ps["port"]);
            }
            catch { return; }
            //���ͨѶ����
            if (CommiType.SerialPort != commitype && port < 0)
                return;
            if (CommiType.SerialPort == commitype &&
                (string.IsNullOrEmpty(ps["baudRate"]) || string.IsNullOrEmpty(ps["parity"])
                    || string.IsNullOrEmpty(ps["dataBits"]) || string.IsNullOrEmpty(ps["stopBits"])))
                return;
            //����ͨѶĿ��
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
                case "ͣ����":
                    target.setProtocol(Protocol.PTLPark); break;
                case "�Ž�":
                    target.setProtocol(Protocol.PTLDoor); break;
                case "����":
                    target.setProtocol(Protocol.PTLEatery); break;
            }
            //ִ�д��ݵ�ָ��
            CmdProtocol cmd = new CmdProtocol(false);
            cmd.setCommand(data);
            ps["dir"] = "response";
            CommiManager.GlobalManager.SendCommand(target, cmd);
            if (!cmd.EventWh.WaitOne(800, false))
                ps["msg"] = "Ŀ��ͨѶ��ʱʧ��";
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

        #region ������Ӧ����ָ����Ӧ�¼�

        /// <summary>
        /// �����·��ת������·��
        /// </summary>
        /// <param name="pathfile">����ָ���е����·��</param>
        /// <returns>����·��</returns>
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
        /// ����Ӧ�������Э��ֳ�������,����ʣ�಻������
        /// </summary>
        /// <param name="buffer">�����ֽ�</param>
        /// <param name="destlist">�������б�</param>
        /// <returns>����ʣ�಻������</returns>
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
                    //��ȡ�����ֽ�
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
                //�������ֽڷ���
                if (index + hlen + flen > buffer.Length)
                    break;
                //�����ֽڲ������б�
                if (flen > 0)
                {
                    Array.Resize<byte>(ref header, header.Length + flen);
                    Array.Copy(buffer, index + hlen, header, hlen, flen);
                }
                destlist.Add(header);
                index = index + hlen + flen;
                header = SvrFileTrans.GetFileheader(buffer, index);
            }
            //���кܳ��ֽ�ʱ��Ȼû������ͷ��Ϣ����Ϊ�ֽ���Ч
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
        /// �Ƿ�ǰָ��Ľ��,��֤�豸ID��ָ��
        /// </summary>
        /// <param name="response">��Ӧ�ֽ�</param>
        /// <returns>�Ƿ���ָ�����Ӧ</returns>
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
        /// ���ش����¼�,������Ӧ��ʽ
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
            //����ļ�ͷ��Ϣ
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
