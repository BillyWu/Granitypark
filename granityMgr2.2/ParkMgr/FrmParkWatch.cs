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
using System.IO;
using Granity.communications;
using System.Net;
using System.Threading;
//using Granity.winTools;
using Granity.CardOneCommi;

namespace Granity.parkStation.cardManager
{
    public partial class FrmParkWatch : Form
    {
        /// <summary>
        /// 车牌识别
        /// </summary>
        HvCarDiscern HVdiscern = new HvCarDiscern();
        DateTime dtlast = DateTime.Now;

        public FrmParkWatch()
        {
            InitializeComponent();
        }

        private void FrmParkWatch_Load(object sender, EventArgs e)
        {
            //搜索车牌硬识别设备
            searchHv();
            //获取停车场设备
            searchPark();
            for (int i = 0; i < this.gdGrid.Rows.Count; i++)
                this.gdGrid.Rows[i].Cells["序号"].Value = (i + 1).ToString();
            Monitor();
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
        /// 双击表格启动识别器监控
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
            string devtype = Convert.ToString(dr.Cells["设备类型"].Value);
            string commitype = Convert.ToString(dr.Cells["通讯类别"].Value);
            string ipaddr = Convert.ToString(dr.Cells["IP地址"].Value);
            if ("车牌识别器" == devtype)
            {
                string msg = HVdiscern.Open(ipaddr);
                if (!string.IsNullOrEmpty(msg))
                    MessageBox.Show(msg);
                else
                    dtlast = DateTime.Now;
                this.grpWatch.Text = Convert.ToString(dr.Cells["设备名称"].Value);
            }
        }

        /// <summary>
        /// 实时视频
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmVideo_Tick(object sender, EventArgs e)
        {
            HvVideoInfo info = this.HVdiscern.getVideoInfo(0);
            if (!(dtlast < info.DtVideo))
                return;
            if (info.VideoSize < 1)
                return;
            dtlast = info.DtVideo;
            MemoryStream streamVideo = this.HVdiscern.getStreamVideo(0);
            if (null != streamVideo)
                this.picVideo.Image = Image.FromStream(streamVideo);
        }

        private void FrmParkWatch_FormClosed(object sender, FormClosedEventArgs e)
        {
            tagData = "结束";
            this.HVdiscern.Close();
            CommiManager.GlobalManager.ClearCommand();
        }

        /// <summary>
        /// 识别车牌
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmHvCardisc_Tick(object sender, EventArgs e)
        {
            HvCarPlateInfo info = this.HVdiscern.getCarPlateInfo(0);
            if (string.IsNullOrEmpty(info.CarNum))
                return;
            this.tbCarNum.Text = info.CarNum;
            this.HVdiscern.ResetInfo(0);
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
                    //表格字段顺序：序号, 设备类型, 设备名称, IP地址, 端口, 通讯类别, 通讯站址, 子网掩码, 默认网关, 物理地址
                    string[] ipAddrs = new string[10];
                    ipAddrs[0] = i.ToString();
                    ipAddrs[1] = "车牌识别器";
                    ipAddrs[2] = "Hv识别器";
                    ipAddrs[3] = Convert.ToString((long)addrIP, 16).PadLeft(8, '0');
                    ipAddrs[4] = "";
                    ipAddrs[5] = "UDP";
                    ipAddrs[6] = "";
                    ipAddrs[7] = Convert.ToString((long)addrMask, 16).PadLeft(8, '0');
                    ipAddrs[8] = Convert.ToString((long)addrGateway, 16).PadLeft(8, '0');
                    ipAddrs[9] = Convert.ToString((long)addrMac, 16).PadLeft(12, '0') + "-";
                    //物理地址
                    for (int k = ipAddrs[9].Length - 4; k > -1; k = k - 2)
                        ipAddrs[9] = ipAddrs[9].Substring(0, k) + ipAddrs[9].Substring(k + 2) + "-" + ipAddrs[9].Substring(k, 2);
                    for (int s = 3; s < ipAddrs.Length - 1; s++)
                    {
                        if (3 < s && s < 7)
                            continue;
                        for (int k = ipAddrs[s].Length - 2; k > -1; k = k - 2)
                        {
                            string val = Convert.ToUInt16(ipAddrs[s].Substring(k, 2), 16).ToString();
                            ipAddrs[s] = ipAddrs[s].Substring(0, k) + "." + val + ipAddrs[s].Substring(k + 2);
                        }
                        if (ipAddrs[s].StartsWith("."))
                            ipAddrs[s] = ipAddrs[s].Substring(1);
                    }
                    ipAddrs[2] = "Hv-" + ipAddrs[3];
                    DataGridViewRow drHv = this.getRow(ipAddrs[3], ipAddrs[4], ipAddrs[6]);
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
                //表格字段顺序：序号, 设备类型, 设备名称, IP地址, 端口, 通讯类别, 通讯站址, 子网掩码, 默认网关, 物理地址
                string[] ipAddrs = new string[10];
                ipAddrs[0] = i.ToString();
                ipAddrs[1] = "停车场验票机";
                ipAddrs[2] = Convert.ToString(dr["名称"]);
                ipAddrs[3] = Convert.ToString(dr["地址"]);
                ipAddrs[4] = port;
                ipAddrs[5] = commitype;
                ipAddrs[6] = Convert.ToString(dr["设备地址"]);
                ipAddrs[7] = "";
                ipAddrs[8] = "";
                ipAddrs[9] = "";
                DataGridViewRow drPark = this.getRow(ipAddrs[3], port, ipAddrs[6]);
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
                string tagdata = "@设备地址=" + addrst;
                CmdProtocol cmdP = new CmdProtocol(devname + "(" + addrst + ")");
                cmdP.setCommand(tpl, cmd, tagdata);
                target.setProtocol(Protocol.PTLPark);

                //持续间隔发送通过监听发现接收数据不正确
                cmdP.TimeFailLimit = TimeSpan.MaxValue;
                cmdP.TimeLimit = TimeSpan.MaxValue;
                cmdP.TimeSendInv = new TimeSpan(0, 0, 1);

                //改为接收响应后再发起指令的方式
                this.target = target;
                cmdP.ResponseHandle += new EventHandler<ResponseEventArgs>(cmdP_ResponseHandle);
                CommiManager.GlobalManager.SendCommand(target, cmdP);
            }
        }

        string tagData = "";
        CommiTarget target = null;
        /// <summary>
        /// 有刷卡，读取卡信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cmdP_ResponseHandle(object sender, ResponseEventArgs e)
        {
            CmdProtocol cmd = sender as CmdProtocol;
            if ("结束" == this.tagData)
                return;
            if (!string.IsNullOrEmpty(cmd.ResponseFormat))
            {
                tagData = cmd.ResponseFormat;
                tagData = basefun.setvaltag(tagData, "设备名称", cmd.CmdId);
            }
        }

        private void tmCard_Tick(object sender, EventArgs e)
        {
            if ("结束" == tagData || string.IsNullOrEmpty(tagData))
                return;
            string tag = tagData;
            tagData = "";
            string devid = basefun.valtag(tag, "{设备地址}");
            string cardnum = basefun.valtag(tag, "{卡号}");
            string dtparkin = basefun.valtag(tag, "{入场时间}");
            string dtparkout = basefun.valtag(tag, "{出入场时间}");
            if (dtparkin == dtparkout)
                tag = basefun.setvaltag(tag, "{出入场时间}", "");

            //设置通讯结果：
            foreach (Control child in this.grpFeeItem.Controls)
            {
                string t = Convert.ToString(child.Tag);
                string pm = basefun.valtag(t, "pm");
                if (string.IsNullOrEmpty(pm))
                    continue;
                child.Text = basefun.valtag(tag, pm);
            }
            this.grpFeeItem.Tag = tag;
            this.grpFeeItem.Text = "收费项目    " + basefun.valtag(tag, "设备名称");
        }
        /// <summary>
        /// 开闸
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btOpenFree_Click(object sender, EventArgs e)
        {
            //执行指令
            string cmd = "管理系统远程闸控制";
            string tpl = "停车场";
            string tag = Convert.ToString(this.grpFeeItem.Tag);
            string devid = basefun.valtag(tag, "{设备地址}");
            if (string.IsNullOrEmpty(devid) || null == this.target)
            {
                MessageBox.Show("当前没有设备");
                return;
            }
            string tagdata = "@设备地址=" + devid;
            tagdata = basefun.setvaltag(tagdata, "{道闸命令}", "01");
            CmdProtocol cmdP = new CmdProtocol(cmd, false);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show(cmdP.ResponseFormat);
        }

        private void btOK_Click(object sender, EventArgs e)
        {
            //执行指令
            string cmd = "管理系统远程闸控制";
            string tpl = "停车场";
            string tag = Convert.ToString(this.grpFeeItem.Tag);
            string devid = basefun.valtag(tag, "{设备地址}");
            if (string.IsNullOrEmpty(devid) || null == this.target)
            {
                MessageBox.Show("当前没有设备");
                return;
            }
            string tagdata = "@设备地址=" + devid;
            tagdata = basefun.setvaltag(tagdata, "{道闸命令}", "01");
            CmdProtocol cmdP = new CmdProtocol(cmd, false);
            cmdP.setCommand(tpl, cmd, tagdata);
            CommiManager.GlobalManager.SendCommand(this.target, cmdP);
            if (!cmdP.EventWh.WaitOne(2000, false))
                MessageBox.Show("通讯失败,请检查设备连接或通讯参数后重试！", "通讯提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show(cmdP.ResponseFormat);
        }

    }
}