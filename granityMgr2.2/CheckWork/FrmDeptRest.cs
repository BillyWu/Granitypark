using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Granity.winTools;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using DevExpress.XtraEditors.Repository;

namespace Granity.granityMgr.CheckWork
{
    /// <summary>
    /// ������Ϣ��ά��
    /// </summary>
    public partial class FrmDeptRest : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "��������";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmDeptRest()
        {
            InitializeComponent();
        }
        private void FrmDeptRest_Load(object sender, EventArgs e)
        {
            //��ʼ�������͵�Ԫ
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //������
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
        }
        /// <summary>
        /// ���
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbRestDept.DataSource as DataTable;
            if (null == tab) return;
            DataRow dr = tab.NewRow();
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// ɾ��
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btndel_Click(object sender, EventArgs e)
        {
            if (this.gridView1.RowCount == 0)
                return;
            DataTable tab = this.dbRestDept.DataSource as DataTable;
            if (null == tab || tab.Rows.Count < 1)
                return;

            DialogResult result = XtraMessageBox.Show("�Ƿ�ɾ����ǰ��¼��", "ϵͳ��ʾ��", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != result)
                return;
            DataRowView drv = this.BindingContext[tab].Current as DataRowView;
            drv.Row.Delete();
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("����ʧ�ܣ����������Ƿ�Ϸ���", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("����ɹ���", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// �˳�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOut_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("ȷ���رմ���!", "ϵͳ��ʾ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// ʱ����֤
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.gridView1.GetDataRow(e.RowHandle)["��ʼ����"].ToString()))
                this.gridView1.GetDataRow(e.RowHandle)["��ʼ����"] = DateTime.Now.ToString();
            if (string.IsNullOrEmpty(this.gridView1.GetDataRow(e.RowHandle)["��������"].ToString()))
                this.gridView1.GetDataRow(e.RowHandle)["��������"] = DateTime.Now.ToString();
            if (Convert.ToDateTime(this.gridView1.GetDataRow(e.RowHandle)["��ʼ����"]) > Convert.ToDateTime(this.gridView1.GetDataRow(e.RowHandle)["��������"]))
                XtraMessageBox.Show("��ʼʱ����ڽ���ʱ��", "ϵͳ��ʾ��", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}