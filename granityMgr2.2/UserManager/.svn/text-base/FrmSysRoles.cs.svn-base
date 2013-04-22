using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Granity.winTools;
using Estar.Common.Tools;
using Estar.Business.DataManager;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Granity.communications;
using Granity.CardOneCommi;
using System.Diagnostics;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;

namespace Granity.granityMgr.UserManager
{

    /// <summary>
    /// 角色定义
    /// </summary>
    public partial class FrmSysRoles : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "用户管理";

        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        public FrmSysRoles()
        {
            InitializeComponent();
        }

        private void FrmSysRoles_Load(object sender, EventArgs e)
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

        /// <summary>
        /// 生成唯一的用户编号
        /// </summary>
        /// <returns></returns>
        public string UserNum()
        {
            Guid tempGuid = Guid.NewGuid();
            byte[] bytes = tempGuid.ToByteArray();
            int i = Math.Abs(((int)bytes[1] << 6));
            return i.ToString();
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbUser.DataSource as DataTable;
            if (null == tab) return;
            DataRow dr = tab.NewRow();
            dr["编号"] = UserNum();
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1; ;
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            if (this.gridView1.RowCount == 0)
                return;
            DataTable tab = this.dbUser.DataSource as DataTable;
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
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("保存成功！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataTable tab = this.ds.Tables["角色定义"];
            dbUser.DataSource = tab;
        }
    }
}