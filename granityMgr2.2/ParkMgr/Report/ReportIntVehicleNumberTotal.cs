using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.XtraReports.UI;
using System.Data;
namespace Granity.granityMgr.ParkMgr.Report
{
    public partial class ReportIntVehicleNumberTotal : DevExpress.XtraReports.UI.XtraReport
    {
        public ReportIntVehicleNumberTotal()
        {
            InitializeComponent();
        }
        public ReportIntVehicleNumberTotal(DataSet ds)
        {
            InitializeComponent();
           // this.dataSetIntVehicleNumberTotal1.Merge(ds);
        }
    }
}
