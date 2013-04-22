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
        /// 前段实例返回新纪录（包含车牌信息，车牌号码，时间，颜色等）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="str"></param>
        public delegate void NewRecord(object sender, string str);
        /// <summary>
        /// 识别出新纪录，返回车牌信息
        /// </summary>
        public event NewRecord NewRecordEnvetHandle;
        public delegate void OnIncoming(object sender, bool success);
        /// <summary>
        /// 实例给应用程序发送信息，产生该事件;该事件用于通知客户端视频状态实时的改变
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
        /// 单独定义AVT实例ID
        /// </summary>
        private string AvtvisId;
        public string avtvisId
        {
            get { return AvtvisId; }
            set { AvtvisId = value; }
        }

        /// <summary>
        ///  显示图像的容器
        /// </summary>
        private Panel[] Pnl;
        public Panel[] pnl
        {
            get { return Pnl; }
            set { Pnl = value; }
        }

        /// <summary>
        /// 摄像机号
        /// </summary>
        private int ConChannel;

        public int conChannel
        {
            get { return ConChannel; }
            set { ConChannel = value; }
        }

        private AxPRJ_CLIENTLib.AxPrj_Client Client;
        /// <summary>
        /// 实例化Client
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

        #region AVT-VIS 处理方法
        /// <summary>
        /// 识别出新纪录，返回车牌信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnNewRecord(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnNewRecordEvent e)
        {
            #region 暂时注销 设计方式变化 汪 2011-4-15
            //string[] msg = e.isz_Msg.Split('|');
            //string frontid = "前段实例ID：" + msg[1];
            //string transDT = "处理时间：" + msg[2];
            //string millisecond = "处理时间毫秒部分：" + msg[3];
            //string laneNo = "车道：" + msg[4];
            //string cam = "摄像机ID：" + msg[5];
            //string lpn = "车牌号码：" + msg[6];
            //string color = "颜色代码：" + msg[7];
            //string filename = "图片文件名称：" + msg[8];
            //string forcetosavelist = "系统强制保存的图片名称：" + msg[9];
            //string binarizedbuffer = "参数序列号：" + msg[10];
            #endregion

            int index = 0;
            string tag = string.Empty;
            string[] msg = e.isz_Msg.Split('|');
            string[] parmMsg ={"前段实例ID", "处理时间", "处理时间毫秒部分", "车道", "摄像机ID", "车牌号码", "颜色代码"
            ,"图片文件名称","系统强制保存的图片名称","参数序列号"};
            //注意 msg是从索引=1 开始，因为AVT返回msg[0]是标示消息类型的字段，客户端不需要该字段值
            foreach (string str in parmMsg)
            {
                index++;
                tag = basefun.setvaltag(tag, "{" + str + "}", msg[index]);
            }

            if (NewRecordEnvetHandle != null)
                NewRecordEnvetHandle(this, tag);
        }

        /// <summary>
        /// 客户端与前段系统建立了软通道，当一个无效触发命令从应用程序发送到前段，产生该事件（注意：客户端只能连接一个实例才触发，多个实例不会触发）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDSUnavailable(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDSUnavailableEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 客户端与前段系统建立了软通道，当一个有效触发命令从应用程序发送到前段，产生该事件（注意：客户端只能连接一个实例才触发，多个实例不会触发）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDSAvailable(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDSAvailableEvent e)
        {
            string str = e.isz_VisId;
            // MessageBox.Show("有效触发命令" + str, "系统提示！");
        }

        /// <summary>
        /// 前段实例与数据桥重新连上，产生该事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnFrontReconnected(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnFrontReconnectedEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        ///  应用程序与数据桥重新连上，产生该事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnReconnected(object sender, EventArgs e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 数据桥与数据库重新连上，产生该事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDatabaseReconnected(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDatabaseReconnectedEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 数据桥跟数据库断开，产生该事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDatabaseDisconnected(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnDatabaseDisconnectedEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 前段实例跟数据桥断开，产生该事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnFrontDisconn(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnFrontDisconnEvent e)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// 数据桥意外中断，产生该事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_OnDisconn(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 实例给应用程序发送信息，产生该事件;该事件用于通知客户端视频状态实时的改变
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
                    //触发客户端事件
                    if (OnIncomingEnvetHandle != null)
                    {
                        OnIncomingEnvetHandle(this, true);
                    }
                }
            }
        }

        /// <summary>
        /// 数据桥未收到信息（不能解析数据，一般是客户传递数据格式错误），产生该事件通知应用程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void axPrj_Client1_OnInvalidMsg(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnInvalidMsgEvent e)
        {
            // XtraMessageBox.Show("Invalid Message: " + e.isz_Msg, "系统提示！");
        }

        /// <summary>
        /// 当前段系统出现错误时,前段通知应用程序，系统发生错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void axPrj_Client1_OnErrMsg(object sender, AxPRJ_CLIENTLib._DPrj_ClientEvents_OnErrMsgEvent e)
        {
            //   MessageBox.Show(e.isz_Msg);
        }

        /// <summary>
        /// 连接数据桥
        /// </summary>
        /// <param name="ip">数据桥所在设备IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="instance">前端实例</param>
        public bool ConnVis(string ip, int port, string instance)
        {
            bool success = false;
            int state = Client.ConnSrv(ip, 1222, AvtvisId);
            if (state == 0)
                success = true;
            return success;
        }
        /// <summary>
        /// 摄像
        /// </summary>
        /// <param name="channel">摄像机号</param>
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
        /// 取消摄像
        /// </summary>
        /// <param name="channel">摄像机号</param>
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
                //没有句柄时数组数据为零
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
