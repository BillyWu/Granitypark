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
using DevExpress.XtraEditors.Repository;
using System.Text.RegularExpressions;
using DevExpress.XtraEditors;

namespace Granity.granityMgr.CardMgr
{
    /// <summary>
    /// 卡片发行
    /// </summary>
    public partial class FrmCardMakeS : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 单元名称
        /// </summary>
        string unitName = "卡片管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = new DataSet();
        BindManager bindMgr;
        string EateryCard = "消费";
        string ParkCard = "停车场";
        string DoorCard = "门禁";
        /// <summary>
        /// 查询实例
        /// </summary>
        QueryDataRes Query = null;
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
        //发行器
        public FrmCardMakeS()
        {
            InitializeComponent();
        }

        private void FrmCardMakeS_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            this.bindMgr = new BindManager(this);
            this.dsUnit = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.bindMgr.BindFld(this, dsUnit);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            ///检测发行器
            ///
            bindMgr.SetGridCols(this.dbUser, "姓名,卡号,部门名称");
            DataTable taboption = initSet();
            if (null == taboption || taboption.Rows.Count < 1)
                XtraMessageBox.Show("请设置发行器", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblMessage.Text = "";
            ShowType();
            tabCardApp.SelectedTabPageIndex = 2;
        }
        int carTypeNo = 0;
        public void CarType()
        {
            switch (this.cartype.Text)
            {
                case "A类车": carTypeNo = 1; break;
                case "B类车": carTypeNo = 2; break;
                case "C类车": carTypeNo = 3; break;
                case "D类车": carTypeNo = 4; break;
                case "E类车": carTypeNo = 5; break;

            }

        }
        /// <summary>
        /// 发行模式
        /// </summary>
        public void ShowType()
        {
            DataTable tab = dsUnit.Tables["卡片参数"];
            if (tab.Rows.Count < 1 || tab == null) return;
            if (Convert.ToString(tab.Rows[0]["消费ID模式"]) == "True")
            {
                this.cboEatery.Text = "ID模式";
                textEdit1.Enabled = false;
                txtCardNum.Enabled = false;
                txtCardRealNo.Enabled = true;
            }
            else
            {
                this.cboEatery.Text = "IC模式";
                txtCardNum.Enabled = true;
                txtCardRealNo.Enabled = false;
            }
            if (Convert.ToString(tab.Rows[0]["门禁ID模式"]) == "True") this.cboDoor.Text = "ID模式"; else { this.cboDoor.Text = "IC模式"; }
            if (Convert.ToString(tab.Rows[0]["停车场ID模式"]) == "True") this.cboPark.Text = "ID模式"; else { this.cboPark.Text = "IC模式"; }
            if (Convert.ToString(tab.Rows[0]["消费ID模式"]) == "False" && Convert.ToString(tab.Rows[0]["消费IC模式"]) == "False") { ckEatery.Checked = false; ckEatery.Enabled = false; }
            if (Convert.ToString(tab.Rows[0]["停车场ID模式"]) == "False" && Convert.ToString(tab.Rows[0]["停车场IC模式"]) == "False") { this.chkPark.Checked = false; chkPark.Enabled = false; }
            if (Convert.ToString(tab.Rows[0]["门禁ID模式"]) == "False" && Convert.ToString(tab.Rows[0]["门禁IC模式"]) == "False") { this.chkDoors.Checked = false; this.chkDoors.Enabled = false; }
        }
        /// <summary>
        /// 读卡
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
            if (string.IsNullOrEmpty(cardsid))
                return;
            string cardnum = this.cmdCard.CardNum;
            if (this.cardID == cardid || (!this.radic.Checked && string.IsNullOrEmpty(cardid)))
                return;
            txtCardRealNo.Text = cardsid;
            // 卡片状态
            for (int i = 0; i < this.gridView.RowCount; i++)
            {
                string cardno = Convert.ToString(this.gridView.GetDataRow(i)["卡号"]);

                if (cardno == cardsid)
                {
                    lblMessage.Text = "此卡已发行";
                    return;
                }
                else
                    lblMessage.Text = "";
            }
            if (!chkZD.Checked) return;
            //连续发行
            tmDev.Interval = 2500;
            CardMake("发行");

        }
        public DataTable initSet()
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
            DataTable taboption = initSet();
            if (null == taboption || taboption.Rows.Count < 1)
            {
                this.cmdCard.SetTarget(null, -1, false);
                return;
            }
            DataRow dr = taboption.Rows[0];
            string port = Convert.ToString(dr["串口"]);
            int baudrate = Convert.ToInt32(dr["波特率"]);
            Parity parity = Parity.None;
            int databits = Convert.ToInt32(dr["数据位"]);
            StopBits stopbits = StopBits.One;
            switch (Convert.ToString(dr["停止位"]))
            {
                case "1.5": stopbits = StopBits.OnePointFive; break;
                case "2": stopbits = StopBits.Two; break;
                default: stopbits = StopBits.One; break;
            }
            CommiTarget target = new CommiTarget(port, baudrate, parity, databits, stopbits);
            int addrst = Convert.ToInt32(dr["站址"]);
            bool success = this.cmdCard.SetTarget(target, addrst, this.radic.Checked);
        }
        /// <summary>
        /// 窗体关闭，同时关闭通讯
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCardMakeS_FormClosing(object sender, FormClosingEventArgs e)
        {
            cmdCard.TrunOffLine();
        }
        /// <summary>
        /// 卡片发行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sbtPublish_Click(object sender, EventArgs e)
        {
            CarType();
            CardMake("发行");
        }
        public void CardMake(string typeName)
        {
            #region 数据完整性验证
            string msg = "";
            if (!this.chkZD.Checked)
            {
                if (string.IsNullOrEmpty(this.txtCardRealNo.Text))
                    msg = "请刷卡!";
            }
            if (ckEatery.Checked == false && chkDoors.Checked == false && chkPark.Checked == false)
                msg = "请选择业务!";
            if (!string.IsNullOrEmpty(msg))
            {
                XtraMessageBox.Show(msg, "发行提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (this.lblMessage.Text == "此卡已发行") { XtraMessageBox.Show("此卡已发行！", "提示!", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (ckEatery.Checked)
            {
                if (string.IsNullOrEmpty(cbbCardType.Text))
                {
                    XtraMessageBox.Show("消费系统数据不完整!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (chkDoors.Checked)
            {
                if (string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(cboTimeNum.Text))
                {
                    XtraMessageBox.Show("门禁系统数据不完整!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (chkPark.Checked)
            {
                if (string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(cartype.Text))
                {
                    XtraMessageBox.Show("停车场系统数据不完整!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            #endregion
            #region IC卡写入
            if (this.radic.Checked)
            {
                if (string.IsNullOrEmpty(textEdit1.Text))
                {
                    XtraMessageBox.Show("卡号不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string CardStatus = cmdCard.WriteCardNum(textEdit1.Text, ckEatery.Checked, true);
                CardStatus = basefun.valtag(CardStatus, "{状态}");
                if (CardStatus != "操作成功！")
                {
                    XtraMessageBox.Show(CardStatus, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //消费
                if (this.ckEatery.Checked)
                {
                    int cardtype = Convert.ToInt32(cbbCardType.SelectedValue);
                    CardStatus = cmdCard.WriteEateryDtLimit(cardtype, Convert.ToDateTime(Convert.ToDateTime(dtStartEatery.Text).ToString("yyyy-MM-dd HH:mm:ss")), Convert.ToDateTime(Convert.ToDateTime(dtEndEatery.Text).ToString("yyyy-MM-dd HH:mm:ss")), 1, "666666", Convert.ToDouble(txtPayMoney.Text), Convert.ToDouble(this.txtYMoney.Text));
                    CardStatus = basefun.valtag(CardStatus, "{状态}");
                    if (CardStatus != "操作成功！")
                    {
                        XtraMessageBox.Show(CardStatus, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                //停车场
                if (this.chkPark.Checked)
                {
                    int cardtype = Convert.ToInt32(comboBox2.SelectedValue);
                    int cartype = Convert.ToInt32(carTypeNo.ToString());
                    CardStatus = cmdCard.WriteParkDtLimit(cardtype, cartype, Convert.ToDateTime(Convert.ToDateTime(dateEdit3.Text).ToString("yyyy-MM-dd HH:mm:ss")), Convert.ToDateTime(Convert.ToDateTime(dateEdit4.Text).ToString("yyyy-MM-dd HH:mm:ss")), txtCarNum.Text);
                    CardStatus = basefun.valtag(CardStatus, "{状态}");
                    if (CardStatus != "操作成功！")
                    {
                        XtraMessageBox.Show(CardStatus, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                //蜂鸣提示
                cmdCard.Buzz(true);
            }
            #endregion
            #region 数据写入数据库
            int CurrFocuseIndex = this.gridView.FocusedRowHandle;
            WriteDate(typeName, this.txtCardRealNo.Text);
            #endregion
            #region 自动设置到下一行
            SetNextFocusedRow(CurrFocuseIndex);
            #endregion
        }
        /// <summary>
        /// 发卡数据写入数据库
        /// </summary>
        public void WriteDate(string typeName, string cardno)
        {
            DataRow dr = this.dsUnit.Tables["发行"].Rows[0];
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            if (radic.Checked) dr["卡号"] = textEdit1.Text;
            if (radid.Checked) dr["卡号"] = cardno;
            dr["消费"] = EateryCard;
            dr["门禁"] = DoorCard;
            dr["停车场"] = ParkCard;
            dr["卡片序列号"] = cardno;
            dr["用户编号"] = txtNum.Text;
            dr["车型"] = carTypeNo.ToString();
            dr["车辆颜色"] = comboBox4.Text;
            // 发卡未成功 可能是修改数据失败
            bool isflag = false;
            NameObjectList ps = ParamManager.createParam(dr);
            ParamManager.MergeParam(ps, this.paramwin, false);
            if (typeName == "发行")
            {
                if (chkZD.Checked)
                {

                    dr["车型"] = carTypeNo;
                    if (gridView.FocusedRowHandle + 1 < dsUnit.Tables["用户选择"].Rows.Count)
                        isflag = this.Query.ExecuteNonQuery("用户选择", ps, ps, ps);
                }
                isflag = this.Query.ExecuteNonQuery("发行", ps, ps, ps);
            }
            if (isflag)
                XtraMessageBox.Show("发行成功！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                XtraMessageBox.Show("发行失败！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// 设置光标到下一行
        /// </summary>
        public void SetNextFocusedRow(int CurrFocuseIndex)
        {
            //刷新当前发行记录
            this.dsUnit.Tables["用户选择"].Clear();
            this.Query.FillDataSet("用户选择", new NameObjectList(), this.dsUnit);
            //设置光标
            gridView.FocusedRowHandle = CurrFocuseIndex + 1;
            //连续发卡时生成新纪录
            if (chkZD.Checked)
            {
                if (CurrFocuseIndex + 1 == dsUnit.Tables["用户选择"].Rows.Count)
                {
                    int index = CurrFocuseIndex + 1;
                    DataTable tab = this.dbUser.DataSource as DataTable;
                    if (null == tab) return;
                    DataRow dr = tab.NewRow();
                    dr["用户编号"] = BindManager.getCodeSn("");
                    dr["姓名"] = Convert.ToString(this.gridView.GetDataRow(CurrFocuseIndex)["姓名"]) + index;
                    dr["部门"] = Convert.ToString(this.gridView.GetDataRow(CurrFocuseIndex)["部门"]);
                    dr["性别"] = Convert.ToString(this.gridView.GetDataRow(CurrFocuseIndex)["性别"]);
                    dr["电话"] = "";
                    dr["车牌"] = "";
                    dr["车辆颜色"] = "0";
                    dr["车型"] = Convert.ToString(this.gridView.GetDataRow(CurrFocuseIndex)["车型"]);
                    tab.Rows.Add(dr);
                    this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
                    gridView.FocusedRowHandle = tab.Rows.Count + 1;
                }
            }
        }
        /// <summary>
        /// 切换ic模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radic_CheckedChanged(object sender, EventArgs e)
        {
            txtCardNum.Enabled = true;
            if (this.isInited)
                initCmdCard();
            if (radic.Checked)
                chkZD.Enabled = false;
            else
                chkZD.Enabled = true;
        }
        /// <summary>
        /// 切换id模式
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radid_CheckedChanged(object sender, EventArgs e)
        {
            if (this.isInited)
                initCmdCard();
            this.txtCardNum.Enabled = false;
            if (radid.Checked)
                chkZD.Enabled = true;
            else
                chkZD.Enabled = false;
        }
        /// <summary>
        /// 过滤选择用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        int flag = 0;
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
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit); flag = 1;
        }
        /// <summary>

        /// 当前行改变,更改用户编号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {

            if (flag == 1) return;
            else
                flag = 0;
            if (gridView.RowCount <= e.FocusedRowHandle || e.FocusedRowHandle < 0)
                return;
            //设置参数
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            ps["卡号"] = textEdit1.Text;
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab1 = this.dsUnit.Tables["发行"];
            if (null == tab1) return;
            tab1.Clear();
            query.FillDataSet(tab1.TableName, ps, this.dsUnit);
            if (blstatus.Text == "")
                lbState.Text = "未发行";
            else
                lbState.Text = this.blstatus.Text;

        }
        /// <summary>

        /// 选择是否发行消费系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckEatery_CheckedChanged(object sender, EventArgs e)
        {
            this.pgEatery.PageVisible = this.ckEatery.Checked;
            DataTable tab = this.dsUnit.Tables["发行"];
            if (tab == null || tab.Rows.Count == 0)
                return;
            DataRow dr = tab.Rows[0];
            dr["消费"] = this.ckEatery.Checked ? "消费" : "";
            if (ckEatery.Checked)
                EateryCard = "消费";
            else
                EateryCard = "";
        }
        /// <summary>
        /// 选项是否发行门禁系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDoors_CheckedChanged(object sender, EventArgs e)
        {
            this.pgDoor.PageVisible = this.chkDoors.Checked;
            DataTable tab = this.dsUnit.Tables["发行"];
            if (tab == null || tab.Rows.Count == -0)
                return;
            DataRow dr = tab.Rows[0];
            dr["门禁"] = this.chkDoors.Checked ? "门禁" : "";
            if (this.chkDoors.Checked)
                DoorCard = "门禁";
            else
                DoorCard = "";
        }
        private void chkPark_CheckedChanged(object sender, EventArgs e)
        {
            this.pgPark.PageVisible = this.chkPark.Checked;
            DataTable tab = this.dsUnit.Tables["发行"];
            if (tab == null || tab.Rows.Count == 0)
                return;
            DataRow dr = tab.Rows[0];
            dr["停车场"] = this.chkPark.Checked ? "停车场" : "";
            if (this.chkPark.Checked)
                ParkCard = "停车场";
            else
                ParkCard = "";
        }
        private void tabCardApp_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

            switch (tabCardApp.SelectedTabPageIndex)
            {
                case 0:
                    if (this.cboEatery.Text == "IC模式")
                        radic.Checked = true;
                    else
                        radid.Checked = true;
                    break;
                case 1:
                    if (this.cboDoor.Text == "IC模式")
                        radic.Checked = true;
                    else
                        radid.Checked = true;
                    break;
                case 2:
                    if (this.cboPark.Text == "IC模式")
                        radic.Checked = true;
                    else
                        radid.Checked = true;
                    break;
            }

        }
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbUser.DataSource as DataTable;
            if (null == tab) return;
            DataRow dr = tab.NewRow();
            dr["用户编号"] = BindManager.getCodeSn("");
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// 判断卡号是否已经使用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit1_Validated(object sender, EventArgs e)
        {
            DataTable tab = dsUnit.Tables["用户选择"];
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                string UserNo = Convert.ToString(tab.Rows[i]["用户编号"]);
                if (UserNo == textEdit1.Text)
                {
                    XtraMessageBox.Show("此卡号已被使用，请重新填写！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }
        /// <summary>
        /// 关闭窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtclose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            this.Close();
        }
        /// <summary>
        /// 主从显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            DataRow mydatarow = this.gridView.GetDataRow(this.gridView.FocusedRowHandle);
            BindManager.SetControlValue(mydatarow, grpUserInfo, "fld", null);
        }
    }
}