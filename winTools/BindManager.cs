#region �汾˵��

/*
 * �������ݣ�   win�������ݰ󶨹�����,��ϵͳ�����Ĺ���
 *
 * ��    �ߣ�   ���ٲ�
 *
 * �� �� �ߣ�   ���ٲ�
 *
 * ��    �ڣ�   2010-05-27
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
    /// ��windowsӦ�õ����ݰ󶨹���,�ṩͳһ�Ĵ���ʽ
    /// </summary>
    public class BindManager
    {
        #region �ڲ�������
        /// <summary>
        /// ������Դ(tag�������)
        /// </summary>
        private const string dbpro = "db";
        /// <summary>
        /// ���ֶ�(tag�������)
        /// </summary>
        private const string fldpro = "fld";
        
        /// <summary>
        /// �ֵ�Դ(DataTable����չ����)
        /// </summary>
        private const string srcpro = "src";
        /// <summary>
        /// �ֵ�Դ�ı��ֶ�(DataTable����չ����)
        /// </summary>
        private const string txtpro = "txt";
        /// <summary>
        /// �ֵ�ֵ�ֶ�(DataTable����չ����)
        /// </summary>
        private const string valpro = "val";
        /// <summary>
        /// ������Ԫ��������(DataTable����չ����)
        /// </summary>
        private const string itempro = "itemname";
        /// <summary>
        /// �п�(DataTable����չ����)
        /// </summary>
        private const string widthpro = "width";
        /// <summary>
        /// �Ƿ�ɼ�(DataTable����չ����)
        /// </summary>
        private const string visipro = "visible";
        /// <summary>
        /// �Ƿ�ֻ��(DataTable����չ����)
        /// </summary>
        private const string readpro = "readonly";
        /// <summary>
        /// У����ʽ(DataTable����չ����)
        /// </summary>
        private const string validpro = "validexp";
        /// <summary>
        /// �Ƿ�ɿ�,(DataTable����չ����)
        /// </summary>
        private const string isnullpro = "isNULL";

        /// <summary>
        /// ��֤�Ƿ���SQL���
        /// </summary>
        private static readonly Regex regexSQL = new Regex(@"\b(execute|exec|select|from)\b", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        /// <summary>
        /// ��֤�Ƿ�������
        /// </summary>
        private static readonly Regex regexINT = new Regex(@"^\d*$", RegexOptions.Compiled);

        #endregion

        //ϵͳ��ǰ�û�
        private static User user = null;

        //������ڴ�ʱ�Ĵ��ݲ���,�ٵ��û�ȡ���ÿ�
        //ÿ�����ô��ݲ���ʱ�򴴽��²���
        private static NameObjectList transParam = null;

        //��ǰ����
        private Form win;
       
        private ToolTip tip;
        /// <summary>
        /// ����Ԫ���ݱ�У�鼰����ĵ������ݱ�
        /// ��ÿ�����ݱ��һ��ʹ��ʱ��¡�������ṹ
        /// </summary>
        private DataSet dsCompte = new DataSet();

        /// <summary>
        /// ���캯��,��ʼ���󶨹������ڵĴ���
        /// </summary>
        /// <param name="win"></param>
        public BindManager(Form win)
        {
            this.win = win;
            tip = new ToolTipMsg();
        }

        /// <summary>
        /// ���win ���Ƿ������ؼ��������ؼ���tree ָ����������ΪNull
        /// </summary>
        private TreeList tree =null;
        /// <summary>
        /// ����������д�����ݱ���չ����,���ڰ�ʱ����
        /// </summary>
        /// <param name="tab">���ݱ�</param>
        /// <param name="item">������</param>
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

        #region ���ݴ���

        /// <summary>
        /// ��ʼ��������Ԫ���ݼ�
        /// </summary>
        /// <param name="unitItem">��Ԫʵ��</param>
        /// <param name="ps">���ݵĻ�������</param>
        /// <returns>��Ԫ�����ݼ�</returns>
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
                //���������
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
                    if (null != tab && tab.Rows.Count > 0 && tab.Columns.Contains("��¼����"))
                        psString = ParamManager.setMacroParam(psString, MacroPmType.topnum, Convert.ToString(tab.Rows[0]["��¼����"]));
                    tab = query.getTable(item.DataSrc, ps, psmacro);
                }
                if (null == tab) continue;
                //����ֵ����ݼ�
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
        /// ���ñ�����ֶΣ������ö��ŷָ�
        /// </summary>
        /// <param name="dbgrid">���ؼ�</param>
        /// <param name="cols">��Ҫ��ʾ������,���ŷָ�,Ϊ��Ĭ�ϲ�����</param>
        public void SetGridCols(DataGridView dbgrid, string cols)
        {
            if (null == dbgrid || dbgrid.Columns.Count < 1 || string.IsNullOrEmpty(cols))
                return;
            //�����ֶ��Ƿ���ʾ������
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
            //�����ֶ�˳��
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
        /// ���ñ�����ֶΣ������ö��ŷָ�
        /// </summary>
        /// <param name="dbgrid">���ؼ�</param>
        /// <param name="cols">��Ҫ��ʾ������,���ŷָ�,Ϊ��Ĭ�ϲ�����</param>
        public void SetGridCols(GridControl dbgrid, string cols)
        {
            if (null == dbgrid || string.IsNullOrEmpty(cols))
                return;
            //�����ֶ��Ƿ���ʾ������
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
            //�����ֶ�˳��
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
        /// ʹ�ø��ĵ����ݱ�tabChanged���������ݱ�tabData,״̬���޸ĺ������ӵĲ�����
        /// ���ݱ�Ҫ����ͬ�Ľṹ����ͬ������
        /// </summary>
        /// <param name="tabData">���ݱ�</param>
        /// <param name="tabChanged">�Աȵ����ݱ�</param>
        /// <param name="addfilter">���¼�¼�Ĺ��˱��ʽ</param>
        public static void UpdateTable(DataTable tabData, DataTable tabChanged, string filter)
        {
            if (null == tabData || null == tabChanged || tabChanged.Rows.Count < 1)
                return;
            //��������,���tabchanged�е�tabpkû�еļ�¼�Ҳ���Added����Ը���
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
        /// �Կؼ����е��ֶ�ֵ��ֵ,�ؼ�tag��ǻ�colmapӳ���ֶ�
        /// </summary>
        /// <param name="tab">���ݱ�</param>
        /// <param name="ct">�ؼ�����</param>
        /// <param name="tagName">�ؼ�tag����ֶε�����,���ֵ���ֶλ��ֶεı���,�޶�Ӧ��ʹ��colmap��ӳ��,���������</param>
        /// <param name="colmap">�ֶ�ӳ��(�ɿ�):tagName��colmap[tagname,fld]��tab.fld</param>
        public static void SetControlValue(DataRow dr, Control ct, string tagName, IDictionary<string, string> colmap)
        {
            if (null == dr || null == ct || string.IsNullOrEmpty(tagName))
                return;
            DataTable tab = dr.Table;
            string tag = Convert.ToString(ct.Tag);
            string fld = basefun.valtag(tag, tagName);
            if (!string.IsNullOrEmpty(fld))
            {
                //ӳ���ֶ�: fld-col-title
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
                //�Կؼ���ֵ
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
            //�ݹ鸳ֵ
            foreach (Control c in ct.Controls)
                SetControlValue(dr, c, tagName, colmap);
        }

        /// <summary>
        /// �Լ�¼�е��ֶ�ֵ��ֵ,�ֶ����ƻ�����ӳ����ͬ
        /// </summary>
        /// <param name="drsrc">Դ�м�¼</param>
        /// <param name="drdest">��Ҫ���µ�Ŀ���м�¼</param>
        /// <param name="colmap">��ӳ��</param>
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
        /// ��������
        /// </summary>
        /// <param name="query">���ݲ�ѯ</param>
        /// <param name="ds">���ݼ�,���µ������������ݱ�����</param>
        /// <param name="ps">��������</param>
        /// <returns>�Ƿ�ִ�гɹ�</returns>
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
        /// ��������
        /// </summary>
        /// <param name="query">���ݲ�ѯ</param>
        /// <param name="tab">���ݱ�,���µ������������ݱ�����</param>
        /// <param name="ps">��������</param>
        /// <returns>�Ƿ�ִ�гɹ�</returns>
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
        
        #region ����ͼƬ
        /// <summary>
        /// ����ͼƬ
        /// </summary>
        /// <param name="id">ͼƬID</param>
        /// <param name="image">ͼƬ����</param>
        public static bool SaveImage(Guid id, Image image)
        {
            if (null == id || null == image)
                return true;
            MemoryStream stream = new MemoryStream();
            image.Save(stream, ImageFormat.Jpeg);
            QueryDataRes query = new QueryDataRes("������");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id.ToString();
            ps["ͼƬ"] = stream.ToArray();
            return query.ExecuteUpdate("CRUD_ͼƬ", ps);
        }
        /// <summary>
        /// ����ͼƬ
        /// </summary>
        /// <param name="id">ͼƬID</param>
        /// <param name="streamImage">ͼƬ�ֽ���</param>
        public static bool SaveImage(Guid id, MemoryStream streamImage)
        {
            if (null == id || null == streamImage)
                return true;
            QueryDataRes query = new QueryDataRes("������");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id.ToString();
            ps["ͼƬ"] = streamImage.ToArray();
            return query.ExecuteUpdate("CRUD_ͼƬ", ps);
        }
        /// <summary>
        /// ����ͼƬ
        /// </summary>
        /// <param name="id">ͼƬID</param>
        /// <param name="image">ͼƬ�ֽ�</param>
        public static bool SaveImage(Guid id, byte[] image)
        {
            if (null == id || null == image)
                return true;
            QueryDataRes query = new QueryDataRes("������");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id.ToString();
            ps["ͼƬ"] = image;
            return query.ExecuteUpdate("CRUD_ͼƬ", ps);
        }
        /// <summary>
        /// ��ȡͼƬ
        /// </summary>
        /// <param name="id">ͼƬID</param>
        /// <returns>����ͼƬ�ֽ�</returns>
        public static byte[] getImage(Guid id)
        {
            if (null == id)
                return new byte[0];
            QueryDataRes query = new QueryDataRes("������");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id.ToString();
            byte[] img = query.ExecuteScalar("CRUD_ͼƬ", ps) as byte[];
            return null == img ? (new byte[0]) : img;
        }
        /// <summary>
        /// ��ȡͼƬ
        /// </summary>
        /// <param name="id">ͼƬID</param>
        /// <returns>����ͼƬ�ֽ�</returns>
        public static byte[] getImage(string id)
        {
            if (string.IsNullOrEmpty(id))
                return new byte[0];
            QueryDataRes query = new QueryDataRes("������");
            NameObjectList ps = new NameObjectList();
            ps["ID"] = id;
            byte[] img = query.ExecuteScalar("CRUD_ͼƬ", ps) as byte[];
            return null == img ? (new byte[0]) : img;
        }
        #endregion

        #endregion

        #region ���ݰ�

        /// <summary>
        /// �󶨱������
        /// </summary>
        /// <param name="dbgrid">��ʾ���ݵı��</param>
        /// <param name="tab">���ݱ�</param>
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
                //���ñ���ֵ�
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
                //��������
                if (typeof(DateTime) == dbcol.DataType)
                {
                    DataGridViewCalendarColumn gdcoldt = new DataGridViewCalendarColumn();
                    gdcoldt.DataPropertyName = dbcol.ColumnName;
                    gdcoldt.Name = dbcol.ColumnName;
                    int index = dbgrid.Columns[dbcol.ColumnName].Index;
                    dbgrid.Columns.RemoveAt(index);
                    dbgrid.Columns.Insert(index, gdcoldt);
                }
                //�������Ƿ�ɼ�
                if (null != dbcol.ExtendedProperties[BindManager.visipro])
                {
                    bool visible = Convert.ToBoolean(dbcol.ExtendedProperties[BindManager.visipro]);
                    if (!visible) dbgrid.Columns[dbcol.ColumnName].Visible = visible;
                }
                //�������Ƿ�ֻ��
                if (null != dbcol.ExtendedProperties[BindManager.visipro])
                {
                    bool readble = Convert.ToBoolean(dbcol.ExtendedProperties[BindManager.readpro]);
                    if (readble) dbgrid.Columns[dbcol.ColumnName].ReadOnly = readble;
                }
                //�б���
                if (!string.IsNullOrEmpty(dbcol.Caption))
                    dbgrid.Columns[dbcol.ColumnName].HeaderText = dbcol.Caption;
            }
        }

        /// <summary>
        /// �󶨱������
        /// </summary>
        /// <param name="dbgrid">��ʾ���ݵı��</param>
        /// <param name="tab">���ݱ�</param>
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
                //���ñ���ֵ�
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
                        //����������ѡ����Щ������ʾ����Щ��������ʾ
                        if (ri.View.Columns.Contains(ri.View.Columns["ID"]))
                        {
                            ri.View.Columns["ID"].Visible = false;
                        }
                        ri.NullText = "��ѡ��";
                        ri.DisplayMember = txt;
                        ri.ValueMember = val;
                        gdcol.ColumnEdit = ri ;
                    }
                }
                //�������Ƿ�ɼ�
                if (null != dbcol.ExtendedProperties[BindManager.visipro])
                {
                    bool visible = Convert.ToBoolean(dbcol.ExtendedProperties[BindManager.visipro]);
                    if (!visible) gdcol.Visible = visible;
                }
                //�������Ƿ�ֻ��
                if (null != dbcol.ExtendedProperties[BindManager.readpro])
                {
                    bool readble = Convert.ToBoolean(dbcol.ExtendedProperties[BindManager.readpro]);
                    if (readble) gdcol.Visible = readble;
                }
            }
        }

        /// <summary>
        /// ������ ת���ɱ�,
        /// </summary>
        /// <param name="dt">�ñ���Ҫ�нṹ������÷��������</param>
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
        /// �ж�ҳ���Ƿ����Tree��dev�����ؼ� ���ҷ������ؼ�
        /// </summary>
        /// <param name="frm">ҳ�棬������</param>
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
        /// ���ñ���б��
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
        /// ����ֵ��л��ʽ����ʾʱ�������
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void dbgrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            if (0 != Convert.ToInt16(e.Context & (DataGridViewDataErrorContexts.Display | DataGridViewDataErrorContexts.Formatting)))
                e.Cancel = true;
        }

        /// <summary>
        /// �༭����¼�,���ݰ�ͬ��
        /// </summary>
        /// <param name="sender">���</param>
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
        /// �����ֵ�ؼ��İ�
        /// </summary>
        /// <param name="tab">�ؼ���������Դ</param>
        /// <param name="ctrl">�����ؼ�</param>
        /// <param name="fld">�ؼ������ֶ�</param>
        /// <param name="dictsrc">�ֵ�����Դ</param>
        /// <param name="dicttxt">�ֵ���ʾֵ</param>
        /// <param name="dictval">�ֵ�Valueֵ</param>
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
        /// �����ֵ�ؼ��İ�
        /// </summary>
        /// <param name="tab">�ؼ���������Դ</param>
        /// <param name="ctrl">�����ؼ�</param>
        /// <param name="fld">�ؼ������ֶ�</param>
        /// <param name="dictsrc">�ֵ�����Դ</param>
        /// <param name="dicttxt">�ֵ���ʾֵ</param>
        /// <param name="dictval">�ֵ�Valueֵ</param>
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
        /// ��ѡ��ؼ���
        /// </summary>
        /// <param name="tab">�ؼ���������Դ</param>
        /// <param name="ctrl">��ѡ��ؼ�</param>
        /// <param name="fld">�ؼ������ֶ�</param>
        private void bindctrl(DataTable tab, CheckBox ctrl, string fld)
        {
            if (null == tab || null == ctrl || string.IsNullOrEmpty(fld))
                return;
            ctrl.DataBindings.Add("Checked", tab, fld, true, DataSourceUpdateMode.OnPropertyChanged, false, "");
            ctrl.Click += new EventHandler(ct_ValueChanged);
        }

        /// <summary>
        /// ��ѡ��ؼ���
        /// </summary>
        /// <param name="tab">�ؼ���������Դ</param>
        /// <param name="ctrl">��ѡ��ؼ�</param>
        /// <param name="fld">�ؼ������ֶ�</param>
        private void bindctrl(DataTable tab, CheckEdit ctrl, string fld)
        {
            if (null == tab || null == ctrl || string.IsNullOrEmpty(fld))
                return;
            ctrl.DataBindings.Add("Checked", tab, fld, true, DataSourceUpdateMode.OnPropertyChanged, false, "");
            ctrl.Click += new EventHandler(ct_ValueChanged);
        }
        /// <summary>
        /// ���ڿؼ���
        /// </summary>
        /// <param name="tab">�ؼ���������Դ</param>
        /// <param name="ctrl">��ѡ��ؼ�</param>
        /// <param name="fld">�ؼ������ֶ�</param>
        private void bindctrl(DataTable tab, DateEdit ctrl, string fld)
        {
            if (null == tab || null == ctrl || string.IsNullOrEmpty(fld))
                return;
            ctrl.DataBindings.Add("Editvalue", tab, fld, true, DataSourceUpdateMode.OnPropertyChanged, false, "");
            ctrl.Click += new EventHandler(ct_ValueChanged);
        }

        /// <summary>
        /// ���ֶ�,�����ֶοؼ���tag������db��fldֵ����Textֵ
        /// </summary>
        /// <param name="ct">�ֶ����ڵ������ؼ����ֶοؼ�����</param>
        /// <param name="ds">���ݼ�</param>
        /// <param name="db">�������ݱ������</param>
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
                //�󶨿ؼ��ı��ֶ�
                if (!string.IsNullOrEmpty(db) && !string.IsNullOrEmpty(fld) && null != tab)
                {
                    if (!tab.Columns.Contains(fld))
                        return;
                    ct.DataBindings.Clear();
                    ct.Validated += new EventHandler(ct_ValueChanged);
                    ct.Validating += new System.ComponentModel.CancelEventHandler(ct_Validating);
                    //���������ֵ����ݼ�
                    DataColumn col = tab.Columns[fld];
                    string src = Convert.ToString(col.ExtendedProperties[BindManager.srcpro]);
                    string txt = Convert.ToString(col.ExtendedProperties[BindManager.txtpro]);
                    string val = Convert.ToString(col.ExtendedProperties[BindManager.valpro]);
                    DataTable tabsrc = null;
                    //����ֵ���SQL���
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
                //�Ǳ����󶨱��
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
        /// У���������ֶ�,У����ʽ��DataTable������չ����
        /// У�飺�Ƿ�Ϊ��,����,����,������ʽ�������ʽ
        /// </summary>
        /// <param name="dr">������</param>
        /// <param name="fld">У���ֶ�</param>
        /// <param name="value">�����ַ�����</param>
        /// <returns>У��Ƿ��򷵻���ʾ�ַ���,���򷵻ؿ�</returns>
        private string validateRow(DataRow dr, string fld,string value)
        {
            if (null == dr || string.IsNullOrEmpty(fld) || !dr.Table.Columns.Contains(fld))
                return "";
            DataColumn col = dr.Table.Columns[fld];
            if (false.Equals(col.ExtendedProperties[BindManager.isnullpro]))
                if (string.IsNullOrEmpty(value))
                    return "�������ݣ�";
            string validexp = Convert.ToString(col.ExtendedProperties[BindManager.validpro]);
            if (string.IsNullOrEmpty(validexp))
                return "";
            
            //������ʽ  ��ʽ��/express/ msg
            int iend = validexp.LastIndexOf(" ");
            string msg = iend < 1 ? "" : validexp.Substring(iend + 1);
            if (string.IsNullOrEmpty(msg))
                msg = "�������ݲ��Ϸ���";
            if (iend > 0)
                validexp = validexp.Substring(0, iend).Trim();

            if (string.IsNullOrEmpty(validexp))
                return "";
            //������֤
            if (validexp.StartsWith("/") && validexp.EndsWith("/"))
            {
                validexp = "^" + validexp.Substring(1, validexp.Length - 2) + "$";
                Regex regex = new Regex(validexp,RegexOptions.Singleline);
                if (!regex.IsMatch(value))
                    return msg;
                return "";
            }

            //������ʽ��֤
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
        /// У��������Ч��,ֻ�Ա༭������ڿ���֤
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
            //�ڼ������ݱ���ʹ��Value�ֶδ��浱ǰ�ַ���ֵ
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
        /// �ֶα༭�ı��ı�ʱ����
        /// </summary>
        /// <param name="sender">�ı��ֶοؼ�</param>
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
        /// ���ֶ�,�����ֶοؼ���tag������db��fldֵ����Textֵ
        /// </summary>
        /// <param name="ct">�ֶ����ڵ������ؼ����ֶοؼ�����</param>
        /// <param name="ds">���ݼ�</param>
        public void BindFld(Control ct, DataSet ds)
        {
            this.BindFld(ct, ds, "");
        }

        /// <summary>
        /// ���ֶ�,�����ֶοؼ���tag������db��fldֵ����Textֵ
        /// </summary>
        /// <param name="ct">�ֶ����ڵ������ؼ����ֶοؼ�����</param>
        /// <param name="ds">���ݼ�</param>
        public void BinddevTestFld(Control ct, DataSet ds)
        {
            this.BindFld(ct, ds, "");
        }

        /// <summary>
        /// ����,����ָ�����ݼ���ʾ�ı������¼����ݹ齨�����ڵ�
        /// </summary>
        /// <param name="trv">���ؼ�</param>
        /// <param name="tab">���ݱ�</param>
        /// <param name="txtfld">�ı��ֶ�</param>
        /// <param name="idfld">���ֶ�</param>
        /// <param name="pidfld">�ϼ��ֶ�</param>
        /// <param name="tagformat">�ڵ�tagֵ��ʽ</param>
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
        /// ����,����ָ�����ݼ���ʾ�ı������¼����ݹ齨�����ڵ�
        /// </summary>
        /// <param name="trv">���ؼ�</param>
        /// <param name="tab">���ݱ�</param>
        /// <param name="txtfld">�ı��ֶ�</param>
        ///  <param name="cardNo">����</param>
        /// <param name="idfld">���ֶ�</param>
        /// <param name="pidfld">�ϼ��ֶ�</param>
        /// <param name="tagformat">�ڵ�tagֵ��ʽ</param>
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
        /// ����,����ָ�����ݼ���ʾ�ı������¼����ݹ齨�����ڵ�
        /// </summary>
        /// <param name="trv">���ؼ�</param>
        /// <param name="tab">���ݱ�</param>
        /// <param name="txtfld">�ı��ֶ�</param>
        ///  <param name="cardNo">����</param>
        /// <param name="idfld">���ֶ�</param>
        /// <param name="pidfld">�ϼ��ֶ�</param>
        /// <param name="tagformat">�ڵ�tagֵ��ʽ</param>
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
        /// �������ݼ�¼�������ڵ�,����ֱ�ӷ���������ӽڵ�
        /// </summary>
        /// <param name="trv">���ؼ�</param>
        /// <param name="dr">���ݼ�¼</param>
        /// <param name="txtfld">�ı��ֶ�</param>
        /// <param name="idfld">��ֵ�ֶ�</param>
        /// <param name="pidfld">�����ֶ�</param>
        /// <param name="tagformat">����tag��ǩ����ֵ��ʽ��������{�ֶ�����}�滻����ֵ,û�и��ֶ����滻</param>
        /// <returns>�������ݼ�¼��Ӧ�����ڵ�</returns>
        public static TreeNode SetTreeNode(TreeView trv, DataRow dr, string txtfld, string idfld, string pidfld, string tagformat)
        {
            //��鴫�����
            if (null == trv || null == dr || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld))
                return null;
            DataTable tab = dr.Table;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld))
                return null;
            if (!string.IsNullOrEmpty(pidfld) && !tab.Columns.Contains(pidfld))
                return null;
            if (DBNull.Value == dr[idfld]) 
                return null;

            //����ֱ�ӷ���
            string key = Convert.ToString(dr[idfld]);
            TreeNode[] trns = trv.Nodes.Find(key, true);
            if (trns.Length > 0)
                return trns[0];

            //û�нڵ�������,�ݹ鴦���ڵ�
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
            //����tag��ǩֵ
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
        /// �������ݼ�¼�������ڵ�,����ֱ�ӷ���������ӽڵ�
        /// </summary>
        /// <param name="trv">���ؼ�</param>
        /// <param name="dr">���ݼ�¼</param>
        /// <param name="txtfld">�ı��ֶ�</param>
        /// <param name="idfld">��ֵ�ֶ�</param>
        /// <param name="pidfld">�����ֶ�</param>
        /// <returns>�������ݼ�¼��Ӧ�����ڵ�</returns>
        public static TreeNode SetTreeNode(TreeView trv, DataRow dr, string txtfld, string idfld, string pidfld)
        {
            return BindManager.SetTreeNode(trv, dr, txtfld, idfld, pidfld, "");
        }

        /// <summary>
        /// ��������ֵ�������ڵ�,����ֱ�ӷ���������ӽڵ�,�ϼ��ڵ������Ǹ��ڵ�
        /// </summary>
        /// <param name="trv">��������</param>
        /// <param name="txt">��ʾ�ı�ֵ</param>
        /// <param name="id">��ֵ</param>
        /// <param name="pid">�ϼ��ڵ�</param>
        /// <param name="tag">tag����ֵ</param>
        /// <returns>���ض�Ӧ�ڵ�</returns>
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
        /// ��������ֵ�������ڵ�,����ֱ�ӷ���������ӽڵ�,�ϼ��ڵ������Ǹ��ڵ�
        /// </summary>
        /// <param name="trv">��������</param>
        /// <param name="txt">��ʾ�ı�ֵ</param>
        /// <param name="id">��ֵ</param>
        /// <param name="pid">�ϼ��ڵ�</param>
        /// <returns>���ض�Ӧ�ڵ�</returns>
        public static TreeNode SetTreeNode(TreeView trv, string txt, string id, string pid)
        {
            return BindManager.SetTreeNode(trv, txt, id, pid, "");
        }

        /// <summary>
        /// �������ݼ�¼�������ڵ�,����ֱ�ӷ���������ӽڵ�
        /// </summary>
        /// <param name="trv">���ؼ�</param>
        /// <param name="dr">���ݼ�¼</param>
        /// <param name="txtfld">�ı��ֶ�</param>
        /// <param name="cardNo">����</param>
        /// <param name="idfld">��ֵ�ֶ�</param>
        /// <param name="pidfld">�����ֶ�</param>
        /// <param name="tagformat">����tag��ǩ����ֵ��ʽ��������{�ֶ�����}�滻����ֵ,û�и��ֶ����滻</param>
        /// <returns>�������ݼ�¼��Ӧ�����ڵ�</returns>
        public static TreeListNode SetTreeNode(TreeList trv, DataRow dr, string txtfld, string idfld, string pidfld, string tagformat)
        {
            //��鴫�����
            if (null == trv || null == dr || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld))
                return null;
            DataTable tab = dr.Table;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld))
                return null;
            if (!string.IsNullOrEmpty(pidfld) && !tab.Columns.Contains(pidfld))
                return null;
            if (DBNull.Value == dr[idfld])
                return null;
            //û�нڵ�������,�ݹ鴦���ڵ�
            DataRow pdr = null;
            TreeListNode trn = null, ptrn = null;
            //����ֱ�ӷ���
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
            //����tag��ǩֵ
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
            //��ӽڵ�
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
        /// �������ݼ�¼�������ڵ�,����ֱ�ӷ���������ӽڵ�
        /// </summary>
        /// <param name="trv">���ؼ�</param>
        /// <param name="dr">���ݼ�¼</param>
        /// <param name="txtfld">�ı��ֶ�</param>
        /// <param name="cardNo">����</param>
        /// <param name="idfld">��ֵ�ֶ�</param>
        /// <param name="pidfld">�����ֶ�</param>
        /// <param name="tagformat">����tag��ǩ����ֵ��ʽ��������{�ֶ�����}�滻����ֵ,û�и��ֶ����滻</param>
        /// <returns>�������ݼ�¼��Ӧ�����ڵ�</returns>
        public static TreeListNode SetTreeNode(TreeList trv, DataRow dr, string txtfld,string cardNo, string idfld, string pidfld, string tagformat)
        {
            //��鴫�����
            if (null == trv || null == dr || string.IsNullOrEmpty(txtfld) || string.IsNullOrEmpty(idfld) || string.IsNullOrEmpty(cardNo))
                return null;
            DataTable tab = dr.Table;
            if (!tab.Columns.Contains(txtfld) || !tab.Columns.Contains(idfld) || !tab.Columns.Contains(cardNo))
                return null;
            if (!string.IsNullOrEmpty(pidfld) && !tab.Columns.Contains(pidfld))
                return null;
            if (DBNull.Value == dr[idfld])
                return null;

            //����ֱ�ӷ���
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

            //û�нڵ�������,�ݹ鴦���ڵ�
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

            //����tag��ǩֵ
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

        #region ϵͳ����(��ǰ�û��Ͳ���)

        /// <summary>
        /// ����ȫ�ֵ�ǰ�û�
        /// </summary>
        /// <param name="userid">�û�ID</param>
        /// <returns>�û�ʵ��</returns>
        public static User setUser(string userid)
        {
            return BindManager.user = new User(userid);
        }
        /// <summary>
        /// ��ȡ��ǰ�û�
        /// </summary>
        /// <returns></returns>
        public static User getUser()
        {
            return BindManager.user;
        }
        /// <summary>
        /// ��ȡ��ǰ���
        /// </summary>
        /// <param name="prefix">ǰ׺</param>
        /// <returns>����ǰ׺����ŵı��</returns>
        public static string getCodeSn(string prefix)
        {
            NameObjectList ps = new NameObjectList();
            ps["ǰ׺"] = prefix;
            QueryDataRes query = new QueryDataRes("������");
            object value = query.ExecuteScalar("��ȡĬ�ϱ���", ps);
            return Convert.ToString(value);
        }

        /// <summary>
        /// ��ȡϵͳ����,ÿ��ȡһ�ζ�����һ���µĲ����б�
        /// </summary>
        /// <returns>ϵͳ�����б�</returns>
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
        /// ���ô��ݲ���,���ݲ����ڻ�ȡ���ÿ�,û�����ô��ݲ����򴴽��µ�ʵ��
        /// </summary>
        /// <param name="ps">���ݲ���,Ϊ���򴴽��²���ʵ��</param>
        /// <returns>���ش��ݲ���������</returns>
        public static NameObjectList setTransParam(NameObjectList ps)
        {
            return BindManager.transParam = null == ps ? new NameObjectList() : ps;
        }

        /// <summary>
        /// ��ȡ���ݲ���,Ȼ���ÿ�
        /// </summary>
        /// <returns>���ش��ݲ���,����ڶ�ȡǰû�������򷵻ؿ�</returns>
        public static NameObjectList getTransParam()
        {
            NameObjectList ps = BindManager.transParam;
            BindManager.transParam = null;
            return null != ps ? ps : (new NameObjectList());
        }

        #endregion


    }
}
