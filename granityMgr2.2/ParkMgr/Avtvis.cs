using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
namespace Granity.granityMgr.ParkMgr
{
    class Avtvis
    {
        [DllImport("User32.dll", EntryPoint = "SetParent")]
        private static extern int SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", EntryPoint = "ShowWindow")]
        private static extern int ShowWindow(Int32 hwnd, Int32 nCmdShow);
        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hwnd, Int32 hWndInsertAfter, Int32 x, Int32 y, Int32 cx, Int32 cy, Int32 uFlags);
        [DllImport("user32.dll", EntryPoint = "SetWindowLong")]
        private static extern int SetWindowLong(Int32 hWnd, Int32 nlndex, Int32 dwNewLong);
        [DllImport("Kernel32.dll", EntryPoint = "GetLastError")]
        private static extern bool GetLastError();

        /// <summary>
        /// ǰ��ʵ�������¼�¼������������Ϣ�����ƺ��룬ʱ�䣬��ɫ�ȣ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="str"></param>
        public delegate void NewRecord(object sender, string str);
        /// <summary>
        /// ʶ����¼�¼�����س�����Ϣ
        /// </summary>
        public event NewRecord NewRecordEnvetHandle;
        public delegate void OnIncoming(object sender, bool success);
        /// <summary>
        /// ʵ����Ӧ�ó�������Ϣ���������¼�;���¼�����֪ͨ�ͻ�����Ƶ״̬ʵʱ�ĸı�
        /// </summary>
        public event OnIncoming OnIncomingEnvetHandle;

        public const Int32 SWP_FRAMECHANGED = 0x20;
        public const Int32 SWP_HIDEWINDOW = 0x80;
        public const Int32 SWP_NOACTIVATE = 0x10;
        public const Int32 SWP_NOCOPYBITS = 0x100;
        public const Int32 SWP_NOMOVE = 0x2;
        public const Int32 SWP_NOOWNERZORDER = 0x200;
        public const Int32 SWP_NOREDRAW = 0x8;
        public const Int32 SWP_NOSIZE = 0x1;
        public const Int32 SWP_NOZORDER = 0x4;
        public const Int32 SWP_SHOWWINDOW = 0x40;
        public const Int32 HWND_BOTTOM = 1;
        public const Int32 HWND_TOP = 0;
        public const Int32 GWL_STYLE = -16;
        public const Int32 WS_CHILD = 0x40000000;
        public const Int32 lFrontHeight = 700;
        string[] NewHandle ={ "0", "0" };
        string[] OldHandle ={ "0", "0" };
        /// <summary>
        /// ��������AVTʵ��ID
        /// </summary>
        private string AvtvisId;
        public string avtvisId
        {
            get { return AvtvisId; }
            set { AvtvisId = value; }
        }

        /// <summary>
        ///  ��ʾͼ�������
        /// </summary>
        private Panel[] Pnl;
        public Panel[] pnl
        {
            get { return Pnl; }
            set { Pnl = value; }
        }

        /// <summary>
        /// �������
        /// </summary>
        private int ConChannel;

        public int conChannel
        {
            get { return ConChannel; }
            set { ConChannel = value; }
        }

        private AxPRJ_CLIENTLib.AxPrj_Client Client;
        /// <summary>
        /// ʵ����Client
        /// </summary>
        /// <param name="OcxClient"></param>
        public Avtvis(AxPRJ_CLIENTLib.AxPrj_Client OcxClient, string instanceId)
        {
            Client = OcxClient;
            AvtvisId = instanceId;
            int i = -1;
            //foreach (Panel pnl in this.pnl)
            //{ 
            //    i++;
            //    NewHandle[i] = pnl.Handle;
            //    OldHandle[i] = pnl.Handle;
            //}
            //    Client.OnDSUnavailable += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDSUnavailableEventHandler(Client_OnDSUnavailable);
            //    Client.OnDSAvailable += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDSAvailableEventHandler(Client_OnDSAvailable);
            //    Client.OnFrontReconnected += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnFrontReconnectedEventHandler(Client_OnFrontReconnected);
            //    Client.OnReconnected += new EventHandler(Client_OnReconnected);
            //    Client.OnDatabaseReconnected += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDatabaseReconnectedEventHandler(Client_OnDatabaseReconnected);
            //    Client.OnDatabaseDisconnected += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDatabaseDisconnectedEventHandler(Client_OnDatabaseDisconnected);
            //    Client.OnFrontDisconn += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnFrontDisconnEventHandler(Client_OnFrontDisconn);
            //    Client.OnDisconn += new EventHandler(Client_OnDisconn);
            //    Client.OnErrMsg += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnErrMsgEventHandler(axPrj_Client1_OnErrMsg);
            //    Client.OnInvalidMsg += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnInvalidMsgEventHandler(axPrj_Client1_OnInvalidMsg);
            //    Client.OnIncomingMsg += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnIncomingMsgEventHandler(axPrj_Client1_OnIncomingMsg);
            //    Client.OnNewRecord += new AxPRJ_CLIENTLib._DPrj_ClientEvents_OnNewRecordEventHandler(Client_OnNewRecord);
        }

        #region AVT-VIS ������
        /// <summary>
        /// ʶ����¼�¼�����س�����Ϣ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnNewRecord(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnNewRecordEvent e)
        {
            #region ��ʱע�� ��Ʒ�ʽ�仯 �� 2011-4-15
            //string[] msg = e.isz_Msg.Split('|');
            //string frontid = "ǰ��ʵ��ID��" + msg[1];
            //string transDT = "����ʱ�䣺" + msg[2];
            //string millisecond = "����ʱ����벿�֣�" + msg[3];
            //string laneNo = "������" + msg[4];
            //string cam = "�����ID��" + msg[5];
            //string lpn = "���ƺ��룺" + msg[6];
            //string color = "��ɫ���룺" + msg[7];
            //string filename = "ͼƬ�ļ����ƣ�" + msg[8];
            //string forcetosavelist = "ϵͳǿ�Ʊ����ͼƬ���ƣ�" + msg[9];
            //string binarizedbuffer = "�������кţ�" + msg[10];
            #endregion

            int index = 0;
            string tag = string.Empty;
            string[] msg = e.isz_Msg.Split('|');
            string[] parmMsg ={"ǰ��ʵ��ID", "����ʱ��", "����ʱ����벿��", "����", "�����ID", "���ƺ���", "��ɫ����"
            ,"ͼƬ�ļ�����","ϵͳǿ�Ʊ����ͼƬ����","�������к�"};
            //ע�� msg�Ǵ�����=1 ��ʼ����ΪAVT����msg[0]�Ǳ�ʾ��Ϣ���͵��ֶΣ��ͻ��˲���Ҫ���ֶ�ֵ
            foreach (string str in parmMsg)
            {
                index++;
                tag = basefun.setvaltag(tag, "{" + str + "}", msg[index]);
            }

            if (NewRecordEnvetHandle != null)
                NewRecordEnvetHandle(this, tag);
        }

        /// <summary>
        /// �ͻ�����ǰ��ϵͳ��������ͨ������һ����Ч���������Ӧ�ó����͵�ǰ�Σ��������¼���ע�⣺�ͻ���ֻ������һ��ʵ���Ŵ��������ʵ�����ᴥ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDSUnavailable(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDSUnavailableEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// �ͻ�����ǰ��ϵͳ��������ͨ������һ����Ч���������Ӧ�ó����͵�ǰ�Σ��������¼���ע�⣺�ͻ���ֻ������һ��ʵ���Ŵ��������ʵ�����ᴥ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDSAvailable(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDSAvailableEvent e)
        {
            string str = e.isz_VisId;
            // MessageBox.Show("��Ч��������" + str, "ϵͳ��ʾ��");
        }

        /// <summary>
        /// ǰ��ʵ�����������������ϣ��������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnFrontReconnected(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnFrontReconnectedEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        ///  Ӧ�ó������������������ϣ��������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnReconnected(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// �����������ݿ��������ϣ��������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDatabaseReconnected(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDatabaseReconnectedEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// �����Ÿ����ݿ�Ͽ����������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDatabaseDisconnected(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDatabaseDisconnectedEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// ǰ��ʵ���������ŶϿ����������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnFrontDisconn(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnFrontDisconnEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// �����������жϣ��������¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDisconn(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// ʵ����Ӧ�ó�������Ϣ���������¼�;���¼�����֪ͨ�ͻ�����Ƶ״̬ʵʱ�ĸı�
        /// </summary>
        /// <param name="isz_Msg"></param>
        void axPrj_Client1_OnIncomingMsg(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnIncomingMsgEvent e)
        {
            if (string.IsNullOrEmpty(e.isz_Msg))
                return;
            Int32 oldHandle = 0;
            string[] info = e.isz_Msg.Split('|');
            if (info[1] != AvtvisId.Trim())
                return;
            if (info[0] == "RLLVA")
                return;
            else if (info[0] == "RQLVA")
            {
                NewHandle[Convert.ToInt32(info[2])] = info[3];
                OldHandle[Convert.ToInt32(info[2])] = info[4];
                try
                {
                    oldHandle = SetParent((IntPtr)(Convert.ToInt32(info[3])), Pnl[Convert.ToInt32(info[2])].Handle);
                }
                catch
                { }
                if (oldHandle == 0)
                    return;
                else
                    SetWindowPos((IntPtr)Convert.ToInt32(info[3]), HWND_TOP, 0, 0, 0, 0, SWP_NOSIZE);
                int fag = Client.Send("[F|LHU|" + info[2] + "]");
                if (fag == 0)
                {
                    //�����ͻ����¼�
                    if (OnIncomingEnvetHandle != null)
                    {
                        OnIncomingEnvetHandle(this, true);
                    }
                }
            }
        }

        /// <summary>
        /// ������δ�յ���Ϣ�����ܽ������ݣ�һ���ǿͻ��������ݸ�ʽ���󣩣��������¼�֪ͨӦ�ó���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void axPrj_Client1_OnInvalidMsg(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnInvalidMsgEvent e)
        {
            // XtraMessageBox.Show("Invalid Message: " + e.isz_Msg, "ϵͳ��ʾ��");
        }

        /// <summary>
        /// ��ǰ��ϵͳ���ִ���ʱ,ǰ��֪ͨӦ�ó���ϵͳ��������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void axPrj_Client1_OnErrMsg(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnErrMsgEvent e)
        {
            //   MessageBox.Show(e.isz_Msg);
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="ip">�����������豸IP��ַ</param>
        /// <param name="port">�˿�</param>
        /// <param name="instance">ǰ��ʵ��</param>
        public bool ConnVis(string ip, int port, string instance)
        {
            bool success = false;
            int state = Client.ConnSrv(ip, 1222, AvtvisId);
            if (state == 0)
                success = true;
            return success;
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="channel">�������</param>
        /// <returns></returns>
        public bool Video(Panel pnl)
        {
            bool success = false;
            string sMsg = "[F|RQLV|" + AvtvisId + "|" + ConChannel + "]";
            int fag = Client.Send(sMsg);
            if (fag == 0)
            {
                this.Pnl[ConChannel] = pnl;
                success = true;
            }
            return success;
        }

        /// <summary>
        /// ȡ������
        /// </summary>
        /// <param name="channel">�������</param>
        /// <returns></returns>
        public bool CancelVideo()
        {
            if (OldHandle.Length == 0 || NewHandle.Length == 0 || this.pnl == null)
                return false;
            bool success = false;
            for (int i = 0; i < this.pnl.Length; i++)
            {
                SetWindowPos((IntPtr)Convert.ToInt32(NewHandle[i]), HWND_BOTTOM, 0, lFrontHeight, 0, 0, SWP_NOSIZE);
                SetParent((IntPtr)(Convert.ToInt32(NewHandle[i])), (IntPtr)Convert.ToInt32(OldHandle[i]));
                string sMsg = "[F|RLLV|" + AvtvisId + "|" + i + "]";
                //û�о��ʱ��������Ϊ��
                NewHandle[i] = "0";
                OldHandle[i] = "0";
                int fag = Client.Send(sMsg);
                if (fag == 0)
                    success = true;
            }
            return success;
        }
        #endregion
    }
}
