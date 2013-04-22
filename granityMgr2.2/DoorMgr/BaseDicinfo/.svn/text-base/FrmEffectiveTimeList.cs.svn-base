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
using System.Collections;
namespace Granity.granityMgr.BaseDicinfo
{
    /// <summary>
    /// 有效时段维护
    /// </summary>
    public partial class FrmEffectiveTimeList : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "门禁分组管理";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        DataSet ds = null;
        public FrmEffectiveTimeList()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 增加有效时段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.gridTimeList.DataSource as DataTable;
            DataRow dr = tab.NewRow();
            if (this.gridViewTimeList.RowCount > 0)
            {
                if (this.gridViewTimeList.RowCount > 253)
                    return;
                string No = tab.Compute("max(时段号)", "").ToString();
                dr["编号"] = Int32.Parse(No) + 1;
                dr["时段号"] = Int32.Parse(No) + 1;
                dr["名称"] = "有效时间" + No.ToString();
                dr["开始时间1"] = System.DateTime.Now.ToShortTimeString();
                dr["开始时间2"] = System.DateTime.Now.ToShortTimeString();
                dr["开始时间3"] = System.DateTime.Now.ToShortTimeString();
                dr["id"] = Guid.NewGuid().ToString();
            }
            else
            {
                int No = 2;
                dr["编号"] = No;
                dr["时段号"] = No;
                dr["名称"] = "有效时间" + No.ToString();
                dr["开始时间1"] = System.DateTime.Now.ToShortTimeString();
                dr["开始时间2"] = System.DateTime.Now.ToShortTimeString();
                dr["开始时间3"] = System.DateTime.Now.ToShortTimeString();
                dr["id"] = Guid.NewGuid().ToString();
            }
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count  - 1;
        }

        /// <summary>
        /// 删除有效时段
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtDel_Click(object sender, EventArgs e)
        {
            if (this.gridViewTimeList.RowCount == 0)
                return;
            DataTable tab = this.gridTimeList.DataSource as DataTable;
            if (tab == null || tab.Rows.Count < 0 )
                return;
            DataRowView drv = this.BindingContext[tab].Current as DataRowView;
            drv.Row.Delete();
        }

        private void BtSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                this.ds.Tables["有效时段"].AcceptChanges();
                XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Frm_EffectiveTimeList_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}