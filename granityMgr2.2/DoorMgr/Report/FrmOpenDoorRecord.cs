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
namespace Granity.granityMgr.Report
{
    /// <summary>
    /// 刷卡开门记录报表
    /// </summary>
    public partial class FrmOpenDoorRecord : DevExpress.XtraEditors.XtraForm
    {
        string unitName = "刷卡记录";//单元
        UnitItem unitItem = null;
        NameObjectList paramwin = null;
        BindManager bindMgr;
        DataSet ds = null;
        public FrmOpenDoorRecord()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 根据开始时间，结束时间查询开门记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtQuery_Click(object sender, EventArgs e)
        {
            if( this.dateStart.Text ==string.Empty )
            {
                XtraMessageBox.Show("请输入开始时间","系统提示！");
                return;
            }
            if (this.dateEnd.Text == string.Empty)
            {
                XtraMessageBox.Show("请输入结束时间", "系统提示！");
                return;
            }
          
            string  dataStart= this.dateStart.Text.Trim();
            string dataEnd = this.dateEnd.Text.Trim();
            string where = " 时间 >=  '" + dataStart + "' and 时间 <= '" + dataEnd + "' ";
            if (this.lookDoor.EditValue != null && this.lookDoor.EditValue.ToString() != "全部")
            {
                where += " and 门编号='" + this.lookDoor.EditValue.ToString() + "'";
            }
            this.paramwin = BindManager.getSystemParam();
            NameObjectList ps = new NameObjectList();
            ps["StartDate"] = this.dateStart.EditValue.ToString();
            ps["EndDate"] = this.dateEnd.EditValue.ToString();
            ParamManager.MergeParam(ps, this.paramwin, false);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            this.ds = this.bindMgr.BuildDataset(this.unitItem,ps);
            DataTable dt = this.ds.Tables["刷卡记录"].Clone();
            DataTable dtReport = FunShare.GetTable(dt, this.ds.Tables["刷卡记录"].Select(where));
            Report.viwReports View = new viwReports();
            View.ReportName = "刷卡报表";
            View.dt = dt;
            View.StartTime = this.dateStart.Text.Trim();
            View.EndTime = this.dateEnd.Text.Trim();
            View.Activate();
            View.Dock = DockStyle.Fill;
            this.groReport.Controls.Clear();
            this.groReport.Controls.Add(View);
        }

        private void Frm_OpenDoorRecord_Load(object sender, EventArgs e)
        {
            this.paramwin = BindManager.getSystemParam();
            NameObjectList pstrans = BindManager.getTransParam();
            ParamManager.MergeParam(this.paramwin, pstrans);
            this.unitItem = new UnitItem(DataAccRes.AppSettings("WorkConfig"), unitName);
            this.bindMgr = new BindManager(this);
            NameObjectList ps = new NameObjectList();
            ParamManager.MergeParam(ps, this.paramwin);
            this.dateStart.EditValue = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            this.dateEnd.EditValue = System.DateTime.Now.AddDays(1).ToString("yyyy-MM-dd HH:mm");
            ps["StartDate"] = this.dateStart.EditValue.ToString();
            ps["EndDate"] = this.dateEnd.EditValue.ToString();
            this.ds = this.bindMgr.BuildDataset(this.unitItem, ps);
            GetDoorInfo();
        }

        private void GetDoorInfo()
        {
            DataRow dr = this.ds.Tables["门"].NewRow();
            dr["名称"] = "全部";
            dr["门编号"] = "全部";
            this.ds.Tables["门"].Rows.Add(dr);
            this.lookDoor.Properties.DataSource = this.ds.Tables["门"];
            this.lookDoor.Properties.DisplayMember = "门编号";
            this.lookDoor.Properties.ValueMember = "门编号";
            this.lookDoor.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[] {            
                         new DevExpress.XtraEditors.Controls.LookUpColumnInfo("名称", "名称", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near),
              new DevExpress.XtraEditors.Controls.LookUpColumnInfo("门编号", "门编号", 100, 
                         DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Near)});
        }

        private void BtCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}