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
    /// 部门休息日维护
    /// </summary>
    public partial class FrmDeptRest : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "基本设置";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet dsUnit = null;
        public FrmDeptRest()
        {
            InitializeComponent();
        }
        private void FrmDeptRest_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            this.dsUnit = bg.BuildDataset(this.unitItem, this.paramwin);
            bg.BindFld(this, this.dsUnit);
        }
        /// <summary>
        /// 添加
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
        /// 删除
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

            DialogResult result = XtraMessageBox.Show("是否删除当前记录！", "系统提示！", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (DialogResult.Yes != result)
                return;
            DataRowView drv = this.BindingContext[tab].Current as DataRowView;
            drv.Row.Delete();
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.dsUnit, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("保存成功！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOut_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// 时间验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView1_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.gridView1.GetDataRow(e.RowHandle)["开始日期"].ToString()))
                this.gridView1.GetDataRow(e.RowHandle)["开始日期"] = DateTime.Now.ToString();
            if (string.IsNullOrEmpty(this.gridView1.GetDataRow(e.RowHandle)["结束日期"].ToString()))
                this.gridView1.GetDataRow(e.RowHandle)["结束日期"] = DateTime.Now.ToString();
            if (Convert.ToDateTime(this.gridView1.GetDataRow(e.RowHandle)["开始日期"]) > Convert.ToDateTime(this.gridView1.GetDataRow(e.RowHandle)["结束日期"]))
                XtraMessageBox.Show("开始时间大于结束时间", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}