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
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;

namespace Granity.granityMgr.CardMgr
{
    /// <summary>
    /// 换卡
    /// </summary>
    public partial class FrmCardUpdate : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "卡片管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        bool isInited = false;
        string cardID = "";
        QueryDataRes Query = null;
        public FrmCardUpdate()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 读卡器
        /// </summary>
        CmdCard cmdCard = null;
        string DevIdS = "";//发行器地址
        /// <summary>
        /// 读取或设置当前窗口读卡器
        /// </summary>
        public CmdCard CmdCard
        {
            get { return cmdCard; }
            set { cmdCard = value; }
        }
        private void FrmCardUpdate_Load(object sender, EventArgs e)
        { //读取业务单元和传递参数
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            bg.SetGridCols(this.dbUser, "姓名,卡号,部门名称");
            DataTable tab = initDev();
            if (null == tab || tab.Rows.Count < 1)
                XtraMessageBox.Show("请设置发行器", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ShowType();
            this.cboDTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.cboETime.Text = DateTime.Now.ToString("yyyy-MM-dd"); ;
            this.cboPTime.Text = DateTime.Now.ToString("yyyy-MM-dd"); ;
        }
        /// <summary>
        /// 初始化发行器
        /// </summary>
        /// <returns></returns>
        public DataTable initDev()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();
            string filter = "(电脑='{0}' or IP地址='{1}') and 通讯类别='发行器'";
            filter = string.Format(filter, Dns.GetHostName(), myip);
            DataTable tab = this.dsUnit.Tables["设备设置"];
            tab.Clear();
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            query.FillDataSet("设备设置", this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
            return tab;
        }
        bool IsaddBZMoney = false;
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
            string EIC = Convert.ToString(tab.Rows[0]["消费IC模式"]);
            string DIC = Convert.ToString(tab.Rows[0]["门禁IC模式"]);
            string PIC = Convert.ToString(tab.Rows[0]["停车场IC模式"]);
            if (EID == "True" || DID == "True" || PID == "True") radid.Checked = true; else radic.Checked = true;
            if (EID == "False" && EIC == "False") Etime.Enabled = false;
            if (DID == "False" && DIC == "False") this.Dtime.Enabled = false;
            if (PID == "False" && PIC == "False") this.Ptime.Enabled = false;
            if (Convert.ToString(tab.Rows[0]["补助累加"]) == "False")
                IsaddBZMoney = false;
            else IsaddBZMoney = true;
        }
        /// <summary>
        /// 读卡
        /// </summary>
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
            if (this.cardID == cardid || (!this.radic.Checked && string.IsNullOrEmpty(cardid)))
                return;
            if (string.IsNullOrEmpty(cardid))
                return;
            // 卡片状态
            for (int i = 0; i < this.gdUser.RowCount; i++)
            {
                string cardno = Convert.ToString(this.gdUser.GetDataRow(i)["卡号"]);

                if (cardno == cardsid)
                {
                    this.txtnewcard.Text = "此卡已发行";
                    return;
                }
                else
                    this.txtnewcard.Text = this.cmdCard.CardSID;
            }
        }

        /// <summary>
        /// 发行器信息
        /// </summary>
        public void initCmdCard()
        {
            DataTable tab = initDev();
            if (null == tab || tab.Rows.Count < 1)
            {
                this.cmdCard.SetTarget(null, -1, false);
                return;
            }
            DataRow dr = this.dsUnit.Tables["设备设置"].Rows[0];
            string port = Convert.ToString(dr["串口"]);
            int baudrate = Convert.ToInt32(dr["波特率"]);
            Parity parity = Parity.None;
            int databits = Convert.ToInt32(dr["数据位"]);
            DevIdS = Convert.ToString(dr["站址"]);
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
        /// 获取最新数据
        /// </summary>
        public void getNewDataByCardNo(string cardno)
        {
            NameObjectList pstrans = new NameObjectList();
            ParamManager.MergeParam(pstrans, this.paramwin);
            pstrans["卡号"] = cardno;
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = dsUnit.Tables["换卡"];
            tab.Clear();
            query.FillDataSet(tab.TableName, pstrans, this.dsUnit);
        }
        /// <summary>
        /// 主从显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gdUser_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            getNewDataByCardNo(Convert.ToString(this.gdUser.GetDataRow(e.RowHandle)["卡号"]));
        }
        /// <summary>
        /// 条件检索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sbtFilter_Click(object sender, EventArgs e)
        {
            //获取当前单元名称
            DataTable tab = dsUnit.Tables["用户选择"];
            if (null == tab) return;
            //数据源
            string filter = "姓名 like '%{0}%' or 用户编号 like '%{0}%' or 卡号 like '%{0}%' or 部门名称 like '%{0}%' or 车牌 like '%{0}%' or 车辆类型 like '%{0}%'";

            filter = string.Format(filter, this.tbFilter.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
        }
        /// <summary>
        /// 换卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        string Dtime1 = "";
        string Etime1 = "";
        string PTime1 = "";
        private void btnCardUpdate_Click(object sender, EventArgs e)
        {
            if (this.txtnewcard.Text == "此卡已发行")
            {
                XtraMessageBox.Show("此卡已发行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DataRow dr = this.dsUnit.Tables["换卡"].Rows[0];
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            bool isflag = false;
            NameObjectList ps = ParamManager.createParam(dr);
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();
            if (this.radYou.Checked)
                ps["是否有卡"] = 1;
            else if (this.radWu.Checked)
                ps["是否有卡"] = 0;
            ps["补助累加"] = IsaddBZMoney;
            ps["卡号"] = txtCardNo.Text;
            ps["新卡片序列号"] = this.txtnewcard.Text;
            ps["新卡号"] = this.txtnewcard.Text;
            ps["补助充值"] = this.txtBZ.Text;
            ps["现金充值"] = this.txtMoney.Text;
            ps["UserAccounts"] = BindManager.getUser().UserName;
            if (Dtime1 == DateTime.Now.ToString("yyyy-MM-dd")) Dtime1 = Dtime.Text;
            if (Etime1 == DateTime.Now.ToString("yyyy-MM-dd")) Etime1 = this.Etime.Text;
            if (PTime1 == DateTime.Now.ToString("yyyy-MM-dd")) PTime1 = this.Ptime.Text;
            if (string.IsNullOrEmpty(Dtime1)) Dtime1 = DateTime.Now.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(Etime1)) Etime1 = DateTime.Now.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(PTime1)) PTime1 = DateTime.Now.ToString("yyyy-MM-dd");
            ps["门禁延期"] = Dtime1;
            ps["消费延期"] = Etime1;
            ps["停车场延期"] = PTime1;
            ps["设备"] = DevIdS;
            ps["LocalIP"] = myip;
            ps["Localhost"] = System.Net.Dns.GetHostName();
            ps["备注"] = "";
            ParamManager.MergeParam(ps, this.paramwin, false);
            isflag = this.Query.ExecuteNonQuery("换卡", ps, ps, ps);
            if (isflag)
                XtraMessageBox.Show("换卡成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                XtraMessageBox.Show("换卡失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            this.dsUnit.Tables["用户选择"].Clear();
            this.Query.FillDataSet("用户选择", new NameObjectList(), this.dsUnit);
            this.dsUnit.Tables["换卡"].Clear();
            this.Query.FillDataSet("换卡", new NameObjectList(), this.dsUnit);
            getNewDataByCardNo(this.txtnewcard.Text);

        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click_1(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            this.Close();
        }

        private void cboDTime_EditValueChanged(object sender, EventArgs e)
        {
            Dtime1 = cboDTime.Text;
        }

        private void cboPTime_EditValueChanged(object sender, EventArgs e)
        {
            PTime1 = cboPTime.Text;
        }

        private void cboETime_EditValueChanged(object sender, EventArgs e)
        {
            Etime1 = cboETime.Text;
        }

    }
}