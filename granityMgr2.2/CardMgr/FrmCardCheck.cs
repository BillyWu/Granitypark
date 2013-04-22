using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Net;
using System.IO.Ports;
using System.Windows.Forms;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
using Granity.CardOneCommi;
using Granity.communications;
using System.Diagnostics;

namespace Granity.granityMgr.CardMgr
{
    /// <summary>
    /// 卡片检测
    /// </summary>
    public partial class FrmCardCheck : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "卡片管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        bool isInited = false;
        string cardID = "";
        static string OTyep = "";
        /// <summary>
        /// 读卡器
        /// </summary>
        CmdCard cmdCard = null;
        /// <summary>
        /// 读取或设置当前窗口读卡器
        /// </summary>
        public CmdCard CmdCard
        {
            get { return cmdCard; }
            set { cmdCard = value; }
        }
        public FrmCardCheck()
        {
            InitializeComponent();
        }

        private void FrmCardCheck_Load(object sender, EventArgs e)
        {
            //读取业务单元和传递参数
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
            ShowType();
        }
        /// <summary>
        /// 发行模式
        /// </summary>
        public void ShowType()
        {
            DataTable tab = dsUnit.Tables["卡片参数"];
            if (tab.Rows.Count < 1 || tab == null) return;
            string EID = Convert.ToString(tab.Rows[0]["消费ID模式"]);
            string DID = Convert.ToString(tab.Rows[0]["门禁ID模式"]);
            string PID = Convert.ToString(tab.Rows[0]["停车场ID模式"]);
            if (EID == "True" || DID == "True" || PID == "True") radid.Checked = true; else radic.Checked = true;
        }
        /// <summary>
        /// 读卡事件,初始化读卡器,由外部传入给定则禁用单选项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tmDev_Tick(object sender, EventArgs e)
        {
            if (!this.isInited)
            {
                this.initCmdCard();
                this.isInited = true;
                return;
            }
            string cardid = this.cmdCard.CardID;
            string cardsid = this.cmdCard.CardSID;
            string cardnum = this.cmdCard.CardNum;
            Debug.WriteLine(DateTime.Now.ToString("ss.fff") + "one卡号：" + cardnum);
            if (this.cardID == cardid || (!this.radic.Checked && string.IsNullOrEmpty(cardid)))
                return;
            this.txtCardRealNo.Text = this.cmdCard.CardSID;
            if (string.IsNullOrEmpty(cardid))
                return;
            //查找卡号信息并置当前行
            getNewDataByCardNo(cardsid);
        }
        /// <summary>
        /// 初始化发行器
        /// </summary>
        public void initCmdCard()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();

            NameObjectList pstrans = new NameObjectList();
            ParamManager.MergeParam(pstrans, this.paramwin);
            pstrans["IP地址"] = myip;
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = dsUnit.Tables["设备设置"];
            tab.Clear();
            query.FillDataSet(tab.TableName, pstrans, this.dsUnit);
            if (null == tab || tab.Rows.Count < 1) return;
            if (tab.Rows.Count < 1)
            {
                MessageBox.Show("请设置发行器");
                return;
            }
            DataRow dr = this.dsUnit.Tables["设备设置"].Rows[0];
            string port = Convert.ToString(dr["串口"]);
            int baudrate = Convert.ToInt32(dr["波特率"]);
            Parity parity = Parity.None;
            int databits = Convert.ToInt32(dr["数据位"]);
            StopBits stopbits = StopBits.One;
            switch (Convert.ToString(dr["停止位"]))
            {
                case "1.5位": stopbits = StopBits.OnePointFive; break;
                case "2位": stopbits = StopBits.Two; break;
                default: stopbits = StopBits.One; break;
            }
            CommiTarget target = new CommiTarget(port, baudrate, parity, databits, stopbits);
            int addrst = Convert.ToInt32(dr["站址"]);
            bool success = this.cmdCard.SetTarget(target, addrst, this.radic.Checked);
            if (success)
                this.cmdCard.Buzz(true);
        }
        /// <summary>
        /// 关闭窗体，同时关闭通讯协议
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCardCheck_FormClosing(object sender, FormClosingEventArgs e)
        {
            //cmdCard.TrunOffLine();
        }
        /// <summary>
        /// 获取最新数据
        /// </summary>
        public void getNewDataByCardNo(string cardno)
        {
            NameObjectList pstrans = new NameObjectList();
            ParamManager.MergeParam(pstrans, this.paramwin);
            pstrans["卡号"] = cardno;
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = dsUnit.Tables["卡检测"];
            tab.Clear();
            query.FillDataSet(tab.TableName, pstrans, this.dsUnit);
        }
        /// <summary>
        /// 切换到ic读卡模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radic_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isInited)
                initCmdCard();
        }
        /// <summary>
        /// 切换到id读卡模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radid_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isInited)
                initCmdCard();
        }
    }

}