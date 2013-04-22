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
    /// �����Ű��ѯ
    /// </summary>
    public partial class FrmDeptScheduleRpt : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "��ѯ����";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmDeptScheduleRpt()
        {
            InitializeComponent();
        }
        private void FrmDeptScheduleRpt_Load(object sender, EventArgs e)
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
            LoadDept();
        }

        /// <summary>
        /// ���ز���
        /// </summary>
        public void LoadDept()
        {
            DataRow drDept = this.dsUnit.Tables["����"].NewRow();
            drDept["����"] = "ȫ��";
            drDept["���"] = string.Empty;
            this.dsUnit.Tables["����"].Rows.Add(drDept);
            this.cboDept.Properties.DataSource = this.dsUnit.Tables["����"];
            this.cboDept.Properties.DisplayMember = "����";
            this.cboDept.Properties.ValueMember = "���";
            this.cboDept.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {  
                    new DevExpress.XtraEditors.Controls.LookUpColumnInfo("���", "���", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
                        , new DevExpress.XtraEditors.Controls.LookUpColumnInfo("����", "����", 200, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)
            });
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
        /// ʱ��β�ѯ,���ţ����� ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbDeptRpt.DataSource as DataTable;
            if (null == tab) return;
            string filter = "a.����>=(SELECT convert(datetime,'{0}',121)) and a.����<=(SELECT convert(datetime, '{1}',121))  and (c.���� like '%{2}%')";
            filter = string.Format(filter, this.cboStart.Text.Replace("'", "''"), this.cboEnd.Text.Replace("'", "''"), this.cboDept.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
        }
        /// <summary>
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRpt_Click(object sender, EventArgs e)
        {
            this.dbDeptRpt.ShowPrintPreview();
        }
    }
}