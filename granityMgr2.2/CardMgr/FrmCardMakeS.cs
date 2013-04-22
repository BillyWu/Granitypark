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
    /// ��Ƭ����
    /// </summary>
    public partial class FrmCardMakeS : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// ��Ԫ����
        /// </summary>
        string unitName = "��Ƭ����";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = new DataSet();
        BindManager bindMgr;
        string EateryCard = "����";
        string ParkCard = "ͣ����";
        string DoorCard = "�Ž�";
        /// <summary>
        /// ��ѯʵ��
        /// </summary>
        QueryDataRes Query = null;
        /// <summary>
        /// �Ƿ��Ѿ���ʼ��������
        /// </summary>
        bool isInited = false;
        /// <summary>
        /// ������
        /// </summary>
        CmdCard cmdCard = new CmdCard();
        /// <summary>
        /// ��ǰ�������к�
        /// </summary>
        string cardID = "";
        //������
        public FrmCardMakeS()
        {
            InitializeComponent();
        }

        private void FrmCardMakeS_Load(object sender, EventArgs e)
        {
            //��ʼ�������͵�Ԫ
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //������
            this.bindMgr = new BindManager(this);
            this.dsUnit = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.bindMgr.BindFld(this, dsUnit);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            ///��ⷢ����
            ///
            bindMgr.SetGridCols(this.dbUser, "����,����,��������");
            DataTable taboption = initSet();
            if (null == taboption || taboption.Rows.Count < 1)
                XtraMessageBox.Show("�����÷�����", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            lblMessage.Text = "";
            ShowType();
            tabCardApp.SelectedTabPageIndex = 2;
        }
        int carTypeNo = 0;
        public void CarType()
        {
            switch (this.cartype.Text)
            {
                case "A�೵": carTypeNo = 1; break;
                case "B�೵": carTypeNo = 2; break;
                case "C�೵": carTypeNo = 3; break;
                case "D�೵": carTypeNo = 4; break;
                case "E�೵": carTypeNo = 5; break;

            }

        }
        /// <summary>
        /// ����ģʽ
        /// </summary>
        public void ShowType()
        {
            DataTable tab = dsUnit.Tables["��Ƭ����"];
            if (tab.Rows.Count < 1 || tab == null) return;
            if (Convert.ToString(tab.Rows[0]["����IDģʽ"]) == "True")
            {
                this.cboEatery.Text = "IDģʽ";
                textEdit1.Enabled = false;
                txtCardNum.Enabled = false;
                txtCardRealNo.Enabled = true;
            }
            else
            {
                this.cboEatery.Text = "ICģʽ";
                txtCardNum.Enabled = true;
                txtCardRealNo.Enabled = false;
            }
            if (Convert.ToString(tab.Rows[0]["�Ž�IDģʽ"]) == "True") this.cboDoor.Text = "IDģʽ"; else { this.cboDoor.Text = "ICģʽ"; }
            if (Convert.ToString(tab.Rows[0]["ͣ����IDģʽ"]) == "True") this.cboPark.Text = "IDģʽ"; else { this.cboPark.Text = "ICģʽ"; }
            if (Convert.ToString(tab.Rows[0]["����IDģʽ"]) == "False" && Convert.ToString(tab.Rows[0]["����ICģʽ"]) == "False") { ckEatery.Checked = false; ckEatery.Enabled = false; }
            if (Convert.ToString(tab.Rows[0]["ͣ����IDģʽ"]) == "False" && Convert.ToString(tab.Rows[0]["ͣ����ICģʽ"]) == "False") { this.chkPark.Checked = false; chkPark.Enabled = false; }
            if (Convert.ToString(tab.Rows[0]["�Ž�IDģʽ"]) == "False" && Convert.ToString(tab.Rows[0]["�Ž�ICģʽ"]) == "False") { this.chkDoors.Checked = false; this.chkDoors.Enabled = false; }
        }
        /// <summary>
        /// ����
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
            // ��Ƭ״̬
            for (int i = 0; i < this.gridView.RowCount; i++)
            {
                string cardno = Convert.ToString(this.gridView.GetDataRow(i)["����"]);

                if (cardno == cardsid)
                {
                    lblMessage.Text = "�˿��ѷ���";
                    return;
                }
                else
                    lblMessage.Text = "";
            }
            if (!chkZD.Checked) return;
            //��������
            tmDev.Interval = 2500;
            CardMake("����");

        }
        public DataTable initSet()
        {
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            string myip = IpEntry.AddressList[0].ToString();

            NameObjectList pstrans = new NameObjectList();
            ParamManager.MergeParam(pstrans, this.paramwin);
            pstrans["IP��ַ"] = myip;
            //��ѯ����
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = dsUnit.Tables["�豸����"];
            tab.Clear();
            query.FillDataSet(tab.TableName, pstrans, this.dsUnit);
            if (null == tab || tab.Rows.Count < 1) return tab;
            return tab;
        }
        /// <summary>
        /// ��������Ϣ
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
            string port = Convert.ToString(dr["����"]);
            int baudrate = Convert.ToInt32(dr["������"]);
            Parity parity = Parity.None;
            int databits = Convert.ToInt32(dr["����λ"]);
            StopBits stopbits = StopBits.One;
            switch (Convert.ToString(dr["ֹͣλ"]))
            {
                case "1.5": stopbits = StopBits.OnePointFive; break;
                case "2": stopbits = StopBits.Two; break;
                default: stopbits = StopBits.One; break;
            }
            CommiTarget target = new CommiTarget(port, baudrate, parity, databits, stopbits);
            int addrst = Convert.ToInt32(dr["վַ"]);
            bool success = this.cmdCard.SetTarget(target, addrst, this.radic.Checked);
        }
        /// <summary>
        /// ����رգ�ͬʱ�ر�ͨѶ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCardMakeS_FormClosing(object sender, FormClosingEventArgs e)
        {
            cmdCard.TrunOffLine();
        }
        /// <summary>
        /// ��Ƭ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sbtPublish_Click(object sender, EventArgs e)
        {
            CarType();
            CardMake("����");
        }
        public void CardMake(string typeName)
        {
            #region ������������֤
            string msg = "";
            if (!this.chkZD.Checked)
            {
                if (string.IsNullOrEmpty(this.txtCardRealNo.Text))
                    msg = "��ˢ��!";
            }
            if (ckEatery.Checked == false && chkDoors.Checked == false && chkPark.Checked == false)
                msg = "��ѡ��ҵ��!";
            if (!string.IsNullOrEmpty(msg))
            {
                XtraMessageBox.Show(msg, "������ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (this.lblMessage.Text == "�˿��ѷ���") { XtraMessageBox.Show("�˿��ѷ��У�", "��ʾ!", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (ckEatery.Checked)
            {
                if (string.IsNullOrEmpty(cbbCardType.Text))
                {
                    XtraMessageBox.Show("����ϵͳ���ݲ�����!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (chkDoors.Checked)
            {
                if (string.IsNullOrEmpty(comboBox1.Text) || string.IsNullOrEmpty(cboTimeNum.Text))
                {
                    XtraMessageBox.Show("�Ž�ϵͳ���ݲ�����!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            if (chkPark.Checked)
            {
                if (string.IsNullOrEmpty(comboBox2.Text) || string.IsNullOrEmpty(cartype.Text))
                {
                    XtraMessageBox.Show("ͣ����ϵͳ���ݲ�����!", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
            #endregion
            #region IC��д��
            if (this.radic.Checked)
            {
                if (string.IsNullOrEmpty(textEdit1.Text))
                {
                    XtraMessageBox.Show("���Ų���Ϊ��", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                string CardStatus = cmdCard.WriteCardNum(textEdit1.Text, ckEatery.Checked, true);
                CardStatus = basefun.valtag(CardStatus, "{״̬}");
                if (CardStatus != "�����ɹ���")
                {
                    XtraMessageBox.Show(CardStatus, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                //����
                if (this.ckEatery.Checked)
                {
                    int cardtype = Convert.ToInt32(cbbCardType.SelectedValue);
                    CardStatus = cmdCard.WriteEateryDtLimit(cardtype, Convert.ToDateTime(Convert.ToDateTime(dtStartEatery.Text).ToString("yyyy-MM-dd HH:mm:ss")), Convert.ToDateTime(Convert.ToDateTime(dtEndEatery.Text).ToString("yyyy-MM-dd HH:mm:ss")), 1, "666666", Convert.ToDouble(txtPayMoney.Text), Convert.ToDouble(this.txtYMoney.Text));
                    CardStatus = basefun.valtag(CardStatus, "{״̬}");
                    if (CardStatus != "�����ɹ���")
                    {
                        XtraMessageBox.Show(CardStatus, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                //ͣ����
                if (this.chkPark.Checked)
                {
                    int cardtype = Convert.ToInt32(comboBox2.SelectedValue);
                    int cartype = Convert.ToInt32(carTypeNo.ToString());
                    CardStatus = cmdCard.WriteParkDtLimit(cardtype, cartype, Convert.ToDateTime(Convert.ToDateTime(dateEdit3.Text).ToString("yyyy-MM-dd HH:mm:ss")), Convert.ToDateTime(Convert.ToDateTime(dateEdit4.Text).ToString("yyyy-MM-dd HH:mm:ss")), txtCarNum.Text);
                    CardStatus = basefun.valtag(CardStatus, "{״̬}");
                    if (CardStatus != "�����ɹ���")
                    {
                        XtraMessageBox.Show(CardStatus, "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                }
                //������ʾ
                cmdCard.Buzz(true);
            }
            #endregion
            #region ����д�����ݿ�
            int CurrFocuseIndex = this.gridView.FocusedRowHandle;
            WriteDate(typeName, this.txtCardRealNo.Text);
            #endregion
            #region �Զ����õ���һ��
            SetNextFocusedRow(CurrFocuseIndex);
            #endregion
        }
        /// <summary>
        /// ��������д�����ݿ�
        /// </summary>
        public void WriteDate(string typeName, string cardno)
        {
            DataRow dr = this.dsUnit.Tables["����"].Rows[0];
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            if (radic.Checked) dr["����"] = textEdit1.Text;
            if (radid.Checked) dr["����"] = cardno;
            dr["����"] = EateryCard;
            dr["�Ž�"] = DoorCard;
            dr["ͣ����"] = ParkCard;
            dr["��Ƭ���к�"] = cardno;
            dr["�û����"] = txtNum.Text;
            dr["����"] = carTypeNo.ToString();
            dr["������ɫ"] = comboBox4.Text;
            // ����δ�ɹ� �������޸�����ʧ��
            bool isflag = false;
            NameObjectList ps = ParamManager.createParam(dr);
            ParamManager.MergeParam(ps, this.paramwin, false);
            if (typeName == "����")
            {
                if (chkZD.Checked)
                {

                    dr["����"] = carTypeNo;
                    if (gridView.FocusedRowHandle + 1 < dsUnit.Tables["�û�ѡ��"].Rows.Count)
                        isflag = this.Query.ExecuteNonQuery("�û�ѡ��", ps, ps, ps);
                }
                isflag = this.Query.ExecuteNonQuery("����", ps, ps, ps);
            }
            if (isflag)
                XtraMessageBox.Show("���гɹ���", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else
                XtraMessageBox.Show("����ʧ�ܣ�", "��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// ���ù�굽��һ��
        /// </summary>
        public void SetNextFocusedRow(int CurrFocuseIndex)
        {
            //ˢ�µ�ǰ���м�¼
            this.dsUnit.Tables["�û�ѡ��"].Clear();
            this.Query.FillDataSet("�û�ѡ��", new NameObjectList(), this.dsUnit);
            //���ù��
            gridView.FocusedRowHandle = CurrFocuseIndex + 1;
            //��������ʱ�����¼�¼
            if (chkZD.Checked)
            {
                if (CurrFocuseIndex + 1 == dsUnit.Tables["�û�ѡ��"].Rows.Count)
                {
                    int index = CurrFocuseIndex + 1;
                    DataTable tab = this.dbUser.DataSource as DataTable;
                    if (null == tab) return;
                    DataRow dr = tab.NewRow();
                    dr["�û����"] = BindManager.getCodeSn("");
                    dr["����"] = Convert.ToString(this.gridView.GetDataRow(CurrFocuseIndex)["����"]) + index;
                    dr["����"] = Convert.ToString(this.gridView.GetDataRow(CurrFocuseIndex)["����"]);
                    dr["�Ա�"] = Convert.ToString(this.gridView.GetDataRow(CurrFocuseIndex)["�Ա�"]);
                    dr["�绰"] = "";
                    dr["����"] = "";
                    dr["������ɫ"] = "0";
                    dr["����"] = Convert.ToString(this.gridView.GetDataRow(CurrFocuseIndex)["����"]);
                    tab.Rows.Add(dr);
                    this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
                    gridView.FocusedRowHandle = tab.Rows.Count + 1;
                }
            }
        }
        /// <summary>
        /// �л�icģʽ
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
        /// �л�idģʽ
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
        /// ����ѡ���û�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 

        int flag = 0;
        private void sbtFilter_Click(object sender, EventArgs e)
        {
            //��ȡ��ǰ��Ԫ����
            DataTable tab = dsUnit.Tables["�û�ѡ��"];
            if (null == tab) return;
            //����Դ
            string filter = "���� like '%{0}%' or �û���� like '%{0}%' or ���� like '%{0}%' or �������� like '%{0}%' or ���� like '%{0}%' or �������� like '%{0}%'";
            filter = string.Format(filter, this.tbFilter.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit); flag = 1;
        }
        /// <summary>

        /// ��ǰ�иı�,�����û����
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
            //���ò���
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            ps["����"] = textEdit1.Text;
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab1 = this.dsUnit.Tables["����"];
            if (null == tab1) return;
            tab1.Clear();
            query.FillDataSet(tab1.TableName, ps, this.dsUnit);
            if (blstatus.Text == "")
                lbState.Text = "δ����";
            else
                lbState.Text = this.blstatus.Text;

        }
        /// <summary>

        /// ѡ���Ƿ�������ϵͳ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ckEatery_CheckedChanged(object sender, EventArgs e)
        {
            this.pgEatery.PageVisible = this.ckEatery.Checked;
            DataTable tab = this.dsUnit.Tables["����"];
            if (tab == null || tab.Rows.Count == 0)
                return;
            DataRow dr = tab.Rows[0];
            dr["����"] = this.ckEatery.Checked ? "����" : "";
            if (ckEatery.Checked)
                EateryCard = "����";
            else
                EateryCard = "";
        }
        /// <summary>
        /// ѡ���Ƿ����Ž�ϵͳ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void chkDoors_CheckedChanged(object sender, EventArgs e)
        {
            this.pgDoor.PageVisible = this.chkDoors.Checked;
            DataTable tab = this.dsUnit.Tables["����"];
            if (tab == null || tab.Rows.Count == -0)
                return;
            DataRow dr = tab.Rows[0];
            dr["�Ž�"] = this.chkDoors.Checked ? "�Ž�" : "";
            if (this.chkDoors.Checked)
                DoorCard = "�Ž�";
            else
                DoorCard = "";
        }
        private void chkPark_CheckedChanged(object sender, EventArgs e)
        {
            this.pgPark.PageVisible = this.chkPark.Checked;
            DataTable tab = this.dsUnit.Tables["����"];
            if (tab == null || tab.Rows.Count == 0)
                return;
            DataRow dr = tab.Rows[0];
            dr["ͣ����"] = this.chkPark.Checked ? "ͣ����" : "";
            if (this.chkPark.Checked)
                ParkCard = "ͣ����";
            else
                ParkCard = "";
        }
        private void tabCardApp_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {

            switch (tabCardApp.SelectedTabPageIndex)
            {
                case 0:
                    if (this.cboEatery.Text == "ICģʽ")
                        radic.Checked = true;
                    else
                        radid.Checked = true;
                    break;
                case 1:
                    if (this.cboDoor.Text == "ICģʽ")
                        radic.Checked = true;
                    else
                        radid.Checked = true;
                    break;
                case 2:
                    if (this.cboPark.Text == "ICģʽ")
                        radic.Checked = true;
                    else
                        radid.Checked = true;
                    break;
            }

        }
        /// <summary>
        /// ����û�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbUser.DataSource as DataTable;
            if (null == tab) return;
            DataRow dr = tab.NewRow();
            dr["�û����"] = BindManager.getCodeSn("");
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// �жϿ����Ƿ��Ѿ�ʹ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEdit1_Validated(object sender, EventArgs e)
        {
            DataTable tab = dsUnit.Tables["�û�ѡ��"];
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                string UserNo = Convert.ToString(tab.Rows[i]["�û����"]);
                if (UserNo == textEdit1.Text)
                {
                    XtraMessageBox.Show("�˿����ѱ�ʹ�ã���������д��", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }
        /// <summary>
        /// �رմ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtclose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("ȷ���رմ���!", "ϵͳ��ʾ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            this.Close();
        }
        /// <summary>
        /// ������ʾ
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