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
    /// 消费查询
    /// </summary>
    public partial class FrmConsumeRecord : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "消费查询";//项目
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        public FrmConsumeRecord()
        {
            InitializeComponent();
        }

        private void FrmConsumeRecord_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据0
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            this.dateStart.EditValue = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            this.dateEnd.EditValue = System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm");
            ps["StartDate"] = this.dateStart.EditValue.ToString();
            ps["EndDate"] = this.dateEnd.EditValue.ToString();
            ds = bg.BuildDataset(this.unitItem, ps);
            bg.BindFld(this, ds);
        }

        /// <summary>
        /// 根据条件，查询出相应的记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtQuery_Click(object sender, EventArgs e)
        {
           if ( this.dateStart.Text ==string.Empty )
           {
               XtraMessageBox.Show("请输入开始时间","系统提示！");
               return;
           }
           if (this.dateEnd.Text == string.Empty)
           {
               XtraMessageBox.Show("请输入结束时间", "系统提示！");
               return;
           }

           NameObjectList pstrans = new NameObjectList();
           ParamManager.MergeParam(pstrans, this.paramwin);
           pstrans["StartDate"] = this.dateStart.EditValue.ToString();
           pstrans["EndDate"] = this.dateEnd.EditValue.ToString();
           //查询数据
           QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
           DataTable tab = this.grdConsumeList.DataSource as DataTable;
           if (null == tab) return;
           tab.Clear();
           query.FillDataSet(tab.TableName, pstrans, this.ds);
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void grdviewCashList_CustomDrawRowIndicator(object sender, DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = Convert.ToString(Convert.ToInt32(e.RowHandle) + 1);
            }
        }

        private void btPrint_Click(object sender, EventArgs e)
        {
            this.grdConsumeList.ShowPrintPreview();
        }
    }
}