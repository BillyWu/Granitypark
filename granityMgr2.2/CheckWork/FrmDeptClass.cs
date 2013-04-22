using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
namespace Granity.granityMgr.CheckWork
{
    /// <summary>
    /// 部门班制
    /// </summary>
    public partial class FrmDeptClass : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "基本设置";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        public FrmDeptClass()
        {
            InitializeComponent();
        }

        private void BtAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.gridControlClass.DataSource as DataTable;
            DataRow dr = tab.NewRow();
            dr["ID"] = Guid.NewGuid().ToString();
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }

        private void BtDel_Click(object sender, EventArgs e)
        {
            DataTable tab = this.gridControlClass.DataSource as DataTable;
            if (null == tab || tab.Rows.Count < 1)
                return;
            DataRowView dv = this.BindingContext[tab].Current as DataRowView;
            dv.Row.Delete();
        }

        private void BtSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
            {
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                this.ds.Tables["部门班制"].AcceptChanges();
                XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FrmDeptClass_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.ds = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.ds);
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void gridViewClass_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (e.Column.FieldName == "班制ID")
            {
                DataTable tab = this.gridControlClass.DataSource as DataTable;
                DataRowView dv = this.BindingContext[tab].Current as DataRowView;
                dv.BeginEdit();
                dv.Row["换班周期"] = this.ds.Tables["班制明细"].Select("PID='" + e.Value.ToString() + "'").Length;
                dv.EndEdit();
            }
        }
    }
}