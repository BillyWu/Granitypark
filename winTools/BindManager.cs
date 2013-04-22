#region 版本说明

/*
 * 功能内容：   win窗口数据绑定管理功能,及系统环境的管理
 *
 * 作    者：   王荣策
 *
 * 审 查 者：   王荣策
 *
 * 日    期：   2010-05-27
 */

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using Estar.Common.Tools;
using Estar.Business.UserRight;
using System.Text.RegularExpressions;
using Estar.Business.DataManager;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.IO;
using System.Drawing.Imaging;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraEditors;
using System.Threading;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraTreeList.Columns;
namespace Granity.winTools
{
    /// <summary>
    /// 对windows应用的数据绑定管理,提供统一的处理方式
    /// </summary>
    public class BindManager
    {
        #region 内部规则常量
        /// <summary>
        /// 绑定数据源(tag标记名称)
        /// </summary>
        private const string dbpro = "db";
        /// <summary>
        /// 绑定字段(tag标记名称)
        /// </summary>
        private const string fldpro = "fld";
        
        /// <summary>
        /// 字典源(DataTable列扩展属性)
        /// </summary>
        private const string srcpro = "src";
        /// <summary>
        /// 字典源文本字段(DataTable列扩展属性)
        /// </summary>
        private const string txtpro = "txt";
        /// <summary>
        /// 字典值字段(DataTable列扩展属性)
        /// </summary>
        private const string valpro = "val";
        /// <summary>
        /// 关联单元子数据项(DataTable列扩展属性)
        /// </summary>
        private const string itempro = "itemname";
        /// <summary>
        /// 列宽(DataTable列扩展属性)
        /// </summary>
        private const string widthpro = "width";
        /// <summary>
        /// 是否可见(DataTable列扩展属性)
        /// </summary>
        private const string visipro = "visible";
        /// <summary>
        /// 是否只读(DataTable列扩展属性)
        /// </summary>
        private const string readpro = "readonly";
        /// <summary>
        /// 校验表达式(DataTable列扩展属性)
        /// </summary>
        private const string validpro = "validexp";
        /// <summary>
        /// 是否可空,(DataTable列扩展属性)
        /// </summary>
        private const string isnullpro = "isNULL";

        /// <summary>
        /// 验证是否是SQL语句
        /// </summary>
        private static readonly Regex regexSQL = new Regex(@"\b(execute|exec|select|from)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        /// <summary>
        /// 验证是否是整数
        /// </summary>
        private static readonly Regex regexINT = new Regex(@"^\d*$", RegexOptions.Compiled);

        #endregion

        //系统当前用户
        private static User user = null;

        //多个窗口打开时的传递参数,再调用获取后置空
        //每次设置传递参数时则创建新参数
        private static NameObjectList transParam = null;

        //当前窗口
        private Form win;
       
        private ToolTip tip;
        /// <summary>
        /// 处理单元数据表校验及计算的单行数据表
        /// 在每个数据表第一次使用时克隆并创建结构
        /// </summary>
        private DataSet dsCompte = new DataSet();

        /// <summary>
        /// 构造函数,初始化绑定管理所在的窗口
        /// </summary>
        /// <param name="win"></param>
        public BindManager(Form win)
        {
            this.win = win;
            tip = new ToolTipMsg();
        }

        /// <summary>
        /// 检查win 里是否有树控件，有树控件，tree 指向它，否则为Null
        /// </summary>
        private TreeList tree =null;
        /// <summary>
        /// 数据项属性写入数据表扩展属性,便于绑定时处理
        /// </summary>
        /// <param name="tab">数据表</param>
        /// <param name="item">数据项</param>
        private static void setTablepro(DataTable tab, WorkItem item)
        {
            if (null == tab || null == item)
                return;
            if (!string.IsNullOrEmpty(item.ColumnKey) && tab.Columns.Contains(item.ColumnKey))
                tab.PrimaryKey = new DataColumn[] { tab.Columns[item.ColumnKey] };
            tab.ExtendedProperties[BindManager.itempro] = item.ItemName;
            DataColumnCollection dbcols = tab.Columns;
            foreach (DictColumn col in item.DictCol)
            {
                if (!dbcols.Contains(col.ColumnName))
                    continue;
                DataColumn dbcol = dbcols[col.ColumnName];
                dbcol.ExtendedProperties[BindManager.widthpro] = col.BarWidth;
                dbcol.ExtendedProperties[BindManager.visipro] = col.Visible;
                dbcol.ExtendedProperties[BindManager.readpro] = col.IsReadOnly;
                dbcol.ExtendedProperties[BindManager.srcpro] = col.DataSrc;
                dbcol.ExtendedProperties[BindManager.txtpro] = col.TextCol;
                dbcol.ExtendedProperties[BindManager.valpro] = col.ValueCol;
                dbcol.ExtendedProperties[BindManager.validpro] = col.ValidateCell.Trim();
                if (col.IsNeed)
                    dbcol.ExtendedProperties[BindManager.isnullpro] = false;
                if (item.LinkCol == col.ColumnName)
                    dbcol.ExtendedProperties[BindManager.visipro] = false;
                if (!string.IsNullOrEmpty(col.Title))
                    dbcol.Caption = col.Title;
            }
        }

        #region 数据处理

        /// <summary>
        /// 初始化创建单元数据集
        /// </summary>
        /// <param name="unitItem">单元实例</param>
        /// <param name="ps">传递的环境参数</param>
        /// <returns>单元的数据集</returns>
        public DataSet BuildDataset(UnitItem unitItem, NameObjectList ps)
        {
            DataSet ds = new DataSet(unitItem.UnitName);
            QueryDataRes query = new QueryDataRes(unitItem.DataSrcFile);
            QueryDataRes queryD = query;
            if (!string.IsNullOrEmpty(unitItem.DictColSrcFile) && unitItem.DictColSrcFile != unitItem.DataSrcFile)
                queryD = new QueryDataRes(unitItem.DictColSrcFile);
            foreach (WorkItem item in unitItem.WorkItemList)
            {
                if (string.IsNullOrEmpty(item.DataSrc))
                    continue;
                //填充数据项
                DataTable tab = null;
                string[] psmacro = ParamManager.setMacroParam(MacroPmType.FW, item.InitFilter);
                if (string.IsNullOrEmpty(item.CountDataSrc))
                {
                    if (!BindManager.regexSQL.IsMatch(item.DataSrc))
                        tab = query.getTable(item.DataSrc, ps, psmacro);
                    else
                        tab = query.GetDataTableBySql(item.DataSrc);
                }
                else
                {
                    string[] psString = ParamManager.setMacroParam(MacroPmType.FW, item.InitFilter);
                    tab = query.getTable(item.CountDataSrc, ps, psmacro);
                    if (null != tab && tab.Rows.Count > 0 && tab.Columns.Contains("记录数量"))
                        psString = ParamManager.setMacroParam(psString, MacroPmType.topnum, Convert.ToString(tab.Rows[0]["记录数量"]));
                    tab = query.getTable(item.DataSrc, ps, psmacro);
                }
                if (null == tab) continue;
                //填充字典数据集
                foreach (DictColumn c in item.DictCol)
                {
                    if (string.IsNullOrEmpty(c.DataSrc) || string.IsNullOrEmpty(c.ValueCol))
                        continue;
                    if (!tab.Columns.Contains(c.ColumnName) || ds.Tables.Contains(c.DataSrc))
                        continue;
                    if (!BindManager.regexSQL.IsMatch(c.DataSrc))
                        queryD.FillDataSet(c.DataSrc, ps, ds);
                    else
                    {
                        DataTable tabtemp = queryD.GetDataTableBySql(c.DataSrc);
                        tabtemp.TableName = c.ColumnName;
                        ds.Tables.Add(tabtemp);
                    }
                }
                ds.Tables.Add(tab);
                BindManager.setTablepro(tab, item);
            }
            return ds;
        }

        /// <summary>
        /// 设置表格列字段，列名用逗号分割
        /// </summary>
        /// <param name="dbgrid">表格控件</param>
        /// <param name="cols">需要显示的列名,逗号分割,为空默认不处理</param>
        public void SetGridCols(DataGridView dbgrid, string cols)
        {
            if (null == dbgrid || dbgrid.Columns.Count < 1 || string.IsNullOrEmpty(cols))
                return;
            //设置字段是否显示及标题
            cols = "," + cols + ",";
            foreach (DataGridViewColumn col in dbgrid.Columns)
            {
                if (!Regex.IsMatch(cols, ",\\s*" + col.Name + "(\\s|,)"))
                    col.Visible = false;
            }
            DataTable tab = null;
            if (dbgrid.DataSource is DataSet)
                tab = ((DataSet)dbgrid.DataSource).Tables[dbgrid.DataMember];
            else if (dbgrid.DataSource is DataTable)
                tab = dbgrid.DataSource as DataTable;
            //排序字段顺序
            int index = 0;
            string[] flds = cols.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string fld in flds)
            {
                string[] str = fld.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (str.Length < 1)
                    continue;
                for (int i = 0; i < str.Length; i++)
                    str[i] = str[i].Trim();
                if (!dbgrid.Columns.Contains(str[0]))
                    continue;
                dbgrid.Columns[str[0]].DisplayIndex = index++;
                if (str.Length > 1 && !string.IsNullOrEmpty(str[1]) && !BindManager.regexINT.IsMatch(str[1]))
                {
                    dbgrid.Columns[str[0]].HeaderText = str[1];
                    if (null != tab) tab.Columns[str[0]].Caption = str[1];
                }
                if (str.Length > 1 && !string.IsNullOrEmpty(str[1]) && BindManager.regexINT.IsMatch(str[1]))
                    dbgrid.Columns[str[0]].Width = Convert.ToInt16(str[1]);
                else if (str.Length > 2 && !string.IsNullOrEmpty(str[2]) && BindManager.regexINT.IsMatch(str[2]))
                    dbgrid.Columns[str[0]].Width = Convert.ToInt16(str[2]);
            }
        }

        /// <summary>
        /// 设置表格列字段，列名用逗号分割
        /// </summary>
        /// <param name="dbgrid">表格控件</param>
        /// <param name="cols">需要显示的列名,逗号分割,为空默认不处理</param>
        public void SetGridCols(GridControl dbgrid, string cols)
        {
            if (null == dbgrid || string.IsNullOrEmpty(cols))
                return;
            //设置字段是否显示及标题
            cols = "," + cols + ",";
            GridView gridview = (GridView)dbgrid.MainView;
            if (gridview.Columns.Count < 1)
                return;
            foreach (GridColumn col in gridview.Columns)
            {
                if (!Regex.IsMatch(cols, ",\\s*" + col.Name + "(\\s|,)")
                    && !Regex.IsMatch(cols, ",\\s*" + col.FieldName + "(\\s|,)"))
                    col.Visible = false;
            }
            DataTable tab = null;
            if (dbgrid.DataSource is DataSet)
                tab = ((DataSet)dbgrid.DataSource).Tables[dbgrid.DataMember];
            else if (dbgrid.DataSource is DataTable)
                tab = dbgrid.DataSource as DataTable;
            //排序字段顺序
            int index = 0;
            string[] flds = cols.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string fld in flds)
            {
                string[] str = fld.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (str.Length < 1)
                    continue;
                for (int i = 0; i < str.Length; i++)
                    str[i] = str[i].Trim();
                GridColumn gcol = gridview.Columns[str[0]];
                if (null == gcol)
                    gcol = gridview.Columns.ColumnByName(str[0]);
                if (null == gcol)
                    continue;
                gcol.VisibleIndex = index++;
                if (str.Length > 1 && !string.IsNullOrEmpty(str[1]) && !BindManager.regexINT.IsMatch(str[1]))
                {
                    gcol.Caption = str[1];
                    if (null != tab) tab.Columns[str[0]].Caption = str[1];
                }
                if (str.Length > 1 && !string.IsNullOrEmpty(str[1]) && BindManager.regexINT.IsMatch(str[1]))
                    gcol.Width = Convert.ToInt16(str[1]);
                else if (str.Length > 2 && !string.IsNullOrEmpty(str[2]) && BindManager.regexINT.IsMatch(str[2]))
                    gcol.Width = Convert.ToInt16(str[2]);
            }
        }

        /// <summary>
        /// 使用更改的数据表tabChanged来更新数据表tabData,状态无修改和新增加的不更新
        /// 数据表都要有相同的结构和相同的主键
        /// </summary>
        /// <param name="tabData">数据表</param>
        /// <param name="tabChanged">对比的数据表</param>
        /// <param name="addfilter">更新记录的过滤表达式</param>
        public static void UpdateTable(DataTable tabData, DataTable tabChanged, string filter)
        {
            if (null == tabData || null == tabChanged || tabChanged.Rows.Count < 1)
                return;
            //更新数据,如果tabchanged有但tabpk没有的记录且不是Added则忽略更新
            DataView dvsub = new DataView(tabChanged, filter, "", DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted);
            int len = dvsub.Table.Columns.Count;
            foreach (DataRowView drv in dvsub)
            {
                object[] dbitem = null;
                if (DataRowState.Deleted != drv.Row.RowState)
                    dbitem = drv.Row.ItemArray;
                else
                {
                    List<object> list = new List<object>();
                    for (int i = 0; i < len; i++)
                        list.Add(drv[i]);
                    dbitem = list.ToArray();
                }
                DataRow dr = tabData.LoadDataRow(dbitem, true);
                switch (drv.Row.RowState)
                {
                    case DataRowState.Added: dr.SetAdded(); break;
                    case DataRowState.Modified: dr.SetModified(); break;
                    case DataRowState.Deleted: dr.Delete(); break;
                }
            }
        }

        /// <summary>
        /// 对控件具有的字段值赋值,控件tag标记或colmap映射字段
        /// </summary>
        /// <param name="tab">数据表</param>
        /// <param name="ct">控件容器</param>
        /// <param name="tagName">控件tag标记字段的名称,标记值是字段或字段的标题,无对应则使用colmap再映射,再无则忽略</param>
        /// <param name="colmap">字段映射(可空):tagName—colmap[tagname,fld]—tab.fld</param>
        public static void SetControlValue(DataRow dr, Control ct, string tagName, IDictionary<string, string> colmap)
        {
            if (null == dr || null == ct || string.IsNullOrEmpty(tagName))
                return;
            DataTable tab = dr.Table;
            string tag = Convert.ToString(ct.Tag);
            string fld = basefun.valtag(tag, tagName);
            if (!string.IsNullOrEmpty(fld))
            {
                //映射字段: fld-col-title
                DataColumn dbcol = null;
                if (tab.Columns.Contains(fld))
                    dbcol = tab.Columns[fld];
                else
                {
                    string col = null == colmap ? "" : colmap[fld];
                    if (!string.IsNullOrEmpty(col) && tab.Columns.Contains(col))
                        dbcol = tab.Columns[col];
                    else
                    {
                        foreach (DataColumn c in tab.Columns)
                        {
                            if (fld != c.Caption && col != c.Caption)
                                continue;
                            dbcol = c;
                            break;
                        }
                    }
                }
                //对控件赋值
                if (null != dbcol)
                {
                    if (ct is ListControl)
                        ((ListControl)ct).SelectedValue = dr[dbcol];
                    else if (ct is CheckBox)
                        ((CheckBox)ct).Checked = true.Equals(dr[dbcol]);
                    else
                        ct.Text = Convert.ToString(dr[dbcol]);
                }
            }
            //递归赋值
            foreach (Control c in ct.Controls)
                SetControlValue(dr, c, tagName, colmap);
        }

        /// <summary>
        /// 对记录行的字段值赋值,字段名称或标题或映射相同
        /// </summary>
        /// <param name="drsrc">源行记录</param>
        /// <param name="drdest">需要更新的目标行记录</param>
        /// <param name="colmap">列映射</param>
        public static void SetDataRowValue(DataRow drsrc, DataRow drdest, IDictionary<string, string> colmap)
        {
            if (null == drsrc || null == drdest)
                return;
            DataColumnCollection colssrc = drsrc.Table.Columns;
            DataColumnCollection colsdest = drdest.Table.Columns;
            foreach (DataColumn csrc in colssrc)
            {
                string fld = csrc.ColumnName;
                string title = csrc.Caption;
                string col = null == colmap ? "" : colmap.ContainsKey(fld) ? colmap[fld] : colmap.ContainsKey(fld) ? colmap[title] : "";
                DataColumn dbcol = null;
                if (colsdest.Contains(fld) || colsdest.Contains(title) || colsdest.Contains(col))
                    dbcol = colsdest.Contains(fld) ? colsdest[fld] : (colsdest.Contains(title) ? colsdest[title] : colsdest[col]);
                else
                    foreach (DataColumn cdest in colsdest)
                    {
                        string caption = cdest.Caption;
                        if (caption != fld && caption != title && caption != col)
                            continue;
                        dbcol = cdest;
                        break;
                    }
                if (null != dbcol)
                    drdest[dbcol] = drsrc[csrc];
            }
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="query">数据查询</param>
        /// <param name="ds">数据集,更新的数据项是数据表名称</param>
        /// <param name="ps">环境参数</param>
        /// <returns>是否执行成功</returns>
        public static bool Save(QueryDataRes query, DataSet ds, NameObjectList ps)
        {
            if (null == query || null == ds || ds.Tables.Count < 1)
                return true;
            foreach (DataTable tab in ds.Tables)
            {
                NameObjectList[] psins = ParamManager.createParam(tab, DataRowState.Added);
                NameObjectList[] psupt = ParamManager.createParam(tab, DataRowState.Modified);
                NameObjectList[] psdel = ParamManager.createParam(tab, DataRowState.Deleted);
                foreach (NameObjectList p in psins)
                    ParamManager.MergeParam(p, ps, false);
                foreach (NameObjectList p in psupt)
                    ParamManager.MergeParam(p, ps, false);
                foreach (NameObjectList p in psdel)
                    ParamManager.MergeParam(p, ps, false);
                if (psins.Length < 1 && psupt.Length < 1 && psdel.Length < 1)
                    continue;
                bool isSuccess = query.ExecuteNonQuery(tab.TableName, psins, psupt, psdel);
                if (!isSuccess) return isSuccess;
                tab.AcceptChanges();
            }
            return true;
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="query">数据查询</param>
        /// <param name="tab">数据表,更新的数据项是数据表名称</param>
        /// <param name="ps">环境参数</param>
        /// <returns>是否执行成功</returns>
        public static bool Save(QueryDataRes query, DataTable tab, NameObjectList ps)
        {
            if (null == query || null == tab || tab.Rows.Count < 1)
                return true;
            NameObjectList[] psins = ParamManager.createParam(tab, DataRowState.Added);
            NameObjectList[] psupt = ParamManager.createParam(tab, DataRowState.Modified);
            NameObjectList[] psdel = ParamManager.createParam(tab, DataRowState.Deleted);
            foreach (NameObjectList p in psins)
                ParamManager.MergeParam(p, ps, false);
            foreach (NameObjectList p in psupt)
                ParamManager.MergeParam(p, ps, false);
            foreach (NameObjectList p in psdel)
                ParamManager.MergeParam(p, ps, false);
            if (psins.Length < 1 && psupt.Length < 1 && psdel.Length < 1)
                return true;
            bool isSuccess = query.ExecuteNonQuery(tab.TableName, psins, psupt, psdel);
            if (isSuccess)
                tab.AcceptChanges();
            return isSuccess;
        }
        
        #region 处理图片
        /// <summary>
        /// 存入图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="image">图片对象</param>
        public static bool SaveImage(Guid id, Image image)
        {
            if (null == id || null == image)
                return true;
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            QueryDataRes query = new QueryDataRes("基础类");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id.ToString();
            ps["图片"] = stream.ToArray();
            return query.ExecuteUpdate("CRUD_图片", ps);
        }
        /// <summary>
        /// 存入图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="streamImage">图片字节流</param>
        public static bool SaveImage(Guid id, MemoryStream streamImage)
        {
            if (null == id || null == streamImage)
                return true;
            QueryDataRes query = new QueryDataRes("基础类");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id.ToString();
            ps["图片"] = streamImage.ToArray();
            return query.ExecuteUpdate("CRUD_图片", ps);
        }
        /// <summary>
        /// 存入图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <param name="image">图片字节</param>
        public static bool SaveImage(Guid id, byte[] image)
        {
            if (null == id || null == image)
                return true;
            QueryDataRes query = new QueryDataRes("基础类");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id.ToString();
            ps["图片"] = image;
            return query.ExecuteUpdate("CRUD_图片", ps);
        }
        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <returns>返回图片字节</returns>
        public static byte[] getImage(Guid id)
        {
            if (null == id)
                return new byte[0];
            QueryDataRes query = new QueryDataRes("基础类");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id.ToString();
            byte[] img = query.ExecuteScalar("CRUD_图片", ps) as byte[];
            return null == img ? (new byte[0]) : img;
        }
        /// <summary>
        /// 读取图片
        /// </summary>
        /// <param name="id">图片ID</param>
        /// <returns>返回图片字节</returns>
        public static byte[] getImage(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new byte[0];
            QueryDataRes query = new QueryDataRes("基础类");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id;
            byte[] img = query.ExecuteScalar("CRUD_图片", ps) as byte[];
            return null == img ? (new byte[0]) : img;
        }
        #endregion

        #endregion

        #region 数据绑定

        /// <summary>
        /// 绑定表格数据
        /// </summary>
        /// <param name="dbgrid">显示数据的表格</param>
        /// <param name="tab">数据表</param>
        public void BindGrid(DataGridView dbgrid, DataTable tab)
        {
            if (null == dbgrid || null == tab)
                return;
            dbgrid.DataSource = tab;
            dbgrid.CellEndEdit += new DataGridViewCellEventHandler(dbgrid_CellEndEdit);
            dbgrid.DataError += new DataGridViewDataErrorEventHandler(dbgrid_DataError);
            dbgrid.RowPostPaint += new DataGridViewRowPostPaintEventHandler(dbgrid_RowPostPaint);
            DataColumnCollection dbcols = tab.Columns;
            if (dbgrid.Columns.Contains("ID"))
                dbgrid.Columns["ID"].Visible = false;
            foreach (DataColumn dbcol in dbcols)
            {
                //设置表格字典
                string src = Convert.ToString(dbcol.ExtendedProperties[BindManager.srcpro]);
                string txt = Convert.ToString(dbcol.ExtendedProperties[BindManager.txtpro]);
                string val = Convert.ToString(dbcol.ExtendedProperties[BindManager.valpro]);
                txt = string.IsNullOrEmpty(txt) ? val : txt;
                val = string.IsNullOrEmpty(val) ? txt : val;
                if (BindManager.regexSQL.IsMatch(src))
                    src = dbcol.ColumnName;
                if (null != tab.DataSet && !string.IsNullOrEmpty(src) && !string.IsNullOrEmpty(txt)&&!string.IsNullOrEmpty(val)
                    && tab.DataSet.Tables.Contains(src))
                {
                    DataTable tabdict = tab.DataSet.Tables[src];
                    if (tabdict.Columns.Contains(txt) && tabdict.Columns.Contains(val))
                    {
                        DataGridViewComboBoxColumn gdcolCbb = new DataGridViewComboBoxColumn();
                        gdcolCbb.DataPropertyName = dbcol.ColumnName;
                        gdcolCbb.Name = dbcol.ColumnName;
                        gdcolCbb.DataSource = tabdict;
                        gdcolCbb.DisplayMember = txt;
                        gdcolCbb.ValueMember = val;
                        gdcolCbb.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing;
                        int index = dbgrid.Columns[dbcol.ColumnName].Index;
                        dbgrid.Columns.RemoveAt(index);
                        dbgrid.Columns.Insert(index, gdcolCbb);
                    }
                }
                //设置日期
                if (typeof(DateTime) == dbcol.DataType)
                {
                    DataGridViewCalendarColumn gdcoldt = new DataGridViewCalendarColumn();
                    gdcoldt.DataPropertyName = dbcol.ColumnName;
                    gdcoldt.Name = dbcol.ColumnName;
                    int index = dbgrid.Columns[dbcol.ColumnName].Index;
                    dbgrid.Columns.RemoveAt(index);
                    dbgrid.Columns.Insert(index, gdcoldt);
                }
                //设置列是否可见
                if (null != dbcol.ExtendedProperties[BindManager.visipro])
                {
                    bool visible = Convert.ToBoolean(dbcol.ExtendedProperties[BindManager.visipro]);
                    if (!visible) dbgrid.Columns[dbcol.ColumnName].Visible = visible;
                }
                //设置列是否只读
                if (null != dbcol.ExtendedProperties[BindManager.visipro])
                {
                    bool readble = Convert.ToBoolean(dbcol.ExtendedProperties[BindManager.readpro]);
                    if (readble) dbgrid.Columns[dbcol.ColumnName].ReadOnly = readble;
                }
                //列标题
                if (!string.IsNullOrEmpty(dbcol.Caption))
                    dbgrid.Columns[dbcol.ColumnName].HeaderText = dbcol.Caption;
            }
        }

        /// <summary>
        /// 绑定表格数据
        /// </summary>
        /// <param name="dbgrid">显示数据的表格</param>
        /// <param name="tab">数据表</param>
        public void BindGrid(GridControl dbgrid, DataTable tab)
        {
            if (null == dbgrid || null == tab)
                return;
            GridView gridview = (GridView)dbgrid.MainView;
            dbgrid.DataSource = tab;
            gridview.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(gridview_CellValueChanged);
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
                string src = Convert.ToString(dbcol.ExtendedProperties[BindManager.srcpro]);
                string txt = Convert.ToString(dbcol.ExtendedProperties[BindManager.txtpro]);
                string val = Convert.ToString(dbcol.ExtendedProperties[BindManager.valpro]);
                txt = string.IsNullOrEmpty(txt) ? val : txt;
                val = string.IsNullOrEmpty(val) ? txt : val;
                if (BindManager.regexSQL.IsMatch(src))
                    src = dbcol.ColumnName;
                if (null != tab.DataSet && !string.IsNullOrEmpty(src) && !string.IsNullOrEmpty(txt) && !string.IsNullOrEmpty(val)
                    && tab.DataSet.Tables.Contains(src))
                {
                    DataTable tabdict = tab.DataSet.Tables[src];
                    if (tabdict.Columns.Contains(txt) && tabdict.Columns.Contains(val))
                    {
                        RepositoryItemGridLookUpEdit ri = new RepositoryItemGridLookUpEdit();
                        ri.DataSource = tabdict;
                        //控制下拉的选项哪些可以显示，哪些不可以显示
                        if (ri.View.Columns.Contains(ri.View.Columns["ID"]))
                        {
                            ri.View.Columns["ID"].Visible = false;
                        }
                        ri.NullText = "请选择";
                        ri.DisplayMember = txt;
                        ri.ValueMember = val;
                        gdcol.ColumnEdit = ri ;
                    }
                }
                //设置列是否可见
                if (null != dbcol.ExtendedProperties[BindManager.visipro])
                {
                    bool visible = Convert.ToBoolean(dbcol.ExtendedProperties[BindManager.visipro]);
                    if (!visible) gdcol.Visible = visible;
                }
                //设置列是否只读
                if (null != dbcol.ExtendedProperties[BindManager.readpro])
                {
                    bool readble = Convert.ToBoolean(dbcol.ExtendedProperties[BindManager.readpro]);
                    if (readble) gdcol.Visible = readble;
                }
            }
        }

        /// <summary>
        /// 行数组 转换成表,
        /// </summary>
        /// <param name="dt">该表需要有结构，否则该方法会出错</param>
        /// <param name="drr"></param>
        /// <returns></returns>
        public static DataTable GetTable(DataTable dt, DataRow[] drr)
        {
            DataTable dtn = new DataTable();
            dtn = dt.Clone();
            try
            {
                foreach (DataRow dr in drr)
                {
                    DataRow drv = dtn.NewRow();
                    drv = dr;
                    dt.ImportRow(drv);
                }
                return dt;
            }
            catch (Exception ex)
            {
                return dt;
            }
        }

        /// <summary>
        /// 判断页面是否包含Tree（dev）树控件 并且返回树控件
        /// </summary>
        /// <param name="frm">页面，或容器</param>
        /// <returns></returns>
        private void GetTree(Control cl)
        {
            TreeList trv = cl as TreeList;
            if (trv != null)
            {
                this.tree = trv;
                return;
            }
            foreach (Control col in cl.Controls)
            {
                GetTree(col);
            }
        }

        void gridview_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            DevExpress.XtraGrid.Views.Grid.GridView gd = sender as DevExpress.XtraGrid.Views.Grid.GridView;
            if (null == gd || null == e.Value)
                return;
            string fld = e.Column.FieldName;
            foreach (Binding b in this.win.BindingContext[gd.DataSource].Bindings)
                if (fld == b.BindingMemberInfo.BindingField)
                    b.ReadValue();
        }

        /// <summary>
        /// 设置表格行标号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbgrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            DataGridView grid = sender as DataGridView;
            using (SolidBrush b = new SolidBrush(grid.RowHeadersDefaultCellStyle.ForeColor))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(CultureInfo.CurrentCulture),
                        grid.DefaultCellStyle.Font, b, e.RowBounds.Location.X + 20, e.RowBounds.Location.Y + 4);
            }
        }
        /// <summary>
        /// 表格字典列或格式列显示时出错忽略
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbgrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (0 != Convert.ToInt16(e.Context & (DataGridViewDataErrorContexts.Display | DataGridViewDataErrorContexts.Formatting)))
                e.Cancel = true;
        }

        /// <summary>
        /// 编辑表格事件,数据绑定同步
        /// </summary>
        /// <param name="sender">表格</param>
        /// <param name="e"></param>
        private void dbgrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView gd = sender as DataGridView;
            if (null == gd || null == gd.CurrentCell)
                return;
            string fld = gd.CurrentCell.OwningColumn.Name;
            foreach (Binding b in this.win.BindingContext[gd.DataSource].Bindings)
                if (fld == b.BindingMemberInfo.BindingField)
                    b.ReadValue();
        }

        /// <summary>
        /// 下拉字典控件的绑定
        /// </summary>
        /// <param name="tab">控件关联数据源</param>
        /// <param name="ctrl">下拉控件</param>
        /// <param name="fld">控件关联字段</param>
        /// <param name="dictsrc">字典数据源</param>
        /// <param name="dicttxt">字典显示值</param>
        /// <param name="dictval">字典Value值</param>
        private void bindctrl(DataTable tab, ListControl ctrl, string fld, DataTable tabdict, string dicttxt, string dictval)
        {
            if (null == tab || null == ctrl || string.IsNullOrEmpty(fld))
                return;
            if (null == tabdict || (string.IsNullOrEmpty(dictval) && string.IsNullOrEmpty(dicttxt)))
            {
                ctrl.DataBindings.Add("Text", tab, fld);
                return;
            }
            ctrl.DataSource = tabdict;
            ctrl.ValueMember = string.IsNullOrEmpty(dictval) ? dicttxt : dictval;
            ctrl.DisplayMember = string.IsNullOrEmpty(dicttxt) ? dictval : dicttxt;
            ctrl.DataBindings.Add("SelectedValue", tab, fld);
        }
        /// <summary>
        /// 下拉字典控件的绑定
        /// </summary>
        /// <param name="tab">控件关联数据源</param>
        /// <param name="ctrl">下拉控件</param>
        /// <param name="fld">控件关联字段</param>
        /// <param name="dictsrc">字典数据源</param>
        /// <param name="dicttxt">字典显示值</param>
        /// <param name="dictval">字典Value值</param>
        private void bindctrl(DataTable tab, LookUpEdit ctrl, string fld, DataTable tabdict, string dicttxt, string dictval)
        {
            if (null == tab || null == ctrl || string.IsNullOrEmpty(fld))
                return;
            if (null == tabdict || (string.IsNullOrEmpty(dictval) && string.IsNullOrEmpty(dicttxt)))
            {
                ctrl.DataBindings.Add("Text", tab, fld);
                return;
            }
            ctrl.Properties.DataSource = tabdict;
            ctrl.Properties.ValueMember = string.IsNullOrEmpty(dictval) ? dicttxt : dictval;
            ctrl.Properties.DisplayMember = string.IsNullOrEmpty(dicttxt) ? dictval : dicttxt;
            ctrl.DataBindings.Add("EditValue", tab, fld);
        }
        /// <summary>
        /// 复选框控件绑定
        /// </summary>
        /// <param name="tab">控件关联数据源</param>
        /// <param name="ctrl">复选框控件</param>
        /// <param name="fld">控件关联字段</param>
        private void bindctrl(DataTable tab, CheckBox ctrl, string fld)
        {
            if (null == tab || null == ctrl || string.IsNullOrEmpty(fld))
                return;
            ctrl.DataBindings.Add("Checked", tab, fld, true, DataSourceUpdateMode.OnPropertyChanged, false, "");
            ctrl.Click += new EventHandler(ct_ValueChanged);
        }

        /// <summary>
        /// 复选框控件绑定
        /// </summary>
        /// <param name="tab">控件关联数据源</param>
        /// <param name="ctrl">复选框控件</param>
        /// <param name="fld">控件关联字段</param>
        private void bindctrl(DataTable tab, CheckEdit ctrl, string fld)
        {
            if (null == tab || null == ctrl || string.IsNullOrEmpty(fld))
                return;
            ctrl.DataBindings.Add("Checked", tab, fld, true, DataSourceUpdateMode.OnPropertyChanged, false, "");
            ctrl.Click += new EventHandler(ct_ValueChanged);
        }
        /// <summary>
        /// 日期控件绑定
        /// </summary>
        /// <param name="tab">控件关联数据源</param>
        /// <param name="ctrl">复选框控件</param>
        /// <param name="fld">控件关联字段</param>
        private void bindctrl(DataTable tab, DateEdit ctrl, string fld)
        {
            if (null == tab || null == ctrl || string.IsNullOrEmpty(fld))
                return;
            ctrl.DataBindings.Add("Editvalue", tab, fld, true, DataSourceUpdateMode.OnPropertyChanged, false, "");
            ctrl.Click += new EventHandler(ct_ValueChanged);
        }

        /// <summary>
        /// 绑定字段,按照字段控件的tag属性内db和fld值来绑定Text值
        /// </summary>
        /// <param name="ct">字段所在的容器控件或字段控件本身</param>
        /// <param name="ds">数据集</param>
        /// <param name="db">关联数据表的名称</param>
        public void BindFld(Control ct, DataSet ds, string db)
        {
            if (null == ct || null == ds || ds.Tables.Count < 1)
                return;
            if (null != ct.Tag && !string.IsNullOrEmpty(ct.Tag.ToString()))
            {
                string tag = Convert.ToString(ct.Tag);
                string db2 = basefun.valtag(tag, BindManager.dbpro);
                if (!string.IsNullOrEmpty(db2))
                    db = db2;
                string fld = basefun.valtag(tag, BindManager.fldpro);
                DataTable tab = ds.Tables[db];
                if (null == tab && !string.IsNullOrEmpty(db))
                    foreach (DataTable t in ds.Tables)
                    {
                        string itemname = Convert.ToString(t.ExtendedProperties[BindManager.itempro]);
                        if (db == itemname)
                        {
                            tab = t;
                            break;
                        }
                    }
                //绑定控件文本字段
                if (!string.IsNullOrEmpty(db) && !string.IsNullOrEmpty(fld) && null != tab)
                {
                    if (!tab.Columns.Contains(fld))
                        return;
                    ct.DataBindings.Clear();
                    ct.Validated += new EventHandler(ct_ValueChanged);
                    ct.Validating += new System.ComponentModel.CancelEventHandler(ct_Validating);
                    //绑定下拉框字典数据集
                    DataColumn col = tab.Columns[fld];
                    string src = Convert.ToString(col.ExtendedProperties[BindManager.srcpro]);
                    string txt = Convert.ToString(col.ExtendedProperties[BindManager.txtpro]);
                    string val = Convert.ToString(col.ExtendedProperties[BindManager.valpro]);
                    DataTable tabsrc = null;
                    //如果字典是SQL语句
                    if (BindManager.regexSQL.IsMatch(src))
                        src = fld;
                    if (!string.IsNullOrEmpty(src) && ds.Tables.Contains(src))
                        tabsrc = ds.Tables[src];
                    if (ct is ListControl)
                        this.bindctrl(tab, (ct as ListControl), fld, tabsrc, txt, val);
                    else if (ct is LookUpEdit)
                        this.bindctrl(tab, (ct as LookUpEdit), fld, tabsrc, txt, val);
                    else if (ct is CheckBox)
                        this.bindctrl(tab, (ct as CheckBox), fld);
                    else if (ct is CheckEdit)
                        this.bindctrl(tab, (ct as CheckEdit), fld);
                    else if(ct is DateEdit)
                        this.bindctrl(tab, (ct as DateEdit ), fld);
                    else
                    {
                        ct.DataBindings.Add("Text", tab, fld);
                        TextBox tb = ct as TextBox;
                        if (null != tb && tb.MaxLength > 1000)
                            tb.MaxLength = 20;
                    }
                    return;
                }
                //是表格则绑定表格
                if (ct is DataGridView)
                {
                    this.BindGrid(ct as DataGridView, tab);
                    return;
                }
                else if (ct is GridControl)
                {
                    this.BindGrid(ct as GridControl, tab);
                    return;
                }
            }
            if (ct is DataGridView || ct is GridControl)
                return;
            foreach (Control ctrlChild in ct.Controls)
                this.BindFld(ctrlChild, ds, db);
        }
        
        /// <summary>
        /// 校验数据行字段,校验表达式在DataTable的列扩展属性
        /// 校验：是否为空,长度,类型,正则表达式或计算表达式
        /// </summary>
        /// <param name="dr">数据行</param>
        /// <param name="fld">校验字段</param>
        /// <param name="value">检验字符数据</param>
        /// <returns>校验非法则返回提示字符串,否则返回空</returns>
        private string validateRow(DataRow dr, string fld,string value)
        {
            if (null == dr || string.IsNullOrEmpty(fld) || !dr.Table.Columns.Contains(fld))
                return "";
            DataColumn col = dr.Table.Columns[fld];
            if (false.Equals(col.ExtendedProperties[BindManager.isnullpro]))
                if (string.IsNullOrEmpty(value))
                    return "必填内容！";
            string validexp = Convert.ToString(col.ExtendedProperties[BindManager.validpro]);
            if (string.IsNullOrEmpty(validexp))
                return "";
            
            //正则表达式  格式：/express/ msg
            int iend = validexp.LastIndexOf(" ");
            string msg = iend < 1 ? "" : validexp.Substring(iend + 1);
            if (string.IsNullOrEmpty(msg))
                msg = "输入数据不合法！";
            if (iend > 0)
                validexp = validexp.Substring(0, iend).Trim();

            if (string.IsNullOrEmpty(validexp))
                return "";
            //正则验证
            if (validexp.StartsWith("/") && validexp.EndsWith("/"))
            {
                validexp = "^" + validexp.Substring(1, validexp.Length - 2) + "$";
                Regex regex = new Regex(validexp,RegexOptions.Singleline);
                if (!regex.IsMatch(value))
                    return msg;
                return "";
            }

            //计算表达式验证
            DataTable tabCompute = null;
            if (!this.dsCompte.Tables.Contains(dr.Table.TableName))
            {
                tabCompute = dr.Table.Clone();
                tabCompute.Columns.Add("Compute#", typeof(bool));
                this.dsCompte.Tables.Add(tabCompute);
            }
            tabCompute = this.dsCompte.Tables[dr.Table.TableName];
            tabCompute.Clear();
            try
            {
                tabCompute.Columns["Compute#"].Expression = validexp;
                DataColumnCollection cols = dr.Table.Columns;
                object[] data = new object[cols.Count];
                for (int i = 0; i < cols.Count; i++)
                    data[i] = dr[cols[i]];
                dr = tabCompute.Rows.Add(data);
                dr[fld] = value;
                if (false.Equals(dr["Compute#"]))
                    return msg;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            return "";
        }

        /// <summary>
        /// 校验数据有效性,只对编辑框和日期框验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ct_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Control ctrl = sender as Control;
            if (null == ctrl || !(ctrl is TextBox || ctrl is DateTimePicker||ctrl is DevExpress.XtraEditors.TextEdit))
                return;
            string value = ctrl.Text;
            string msgall = "";
            //在计算数据表中使用Value字段代替当前字符串值
            foreach (Binding b in ctrl.DataBindings)
            {
                string fld = b.BindingMemberInfo.BindingField;
                DataTable tab = b.DataSource as DataTable;
                if (null == tab || !tab.Columns.Contains(fld))
                    continue;
                if (b.BindingManagerBase.Position < 0) continue;
                DataRowView drvcur = b.BindingManagerBase.Current as DataRowView;
                if (null == drvcur || DataRowState.Deleted == drvcur.Row.RowState)
                    continue;
                string msg = this.validateRow(drvcur.Row, fld, value);
                if (string.IsNullOrEmpty(msg))
                    continue;
                msgall += string.IsNullOrEmpty(msgall) ? msg : "\r\n" + msg;
                e.Cancel = true;
            }
            if (string.IsNullOrEmpty(msgall))
            {
                this.tip.SetToolTip(ctrl, string.Empty);
                this.tip.Hide(this.win);
            }
            else
            {
                this.tip.SetToolTip(ctrl, msgall);
                this.tip.Show(msgall, this.win, ctrl.Location.X + ctrl.Height, ctrl.Location.Y + ctrl.Width, 5000);
            }
        }
        /// <summary>
        /// 字段编辑文本改变时触发
        /// </summary>
        /// <param name="sender">文本字段控件</param>
        /// <param name="e"></param>
        private void ct_ValueChanged(object sender, EventArgs e)
        {
            Control ctrl = sender as Control;
            if (null == ctrl)       return;
            foreach (Binding b in ctrl.DataBindings)
            {
                BindingManagerBase mgr = b.BindingManagerBase;
                DataRowView drv = (mgr.Position < 0) ? null : mgr.Current as DataRowView;
                if (null != drv && DataRowState.Unchanged == drv.Row.RowState)
                {
                    string fld = b.BindingMemberInfo.BindingField;
                    if (!drv.Row[fld].Equals(drv.Row[fld, DataRowVersion.Original]))
                    {
                        b.ControlUpdateMode = ControlUpdateMode.Never;
                        b.DataSourceUpdateMode = DataSourceUpdateMode.Never;
                        object val = drv[fld];
                        drv.Row.SetModified();
                        drv[fld] = val;
                        b.ControlUpdateMode = ControlUpdateMode.OnPropertyChanged;
                        b.DataSourceUpdateMode = DataSourceUpdateMode.OnValidation;
                    }
                }
            }
            this.win.Refresh();
        }

        /// <summary>
        /// 绑定字段,按照字段控件的tag属性内db和fld值来绑定Text值
        /// </summary>
        /// <param name="ct">字段所在的容器控件或字段控件本身</param>
        /// <param name="ds">数据集</param>
        public void BindFld(Control ct, DataSet ds)
        {
            this.BindFld(ct, ds, "");
        }

        /// <summary>
        /// 绑定字段,按照字段控件的tag属性内db和fld值来绑定Text值
        /// </summary>
        /// <param name="ct">字段所在的容器控件或字段控件本身</param>
        /// <param name="ds">数据集</param>
        public void BinddevTestFld(Control ct, DataSet ds)
        {
            this.BindFld(ct, ds, "");
        }

        /// <summary>
        /// 绑定树,根据指定数据及显示文本和上下级键递归建立树节点
        /// </summary>
        /// <param name="trv">树控件</param>
        /// <param name="tab">数据表</param>
        /// <param name="txtfld">文本字段</param>
        /// <param name="idfld">键字段</param>
        /// <param name="pidfld">上级字段</param>
        /// <param name="tagformat">节点tag值格式</param>
        public void BindTrv(TreeView trv, DataTable tab, string txtfld, string idfld, string pidfld, string tagformat)
        {
            if (null == trv || null == tab || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld))
                return;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld))
                return;
            trv.Nodes.Clear();
            foreach (DataRow dr in tab.Rows)
                BindManager.SetTreeNode(trv, dr, txtfld, idfld, pidfld, tagformat);
        }

        /// <summary>
        /// 绑定树,根据指定数据及显示文本和上下级键递归建立树节点
        /// </summary>
        /// <param name="trv">树控件</param>
        /// <param name="tab">数据表</param>
        /// <param name="txtfld">文本字段</param>
        ///  <param name="cardNo">卡号</param>
        /// <param name="idfld">键字段</param>
        /// <param name="pidfld">上级字段</param>
        /// <param name="tagformat">节点tag值格式</param>
        public void BindTrv(TreeList trv, DataTable tab, string txtfld, string idfld, string pidfld, string tagformat)
        {
            if (null == trv || null == tab || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld))
                return;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld))
                return;
            string caption = txtfld;
            txtfld = txtfld.ToUpper();
            idfld = idfld.ToUpper();
            pidfld = pidfld.ToUpper();
            for (int i = 0; i < trv.Columns.Count; i++)
            {
                TreeListColumn col = trv.Columns[i];
                if (!col.Visible) continue;
                caption = col.Caption;
                break;
            }
            trv.Columns.Clear();
            trv.Columns.AddField(txtfld).Caption = caption;
            trv.Columns.AddField(idfld).Visible = false;
            trv.KeyFieldName = idfld;
            TreeListColumn coltxt = trv.Columns.ColumnByFieldName(txtfld);
            coltxt.OptionsColumn.AllowEdit = false;
            coltxt.Visible = true;
            trv.Nodes.Clear();
            foreach (DataRow dr in tab.Rows)
                BindManager.SetTreeNode(trv, dr, txtfld, idfld, pidfld, tagformat);
        }

        /// <summary>
        /// 绑定树,根据指定数据及显示文本和上下级键递归建立树节点
        /// </summary>
        /// <param name="trv">树控件</param>
        /// <param name="tab">数据表</param>
        /// <param name="txtfld">文本字段</param>
        ///  <param name="cardNo">卡号</param>
        /// <param name="idfld">键字段</param>
        /// <param name="pidfld">上级字段</param>
        /// <param name="tagformat">节点tag值格式</param>
        public void BindTrv(TreeList trv, DataTable tab, string txtfld,string cardNo, string idfld, string pidfld, string tagformat)
        {
            if (null == trv || null == tab || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld))
                return;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld))
                return;
            trv.Nodes.Clear();
            foreach (DataRow dr in tab.Rows)
                BindManager.SetTreeNode(trv, dr, txtfld,cardNo, idfld, pidfld, tagformat);
        }

        /// <summary>
        /// 根据数据记录设置树节点,有则直接返回无则添加节点
        /// </summary>
        /// <param name="trv">树控件</param>
        /// <param name="dr">数据记录</param>
        /// <param name="txtfld">文本字段</param>
        /// <param name="idfld">键值字段</param>
        /// <param name="pidfld">父键字段</param>
        /// <param name="tagformat">附加tag标签属性值格式：对其中{字段名称}替换具体值,没有该字段则不替换</param>
        /// <returns>返回数据记录对应的树节点</returns>
        public static TreeNode SetTreeNode(TreeView trv, DataRow dr, string txtfld, string idfld, string pidfld, string tagformat)
        {
            //检查传入参数
            if (null == trv || null == dr || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld))
                return null;
            DataTable tab = dr.Table;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld))
                return null;
            if (!string.IsNullOrEmpty(pidfld) && !tab.Columns.Contains(pidfld))
                return null;
            if (DBNull.Value == dr[idfld]) 
                return null;

            //有则直接返回
            string key = Convert.ToString(dr[idfld]);
            TreeNode[] trns = trv.Nodes.Find(key, true);
            if (trns.Length > 0)
                return trns[0];

            //没有节点则增加,递归处理父节点
            DataRow pdr = null;
            TreeNode trn = null, ptrn = null;
            if (!string.IsNullOrEmpty(pidfld) && DBNull.Value != dr[pidfld])
            {
                object pobj = dr[pidfld];
                for (int i = 0, len = tab.Rows.Count; i < len; i++)
                {
                    if (!pobj.Equals(tab.Rows[i][idfld]))
                        continue;
                    pdr = tab.Rows[i];
                    break;
                }
            }
            if (null != pdr)
                ptrn = BindManager.SetTreeNode(trv, pdr, txtfld, idfld, pidfld, tagformat);
            trn = new TreeNode(Convert.ToString(dr[txtfld]));
            trn.Name = key;
            //附加tag标签值
            DataColumnCollection cols = tab.Columns;
            foreach (DataColumn col in cols)
            {
                string fld = col.ColumnName;
                string val = "";
                if (DBNull.Value != dr[fld])
                    val = Convert.ToString(dr[fld]);
                tagformat = tagformat.Replace("{" + fld + "}", val);
            }
            if (!string.IsNullOrEmpty(tagformat))
                trn.Tag = tagformat;
            if (null == ptrn)
                trv.Nodes.Add(trn);
            else
                ptrn.Nodes.Add(trn);

            return trn;
        }

        /// <summary>
        /// 根据数据记录设置树节点,有则直接返回无则添加节点
        /// </summary>
        /// <param name="trv">树控件</param>
        /// <param name="dr">数据记录</param>
        /// <param name="txtfld">文本字段</param>
        /// <param name="idfld">键值字段</param>
        /// <param name="pidfld">父键字段</param>
        /// <returns>返回数据记录对应的树节点</returns>
        public static TreeNode SetTreeNode(TreeView trv, DataRow dr, string txtfld, string idfld, string pidfld)
        {
            return BindManager.SetTreeNode(trv, dr, txtfld, idfld, pidfld, "");
        }

        /// <summary>
        /// 根据数据值设置树节点,有则直接返回无则添加节点,上级节点无则是根节点
        /// </summary>
        /// <param name="trv">建立的树</param>
        /// <param name="txt">显示文本值</param>
        /// <param name="id">键值</param>
        /// <param name="pid">上级节点</param>
        /// <param name="tag">tag属性值</param>
        /// <returns>返回对应节点</returns>
        public static TreeNode SetTreeNode(TreeView trv, string txt, string id, string pid, string tag)
        {
            if (null == trv || string.IsNullOrEmpty(txt) || string.IsNullOrEmpty(id))
                return null;
            TreeNode[] trns = trv.Nodes.Find(id, true);
            if (trns.Length > 0)
                return trns[0];
            TreeNode trn = null;

            trns = trv.Nodes.Find(pid, true);
            TreeNode ptrn = trns.Length > 0 ? trns[0] : null;
            trn = new TreeNode(txt);
            trn.Name = id;
            trn.Tag = tag;
            if (null == ptrn)
                trv.Nodes.Add(trn);
            else
                ptrn.Nodes.Add(trn);
            return trn;
        }

        /// <summary>
        /// 根据数据值设置树节点,有则直接返回无则添加节点,上级节点无则是根节点
        /// </summary>
        /// <param name="trv">建立的树</param>
        /// <param name="txt">显示文本值</param>
        /// <param name="id">键值</param>
        /// <param name="pid">上级节点</param>
        /// <returns>返回对应节点</returns>
        public static TreeNode SetTreeNode(TreeView trv, string txt, string id, string pid)
        {
            return BindManager.SetTreeNode(trv, txt, id, pid, "");
        }

        /// <summary>
        /// 根据数据记录设置树节点,有则直接返回无则添加节点
        /// </summary>
        /// <param name="trv">树控件</param>
        /// <param name="dr">数据记录</param>
        /// <param name="txtfld">文本字段</param>
        /// <param name="cardNo">卡号</param>
        /// <param name="idfld">键值字段</param>
        /// <param name="pidfld">父键字段</param>
        /// <param name="tagformat">附加tag标签属性值格式：对其中{字段名称}替换具体值,没有该字段则不替换</param>
        /// <returns>返回数据记录对应的树节点</returns>
        public static TreeListNode SetTreeNode(TreeList trv, DataRow dr, string txtfld, string idfld, string pidfld, string tagformat)
        {
            //检查传入参数
            if (null == trv || null == dr || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld))
                return null;
            DataTable tab = dr.Table;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld))
                return null;
            if (!string.IsNullOrEmpty(pidfld) && !tab.Columns.Contains(pidfld))
                return null;
            if (DBNull.Value == dr[idfld])
                return null;
            //没有节点则增加,递归处理父节点
            DataRow pdr = null;
            TreeListNode trn = null, ptrn = null;
            //有则直接返回
            trn = trv.FindNodeByKeyID(Convert.ToString(dr[idfld]));
            if (null != trn)
                return trn;
            if (!string.IsNullOrEmpty(pidfld) && DBNull.Value != dr[pidfld])
            {
                object pobj = dr[pidfld];
                for (int i = 0, len = tab.Rows.Count; i < len; i++)
                {
                    if (!pobj.Equals(tab.Rows[i][idfld]))
                        continue;
                    pdr = tab.Rows[i];
                    break;
                }
            }
            if (null != pdr)
                ptrn = BindManager.SetTreeNode(trv, pdr, txtfld, idfld, pidfld, tagformat);
            //附加tag标签值
            if (!string.IsNullOrEmpty(tagformat))
            {
                DataColumnCollection cols = tab.Columns;
                foreach (DataColumn col in cols)
                {
                    string fld = col.ColumnName;
                    string val = "";
                    if (DBNull.Value != dr[fld])
                        val = Convert.ToString(dr[fld]);
                    tagformat = tagformat.Replace("{" + fld + "}", val);
                }
            }
            //添加节点
            if (null == ptrn)
            {
                trn = trv.AppendNode(new object[] { Convert.ToString(dr[txtfld]), Convert.ToString(dr[idfld]) }, null, tagformat);
                trn.ImageIndex = 1;
                trn.SelectImageIndex = 1;
            }
            else
            {
                trn = trv.AppendNode(new object[] { Convert.ToString(dr[txtfld]), Convert.ToString(dr[idfld]) }, ptrn, tagformat);
                trn.ImageIndex = 2;
                trn.SelectImageIndex = 2;
            }
            return trn;
        }

        /// <summary>
        /// 根据数据记录设置树节点,有则直接返回无则添加节点
        /// </summary>
        /// <param name="trv">树控件</param>
        /// <param name="dr">数据记录</param>
        /// <param name="txtfld">文本字段</param>
        /// <param name="cardNo">卡号</param>
        /// <param name="idfld">键值字段</param>
        /// <param name="pidfld">父键字段</param>
        /// <param name="tagformat">附加tag标签属性值格式：对其中{字段名称}替换具体值,没有该字段则不替换</param>
        /// <returns>返回数据记录对应的树节点</returns>
        public static TreeListNode SetTreeNode(TreeList trv, DataRow dr, string txtfld,string cardNo, string idfld, string pidfld, string tagformat)
        {
            //检查传入参数
            if (null == trv || null == dr || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld) || string.IsNullOrEmpty(cardNo))
                return null;
            DataTable tab = dr.Table;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld) || !tab.Columns.Contains(cardNo))
                return null;
            if (!string.IsNullOrEmpty(pidfld) && !tab.Columns.Contains(pidfld))
                return null;
            if (DBNull.Value == dr[idfld])
                return null;

            //有则直接返回
            string key = Convert.ToString(dr[idfld]);
            //   TreeNode[] trns = trv.Nodes.Find(key, true);

            foreach (TreeListNode nd in trv.Nodes)
            {
                TreeListNode trns = trv.FindNodeByFieldValue("id", dr["id"].ToString());
                if (trns != null)
                {
                    return trns;
                }
            }

            //没有节点则增加,递归处理父节点
            DataRow pdr = null;
            TreeListNode trn = null, ptrn = null;
            if (!string.IsNullOrEmpty(pidfld) && DBNull.Value != dr[pidfld])
            {
                object pobj = dr[pidfld];
                for (int i = 0, len = tab.Rows.Count; i < len; i++)
                {
                    if (!pobj.Equals(tab.Rows[i][idfld]))
                        continue;
                    pdr = tab.Rows[i];
                    break;
                }
            }
            if (null != pdr)
                ptrn = BindManager.SetTreeNode(trv, pdr, txtfld, cardNo,idfld, pidfld, tagformat);
            if (null == ptrn)
            {
                if (dr[cardNo].ToString() != string.Empty)
                {
                    trn = trv.AppendNode(new object[] { Convert.ToString(dr[txtfld]) + "(" + Convert.ToString(dr[cardNo]) + ")", Convert.ToString(dr[idfld]) }, null, key);
                }
                else
                {
                    trn = trv.AppendNode(new object[] { Convert.ToString(dr[txtfld]), Convert.ToString(dr[idfld]) }, null, key);
                }
                trn.ImageIndex = 1;
                trn.SelectImageIndex =1;
            }
            else
            {
                if (dr[cardNo].ToString() != string.Empty)
                {

                    trn = trv.AppendNode(new object[] { Convert.ToString(dr[txtfld]) + "(" + Convert.ToString(dr[cardNo]) + ")", Convert.ToString(dr[idfld]) }, ptrn, key);
                }
                else
                {
                    trn = trv.AppendNode(new object[] { Convert.ToString(dr[txtfld]), Convert.ToString(dr[idfld]) }, ptrn, key);
                }
                trn.ImageIndex = 2;
                trn.SelectImageIndex = 2;
            }

            //附加tag标签值
            if (string.IsNullOrEmpty(tagformat))
                return trn;
            DataColumnCollection cols = tab.Columns;
            foreach (DataColumn col in cols)
            {
                string fld = col.ColumnName;
                string val = "";
                if (DBNull.Value != dr[fld])
                    val = Convert.ToString(dr[fld]);
                tagformat = tagformat.Replace("{" + fld + "}", val);
            }
            trn.Tag = tagformat;
            return trn;
        }

        #endregion

        #region 系统环境(当前用户和参数)

        /// <summary>
        /// 设置全局当前用户
        /// </summary>
        /// <param name="userid">用户ID</param>
        /// <returns>用户实例</returns>
        public static User setUser(string userid)
        {
            return BindManager.user = new User(userid);
        }
        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns></returns>
        public static User getUser()
        {
            return BindManager.user;
        }
        /// <summary>
        /// 获取当前编号
        /// </summary>
        /// <param name="prefix">前缀</param>
        /// <returns>返回前缀加序号的编号</returns>
        public static string getCodeSn(string prefix)
        {
            NameObjectList ps = new NameObjectList();
            ps["前缀"] = prefix;
            QueryDataRes query = new QueryDataRes("基础类");
            object value = query.ExecuteScalar("获取默认编码", ps);
            return Convert.ToString(value);
        }

        /// <summary>
        /// 获取系统参数,每获取一次都创建一个新的参数列表
        /// </summary>
        /// <returns>系统参数列表</returns>
        public static NameObjectList getSystemParam()
        {
            NameObjectList ps = new NameObjectList();
            
            IPHostEntry IpEntry = Dns.GetHostEntry(Dns.GetHostName());
            ps.Add("StartDate", DateTime.Today.AddMonths(-1));
            ps.Add("EndDate", DateTime.Today);
            ps.Add("LocalIP", IpEntry.AddressList.Length < 1 ? "127.0.0.1" : IpEntry.AddressList[0].ToString());
            ps.Add("Localhost", Dns.GetHostName());
            
            User user = BindManager.user;
            if (null == user) return ps;

            ps.Add("UserAccounts", user.UserAccounts);
            ps.Add("UserName", user.UserName);
            ps.Add("OPTUnitID", user.OPTUnitID);
            ps.Add("UnitCode", user.UnitCode);
            ps.Add("DWName", user.UnitName);
            ps.Add("DWSupName", user.UnitSup);
            ps.Add("DeptSaleName", user.DeptSaleName);
            ps.Add("DeptSupName", user.DeptSup);
            ps.Add("DeptName", user.DeptmentName);
            ps.Add("DeptCode", user.DeptmentCode);
            ps.Add("LimitDays", user.LimitDays);
            return ps;
        }

        /// <summary>
        /// 设置传递参数,传递参数在获取后置空,没有设置传递参数则创建新的实例
        /// </summary>
        /// <param name="ps">传递参数,为空则创建新参数实例</param>
        /// <returns>返回传递参数的引用</returns>
        public static NameObjectList setTransParam(NameObjectList ps)
        {
            return BindManager.transParam = null == ps ? new NameObjectList() : ps;
        }

        /// <summary>
        /// 读取传递参数,然后置空
        /// </summary>
        /// <returns>返回传递参数,如果在读取前没有设置则返回空</returns>
        public static NameObjectList getTransParam()
        {
            NameObjectList ps = BindManager.transParam;
            BindManager.transParam = null;
            return null != ps ? ps : (new NameObjectList());
        }

        #endregion


    }
}
