using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
namespace Granity.granityMgr.Report
{
    public partial class ReportOpenRecord : DevExpress.XtraReports.UI.XtraReport
    {
        public ReportOpenRecord()
        {
            InitializeComponent();
        }
        public ReportOpenRecord( DataTable dt )
        {
            InitializeComponent();
            this.dataSetOpenDoorRecord1.Merge(dt);
        }
    }
}
