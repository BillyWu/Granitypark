using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;

namespace Granity.granityMgr.ParkMgr.Report
{
    public partial class PrintInfo : DevExpress.XtraReports.UI.XtraReport
    {
        public PrintInfo()
        {
            InitializeComponent();
        }

        public PrintInfo(string tag)
        {
            InitializeComponent();
            GetInfo(tag);
        }

        private void GetInfo(string tag)
        {
            this.xrOper.Text = basefun.valtag(tag, "操作员");
            this.xrCardNo.Text = basefun.valtag(tag, "{卡号}");
            this.xrName.Text = basefun.valtag(tag, "{姓名}");
            this.xrNo.Text = basefun.valtag(tag, "{打印编号}");
            this.xrCarNo.Text = basefun.valtag(tag, "{车牌号码}");
            this.xrInTime.Text = basefun.valtag(tag, "{出入场时间}");
            this.xrOutTime.Text = basefun.valtag(tag, "{出场时间}");
            this.xrTime.Text = basefun.valtag(tag, "{停车时长}");
            this.xrMoney.Text = basefun.valtag(tag, "{收费金额}");
        }
    }
}
