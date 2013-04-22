using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using System.Net;
using Granity.communications;
using System.IO;
using System.Globalization;
using Granity.CardOneCommi;


namespace Granity.parkStation
{
    public partial class FrmWatchStation : Form
    {
        #region 车牌处理变量

        /// <summary>
        /// 车牌识别监控
        /// </summary>
        private class HvCarWatch
        {
            /// <summary>
            /// 车牌识别
            /// </summary>
            public HvCarDiscern HVdiscern = new HvCarDiscern();
            /// <summary>
            /// 视频更新最后时刻
            /// </summary>
            public DateTime dslast = DateTime.Now;
        }
        /// <summary>
        /// 车牌识别图像监控
        /// </summary>
        private class picHv
        {
            /// <summary>
            /// 视频图像
            /// </summary>
            public PictureBox picVideo;
            /// <summary>
            /// 抓拍大图
            /// </summary>
            public PictureBox picBigImage;
            /// <summary>
            /// 是否享用了视频布,当视频布被占用则视频和大图不显示
            /// </summary>
            public bool shareVideo = false;
            /// <summary>
            /// 对比图(为空不对比,优先使用此显示大图或者回放入口大图)
            /// </summary>
            public picHv pichvCompare;

            /// <summary>
            /// 标签说明
            /// </summary>
            public CheckBox lbHv;
            /// <summary>
            /// 视频更新时间
            /// </summary>
            public DateTime dtVideo = DateTime.Now;
            /// <summary>
            /// 拍照时间
            /// </summary>
            public DateTime dtSpan = DateTime.Now;
        }

        /// <summary>
        /// 当前启动的车牌识别器设备列表
        /// </summary>
        private List<string> hvDevices = new List<string>();
        /// <summary>
        /// 车牌识别器
        /// </summary>
        private Dictionary<string, HvCarWatch> hvWatchs = new Dictionary<string, HvCarWatch>();
        /// <summary>
        /// 监控画面显示
        /// </summary>
        private List<picHv> picWatchs = new List<picHv>();
        /// <summary>
        /// 当前监控图形索引号
        /// </summary>
        private int indexWatch = 0;

        #endregion

        /// <summary>
        /// 刷卡缓存数据(结束/模式对话/data)
        /// </summary>
        string tagData = "";

        /// <summary>
        /// 当前窗口设备通讯目标参数
        /// </summary>
        CommiTarget target = null;
        /// <summary>
        /// 入场设备地址
        /// </summary>
        string devNumIn = "";
        /// <summary>
        /// 出场设备地址
        /// </summary>
        string devNumOut = "";
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
        /// 执行数据处理的Query,由单元初始化
        /// </summary>
        QueryDataRes Query = null;
        NameObjectList paramSystem = null;
        /// <summary>
        /// 是否已经系统初始化，在加载完毕后定时器事件上执行
        /// </summary>
        bool isInitLoad = false;

        public FrmWatchStation()
        {
            InitializeComponent();
        }

        private void FrmWatchStation_Load(object sender, EventArgs e)
        {
            //监控图形
            picHv pichv = new picHv();
            pichv.picVideo = this.picVideoIn1;
            pichv.picBigImage = this.picBigImageIn1;
            pichv.lbHv = this.ckbIn1;
            this.picWatchs.Add(pichv);
            pichv = new picHv();
            pichv.picVideo = this.picVideoOut1;
            pichv.picBigImage = this.picBigImageOut1;
            pichv.lbHv = this.ckbOut1;
            this.picWatchs.Add(pichv);
            pichv = new picHv();
            pichv.picVideo = this.picVideoIn2;
            pichv.picBigImage = this.picBigImageIn2;
            pichv.lbHv = this.ckbIn2;
            this.picWatchs.Add(pichv);
            pichv = new picHv();
            pichv.picVideo = this.picVideoOut2;
            pichv.picBigImage = this.picBigImageOut2;
            pichv.lbHv = this.ckbOut2;
            this.picWatchs.Add(pichv);
            foreach (picHv p in this.picWatchs)
                p.picBigImage.Visible = false;

            this.UnitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), "收费站");
            //初始化下拉框字典
            this.Query = new QueryDataRes(this.UnitItem.DataSrcFile);
            this.paramSystem = BindManager.getSystemParam();
            DataSet ds = new DataSet("字典");
            this.Query.FillDataSet("卡片类型", this.paramSystem, ds);
            this.Query.FillDataSet("卡片类型", this.paramSystem, ds);
            DataRow dr = ds.Tables["卡片类型"].NewRow();
            ds.Tables["卡片类型"].Rows.InsertAt(dr, 0);
            dr = ds.Tables["卡片类型"].NewRow();
            ds.Tables["卡片类型"].Rows.InsertAt(dr, 0);

            this.cbbCardType.DataSource = ds.Tables["卡片类型"];
            this.cbbCardType.DisplayMember = "卡类";
            this.cbbCardType.ValueMember = "编号";
            this.cbbCarType.DataSource = ds.Tables["车型"];
            this.cbbCarType.DisplayMember = "车类";
            this.cbbCarType.ValueMember = "编号";

            this.gdGrid.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dbgrid_RowPostPaint);
            CommiManager.GlobalManager.ErrorOpenHandle += new EventHandler<ErrorCommiEventArgs>(GlobalManager_ErrorOpenHandle);
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
        /// 从设备表格找到指定IP地址和端口的行记录
        /// </summary>
        /// <param name="ipaddr">IP地址</param>
        /// <param name="comport">COM端口</param>
        /// <param name="addrst">设备站址</param>
        /// <returns>表格行记录</returns>
        private DataGridViewRow getRow(string ipaddr, string comport, string addrst)
        {
            foreach (DataGridViewRow dr in this.gdGrid.Rows)
            {
                string ip = Convert.ToString(dr.Cells["IP地址"].Value);
                string com = Convert.ToString(dr.Cells["端口"].Value);
                string addr = Convert.ToString(dr.Cells["通讯站址"].Value);
                if (ipaddr == ip && comport == com && addrst == addr)
                    return dr;
            }
            return null;
        }

        /// <summary>
        /// 更加tag标记值更新控件内控件值,有下拉框则添加对应“名称”值
        /// </summary>
        /// <param name="ct">控件容器</param>
        /// <param name="tag">tag格式数据</param>
        /// <param name="keyName">tag标记映射标记名称</param>
        /// <returns>返回tag格式数据</returns>
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
                if (!(ct is ListControl))
                    ct.Text = v;
                else
                {
                    ListControl cbb = ct as ListControl;
                    try { cbb.SelectedValue = v; }
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
        /// 刷新搜索网络上的(车牌识别器)
        /// </summary>
        private void searchHv()
        {
            this.gdGrid.Rows.Clear();
            uint count = 0;
            unsafe
            {
                int result = HVDLLFun.SearchHVDeviceCount(&count);
                for (int i = 0; i < count; i++)
                {
                    UInt64 addrMac = 0;
                    UInt32 addrIP = 0, addrMask = 0, addrGateway = 0;
                    result = HVDLLFun.GetHVDeviceAddr(i, &addrMac, &addrIP, &addrMask, &addrGateway);
                    if (addrMac < 1 || addrIP < 1 || addrMask < 1 || addrGateway < 1)
                        continue;
                    //其中物理地址高字节在前需要反序处理
                    //表格字段顺序：设备类型, 设备名称, IP地址, 端口, 通讯类别, 通讯站址, 物理地址
                    string[] ipAddrs = new string[7];
                    ipAddrs[0] = "车牌识别器";
                    ipAddrs[1] = "Hv识别器";
                    ipAddrs[2] = Convert.ToString((long)addrIP, 16).PadLeft(8, '0');
                    ipAddrs[3] = "";
                    ipAddrs[4] = "UDP";
                    ipAddrs[5] = "";
                    ipAddrs[6] = Convert.ToString((long)addrMac, 16).PadLeft(12, '0') + "-";
                    //物理地址
                    for (int k = ipAddrs[6].Length - 4; k > -1; k = k - 2)
                        ipAddrs[6] = ipAddrs[6].Substring(0, k) + ipAddrs[6].Substring(k + 2) + "-" + ipAddrs[6].Substring(k, 2);
                    for (int s = 2; s < ipAddrs.Length - 1; s++)
                    {
                        if (2 < s && s < 6)
                            continue;
                        for (int k = ipAddrs[s].Length - 2; k > -1; k = k - 2)
                        {
                            string val = Convert.ToUInt16(ipAddrs[s].Substring(k, 2), 16).ToString();
                            ipAddrs[s] = ipAddrs[s].Substring(0, k) + "." + val + ipAddrs[s].Substring(k + 2);
                        }
                        if (ipAddrs[s].StartsWith("."))
                            ipAddrs[s] = ipAddrs[s].Substring(1);
                    }
                    ipAddrs[1] = "Hv-" + ipAddrs[2];
                    //这个位置换成通讯状态,初始为空
                    ipAddrs[6] = "";
                    DataGridViewRow drHv = this.getRow(ipAddrs[2], ipAddrs[3], ipAddrs[5]);
                    if (null != drHv)
                        drHv.SetValues(ipAddrs);
                    else
                        this.gdGrid.Rows.Add(ipAddrs);
                }
            }
        }
        /// <summary>
        /// 搜索停车场验票机
        /// </summary>
        private void searchPark()
        {
            QueryDataRes query = new QueryDataRes("cardone");
            NameObjectList ps = new NameObjectList();
            DataTable tab = query.getTable("devlist", ps);
            if (null == tab || tab.Rows.Count < 1)
                return;
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                DataRow dr = tab.Rows[i];
                string commitype = Convert.ToString(dr["访问方式"]);
                if ("1" == commitype) commitype = "Ser";
                if ("2" == commitype) commitype = "TCP";
                if ("Ser" != commitype && "TCP" != commitype)
                    commitype = "UDP";
                string port = Convert.ToString(dr["端口"]);
                if ("Ser" == commitype)
                    port = "COM" + port;
                //表格字段顺序：设备类型, 设备名称, IP地址, 端口, 通讯类别, 通讯站址, 物理地址
                string[] ipAddrs = new string[7];
                ipAddrs[0] = "停车场验票机";
                ipAddrs[1] = Convert.ToString(dr["名称"]);
                ipAddrs[2] = Convert.ToString(dr["地址"]);
                ipAddrs[3] = port;
                ipAddrs[4] = commitype;
                ipAddrs[5] = Convert.ToString(dr["设备地址"]);
                ipAddrs[6] = "";
                DataGridViewRow drPark = this.getRow(ipAddrs[2], port, ipAddrs[5]);
                if (null != drPark)
                    drPark.SetValues(ipAddrs);
                else
                    this.gdGrid.Rows.Add(ipAddrs);
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

            string tpl = "停车场", cmd = "收集下一条记录";
            foreach (DataGridViewRow dr in this.gdGrid.Rows)
            {
                string devname = Convert.ToString(dr.Cells["设备名称"].Value);
                string ipaddr = Convert.ToString(dr.Cells["IP地址"].Value);
                string devicetype = Convert.ToString(dr.Cells["设备类型"].Value);
                string commitype = Convert.ToString(dr.Cells["通讯类别"].Value);
                string port = Convert.ToString(dr.Cells["端口"].Value);
                string addrst = Convert.ToString(dr.Cells["通讯站址"].Value);
                //车牌监控
                if ("车牌识别器" == devicetype && this.hvWatchs.Count < this.picWatchs.Count / 2)
                {
                    HvCarWatch hv = new HvCarWatch();
                    string msg = hv.HVdiscern.Open(ipaddr);
                    if (!string.IsNullOrEmpty(msg))
                        continue;
                    hv.dslast = DateTime.Now;
                    this.hvWatchs.Add(devname, hv);
                    this.hvDevices.Add(devname);

                    string info = basefun.setvaltag("", "devname", devname);
                    info = basefun.setvaltag(info, "ipaddr", ipaddr);
                    this.picWatchs[this.indexWatch * 2].lbHv.Text = "入口：" + devname;
                    this.picWatchs[this.indexWatch * 2].lbHv.Enabled = true;
                    this.picWatchs[this.indexWatch * 2].lbHv.Checked = true;
                    this.picWatchs[this.indexWatch * 2].lbHv.Tag = info;
                    this.picWatchs[this.indexWatch * 2 + 1].lbHv.Text = "出口：" + devname;
                    this.picWatchs[this.indexWatch * 2 + 1].lbHv.Enabled = true;
                    this.picWatchs[this.indexWatch * 2 + 1].lbHv.Checked = true;
                    this.picWatchs[this.indexWatch * 2 + 1].lbHv.Tag = info;
                    this.indexWatch++;
                    continue;
                }
                if ("停车场验票机" != devicetype || string.IsNullOrEmpty(ipaddr))
                    continue;
                if ("Ser" == commitype && !ipstr.Contains(ipaddr))
                    continue;
                if (string.IsNullOrEmpty(addrst))
                    continue;

                //设置命令定时采集通讯
                CommiTarget target = null;
                if ("Ser" == commitype)
                    target = new CommiTarget(port, 19200);
                if ("UDP" == commitype)
                    target = new CommiTarget(ipaddr, Convert.ToInt16(port), CommiType.UDP);
                if ("TCP" == commitype)
                    target = new CommiTarget(ipaddr, Convert.ToInt16(port), CommiType.TCP);
                int addrdev = Convert.ToInt16(addrst);
                if (addrdev < 129)
                    this.devNumIn = addrst;
                else
                    this.devNumOut = addrst;
                string tagdata = "@设备地址=" + addrst;
                CmdProtocol cmdP = new CmdProtocol(devname + "(" + addrst + ")");
                cmdP.setCommand(tpl, cmd, tagdata);
                cmdP.Tag = tagdata;
                target.setProtocol(Protocol.PTLPark);

                //持续间隔发送通过监听发现接收数据不正确
                cmdP.TimeFailLimit = TimeSpan.MaxValue;
                cmdP.TimeLimit = TimeSpan.MaxValue;
                cmdP.TimeSendInv = new TimeSpan(0, 0, 1);

                //改为接收响应后再发起指令的方式
                this.target = target;
                cmdP.ResponseHandle += new EventHandler<ResponseEventArgs>(cmdP_ResponseHandle);
                CommiManager.GlobalManager.SendCommand(target, cmdP);
                this.cmdDevs.Add(cmdP);
            }
        }
        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="pichv"></param>
        private void Snap(picHv pichv)
        {
            if (null == pichv || null == pichv.picVideo.Image)
                return;
            if (null != pichv.pichvCompare)
            {
                pichv.picBigImage.Visible = false;
                pichv.pichvCompare.picVideo.Image = pichv.picVideo.Image.Clone() as Image;
            }
            else
            {
                pichv.picBigImage.Visible = true;
                pichv.picBigImage.Image = pichv.picVideo.Image.Clone() as Image;
            }
            pichv.dtSpan = DateTime.Now;
        }
        /// <summary>
        /// 刷新场内停车
        /// </summary>
        private void RefreshParkInfo()
        {
            DataTable tab = this.Query.getTable("场内停车信息", this.paramSystem);
            string tag = "设备地址" + this.devNumIn;
            if (null != tab && tab.Rows.Count > 0)
            {
                DataRow dr = tab.Rows[0];
                foreach (DataColumn c in tab.Columns)
                    tag = basefun.setvaltag(tag, "{" + c.ColumnName + "}", Convert.ToString(dr[c]));
            }
            this.setContainerTagValue(this.grpParkInfo, tag, "pm");
        }
        /// <summary>
        /// 存入改组设备的图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="pichv"></param>
        private void SaveImageHv(string id, picHv pichv)
        {
            if (string.IsNullOrEmpty(id) || null == pichv || !pichv.picVideo.Visible || null == pichv.picVideo.Image)
                return;
            Image img = pichv.picBigImage.Image;
            if ((!pichv.picBigImage.Visible || null == img) && null != pichv.pichvCompare && pichv.pichvCompare.shareVideo)
                img = pichv.pichvCompare.picVideo.Image.Clone() as Image;
            else if ((!pichv.picBigImage.Visible || null == img))
                img = pichv.picVideo.Image.Clone() as Image;
            BindManager.SaveImage(new Guid(id), img);
        }
        /// <summary>
        /// 执行业务入场或出场,无指定开闸模式则不开闸
        /// </summary>
        /// <param name="tag">出入场数据,tag格式数据</param>
        /// <param name="display">是否显示数据信息</param>
        private void ExecuteInOut(string tag, bool display)
        {
            //设置通讯结果：
            string devid = basefun.valtag(tag, "{设备地址}");
            string cardnum = basefun.valtag(tag, "{卡号}");
            string dtparkin = basefun.valtag(tag, "{入场时间}");
            string dtparkout = basefun.valtag(tag, "{出入场时间}");
            string openmode = basefun.valtag(tag, "{开闸方式}");
            string strindex = basefun.valtag(tag, "picindex");
            int picindex = string.IsNullOrEmpty(strindex) ? -1 : Convert.ToInt32(strindex);
            if (dtparkin == dtparkout || string.IsNullOrEmpty(dtparkout))
                tag = basefun.setvaltag(tag, "{出入场时间}", "");
            else
            {
                DateTime dtOut = Convert.ToDateTime(dtparkout);
                DateTime dtIn = Convert.ToDateTime(dtparkin);
                TimeSpan tmSpan = dtOut - dtIn;
                tag = basefun.setvaltag(tag, "{停车时间}", tmSpan.ToString());
            }
            if (display)
            {
                tag = this.setContainerTagValue(this.grpInfoM, tag, "pm");
                tag = this.setContainerTagValue(this.grpInfoDetail, tag, "pm");
            }
            //非44开闸的出入场数据处理
            if (string.IsNullOrEmpty(openmode))
                return;
            string imgID = Guid.NewGuid().ToString();
            if (dtparkin == dtparkout || string.IsNullOrEmpty(dtparkout) || "44" != openmode)
            {
                NameObjectList ps = ParamManager.createParam(tag);
                ParamManager.MergeParam(ps, this.paramSystem, false);
                if (dtparkin == dtparkout || string.IsNullOrEmpty(dtparkout))
                {
                    //默认抓拍视频
                    if (picindex < 0)
                        picindex = this.picWatchs[0].lbHv.Checked ? 0 : 2;
                    if (!this.picWatchs[picindex].lbHv.Checked || this.picWatchs[picindex].shareVideo)
                        imgID = "";
                    ps["入场图片路径"] = imgID;
                    this.Query.ExecuteNonQuery("入场登记", ps, ps, ps);
                }
                else
                {
                    //默认抓拍视频
                    if (picindex < 0)
                        picindex = this.picWatchs[1].lbHv.Checked ? 1 : 3;
                    if (!this.picWatchs[picindex].lbHv.Checked || this.picWatchs[picindex].shareVideo)
                        imgID = "";
                    ps["出场图片路径"] = imgID;
                    this.Query.ExecuteNonQuery("出场登记", ps, ps, ps);
                }
                if (!String.IsNullOrEmpty(imgID) && picindex < this.picWatchs.Count)
                    this.SaveImageHv(imgID, this.picWatchs[picindex]);
                this.RefreshParkInfo();
                return;
            }
            tagData = "模式对话";
            //收费开闸及重新启动巡检
            FrmFee win = new FrmFee();
            win.DataTag = tag;
            DialogResult rsl = win.ShowDialog();
            while (DialogResult.Yes == rsl || DialogResult.No == rsl)
            {
                if (DialogResult.No == rsl)
                {
                    tag = basefun.setvaltag(tag, "{收费金额}", "0");
                    this.setContainerTagValue(this.grpInfoM, tag, "pm");
                }
                string cmd = "管理系统远程闸控制";
                string tpl = "停车场";
                string cmdData = "@设备地址=" + devid;
                cmdData = basefun.setvaltag(cmdData, "{道闸命令}", "01");
                CmdProtocol cmdP = new CmdProtocol(cmd, false);
                cmdP.setCommand(tpl, cmd, cmdData);
                CommiManager.GlobalManager.SendCommand(this.target, cmdP);
                if (!cmdP.EventWh.WaitOne(5000, false))
                {
                    MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    rsl = win.ShowDialog();
                }
                else
                    rsl = DialogResult.Cancel;
            }
            foreach (CommandBase c in this.cmdDevs)
                c.TimeSendInv = new TimeSpan(0, 0, 1);
            NameObjectList psOut = ParamManager.createParam(tag);
            ParamManager.MergeParam(psOut, this.paramSystem, false);
            if (picindex < 0)
                picindex = this.picWatchs[1].lbHv.Checked ? 1 : 3;
            if (!this.picWatchs[picindex].lbHv.Checked || this.picWatchs[picindex].shareVideo)
                imgID = "";
            psOut["出场图片路径"] = imgID;
            this.Query.ExecuteNonQuery("出场登记", psOut, psOut, psOut);
            if (!String.IsNullOrEmpty(imgID) && picindex < this.picWatchs.Count)
                this.SaveImageHv(imgID, this.picWatchs[picindex]);
            //模式对话结束恢复
            tagData = "";
            this.RefreshParkInfo();
        }

        /// <summary>
        /// 有刷卡，读取卡信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdP_ResponseHandle(object sender, ResponseEventArgs e)
        {
            CmdProtocol cmd = sender as CmdProtocol;
            if ("结束" == this.tagData || null == cmd)
                return;
            CommiTarget commipm = e.Target;
            string station = basefun.valtag(Convert.ToString(cmd.Tag), "设备地址");
            switch (commipm.ProtocolType)
            {
                case CommiType.TCP:
                case CommiType.UDP:
                    string ip = commipm.SrvEndPoint.ToString();
                    error[ip] = "通讯正常";
                    if (!e.Success)
                        error[ip] = "通讯指令异常";
                    break;
                case CommiType.SerialPort:
                    string portname = commipm.PortName + ":" + station;
                    error[portname] = e.Success ? "通讯正常" : "通讯指令异常";
                    break;
            }
            if (!e.Success) return;
            if (!string.IsNullOrEmpty(cmd.ResponseFormat))
            {
                //开闸方式管理机开闸时咱停巡检,收费交互后重新启动
                string tag = cmd.ResponseFormat;
                if ("44" == basefun.valtag(tag, "{开闸方式}"))
                {
                    //出入场时间相同则是入场,入场不限定
                    string dtparkin = basefun.valtag(tag, "{入场时间}");
                    string dtparkout = basefun.valtag(tag, "{出入场时间}");
                    if (dtparkin != dtparkout)
                    {
                        foreach (CommandBase c in this.cmdDevs)
                            c.TimeSendInv = new TimeSpan(365, 0, 0, 0);
                    }
                }
                tagData = basefun.setvaltag(tag, "设备名称", cmd.CmdId);
            }
        }

        /// <summary>
        /// 双击打开监控画面,两个画面都已经打开则循环关闭启用新画面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gdGrid_DoubleClick(object sender, EventArgs e)
        {
            if (null == sender || !(sender is DataGridView))
                return;
            DataGridView gd = sender as DataGridView;
            if (gd.Rows.Count < 1 || null == gd.CurrentRow)
                return;
            DataGridViewRow dr = gd.CurrentRow;
            string devname = Convert.ToString(dr.Cells["设备名称"].Value);
            string devtype = Convert.ToString(dr.Cells["设备类型"].Value);
            string commitype = Convert.ToString(dr.Cells["通讯类别"].Value);
            string ipaddr = Convert.ToString(dr.Cells["IP地址"].Value);
            if (this.hvWatchs.ContainsKey(devname))
                return;
            if ("车牌识别器" != devtype)
                return;
            HvCarWatch hv = new HvCarWatch();
            string msg = hv.HVdiscern.Open(ipaddr);
            if (!string.IsNullOrEmpty(msg))
            {
                MessageBox.Show(msg);
                return;
            }
            hv.dslast = DateTime.Now;
            //多个监控图形，可循环选择使用
            if (this.hvDevices.Count > this.picWatchs.Count / 2 - 1)
            {
                if (this.indexWatch > this.hvDevices.Count - 1)
                    this.indexWatch = 0;
                string key = this.hvDevices[this.indexWatch];
                this.hvWatchs[key].HVdiscern.Close();
                this.hvWatchs.Remove(key);
                this.hvDevices.RemoveAt(this.indexWatch);
                this.hvDevices.Insert(this.indexWatch, devname);
            }
            else
                this.hvDevices.Add(devname);
            this.hvWatchs.Add(devname, hv);
            string info = basefun.setvaltag("", "devname", devname);
            info = basefun.setvaltag(info, "ipaddr", ipaddr);
            this.picWatchs[this.indexWatch * 2].lbHv.Text = "入口：" + devname;
            this.picWatchs[this.indexWatch * 2].lbHv.Enabled = true;
            this.picWatchs[this.indexWatch * 2].lbHv.Tag = info;
            this.picWatchs[this.indexWatch * 2 + 1].lbHv.Text = "出口：" + devname;
            this.picWatchs[this.indexWatch * 2 + 1].lbHv.Enabled = true;
            this.picWatchs[this.indexWatch * 2 + 1].lbHv.Tag = info;
            this.indexWatch = this.picWatchs.Count / 2 - 1 == this.indexWatch ? 0 : this.indexWatch + 1;
        }
        /// <summary>
        /// 更新视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmVideo_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < this.hvDevices.Count; i++)
            {
                string dev = this.hvDevices[i];
                HvCarWatch hv = this.hvWatchs[dev];
                for (int k = 0; k < 2; k++)
                {
                    if (this.picWatchs.Count < i * 2 + k + 1)
                        continue;
                    picHv pichv = this.picWatchs[i * 2 + k];
                    if (pichv.dtSpan.AddSeconds(5) < DateTime.Now)
                    {
                        if (null != pichv.pichvCompare)
                            pichv.pichvCompare.shareVideo = false;
                        if (pichv.picBigImage.Visible)
                            pichv.picBigImage.Visible = false;
                    }
                    HvVideoInfo info = hv.HVdiscern.getVideoInfo(k);
                    if (!(hv.dslast < info.DtVideo) || info.VideoSize < 1)
                        continue;
                    if (pichv.shareVideo || !pichv.lbHv.Checked)
                        continue;
                    if (!pichv.picVideo.Visible)
                        pichv.picVideo.Visible = true;
                    hv.dslast = pichv.dtVideo = info.DtVideo;
                    MemoryStream streamVideo = hv.HVdiscern.getStreamVideo(k);
                    if (null != streamVideo)
                        pichv.picVideo.Image = Image.FromStream(streamVideo);
                }
            }
        }
        /// <summary>
        /// 识别车牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmHvCardisc_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < this.hvDevices.Count; i++)
            {
                string dev = this.hvDevices[i];
                HvCarWatch hv = this.hvWatchs[dev];
                for (int k = 0; k < 2; k++)
                {
                    if (this.picWatchs.Count < i * 2 + k + 1)
                        continue;
                    picHv pichv = this.picWatchs[i * 2 + k];
                    HvCarPlateInfo info = hv.HVdiscern.getCarPlateInfo(k);
                    if (string.IsNullOrEmpty(info.CarNum))
                    {
                        if (pichv.dtSpan.AddSeconds(5) > DateTime.Now)
                            continue;
                        if (null != pichv.pichvCompare)
                            pichv.pichvCompare.shareVideo = false;
                        if (pichv.picBigImage.Visible)
                            pichv.picBigImage.Visible = false;
                        continue;
                    }
                    hv.HVdiscern.ResetInfo(k);
                    if (!pichv.lbHv.Checked)
                        continue;
                    string carNum = info.CarNum;
                    Image image = Image.FromStream(new MemoryStream(info.BigImage));
                    DateTime dtspan = DateTime.Now;
                    //核对车牌
                    NameObjectList ps = new NameObjectList();
                    ps["车牌号码"] = carNum;
                    string tag = "@{车牌号码}=" + carNum;
                    DataTable tab = this.Query.getTable("车牌号码在场记录", ps);
                    DataRow dr = (null == tab || tab.Rows.Count < 1) ? null : tab.Rows[0];
                    //抓拍图片(出场口只有tagData空时抓拍)
                    if (null != pichv.pichvCompare)
                    {
                        if (0 == k || (null != dr && DBNull.Value != dr["入场时间"] && 1 == k))
                            pichv.pichvCompare.shareVideo = true;
                        else
                            pichv.pichvCompare.shareVideo = false;
                        if (0 == k)
                            pichv.pichvCompare.picVideo.Image = image;
                        else if (null != dr && DBNull.Value != dr["入场时间"] && string.IsNullOrEmpty(tagData))
                        {
                            string id = Convert.ToString(dr["入场图片"]);
                            byte[] bytes = BindManager.getImage(id);
                            Image imgcompare = (null == bytes || bytes.Length < 1) ? null : Image.FromStream(new MemoryStream(bytes));
                            pichv.pichvCompare.picVideo.Image = imgcompare;
                        }
                    }
                    if (null == pichv.pichvCompare || 1 == k)
                    {
                        pichv.picBigImage.Image = image;
                        pichv.picBigImage.Visible = true;
                        pichv.dtSpan = DateTime.Now;
                    }
                    //无效车牌(模式对话时只抓拍显示忽略其他处理，非模式对话时提取车牌)
                    if (null == tab || tab.Rows.Count < 1)
                    {
                        if ("模式对话" == tagData || this.txtCar_ID.Text == carNum)
                            continue;
                        this.txtCar_ID.Text = carNum;
                        this.txtCar_ID.Tag = basefun.setvaltag(Convert.ToString(this.txtCar_ID.Tag), "picindex", Convert.ToString(i * 2 + k));
                        if (string.IsNullOrEmpty(tagData))
                            this.ExecuteInOut(tag, true);
                        continue;
                    }
                    //模式对话时，入口可继续工作，出口停止工作
                    if ("模式对话" == tagData && 1 == k)
                        continue;
                    //车牌有效
                    this.txtCar_ID.Text = carNum;
                    foreach (DataColumn c in tab.Columns)
                        tag = basefun.setvaltag(tag, "{" + c.ColumnName + "}", Convert.ToString(dr[c]));
                    tag = basefun.setvaltag(tag, "{设备地址}", this.devNumOut);
                    tag = basefun.setvaltag(tag, "picindex", Convert.ToString(i * 2 + k));
                    string tagOpen = basefun.valtag(tag, "{开闸方式}");
                    //在入口进场或出口出场且不是44方式，开闸
                    if ("44" != tagOpen && ((DBNull.Value == dr["入场时间"] && 0 == k) || (DBNull.Value != dr["入场时间"] && 1 == k)))
                    {
                        //没有入场记录,则入场口允许入场
                        string tpl = "停车场";
                        string cmd = "管理系统远程闸控制";
                        if (0 == k)
                            tag = basefun.setvaltag(tag, "{入场时间}", DateTime.Now.ToString());
                        tag = basefun.setvaltag(tag, "{设备地址}", 1 == k ? this.devNumOut : this.devNumIn);

                        string cmdData = "@设备地址=" + (1 == k ? this.devNumOut : this.devNumIn);
                        cmdData = basefun.setvaltag(cmdData, "{道闸命令}", "01");
                        CmdProtocol cmdP = new CmdProtocol(cmd, false);
                        cmdP.setCommand(tpl, cmd, cmdData);
                        CommiManager.GlobalManager.SendCommand(this.target, cmdP);
                        if (!cmdP.EventWh.WaitOne(5000, false))
                        {
                            tag = basefun.setvaltag(tag, "{开闸方式}", "");
                            MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                    //已经入场记录,则出场口允许出场
                    //在入场口出场或在出场口入场,只抓拍和显示不开闸
                    if (DBNull.Value != dr["入场时间"] && 1 == k)
                        tag = basefun.setvaltag(tag, "{出入场时间}", DateTime.Now.ToString());
                    else if (!(DBNull.Value == dr["入场时间"] && 0 == k))
                        tag = basefun.setvaltag(tag, "{开闸方式}", "");
                    if ("模式对话" == tagData)
                        this.ExecuteInOut(tag, false);
                    else
                        this.ExecuteInOut(tag, true);
                }
            }
        }
        /// <summary>
        /// 刷新卡信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmCard_Tick(object sender, EventArgs e)
        {
            if (!this.isInitLoad)
            {
                //搜索车牌硬识别设备
                this.searchHv();
                //获取停车场设备
                this.searchPark();
                this.Monitor();
                this.RefreshParkInfo();
                this.isInitLoad = true;
                this.setPichvCompare();
                return;
            }
            //更新异常信息
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string ipstr = ",";
            foreach (IPAddress ip in IpEntry.AddressList)
                ipstr += ip + ",";
            foreach (DataGridViewRow gdr in this.gdGrid.Rows)
            {
                string ip = Convert.ToString(gdr.Cells["IP地址"].Value);
                string portname = Convert.ToString(gdr.Cells["端口"].Value);
                string station = Convert.ToString(gdr.Cells["通讯站址"].Value);
                if (error.ContainsKey(ip + ":" + portname))
                {
                    gdr.Cells["通讯状态"].Value = error[ip];
                    continue;
                }
                if (!ipstr.Contains("," + ip + ","))
                    continue;
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
            //核对车牌
            tag = basefun.setvaltag(tag, "{车牌号码}", this.txtCar_ID.Text);
            //查找卡信息
            NameObjectList ps = new NameObjectList();
            ps["刷卡号码"] = cardnum;
            DataTable tab = this.Query.getTable("刷卡号码在场记录", ps);
            DataRow dr = (null == tab || tab.Rows.Count < 1) ? null : tab.Rows[0];
            if (null != dr)
            {
                tag = basefun.setvaltag(tag, "{姓名}", Convert.ToString(dr["姓名"]));
                string carnum = Convert.ToString(dr["车牌号码"]);
                if (!string.IsNullOrEmpty(carnum))
                {
                    this.txtCar_ID.Text = carnum;
                    tag = basefun.setvaltag(tag, "{车牌号码}", carnum);
                }
            }
            string picindex = basefun.valtag(Convert.ToString(this.txtCar_ID.Tag), "picindex");
            //判断是否抓拍
            string devid = basefun.valtag(tag, "{设备地址}");
            int idevid = Convert.ToInt16(devid);
            if (idevid < 129 && (string.IsNullOrEmpty(picindex) || "1" == picindex || "3" == picindex))
                picindex = "3" == picindex ? "2" : "0";
            else if (idevid > 128 && (string.IsNullOrEmpty(picindex) || "0" == picindex || "2" == picindex))
                picindex = "2" == picindex ? "3" : "1";
            int ipic = 0;
            try { ipic = Convert.ToInt16(picindex); }
            catch { }
            if (!this.picWatchs[ipic].lbHv.Checked)
            {
                ipic = ipic > 1 ? ipic - 2 : ipic + 2;
                picindex = Convert.ToString(ipic);
            }
            this.txtCar_ID.Tag = tag = basefun.setvaltag(tag, "picindex", picindex);
            if (this.picWatchs.Count > ipic && !this.picWatchs[ipic].shareVideo)
            {
                picHv pichv = this.picWatchs[ipic];
                if ((null == pichv.pichvCompare && !pichv.picBigImage.Visible)
                    || (null != pichv.pichvCompare && !pichv.pichvCompare.shareVideo))
                {
                    if (null == pichv.pichvCompare || 1 == ipic || 3 == ipic)
                    {
                        pichv.picBigImage.Visible = true;
                        pichv.picBigImage.Image = pichv.picVideo.Image.Clone() as Image;
                    }
                    else
                    {
                        pichv.pichvCompare.shareVideo = true;
                        pichv.pichvCompare.picVideo.Image = pichv.picVideo.Image.Clone() as Image;
                    }
                    pichv.dtSpan = DateTime.Now.AddSeconds(-4);
                    //出口则显示对比图
                    if ((1 == ipic || 3 == ipic) && null != dr && DBNull.Value != dr["入场时间"] && null != this.picWatchs[ipic].pichvCompare)
                    {
                        string id = Convert.ToString(dr["入场图片"]);
                        byte[] bytes = BindManager.getImage(id);
                        Image imgcompare = (null == bytes || bytes.Length < 1) ? null : Image.FromStream(new MemoryStream(bytes));
                        this.picWatchs[ipic].pichvCompare.shareVideo = true;
                        pichv.pichvCompare.picVideo.Image = imgcompare;
                    }
                }
            }

            //执行业务
            this.ExecuteInOut(tag, true);
        }


        private void FrmWatchStation_FormClosing(object sender, FormClosingEventArgs e)
        {
            tagData = "结束";
            CommiManager.GlobalManager.ClearCommand();
            foreach (string dev in this.hvDevices)
                this.hvWatchs[dev].HVdiscern.Close();
        }
        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIn1_Click(object sender, EventArgs e)
        {
            this.Snap(this.picWatchs[0]);
        }
        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOut1_Click(object sender, EventArgs e)
        {
            this.Snap(this.picWatchs[1]);
        }
        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnIn2_Click(object sender, EventArgs e)
        {
            this.Snap(this.picWatchs[2]);
        }
        /// <summary>
        /// 抓拍
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOut2_Click(object sender, EventArgs e)
        {
            this.Snap(this.picWatchs[3]);
        }

        private void tmParkInfo_Tick(object sender, EventArgs e)
        {
            this.RefreshParkInfo();
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

        private void setPichvCompare()
        {
            if (!this.isInitLoad) return;
            bool chk0 = this.picWatchs[0].lbHv.Checked;
            bool chk1 = this.picWatchs[1].lbHv.Checked;
            bool chk2 = this.picWatchs[2].lbHv.Checked;
            bool chk3 = this.picWatchs[3].lbHv.Checked;
            string devname = "", ipaddr = "";
            //两者关闭
            if (!chk0 && !chk1)
            {
                devname = basefun.valtag(Convert.ToString(this.picWatchs[0].lbHv.Tag), "devname");
                if (this.hvWatchs.ContainsKey(devname))
                    this.hvWatchs[devname].HVdiscern.Close();
                this.picWatchs[2].pichvCompare = this.picWatchs[0];
                this.picWatchs[3].pichvCompare = this.picWatchs[1];
            }
            if (!chk2 && !chk3)
            {
                devname = basefun.valtag(Convert.ToString(this.picWatchs[2].lbHv.Tag), "devname");
                if (this.hvWatchs.ContainsKey(devname))
                    this.hvWatchs[devname].HVdiscern.Close();
                this.picWatchs[0].pichvCompare = this.picWatchs[2];
                this.picWatchs[1].pichvCompare = this.picWatchs[3];
            }
            if (chk0 && chk1)
            {
                if (chk2)
                    this.picWatchs[0].pichvCompare = null;
                else
                    this.picWatchs[0].pichvCompare = this.picWatchs[2];
                if (chk3)
                    this.picWatchs[1].pichvCompare = null;
                else
                    this.picWatchs[1].pichvCompare = this.picWatchs[3];
            }
            if (chk2 && chk3)
            {
                if (chk0)
                    this.picWatchs[2].pichvCompare = null;
                else
                    this.picWatchs[2].pichvCompare = this.picWatchs[0];
                if (chk1)
                    this.picWatchs[3].pichvCompare = null;
                else
                    this.picWatchs[3].pichvCompare = this.picWatchs[1];
            }
            if ((chk0 && !chk1) || (!chk0 && chk1))
            {
                devname = basefun.valtag(Convert.ToString(this.picWatchs[0].lbHv.Tag), "devname");
                ipaddr = basefun.valtag(Convert.ToString(this.picWatchs[0].lbHv.Tag), "ipaddr");
                if (this.hvWatchs.ContainsKey(devname))
                {
                    string msg = this.hvWatchs[devname].HVdiscern.Open(ipaddr);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        MessageBox.Show(msg, "视频提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.picWatchs[0].lbHv.Checked = false;
                        this.picWatchs[1].lbHv.Checked = false;
                        return;
                    }
                }
                if (chk0)
                    this.picWatchs[0].pichvCompare = this.picWatchs[1];
                else
                    this.picWatchs[1].pichvCompare = this.picWatchs[0];
            }
            if ((chk2 && !chk3) || (!chk2 && chk3))
            {
                devname = basefun.valtag(Convert.ToString(this.picWatchs[2].lbHv.Tag), "devname");
                ipaddr = basefun.valtag(Convert.ToString(this.picWatchs[2].lbHv.Tag), "ipaddr");
                if (this.hvWatchs.ContainsKey(devname))
                {
                    string msg = this.hvWatchs[devname].HVdiscern.Open(ipaddr);
                    if (!string.IsNullOrEmpty(msg))
                    {
                        MessageBox.Show(msg, "视频提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        this.picWatchs[2].lbHv.Checked = false;
                        this.picWatchs[3].lbHv.Checked = false;
                        return;
                    }
                }
                if (chk2)
                    this.picWatchs[2].pichvCompare = this.picWatchs[3];
                else
                    this.picWatchs[3].pichvCompare = this.picWatchs[2];
            }
        }

        private void ckbIn1_CheckedChanged(object sender, EventArgs e)
        {
            this.setPichvCompare();
        }

        private void ckbOut1_CheckedChanged(object sender, EventArgs e)
        {
            this.setPichvCompare();
        }

        private void ckbIn2_CheckedChanged(object sender, EventArgs e)
        {
            this.setPichvCompare();
        }

        private void ckbOut2_CheckedChanged(object sender, EventArgs e)
        {
            this.setPichvCompare();
        }
        /// <summary>
        /// 交接班
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btHandOver_Click(object sender, EventArgs e)
        {
            //FormUpDownWork win = new FormUpDownWork();
            //tagData = "模式对话";
            //foreach (CommandBase c in this.cmdDevs)
            //    c.TimeSendInv = new TimeSpan(365, 0, 0, 0);
            //DialogResult rsl = win.ShowDialog();
            //foreach (CommandBase c in this.cmdDevs)
            //    c.TimeSendInv = new TimeSpan(0, 0, 1);
            //tagData = "";
            //if (DialogResult.Yes != rsl)
            //    return;
            //this.paramSystem = BindManager.getSystemParam();
            //this.RefreshParkInfo();
        }

        /// <summary>
        /// 打开一个功能单元
        /// </summary>
        /// <param name="unitName">功能单元</param>
        private void openUnitWin(string unitName)
        {
           // NameObjectList ps = BindManager.getSystemParam();
           // ps["name"] = unitName;
           // BindManager.setTransParam(ps);
           //// Form winForm = FormFactory.CreateForm(unitName);
           // if (null == winForm) return;
           // winForm.Show();
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
            openUnitWin("操作员收费统计表");
        }

        private void btModifyPwd_Click(object sender, EventArgs e)
        {
            //FrmUpdatePassword win = new FrmUpdatePassword();
           // win.ShowDialog(this);
        }


    }
}
