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
            this.xrOper.Text = basefun.valtag(tag, "����Ա");
            this.xrCardNo.Text = basefun.valtag(tag, "{����}");
            this.xrName.Text = basefun.valtag(tag, "{����}");
            this.xrNo.Text = basefun.valtag(tag, "{��ӡ���}");
            this.xrCarNo.Text = basefun.valtag(tag, "{���ƺ���}");
            this.xrInTime.Text = basefun.valtag(tag, "{���볡ʱ��}");
            this.xrOutTime.Text = basefun.valtag(tag, "{����ʱ��}");
            this.xrTime.Text = basefun.valtag(tag, "{ͣ��ʱ��}");
            this.xrMoney.Text = basefun.valtag(tag, "{�շѽ��}");
        }
    }
}
