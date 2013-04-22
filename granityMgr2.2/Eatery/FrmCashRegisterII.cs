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
namespace Granity.granityMgr.Eatery
{
    /// <summary>
    /// 餐厅现金登记
    /// </summary>
    public partial class FrmCashRegisterII : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "餐厅现金登记";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        DataSet ds = null;
        public FrmCashRegisterII()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 增加行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtAdd_Click(object sender, EventArgs e)
        {
            this.grdviewCashList.AddNewRow();
        }

       /// <summary>
       /// 删除行
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void BtDel_Click(object sender, EventArgs e)
        {
            this.grdviewCashList.DeleteRow(this.grdviewCashList.FocusedRowHandle);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FrmCashRegisterII_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据0
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
        }

        /// <summary>
        /// 添加新行时，为新行初始化第0列的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdviewCashList_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            this.grdviewCashList.FocusedRowHandle = e.RowHandle;
            this.grdviewCashList.SetFocusedRowCellValue(this.grdviewCashList.Columns[0], Guid.NewGuid().ToString());
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}