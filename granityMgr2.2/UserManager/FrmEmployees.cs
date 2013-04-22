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
using System.Text.RegularExpressions;


namespace Granity.granityMgr.UserManager
{
    /// <summary>
    /// 公司员工
    /// </summary>
    public partial class FrmEmployees : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "用户管理";
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        string DeptNo = "";
        string num = "";
        public FrmEmployees()
        {
            InitializeComponent();
        }
        private void FrmEmployees_Load(object sender, EventArgs e)
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
            DataTable tab = this.ds.Tables["用户信息"];
            this.bindMgr.BindTrv(this.treeUser, tab, "名称", "id", "PID", "@ID={ID},@PID={PID},@代码={代码},@名称={名称}");
            this.treeUser.ExpandAll();
            LoadSex();
            this.textEdit5.EditValueChanged += new EventHandler(txtEditValueChanged);
            this.textEdit4.EditValueChanged += new EventHandler(txtEditValueChanged);
        }
        /// <summary>
        /// 修改树节点text事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtEditValueChanged(object sender, EventArgs e)
        {
            DevExpress.XtraEditors.TextEdit txt = sender as DevExpress.XtraEditors.TextEdit;
            setTreeNodeText(this.treeUser, "名称", txt.Text.Trim());
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
        /// 性别
        public void LoadSex()
        {
            string[] Sex ={
                "男","女"};
            RepositoryItemComboBox riCombo = new RepositoryItemComboBox();
            for (int i = 0; i < Sex.Length; i++)
            {
                riCombo.Items.Add(Sex[i].ToString());
            }
            riCombo.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
            riCombo.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
            this.dbUser.RepositoryItems.Add(riCombo);
            this.gridView3.Columns["性别"].ColumnEdit = riCombo;
        }

        ExcelHander hander = new ExcelHander();
        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            this.txtExcelName.Text = openFileDialog1.FileName.ToString();
            if (openFileDialog1.FileName != null)
            {
                hander.GetSheelName(openFileDialog1.FileName.ToString(), excelName);
            }
        }
        /// <summary>
        /// 保存部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSaveDept_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("保存成功！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelDept_Click(object sender, EventArgs e)
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
        /// 添加部门
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddDept_Click(object sender, EventArgs e)
        {

            TreeListNode trnsel = this.treeUser.FocusedNode;
            if (trnsel == null)
                return;
            string pid = basefun.valtag(Convert.ToString(trnsel.Tag), "ID");
            DataTable tab = this.ds.Tables["用户信息"];
            DataRow drnew = tab.NewRow();
            drnew["ID"] = Guid.NewGuid().ToString();
            drnew["名称"] = "匿名部门";
            drnew["代码"] = BindManager.getCodeSn("");
            drnew["PID"] = pid;
            tab.Rows.Add(drnew);
            TreeListNode node;
            node = this.treeUser.AppendNode(new object[] { drnew["名称"].ToString(), drnew["ID"].ToString() }, this.treeUser.FocusedNode);
            node.Tag = "@ID=" + drnew["ID"].ToString() + ",@PID='',@编号=" + drnew["名称"].ToString() + "";
            node.ImageIndex = 1;
            this.treeUser.FocusedNode = node;
        }
        /// <summary>
        /// 根据部门查询
        /// </summary>
        public void GetUserByDept(string Num)
        {
            try
            {
                DataTable tab = this.dbUser.DataSource as DataTable;
                if (null == tab) return;
                string filter = "DEPARTMENT='{0}'";
                filter = string.Format(filter, Num.Replace("'", "''"));
                QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
                tab.Clear();
                query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.ds);
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 条件查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCheck_Click(object sender, EventArgs e)
        {
            string filter = "a.NAME like '%{0}%' or a.EMPCODE like '%{0}%' or b.名称 like '%{0}%'";
            filter = string.Format(filter, this.txtCheck.Text.Replace("'", "''"));
            //得到数据源
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            DataTable tab = this.dbUser.DataSource as DataTable;
            if (null == tab) return;
            tab.Clear();
            query.FillDataSet(tab.TableName, this.paramwin, ParamManager.setMacroParam(MacroPmType.FW, filter), this.ds);

        }
        /// <summary>
        /// 选择Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btlSeclect_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }
        /// <summary>
        /// 导入excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLoadExcel_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtExcelName.Text) || string.IsNullOrEmpty(this.excelName.Text))
            {
                MessageBox.Show("请选择Excel", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                if (hander.Getexcelds(openFileDialog1.FileName.ToString(), excelName.Text) != null)
                {
                    QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
                    DataTable tab = this.dbUser.DataSource as DataTable;
                    if (null == tab) return;
                    this.dbUser.DataSource = hander.Getexcelds(openFileDialog1.FileName.ToString(), excelName.Text); //绑定到界面
                    this.dbUser.DataMember = "公司员工";
                    DataSet empds = dbUser.DataSource as DataSet;
                    if (empds.Tables["公司员工"].Rows.Count < 1 || empds.Tables["公司员工"] == null) return;
                    foreach (DataRow Empdr in empds.Tables["公司员工"].Rows)
                    {
                        DataRow dr = tab.NewRow();
                        dr["用户编号"] = Convert.ToString(Empdr["用户编号"]);
                        dr["姓名"] = Convert.ToString(Empdr["姓名"]);
                        dr["部门名称"] = Convert.ToString(Empdr["部门名称"]);
                        //dr["性别"] = Convert.ToString(Empdr["性别"]);
                        //dr["职务"] = Convert.ToString(Empdr["职务"]);
                        //dr["生日"] = Convert.ToString(Empdr["生日"]);
                        //dr["身份证"] = Convert.ToString(Empdr["身份证"]);
                        //dr["地址"] = Convert.ToString(Empdr["地址"]);
                        //dr["Email"] = Convert.ToString(Empdr["Email"]);
                        //dr["学历"] = Convert.ToString(Empdr["学历"]);
                        //dr["联系电话"] = Convert.ToString(Empdr["联系电话"]);
                        tab.Rows.Add(dr);
                        this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
                    }
                }
            }
        }
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelEmp_Click(object sender, EventArgs e)
        {
            if (this.gridView3.RowCount == 0)
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
        /// 添加员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddEmp_Click(object sender, EventArgs e)
        {
            DataTable tab = this.dbUser.DataSource as DataTable;
            if (null == tab) return;
            DataRow dr = tab.NewRow();
            dr["用户编号"] = BindManager.getCodeSn(this.txtHeader.Text);
            if (string.IsNullOrEmpty(DeptNo)) DeptNo = "0";
            dr["部门名称"] = DeptNo;
            dr["车辆颜色"] = 0;
            dr["车型"] = 0;
            tab.Rows.Add(dr);
            this.BindingContext[tab].Position = this.BindingContext[tab].Count - 1;
        }
        /// <summary>
        /// 保存员工
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEmpAdd_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                XtraMessageBox.Show("保存成功！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DataTable tab = this.ds.Tables["公司员工"];
            dbUser.DataSource = tab;
        }
        /// <summary>
        /// 数据合法性验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridView3_CellValueChanged_1(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            //Email
            Regex EmailReg = new Regex(@"^\w+([\-+.]\w+)*@\w+([\-.]\w+)*\.\w+([\-.]\w+)*$");
            string Emailval = this.gridView3.GetDataRow(e.RowHandle)["Email"].ToString();
            if (!EmailReg.IsMatch(Emailval))
                this.gridView3.GetDataRow(e.RowHandle)["Email"] = "";
            //身份证
            Regex IDCardReg18 = new Regex(@"^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}(\d|x|X)$");
            string IDCardVal = gridView3.GetDataRow(e.RowHandle)["身份证"].ToString();
            if (!IDCardReg18.IsMatch(IDCardVal))
                this.gridView3.GetDataRow(e.RowHandle)["身份证"] = "";
            //电话
            Regex PhoneReg = new Regex(@"((\d{11})|^((\d{7,8})|(\d{4}|\d{3})-(\d{7,8})|(\d{4}|\d{3})-(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1})|(\d{7,8})-(\d{4}|\d{3}|\d{2}|\d{1}))$)");
            string PhoneVal = gridView3.GetDataRow(e.RowHandle)["联系电话"].ToString();
            if (!PhoneReg.IsMatch(PhoneVal))
                this.gridView3.GetDataRow(e.RowHandle)["联系电话"] = "";
            if (string.IsNullOrEmpty(gridView3.GetDataRow(e.RowHandle)["姓名"].ToString()))
                this.gridView3.GetDataRow(e.RowHandle)["姓名"] = "不能为空";
            //验证用户编号是否已经被使用
            string[] str = new string[this.gridView3.RowCount];
            if (this.gridView3.RowCount < 1) return;
            for (int i = 0; i < this.gridView3.RowCount; i++)
            {
                if (e.RowHandle == i) str[i] = "0";
                else
                    str[i] = Convert.ToString(this.gridView3.GetDataRow(i)["用户编号"]);
                if (Convert.ToString(this.gridView3.GetDataRow(e.RowHandle)["用户编号"]) == str[i].ToString())
                {
                    XtraMessageBox.Show("此编号已经使用，请重新写入！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.gridView3.GetDataRow(e.RowHandle)["用户编号"] = "0";
                    return;
                }
            }
        }
        /// <summary>
        /// 退出系统
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtClose_Click(object sender, EventArgs e)
        {
            DialogResult msgquet = XtraMessageBox.Show("确定关闭窗体!", "系统提示！", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            if (DialogResult.Cancel == msgquet)
                return;
            else
                this.Close();
        }
        /// <summary>
        /// 焦点数改变事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treeUser_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
        {
            if (e.Node == null)
            {
                return;
            }
            int rowIndex = 0;
            string tag = basefun.valtag(Convert.ToString(e.Node.Tag), "ID");
            num = basefun.valtag(Convert.ToString(e.Node.Tag), "代码");
            DeptNo = basefun.valtag(Convert.ToString(e.Node.Tag), "代码");
            DataTable tab = this.ds.Tables["用户信息"];
            rowIndex = 0;
            if (tab == null) return;
            foreach (DataRow dr in tab.Rows)
            {
                rowIndex++;
                if (DataRowState.Deleted != dr.RowState && dr["id"].ToString() == tag)
                {
                    BindingManagerBase bindMgrbase = this.BindingContext[tab];
                    if (bindMgrbase != null)
                    {
                        bindMgrbase.Position = rowIndex - 1;
                    }
                    break;
                }
            }
            GetUserByDept(num);
        }
    }
}