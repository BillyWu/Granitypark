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
namespace Granity.granityMgr.Eatery
{
    /// <summary>
    /// 现金收入汇总
    /// </summary>
    public partial class FrmCashSum : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "现金收入汇总";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        public FrmCashSum()
        {
            InitializeComponent();
        }

        private void FrmCashSum_Load(object sender, EventArgs e)
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
        /// 根据条件查询现金收入报表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            pstrans["RestaurantNo"] = this.lookDept.EditValue != null ? this.lookDept.EditValue.ToString() : null;
            pstrans["EmployNO"] = (object)this.txtEmployNo.Text;
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            BindManager bg = new BindManager(this);
            ds = bg.BuildDataset(this.unitItem, pstrans);
            Report.viwReports View = new Granity.granityMgr.Report.viwReports();
            View.ReportName = "现金收入汇总";
            View.ds = ds;
            View.StartTime = this.dateStart.Text.Trim();
            View.EndTime = this.dateEnd.Text.Trim();
            View.Activate();
            View.Dock = DockStyle.Fill;
            this.groReport.Controls.Clear();
            this.groReport.Controls.Add(View);
        }

        /// <summary>
        /// 初始化部门，分类下拉框
        /// </summary>
        private void InitLookUp()
        {
            DataRow drDept = this.ds.Tables["所有餐厅"].NewRow();
            drDept["名称"] = "全部";
            drDept["编号"] = string.Empty;
            this.ds.Tables["所有餐厅"].Rows.Add(drDept);
            this.lookDept.Properties.DataSource = this.ds.Tables["所有餐厅"];
            this.lookDept.Properties.DisplayMember = "名称";
            this.lookDept.Properties.ValueMember = "编号";
            this.lookDept.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                         new DevExpress.XtraEditors.Controls.LookUpColumnInfo("编号", "编号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near),
              new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}