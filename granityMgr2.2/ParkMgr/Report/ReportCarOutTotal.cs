using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;

namespace Granity.granityMgr.ParkMgr.Report
{
    public partial class ReportCarOutTotal : DevExpress.XtraReports.UI.XtraReport
    {
        public ReportCarOutTotal()
        {
            InitializeComponent();
        }
        public ReportCarOutTotal(DataSet ds)
        {
            InitializeComponent();
            //this.dataSet11.Merge(ds);
        }
    }
}
