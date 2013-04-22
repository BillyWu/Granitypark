using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Granity.communications;
using Granity.CardOneCommi;
using System.Diagnostics;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using System.Text.RegularExpressions;

namespace Granity.granityMgr.Eatery
{
    /// <summary>
    /// ����ά��
    /// </summary>
    public partial class FrmRestaurant : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "��������";//��Ԫ����
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        string DinName = "";
        public FrmRestaurant()
        {
            InitializeComponent();
        }
        private void FrmRestaurant_Load(object sender, EventArgs e)
        {
            //��ʼ�������͵�Ԫ
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //������
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
            //�����Ĳ���
            DataTable tab = this.ds.Tables["����ά��"];
            this.bindMgr.BindTrv(this.TreeDin, tab, "����", "id", "PID", "@ID={ID},@PID={PID},@���={���},@����={����}");
            this.TreeDin.ExpandAll();
            LoadTypeandCOM();
            this.txtDinName.EditValueChanged += new EventHandler(txtEditValueChanged);
        }
        /// <summary>
        /// �޸����ڵ�text�¼�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = sender as DevExpress.XtraEditors.TextEdit;
            setTreeNodeText(this.TreeDin, "����", txt.Text.Trim());
        }
        /// <summary>
        /// �޸����ڵ��textֵ
        /// </summary>
        /// <param name="trv">��</param>
        /// <param name="colName">���ڵ�����</param>
        /// <param name="colValue">��ֵ</param>
        private void setTreeNodeText(TreeList trv, string colName, string colValue)
        {
            try
            {
                if (trv == null || colName == null || colValue == string.Empty)
                    return;
                if (trv.Nodes.Count == 0)
                    return;
                trv.FocusedNode.SetValue(trv.Columns[colName], colValue.Trim());
            }
            catch
            { }
        }
        /// <summary>
        /// ����������
        /// </summary>
        public void LoadTypeandCOM()
        {
            //����ͨѶ���
            string[] type ={
              "TCP/IP(������)","UDP/IP(������)","����"};
            LoadComboBox(gridView4, type, "ͨѶ���");
            //���ش���
            string[] Com ={
              "COM1","COM2","COM3","COM4","COM5","COM6","COM7","COM8","COM9","COM10"};
            LoadComboBox(gridView4, Com, "����");
            //��������
            string[] Area ={
              "1","2","3","4","5","6","7","8","9","10","11","12","13","14","15"};
            LoadComboBox(gridView4, Area, "��Ƭ����");
            string[] Week ={
              "����һ","���ڶ�","������","������","������","������","������"};
            LoadComboBox(gridView1, Week, "����");
            //���ز�����
            string[] BTL ={ "4800", "9600", "19200", "38400", "56000" };
            LoadComboBox(this.gridView4, BTL, "������");
            //���ض˿�
            string[] DK ={ "30000", "60000" };
            LoadComboBox(this.gridView4, DK, "�˿�");
            //��������λ
            string[] DataW ={ "5", "6", "7", "8" };
            LoadComboBox(gridView4, DataW, "����λ");
            //����ֹͣλ
            string[] DopW ={ "1", "2" };
            LoadComboBox(gridView4, DopW, "ֹͣλ");
        }
        /// <summary>
        /// ����Gridview�������б�
        /// </summary>
        /// <param name="db"></param>
        /// <param name="str"></param>
        /// <param name="ColuName"></param>
        public void LoadComboBox(DevExpress.XtraGrid.Views.Grid.GridView db, string[] str, string ColuName)
        {
            RepositoryItemComboBox riCombo = new RepositoryItemComboBox();
            if (ColuName == "ͨѶ���")
            {
                riCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;

            }
            for (int i = 0; i < str.Length; i++)
            {
                riCombo.Items.Add(str[i].ToString());
            }
            riCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            riCombo.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            this.dbDev.RepositoryItems.Add(riCombo);
            db.Columns[ColuName].ColumnEdit = riCombo;
        }
        /// <summary>
        /// ������� ����ʱ��,�˵�,����,���ѻ�
        /// </summary>
        /// <param name="db">GridControl</param>
        public void AddNewDate(DevExpress.XtraGrid.GridControl db, string flag)
        {
            DataTable tab = db.DataSource as DataTable;
            if (null == tab) return;
            #region ����Ƿ�ָ���˲���
            if (string.IsNullOrEmpty(DinName))
            {
                XtraMessageBox.Show("��ѡ�����!", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            #endregion
            DataRow dr = tab.NewRow();
            switch (flag)
            {
                case "1":
                    if (gridView1.RowCount > 0)
                    {
                        dr["��Ϳ�ʼ"] = gridView1.GetDataRow(0)["��Ϳ�ʼ"];
                        dr["��Ϳ�ʼ"] = gridView1.GetDataRow(0)["��Ϳ�ʼ"];
                        dr["��Ϳ�ʼ"] = gridView1.GetDataRow(0)["��Ϳ�ʼ"];
                        dr["ҹ����ʼ"] = gridView1.GetDataRow(0)["ҹ����ʼ"];
                        dr["��ͽ���"] = gridView1.GetDataRow(0)["��ͽ���"];
                        dr["��ͽ���"] = gridView1.GetDataRow(0)["��ͽ���"];
                        dr["��ͽ���"] = gridView1.GetDataRow(0)["��ͽ���"];
                        dr["ҹ������"] = gridView1.GetDataRow(0)["ҹ������"];
                    }
                    dr["����ID"] = DinName;
                    dr["ID"] = Guid.NewGuid();
                    break;
                case "2":
                    dr["����ID"] = DinName;
                    dr["ID"] = Guid.NewGuid();
                    break;
                case "3":
                    dr["�������"] = DinName;
                    break;
                case "4":
                    dr["ID"] = Guid.NewGuid();
                    dr["��������"] = DinName;
                    dr["������"] = "19200";
                    dr["����λ"] = 8;
                    dr["ֹͣλ"] = 1;
                    dr["�˿�"] = 30000;
                    dr["����"] = "COM1";
                    break;
            }
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// ɾ������ ����ʱ��,�˵�,����,���ѻ�
        /// </summary>
        /// <param name="db">GridControl</param>
        public void DelDate(DevExpress.XtraGrid.GridControl db, DevExpress.XtraGrid.Views.Grid.GridView grid)
        {
            if (grid.RowCount == 0)
                return;
            DataTable tab = db.DataSource as DataTable;
            if (null == tab || tab.Rows.Count < 1)
                return;
            DialogResult result = XtraMessageBox.Show("�Ƿ�ɾ����ǰ��¼", "ϵͳ��ʾ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != result)
                return;
            DataRowView drv = this.BindingContext[tab].Current as DataRowView;
            drv.Row.Delete();
        }
        /// <summary>
        /// ���ݲ������� ����ʱ��,�˵�,���ѻ�ddd
        /// </summary>
        /// <param name="db">GridControl</param>
        /// <param name="flag">����ʾ</param>
        /// <param name="ID">���</param>
        public void CheckByDin(DevExpress.XtraGrid.GridControl db, string flag, string ID)
        {
            try
            {
                DataTable tab = db.DataSource as DataTable;
                if (null == tab) return;
                if (string.IsNullOrEmpty(ID)) return;
                string filter = "";
                switch (flag)
                {
                    case "1":
                        filter = "����ID='{0}' order by �������";
                        break;
                    case "2":
                        filter = "����ID='{0}'";
                        break;
                    case "4":
                        filter = "a.����ID='{0}'";
                        break;
                    case "3":
                        filter = "����ID='{0}'";
                        break;
                    default:
                        break;
                }
                filter = string.Format(filter, ID.Replace("'", "''"));
                QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
                tab.Clear();
                query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.ds);
            }
            catch (Exception ex)
            {

            }
        }


        /// <summary>
        /// ��� ʱ�Σ����ѻ�������˵�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click_1(object sender, EventArgs e)
        {
            string flag = dbEatery.SelectedTabPageIndex.ToString();
            switch (flag)
            {
                case "0":
                    AddNewDate(dbEateyTime, "1");
                    break;
                case "1":
                    AddNewDate(dbMenu, "2");
                    break;
                case "2":
                    AddNewDate(dbMoney, "3");
                    break;
                default:
                    AddNewDate(dbDev, "4");
                    break;
            }
        }

        /// <summary>
        /// ����Ψһ�Ĳ������
        /// </summary>
        /// <returns></returns>
        public string DinNum()
        {
            Guid tempGuid = Guid.NewGuid();
            byte[] bytes = tempGuid.ToByteArray();
            int i = Math.Abs(((int)bytes[1] << 6));
            return i.ToString();
        }
        /// <summary>
        /// ��ӽڵ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        TreeListNode nodeid;
        private void btnDinAdd_Click_1(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeDin.FocusedNode;
            string pid = "";
            pid = Guid.NewGuid().ToString();
            DataTable tab = this.ds.Tables["����ά��"];
            DataRow drnew = tab.NewRow();
            drnew["����"] = "����";
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["PID"] = pid;
            drnew["���"] = DinNum();
            tab.Rows.Add(drnew);
            TreeListNode node;
            node = this.TreeDin.AppendNode(new object[] { drnew["����"].ToString(), drnew["ID"].ToString() }, this.TreeDin.FocusedNode);
            node.Tag = "@ID=" + drnew["ID"].ToString() + ",@PID='',@���=" + drnew["����"].ToString() + "";
            node.ImageIndex = 1;
            node.SelectImageIndex = 4;
            this.TreeDin.FocusedNode = node;
            nodeid = this.TreeDin.FocusedNode;
        }
        /// <summary>
        /// ɾ���ڵ� ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 
        private void btnDinDel_Click_1(object sender, EventArgs e)
        {
            TreeListNode trn = this.TreeDin.FocusedNode;
            if (null == trn)
                return;
            string tip = "ȷ���Ƿ�ɾ����ǰ�ڵ�";
            if (trn.Nodes.Count > 0)
                tip += ",ͬʱ��ɾ���ýڵ���¼������ڵ㣡";
            DialogResult msgquet = XtraMessageBox.Show(tip, "ϵͳ��ʾ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            //�ݹ�ɾ���ڵ�
            TreeListNode trncurrent = trn;
            DataTable tabGroup = this.ds.Tables["����ά��"];
            string tag = basefun.valtag(Convert.ToString(trn.Tag), "ID");
            foreach (DataRow drGroup in tabGroup.Select("id='" + tag + "'"))
            {
                if (this.TreeDin.FindNodeByFieldValue("id", drGroup["id"].ToString()) != null)
                {
                    TreeListNode tn1 = this.TreeDin.FindNodeByFieldValue("id", drGroup["id"].ToString());
                    this.TreeDin.Nodes.Remove(tn1);
                }
                drGroup.Delete();
            }
        }
        /// <summary>
        /// ����ڵ� ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDinSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("����ʧ�ܣ����������Ƿ�Ϸ���", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("����ɹ�", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataTable tab = this.ds.Tables["����ά��"];
            this.bindMgr.BindTrv(this.TreeDin, tab, "����", "id", "PID", "@ID={ID},@PID={PID},@���={���},@����={����}");
            this.TreeDin.ExpandAll();
            this.TreeDin.FocusedNode = nodeid;
        }
        /// <summary>
        /// ɾ��ʱ�Σ��˵���������ѻ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            string flag = dbEatery.SelectedTabPageIndex.ToString();
            switch (flag)
            {
                case "0":
                    DelDate(dbEateyTime, this.gridView1);
                    break;
                case "1":
                    DelDate(dbMenu, this.gridView2);
                    break;
                case "2":
                    DelDate(dbMoney, this.gridView3);
                    break;
                default:
                    DelDate(dbDev, this.gridView4);
                    break;
            }
        }
        /// <summary>
        /// ���� ʱ�Σ��˵���������ѻ�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("����ʧ�ܣ����������Ƿ�Ϸ���", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("����ɹ���", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //���ָ���Ĳ���
            CheckByDin(dbEateyTime, "1", DinName);
            CheckByDin(dbMenu, "2", DinName);
            CheckByDin(dbDev, "4", DinName);
            CheckByDin(dbMoney, "3", DinName);
        }
        /// <summary>
        /// �˳�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtCancel_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("ȷ���رմ���!", "ϵͳ��ʾ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();

        }
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.TreeDin.FocusedNode;
            string pid = "";
            if (trnsel == null)
            {
                pid = Guid.NewGuid().ToString();
            }
            else
            {
                pid = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            }
            DataTable tab = this.ds.Tables["����ά��"];
            DataRow drnew = tab.NewRow();
            drnew["����"] = "����";
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["PID"] = pid;
            drnew["���"] = DinNum();
            tab.Rows.Add(drnew);
            TreeListNode node;
            node = this.TreeDin.AppendNode(new object[] { drnew["����"].ToString(), drnew["ID"].ToString() }, this.TreeDin.FocusedNode);
            node.Tag = "@ID=" + drnew["ID"].ToString() + ",@PID='',@���=" + drnew["����"].ToString() + "";
            node.ImageIndex = 1;
            node.SelectImageIndex = 4;
            this.TreeDin.FocusedNode = node;
            nodeid = this.TreeDin.FocusedNode;
        }
        /// <summary>
        /// �ڵ��л��ı�����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeDin_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }
            int rowIndex = 0;
            string tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
            if (string.IsNullOrEmpty(tag))
            {
                tag = Convert.ToString(e.Node.Tag);
                DinName = tag;
            }
            else
            {
                tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
                DinName = tag;
            }
            DataTable tab = ds.Tables["����ά��"];
            rowIndex = 0;
            foreach (DataRow dr in tab.Rows)
            {
                rowIndex++;
                if (DataRowState.Deleted != dr.RowState && dr["id"].ToString() == tag)
                {
                    BindingManagerBase bindMgrbase = this.BindingContext[tab];
                    if (bindMgrbase != null)
                    {
                        bindMgrbase.Position = rowIndex - 1;
                    }
                    break;
                }
            }

            #region ˢ�¼�¼
            CheckByDin(dbEateyTime, "1", tag);
            CheckByDin(dbMenu, "2", tag);
            CheckByDin(dbDev, "4", tag);
            CheckByDin(dbMoney, "3", tag);
            this.txtDinName.TabIndex = 1;
            #endregion
        }
        /// <summary>s
        /// ���ݺϷ�����֤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView4_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //IP��ַ
            Regex IpReg = new Regex(@"^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$");
            string IpVal = this.gridView4.GetDataRow(e.RowHandle)["IP��ַ"].ToString();
            if (!IpReg.IsMatch(IpVal))
                this.gridView4.GetDataRow(e.RowHandle)["IP��ַ"] = "";
            //������
            Regex NumReg = new Regex(@"^(0|[1-9]\d*)$");
            string NumVal = this.gridView4.GetDataRow(e.RowHandle)["���"].ToString();
            if (!NumReg.IsMatch(NumVal))
                this.gridView4.GetDataRow(e.RowHandle)["���"] = "";
        }
        /// <summary>
        /// �˵����ݺϷ�����֤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView2_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string[] str = new string[this.gridView2.RowCount];
            if (this.gridView2.RowCount < 1) return;
            for (int i = 0; i < this.gridView2.RowCount; i++)
            {
                if (e.RowHandle == i) str[i] = "0";
                else
                    str[i] = Convert.ToString(this.gridView2.GetDataRow(i)["���"]);
                if (Convert.ToString(this.gridView2.GetDataRow(e.RowHandle)["���"]) == str[i].ToString())
                {
                    XtraMessageBox.Show("�˱���Ѿ�ʹ�ã�������д�룡", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.gridView2.GetDataRow(e.RowHandle)["���"] = "0";
                    return;
                }
            }
        }
        /// <summary>
        /// ��֤������ϸ�Ϸ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView3_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {

            string[] str = new string[this.gridView3.RowCount];
            if (this.gridView3.RowCount < 1) return;
            for (int i = 0; i < this.gridView3.RowCount; i++)
            {
                if (e.RowHandle == i) str[i] = "0";
                else
                    str[i] = Convert.ToString(this.gridView3.GetDataRow(i)["���"]);
                if (Convert.ToString(this.gridView3.GetDataRow(e.RowHandle)["���"]) == str[i].ToString())
                {
                    XtraMessageBox.Show("�˱���Ѿ�ʹ�ã�������д�룡", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.gridView3.GetDataRow(e.RowHandle)["���"] = "0";
                    return;
                }
            }
        }
        /// <summary>
        /// ��֤ʱ���Ƿ�Ψһ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string[] str = new string[this.gridView1.RowCount];
            if (this.gridView1.RowCount < 1) return;
            for (int i = 0; i < this.gridView1.RowCount; i++)
            {
                if (e.RowHandle == i) str[i] = "0";
                else
                    str[i] = Convert.ToString(this.gridView1.GetDataRow(i)["����"]);
                if (Convert.ToString(this.gridView1.GetDataRow(e.RowHandle)["����"]) == str[i].ToString())
                {
                    XtraMessageBox.Show("�˱���Ѿ�ʹ�ã�������д�룡", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.gridView1.GetDataRow(e.RowHandle)["����"] = "0";
                    return;
                }
            }
        }
    }
}