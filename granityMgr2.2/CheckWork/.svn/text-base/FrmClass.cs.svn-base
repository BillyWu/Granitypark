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
    /// 班制定义，班制明细定义
    /// </summary>
    public partial class FrmClass : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "基本设置";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        public FrmClass()
        {
            InitializeComponent();
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
                this.ds.Tables["班制"].AcceptChanges();
                this.ds.Tables["班制明细"].AcceptChanges();
                XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void FrmClass_Load(object sender, EventArgs e)
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

        /// <summary>
        /// 增加班制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.ds.Tables["班制"];
            string code = BindManager.getCodeSn("班");
            DataRow dr = tab.NewRow();
            dr["ID"] = Guid.NewGuid().ToString();
            dr["名称"] = "新增班";
            dr["编号"] = "班" + code;
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }

        /// <summary>
        /// 删除班制
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtDel_Click(object sender, EventArgs e)
        {
            if (this.gridViewClass.RowCount > 0)
            {
                string id = this.gridViewClass.GetDataRow(this.gridViewClass.FocusedRowHandle)["id"].ToString();
                this.ds.Tables["班制"].Select("id='" + id + "'")[0].Delete();
            }
        }

       /// <summary>
       /// 增加班制明细
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void BtClassListAdd_Click(object sender, EventArgs e)
        {
            this.gridViewList.AddNewRow();
        }

        /// <summary>
        /// 删除班制明细
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtClassListDel_Click(object sender, EventArgs e)
        {
            if(this.gridViewList.RowCount==0)
            return ;
            string id = this.gridViewList.GetDataRow(this.gridViewList.FocusedRowHandle)["id"].ToString();
            this.gridViewList.DeleteRow(this.gridViewList.FocusedRowHandle);
            if (this.ds.Tables["班制明细"].Select("id='" + id + "'").Length > 0)
            {
                this.ds.Tables["班制明细"].Select("id='" + id + "'")[0].Delete();
            }
        }

        /// <summary>
        /// 增加班制明细时初始化新增的行数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridViewList_InitNewRow(object sender, DevExpress.XtraGrid.Views.Grid.InitNewRowEventArgs e)
        {
            if (this.gridViewClass.RowCount == 0)
                return;
            DataRow drr;
            string id = this.gridViewClass.GetDataRow(this.gridViewClass.FocusedRowHandle)["id"].ToString();
            this.gridViewList.FocusedRowHandle = e.RowHandle;
            string ClassListTag = Guid.NewGuid().ToString();
            string No = this.ds.Tables["班制明细"].Compute("max(班次)", "pid='" + id + "'").ToString();
            if (No == string.Empty || No == null)
            {
                No = "0";
            }
            else
            {
                int a = Int32.Parse(No) + 1;
                No = a.ToString();
            }
            this.gridViewList.SetRowCellValue(e.RowHandle, this.gridViewList.Columns["ID"], ClassListTag);
            this.gridViewList.SetRowCellValue(e.RowHandle, this.gridViewList.Columns["PID"], id);
            this.gridViewList.SetRowCellValue(e.RowHandle, this.gridViewList.Columns["班次"], No);
            this.gridViewList.SetRowCellValue(e.RowHandle, this.gridViewList.Columns["日期序号"], ConvertToWeekDayOf(int.Parse(No)));
          
            DataRow dr = this.ds.Tables["班制明细"].NewRow();
            dr["id"] = ClassListTag;
            dr["PID"] = id;
            dr["班次"] = No;
            dr["日期序号"] = ConvertToWeekDayOf(int.Parse(No));
            this.ds.Tables["班制明细"].Rows.Add(dr);
        }

        private string ConvertToWeekDayOf(int No)
        {
            string day = string.Empty;
            int no = No % 7;
            switch (no)
            {
                case 0:
                    day = "星期一";
                    break;
                case 1:
                    day = "星期二";
                    break;
                case 2:
                    day = "星期三";
                    break;
                case 3:
                    day = "星期四";
                    break;
                case 4:
                    day = "星期五";
                    break;
                case 5:
                    day = "星期六";
                    break;
                case 6:
                    day = "星期日";
                    break;
            }
            return day;
        }

        /// <summary>
        /// 班制改变时，班制对应的班制明细相应更改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridViewClass_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            DataTable tab = this.ds.Tables["班制明细"].Clone();
            string id = this.gridViewClass.GetDataRow(e.FocusedRowHandle)["id"].ToString();
            Granity.granityMgr.FunShare.GetTable(tab, this.ds.Tables["班制明细"].Select("pid='" + id + "'"));
            this.grdClassList.DataSource = tab;
        }
        
       /// <summary>
       /// 修改班制明细字段的值
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void gridViewList_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            string id = this.gridViewList.GetDataRow(e.RowHandle)["id"].ToString();
            if (this.ds.Tables["班制明细"].Select("id='" + id + "'").Length > 0)
            {
                DataRow dr = this.ds.Tables["班制明细"].Select("id='" + id + "'")[0];
                dr.BeginEdit();
                dr[e.Column.FieldName] = e.Value.ToString();
                dr.EndEdit();
            }
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}