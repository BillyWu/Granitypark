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
using System.Drawing.Printing;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors;
namespace Granity.granityMgr.util
{
    public partial class FrmCardParam : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "ϵͳ����";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmCardParam()
        {
            InitializeComponent();
        }
        private void FrmCardParam_Load(object sender, EventArgs e)
        {
            //��ȡҵ��Ԫ�ʹ��ݲ���
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //������
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton1_Click(object sender, EventArgs e)
        {
            if (Convert.ToString(gridView1.GetDataRow(0)["ͣ����IDģʽ"]) != Convert.ToString(gridView1.GetDataRow(0)["�Ž�IDģʽ"]) || Convert.ToString(gridView1.GetDataRow(0)["ͣ����IDģʽ"]) != Convert.ToString(gridView1.GetDataRow(0)["����IDģʽ"]) || Convert.ToString(gridView1.GetDataRow(0)["����IDģʽ"]) != Convert.ToString(gridView1.GetDataRow(0)["�Ž�IDģʽ"]))
            {
                MessageBox.Show("IC,IDģʽ����ͬʱʹ�ã�", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            if (!isSuccess)
                MessageBox.Show("����ʧ�ܣ����������Ƿ�Ϸ���", "������ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("����ɹ���", "������ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// �˳�
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void simpleButton2_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("ȷ���رմ���!", "ϵͳ��ʾ��", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// ���Ƶ�ѡ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (Convert.ToString(gridView1.GetDataRow(e.RowHandle)["����IDģʽ"]) == "True" && Convert.ToString(gridView1.GetDataRow(e.RowHandle)["����ICģʽ"]) == "True")
            {
                MessageBox.Show("����IC,IDģʽ����ͬʱѡ��", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView1.GetDataRow(e.RowHandle)["����IDģʽ"] = "False";
                gridView1.GetDataRow(e.RowHandle)["����ICģʽ"] = "False";
            }
            if (Convert.ToString(gridView1.GetDataRow(e.RowHandle)["�Ž�IDģʽ"]) == "True" && Convert.ToString(gridView1.GetDataRow(e.RowHandle)["�Ž�ICģʽ"]) == "True")
            {
                MessageBox.Show("�Ž�IC,IDģʽ����ͬʱѡ��", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView1.GetDataRow(e.RowHandle)["�Ž�IDģʽ"] = "False";
                gridView1.GetDataRow(e.RowHandle)["�Ž�ICģʽ"] = "False";
            }
            if (Convert.ToString(gridView1.GetDataRow(e.RowHandle)["ͣ����IDģʽ"]) == "True" && Convert.ToString(gridView1.GetDataRow(e.RowHandle)["ͣ����ICģʽ"]) == "True")
            {
                MessageBox.Show("ͣ����IC,IDģʽ����ͬʱѡ��", "ϵͳ��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                gridView1.GetDataRow(e.RowHandle)["ͣ����IDģʽ"] = "False";
                gridView1.GetDataRow(e.RowHandle)["ͣ����ICģʽ"] = "False";
            }
        }
    }
}