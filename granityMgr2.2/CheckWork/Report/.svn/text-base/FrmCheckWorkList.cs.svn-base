using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using Estar.Business.DataManager;
using Estar.Common.Tools;
using Granity.winTools;
namespace Granity.granityMgr.CheckWork.Report
{
    public partial class FrmCheckWorkList : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "考勤明细查询";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        public FrmCheckWorkList()
        {
            InitializeComponent();
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtQuery_Click(object sender, EventArgs e)
        {
            if (this.dateStart.Text == string.Empty)
            {
                XtraMessageBox.Show("请输入开始时间", "系统提示！");
                return;
            }
            if (this.dateEnd.Text == string.Empty)
            {
                XtraMessageBox.Show("请输入结束时间", "系统提示！");
                return;
            }

            NameObjectList pstrans = new NameObjectList();
            ParamManager.MergeParam(pstrans, this.paramwin);
            pstrans["StartDate"] = this.dateStart.EditValue.ToString();
            pstrans["EndDate"] = this.dateEnd.EditValue.ToString();
            pstrans["Dept"] = this.lookDept.EditValue;
            pstrans["EmployNO"] = (object)this.txtEmployName.Text;
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            BindManager bg = new BindManager(this);
            ds = bg.BuildDataset(this.unitItem, pstrans);
            Granity.granityMgr.Report.viwReports  View = new Granity.granityMgr.Report.viwReports();
            View.ReportName = "考勤明细查询";
            View.ds = ds;
            View.StartTime = this.dateStart.Text.Trim();
            View.EndTime = this.dateEnd.Text.Trim();
            View.Activate();
            View.Dock = DockStyle.Fill;
            this.groReport.Controls.Clear();
            this.groReport.Controls.Add(View);
        }

        private void FrmCheckWorkList_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            this.dateStart.EditValue = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            this.dateEnd.EditValue = System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm");
            ps["StartDate"] = this.dateStart.EditValue.ToString();
            ps["EndDate"] = this.dateEnd.EditValue.ToString();
            ds = bg.BuildDataset(this.unitItem, ps);
            InitLookUp();
        }

        /// <summary>
        /// 初始化部门，分类下拉框
        /// </summary>
        private void InitLookUp()
        {
            DataRow drDept = this.ds.Tables["部门"].NewRow();
            drDept["名称"] = "全部";
            drDept["代码"] = string.Empty;
            this.ds.Tables["部门"].Rows.Add(drDept);
            this.lookDept.Properties.DataSource = this.ds.Tables["部门"];
            this.lookDept.Properties.DisplayMember = "名称";
            this.lookDept.Properties.ValueMember = "代码";
            this.lookDept.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                         new DevExpress.XtraEditors.Controls.LookUpColumnInfo("代码", "代码", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near),
              new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});
        }
    }
}