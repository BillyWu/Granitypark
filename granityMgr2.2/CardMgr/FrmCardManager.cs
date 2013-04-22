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
    /// 卡片管理 充值，延期，挂失，解挂，退卡，检测
    /// </summary>
    public partial class FrmCardManager : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "卡片管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        string dtime1 = "";
        string etime1 = "";
        string ptime1 = "";

        /// <summary>
        /// 查询实例
        /// </summary>
        QueryDataRes Query = null;
        static string status = "";//卡片状态
        string DevIdS = "";//发行器地址
        /// <summary>
        /// 是否已经初始化读卡器
        /// </summary>
        bool isInited = false;
        /// <summary>
        /// 读卡器
        /// </summary>
        CmdCard cmdCard = new CmdCard();
        /// <summary>
        /// 当前卡号序列号
        /// </summary>
        string cardID = "";
        public FrmCardManager()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 补助金额是否累加
        /// </summary>
        bool IsaddBZMoney = false;
        private void FrmCardManager_Load(object sender, EventArgs e)
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
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            bg.SetGridCols(this.dbUser, "姓名,卡号,部门名称");
            this.Etime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.Dtime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.Ptime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            DataTable tab = initDev();
            if (null == tab || tab.Rows.Count < 1)
                XtraMessageBox.Show("请设置发行器", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            ShowType();
        }
        /// <summary>
        /// 卡片模式
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
        /// 条件模糊过滤用户信息
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
        /// 初始化发行器
        /// </summary>
        /// <returns></returns>
        public DataTable initDev()
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
            if (null == tab || tab.Rows.Count < 1) return tab;
            return tab;
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
        /// 根据fld获取value
        /// </summary>
        /// <param name="ct"></param>
        /// <param name="ps"></param>
        /// <returns></returns>
        private NameObjectList buildParam(Control ct, NameObjectList ps)
        {
            if (null == ps)
                ps = new NameObjectList();
            string tag = Convert.ToString(ct.Tag);
            if (!string.IsNullOrEmpty(basefun.valtag(tag, "fld")))
            {
                ps[basefun.valtag(tag, "fld")] = ct.Text;
                return ps;
            }
            foreach (Control child in ct.Controls)
                this.buildParam(child, ps);
            return ps;
        }
        /// <summary>
        /// 根据卡号获取当前信息
        /// </summary>
        public DataTable getNewDataByCardNo(string cardno)
        {
            NameObjectList pstrans = new NameObjectList();
            ParamManager.MergeParam(pstrans, this.paramwin);
            pstrans["卡号"] = cardno;
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = dsUnit.Tables["selectcard"];
            tab.Clear();
            query.FillDataSet(tab.TableName, pstrans, this.dsUnit);
            if (null == tab || tab.Rows.Count < 1) return tab;
            status = Convert.ToString(tab.Rows[0]["状态"]);
            return tab;
        }
        /// <summary>
        /// 关闭窗体，同时关闭通讯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCardManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            cmdCard.TrunOffLine();
        }
        /// <summary>
        /// 充值延期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCardPay_Click(object sender, EventArgs e)
        {
            getNewDataByCardNo(txtCardRealNo.Text);
            if (string.IsNullOrEmpty(txtCardRealNo.Text))
            {
                XtraMessageBox.Show("请读卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dsUnit.Tables["selectCard"].Rows.Count < 1)
            {
                XtraMessageBox.Show("此卡未发行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (status == "退卡")
            {
                XtraMessageBox.Show("此卡号状态为退卡，不能充值！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (status == "挂失")
            {
                XtraMessageBox.Show("此卡号状态为挂失，不能充值！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (radic.Checked)
            {
                if (WriteInfo("充值") == false) return;
            }
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();
            NameObjectList ps = this.buildParam(this, null);
            if (this.radYou.Checked)
            {
                ps["是否有卡"] = 0;
            }
            else if (this.radWu.Checked)
            {
                ps["是否有卡"] = 1;
            }
            if (!this.Etime.Enabled) ps["消费"] = ""; else ps["消费"] = "消费";
            if (!this.Dtime.Enabled) ps["门禁"] = ""; else ps["门禁"] = "门禁";
            if (!this.Ptime.Enabled) ps["停车场"] = ""; else ps["停车场"] = "停车场";
            if (dtime1 == DateTime.Now.ToString("yyyy-MM-dd")) dtime1 = this.txtdtime.Text;
            if (etime1 == DateTime.Now.ToString("yyyy-MM-dd")) etime1 = this.txtEtime.Text;
            if (ptime1 == DateTime.Now.ToString("yyyy-MM-dd")) ptime1 = this.txtptime.Text;
            if (string.IsNullOrEmpty(dtime1)) dtime1 = DateTime.Now.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(etime1)) etime1 = DateTime.Now.ToString("yyyy-MM-dd");
            if (string.IsNullOrEmpty(ptime1)) ptime1 = DateTime.Now.ToString("yyyy-MM-dd");
            ps["门禁延期"] = dtime1;
            ps["消费延期"] = etime1;
            ps["停车场延期"] = ptime1;
            ps["现金充值"] = txtXJmoney.Text;
            ps["补助充值"] = txtBZmoney.Text;
            ps["补助累加"] = IsaddBZMoney;
            ps["UserAccounts"] = BindManager.getUser().UserName;
            ps["设备"] = DevIdS;
            ps["LocalIP"] = myip;
            ps["Localhost"] = System.Net.Dns.GetHostName();
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            NameObjectList psnull = new NameObjectList();
            if (query.ExecuteNonQuery("充值", ps, ps, ps))
                XtraMessageBox.Show("充值成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else XtraMessageBox.Show("充值失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //刷新纪录
            this.dsUnit.Tables["用户选择"].Clear();
            this.Query.FillDataSet("用户选择", new NameObjectList(), this.dsUnit);
            this.dsUnit.Tables["日志"].Clear();
            this.Query.FillDataSet("日志", new NameObjectList(), this.dsUnit);
            getNewDataByCardNo(txtCardRealNo.Text);
        }
        /// <summary>
        /// IC卡充值写入
        /// </summary>
        public bool WriteInfo(string TypeName)
        {
            string CardStatus = "";
            DataTable tab = getNewDataByCardNo(this.txtCardRealNo.Text);
            if (tab.Rows.Count < 1 || tab == null) return false;
            int Pcardtype = Convert.ToInt32(tab.Rows[0]["P卡型"]);
            int Pcartype = Convert.ToInt32(tab.Rows[0]["车型"]);
            string CardNo = Convert.ToString(tab.Rows[0]["车牌"]);
            string EXtime = "";
            string PXtime = "";
            if (TypeName == "充值")
            {
                PXtime = Convert.ToString(tab.Rows[0]["P现有效期"]);
                EXtime = Convert.ToString(tab.Rows[0]["E现有效期"]);
            }
            else if (TypeName == "延期")
            {
                PXtime = Ptime.Text;
                EXtime = Etime.Text;
            }
            //消费
            if (Etime.Enabled)
            {
                CardStatus = cmdCard.WriteEateryDtLimit(Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), Convert.ToDateTime(Convert.ToDateTime(EXtime).ToString("yyyy-MM-dd HH:mm:ss")), Convert.ToDouble(txtXJmoney.Text), Convert.ToDouble(this.txtBZmoney.Text), IsaddBZMoney);
                CardStatus = basefun.valtag(CardStatus, "{状态}");
                if (CardStatus != "操作成功！")
                {
                    XtraMessageBox.Show(CardStatus, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            //停车场
            if (Ptime.Enabled)
            {
                if (string.IsNullOrEmpty(CardNo)) CardNo = "沪12345";
                CardStatus = cmdCard.WriteParkDtLimit(Pcardtype, Pcartype, Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")), Convert.ToDateTime(Convert.ToDateTime(PXtime).ToString("yyyy-MM-dd HH:mm:ss")), CardNo);
                CardStatus = basefun.valtag(CardStatus, "{状态}");
                if (CardStatus != "操作成功！")
                {
                    XtraMessageBox.Show(CardStatus, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            //蜂鸣提示
            cmdCard.Buzz(true);
            return true;
        }
        /// <summary>
        /// 读卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        string cardno = "";
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

            if ((!this.radic.Checked && string.IsNullOrEmpty(cardid)) || cardsid == "")
                return;
           
            this.cardID = cardid;
            cardno = cardsid;
            this.txtCardRealNo.Text = cardsid;
            //查找卡号信息并置当前行
            getNewDataByCardNo(cardsid);
            for (int i = 0; i < this.gdUser.RowCount; i++)
            {
                DataRow dr = this.gdUser.GetDataRow(i);
                if (null == dr) continue;
                if (!cardnum.Equals(dr["卡号"]))
                    continue;
                this.gdUser.SelectRow(i);
                this.gdUser.FocusedRowHandle = i;
                break;
            }
            this.txtXJmoney.Text = "0.00";
            this.txtBZmoney.Text = "0.00";
        }
        /// <summary>
        /// 挂失
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton4_Click(object sender, EventArgs e)
        {
            getNewDataByCardNo(txtCardRealNo.Text);
            if (string.IsNullOrEmpty(txtCardRealNo.Text))
            {
                XtraMessageBox.Show("请读卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dsUnit.Tables["selectCard"].Rows.Count < 1)
            {
                XtraMessageBox.Show("此卡未发行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (status == "退卡")
            {
                XtraMessageBox.Show("此卡号状态为退卡，不能挂失！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();
            NameObjectList ps = this.buildParam(this, null);
            ps["设备"] = DevIdS;
            ps["LocalIP"] = myip;
            ps["Localhost"] = System.Net.Dns.GetHostName();
            ps["UserAccounts"] = BindManager.getUser().UserName;
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            NameObjectList psnull = new NameObjectList();
            if (query.ExecuteNonQuery("挂失", ps, ps, psnull))
                XtraMessageBox.Show("挂失成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else XtraMessageBox.Show("挂失失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //刷新纪录
            this.dsUnit.Tables["用户选择"].Clear();
            this.Query.FillDataSet("用户选择", new NameObjectList(), this.dsUnit);
            this.dsUnit.Tables["日志"].Clear();
            this.Query.FillDataSet("日志", new NameObjectList(), this.dsUnit);
        }
        /// <summary>
        /// 退卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCardQuit_Click(object sender, EventArgs e)
        {
            getNewDataByCardNo(cardno);
            if (string.IsNullOrEmpty(cardno))
            {
                XtraMessageBox.Show("请读卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dsUnit.Tables["selectCard"].Rows.Count < 1)
            {
                XtraMessageBox.Show("此卡未发行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (status == "挂失")
            {
                XtraMessageBox.Show("此卡号状态为挂失，不能退卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            #region Ic卡清空数及退卡
            if (radic.Checked)
            {
                string CardStatus = "";
                //消费
                if (Etime.Enabled)
                {
                    CardStatus = cmdCard.ClearData(CardArea.Eatery);
                    CardStatus = basefun.valtag(CardStatus, "{状态}");
                    if (CardStatus != "操作成功！")
                    {
                        MessageBox.Show(CardStatus, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                //停车场
                if (Ptime.Enabled)
                {
                    CardStatus = cmdCard.ClearData(CardArea.Park);
                    CardStatus = basefun.valtag(CardStatus, "{状态}");
                    if (CardStatus != "操作成功！")
                    {
                        MessageBox.Show(CardStatus, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                //蜂鸣提示
                cmdCard.Buzz(true);
            }
            #endregion
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();
            NameObjectList ps = this.buildParam(this, null);
            if (this.radYou.Checked)
            {
                ps["是否有卡"] = 0;
            }
            else if (this.radWu.Checked)
            {
                ps["是否有卡"] = 1;
            }
            ps["UserAccounts"] = BindManager.getUser().UserName;
            ps["设备"] = DevIdS;
            ps["LocalIP"] = myip;
            ps["Localhost"] = System.Net.Dns.GetHostName();
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            NameObjectList psnull = new NameObjectList();
            if (query.ExecuteNonQuery("退卡", ps, ps, ps))
                XtraMessageBox.Show("退卡成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else XtraMessageBox.Show("退卡失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //刷新纪录
            this.dsUnit.Tables["用户选择"].Clear();
            this.Query.FillDataSet("用户选择", new NameObjectList(), this.dsUnit);
            this.dsUnit.Tables["日志"].Clear();
            this.Query.FillDataSet("日志", new NameObjectList(), this.dsUnit);
        }
        /// <summary>
        /// 解挂
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCardUnLost_Click(object sender, EventArgs e)
        {
            getNewDataByCardNo(txtCardRealNo.Text);
            if (string.IsNullOrEmpty(txtCardRealNo.Text))
            {
                XtraMessageBox.Show("请读卡！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (dsUnit.Tables["selectCard"].Rows.Count < 1)
            {
                XtraMessageBox.Show("此卡未发行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (status == "退卡")
            {
                XtraMessageBox.Show("此卡号状态为退卡，不能解挂！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (status != "挂失")
            {
                XtraMessageBox.Show("此卡未挂失，不能解挂！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();
            NameObjectList ps = this.buildParam(this, null);
            ps["设备"] = DevIdS;
            ps["LocalIP"] = myip;
            ps["Localhost"] = System.Net.Dns.GetHostName();
            ps["UserAccounts"] = BindManager.getUser().UserName;
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            NameObjectList psnull = new NameObjectList();
            if (query.ExecuteNonQuery("解挂", ps, ps, psnull))
                XtraMessageBox.Show("解挂成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else XtraMessageBox.Show("解挂失败", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //刷新纪录
            this.dsUnit.Tables["用户选择"].Clear();
            this.Query.FillDataSet("用户选择", new NameObjectList(), this.dsUnit);
            this.dsUnit.Tables["日志"].Clear();
            this.Query.FillDataSet("日志", new NameObjectList(), this.dsUnit);
        }
        /// <summary>
        /// 卡检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            FrmCardCheck f = new FrmCardCheck();
            f.CmdCard = this.cmdCard;
            f.ShowDialog();
        }
        /// <summary>
        /// 切换IC模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radic_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isInited)
                initCmdCard();
        }
        /// <summary>
        /// 切换ID模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radid_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isInited)
                initCmdCard();
        }
        /// <summary>
        /// 数据主从现实
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gdUser_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow mydatarow = this.gdUser.GetDataRow(this.gdUser.FocusedRowHandle);
            BindManager.SetControlValue(mydatarow, grpUserInfo, "fld", null);
        }

        /// <summary>
        /// 验证消费有效期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Etime_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(Etime.Text) < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")))
            {
                XtraMessageBox.Show("卡片有效期不能小于今天!", "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            etime1 = Etime.Text;
        }
        /// <summary>
        /// 验证门禁有效期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Dtime_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(Dtime.Text) < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")))
            {
                XtraMessageBox.Show("卡片有效期不能小于今天!", "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            dtime1 = Dtime.Text;
        }
        /// <summary>
        /// 停车场有效期
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ptime_EditValueChanged(object sender, EventArgs e)
        {
            if (Convert.ToDateTime(this.Ptime.Text) < Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd")))
            {
                XtraMessageBox.Show("卡片有效期不能小于今天!", "系统提示!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
            ptime1 = Ptime.Text;
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnclose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            this.Close();
        }
        /// <summary>
        /// 换卡
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpdateCard_Click(object sender, EventArgs e)
        {
            FrmCardUpdate f = new FrmCardUpdate();
            f.CmdCard = this.cmdCard;
            f.ShowDialog();
        }
    }
}