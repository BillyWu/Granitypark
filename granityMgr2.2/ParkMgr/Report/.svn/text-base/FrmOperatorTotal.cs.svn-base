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


namespace Granity.granityMgr.ParkMgr.Report
{
    public partial class FrmOperatorTotal : DevExpress.XtraEditors.XtraForm
    {
        /// <summary>
        /// 收费员收费统计表
        /// </summary>
        string unitName = "收费员收费统计表";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        DataSet ds = null;
        public FrmOperatorTotal()
        {
            InitializeComponent();
        }

        private void FrmOperatorTotal_Load(object sender, EventArgs e)
        {
            //初始化参数和单元
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            //绑定数据
            BindManager bg = new BindManager(this);
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            this.dateStart.EditValue = System.DateTime.Now.ToString("yyyy-MM-dd");
            ps["dt"] = this.dateStart.EditValue.ToString();
            ps["type"] = null;
            ps["UserName"] = null;
            ds = bg.BuildDataset(this.unitItem, ps);
            InitLookUp();
            InitDoor();
        }

        /// <summary>
        /// 初始化入场报表类型，分类下拉框
        /// </summary>
        private void InitLookUp()
        {
            this.lookDate.Properties.DataSource = this.ds.Tables["报表类型"];
            this.lookDate.Properties.DisplayMember = "名称";
            this.lookDate.Properties.ValueMember = "编码";
            this.lookDate.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                         new DevExpress.XtraEditors.Controls.LookUpColumnInfo("编码", "编码", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near),
              new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});
        }

        /// <summary>
        /// 初始化门岗
        /// </summary>
        private void InitDoor()
        {
            this.cboDoorName.Properties.DataSource = this.ds.Tables["操作员"];
            this.cboDoorName.Properties.DisplayMember = "帐号";
            this.cboDoorName.Properties.ValueMember = "编号";
            this.cboDoorName.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                         new DevExpress.XtraEditors.Controls.LookUpColumnInfo("编号", "编号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near),
              new DevExpress.XtraEditors.Controls.LookUpColumnInfo("帐号", "帐号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});
        }
        /// <summary>
        /// 年月日统计
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtQuery_Click(object sender, EventArgs e)
        {
            if (this.dateStart.Text == string.Empty)
            {
                XtraMessageBox.Show("请输入开始时间", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (this.cboDoorName.Text == string.Empty)
            {
                XtraMessageBox.Show("请选择操作员", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (this.lookDate.EditValue == null)
            {
                XtraMessageBox.Show("请选择表报", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            NameObjectList pstrans = new NameObjectList();
            ParamManager.MergeParam(pstrans, this.paramwin);
            pstrans["dt"] = this.dateStart.EditValue.ToString();
            pstrans["type"] = this.lookDate.EditValue.ToString();
            pstrans["UserName"] = Convert.ToString(this.cboDoorName.Text);
            //查询数据
            QueryDataRes query = new QueryDataRes(this.unitItem.DataSrcFile);
            BindManager bg = new BindManager(this);
            ds = bg.BuildDataset(this.unitItem, pstrans);
            //if (ds.Tables["收费员收费统计"].Rows.Count < 1 || ds.Tables["收费员收费统计"] == null)
            //{
            //    XtraMessageBox.Show("没有数据！", "系统提示！", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    this.groReport.Controls.Clear();
            //    return;
            //}
            Granity.granityMgr.Report.viwReports View = new Granity.granityMgr.Report.viwReports();
            View.ReportName = "收费员收费统计";
            View.ds = ds;
            View.StartTime = this.dateStart.Text.Trim();
            View.EndTime = "";
            View.Activate();
            View.Dock = DockStyle.Fill;
            this.groReport.Controls.Clear();
            this.groReport.Controls.Add(View);
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnclose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}