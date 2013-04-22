using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using DevExpress.XtraEditors;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraEditors.Repository;
namespace Granity.granityMgr.CheckWork
{
    /// <summary>
    /// 员工考勤管理
    /// </summary>
    public partial class FrmEmployeeRegister : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 字典源(DataTable列扩展属性)
        /// </summary>
        private  string srcpro = "src";
        /// <summary>
        /// 字典源文本字段(DataTable列扩展属性)
        /// </summary>
        private  string txtpro = "txt";
        /// <summary>
        /// 字典值字段(DataTable列扩展属性)
        /// </summary>
        private  string valpro = "val";
        string unitName = "员工考勤管理";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        DataSet ds = null;
        /// <summary>
        /// 重复行id 用于删除列ID 有重复值的记录
        /// </summary>
        private string RepeatID = string.Empty;
        public FrmEmployeeRegister()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 通过制表控件的text值来判读用户选择的GridView.从而在选中的GridView中添加行
        /// 暂时这样写，有时间将case里面的语句抽象出来
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtAdd_Click(object sender, EventArgs e)
        {
            if (this.treDept.FocusedNode == null)
                return;
            string tag = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag.ToString()), "ID");
            string id = Guid.NewGuid().ToString();
            string ColumnName = string.Empty;
            Hashtable hs = new Hashtable();
            DataTable dtTemp = new DataTable();
            string TabTxt = this.TabRegisterClassInfo.SelectedTabPage.Text.Trim();
            switch (TabTxt)
            {
                case "签到":
                    ColumnName = this.gridViewSignIn.Columns["ID"].FieldName.ToString();
                    hs.Add(ColumnName, id);
                    hs.Add("部门id", tag);
                    AddTabRow(this.ds.Tables["员工签到"], hs, "id", id);
                    dtTemp =this.ds.Tables["员工签到"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["员工签到"].Select("部门id='" + tag + "'"));
                    this.grdSignIn.DataSource = dtTemp;
                    GetDeptEmployee(this.grdSignIn, this.ds.Tables["员工签到"], tag);
                    break;
                case "加班":
                    ColumnName = this.gridViewOvertime.Columns["ID"].FieldName.ToString();
                    hs.Add(ColumnName, id);
                    hs.Add("加班类别", "加班");
                    hs.Add("部门id", tag);
                    AddTabRow(this.ds.Tables["员工加班"], hs, "id", id);
                    dtTemp = this.ds.Tables["员工加班"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["员工加班"].Select("部门id='" + tag + "'"));
                    this.grdOvertime.DataSource = dtTemp;
                    GetDeptEmployee(this.grdOvertime, this.ds.Tables["员工加班"], tag);
                    break;
                case "出差":
                    ColumnName = this.gridViewEvection.Columns["ID"].FieldName.ToString();
                    hs.Add(ColumnName, id);
                    hs.Add("出差类别", "出差");
                    hs.Add("部门id", tag);
                    AddTabRow(this.ds.Tables["员工出差"], hs, "id", id);
                    dtTemp = this.ds.Tables["员工出差"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["员工出差"].Select("部门id='" + tag + "'"));
                    this.grdEvection.DataSource = dtTemp;
                    GetDeptEmployee(this.grdEvection, this.ds.Tables["员工出差"], tag);
                    break;
                case "调休":
                    ColumnName = this.gridViewExchangeRelax.Columns["ID"].FieldName.ToString();
                    hs.Add(ColumnName, id);
                    hs.Add("休假", CheckState.Checked);
                    hs.Add("部门id", tag);
                    AddTabRow(this.ds.Tables["部门员工假期"], hs, "id", id);
                    dtTemp = this.ds.Tables["部门员工假期"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["部门员工假期"].Select("部门id='" + tag + "'"));
                    this.grdExchangeRelax.DataSource = dtTemp;
                    GetDeptEmployee(this.grdExchangeRelax, this.ds.Tables["部门员工假期"], tag);
                    break;
                case "请假":
                    ColumnName = this.gridViewLeave.Columns["ID"].FieldName.ToString();
                    hs.Add(ColumnName, id);
                    hs.Add("请假类别", "请假");
                    hs.Add("部门id", tag);
                    AddTabRow(this.ds.Tables["员工请假"], hs, "id", id);
                    dtTemp = this.ds.Tables["员工请假"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["员工请假"].Select("部门id='" + tag + "'"));
                    this.grdLeave.DataSource = dtTemp;
                    GetDeptEmployee(this.grdLeave, this.ds.Tables["员工请假"], tag);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 绑定表格数据并且只根据部门来绑定过滤后的数据
        /// </summary>
        /// <param name="dbgrid">显示数据的表格</param>
        /// <param name="tab">数据表</param>
        /// <param name="tag">部门id</param>
        private void GetDeptEmployee(GridControl dbgrid, DataTable tab, string tag)
        {
            if (null == dbgrid || null == tab)
                return;
            GridView gridview = (GridView)dbgrid.MainView;
            DataTable dt = tab.Clone();
            if (!dt.Columns.Contains("部门id"))
                return;
            dt = FunShare.GetTable(dt, tab.Select("部门id='" + tag + "'"));
            dbgrid.DataSource = dt;
            DataColumnCollection dbcols = tab.Columns;
            if (null != gridview.Columns["ID"])
                gridview.Columns["ID"].Visible = false;
            foreach (DataColumn dbcol in dbcols)
            {
                //设置表格字典
                GridColumn gdcol = gridview.Columns.ColumnByFieldName(dbcol.ColumnName);
                if (null == gdcol)
                    gdcol = gridview.Columns.ColumnByName(dbcol.ColumnName);
                if (null == gdcol)
                    continue;
                string src = Convert.ToString(dbcol.ExtendedProperties[srcpro]);
                string txt = Convert.ToString(dbcol.ExtendedProperties[txtpro]);
                string val = Convert.ToString(dbcol.ExtendedProperties[valpro]);
                txt = string.IsNullOrEmpty(txt) ? val : txt;
                val = string.IsNullOrEmpty(val) ? txt : val;
                src = dbcol.ColumnName;
                if (null != tab.DataSet && !string.IsNullOrEmpty(src) && !string.IsNullOrEmpty(txt) && !string.IsNullOrEmpty(val)
                    && tab.DataSet.Tables.Contains(src))
                {
                    DataTable tabdict = tab.DataSet.Tables[src];
                    if (tabdict.Columns.Contains(txt) && tabdict.Columns.Contains(val))
                    {
                        RepositoryItemGridLookUpEdit ri = new RepositoryItemGridLookUpEdit();
                        DataTable dtDict = tabdict.Clone();
                        if (dtDict.Columns.Contains("id"))
                        {
                            dtDict = FunShare.GetTable(dtDict, tabdict.Select("id='" + tag + "'"));
                            ri.DataSource = dtDict;
                        }
                        else
                        {
                            ri.DataSource = tabdict;
                        }
                        //控制下拉的选项哪些可以显示，哪些不可以显示
                        if (ri.View.Columns.Contains(ri.View.Columns["ID"]))
                        {
                            ri.View.Columns["ID"].Visible = false;
                        }
                        ri.NullText = "请选择";
                        ri.DisplayMember = txt;
                        ri.ValueMember = val;
                        gdcol.ColumnEdit = ri;
                    }
                }
            }
        }

        /// <summary>
        /// 循环初始化所有DataGrid
        /// </summary>
        private void FrmDataGrid()
        {
            if (this.treDept.FocusedNode == null)
                return;
            string tag = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag.ToString()), "ID");
            DataTable dt = new DataTable();
            foreach (DataTable tab in this.ds.Tables)
            {
                dt = tab.Clone();
                if (!dt.Columns.Contains("部门id"))
                    continue;
                dt = FunShare.GetTable(dt, tab.Select("部门id='" + tag + "'"));
                switch (tab.TableName)
                {
                    case "部门员工假期":
                        this.grdExchangeRelax.DataSource = dt;
                        GetDeptEmployee(this.grdExchangeRelax, this.ds.Tables["部门员工假期"], tag);
                        break;
                    case "员工出差":
                        this.grdEvection.DataSource = dt;
                        GetDeptEmployee(this.grdEvection, this.ds.Tables["员工出差"], tag);
                        break;
                    case "员工加班":
                        this.grdOvertime.DataSource = dt;
                        GetDeptEmployee(this.grdOvertime, this.ds.Tables["员工加班"], tag);
                        break;
                    case "员工签到":
                        this.grdSignIn.DataSource = dt;
                        GetDeptEmployee(this.grdSignIn, this.ds.Tables["员工签到"], tag);
                        break;
                    case "员工请假":
                        this.grdLeave.DataSource = dt;
                        GetDeptEmployee(this.grdLeave, this.ds.Tables["员工请假"], tag);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 通过制表控件的text值来判读用户选择的GridView.从而在选中的GridView中删除行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtDel_Click(object sender, EventArgs e)
        {
            if (this.treDept.FocusedNode == null)
                return;
            string tag = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag.ToString()), "ID");
            string id = string.Empty;
            string TabTxt = this.TabRegisterClassInfo.SelectedTabPage.Text.Trim();
            switch (TabTxt)
            {
                case "签到":
                    if (this.gridViewSignIn.RowCount == 0)
                        return;
                    id = this.gridViewSignIn.GetDataRow(this.gridViewSignIn.FocusedRowHandle)["id"].ToString();
                    this.grdSignIn.DataSource = DelTabRow(this.ds.Tables["员工签到"], "id", id, "部门id", tag);
                    break;
                case "加班":
                    if (this.gridViewOvertime.RowCount == 0)
                        return;
                    id = this.gridViewOvertime.GetDataRow(this.gridViewOvertime.FocusedRowHandle)["id"].ToString();
                    this.grdOvertime.DataSource = DelTabRow(this.ds.Tables["员工加班"], "id", id, "部门id", tag);
                    break;
                case "出差":
                    if (this.gridViewEvection.RowCount == 0)
                        return;
                    id = this.gridViewEvection.GetDataRow(this.gridViewEvection.FocusedRowHandle)["id"].ToString();
                    this.grdEvection.DataSource = DelTabRow(this.ds.Tables["员工出差"], "id", id, "部门id", tag);
                    break;
                case "调休":
                    if (this.gridViewExchangeRelax.RowCount == 0)
                        return;
                    id = this.gridViewExchangeRelax.GetDataRow(this.gridViewExchangeRelax.FocusedRowHandle)["id"].ToString();
                    this.grdExchangeRelax.DataSource = DelTabRow(this.ds.Tables["部门员工假期"], "id", id, "部门id", tag);
                    break;
                case "请假":
                    if (this.gridViewLeave.RowCount == 0)
                        return;
                    id = this.gridViewLeave.GetDataRow(this.gridViewLeave.FocusedRowHandle)["id"].ToString();
                    this.grdLeave.DataSource = DelTabRow(this.ds.Tables["员工请假"], "id", id, "部门id", tag);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 删除表里的记录
        /// </summary>
        /// <param name="dt">被删除记录的表</param>
        /// <param name="ColValue"></param>
        /// <returns></returns>
        private DataTable DelTabRow(DataTable dt, string FirColName,string FirColValue,string SecColName,string SecColValue)
        {
            string Expression = FirColName + "='" + FirColValue + "'";
            foreach (DataRow dr in dt.Select(Expression))
            {
                dr.BeginEdit();
                dr.Delete();
                dr.EndEdit();
            }
            DataTable tab = dt.Clone();
            Expression = SecColName + "='" + SecColValue + "'";
            tab = FunShare.GetTable(tab, dt.Select(Expression));
            return tab;
        }

       /// <summary>
       /// 删除表里的记录,当有重复记录时，只删除一条记录,这是个根据业务特写的方法，不具通用性
       /// 主要是解决GridView AddNewRow 绑定自动生成的列，这里因实现方式不一样，不需要联动，否则
       /// 记录出错,数据库里会多保存一条记录
       /// </summary>
       /// <param name="dt">操作表</param>
       /// <param name="ColName">寻找的列</param>
       /// <param name="ColValue">寻找的列的值</param>
       /// <param name="FagColName">标志的列</param>
       /// <returns></returns>
        private DataTable DelTabRow(DataTable dt, string ColName, string ColValue, string FagColName)
        {
            if (dt == null)
                return dt;
            if (!dt.Columns.Contains(ColName.Trim()))
                return dt;
            string Expression = string.Empty;
            if (!dt.Columns.Contains(FagColName))
            {
                Expression = ColName.Trim() + "='" + ColValue.Trim() + "'";
                foreach (DataRow dr in dt.Select(Expression))
                {
                    dr.Delete();
                    break;
                }
            }
            else
            {
                Expression = ColName.Trim() + "='" + ColValue.Trim() + "' and " + FagColName.Trim() + " is null";
                foreach (DataRow dr in dt.Select(Expression))
                {
                    dr.Delete();
                    break;
                }
            }
            return dt;
        }

        private void BtSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void FrmEmployeeRegister_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            bindMgr = new BindManager(this);
            this.ds = bindMgr.BuildDataset(this.unitItem, this.paramwin);
        //    bindMgr.BindFld(this, this.ds);
            DataTable tab = this.ds.Tables["部门信息"];
            this.bindMgr.BindTrv(this.treDept, tab, "名称", "ID", "PID", "@ID={ID},@PID={PID},@编号={编号},@站址={站址}");
            this.treDept.ExpandAll();
            this.treDept.FocusedNodeChanged +=new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(treDept_FocusedNodeChanged);
            FrmDataGrid();
        }

        /// <summary>
        /// 在表新增一行，并给行赋值，支持多列值的修改
        /// </summary>
        /// <param name="dt">表</param>
        /// <param name="hst">哈希表</param>
        /// <param name="FagColumn">标志列名</param>
        /// <param name="FagColumnValue">标志列值</param>
        private void AddTabRow(DataTable dt, Hashtable hst, string FagColumn, string FagColumnValue)
        {
            string exeption = FagColumn + "='" + FagColumnValue + "'";
            if (dt.Select(exeption).Length > 0)
            {
                return;
            }
            Hashtable hs = hst;
            DataRow dr = dt.NewRow();
            dr.BeginEdit();
            foreach (DictionaryEntry de in hs)
            {
                dr[de.Key.ToString()] = de.Value;
            }
            dr.EndEdit();
            dt.Rows.Add(dr);
        }

        /// <summary>
        /// 修改datatable里记录的值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void gridViewCellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView focusView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            string gridViewName = focusView.Name.ToString();
            string id = focusView.GetDataRow(focusView.FocusedRowHandle)["ID"].ToString();
            string tag = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag.ToString()), "ID");
            DataTable dtTemp = new DataTable();
            switch( gridViewName )
            {
                case "gridViewExchangeRelax":
                    EditTabColValue(this.ds.Tables["部门员工假期"], e.Column.FieldName, e.Value.ToString(), id);
                    dtTemp = this.ds.Tables["部门员工假期"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["部门员工假期"].Select("部门id='" + tag + "'"));
                    this.grdExchangeRelax.DataSource = dtTemp; 
                    break;
                case "gridViewEvection":
                    EditTabColValue(this.ds.Tables["员工出差"], e.Column.FieldName, e.Value.ToString(),id);
                    dtTemp = this.ds.Tables["员工出差"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["员工出差"].Select("部门id='" + tag + "'"));
                    this.grdEvection.DataSource = dtTemp;
                    break;
                case "gridViewOvertime":
                    EditTabColValue(this.ds.Tables["员工加班"], e.Column.FieldName, e.Value.ToString(), id);
                    dtTemp = this.ds.Tables["员工加班"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["员工加班"].Select("部门id='" + tag + "'"));
                    this.grdOvertime.DataSource = dtTemp;
                    break;
                case "gridViewSignIn":
                    EditTabColValue(this.ds.Tables["员工签到"], e.Column.FieldName, e.Value.ToString(), id);
                    dtTemp = this.ds.Tables["员工签到"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["员工签到"].Select("部门id='" + tag + "'"));
                    this.grdSignIn.DataSource = dtTemp;
                    break;
                case "gridViewLeave":
                    EditTabColValue(this.ds.Tables["员工请假"], e.Column.FieldName, e.Value.ToString(), id);
                    dtTemp = this.ds.Tables["员工请假"].Clone();
                    dtTemp = FunShare.GetTable(dtTemp, this.ds.Tables["员工请假"].Select("部门id='" + tag + "'"));
                    this.grdLeave.DataSource = dtTemp;
                    break;
                default:
                    break; 
            }
        }

        /// <summary>
        /// 根据记录Tag,修改虚表记录的值(传递的表里一定有id字段，否则该方法无效)
        /// </summary>
        /// <param name="dt">表名</param>
        /// <param name="ColName">列名</param>
        /// <param name="ColValue">列值</param>
        /// <param name="Tag">记录id</param>
        private void EditTabColValue(DataTable dt,string ColName,string ColValue,string Tag)
        {
            if (!dt.Columns.Contains("id"))
                return;
            foreach (DataRow dr in dt.Select("id='" + Tag + "'"))
            {
                dr.BeginEdit();
                dr[ColName] = ColValue;
                dr.EndEdit();
            }
        }

        /// <summary>
        /// 树节点改变GridList 跟着改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void treDept_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            foreach (DataTable tab in this.ds.Tables)
            {
                string tag = basefun.valtag(Convert.ToString(e.Node.Tag.ToString()), "ID");
                if (tag == string.Empty)
                    return;
                if(!tab.Columns.Contains("部门id"))
                {
                    continue;  
                }
                DataRow[] drallInfo = tab.Select("部门id='" + tag + "'");
                DataTable tabInfo = tab.Clone();
                tabInfo = FunShare.GetTable(tabInfo, drallInfo);
                string tabName = tabInfo.TableName;
                switch (tabName)
                {
                    case "部门员工假期":
                        this.grdExchangeRelax.DataSource = tabInfo;
                        GetDeptEmployee(this.grdExchangeRelax, this.ds.Tables["部门员工假期"], tag);
                        break;
                    case "员工出差":
                        this.grdEvection.DataSource = tabInfo;
                        GetDeptEmployee(this.grdEvection, this.ds.Tables["员工出差"], tag);
                        break;
                    case "员工加班":
                        this.grdOvertime.DataSource = tabInfo;
                        GetDeptEmployee(this.grdOvertime, this.ds.Tables["员工加班"], tag);
                        break;
                    case "员工签到":
                        this.grdSignIn.DataSource = tabInfo;
                        GetDeptEmployee(this.grdSignIn, this.ds.Tables["员工签到"], tag);
                        break;
                    case "员工请假":
                        this.grdLeave.DataSource = tabInfo;
                        GetDeptEmployee(this.grdLeave, this.ds.Tables["员工请假"], tag);
                        break;
                    default:
                        break;
                }
            }
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CheckData()
        { }
    }
}