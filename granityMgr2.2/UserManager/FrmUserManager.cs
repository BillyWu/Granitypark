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
using DevExpress.XtraGrid.Views.Grid;

namespace Granity.granityMgr.UserManager
{
    /// <summary>
    /// 系统用户管理,以及部门管理
    /// </summary>
    public partial class FrmUserManager : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "用户管理集团";

        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        public FrmUserManager()
        {
            InitializeComponent();
        }
        private void UserManager_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            for (int i = 0; i < this.unitItem.WorkItemList.Length; i++)
            {
                if ("用户管理" != this.unitItem.WorkItemList[i].ItemName)
                    continue;
                WorkItem wk = unitItem.WorkItemList[i];
                for (int j = 0; j < wk.DictCol.Length; j++)
                {
                    DictColumn col = wk.DictCol[j];
                    if ("姓名" != col.ColumnName)
                        continue;
                    col.DataSrc = col.TextCol = col.ValueCol = "";
                    break;
                }
                break;
            }
            //绑定数据
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
            //对树的操作
            DataTable tab = this.ds.Tables["orgtreegroup"];
            this.bindMgr.BindTrv(this.treeUser, tab, "部门名称", "ID", "PID", "@ID={ID},@PID={PID},@部门={部门},@部门名称={部门名称}");
            this.treeUser.ExpandAll();
            this.txtName.EditValueChanged += new EventHandler(txtEditValueChanged);
            this.txtNum.EditValueChanged += new EventHandler(txtEditValueChanged);
        }
        /// <summary>
        /// 修改树节点text事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = sender as DevExpress.XtraEditors.TextEdit;
            setTreeNodeText(this.treeUser, "部门名称", txt.Text.Trim());
        }
        /// <summary>
        /// 修改树节点的text值
        /// </summary>
        /// <param name="trv">树</param>
        /// <param name="colName">树节点列名</param>
        /// <param name="colValue">列值</param>
        private void setTreeNodeText(TreeList trv, string colName, string colValue)
        {
            try
            {
                if (trv == null || colName == null || colValue == string.Empty)
                    return;
                if (trv.Nodes.Count == 0)
                    return;
                trv.FocusedNode.SetValue(trv.Columns[colName], colValue.Trim());
            }
            catch
            { }
        }

        /// <summary>
        /// 添加树节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bbtnPAdd_Click(object sender, EventArgs e)
        {
            TreeListNode trnsel = this.treeUser.FocusedNode;
            if (trnsel == null)
                return;
            string pid = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            DataTable tab = this.ds.Tables["orgtreegroup"];
            DataRow drnew = tab.NewRow();
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["部门名称"] = "匿名部门";
            drnew["部门"] = basefun.valtag(Convert.ToString(trnsel.Tag), "部门") + tab.Rows.Count + 1 + "0";
            drnew["PID"] = pid;
            tab.Rows.Add(drnew);
            TreeListNode node;
            node = this.treeUser.AppendNode(new object[] { drnew["部门名称"].ToString(), drnew["ID"].ToString() }, this.treeUser.FocusedNode);
            string tag = "@ID={ID},@PID={PID},@部门={部门},@部门名称={部门名称}";
            foreach (DataColumn col in tab.Columns)
                tag = tag.Replace(col.ColumnName, Convert.ToString(drnew[col]));
            node.Tag = tag;
            node.ImageIndex = 1;
            this.treeUser.FocusedNode = node;
        }
        /// <summary>
        /// 保存树节点
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

        }
        /// <summary>
        /// 删除树节点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDel_Click(object sender, EventArgs e)
        {
            TreeListNode trn = this.treeUser.FocusedNode;
            if (null == trn)
                return;
            string tip = "确定是否删除当前节点！";
            if (trn.Nodes.Count > 0)
                tip += ",同时将删除该节点的下级关联节点！";
            DialogResult msgquet = XtraMessageBox.Show(tip, "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            //递归删除节点
            TreeListNode trncurrent = trn;
            DevExpress.XtraTreeList.Nodes.TreeListNode trnnext = trn.PrevNode;
            DataTable tabGroup = this.ds.Tables["用户信息"];
            string tag = basefun.valtag(Convert.ToString(trn.Tag), "ID");
            foreach (DataRow drGroup in tabGroup.Select("id='" + tag + "'"))
            {
                if (this.treeUser.FindNodeByFieldValue("id", drGroup["id"].ToString()) != null)
                {
                    TreeListNode tn1 = this.treeUser.FindNodeByFieldValue("id", drGroup["id"].ToString());
                    this.treeUser.Nodes.Remove(tn1);
                }
                drGroup.Delete();
                trnnext = trn.PrevNode;
                if (null == trnnext)
                    trnnext = trn.ParentNode;
                trn.Nodes.Remove(trn);
                if (trncurrent != trn)
                    trn = trnnext;
                else
                    trn = null;
            }
            this.treeUser.FocusedNode = trnnext;
        }
        /// <summary>
        /// 添加帐号信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnAddOp_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbUser.DataSource as DataTable;
            TreeListNode trn = this.treeUser.FocusedNode;
            if (null == tab || null == trn || string.IsNullOrEmpty(Convert.ToString(trn.Tag)))
                return;
            DataRow dr = tab.NewRow();
            dr["编号"] = BindManager.getCodeSn("");
            dr["ID"] = Guid.NewGuid();
            string tag = Convert.ToString(trn.Tag);
            dr["部门"] = basefun.valtag(tag, "部门");
            dr["部门名称"] = basefun.valtag(tag, "部门名称");
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (this.dbUserView.RowCount < 1)
                return;
            DataRow dr = this.dbUserView.GetDataRow(this.dbUserView.FocusedRowHandle);
            NameObjectList ps = ParamManager.createParam(dr);
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            if (!query.ExecuteNonQuery("用户密码初始化", ps, ps, ps))
                MessageBox.Show("重置密码失败！", "重置密码", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show("成功重置密码为：abc123\r\n请及时修改密码确保安全", "重置密码", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 保存操作员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveOp_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("保存成功！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataTable tab = this.ds.Tables["高级用户管理"];
            dbUser.DataSource = tab;
        }
        /// <summary>
        /// 删除操作员
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelOp_Click(object sender, EventArgs e)
        {
            if (this.dbUserView.RowCount == 0)
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
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }

        /// <summary>
        /// 用户编号检查是否重复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dbUserView_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            GridView view = sender as GridView;
            if (null == e || null == view || "编号" != e.Column.FieldName)
                return;
            string val = Convert.ToString(e.Value);
            DataTable tab = view.GridControl.DataSource as DataTable;
            if (null == tab) return;
            bool isVai = true;
            for (int i = 0; i < view.RowCount; i++)
            {
                if (i == e.RowHandle)
                    continue;
                if (val != Convert.ToString(view.GetRowCellValue(i, e.Column)))
                    continue;
                view.SetRowCellValue(e.RowHandle, e.Column, "");
                isVai = false;
                break;
            }
            if (!isVai)
                XtraMessageBox.Show("此编号已经使用，请重新写入！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        /// <summary>
        /// 导航树节点改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeUser_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (null == e || null == e.Node)
                return;
            int rowIndex = 0;
            string id = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
            DataTable tab = this.ds.Tables["orgtreegroup"];
            BindingManagerBase bindMgrbase = this.BindingContext[tab];
            NameObjectList ps = new NameObjectList();
            rowIndex = 0;
            foreach (DataRow dr in tab.Rows)
            {
                if (DataRowState.Deleted != dr.RowState && dr["id"].ToString() == id)
                {
                    bindMgrbase.Position = rowIndex;
                    ps["部门"] = Convert.ToString(dr["部门"]);
                    break;
                }
                rowIndex++;
            }
            tab = this.dbUser.DataSource as DataTable;
            if (null == tab) return;
            tab.Clear();
            this.Query.FillDataSet(tab.TableName, ps, tab.DataSet);
        }
    }
}