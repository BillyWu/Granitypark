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
    /// Ա���򿨼�¼��ѯ
    /// </summary>
    public partial class FrmHitCardRpt : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "��ѯ����";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmHitCardRpt()
        {
            InitializeComponent();
        }

        private void FrmHitCardRpt_Load(object sender, EventArgs e)
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
        /// ʱ��β飬�������û���ţ����� ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbHitCard.DataSource as DataTable;
            if (null == tab) return;

            string filter = " a.ʱ��>=(SELECT convert(datetime,'{0}',121)) and a.ʱ��<=(SELECT convert(datetime,'{1}',121)) and(a.���� like'%{2}%' or a.�û���� like '%{2}%'or b.NAME like'%{2}%' or c.���� like '%{2}%' )";
            filter = string.Format(filter, this.cboStart.Text.Replace("'", "''"), this.cboEnd.Text.Replace("'", "''"), this.txtCheck.Text.Replace("'", "''"));
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.dsUnit);
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
        /// ����
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRpt_Click(object sender, EventArgs e)
        {
            this.dbHitCard.ShowPrintPreview();
        }
    }
}