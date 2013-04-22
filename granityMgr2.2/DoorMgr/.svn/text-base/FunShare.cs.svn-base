using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
namespace Granity.granityMgr
{
    class FunShare
    {
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
        /// 在表新增一行，并给指定的一列赋值
        /// </summary>
        /// <param name="dt">表结构字段有ID字段，否则异常</param>
        /// <param name="ColName">指定的列名称</param>
        /// <param name="ColValue">指定列的值</param>
        public static void AddTabRow(DataTable dt, string ColName, string ColValue, string FagColumn)
        {
            string exeption = FagColumn + "='" + ColName + "'";
            if (dt.Select(exeption).Length > 0)
            {
                return;
            }
            DataRow dr = dt.NewRow();
            dr[ColName] = ColValue;
            dt.Rows.Add(dr);
        }

        /// <summary>
        /// 在表新增一行，并给指定的两列赋值
        /// </summary>
        /// <param name="dt">表，要有结构，否则异常</param>
        /// <param name="ColName">指定的列名称</param>
        /// <param name="ColValue">指定列的值</param>
        /// <param name="ColPareName">指定的列名称</param>
        /// <param name="Pid">指定列的值</param>
        public static void AddTabRow(DataTable dt, string ColFirName, string ColFirValue, string ColSecName, string ColSecValue, string FagColumn, string FagColumnValue)
        {
            string exeption = FagColumn + "='" + FagColumnValue + "'";
            if (dt.Select(exeption).Length > 0)
            {
                return;
            }
            DataRow dr = dt.NewRow();
            dr[ColFirName] = ColFirValue;
            dr[ColSecName] = ColSecValue;
            dt.Rows.Add(dr);
        }

        /// <summary>
        /// 在表新增一行，并给行赋值，支持多列值的修改
        /// </summary>
        /// <param name="dt">表</param>
        /// <param name="hst">哈希表</param>
        /// <param name="FagColumn">标志列名</param>
        /// <param name="FagColumnValue">标志列值</param>
        public static void AddTabRow(DataTable dt, Hashtable hst, string FagColumn, string FagColumnValue)
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
                dr[de.Key.ToString()] = de.Value.ToString();
            }
            dr.EndEdit();
            dt.Rows.Add(dr);
        }

        /// <summary>
        /// 检查表状态是否发生变化,该方法只适用采用联动数据绑定的情况
        /// </summary>
        /// <param name="ds">数据集</param>
        /// <returns></returns>
        public static bool CheckDataState(DataSet ds)
        {
            bool fag = false;
            foreach (DataTable dt in ds.Tables)
            {
                DataTable dtFag = dt.GetChanges();
                if (dtFag == null)
                    continue;
                if (dtFag.Rows.Count > 0)
                {
                    fag = true;
                    break;
                }
            }
            return fag;
        }
    }
}
