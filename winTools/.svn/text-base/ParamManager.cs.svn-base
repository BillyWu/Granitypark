#region 版本说明

/*
 * 功能内容：   参数管理工具
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
using Estar.Common.Tools;
using System.Text.RegularExpressions;
using System.Data;

namespace Granity.winTools
{
    /// <summary>
    /// 参数管理工具类
    /// </summary>
    public static class ParamManager
    {

        private static Regex regex = new Regex(@"@([\u4E00-\u9FA5\s\w\{\}]+)=", RegexOptions.Compiled);
        /// <summary>
        /// 根据tag标记创建参数
        /// </summary>
        /// <param name="tag">包含参数的tag标记</param>
        /// <returns>返回新建立的参数</returns>
        public static NameObjectList createParam(string tag)
        {
            MatchCollection matchs = regex.Matches(tag);
            NameObjectList ps = new NameObjectList();
            foreach (Match m in matchs)
            {
                string key = m.Groups[1].Value;
                string k = key;
                if (key.StartsWith("{") && key.EndsWith("}"))
                    k = key.Substring(1, key.Length - 2);
                ps[k] = basefun.valtag(tag, key);
            }
            return ps;
        }

        /// <summary>
        /// 根据数据记录DataRow创建参数
        /// </summary>
        /// <param name="dr">数据记录</param>
        /// <returns>返回新建立的参数</returns>
        public static NameObjectList createParam(DataRow dr)
        {
            NameObjectList ps = new NameObjectList();
            if (null == dr) return ps;
            DataColumnCollection dbcols = dr.Table.Columns;
            foreach (DataColumn dbcol in dbcols)
                ps[dbcol.ColumnName] = dr[dbcol];
            return ps;
        }

        /// <summary>
        /// 根据数据表创建参数列表
        /// </summary>
        /// <param name="tab">数据表</param>
        /// <returns>返回参数列表</returns>
        public static NameObjectList[] createParam(DataTable tab, DataRowState state)
        {
            if (null == tab || tab.Rows.Count < 1)
                return new NameObjectList[0];
            List<NameObjectList> psList = new List<NameObjectList>();
            DataView dvsub = new DataView(tab, "", "", DataViewRowState.ModifiedCurrent | DataViewRowState.Added | DataViewRowState.Deleted);
            DataColumnCollection cols = tab.Columns;
            int len = cols.Count;
            for (int i = 0; i < dvsub.Count; i++)
            {
                if (state != dvsub[i].Row.RowState)
                    continue;
                DataRowView drv = dvsub[i];
                NameObjectList ps = new NameObjectList();
                for (int c = 0; c < len; c++)
                    ps[cols[c].ColumnName] = drv[c];
                psList.Add(ps);
            }
            return psList.ToArray();
        }

        /// <summary>
        /// 把次级参数更新到主参数上,默认覆盖模式true
        /// </summary>
        /// <param name="psMaster">主参数</param>
        /// <param name="psSub">次级参数</param>
        /// <param name="isOver">是否覆盖主参数,默认true</param>
        /// <returns>返回合并的主参数</returns>
        public static void MergeParam(NameObjectList psMaster, NameObjectList psSub, bool isOver)
        {
            if (null == psMaster || null == psSub || psSub.Count < 1)
                return;
            foreach (string key in psSub.AllKeys)
            {
                if (!isOver && null != psMaster[key])
                    continue;
                psMaster[key] = psSub[key];
            }
        }

        /// <summary>
        /// 把次级参数更新到主参数上,默认覆盖模式true
        /// </summary>
        /// <param name="psMaster">主参数</param>
        /// <param name="psSub">次级参数</param>
        /// <returns>返回合并的主参数</returns>
        public static void MergeParam(NameObjectList psMaster, NameObjectList psSub)
        {
            MergeParam(psMaster, psSub, true);
        }

        /// <summary>
        /// 设置宏参数的其中一个值,其他值使用默认值
        /// </summary>
        /// <param name="tp">宏参数类别</param>
        /// <param name="value">宏参数值</param>
        /// <returns>返回设定的宏参数数组</returns>
        public static string[] setMacroParam(MacroPmType tp, string value)
        {
            //字符串替换：pagesize/分页尺寸； firstnum-topnum/分页起止行记录； QW/(1>0,1=1)； FW/(2>0,2=2)；initFilter/(2>0,2=2)；
            //              FGroup/(###)； FSumcol/($$$)； FWhere/(***)
            string pagesize = "100", firstnum = "0", QW = "", FW = "", FGroup = "", FSumcol = "", FWhere = "", topnum = "1000",
                initFilter = "";
            string[] psString = new string[] { pagesize, firstnum, QW, FW, FGroup, FSumcol, FWhere, topnum, initFilter };
            return setMacroParam(psString, tp, value);
        }

        private static Regex regexInt = new Regex(@"\d+", RegexOptions.Compiled);
        /// <summary>
        /// 设置宏参数指定值
        /// </summary>
        /// <param name="ps">宏参数数组,参数长度不够9则直接返回忽略设置新值</param>
        /// <param name="tp">宏参数类别</param>
        /// <param name="value">宏参数值</param>
        /// <returns>返回宏参数值</returns>
        public static string[] setMacroParam(string[] ps, MacroPmType tp, string value)
        {
            if (null == ps || ps.Length < 9)
                return ps;
            switch (tp)
            {
                case MacroPmType.pagesize:  ps[0] = regexInt.IsMatch(value) ? value : "1000"; break;
                case MacroPmType.firstnum:  ps[1] = regexInt.IsMatch(value) ? value : "0"; break;
                case MacroPmType.QW: ps[2] = value; break;
                case MacroPmType.FW: ps[3] = value; break;
                case MacroPmType.FGroup: ps[4] = value; break;
                case MacroPmType.FSumcol: ps[5] = value; break;
                case MacroPmType.FWhere: ps[6] = value; break;
                case MacroPmType.topnum: ps[7] = regexInt.IsMatch(value) ? value : "1000"; break;
            }
            return ps;
        }
    }

    /// <summary>
    /// 数据项查询时传递的宏参数类别
    /// </summary>
    public enum MacroPmType
    {
        /// <summary>
        /// (?num0)分页尺寸,默认100
        /// </summary>
        pagesize,
        /// <summary>
        /// (?num1)分页起始行
        /// </summary>
        firstnum, 
        /// <summary>
        /// (1>0,1=1)快捷查询宏参数
        /// </summary>
        QW, 
        /// <summary>
        /// (2>0,2=2)过滤数据或初始化过滤宏参数
        /// </summary>
        FW, 
        /// <summary>
        /// (###)分组宏参数
        /// </summary>
        FGroup, 
        /// <summary>
        /// ($$$)计算合计列
        /// </summary>
        FSumcol, 
        /// <summary>
        /// (***)
        /// </summary>
        FWhere, 
        /// <summary>
        /// (?num2)分页结尾行
        /// </summary>
        topnum
    }
}
