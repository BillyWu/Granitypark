using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Configuration;
using Granity.communications;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using System.Net;
using System.Globalization;
using System.IO;
using Granity.CardOneCommi;
using Granity.winTools;
using System.IO.Ports;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using DevExpress.XtraEditors;
using System.Collections;
using System.Runtime.Serialization;

namespace Granity.granityMgr.ParkMgr
{
    public partial class FrmStatonWatchingStandard : DevExpress.XtraEditors.XtraForm
    {
        #region 停车场参数定义
        /// <summary>
        /// 临时地址信息
        /// </summary>
        private string tempInfo = string.Empty;
        /// <summary>
        /// 判断应用程序监控启动项。1表示启动视频卡；2表示启动车牌硬识别；
        /// </summary>
        public int videoFlag = 2;
        /// <summary>
        /// 视频卡提示信息
        /// </summary>
        string videoMsg = "";
        /// <summary>
        /// 刷卡缓存数据(结束/模式对话/data),@操作类别=进出场管理|押金管理
        /// </summary>
        string tagData = "";
        /// <summary>
        /// 当前窗口停车场设备目标
        /// </summary>
        Dictionary<string, CommiTarget> targets = new Dictionary<string, CommiTarget>();
        /// <summary>
        /// 入场设备地址
        /// </summary>
        string devNumIn = "";
        /// <summary>
        /// 出场设备地址
        /// </summary>
        string devNumOut = "";
        int[] dvhandle = new int[2];
        /// <summary>
        /// 是否已经加载监控
        /// </summary>
        bool isInitLoad = false;
        /// <summary>
        /// 当前窗口通讯指令列表
        /// </summary>
        List<CommandBase> cmdDevs = new List<CommandBase>();
        /// <summary>
        /// 设备通讯异常信息
        /// </summary>
        Dictionary<string, string> error = new Dictionary<string, string>();
        UnitItem UnitItem = null;
        /// <summary>
        /// 设备信息
        /// </summary>
        private string[] deviceInfo = null;
        /// <summary>
        /// 执行数据处理的Query,由单元初始化
        /// </summary>
        QueryDataRes Query = null;
        NameObjectList paramSystem = null;
        /// <summary>
        /// 进出场类型 1 进 2 出 3 一进一出 4 两进 5 两出 6 两进两出
        /// </summary>
        private string InOutType = "1";
        /// <summary>
        /// 视频卡类型
        /// </summary>
        public enum CardType
        {
            /// <summary>
            /// 微视视频卡
            /// </summary>
            MvAPI,
            /// <summary>
            /// 天敏视频卡
            /// </summary>
            Sa7134Capture,
        }
        /// <summary>
        /// panelVideo原始的宽度
        /// </summary>
        private int strartWidth = 0;
        /// <summary>
        /// panelVideo原始的高度
        /// </summary>
        private int strartHight = 0;
        /// <summary>
        /// 储值卡
        /// </summary>
        private string CardMoney = string.Empty;
        /// <summary>
        /// 初始化视频操作类
        /// </summary>
        VideoPreview preView = new VideoPreview();
        /// <summary>
        /// 双击某一panel 时，生成的通道号
        /// </summary>
        private Dictionary<int, Panel> currentChannel = new Dictionary<int, Panel>();
        private Dictionary<int, Panel> PnlVideo = new Dictionary<int, Panel>();
        /// <summary>
        /// 获取门岗的设备数量
        /// </summary>
        private int ParkDevCount;
        public int parkDevCount
        {
            get { return ParkDevCount; }
            set { ParkDevCount = value; }
        }
        /// <summary>
        /// 获取门岗进出场通讯设备的站址
        /// </summary>
        private string[] StatAddress;

        public string[] statAddress
        {
            get { return StatAddress; }
            set { StatAddress = value; }
        }

        private string AvtvisId = "AVT-VIS-01";
        /// <summary>
        /// 跟AVT交互的实例
        /// </summary>
        Avtvis Vis;

        /// <summary>
        /// Vis进场车牌号码，颜色，时间等信息
        /// </summary>
        private string CarNumberInfoIn = string.Empty;

        /// <summary>
        /// Vis出场车牌号码，颜色，时间等信息
        /// </summary>
        private string CarNumberInfoOut = string.Empty;

        /// <summary>
        /// 读卡器
        /// </summary>
        CmdCard cmdCard = new CmdCard();

        #endregion

        public FrmStatonWatchingStandard()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗口初始化加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmStationWatchingII_Load(object sender, EventArgs e)
        {
            //初始化场内停车信息
            this.UnitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "收费站");
            this.Query = new QueryDataRes(this.UnitItem.DataSrcFile);
            this.paramSystem = BindManager.getSystemParam();
            DataSet ds = new DataSet("字典");
            this.Query.FillDataSet("卡片类型", this.paramSystem, ds);
            this.Query.FillDataSet("车型", this.paramSystem, ds);
            DataRow drr = ds.Tables["卡片类型"].NewRow();
            ds.Tables["卡片类型"].Rows.InsertAt(drr, 0);
            drr = ds.Tables["车型"].NewRow();
            ds.Tables["车型"].Rows.InsertAt(drr, 0);
            this.cbbCardType.Properties.DataSource = ds.Tables["卡片类型"];
            this.cbbCardType.Properties.DisplayMember = "卡类";
            this.cbbCardType.Properties.ValueMember = "编号";
            this.cbbCarType.Properties.DataSource = ds.Tables["车型"];
            this.cbbCarType.Properties.DisplayMember = "车类";
            this.cbbCarType.Properties.ValueMember = "编号";

            videoFlag = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["videoFlag"]);
            InOutType = System.Configuration.ConfigurationManager.AppSettings["InOutType"];
            ReSize();
            //刷新场内停车管理块
            RefreshParkInfo();
            VideoView();
            GetAvt();
            this.gdDevice.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dbgrid_RowPostPaint);
            CommiManager.GlobalManager.ErrorOpenHandle += new EventHandler<ErrorCommiEventArgs>(GlobalManager_ErrorOpenHandle);
            this.strartWidth = panelVideo.Width;
            this.strartHight = panelVideo.Height;
        }

        /// <summary>
        /// 连接AVT实例，并获取图像
        /// </summary>
        private void GetAvt()
        {
            if (videoFlag != 2)
                return;
            int ipIndex = -1;
            Panel[] pnl = { this.p_in1, this.p_out1 };
            string[] allkeys = this.paramSystem.AllKeys;
            Array allValues = this.paramSystem.AllValues;
            foreach (string str in allkeys)
            {
                ipIndex++;
                if (str == "LocalIP")
                    break;
            }
            string ip = allValues.GetValue(ipIndex).ToString();
            Vis = new Avtvis(this.Client, AvtvisId);
            bool fag = Vis.ConnVis("127.0.0.1", 1222, AvtvisId);
            if (!fag)
                XtraMessageBox.Show("与前端实例连接失败，请检查系统设置", "系统提示！");
            else
            {
                Vis.OnIncomingEnvetHandle += new Avtvis.OnIncoming(vis_OnIncomingEnvetHandle);
                Vis.NewRecordEnvetHandle += new Avtvis.NewRecord(vis_NewRecordEnvetHandle);
                for (int i = 0; i < 2; i++)
                {
                    Vis.conChannel = i;
                    Vis.pnl = pnl;
                    Vis.Video( pnl[i]);
                }
            }
        }

        /// <summary>
        /// 获取前段系统识别新纪录信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="str"></param>
        void vis_NewRecordEnvetHandle(object sender, string str)
        {
            string Lan = basefun.valtag(str, "{车道}");
            if (Lan == "LN1")
                CarNumberInfoIn = str;
            else
                CarNumberInfoOut = str;
        }

        /// <summary>
        /// 从前段系统获取图像句柄
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="success"></param>
        void vis_OnIncomingEnvetHandle(object sender, bool success)
        {
            if (success == false)
                XtraMessageBox.Show("启动摄像失败", "系统提示！");
        }

        /// <summary>
        /// 判断是否加载视频卡，以及读出视频图像  车牌识别就不要初始化
        /// </summary>
        private void VideoView()
        {
            if (videoFlag == 2)
                return;
            Panel[] pl ={ p_in1, p_out1 };
            int channel = 0;
            videoFlag = 1;
            DataSet ds = new DataSet();
            this.Query.FillDataSet("门岗", this.paramSystem, ds);
            string videoName = ds.Tables[0].Rows[0]["视频卡"].ToString().Trim();
            switch (videoName)
            {
                case "天敏卡":
                    videoName = "Sa7134Capture";
                    break;
                case "MINI卡":
                    videoName = "MvAPI";
                    break;
            }
            channel = preView.InitVideo(this.Handle, (VideoType)(Enum.Parse(typeof(CardType), videoName)));
            if (channel == 0)
            {
                XtraMessageBox.Show("没有安装视频驱动", "系统提示！");
                return;
            }
            switch (InOutType)
            { 
                case "1":
                    PnlVideo.Add(0, this.p_in1);
                    this.p_in1.Tag = 0;
                    this.p_in2.Tag = 1;
                    this.p_out1.Tag = 2;
                    this.p_out2.Tag = 3;
                    break;
                case "2":
                    PnlVideo.Add(0, this.p_in1);
                    this.p_in1.Tag = 0;
                    this.p_in2.Tag = 1;
                    this.p_out1.Tag = 2;
                    this.p_out2.Tag = 3;
                    break;
                case "3":
                    PnlVideo.Add(0, this.p_in1);
                    PnlVideo.Add(1, this.p_out1);
                    this.p_in1.Tag = 0;
                    this.p_out1.Tag = 1;
                    this.p_in2.Tag = 2;
                    this.p_out2.Tag = 3;
                    break;
                case "4":
                    PnlVideo.Add(0, this.p_in1);
                    PnlVideo.Add(1, this.p_in2);
                    this.p_in1.Tag = 0;
                    this.p_in2.Tag = 2;
                    this.p_out1.Tag = 1;
                    this.p_out2.Tag = 3;
                    break;
                case "5":
                    PnlVideo.Add(0, this.p_in1);
                    PnlVideo.Add(1, this.p_in2);
                    this.p_in1.Tag = 0;
                    this.p_in2.Tag = 2;
                    this.p_out1.Tag = 1;
                    this.p_out2.Tag = 3;
                    break;
                case "6":
                    PnlVideo.Add(0, this.p_in1);
                    PnlVideo.Add(1, this.p_in2);
                    PnlVideo.Add(2, this.p_out1);
                    PnlVideo.Add(3, this.p_out2);
                    this.p_in1.Tag = 0;
                    this.p_in2.Tag = 2;
                    this.p_out1.Tag = 1;
                    this.p_out2.Tag = 3;
                    break;
            }
            foreach ( KeyValuePair<int,Panel>  dic in PnlVideo )
            {
                bool fag = preView.OpenChannel(dic.Key, dic.Value);
                dic.Value.BackColor = Color.Magenta;
            }
        }

        /// <summary>
        /// 判断是否加载视频卡，以及读出视频图像
        /// </summary>
        private void VideoView(Panel[] pnlVideo)
        {
            if (videoFlag == 2)
                return;
            Panel[] pl = pnlVideo;
            int channel = 0;
            videoFlag = 1;
            DataSet ds = new DataSet();
            this.Query.FillDataSet("门岗", this.paramSystem, ds);
            string videoName = ds.Tables[0].Rows[0]["视频卡"].ToString().Trim();
            switch (videoName)
            {
                case "天敏卡":
                    videoName = "Sa7134Capture";
                    break;
                case "MINI卡":
                    videoName = "MvAPI";
                    break;
            }
            channel = preView.InitVideo(this.Handle,(VideoType)(Enum.Parse(typeof(CardType), videoName)));
            if (channel == 0)
                XtraMessageBox.Show("没有安装视频驱动", "系统提示！");
            else
            {
                foreach (Panel pnl in pl)
                {
                    int i = Convert.ToInt16(pnl.Tag);
                    preView.OpenChannel(i, pnl);
                    pnl.BackColor = Color.Magenta;
                }
            }
        }

        /// <summary>
        /// 获取连接的押金设备(读卡器)
        /// </summary>
        private void SearchReadCardDev()
        {
            DataTable tab = this.Query.getTable("押金设备", this.paramSystem);
            if (null == tab || tab.Rows.Count < 1)
                return;
            DataColumnCollection datacols = tab.Columns;
            string[] cols ={ "设备类型", "设备名称", "IP地址", "串口", "通讯站址", "通讯状态", "通讯类别", "端口", "波特率", "数据位", "停止位" };
            foreach (DataRow dr in tab.Rows)
            {
                //表格字段顺序：
                string[] vals = new string[cols.Length];
                for (int i = 0; i < cols.Length; i++)
                {
                    if (!datacols.Contains(cols[i]))
                        continue;
                    vals[i] = Convert.ToString(dr[cols[i]]);
                }
                deviceInfo = vals;
                this.gdDevice.Rows.Add(vals);
            }
        }

        /// <summary>
        /// 得到该门岗的停车场设备
        /// </summary>
        private void SearchParkDev()
        {
            //bs里面多加了个条件 所以没有数据出来   and 通讯类别='发行器' 不需要
            DataTable tab = this.Query.getTable("设备列表", this.paramSystem);
            if (null == tab || tab.Rows.Count < 1)
                return;
            DataColumnCollection datacols = tab.Columns;
            string[] cols ={ "设备类型", "设备名称", "IP地址", "串口", "通讯站址", "通讯状态", "通讯类别", "端口", "波特率", "数据位", "停止位" };
            int index = 0;
            foreach (DataRow dr in tab.Select("", " 通讯站址 asc "))
            {
                //表格字段顺序：
                string[] vals = new string[cols.Length];
                for (int i = 0; i < cols.Length; i++)
                {
                    if (!datacols.Contains(cols[i]))
                        continue;
                    vals[i] = Convert.ToString(dr[cols[i]]);
                }
                deviceInfo = vals;
                this.gdDevice.Rows.Add(vals);
                index++;
            }
        }

        /// <summary>
        /// 将图片传输进入服务器
        /// </summary>
        /// <param name="filepath">文件路径</param>
        /// <param name="filedest">目标路径,相对路径</param>
        public void SendImage(string filepath, string filedest)
        {
            if (string.IsNullOrEmpty(filepath) || !File.Exists(filepath))
                return;
            int port = 2010;
            string ipsrv = "127.0.0.1";
            string sport = DataAccRes.AppSettings("Granity文件服务");
            string conn = DataAccRes.DefaultDataConnInfo.Value;
            if (!string.IsNullOrEmpty(sport))
                try { port = Convert.ToInt32(sport); }
                catch { return; }
            Regex regIP = new Regex(@"server=([\w.\(\)]*)(;|\\)");
            if (regIP.IsMatch(conn))
            {
                Match mt = regIP.Match(conn);
                if (mt.Groups.Count > 1)
                    ipsrv = mt.Groups[1].Value.ToLower();
                if ("(local)" == ipsrv || "127.0.0.1" == ipsrv)
                    ipsrv = Dns.GetHostName();
                ipsrv = Dns.GetHostAddresses(ipsrv)[0].ToString();
            }

            byte[] context = File.ReadAllBytes(filepath);
            CmdFileTrans cmd = new CmdFileTrans(false);
            cmd.SendFile(filedest, context);
            CommiTarget target = new CommiTarget(ipsrv, port, CommiType.TCP);
            CommiManager.GlobalManager.SendCommand(target, cmd);
            target.setProtocol(CmdFileTrans.PTL);
            if (!cmd.EventWh.WaitOne(new TimeSpan(0, 0, 15), false))
                return;
        }

        /// <summary>
        /// 从服务器中获得图片
        /// </summary>
        /// <param name="savePath">图片保存相对路径</param>
        /// <param name="pl">显示图片的容器</param>
        public void GetPicServer(string filepath, Panel pl)
        {
            if (string.IsNullOrEmpty(filepath))
                return;
            int port = 2010;
            string ipsrv = "127.0.0.1";
            string sport = DataAccRes.AppSettings("Granity文件服务");
            string conn = DataAccRes.DefaultDataConnInfo.Value;
            if (!string.IsNullOrEmpty(sport))
                try { port = Convert.ToInt32(sport); }
                catch { return; }
            Regex regIP = new Regex(@"server=([\w.\(\)]*)(;|\\)");
            if (regIP.IsMatch(conn))
            {
                Match mt = regIP.Match(conn);
                if (mt.Groups.Count > 1)
                    ipsrv = mt.Groups[1].Value.ToLower();
                if ("(local)" == ipsrv || "127.0.0.1" == ipsrv)
                    ipsrv = Dns.GetHostName();
                ipsrv = Dns.GetHostAddresses(ipsrv)[0].ToString();
            }
            CommiTarget target = new CommiTarget(ipsrv, port, CommiType.TCP);
            target.setProtocol(CmdFileTrans.PTL);
            CmdFileTrans cmd = new CmdFileTrans(false);
            cmd.GetFile(filepath);
            CommiManager.GlobalManager.SendCommand(target, cmd);
            if (cmd.EventWh.WaitOne(new TimeSpan(0, 0, 15), false))
            {
                byte[] data = cmd.FileContext;
                if (data.Length < 1)
                    return;
                MemoryStream stream = new MemoryStream(data);
                pl.BackgroundImage = Image.FromStream(stream);
            }
        }

        /// <summary>
        /// 图片抓拍,并显示图片
        /// </summary>
        /// <param name="VideoID">视频窗口ID</param>
        /// <param name="pl">显示图片容器</param>
        /// <returns>返回图片文件路径</returns>
        public string CutVideo(int videoID, Panel pl)
        { 
            string filepath=string.Empty ;
            if (videoFlag == 2)
            {
                string savePicPath = System.Configuration.ConfigurationManager.AppSettings["SavePicPath"].ToString();
                string pathfile = string.Format(@"{0}\image", savePicPath);
                pathfile = pathfile.Replace("/", "\\");
                if (!Directory.Exists(pathfile))
                    Directory.CreateDirectory(pathfile);
                if (videoID==0)
                 filepath = pathfile + basefun.valtag(CarNumberInfoIn, "{图片文件名称}");
                else
                 filepath = pathfile + basefun.valtag(CarNumberInfoOut, "{图片文件名称}");
            }
            else
            {
                //视频卡抓拍图像并保存到指定位置
                filepath = getLocalPath("");
                preView.SaveCaptureFile(0, filepath);
            }
            if (!File.Exists(filepath))
                return "";
            byte[] context = File.ReadAllBytes(filepath);
           // 读出所需要的图片
            pl.BackgroundImage = Image.FromFile(filepath);
            return filepath;
        }

        /// <summary>
        /// 获取数据行的设备目标位置参数
        /// 记录包含字段【访问方式】(TCP/UDP/SerialPort)、【端口】(60000/COM1)、【地址】(192.168.1.146)
        /// </summary>
        /// <param name="dr">数据记录</param>
        /// <returns></returns>
        private CommiTarget getTarget(DataGridViewCellCollection cells)
        {
            if (null == cells) return null;
            CommiTarget target = new CommiTarget();
            CommiType commiType = CommiType.UDP;
            string stype = Convert.ToString(cells["通讯类别"].Value);
            switch (stype)
            {
                case "TCP/IP(局域网)":
                    commiType = CommiType.TCP; break;
                case "UDP/IP(局域网)":
                    commiType = CommiType.UDP; break;
                default:
                    commiType = CommiType.SerialPort; break;
            }
            try
            {
                if (CommiType.SerialPort == commiType)
                {
                    string portname = Convert.ToString(cells["串口"].Value);
                    int baudRate = Convert.ToInt16(cells["波特率"].Value);
                    int dataBits = Convert.ToInt16(cells["数据位"].Value);
                    decimal s = Convert.ToDecimal(cells["停止位"].Value);
                    StopBits sb = StopBits.None;
                    if (1 == s) sb = StopBits.One;
                    else if (2 == s) sb = StopBits.Two;
                    else if (1 < s && s < 2) sb = StopBits.OnePointFive;
                    target.SetProtocolParam(portname, baudRate, Parity.None, dataBits, sb);
                }
                else
                {
                    string addr = Convert.ToString(cells["IP地址"].Value);
                    int port = Convert.ToInt32(cells["端口"].Value);
                    target.SetProtocolParam(addr, port, commiType);
                }
                target.setProtocol(Protocol.PTLPark);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "通讯设备提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return target;
        }

        /// <summary>
        /// 为场内停车信息版块赋值
        /// </summary>
        /// <param name="ct">版块容器</param>
        /// <param name="tag">传入值</param>
        /// <param name="keyName">键值</param>
        /// <returns>返回tag</returns>
        private string setContainerTagValue(Control ct, string tag, string keyName)
        {
            if (null == ct || string.IsNullOrEmpty(tag) || string.IsNullOrEmpty(keyName))
                return tag;
            string t = Convert.ToString(ct.Tag);
            string pm = basefun.valtag(t, keyName);
            if (!string.IsNullOrEmpty(pm))
            {
                string val = basefun.valtag(tag, "{" + pm + "}");
                string v = val;
                if (string.IsNullOrEmpty(val))
                    v = basefun.valtag(tag, pm);
                if (!(ct is DevExpress.XtraEditors.LookUpEdit))
                    ct.Text = v;
                else
                {
                    DevExpress.XtraEditors.LookUpEdit cbb = ct as DevExpress.XtraEditors.LookUpEdit;
                    try { cbb.EditValue = int.Parse(v); }
                    catch { }
                    if (string.IsNullOrEmpty(val))
                        tag = basefun.setvaltag(tag, pm + "名称", cbb.Text);
                    else
                        tag = basefun.setvaltag(tag, "{" + pm + "名称" + "}", cbb.Text);
                }
            }
            foreach (Control child in ct.Controls)
                tag = this.setContainerTagValue(child, tag, keyName);
            return tag;
        }

        /// <summary>
        /// 获得场内停车信息
        /// </summary>
        private void RefreshParkInfo()
        {
            NameObjectList ps = BindManager.getSystemParam();
            DataTable tab = this.Query.getTable("场内停车信息统计", ps);
            if (null == tab || tab.Rows.Count < 1)
                return;
            string tag = "";
            DataRow dr = tab.Rows[0];
            foreach (DataColumn c in tab.Columns)
                tag = basefun.setvaltag(tag, c.ColumnName, Convert.ToString(dr[c]));
            this.setContainerTagValue(this.grpParkInfo, tag, "pm");
        }

        /// <summary>
        /// 设置表格行标号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbgrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            if (null == grid) return;
            using (SolidBrush b = new SolidBrush(grid.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(CultureInfo.CurrentCulture),
                        grid.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
        }

        /// <summary>
        /// 通讯连接错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GlobalManager_ErrorOpenHandle(object sender, ErrorCommiEventArgs e)
        {
            if (null == e || null == e.Client || null == e.Target)
                return;
            CommiTarget commip = e.Target;
            switch (commip.ProtocolType)
            {
                case CommiType.TCP:
                case CommiType.UDP:
                    string ip = commip.SrvEndPoint.ToString();
                    error[ip] = e.Exception.Message;
                    break;
                case CommiType.SerialPort:
                    string portname = commip.PortName;
                    error[portname] = e.Exception.Message;
                    break;
            }
        }

        /// <summary>
        /// 监控停车场设备
        /// </summary>
        private void Monitor()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string ipstr = ",";
            foreach (IPAddress ip in IpEntry.AddressList)
                ipstr += ip + ",";
            string tpl = "停车场", cmd = "检测状态";
            foreach (DataGridViewRow dr in this.gdDevice.Rows)
            {
                string devicetype = Convert.ToString(dr.Cells["设备类型"].Value);
                string devname = Convert.ToString(dr.Cells["设备名称"].Value);
                string port = Convert.ToString(dr.Cells["串口"].Value);
                int baudrate = Convert.ToInt32(dr.Cells["波特率"].Value);
                CommiTarget target = this.getTarget(dr.Cells);
                string addrst = Convert.ToString(dr.Cells["通讯站址"].Value);
                int addrdev = Convert.ToInt16(addrst);
                string tagdata = "@设备地址=" + addrst;
                if (this.targets.ContainsKey(addrst.ToString()))
                    continue;
                if ("发行器" == devicetype)
                {
                    Parity parity = Parity.None;
                    int databits = 8;
                    StopBits stopbits = StopBits.One;
                    CommiTarget targetRead = new CommiTarget(port, baudrate, parity, databits, stopbits);
                    bool success = this.cmdCard.SetTarget(targetRead, Convert.ToInt32(addrst), false);
                    continue;
                }
                CmdProtocol cmdP = new CmdProtocol(devname + "(" + addrst + ")");
                cmdP.EventWh = new ManualResetEvent(false);
                //停车场
                if (addrdev < 129)
                    this.devNumIn = addrst;
                else
                    this.devNumOut = addrst;
                cmdP.setCommand(tpl, cmd, tagdata);
                cmdP.ResponseHandle += new EventHandler<ResponseEventArgs>(cmdP_ResponseHandle);
                cmdP.Tag = tagdata;
                //持续间隔发送通过监听发现是否有新纪录
                cmdP.TimeFailLimit = TimeSpan.MaxValue;
                cmdP.TimeLimit = TimeSpan.MaxValue;
                cmdP.TimeSendInv = new TimeSpan(0, 0, 1);
                CommiManager.GlobalManager.SendCommand(target, cmdP);
                this.targets.Add(addrst, target);
                this.cmdDevs.Add(cmdP);
            }
        }

        /// <summary>
        /// 停车场有刷卡，读取卡信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdP_ResponseHandle(object sender, ResponseEventArgs e)
        {
            CmdProtocol cmd = sender as CmdProtocol;
            if ("结束" == this.tagData || null == cmd)
                return;
            CommiTarget commipm = e.Target;
            string tag = cmd.ResponseFormat;
            string ErrorInfo = CommErrorInfo(tag);
            string station = basefun.valtag(Convert.ToString(cmd.Tag), "设备地址");
            switch (commipm.ProtocolType)
            {
                case CommiType.TCP:
                case CommiType.UDP:
                    string ip = commipm.SrvEndPoint.ToString();
                    error[ip] = "通讯正常";
                    if (!string.IsNullOrEmpty(ErrorInfo))
                        error[ip] = ErrorInfo;
                    break;
                case CommiType.SerialPort:
                    string portname = commipm.PortName + ":" + station;
                    error[portname] = string.IsNullOrEmpty(ErrorInfo) ? "通讯正常" : ErrorInfo;
                    break;
            }
            string recordCount = basefun.valtag(tag, "{新记录}");
            string cardnum = basefun.valtag(tag, "{卡号}");
            cardnum = "0" == cardnum ? "" : cardnum;
            if (string.IsNullOrEmpty(cardnum) && (string.IsNullOrEmpty(recordCount) || int.Parse(recordCount) <= 0))
                return;
            if (!string.IsNullOrEmpty(cardnum) && cardnum == CardMoney)
                return;
            CardMoney = cardnum;
            if ("0" != recordCount)
            {
                string tpl = "停车场", cmdNextName = "收集下一条记录";
                CmdProtocol cmdNext = new CmdProtocol(false);
                tag = basefun.setvaltag(tag, "设备地址", station);
                cmdNext.setCommand(tpl, cmdNextName, tag);
                CommiManager.GlobalManager.SendCommand(e.Target, cmdNext);
                if (cmdNext.EventWh.WaitOne(800, false))
                {
                    tag = cmdNext.ResponseFormat;
                    tag = basefun.setvaltag(tag, "操作类别", "进出场管理");
                    tag = basefun.setvaltag(tag, "设备名称", cmd.CmdId);
                    cardnum = basefun.valtag(tag, "{卡号}");
                    string carnum = basefun.valtag(tag, "{车牌号码}");
                    if (string.IsNullOrEmpty(carnum.Replace("0", "")))
                        tag = basefun.setvaltag(tag, "{车牌号码}", "");
                    Debug.WriteLine(DateTime.Now.ToString("ss.fff") + "   tag;" + tag + "  新记录：" + recordCount);
                }
            }
            NameObjectList ps = new NameObjectList();
            ps["卡号"] = cardnum;
            DataTable tab = this.Query.getTable("获得卡信息", ps);
            if (null == tab || tab.Rows.Count < 1)
                return;
            DataRow dr = tab.Rows[0];
            foreach (DataColumn c in tab.Columns)
            {
                string fld = c.ColumnName;
                string val = basefun.valtag(tag, "{" + fld + "}");
                string val2 = basefun.valtag(tag, fld);
                if (string.IsNullOrEmpty(val) && !string.IsNullOrEmpty(val2))
                    basefun.setvaltag(tag, "{" + fld + "}", val2);
                if (!string.IsNullOrEmpty(val) || !string.IsNullOrEmpty(val2))
                    continue;
                val = Convert.ToString(dr[c]);
                if (string.IsNullOrEmpty(val))
                    continue;
                tag = basefun.setvaltag(tag, "{" + fld + "}", val);
            }
            string strNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (string.IsNullOrEmpty(basefun.valtag(tag, "{入场时间}")))
                tag = basefun.setvaltag(tag, "{入场时间}", strNow);
            if (string.IsNullOrEmpty(basefun.valtag(tag, "{出入场时间}")))
                tag = basefun.setvaltag(tag, "{出入场时间}", strNow);
            if (string.IsNullOrEmpty(basefun.valtag(tag, "{出场时间}")))
                tag = basefun.setvaltag(tag, "{出场时间}", basefun.valtag(tag, "{出入场时间}"));
            tag = basefun.setvaltag(tag, "操作类别", "进出场管理");
            tagData = tag;
        }

        /// <summary>
        /// 暂停或恢复巡检
        /// </summary>
        /// <param name="ispause">是否暂停：true/暂停，false/恢复</param>
        private void Pause(bool ispause)
        {
            TimeSpan ts = new TimeSpan(24, 0, 0);
            if (!ispause)
                ts = new TimeSpan(0, 0, 1);
            foreach (CommandBase c in this.cmdDevs)
                c.TimeSendInv = ts;
            this.cmdCard.Pause(ispause);
            this.tagData = ispause ? "模式对话" : "";
        }

        /// <summary>
        /// 通讯错误信息
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        private string CommErrorInfo(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return "";
            string Info = string.Empty;
            string[] ParmAll ={ "系统参数错", "白名单错", "收费方式错", "时间格式错", "白名单溢出", "时段错",
                "车位错", "服务器", "出卡机卡欠量", "出卡机错", "地感错", "CAN通信失败","中文屏错","满位屏错",
                "外设保留","收费屏错","LCD错","缺纸","纸少","记录区溢出","入口堵塞" };
            foreach (string str in ParmAll)
            {
                string info = basefun.valtag(tag, str);
                if (info == "1")
                {
                    if (Info == string.Empty)
                    {
                        Info = string.Format("{0}", str);
                    }
                    else
                    {
                        Info += string.Format(",{0}", str);
                    }
                }
            }
            return Info;
        }

        /// <summary>
        /// 获取该门岗出入通讯设备的数量
        /// </summary>
        /// <returns></returns>
        private int GetParkDevCount()
        {
            int ParkDevCount = 2;
            DataTable tab = this.Query.getTable("设备列表", this.paramSystem);
            if (null != tab)
                ParkDevCount = tab.Rows.Count;
            return ParkDevCount;
        }

        /// <summary>
        /// 处理进出场数据，并抓拍图像
        /// </summary>
        /// <param name="tag">出入场数据</param>
        /// <param name="display">判断是否显示刷卡数据信息</param>
        private void ExecuteInOut(string tag, bool display)
        {
            string station = basefun.valtag(tag, "{设备地址}");
            string cardnum = basefun.valtag(tag, "{卡号}");
            string cardtype = string.Empty;
            cardtype = basefun.valtag(tag, "卡类");
            if(string.IsNullOrEmpty(  cardtype))
                cardtype = basefun.valtag(tag, "{卡类}");
            string dtparkin = basefun.valtag(tag, "{入场时间}");
            string dtparkout = basefun.valtag(tag, "{出入场时间}");
            string openmode = basefun.valtag(tag, "{开闸方式}");
            if (string.IsNullOrEmpty(cardnum))
                return;

            #region 抓拍图片并上传服务器
            string filepath = "", filedest = "";
            if (videoFlag > 0)
            {
                int videonum = 1;
                string imgkey = "出场图片";
                if (dtparkin == dtparkout)
                {
                    videonum = 0;
                    imgkey = "入场图片";
                }
                filepath = CutVideo(videonum, 0 == videonum ? p_in2 : p_out2);
                if (!string.IsNullOrEmpty(filepath))
                    filedest = filepath.ToLower().Substring(filepath.IndexOf("image"));
                tag = basefun.setvaltag(tag, imgkey, filedest);
            }
            SendImage(filepath, filedest);
            #endregion

            //不同类别卡做出入场业务处理
            //1,无识别车牌，则模式对话输入
            //2,出场识别车牌与入场车牌不一致，则模式对话输入
            //3,临时卡入场识别车牌后直接开闸
            //4,储值卡车牌对比后再校验卡片余额
            //5,期卡车牌对比后直接开闸

            //储值卡已经入场再入场提示，未入场刷出场时提示
            if ("9" == cardtype)
            {
                int addr = Convert.ToInt32(station);
                string tipsound = "";
                if (addr < 128 && dtparkin != dtparkout)
                    tipsound = "0C";
                else if (addr >= 128 && dtparkin == dtparkout)
                    tipsound = "2A";

                // 0C此卡已入场，2A此卡已出场
                if (!string.IsNullOrEmpty(tipsound))
                {
                    string tagsound = string.Empty;
                    tagsound = basefun.setvaltag(tagsound, "{道闸命令}", "00");
                    tagsound = basefun.setvaltag(tagsound, "{语音命令}", tipsound);
                    CmdExecute("停车场", "屏显语音道闸", tagsound, station);
                    return;
                }
            }
            //出场时，首先计算临时卡和储值卡收费金额
            if ("9" == cardtype && dtparkin != dtparkout)
            {
                if (string.IsNullOrEmpty(dtparkout))
                {
                    dtparkout = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    tag = basefun.setvaltag(tag, "{出入场时间}", dtparkout);
                }
                tag = basefun.setvaltag(tag, "{出场时间}", dtparkout);
                string rsponseFormat = CmdExecute("停车场", "计算收费金额", tag, station, true);
                tag = basefun.setvaltag(tag, "{收费金额}", basefun.valtag(rsponseFormat, "{收费金额}"));
            }
            //识别车牌不一致，或临时卡时模式对话
            string carnum = basefun.valtag(tag, "{车牌号码}");
            string caravt=string.Empty ;
            if (videoFlag == 2)
                caravt = basefun.valtag(tag, "{识别车牌号码}");
            string strmoney = basefun.valtag(tag, "{卡片余额}");
            string strpay = basefun.valtag(tag, "{收费金额}");
            bool isDialog = false;
            if ("44" == openmode || string.IsNullOrEmpty(openmode))
            {
                if (videoFlag == 2)
                    isDialog = this.IsDialogInOut(cardtype, dtparkin, dtparkout, carnum, caravt, strmoney, strpay);
                else
                    isDialog = this.IsDialogInOut(cardtype, dtparkin, dtparkout, strmoney, strpay);
            }
            //模式对话处理业务
            DialogResult rsl = DialogResult.Yes;
            if (isDialog)
            {
                this.Pause(true);
                if (dtparkin == dtparkout)
                {
                    FrmCarNo inPark = new FrmCarNo();
                    inPark.tagDate = tag;
                    rsl = inPark.ShowDialog();
                    tag = inPark.tagDate;
                    if (videoFlag == 2)
                        tag = basefun.setvaltag(tag, "{车牌号码}", basefun.valtag(inPark.tagDate, "输入进场车牌"));
                }
                else
                {
                    FrmFee win = new FrmFee();
                    tag = basefun.setvaltag(tag, "操作员", BindManager.getUser().UserAccounts);
                    win.DataTag = tag;
                    win.Query = this.Query;
                    rsl = win.ShowDialog();
                    tag = win.DataTag;
                }
                this.Pause(false);
            }
            else
            {
                if (cardtype == "5" && videoFlag == 2)
                    tag = basefun.setvaltag(tag, "{车牌号码}", basefun.valtag(tag, "{识别车牌号码}"));
            }
            string db = dtparkin == dtparkout ? "入场管理" : "出场管理";
            //不开闸，临时卡和期卡删除下位机记录
            if (DialogResult.Yes != rsl && DialogResult.OK != rsl)
            {
                this.setContainerTagValue(this.grpInfoM, tag, "pm");
                this.InOutParkRecord(tag, db);
                RefreshParkInfo();
                CmdExecute("停车场", "删除一条停车记录", tag, station);
                CardMoney = string.Empty;
                return;
            }
            string ps = tag;
            string strts = "0";
            if (dtparkin != dtparkout)
            {
                TimeSpan ts = Convert.ToDateTime(dtparkout) - Convert.ToDateTime(dtparkin);
                strts = Convert.ToInt32(ts.Days * 24 * 60 + ts.Hours * 60 + ts.Minutes).ToString();
            }
            string[,] map ={ { "{道闸命令}", "02" }, { "{收费额}", "{收费金额}" }, { "{余额}", "{卡片余额}" }, { "{停车时长}", strts } };
            for (int i = 0; i < map.GetLength(0); i++)
            {
                if (map[i, 1].StartsWith("{"))
                    tag = basefun.setvaltag(tag, map[i, 0], basefun.valtag(tag, map[i, 1]));
                else
                    tag = basefun.setvaltag(tag, map[i, 0], map[i, 1]);
            }
            tag = basefun.setvaltag(tag, "{语音命令}", this.SoundCmd(cardtype, dtparkin, dtparkout));
            this.Pause(true);
            CmdExecute("停车场", "屏显语音道闸", tag, station);
            this.Pause(false);
            this.setContainerTagValue(this.grpInfoM, tag, "pm");
            this.InOutParkRecord(tag, db);
            RefreshParkInfo();
        }

        /// <summary>
        ///  发送语音,执行语音开闸
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="fag">fag=true 开闸　其他不开闸 </param>
        private string SendSound(string tag, string dtNow, string dtparkin, string cardtype, bool fag)
        {
            string strts = "0";
            if (!string.IsNullOrEmpty(dtparkin))
            {
                TimeSpan ts = Convert.ToDateTime(dtNow) - Convert.ToDateTime(dtparkin);
                strts = Convert.ToInt32(ts.Days * 24 * 60 + ts.Hours * 60 + ts.Minutes).ToString();
            }
            string[,] map = new string[4, 2];
            string[,] mapTrue ={ { "{道闸命令}", "02" }, { "{收费额}", "{收费金额}" }, { "{余额}", "{卡片余额}" }, { "{停车时长}", strts } };
            string[,] mapFalse ={ { "{道闸命令}", "00" }, { "{收费额}", "{收费金额}" }, { "{余额}", "{卡片余额}" }, { "{停车时长}", strts } };
            if (fag)
                map = mapTrue;
            else
                map = mapFalse;
            for (int i = 0; i < map.GetLength(0); i++)
            {
                if (map[i, 1].StartsWith("{"))
                    tag = basefun.setvaltag(tag, map[i, 0], basefun.valtag(tag, map[i, 1]));
                else
                    tag = basefun.setvaltag(tag, map[i, 0], map[i, 1]);
            }
            tag = basefun.setvaltag(tag, "{语音命令}", this.SoundCmd(cardtype, dtparkin, dtNow));
            this.Pause(true);
            Thread.Sleep(500);
            CmdExecute("停车场", "屏显语音道闸", tag, this.devNumOut);
            this.Pause(false);
            return tag;
        }

        /// <summary>
        /// 对比判定是否需要模式对话过程
        /// </summary>
        /// <param name="cardtype">卡类</param>
        /// <param name="dtparkin">入场时间</param>
        /// <param name="dtparkout">出场时间</param>
        /// <param name="carnum">原车牌号码</param>
        /// <param name="caravt">识别的车牌号码</param>
        /// <param name="strmoney">卡片余额</param>
        /// <param name="strpay">收费金额</param>
        /// <returns></returns>
        private bool IsDialogInOut(string cardtype, string dtparkin, string dtparkout, string carnum, string caravt, string strmoney, string strpay)
        {
            bool isDialog = false;
            if (string.IsNullOrEmpty(strpay))
                strpay = "0";
            switch (cardtype)
            {
                case "9":
                    if (string.IsNullOrEmpty(caravt) || carnum != caravt)
                        isDialog = true;
                    else if (Convert.ToDecimal(strmoney)< Convert.ToDecimal(strpay))
                        isDialog = true;
                    break;
                case "3":
                    if (string.IsNullOrEmpty(caravt) || carnum != caravt)
                        isDialog = true;
                    break;
                case "5":
                    if (dtparkin == dtparkout && string.IsNullOrEmpty(caravt))
                        isDialog = true;
                    else if (dtparkin != dtparkout)
                        isDialog = true;
                    break;
                    break;
                default:
                    break;
            }
            //储值卡欠费
            if (!isDialog && "9" == cardtype)
            {
                double leavemoney = Convert.ToDouble(strmoney);
                double paymoney = 0;
                if (dtparkin != dtparkout)
                    paymoney = Convert.ToDouble(string.IsNullOrEmpty(strpay) ? "0" : strpay);
                if (string.IsNullOrEmpty(dtparkin) && leavemoney < 1)
                    isDialog = true;
                else if (dtparkin != dtparkout && paymoney > leavemoney)
                    isDialog = true;
            }
            return isDialog;
        }

        /// <summary>
        /// 对比判定是否需要模式对话过程
        /// </summary>
        /// <param name="cardtype">卡类</param>
        /// <param name="dtparkin">入场时间</param>
        /// <param name="dtparkout">出场时间</param>
        /// <param name="strmoney">卡片余额</param>
        /// <param name="strpay">收费金额</param>
        /// <returns></returns>
        private bool IsDialogInOut(string cardtype, string dtparkin, string dtparkout, string strmoney, string strpay)
        {
            bool isDialog = false;
            if (string.IsNullOrEmpty(strpay))
                strpay = "0";
            //储值卡欠费
            if (!isDialog && "9" == cardtype)
            {
                double leavemoney = Convert.ToDouble(strmoney);
                double paymoney = 0;
                if (dtparkin != dtparkout)
                    paymoney = Convert.ToDouble(string.IsNullOrEmpty(strpay) ? "0" : strpay);
                if (string.IsNullOrEmpty(dtparkin) && leavemoney < 1)
                    isDialog = true;
                else if (dtparkin != dtparkout && paymoney > leavemoney)
                    isDialog = true;
            }
            return isDialog;
        }

        /// <summary>
        /// 获取语音指令号
        /// </summary>
        /// <param name="cardtype"></param>
        /// <param name="dtparkin"></param>
        /// <param name="dtparkout"></param>
        /// <returns></returns>
        private string SoundCmd(string cardtype, string dtparkin, string dtparkout)
        {
            string sound = "22";
            switch (cardtype)
            {
                case "3":
                    sound = dtparkin == dtparkout ? "04" : "24";
                    break;
                case "5":
                    sound = dtparkin == dtparkout ? "08" : "22";
                    break;
                case "9":
                    sound = dtparkin == dtparkout ? "06" : "27";
                    break;
            }
            return sound;
        }

        /// <summary>
        /// 巡检停车场刷卡结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmCard_Tick(object sender, EventArgs e)
        {
            if (!this.isInitLoad)
            {
                this.tmCard.Stop();
                this.gdDevice.Rows.Clear();
                //获取押金设备
                SearchReadCardDev();
                //获得设备列表
                SearchParkDev();
                if (!string.IsNullOrEmpty(this.videoMsg))
                {
                    videoFlag = 0;
                    MessageBox.Show(this.videoMsg, "视频卡提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.Monitor();
                //刷新场内停车管理块
                RefreshParkInfo();
                this.isInitLoad = true;
                this.tmCard.Start();
                return;
            }
            foreach (DataGridViewRow gdr in this.gdDevice.Rows)
            {
                string ip = Convert.ToString(gdr.Cells["IP地址"].Value);
                string portname = Convert.ToString(gdr.Cells["串口"].Value);
                string station = Convert.ToString(gdr.Cells["通讯站址"].Value);
                if (error.ContainsKey(ip + ":" + portname))
                {
                    gdr.Cells["通讯状态"].Value = error[ip + ":" + portname];
                    continue;
                }
                if (error.ContainsKey(portname))
                    gdr.Cells["通讯状态"].Value = error[portname];
                if (error.ContainsKey(portname + ":" + station))
                    gdr.Cells["通讯状态"].Value = error[portname + ":" + station];
            }
            if ("结束" == tagData || "模式对话" == tagData || string.IsNullOrEmpty(tagData))
                return;
            string tag = tagData;
            tagData = "";
            string cardnum = basefun.valtag(tag, "{卡号}");
            string optype = basefun.valtag(tag, "操作类别");
            if (string.IsNullOrEmpty(cardnum) || string.IsNullOrEmpty(optype) || "进出场管理" != optype)
                return;
            //从板子取值则判断相等时"入场"，从数据库取值则都为空时"入场"
            if (videoFlag == 2)
            {
                string dtparkin = basefun.valtag(tag, "{入场时间}");
                string dtparkout = basefun.valtag(tag, "{出入场时间}");
                if (dtparkin == dtparkout)
                    tag = basefun.setvaltag(tag, "{识别车牌号码}", basefun.valtag(this.CarNumberInfoIn, "{车牌号码}"));
                else
                    tag = basefun.setvaltag(tag, "{识别车牌号码}", basefun.valtag(this.CarNumberInfoOut, "{车牌号码}"));
            }
            this.ExecuteInOut(tag, true);
        }

        /// <summary>
        /// 前一次刷卡卡号,最近两次卡号相同则忽略处理
        /// </summary>
        string cardNoFirst = "";
        /// <summary>
        /// 巡检押金读卡器,有新的卡号就显示押金
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmReader_Tick(object sender, EventArgs e)
        {
            if (this.cmdCard == null)
                return;
            string cardnum = this.cmdCard.CardNum;
            string portname = string.Empty;
            if (this.cmdCard.Targetwr != null)
            {
                portname = this.cmdCard.Targetwr.PortName + ":" + "3";
                error[portname] = string.IsNullOrEmpty(this.cmdCard.StateResponse) ? "通讯正常" : this.cmdCard.StateResponse;
            }
            if (string.IsNullOrEmpty(this.devNumOut) || cardnum == this.cardNoFirst)
                return;
            if ("结束" == tagData || "模式对话" == tagData )
                return;
            this.cardNoFirst = cardnum;
            if (string.IsNullOrEmpty(cardnum))
                return;
            NameObjectList pm = new NameObjectList();
            pm["卡号"] = cardnum;
            DataTable tab = this.Query.getTable("获得卡信息", pm);
            if (null == tab || tab.Rows.Count < 1)
                return;
            //根据数据库初始化tag信息
            string tag = "";
            DataRow dr = tab.Rows[0];
            foreach (DataColumn c in tab.Columns)
                tag = basefun.setvaltag(tag, "{" + c.ColumnName + "}", Convert.ToString(dr[c]));
            string dtparkin = basefun.valtag(tag, "{入场时间}");
            //补充识别车牌号码及收费金额
            string cardtype = basefun.valtag(tag, "{卡类}");
            string dtNow = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (string.IsNullOrEmpty(dtparkin))
                tag = basefun.setvaltag(tag, "{识别车牌号码}", basefun.valtag(this.CarNumberInfoIn, "{车牌号码}"));
            else
            {
                // 抓拍图片并上传服务器
                string filepath = "", filedest = "";
                if (videoFlag > 0)
                {
                    int videonum = 1;
                    string imgkey = "出场图片";
                    filepath = CutVideo(videonum, p_out2);
                    if (!string.IsNullOrEmpty(filepath))
                        filedest = filepath.ToLower().Substring(filepath.IndexOf("image"));
                    tag = basefun.setvaltag(tag, imgkey, filedest);
                }
                SendImage(filepath, filedest);
                tag = basefun.setvaltag(tag, "{识别车牌号码}", basefun.valtag(this.CarNumberInfoOut, "{车牌号码}"));
                tag = basefun.setvaltag(tag, "{出场时间}", dtNow);
                tag = basefun.setvaltag(tag, "{出入场时间}", dtNow);
                if ("3" != cardtype)
                {
                    string responseFormat = CmdExecute("停车场", "计算收费金额", tag, this.devNumOut, true);
                    if (string.IsNullOrEmpty(responseFormat))
                        return;
                    tag = basefun.setvaltag(tag, "{收费金额}", basefun.valtag(responseFormat, "{收费金额}"));
                }
                else
                    tag = basefun.setvaltag(tag, "{收费金额}", "0");
            }
            //没有车牌识别系统时，把之前处理的车牌识别号码取消置空
            if (videoFlag != 2)
                tag = basefun.setvaltag(tag, "{识别车牌号码}", "");
            //模式对话，当前卡信息
            DialogResult rsl = DialogResult.Yes;
            tag = SendSound(tag, dtNow, dtparkin, cardtype, false);
            this.Pause(true);
            FrmTempTotalMoeny win = new FrmTempTotalMoeny();
            NameObjectList info = new NameObjectList();
            string oper = BindManager.getUser().UserAccounts;
            tag = basefun.setvaltag(tag, "操作员", oper);
            win.DataTag = tag;
            win.Query = this.Query;
            rsl = win.ShowDialog();
            tag = win.DataTag;
            this.Pause(false);
            if (DialogResult.Yes != rsl && DialogResult.OK != rsl)
            {
                CmdExecute("停车场", "删除一条停车记录", tag, this.devNumIn);
                this.InOutParkRecord(tag, "出场管理");
                RefreshParkInfo();
                return;
            }
            string ps = tag;
            string strts = "0";
            if (!string.IsNullOrEmpty(dtparkin))
            {
                TimeSpan ts = Convert.ToDateTime(dtNow) - Convert.ToDateTime(dtparkin);
                strts = Convert.ToInt32(ts.Days * 24 * 60 + ts.Hours * 60 + ts.Minutes).ToString();
            }
            tag = SendSound(tag, dtNow, dtparkin, cardtype, true);
            CmdExecute("停车场", "删除一条停车记录", tag, this.devNumIn);
            this.InOutParkRecord(tag, "出场管理");
            RefreshParkInfo();
        }

        /// <summary>
        /// 综合处理出入场记录
        /// </summary>
        /// <param name="tag">记录数据,tag格式</param>
        /// <param name="inOut">执行业务：入场管理,出场管理</param>
        private void InOutParkRecord(string tag, string inOut)
        {
            NameObjectList ps = ParamManager.createParam(tag);
            ParamManager.MergeParam(ps, this.paramSystem, false);
            tag = this.setContainerTagValue(this.grpInfoM, tag, "pm");
            tag = this.setContainerTagValue(this.grpInfoDetail, tag, "pm");
            if (this.gdParkInout.Rows.Count >= 9)
                this.gdParkInout.Rows.RemoveAt(this.gdParkInout.Rows.Count - 1);
            string dtOut = basefun.valtag(tag, "{出入场时间}");
            string dtin = basefun.valtag(tag, "{入场时间}");
            string dtout = string.Empty;
            if (!string.IsNullOrEmpty(dtOut))
                dtout = Convert.ToDateTime(dtOut).ToString("dd日 HH:mm:ss");
            if (!string.IsNullOrEmpty(dtin))
                dtin = Convert.ToDateTime(dtin).ToString("dd日 HH:mm:ss");
            //出入场时间相等表示入场
            if (dtin == dtout)
                dtout = string.Empty;
            string cardNo = basefun.valtag(tag, "{卡号}");
            string carType = basefun.valtag(tag, "车型名称");
            if (string.IsNullOrEmpty(carType))
                carType = basefun.valtag(tag, "{车型名称}");
            this.gdParkInout.Rows.Insert(0, cardNo, carType, dtin, dtout);
            this.Query.ExecuteNonQuery(inOut, ps, ps, ps);
            if (videoFlag == 2)
            {
                if (inOut == "入场管理")
                    CarNumberInfoIn = string.Empty;
                else
                    CarNumberInfoOut = string.Empty;
            }
        }

        /// <summary>
        /// 执行指令; 指令执行失败时，会重新执行一次
        /// </summary>
        /// <param name="ptl">协议</param>
        /// <param name="cmd">指令</param>
        /// <param name="tag">指令需要数据</param>
        /// <param name="devid">设备地址</param>
        /// <returns>true false</returns>
        private bool CmdExecute(string ptl, string cmd, string tag, string devid)
        {
            CmdProtocol cmdOpenP = new CmdProtocol(cmd, false);
            tag = basefun.setvaltag(tag, "设备地址", devid);
            cmdOpenP.setCommand(ptl, cmd, tag);
            Debug.WriteLine(DateTime.Now.ToString("mm:ss.fff") + "   tag:" + tag);
            CommiManager.GlobalManager.SendCommand(this.targets[devid], cmdOpenP);
            return  cmdOpenP.EventWh.WaitOne(1000, false);
          //  Debug.WriteLine(DateTime.Now.ToString("mm:ss.fff") + "   cmdOpenp:" + cmdOpenP.ResponseFormat + "成功标志：" + success.ToString());
        }

        /// <summary>
        /// 执行指令; 指令执行失败时，会重新执行一次
        /// </summary>
        /// <param name="ptl">协议</param>
        /// <param name="cmd">指令</param>
        /// <param name="tag">指令需要数据</param>
        /// <param name="devid">设备地址</param>
        /// <param name="fag">true 返回数据 false 不返回数据</param>
        /// <returns>返回指令相应数据，无表示指令执行失败</returns>
        private string CmdExecute(string ptl, string cmd, string tag, string devid, bool fag)
        {
            bool success = false;
            string responseFormat = string.Empty;
            CmdProtocol cmdOpenP = new CmdProtocol(cmd, false);
            for (int i = 0; i < 2; i++)
            {
                tag = basefun.setvaltag(tag, "设备地址", devid);
                cmdOpenP.setCommand(ptl, cmd, tag);
                CommiManager.GlobalManager.SendCommand(this.targets[devid], cmdOpenP);
                success = cmdOpenP.EventWh.WaitOne(800, false);
                if (success)
                    break;
            }
            if (success)
                responseFormat = cmdOpenP.ResponseFormat;
            else
                responseFormat = string.Empty;
            if (fag)
                return responseFormat;
            else
                return string.Empty;
        }

        /// <summary>
        /// 抓拍图片跟获取服务器的图片
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="pl"></param>
        /// <returns></returns>
        private string CutPicture(DataTable tab, string tag, Panel plIn, Panel plOut)
        {
            string filedest = string.Empty;
            string filepath = string.Empty;
            //在场停车则抓拍出口
            if (0 < videoFlag && DBNull.Value != tab.Rows[0]["入场时间"])
            {
                filedest = string.Empty;
                filepath = this.CutVideo(1, plOut);
                if (!string.IsNullOrEmpty(filepath))
                    filedest = filepath.ToLower().Substring(filepath.IndexOf("image"));
                tag = basefun.setvaltag(tag, "{出场图片}", filedest);
            }
            SendImage(filepath, filedest);
            string filein = Convert.ToString(tab.Rows[0]["入场图片"]);
            this.GetPicServer(filein, plIn);
            return tag;
        }

        private void tmParkInfo_Tick(object sender, EventArgs e)
        {
            //刷新场内停车管理块
            RefreshParkInfo();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmStationWatching_FormClosing(object sender, FormClosingEventArgs e)
        {
            //释放句柄
            Vis.CancelVideo();
            preView.CloseVideo();
            tagData = "结束";
            foreach (CommiTarget target in this.targets.Values)
                CommiManager.GlobalManager.RemoveCommand(target);
        }

        private void btHandOver_Click(object sender, EventArgs e)
        {
            FormUpDownWork win = new FormUpDownWork();
            tagData = "模式对话";
            foreach (CommandBase c in this.cmdDevs)
                c.TimeSendInv = new TimeSpan(365, 0, 0, 0);
            DialogResult rsl = win.ShowDialog();
            foreach (CommandBase c in this.cmdDevs)
                c.TimeSendInv = new TimeSpan(0, 0, 1);
            tagData = "";
            if (DialogResult.Yes != rsl)
                return;
            this.paramSystem = BindManager.getSystemParam();
            this.RefreshParkInfo();
        }

        /// <summary>
        /// 打开一个功能单元
        /// </summary>
        /// <param name="unitName">功能单元</param>
        private void openUnitWin(string unitName)
        {
            NameObjectList ps = BindManager.getSystemParam();
            ps["name"] = unitName;
            BindManager.setTransParam(ps);
            switch (unitName)
            {
                case "入场记录和图像":
                    Granity.granityMgr.ParkMgr.FrmInQueryManage InPick = new FrmInQueryManage();
                    // Granity.granityMgr.ParkMgr.Report.FrmIntVehicleNumberTotal IntVehicleNumberTotal = new Granity.granityMgr.ParkMgr.Report.FrmIntVehicleNumberTotal();
                    InPick.WindowState = FormWindowState.Normal;
                    InPick.Show();
                    break;
                case "出场记录和图像":
                    Granity.parkStation.FrmOutQueryManage CarOutTotal = new Granity.parkStation.FrmOutQueryManage();
                    //   Granity.granityMgr.ParkMgr.Report.FrmCarOutTotal CarOutTotal = new Granity.granityMgr.ParkMgr.Report.FrmCarOutTotal();
                    CarOutTotal.WindowState = FormWindowState.Normal;
                    CarOutTotal.Show();
                    break;
                case "场内停车记录":
                    Granity.granityMgr.ParkMgr.FrmQueryManage QueryManage = new FrmQueryManage();
                    QueryManage.WindowState = FormWindowState.Normal;
                    QueryManage.Show();
                    break;
                case "修改密码":
                    Granity.granityMgr.UserManager.FrmUpdatePass pass = new Granity.granityMgr.UserManager.FrmUpdatePass();
                    pass.ShowDialog();
                    break;
                case "停车场收费统计表":
                    Granity.granityMgr.ParkMgr.Report.FrmParkTotal Total = new Granity.granityMgr.ParkMgr.Report.FrmParkTotal();
                    Total.WindowState = FormWindowState.Normal;
                    Total.Show();
                    break;
                default:
                    return;
            }
        }

        private void btParking_Click(object sender, EventArgs e)
        {
            openUnitWin("场内停车记录");
        }

        private void btParkInOut_Click(object sender, EventArgs e)
        {
            openUnitWin("出场记录和图像");
        }

        private void btFeeQuery_Click(object sender, EventArgs e)
        {
            openUnitWin("停车场收费统计表");
        }

        private void btnOut1_Click_1(object sender, EventArgs e)
        {
            string data = string.Empty;
            data = basefun.setvaltag(data, "设备地址", this.devNumOut);
            data = basefun.setvaltag(data, "{语音命令}", "24");
            data = basefun.setvaltag(data, "{道闸命令}", "02");
            CmdExecute("停车场", "屏显语音道闸", data, this.devNumOut);
        }

        /// <summary>
        /// 由相对路径转换本地路径
        /// </summary>
        /// <param name="pathfile">请求指令中的相对路径</param>
        /// <returns>本地路径</returns>
        private static string getLocalPath(string pathfile)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (string.IsNullOrEmpty(pathfile))
                pathfile = string.Format("images/{0}/{1}.jpg", DateTime.Now.ToString(@"yyyyMM/dd"), Guid.NewGuid());
            if (!string.IsNullOrEmpty(baseDir))
            {
                if (pathfile.StartsWith("~/"))
                    pathfile = pathfile.Replace("~/", baseDir);
                else if (!pathfile.Contains(":"))
                    pathfile = Path.Combine(baseDir, pathfile);
                pathfile = pathfile.Replace("/", "\\");
            }
            string path = Path.GetDirectoryName(pathfile);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return pathfile;
        }

        private void btnModifyPwd_Click(object sender, EventArgs e)
        {
            string data = string.Empty;
            data = basefun.setvaltag(data, "{语音命令}", "04");
            data = basefun.setvaltag(data, "{道闸命令}", "02");
            CmdExecute("停车场", "屏显语音道闸", data, this.devNumIn);
        }

        private void btParkIn_Click(object sender, EventArgs e)
        {
            openUnitWin("入场记录和图像");
        }

        private void btSystemParm_Click(object sender, EventArgs e)
        {
            granityMgr.util.FrmDownParam downParm = new Granity.granityMgr.util.FrmDownParam();
            NameObjectList pstrans = new NameObjectList();
            pstrans["功能单元"] = "参数下载";
            BindManager.setTransParam(pstrans);
            downParm.StartPosition = FormStartPosition.CenterScreen;
            downParm.Show();
        }

        /// <summary>
        /// 改变panel lab 的大小以及相对位置
        /// </summary>
        private void ChangPnlLocationAndSize()
        {
            int endWidth = this.panelVideo.Width - this.strartWidth;
            int endHight = this.panelVideo.Height - this.strartHight;
            int addWidth = endWidth / 2;
            int addHight = endHight / 2;
            Control[] plRight = { p_out1, p_out2, labFirstOut, labSecondOut };
            Control[] plLeft = { p_in1, p_in2, labFirstIn, labSecondIn };
            foreach (Control cl in this.panelVideo.Controls)
            {
                cl.Width = cl.Width + addWidth;
                if (cl is Panel && cl.Name == "p_in1" || cl.Name == "p_in2")
                    cl.Location = new Point(cl.Location.X, cl.Location.Y);
                else
                    cl.Location = new Point(cl.Location.X + addWidth, cl.Location.Y);
            }
            foreach (Control cl in this.panelVideo.Controls)
            {
                cl.Height = cl.Height + addHight;
                if (cl is Panel && cl.Name == "p_in1" || cl.Name == "p_out1")
                    cl.Location = new Point(cl.Location.X, cl.Location.Y);
                else
                    if (cl is LabelControl && cl.Name == "labSecondIn" || cl.Name == "labSecondOut")
                        cl.Location = new Point(cl.Location.X, cl.Location.Y + addHight * 2);
                    else
                        cl.Location = new Point(cl.Location.X, cl.Location.Y + addHight);
            }
            //当前位置修改成下次的初始化值
            this.strartHight = this.panelVideo.Height;
            this.strartWidth = this.panelVideo.Width;
        }

        private void SplitterMoved(object sender, SplitterEventArgs e)
        {
            SplitMove();
        }

        /// <summary>
        /// splitter 控件移动处理程序
        /// </summary>
        private void SplitMove()
        {
            if (videoFlag == 2)
                return;
            ChangPnlLocationAndSize();
            preView.UpdateChannel(0, this.p_in1);
            preView.UpdateChannel(1, this.p_out1);
            preView.UpdateChannel(2, this.p_in2);
            preView.UpdateChannel(3, this.p_out2);
        }

        /// <summary>
        /// pnl 双击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PnlDoubleClick(object sender, EventArgs e)
        {
            Panel pnl = sender as Panel;
            int currentChannel = Convert.ToInt16(pnl.Tag);
            OpenBigVideo(currentChannel, pnl);
        }

        /// <summary>
        /// 双击pnl时的放大图像处理程序
        /// </summary>
        private void OpenBigVideo(int channel, Panel pl)
        {
            if (videoFlag == 2)
                return;
            currentChannel.Clear();
            currentChannel.Add(channel, pl);
            this.panelBigVideo.Visible = true;
            this.panelBigVideo.Dock = DockStyle.Fill;
            foreach (Control cl in this.panelVideo.Controls)
            {
                Panel pnl = cl as Panel;
                if (pnl != null)
                {
                    int i = Convert.ToInt16(pnl.Tag);
                    if (pnl.Name == "panelBigVideo")
                        continue;
                    preView.StopChannel(i, pnl.Handle);
                }
            }
            preView.UpdateChannel(Convert.ToUInt16(channel), this.panelBigVideo);
            panelBigVideo.BackColor = Color.Magenta;
        }

        private void panelVideo_DoubleClick(object sender, EventArgs e)
        {
            OpenSmallVideo();
        }

        /// <summary>
        /// 双击pnl时的缩小图像处理程序
        /// </summary>
        private void OpenSmallVideo()
        {
            if (videoFlag == 2)
                return;
            this.panelBigVideo.Visible = false;
            this.panelBigVideo.Dock = DockStyle.None;
            preView.CloseVideo();
            foreach (string str in Enum.GetNames(typeof(CardType)))
            {
                preView.InitVideo(this.Handle, (VideoType)(Enum.Parse(typeof(CardType), str)));
            }
            foreach (Control cl in this.panelVideo.Controls)
            {
                Panel pnl = cl as Panel;
                if (pnl != null)
                {
                    int m = Convert.ToInt16(pnl.Tag);
                    if (pnl.Name == "panelBigVideo")
                        continue;
                    preView.OpenChannel(m, pnl);
                    pnl.BackColor = Color.Magenta;
                }
            }
        }

        private void timerCardMoney_Tick(object sender, EventArgs e)
        {
            this.CardMoney = string.Empty;
        }

        /// <summary>
        /// 图象显示控件适应不同分辨率自适应大小
        /// </summary>
        private void ReSize()
        {
            int width = this.panelVideo.Width;
            int Height = this.panelVideo.Height;
            int x = 0;
            int y = 0;
            x = (width - 28) / 2;
            y = (Height - 46) / 2;
            this.p_in1.Location = new Point(5, 2);
            this.p_in1.Size = new Size(x, y);
            this.labFirstIn.Location = new Point(this.labFirstIn.Location.X, this.p_in1.Location.Y + y + 4);
            this.p_out1.Location = new Point(x + 18, this.p_out1.Location.Y);
            this.labFirstOut.Location = new Point(this.labFirstOut.Location.X + x - this.p_out1.Size.Width, this.p_out1.Location.Y + y + 4);
            this.p_out1.Size = new Size(x, y);
            this.p_in2.Location = new Point(this.p_in2.Location.X, y + 25);
            this.p_in2.Size = new Size(x, y);
            this.labSecondIn.Location = new Point(this.labSecondIn.Location.X, this.p_in2.Location.Y + y + 4);
            this.p_out2.Location = new Point(x + 18, y + 25);
            this.labSecondOut.Location = new Point(this.labSecondOut.Location.X + x - this.p_out2.Size.Width, this.p_out2.Location.Y + y + 4);
            this.p_out2.Size = new Size(x, y);
        }
    }
}