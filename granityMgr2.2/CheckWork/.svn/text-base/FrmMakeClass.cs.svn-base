using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
using System.Collections;
namespace Granity.granityMgr.CheckWork
{
    public enum WorkState
    { 
        /// <summary>
        /// 上班
        /// </summary>
       Work,
        /// <summary>
        /// 休息
        /// </summary>
        Relax
    }

    /// <summary>
    /// 部门排班
    /// </summary>
    public partial class FrmMakeClass : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "部门排班";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        QueryDataRes Query = null;
        DataSet ds = null;
        public FrmMakeClass()
        {
            InitializeComponent();
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmMakeClass_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据0
            this.bindMgr = new BindManager(this);
            this.paramwin["code"] = DBNull.Value;
            this.ds = this.bindMgr.BuildDataset(this.unitItem, this.paramwin);
            this.Query = new QueryDataRes(this.unitItem.DataSrcFile);
            this.bindMgr.BindFld(this, ds);
            DataTable tabDept = this.ds.Tables["部门信息"];
            this.bindMgr.BindTrv(this.treDept, tabDept, "名称", "ID", "PID", "@ID={ID},@PID={PID},@代码={代码},@站址={站址}");
           // this.treDept.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(treDept_FocusedNodeChanged);
            this.treDept.ExpandAll();
           
        }

        void treDept_FocusedNodeChanged(object sender, DevExpress.XtraTreeList.FocusedNodeChangedEventArgs e)
        {
            ///部门班制
            string deptNo = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag), "代码");
            DataTable dtDeptClass = this.ds.Tables["部门班制"].Clone();
            dtDeptClass = FunShare.GetTable(dtDeptClass, this.ds.Tables["部门班制"].Select("部门='" + deptNo + "'"));
            this.grdClass.DataSource = dtDeptClass;
            //部门休息日
            DataTable deptHolidaysState = this.ds.Tables["部门休息日"].Clone();
            deptHolidaysState = FunShare.GetTable(deptHolidaysState, this.ds.Tables["部门休息日"].Select("部门='" + deptNo + "'"));
            this.dbRestDept.DataSource = deptHolidaysState;
            //部门排班明细
            if (this.gridViewClass.RowCount == 0)
                return;
            string classNo = this.gridViewClass.GetDataRow(this.gridViewClass.FocusedRowHandle)["班制id"].ToString();
            string tag = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag), "ID");
            DataTable dtClassList = this.ds.Tables["排班明细"].Clone();
            dtClassList = FunShare.GetTable(dtClassList, this.ds.Tables["排班明细"].Select("部门='" + tag + "' and 班制='" + classNo + "'", " 日期 asc"));
            this.grdClassList.DataSource = dtClassList;
        }

        private void btMakeClass_Click(object sender, EventArgs e)
        {
            if (this.treDept.FocusedNode == null)
                return;
            if (this.gridViewClass.RowCount == 0)
                return;
            //状态 休息，上班
            string relaxState = "";
            this.ds.Tables["排班明细"].Clear();
            string startDate = string.Empty;
            string endDate = string.Empty;
            string deptNo = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag), "代码");
            string ClassNo = this.gridViewClass.GetDataRow(this.gridViewClass.FocusedRowHandle)["班制编号"].ToString();
            string tag = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag), "ID");
            if (this.ds.Tables["部门班制"].Select("部门='" + deptNo + "' and 班制编号='" + ClassNo + "'").Length == 0)
                return;
            //选中部门的班制
            DataRow drClass = this.ds.Tables["部门班制"].Select("部门='" + deptNo + "' and 班制编号='" + ClassNo + "'")[0];
            //选中部门的班制明细
            DataTable dtAllClassList = this.ds.Tables["班制明细"].Clone();
            dtAllClassList = FunShare.GetTable(dtAllClassList, this.ds.Tables["班制明细"].Select("编号='" + drClass["班制编号"] + "'", "班次 asc"));
            startDate = drClass["启动日期"].ToString();
            endDate = drClass["结束日期"].ToString();
            TimeSpan days = DateTime.Parse(endDate).AddDays(1).Date - DateTime.Parse(startDate).Date;
            int classDays = int.Parse(drClass["换班周期"].ToString());
            int len = this.ds.Tables["班制明细"].Select("编号='" + drClass["班制编号"] + "'", "班次 asc").Length;
            int totalDays = days.Days;
            //正周期循环次数
            int classCircleTime = 0;
            //周期余数循环次数
            int classListCircleTime = 0;
            classCircleTime = totalDays / classDays;
            classListCircleTime = totalDays % classDays;
            int count = classDays - len;
            int last = 0;
            if (count < 0)
            {
                string deptName = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag), "名称");
                XtraMessageBox.Show(deptName+"设置的换班周期应当大于或等于班制明细，请重新设置！", "系统提示！");
                return;
            }
            //正周期循环次数
            for (int i = 1; i <= classCircleTime; i++)
            {
                int n = -1;
                int m = 0;
                #region 插入班制明细
                if (i == 1)
                {
                    SyncStartDayOfWeek(dtAllClassList, drClass, classDays, i, ref last, deptNo, startDate, relaxState);
                }
                else
                {
                    InsertCalssList(dtAllClassList, drClass, classDays, len, last, m, i, n, startDate, deptNo, tag, relaxState);
                }
                #endregion
            }
            //周期余数循环次数
            int day = -1;
            #region 插入班制明细
            foreach (DataRow drClassList in this.ds.Tables["班制明细"].Select("编号='" + drClass["班制编号"] + "'", "班次 asc"))
            {
                day++;
                if (day == classListCircleTime + last)
                    break;
                //计算日期，日期按天递增
                int k = classCircleTime * classDays-last + day;
                relaxState = dtAllClassList.Rows[day]["休假"].ToString();
                relaxState = DateJudge(relaxState, this.treDept.FocusedNode, DateTime.Parse(startDate).AddDays(k));
                BuidDate(this.treDept.FocusedNode,drClassList, drClass, k, startDate, relaxState);
            }
           
            #endregion
            DataTable dtTemp = this.ds.Tables["排班明细"].Clone();
            this.grdClassList.DataSource = FunShare.GetTable(dtTemp, this.ds.Tables["排班明细"].Select("部门='" + tag + "'"));
        }

        /// <summary>
        /// 一个完整的排班周期
        /// </summary>
        /// <param name="dtAllClassList"></param>
        /// <param name="drClass"></param>
        /// <param name="classDays"></param>
        /// <param name="len"></param>
        /// <param name="last"></param>
        /// <param name="m"></param>
        /// <param name="i"></param>
        /// <param name="n"></param>
        /// <param name="startDate"></param>
        /// <param name="deptNo"></param>
        /// <param name="tag"></param>
        /// <param name="relaxState"></param>
        private void InsertCalssList(DataTable dtAllClassList, DataRow drClass, int classDays, int len, int last, int m, int i,int n, string startDate, string deptNo, string tag, string relaxState)
        {
            int a = -1;
            foreach (DataRow drClassList in dtAllClassList.Rows)
            {
                a++;
                n++;
                //计算日期 日期按天递增
                m = ((i - 1) * classDays - last) + n;
                relaxState = dtAllClassList.Rows[a]["休假"].ToString();
                relaxState = DateJudge(relaxState,this.treDept.FocusedNode, DateTime.Parse(startDate).AddDays(m));
                BuidDate(this.treDept.FocusedNode, drClassList, drClass, m, startDate, relaxState);
            }
        }

        /// <summary>
        /// 把星期几的英文格式转换成汉语
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string  EnglistConvertToChinese(string str)
        {
            string weedDay = string.Empty;
            switch (str)
            {
                case "Monday":
                    weedDay = "星期一";
                    break;
                case "Tuesday":
                    weedDay = "星期二";
                    break;
                case "Wednesday":
                    weedDay = "星期三";
                    break;
                case "Thursday":
                    weedDay = "星期四";
                    break;
                case "Friday":
                    weedDay = "星期五";
                    break;
                case "Saturday":
                    weedDay = "星期六";
                    break;
                case "Sunday":
                    weedDay = "星期日";
                    break;
            }
            return weedDay;
        }

        /// <summary>
        /// 把星期几的汉语格式转换成英文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ChineseConvertToEnglist(string str)
        {
            string weedDay = string.Empty;
            switch (str)
            {
                case "星期一":
                    weedDay = "Monday";
                    break;
                case "星期二":
                    weedDay = "Tuesday";
                    break;
                case "星期三":
                    weedDay = "Wednesday";
                    break;
                case "星期四":
                    weedDay = "Thursday";
                    break;
                case "星期五":
                    weedDay = "Friday";
                    break;
                case "星期六":
                    weedDay = "Saturday";
                    break;
                case "星期日":
                    weedDay = "Sunday";
                    break;
                case "星期天":
                    weedDay = "Sunday";
                    break;
            }
            return weedDay;
        }

        /// <summary>
        /// 协调启动日期星期几跟班制明细星期几相同
        /// </summary>
        /// <param name="dtAllClassList">班制明细表</param>
        /// <param name="drClass">班制</param>
        /// <param name="classDays">周期</param>
        /// <param name="i">循环的次数</param>
        /// <param name="m">计算日期变量</param>
        /// <param name="n">计算日期变量</param>
        /// <param name="deptNo">部门</param>
        /// <param name="startDate">开始日期</param>
        private void SyncStartDayOfWeek(DataTable dtAllClassList, DataRow drClass, int classDays, int i, ref int last, string deptNo, string startDate, string relaxState)
        {
            if (this.treDept.FocusedNode == null)
                return;
            int n = -1;
            int m = 0;
            foreach (DataRow drStartWeek in dtAllClassList.Rows)
            {
                string weedDay = drStartWeek["日期序号"].ToString();
                weedDay = ChineseConvertToEnglist(weedDay);
                if (weedDay != DateTime.Parse(startDate).DayOfWeek.ToString())
                {
                    last++;
                    continue;
                }
                else
                {
                    break;
                }
            }
            for (int a = last ; a < dtAllClassList.Rows.Count; a++)
            {
                n++;
                //计算日期 日期按天递增
                m = (i - 1) * classDays + n;
                relaxState = dtAllClassList.Rows[a]["休假"].ToString();
                relaxState = DateJudge(relaxState, this.treDept.FocusedNode, DateTime.Parse(startDate).AddDays(m));
                BuidDate(this.treDept.FocusedNode, dtAllClassList.Rows[a], drClass, m, startDate, relaxState);
            }
        }

        /// <summary>
        /// 构建排班明细记录行
        /// </summary>
        /// <param name="node">部门节点</param>
        /// <param name="drClassList">班制明细记录</param>
        /// <param name="drClass">班制</param>
        /// <param name="m">天数</param>
        /// <param name="deptNo">部门编号</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="relaxState">状态 (上班，休息)</param>
        private void BuidDate(TreeListNode node, DataRow drClassList, DataRow drClass, int m, string startDate, string relaxState)
        {
            if (node == null)
                return;
            string tag = basefun.valtag(Convert.ToString(node.Tag), "ID");
            DataRow drList = this.ds.Tables["排班明细"].NewRow();
            DataRow drwClass = this.ds.Tables["部门班制"].NewRow();
            drwClass = drClass;
            drList["ID"] = Guid.NewGuid().ToString();
            drList["部门"] = tag;
            drList["部门名称"] = drwClass["部门名称"].ToString();
            drList["班制"] = drwClass["班制id"].ToString();
            drList["启动日期"] = DateTime.Parse(drwClass["启动日期"].ToString());
            drList["班次"] = drClassList["班次"].ToString();
            drList["日期"] = DateTime.Parse(startDate).AddDays(m).ToShortDateString();
            drList["星期"] = EnglistConvertToChinese(DateTime.Parse(startDate).AddDays(m).DayOfWeek.ToString());
            drList["状态"] = relaxState;
            drList["班制编号"] = drwClass["班制编号"].ToString();
            drList["上班1"] = drClassList["上班1"].ToString()!=string.Empty ?  DateTime.Parse(startDate).AddDays(m).ToShortDateString() + " " + DateTime.Parse(drClassList["上班1"].ToString()).ToLongTimeString():DBNull.Value.ToString();
            drList["下班1"] = drClassList["下班1"].ToString()!=string.Empty ? DateTime.Parse(startDate).AddDays(m).ToShortDateString() + " " + DateTime.Parse(drClassList["下班1"].ToString()).ToLongTimeString() : DBNull.Value.ToString();
            drList["上班2"] = drClassList["上班2"].ToString()!=string.Empty ? DateTime.Parse(startDate).AddDays(m).ToShortDateString() + " " + DateTime.Parse(drClassList["上班2"].ToString()).ToLongTimeString() : DBNull.Value.ToString();
            drList["下班2"] = drClassList["下班2"].ToString() != string.Empty ? DateTime.Parse(startDate).AddDays(m).ToShortDateString() + " " + DateTime.Parse(drClassList["下班2"].ToString()).ToLongTimeString() : DBNull.Value.ToString();
            drList["允许迟到"] = drClassList["允许迟到"] != null ? drClassList["允许迟到"].ToString() : DBNull.Value.ToString();
            drList["迟到限制"] = drClassList["迟到限制"] != null ? drClassList["迟到限制"].ToString() : DBNull.Value.ToString();
            drList["允许早退"] = drClassList["允许早退"] != null ? drClassList["允许早退"].ToString() : DBNull.Value.ToString();
            drList["早退限制"] = drClassList["早退限制"] != null ? drClassList["早退限制"].ToString() : DBNull.Value.ToString();
            drList["上班有效"] = drClassList["上班有效"] != null ? drClassList["上班有效"].ToString() : DBNull.Value.ToString();
            drList["下班有效"] = drClassList["下班有效"] != null ? drClassList["下班有效"].ToString() : DBNull.Value.ToString();
            this.ds.Tables["排班明细"].Rows.Add(drList);
        }

        /// <summary>
        ///  判断日期是否属于部门休息日跟法定休息日
        /// </summary>
        /// <param name="date">开始日期</param>
        /// <param name="state">是否允许部门节假日休假</param>
        /// <param name="state">是否允许法定节假日休假</param>
        /// <returns></returns>
        private string DateJudge(string workState, TreeListNode node, DateTime date)
        {
            string fag = workState;
            string deptNo = basefun.valtag(Convert.ToString(node.Tag), "代码");
            bool deptHolidayState = false;
            bool HolidayState = false;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            DataTable deptHolidays = this.ds.Tables["部门休息日"].Clone();
            deptHolidays = FunShare.GetTable(deptHolidays, this.ds.Tables["部门休息日"].Select("部门='" + deptNo + "'"));
            DataTable[] tabList = { this.ds.Tables["法定休息日"], deptHolidays };
            foreach (DataTable tab in tabList)
            {
                int i = 0;
                foreach (DataRow drDept in tab.Rows)
                {
                    if (DateTime.Parse(drDept["开始日期"].ToString()) > date || DateTime.Parse(drDept["结束日期"].ToString()) < date)
                    {
                        bool state = bool.Parse(fag);
                        switch (state)
                        {
                            case true:
                                fag = workState;
                                break;
                            case false:
                                fag = workState;
                                break;
                        }
                    }
                    else
                    {
                        if (tab.TableName == "部门休息日")
                        {
                            deptHolidayState = Boolean.Parse(drDept["休假"].ToString());
                        }
                        else
                        {
                            HolidayState = Boolean.Parse(drDept["休假"].ToString());
                        }
                        startDate = DateTime.Parse(drDept["开始日期"].ToString());
                        endDate = DateTime.Parse(drDept["结束日期"].ToString());
                        for (DateTime sDate = startDate; sDate <= endDate; sDate = sDate.AddDays(1))
                        {
                            if (sDate.ToShortDateString() == date.ToShortDateString())
                            {
                                if (tab.TableName == "部门休息日" && deptHolidayState == true)
                                {
                                    fag = "休息";
                                    return fag;
                                }
                                else if (tab.TableName == "部门休息日" && deptHolidayState == false)
                                {
                                    fag = "上班";
                                    return fag;
                                }
                                else if (tab.TableName == "法定休息日" && HolidayState == true)
                                {
                                    fag = "休息";
                                    return fag;
                                }
                                else if (tab.TableName == "法定休息日" && HolidayState == false)
                                {
                                    fag = "上班";
                                    return fag;
                                }
                            }
                        }
                    }
                }
            }
            if (fag.ToLower() == "true")
                fag = "休息";
            else
                fag = "上班";
            return fag;
        }

        /// <summary>
        /// 填充行记录时判断调班或节假日是否要上班
        /// </summary>
        /// <param name="node"></param>
        /// <param name="date"></param>
        /// <param name="deptHolidayState"></param>
        /// <param name="HolidayState"></param>
        /// <returns></returns>
        private string DateJudge(DataTable tab, TreeListNode node, DateTime date)
        {
            string fag = "休息";
            if (node == null)
                return fag;
            bool deptHolidayState = false;
            DateTime startDate = new DateTime();
            DateTime endDate = new DateTime();
            foreach (DataRow drDept in tab.Rows)
            {
                deptHolidayState = Boolean.Parse(drDept["休假"].ToString());
                if (deptHolidayState)
                    return fag;
                if (DateTime.Parse(drDept["开始日期"].ToString()) > date || DateTime.Parse(drDept["结束日期"].ToString()) < date)
                    continue;
                else
                {
                    startDate = DateTime.Parse(drDept["开始日期"].ToString());
                    endDate = DateTime.Parse(drDept["结束日期"].ToString());
                    for (DateTime sDate = startDate; sDate <= endDate; sDate = sDate.AddDays(1))
                    {
                        if (sDate == date)
                        {
                            fag = "上班";
                            return fag;
                        }
                    }
                }
            }
            return fag;
        }

        private void BtSave_Click(object sender, EventArgs e)
        {
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            bool isSuccess = BindManager.Save(query, this.ds, this.paramwin);
            if (!isSuccess)
            {
                XtraMessageBox.Show("保存失败，请检查数据是否合法！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                this.ds.Tables["排班明细"].AcceptChanges();
                XtraMessageBox.Show("保存成功！", "保存提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btResetMakeClass_Click(object sender, EventArgs e)
        {
            if(this.treDept.FocusedNode ==null)
                return;
            if (this.gridViewClass.RowCount == 0)
                return;
            string tag = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag), "ID");
            string ClassNo = this.gridViewClass.GetDataRow(this.gridViewClass.FocusedRowHandle)["班制编号"].ToString();
            foreach (DataRow dr in this.ds.Tables["排班明细"].Select("部门='" + tag + "' and 班制编号='" + ClassNo + "'"))
            {
                dr.Delete();
            }
            this.grdClassList.DataSource = this.ds.Tables["排班明细"].Select("部门='" + tag + "' and 班制编号='" + ClassNo + "'", " 日期 asc", DataViewRowState.ModifiedCurrent);
        }

        /// <summary>
        /// 打印排班，也可以用Excel导出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPrintClassList_Click(object sender, EventArgs e)
        {
            this.grdClassList.ShowPrintPreview();
        }

        private void gridViewClass_RowClick(object sender, DevExpress.XtraGrid.Views.Grid.RowClickEventArgs e)
        {
            if (this.treDept.FocusedNode == null)
                return;
            if (this.gridViewClass.RowCount == 0)
                return;
            string tag = basefun.valtag(Convert.ToString(this.treDept.FocusedNode.Tag), "ID");
            string ClassNo = this.gridViewClass.GetDataRow(this.gridViewClass.FocusedRowHandle)["班制编号"].ToString();
            DataTable dtClassList = this.ds.Tables["排班明细"].Clone();
            dtClassList = FunShare.GetTable(dtClassList, this.ds.Tables["排班明细"].Select("部门='" + tag + "' and 班制编号='" + ClassNo + "'", " 日期 asc"));
            this.grdClassList.DataSource = dtClassList;
        }
    }
}