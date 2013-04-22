using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Data;

namespace Granity.granityMgr.UserManager
{
    public class ExcelHander
    {
        /// <summary>
        /// 导入Excel到Datagrid
        /// </summary>
        /// <param name="excel_filepath"></param>
        /// <param name="ExcelName"></param>
        public void GetSheelName(String excel_filepath, DevExpress.XtraEditors.LookUpEdit ExcelName)
        {
            ExcelName.Properties.Columns.Clear();

            string strConn = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + excel_filepath + "';Extended Properties='Excel 8.0;HDR=YES;IMEX=1'");//第一行作为列
            OleDbConnection conn = new OleDbConnection(strConn);
            try
            {
                conn.Open();
                DataTable dtSheetName = conn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                string[] str = new string[dtSheetName.Rows.Count];
                DataTable dtExcels = new DataTable();
            
                dtExcels.Columns.Add("id");
                dtExcels.Columns.Add("excel");
                for (int k = 0; k < dtSheetName.Rows.Count; k++)
                {
                    DataRow dr = dtExcels.NewRow();
                    dr["id"] = k;
                    dr["excel"] = dtSheetName.Rows[k][2].ToString();
                    dtExcels.Rows.Add(dr);
                }
                ExcelName.Properties.DataSource = dtExcels;//指定表
                ExcelName.Properties.DisplayMember = "excel";//对应windowsr的Text
                ExcelName.Properties.ValueMember = "id";//对应windows的value
                ExcelName.Properties.Columns.AddRange(new
                             DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                         new DevExpress.XtraEditors.Controls.LookUpColumnInfo("excel", " Excel", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});
                conn.Close();
            }
            catch (Exception error)
            {
                // MessageBox.Show(error.Message, "错误提示！");
            }
            finally
            {
                conn.Close();
            }

        }
        /// <summary>
        /// 导入到dataset
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="sheetname"></param>
        /// <returns></returns>
        public DataSet Getexcelds(string Path, String sheetname)
        {
            string strConn = string.Format("Provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + Path + "';Extended Properties='Excel 8.0;HDR=YES;IMEX=1'");//第一行作为列
            OleDbConnection conn = new OleDbConnection(strConn);
            conn.Open();
            try
            {
                string strExcel = "select * from [" + sheetname + "]";
                if (sheetname.IndexOf('$') < 0)//if it have not $
                {
                    strExcel = "select * from[" + sheetname + "$]";
                }
                OleDbDataAdapter da = new OleDbDataAdapter(strExcel, strConn);
                DataSet ds = new DataSet();
                da.Fill(ds, "公司员工");
                conn.Close();
                return ds;

            }
            catch (Exception error)
            {
                return null;
            }
            finally
            {
                conn.Close();
            }

        }
    }
}
